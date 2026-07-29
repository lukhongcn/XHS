using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using BLL;
using Utility;
using XHS.IDAL;
using XHS.Model;

namespace XHS.BLL
{
    /// <summary>
    /// 装箱操作事务服务。所有装箱数据的修改操作都必须在同一事务中完成。
    /// </summary>
    public class PackingOperationService
    {
        private readonly IPackingRecord packingRecordDal;
        private readonly IPackingScanRecord packingScanRecordDal;
        private readonly IPackingException packingExceptionDal;
        private readonly string connectionString;

        public PackingOperationService()
        {
            this.packingRecordDal = XHS.DALFactory.PackingRecord.Create();
            this.packingScanRecordDal = XHS.DALFactory.PackingScanRecord.Create();
            this.packingExceptionDal = XHS.DALFactory.PackingException.Create();
            this.connectionString = ConfigurationSettings.AppSettings["MsSQLConnString"];
        }

        #region 公共方法

        /// <summary>按 KD 查找已有任务或创建新任务。</summary>
        public PackingOperationResult OpenOrCreateByKd(
            string supplyBatchNo,
            string partNo,
            string cartonNo,
            int planQty,
            string kdQRCode,
            string userName)
        {
            return ScanKDQRCode(supplyBatchNo, partNo, cartonNo, planQty, kdQRCode, userName);
        }

        /// <summary>扫描 KD 标签：按 KD 查找已有任务，否则在事务中创建新任务。</summary>
        public PackingOperationResult ScanKDQRCode(
            string supplyBatchNo,
            string partNo,
            string cartonNo,
            int planQty,
            string kdQRCode,
            string userName)
        {
            if (string.IsNullOrWhiteSpace(kdQRCode))
            {
                return PackingOperationResult.Error("KD 标签不能为空。");
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // KD 是查找或建立装箱任务的唯一入口。
                            PackingRecordInfo existingRecord = packingRecordDal.GetPackingRecordByKdQRCode(kdQRCode.Trim(), connection, transaction);
                            if (existingRecord != null)
                            {
                                transaction.Commit();
                                return BuildOpenExistingResult(existingRecord);
                            }

                            if (string.IsNullOrWhiteSpace(supplyBatchNo) || string.IsNullOrWhiteSpace(partNo) || string.IsNullOrWhiteSpace(cartonNo))
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("请填写供货批次号、零件编号和箱号。");
                            }

                            if (planQty <= 0)
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("计划数量必须为大于 0 的整数。");
                            }

                            string newToken = GenerateToken();
                            PackingRecordInfo record = new PackingRecordInfo
                            {
                                SupplyBatchNo = supplyBatchNo.Trim(),
                                PartNo = partNo.Trim(),
                                CartonNo = cartonNo.Trim(),
                                KDQRCode = kdQRCode.Trim(),
                                TaskId = Guid.NewGuid(),
                                Status = 0,
                                PlanQty = planQty,
                                PackingQty = 0,
                                ExceptionStatus = 0,
                                PackingStage = PackingStageInfo.装箱中.Status,
                                LockToken = newToken,
                                CreateUser = userName,
                                CreateTime = DateTime.Now,
                                UpdateUser = userName,
                                UpdateTime = DateTime.Now
                            };

                            long newId = packingRecordDal.InsertPackingRecord(record, connection, transaction);

                            // 插入 KD 扫描记录
                            packingScanRecordDal.InsertScanRecord(newId, PackingScanRecordQRCodeTypeInfo.KD, kdQRCode, string.Empty, 0, userName, connection, transaction);

                            transaction.Commit();

                            // 提交后重新读取完整记录
                            record.Id = newId;
                            record = LoadPackingRecord(newId);

                            return PackingOperationResult.Ok("KD 标签扫描成功，已创建装箱任务。", newToken, record);
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    PackingRecordInfo existingRecord = LoadPackingRecordByKdQRCode(kdQRCode.Trim());
                    if (existingRecord != null)
                    {
                        return BuildOpenExistingResult(existingRecord);
                    }
                }
                Log.WriteLog("PackingOperationService.log", "ScanKDQRCode error: " + ex.Message + "\r\n" + ex.StackTrace);
                return PackingOperationResult.Error("创建装箱任务失败：" + ex.Message);
            }
            catch (Exception ex)
            {
                Log.WriteLog("PackingOperationService.log", "ScanKDQRCode error: " + ex.Message + "\r\n" + ex.StackTrace);
                return PackingOperationResult.Error("创建装箱任务失败：" + ex.Message);
            }
        }

        /// <summary>扫描随箱码（事务内完成）。</summary>
        public PackingOperationResult ScanPackingQRCode(
            long packingId,
            string pageToken,
            string packingQRCode,
            string userName)
        {
            if (string.IsNullOrWhiteSpace(packingQRCode))
            {
                return PackingOperationResult.Error("随箱码不能为空。");
            }

            string newToken = GenerateToken();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. 以 UPDLOCK 读取装箱记录
                            PackingRecordInfo record = packingRecordDal.GetPackingRecordForUpdate(packingId, connection, transaction);
                            if (record == null)
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("装箱任务不存在。");
                            }

                            // 2. 检查 Token
                            if (!string.Equals(record.LockToken, pageToken, StringComparison.Ordinal))
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Stale("装箱令牌已变更，请重新加载页面。");
                            }

                            // 3. 检查状态
                            if (record.Status == 1)
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("装箱已完成，不能继续操作。");
                            }

                            if (record.ExceptionStatus.HasValue && record.ExceptionStatus.Value != 0)
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Lock("装箱已异常锁定，等待主管审核。", record.LockToken, record);
                            }

                            // 4. 检查是否已有随箱码
                            if (!string.IsNullOrWhiteSpace(record.PackingQRCode))
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("该箱已扫描随箱码，不能重复扫描。");
                            }

                            // 5. 检查随箱码是否被其他装箱任务使用
                            if (packingScanRecordDal.CheckPackingQRCodeUsedByOther(packingId, packingQRCode, connection, transaction))
                            {
                                // 随箱码重复 -> 异常锁定
                                PackingOperationResult lockResult = LockForExceptionInternal(packingId, pageToken, "BOX001", "随箱码已被其他箱使用", packingQRCode, userName, connection, transaction);
                                if (lockResult.Status == "LOCK")
                                {
                                    transaction.Commit();
                                    lockResult.PackingRecord = LoadPackingRecord(packingId);
                                }
                                else
                                {
                                    transaction.Rollback();
                                }
                                return lockResult;
                            }

                            // 6. 更新随箱码
                            if (!packingRecordDal.UpdatePackingQRCode(packingId, pageToken, packingQRCode, newToken, userName, connection, transaction))
                            {
                                // 更新失败（可能并发修改）-> 重新检查
                                transaction.Rollback();
                                record = LoadPackingRecord(packingId);
                                if (record != null && !string.Equals(record.LockToken, pageToken, StringComparison.Ordinal))
                                {
                                    return PackingOperationResult.Stale("装箱令牌已变更，请重新加载页面。");
                                }
                                return PackingOperationResult.Error("更新随箱码失败，请重试。");
                            }

                            // 7. 插入随箱码扫描记录
                            packingScanRecordDal.InsertScanRecord(packingId, PackingScanRecordQRCodeTypeInfo.Packing, packingQRCode, string.Empty, 0, userName, connection, transaction);

                            transaction.Commit();

                            // 重新读取完整记录
                            record = LoadPackingRecord(packingId);
                            return PackingOperationResult.Ok("随箱码扫描成功。", newToken, record);
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("PackingOperationService.log", "ScanPackingQRCode error: " + ex.Message + "\r\n" + ex.StackTrace);
                return PackingOperationResult.Error("随箱码扫描失败：" + ex.Message);
            }
        }

        /// <summary>扫描物料标签（事务内完成）。</summary>
        public PackingOperationResult ScanMaterialQRCode(
            long packingId,
            string pageToken,
            string qrCode,
            string materialNo,
            int qty,
            string userName)
        {
            if (string.IsNullOrWhiteSpace(qrCode))
            {
                return PackingOperationResult.Error("二维码不能为空。");
            }

            if (qty <= 0)
            {
                // 普通输入错误，不锁箱
                return PackingOperationResult.Error("本次数量必须为大于 0 的整数。");
            }

            if (string.IsNullOrWhiteSpace(materialNo))
            {
                return PackingOperationResult.Error("请填写物料号后再扫描零件标签。");
            }

            // 使用 LabelCodeRule 正则解析零件标签二维码。
            ShippingGoodsInfo parsedMaterial = new FactoryBarcodeParser().ParseShippingGoodsBarcode(qrCode, "XHSFZPart");
            string effectiveMaterialNo = string.IsNullOrWhiteSpace(parsedMaterial.PartNo)
                ? materialNo.Trim()
                : parsedMaterial.PartNo.Trim();
            // 零件标签每成功绑定一次只计 1 件，装箱数量等于零件标签绑定次数。
            int effectiveQty = qty;

            string newToken = GenerateToken();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. 以 UPDLOCK 读取装箱记录
                            PackingRecordInfo record = packingRecordDal.GetPackingRecordForUpdate(packingId, connection, transaction);
                            if (record == null)
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("装箱任务不存在。");
                            }

                            // 2. 检查 Token
                            if (!string.Equals(record.LockToken, pageToken, StringComparison.Ordinal))
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Stale("装箱令牌已变更，请重新加载页面。");
                            }

                            // 3. 检查状态
                            if (record.Status == 1)
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("装箱已完成，不能继续扫描。");
                            }

                            if (record.ExceptionStatus.HasValue && record.ExceptionStatus.Value != 0)
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Lock("装箱已异常锁定，等待主管审核。", record.LockToken, record);
                            }

                            // 4. 检查二维码是否重复
                            if (packingScanRecordDal.CheckDuplicateQRCode(packingId, qrCode, connection, transaction))
                            {
                                PackingOperationResult lockResult = LockForExceptionInternal(packingId, pageToken, "LABEL001", "零件标签重复扫描", qrCode, userName, connection, transaction);
                                if (lockResult.Status == "LOCK")
                                {
                                    transaction.Commit();
                                    lockResult.PackingRecord = LoadPackingRecord(packingId);
                                }
                                else
                                {
                                    transaction.Rollback();
                                }
                                return lockResult;
                            }

                            // 6. 检查物料是否匹配
                            if (!string.Equals(record.PartNo, effectiveMaterialNo, StringComparison.OrdinalIgnoreCase))
                            {
                                string partMismatchMsg = string.Format("KD标签的零件是：{0}，零件标签是：{1}，零件不匹配。", record.PartNo ?? string.Empty, effectiveMaterialNo ?? string.Empty);
                                PackingOperationResult lockResult = LockForExceptionInternal(packingId, pageToken, "PART001", partMismatchMsg, qrCode, userName, connection, transaction);
                                if (lockResult.Status == "LOCK")
                                {
                                    transaction.Commit();
                                    lockResult.PackingRecord = LoadPackingRecord(packingId);
                                }
                                else
                                {
                                    transaction.Rollback();
                                }
                                return lockResult;
                            }

                            // 7. 汇总已扫描数量
                            int currentQty = packingScanRecordDal.GetScannedMaterialQty(packingId, connection, transaction);

                            // 8. 检查是否超过计划数量
                            if (currentQty + effectiveQty > (record.PlanQty ?? 0))
                            {
                                PackingOperationResult lockResult = LockForExceptionInternal(packingId, pageToken, "QTY001", "扫描数量超过计划数量", qrCode, userName, connection, transaction);
                                if (lockResult.Status == "LOCK")
                                {
                                    transaction.Commit();
                                    lockResult.PackingRecord = LoadPackingRecord(packingId);
                                }
                                else
                                {
                                    transaction.Rollback();
                                }
                                return lockResult;
                            }

                            // 9. 插入扫描明细
                            if (!packingScanRecordDal.InsertScanRecord(packingId, PackingScanRecordQRCodeTypeInfo.MaterialLabel, qrCode, effectiveMaterialNo, effectiveQty, userName, connection, transaction))
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("插入扫描明细失败。");
                            }

                            // 10. 更新装箱数量和 Token
                            int newQty = currentQty + effectiveQty;
                            if (!packingRecordDal.UpdatePackingRecordScan(packingId, pageToken, newQty, newToken, userName, connection, transaction))
                            {
                                transaction.Rollback();
                                // 可能并发修改
                                record = LoadPackingRecord(packingId);
                                if (record != null && !string.Equals(record.LockToken, pageToken, StringComparison.Ordinal))
                                {
                                    return PackingOperationResult.Stale("装箱令牌已变更，请重新加载页面。");
                                }
                                return PackingOperationResult.Error("更新装箱数量失败，请重试。");
                            }

                            transaction.Commit();

                            record = LoadPackingRecord(packingId);
                            return PackingOperationResult.Ok("零件标签扫描成功。", newToken, record);
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("PackingOperationService.log", "ScanMaterialQRCode error: " + ex.Message + "\r\n" + ex.StackTrace);
                return PackingOperationResult.Error("零件扫描失败：" + ex.Message);
            }
        }

        /// <summary>扫描配送单：解码 → 核对零件/批号/数量 → 匹配则完成，不匹配则异常锁定（事务内完成）。</summary>
        public PackingOperationResult ScanDeliveryNote(
            long packingId,
            string pageToken,
            string qrCode,
            string userName)
        {
            if (string.IsNullOrWhiteSpace(qrCode))
                return PackingOperationResult.Error("配送单二维码不能为空。");

            ShippingGoodsInfo parsedInfo = new FactoryBarcodeParser()
                .ParseShippingGoodsBarcode(qrCode, "XHSFZPS");
            if (parsedInfo == null ||
                string.IsNullOrWhiteSpace(parsedInfo.PartNo) ||
                string.IsNullOrWhiteSpace(parsedInfo.SupplyBatchNo) ||
                !parsedInfo.Quantity.HasValue || parsedInfo.Quantity.Value <= 0)
            {
                return PackingOperationResult.Error("配送单解析失败，缺少零件号、供货批次号或数量。");
            }

            string newToken = GenerateToken();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. UPDLOCK 读取装箱记录
                            PackingRecordInfo record = packingRecordDal.GetPackingRecordForUpdate(packingId, connection, transaction);
                            if (record == null)
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("装箱任务不存在。");
                            }

                            // 2. 验证 Token
                            if (!string.Equals(record.LockToken, pageToken, StringComparison.Ordinal))
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Stale("装箱令牌已变更，请重新加载页面。");
                            }

                            // 3. 检查状态
                            if (record.Status == 1)
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("装箱已完成，不能继续操作。");
                            }

                            if (record.ExceptionStatus.HasValue && record.ExceptionStatus.Value != 0)
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Lock("装箱已异常锁定。", record.LockToken, record);
                            }

                            // 4. 核对零件编号
                            if (!string.Equals(record.PartNo, parsedInfo.PartNo, StringComparison.OrdinalIgnoreCase))
                            {
                                string msg = string.Format("KD标签的零件是：{0}，配送单是：{1}，零件不匹配。", record.PartNo ?? string.Empty, parsedInfo.PartNo ?? string.Empty);
                                PackingOperationResult lockResult = LockForExceptionInternal(packingId, pageToken, "PART001", msg, qrCode, userName, connection, transaction);
                                if (lockResult.Status == "LOCK")
                                {
                                    transaction.Commit();
                                    lockResult.PackingRecord = LoadPackingRecord(packingId);
                                }
                                else { transaction.Rollback(); }
                                return lockResult;
                            }

                            // 5. 核对供货批次号
                            if (!string.Equals(record.SupplyBatchNo, parsedInfo.SupplyBatchNo, StringComparison.OrdinalIgnoreCase))
                            {
                                string msg = string.Format("KD标签的批号是：{0}，配送单是：{1}，批号不匹配。", record.SupplyBatchNo ?? string.Empty, parsedInfo.SupplyBatchNo ?? string.Empty);
                                PackingOperationResult lockResult = LockForExceptionInternal(packingId, pageToken, "PART001", msg, qrCode, userName, connection, transaction);
                                if (lockResult.Status == "LOCK")
                                {
                                    transaction.Commit();
                                    lockResult.PackingRecord = LoadPackingRecord(packingId);
                                }
                                else { transaction.Rollback(); }
                                return lockResult;
                            }

                            // 6. 核对数量
                            int deliveryQty = parsedInfo.Quantity.Value;
                            if (deliveryQty != (record.PlanQty ?? 0))
                            {
                                string msg = string.Format("KD标签的计划数量是：{0}，配送单数量是：{1}，数量不匹配。", record.PlanQty ?? 0, deliveryQty);
                                PackingOperationResult lockResult = LockForExceptionInternal(packingId, pageToken, "QTY001", msg, qrCode, userName, connection, transaction);
                                if (lockResult.Status == "LOCK")
                                {
                                    transaction.Commit();
                                    lockResult.PackingRecord = LoadPackingRecord(packingId);
                                }
                                else { transaction.Rollback(); }
                                return lockResult;
                            }

                            // 7. 插入配送单扫描记录
                            packingScanRecordDal.InsertScanRecord(packingId, PackingScanRecordQRCodeTypeInfo.Packing, qrCode, string.Empty, 0, userName, connection, transaction);

                            // 8. 完成装箱
                            if (!packingRecordDal.CompletePackingRecord(packingId, pageToken, newToken, userName, connection, transaction))
                            {
                                transaction.Rollback();
                                record = LoadPackingRecord(packingId);
                                if (record != null && !string.Equals(record.LockToken, pageToken, StringComparison.Ordinal))
                                    return PackingOperationResult.Stale("装箱令牌已变更，请重新加载页面。");
                                return PackingOperationResult.Error("完成装箱失败，请重试。");
                            }

                            transaction.Commit();
                            record = LoadPackingRecord(packingId);
                            return PackingOperationResult.Completed("配送单核验通过，装箱已完成。", newToken, record);
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("PackingOperationService.log", "ScanDeliveryNote error: " + ex.Message + "\r\n" + ex.StackTrace);
                return PackingOperationResult.Error("配送单扫描失败：" + ex.Message);
            }
        }

        /// <summary>更新装箱阶段（事务内完成）。</summary>
        public PackingOperationResult UpdatePackingStage(
            long packingId,
            string pageToken,
            string packingStage,
            string userName)
        {
            string newToken = GenerateToken();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            PackingRecordInfo record = packingRecordDal.GetPackingRecordForUpdate(packingId, connection, transaction);
                            if (record == null)
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("装箱任务不存在。");
                            }

                            if (!string.Equals(record.LockToken, pageToken, StringComparison.Ordinal))
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Stale("装箱令牌已变更，请重新加载页面。");
                            }

                            if (record.Status == 1)
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("装箱已完成，不能更新阶段。");
                            }

                            if (record.ExceptionStatus.HasValue && record.ExceptionStatus.Value != 0)
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Lock("装箱已异常锁定。", record.LockToken, record);
                            }

                            if (!packingRecordDal.UpdatePackingStage(packingId, pageToken, packingStage, newToken, userName, connection, transaction))
                            {
                                transaction.Rollback();
                                record = LoadPackingRecord(packingId);
                                if (record != null && !string.Equals(record.LockToken, pageToken, StringComparison.Ordinal))
                                {
                                    return PackingOperationResult.Stale("装箱令牌已变更，请重新加载页面。");
                                }
                                return PackingOperationResult.Error("更新装箱阶段失败，请重试。");
                            }

                            transaction.Commit();

                            record = LoadPackingRecord(packingId);
                            return PackingOperationResult.Ok("装箱阶段已更新为" + PackingStageInfo.GetStatusName(packingStage) + "。", newToken, record);
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("PackingOperationService.log", "UpdatePackingStage error: " + ex.Message + "\r\n" + ex.StackTrace);
                return PackingOperationResult.Error("更新装箱阶段失败：" + ex.Message);
            }
        }

        /// <summary>异常锁定（事务内完成）。可由其他方法内部调用或页面直接调用。</summary>
        public PackingOperationResult LockForException(
            long packingId,
            string pageToken,
            string exceptionCode,
            string exceptionMessage,
            string triggerQRCode,
            string userName,
            string machineId)
        {
            string newToken = GenerateToken();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            PackingOperationResult result = LockForExceptionInternal(
                                packingId, pageToken, exceptionCode, exceptionMessage,
                                triggerQRCode, userName, machineId, connection, transaction);

                            if (result.Success || result.Status == "LOCK")
                            {
                                transaction.Commit();
                                if (result.PackingRecord == null)
                                {
                                    result.PackingRecord = LoadPackingRecord(packingId);
                                }
                            }
                            else
                            {
                                transaction.Rollback();
                            }

                            return result;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("PackingOperationService.log", "LockForException error: " + ex.Message + "\r\n" + ex.StackTrace);
                return PackingOperationResult.Error("异常锁定失败：" + ex.Message);
            }
        }

        /// <summary>审核解除（事务内完成）。</summary>
        public PackingOperationResult ReviewException(
            long packingId,
            long exceptionId,
            string auditUser,
            string auditRemark,
            int newStatus)
        {
            if (newStatus < 0 || newStatus > 2)
            {
                return PackingOperationResult.Error("异常审核状态无效。");
            }

            string newToken = GenerateToken();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            PackingExceptionInfo exception = packingExceptionDal.GetPendingExceptionByPackingId(packingId, connection, transaction);
                            if (exception == null || !exception.Id.HasValue || exception.Id.Value != exceptionId)
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("异常记录不存在、已处理或与装箱任务不匹配。");
                            }

                            PackingRecordInfo record = packingRecordDal.GetPackingRecordForUpdate(packingId, connection, transaction);
                            if (record == null)
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("装箱任务不存在。");
                            }

                            if (!string.Equals(record.LockToken, exception.LockToken, StringComparison.Ordinal))
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Stale("装箱令牌已变更，请重新加载页面。");
                            }

                            if (!packingExceptionDal.AuditException(exceptionId, auditUser, auditRemark, newStatus, connection, transaction))
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("更新异常审核状态失败。");
                            }

                            if (newStatus == 1)
                            {
                                if (!packingRecordDal.UnlockPackingRecord(packingId, exception.LockToken, newToken, connection, transaction))
                                {
                                    transaction.Rollback();
                                    return PackingOperationResult.Error("解除装箱锁定失败，可能已被其他操作修改。");
                                }

                                transaction.Commit();
                                record = LoadPackingRecord(packingId);
                                return PackingOperationResult.Ok("审核通过，装箱已解除锁定。", newToken, record);
                            }

                            transaction.Commit();
                            record = LoadPackingRecord(packingId);
                            string statusText = newStatus == 0 ? "暂停" : "取消";
                            return PackingOperationResult.Lock("异常审核已标记为" + statusText + "，装箱继续保持锁定。", record == null ? exception.LockToken : record.LockToken, record);
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("PackingOperationService.log", "ReviewException error: " + ex.Message + "\r\n" + ex.StackTrace);
                return PackingOperationResult.Error("异常审核失败：" + ex.Message);
            }
        }

        public PackingOperationResult ReviewAndUnlock(
            long packingId,
            string pageToken,
            string auditUser,
            string auditRemark)
        {
            string newToken = GenerateToken();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. 查询待审核异常
                            PackingExceptionInfo exception = packingExceptionDal.GetPendingException(packingId, connection, transaction);
                            if (exception == null)
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("未找到待审核的异常记录。");
                            }

                            // 2. 确认异常仍然是待审核
                            if (exception.Status != 0)
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("异常记录已审核，不能重复操作。");
                            }

                            // 3. 以 UPDLOCK 读取装箱记录
                            PackingRecordInfo record = packingRecordDal.GetPackingRecordForUpdate(packingId, connection, transaction);
                            if (record == null)
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("装箱任务不存在。");
                            }

                            // 4. 确认装箱记录的 LockToken 等于异常记录的 LockToken
                            if (!string.Equals(record.LockToken, exception.LockToken, StringComparison.Ordinal))
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Stale("装箱令牌已变更，异常已过期，请重新加载页面。");
                            }

                            // 5. 更新异常记录为审核通过
                            if (!packingExceptionDal.AuditException(exception.Id ?? 0L, auditUser, auditRemark, 1, connection, transaction))
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("更新异常记录失败。");
                            }

                            // 6. 解锁装箱记录
                            if (!packingRecordDal.UnlockPackingRecord(packingId, exception.LockToken, newToken, connection, transaction))
                            {
                                transaction.Rollback();
                                return PackingOperationResult.Error("解锁装箱记录失败，可能已被其他操作修改。");
                            }

                            transaction.Commit();

                            record = LoadPackingRecord(packingId);
                            return PackingOperationResult.Ok("审核通过，装箱已解锁。", newToken, record);
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("PackingOperationService.log", "ReviewAndUnlock error: " + ex.Message + "\r\n" + ex.StackTrace);
                return PackingOperationResult.Error("审核解锁失败：" + ex.Message);
            }
        }

        /// <summary>获取装箱状态（只读，不使用事务）。</summary>
        public PackingOperationResult GetPackingState(long packingId)
        {
            try
            {
                PackingRecordInfo record = LoadPackingRecord(packingId);
                if (record == null)
                {
                    return PackingOperationResult.Error("装箱任务不存在。");
                }

                PackingExceptionInfo exception = LoadPendingException(record.Id ?? 0L);
                return BuildOpenExistingResult(record, exception);
            }
            catch (Exception ex)
            {
                Log.WriteLog("PackingOperationService.log", "GetPackingState error: " + ex.Message);
                return PackingOperationResult.Error("读取装箱状态失败：" + ex.Message);
            }
        }

        /// <summary>按 TaskId 获取装箱状态，供页面刷新和重新打开使用。</summary>
        public PackingOperationResult GetPackingState(Guid taskId)
        {
            try
            {
                PackingRecordInfo record = LoadPackingRecordByTaskId(taskId);
                if (record == null)
                {
                    return PackingOperationResult.Error("装箱任务不存在。");
                }

                PackingExceptionInfo exception = LoadPendingException(record.Id ?? 0L);
                return BuildOpenExistingResult(record, exception);
            }
            catch (Exception ex)
            {
                Log.WriteLog("PackingOperationService.log", "GetPackingState by TaskId error: " + ex.Message);
                return PackingOperationResult.Error("读取装箱状态失败：" + ex.Message);
            }
        }

        /// <summary>按 KD 二维码查询装箱记录（只读，不涉及事务）。</summary>
        public PackingOperationResult GetPackingStateByKdQRCode(string kdQRCode)
        {
            try
            {
                PackingRecordInfo record = LoadPackingRecordByKdQRCode(kdQRCode);
                if (record == null)
                {
                    return PackingOperationResult.Error("该 KD 标签尚未创建装箱任务。");
                }

                PackingExceptionInfo exception = LoadPendingException(record.Id ?? 0L);
                return BuildOpenExistingResult(record, exception);
            }
            catch (Exception ex)
            {
                Log.WriteLog("PackingOperationService.log", "GetPackingStateByKdQRCode error: " + ex.Message);
                return PackingOperationResult.Error("读取装箱状态失败：" + ex.Message);
            }
        }

        /// <summary>按当前装箱任务 Id 读取扫描明细。</summary>
        public List<PackingScanRecordInfo> GetScanRecordsByPackingId(long packingId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return packingScanRecordDal.GetScanRecordsByPackingId(packingId, connection, null);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("PackingOperationService.log", "GetScanRecordsByPackingId error: " + ex.Message);
                return new List<PackingScanRecordInfo>();
            }
        }

        #endregion

        #region 内部方法

        /// <summary>生成 Token。</summary>
        private static string GenerateToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>只读加载装箱记录（新建连接，不参与事务）。</summary>
        private PackingRecordInfo LoadPackingRecord(long packingId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return packingRecordDal.GetPackingRecordById(packingId, connection, null);
                }
            }
            catch
            {
                return null;
            }
        }

        private PackingRecordInfo LoadPackingRecordByTaskId(Guid taskId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return packingRecordDal.GetPackingRecordByTaskId(taskId, connection, null);
                }
            }
            catch
            {
                return null;
            }
        }

        private PackingRecordInfo LoadPackingRecordByKdQRCode(string kdQRCode)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return packingRecordDal.GetPackingRecordByKdQRCode(kdQRCode, connection, null);
                }
            }
            catch
            {
                return null;
            }
        }

        private PackingExceptionInfo LoadPendingException(long packingId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return packingExceptionDal.GetPendingExceptionByPackingId(packingId, connection, null);
                }
            }
            catch
            {
                return null;
            }
        }

        private static PackingOperationResult BuildOpenExistingResult(PackingRecordInfo record)
        {
            return BuildOpenExistingResult(record, null);
        }

        private static PackingOperationResult BuildOpenExistingResult(PackingRecordInfo record, PackingExceptionInfo exception)
        {
            if (record.ExceptionStatus.HasValue && record.ExceptionStatus.Value != 0)
            {
                string message = exception == null || string.IsNullOrWhiteSpace(exception.ExceptionMessage)
                    ? "装箱已异常锁定，等待主管审核。"
                    : exception.ExceptionMessage;
                return PackingOperationResult.Lock(message, record.LockToken, record);
            }

            if (record.Status.HasValue && record.Status.Value == 1)
            {
                return PackingOperationResult.Completed("装箱已完成，禁止继续扫描。", record.LockToken, record);
            }

            return PackingOperationResult.Ok("已读取该箱的装箱任务。", record.LockToken, record);
        }

        /// <summary>在已有事务中执行异常锁定逻辑。</summary>
        private PackingOperationResult LockForExceptionInternal(
            long packingId,
            string pageToken,
            string exceptionCode,
            string exceptionMessage,
            string triggerQRCode,
            string userName,
            string machineId,
            SqlConnection connection,
            SqlTransaction transaction)
        {
            // 1. 读取装箱记录（已在之前用 UPDLOCK 读取，此处再次检查）
            PackingRecordInfo record = packingRecordDal.GetPackingRecordForUpdate(packingId, connection, transaction);
            if (record == null)
            {
                return PackingOperationResult.Error("装箱任务不存在。");
            }

            // 2. 检查 Token（如果之前已检查过则会通过，防御性检查）
            if (string.IsNullOrWhiteSpace(pageToken) || !string.Equals(record.LockToken, pageToken, StringComparison.Ordinal))
            {
                return PackingOperationResult.Stale("装箱令牌已变更，操作被拒绝。");
            }

            if (record.Status.HasValue && record.Status.Value == 1)
            {
                return PackingOperationResult.Completed("装箱已完成，不能再建立异常。", record.LockToken, record);
            }

            // 3. 如果已经锁定，返回现有锁定状态
            if (record.ExceptionStatus.HasValue && record.ExceptionStatus.Value != 0)
            {
                return PackingOperationResult.Lock("装箱已异常锁定。", record.LockToken, record);
            }

            // 4. 检查是否已存在待审核异常（防御性检查，防止重复插入）
            PackingExceptionInfo existingException = packingExceptionDal.GetPendingException(packingId, connection, transaction);
            if (existingException != null)
            {
                return PackingOperationResult.Lock("装箱已有待审核异常。", existingException.LockToken, record);
            }

            // 5. 生成新 Token
            string newToken = GenerateToken();

            // 6. 更新装箱记录为锁定状态
            if (!packingRecordDal.LockPackingRecord(packingId, record.LockToken, newToken, userName, machineId, connection, transaction))
            {
                // 并发修改 -> STALE
                return PackingOperationResult.Stale("锁定装箱失败，令牌已变更。");
            }

            // 7. 插入异常记录
            PackingExceptionInfo exceptionInfo = new PackingExceptionInfo
            {
                PackingId = packingId,
                BoxCode = record.CartonNo,
                CartonNo = record.CartonNo,
                ExceptionCode = exceptionCode,
                ExceptionMessage = exceptionMessage,
                TriggerQRCode = triggerQRCode,
                LockToken = newToken,
                Status = 0,
                CreateUser = userName,
                CreateTime = DateTime.Now
            };

            if (!packingExceptionDal.InsertException(exceptionInfo, connection, transaction))
            {
                // 插入异常记录失败 -> 整个事务失败
                return PackingOperationResult.Error("写入异常记录失败。");
            }

            // 8. 更新 record 的 LockToken 供返回
            record.LockToken = newToken;
            record.ExceptionStatus = 1;
            record.LockTime = DateTime.Now;
            record.LockUser = userName;
            record.LockMachine = machineId;

            return PackingOperationResult.Lock(exceptionMessage, newToken, record);
        }

        /// <summary>在已有事务中执行异常锁定（使用 pageToken 而非 record.LockToken）。</summary>
        private PackingOperationResult LockForExceptionInternal(
            long packingId,
            string pageToken,
            string exceptionCode,
            string exceptionMessage,
            string triggerQRCode,
            string userName,
            SqlConnection connection,
            SqlTransaction transaction)
        {
            return LockForExceptionInternal(
                packingId, pageToken, exceptionCode, exceptionMessage,
                triggerQRCode, userName, null, connection, transaction);
        }

        #endregion
    }
}
