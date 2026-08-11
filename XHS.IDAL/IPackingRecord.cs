using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 装箱记录数据访问接口。
    /// </summary>
    public interface IPackingRecord
    {
        List<PackingRecordInfo> GetExPackingRecords(string supplyBatchNo, string partNo);

        /// <summary>按 KD 码、零件编号和零件码查询装箱记录。</summary>
        List<PackingRecordInfo> SearchPackingRecords(string kdQRCode, string partNo, string partCode);

        ParamterInfo InsertPackingRecord(List<PackingRecordInfo> packingRecordInfos);

        ParamterInfo UpdatePackingRecord(List<PackingRecordInfo> packingRecordInfos);

        /// <summary>在事务内以 UPDLOCK 读取装箱记录。</summary>
        PackingRecordInfo GetPackingRecordForUpdate(long packingId, SqlConnection connection, SqlTransaction transaction);

        /// <summary>在事务内查询装箱记录（共享锁，用于查询状态）。</summary>
        PackingRecordInfo GetPackingRecordById(long packingId, SqlConnection connection, SqlTransaction transaction);

        /// <summary>按 TaskId 查询装箱记录。</summary>
        PackingRecordInfo GetPackingRecordByTaskId(Guid taskId, SqlConnection connection, SqlTransaction transaction);

        /// <summary>按 KD 二维码查询装箱记录。</summary>
        PackingRecordInfo GetPackingRecordByKdQRCode(string kdQRCode, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内插入装箱记录，返回新 Id。</summary>
        long InsertPackingRecord(PackingRecordInfo info, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内更新装箱扫描数量和新 Token。</summary>
        bool UpdatePackingRecordScan(long packingId, string pageToken, int newPackingQty, string newToken, string userName, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内锁定装箱记录。</summary>
        bool LockPackingRecord(long packingId, string pageToken, string newToken, string userName, string machineId, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内为重新装箱审核锁定记录，允许当前记录已经完成但不允许已上传。</summary>
        bool LockPackingRecordForRepack(long packingId, string pageToken, string newToken, string userName, string machineId, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内完成装箱。</summary>
        bool CompletePackingRecord(long packingId, string pageToken, string newToken, string userName, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内解锁装箱记录（审核通过）。</summary>
        bool UnlockPackingRecord(long packingId, string exceptionLockToken, string newToken, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内审核通过重新装箱申请后重新开放记录。</summary>
        bool ReopenPackingRecordForRepack(long packingId, string exceptionLockToken, string newToken, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内更新装箱随箱码。</summary>
        bool UpdatePackingQRCode(long packingId, string pageToken, string packingQRCode, string newToken, string userName, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内更新装箱阶段。</summary>
        bool UpdatePackingStage(long packingId, string pageToken, string packingStage, string newToken, string userName, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内将已完成的装箱记录更新为已上传。</summary>
        bool MarkPackingRecordUploaded(long packingId, string userName, SqlConnection connection, SqlTransaction transaction);

        /// <summary>同步更新 tb_ShippingGoods.PackingStage。</summary>
        bool UpdateShippingGoodsPackingStage(string supplyBatchNo, string partNo, string cartonNo, string packingStage, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内将出货货品状态及装箱阶段更新为已上传。</summary>
        bool MarkShippingGoodsUploaded(string supplyBatchNo, string partNo, string cartonNo, SqlConnection connection, SqlTransaction transaction);
    }
}
