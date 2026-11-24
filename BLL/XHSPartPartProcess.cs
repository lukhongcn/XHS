using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ModuleWorkFlow.Model;
using Utility;
using XHS.IDAL;
using XHS.Model;
using WorkFlow.BLL.Standard;
using WorkFlow.Model.Standard;

namespace XHS.BLL
{
    /// <summary>
    /// 零件工序业务层。
    /// </summary>
    public class XHSPartPartProcess
    {
        private const string RecheckProcessId = "QMJC";
        private const string ReadyStatusId = "Ready";
        private const string ImplementationStatusId = "Implementation";
        private const string PendingStatusId = "Pending";

        private readonly IXHSPartPartProcess dal;

        public XHSPartPartProcess()
        {
            dal = XHS.DALFactory.XHSPartPartProcess.Create();
        }

        public List<XHSPartPartProcessInfo> GetPartProcesses(
            string customerAbbr,
            string partNo,
            string barcode,
            IList<string> qrDateCodes)
        {
            return dal.GetPartProcesses(customerAbbr, partNo, barcode, qrDateCodes);
        }

        public List<XHSPartPartProcessInfo> GetPartProcessesByPartNoId(string partNoId)
        {
            return dal.GetPartProcessesByPartNoId(partNoId);
        }

        public List<PartPartProcessInfo> GetPartProcessInfoListByPartNoId(string partNoId)
        {
            List<XHSPartPartProcessInfo> infos =
                GetPartProcessesByPartNoId(partNoId) ?? new List<XHSPartPartProcessInfo>();
            return infos.Where(item => item != null)
                .Select(ToPartProcessInfo)
                .ToList();
        }

        public List<PartPartProcessInfo> GetPartProcessInfoListFromProcessNo(
            string partNoId,
            int processNo)
        {
            List<PartPartProcessInfo> processes =
                GetPartProcessInfoListByPartNoId(NormalizeValue(partNoId));
            PartPartProcessInfo current = processes.FirstOrDefault(item =>
                item != null && item.ProcessNo == processNo);
            if (current == null)
            {
                throw new InvalidOperationException(
                    "未找到 ProcessNo " + processNo + " 对应的工序资料。");
            }

            return processes.Where(item =>
                    item != null && item.ProcessOrder >= current.ProcessOrder)
                .OrderBy(item => item.ProcessOrder)
                .ThenBy(item => item.ProcessNo)
                .ToList();
        }

        public ModuleWorkFlow.Model.ParamterInfo UpdatePartProcessFromModifyEdit(
            List<PartPartProcessInfo> source)
        {
            return dal.UpdatePartProcessFromModifyEdit(source);
        }

        public ModuleWorkFlow.Model.ParamterInfo updateDiscount(string partNoId)
        {
            return dal.updateDiscount(partNoId);
        }

        public ModuleWorkFlow.Model.ParamterInfo UpdatePartNoId(
            string partNoId,
            string replacementPartNoId)
        {
            return dal.UpdatePartNoId(partNoId, replacementPartNoId);
        }

        public List<XHSPartPartProcessInfo> GetPartProcessesByPartNoIdAndProcessIdAndStatusId(
            string partNoId,
            string processId,
            string statusId)
        {
            return dal.GetPartProcessesByPartNoIdAndProcessIdAndStatusId(partNoId, processId, statusId);
        }

        public XHSPartPartProcessInfo GetBarcodePartProcess(
            string partNoId,
            string processId,
            string statusId)
        {
            return dal.GetBarcodePartProcess(partNoId, processId, statusId);
        }

        public int CreateByStandardForLabels(
            string moduleId,
            string partNo,
            IEnumerable<string> partNoIds,
            string standardProcessName,
            bool rejectWhenExists = true)
        {
            ValidateCreateArguments(moduleId, partNo, standardProcessName);

            List<string> labels = (partNoIds ?? Enumerable.Empty<string>())
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (labels.Count == 0)
            {
                throw new ArgumentException("至少需要一个条码标签", "partNoIds");
            }

            IList standardProcesses = new StandProcess()
                .GetStandProcessByName(standardProcessName);
            if (standardProcesses == null || standardProcesses.Count == 0)
            {
                throw new InvalidOperationException("未找到标准工艺：" + standardProcessName);
            }

            int createdCount = 0;
            foreach (StandProcessInfo standardInfo in standardProcesses)
            {
                foreach (string partNoId in labels)
                {
                    PartProcessInfo processInfo = new PartProcessInfo
                    {
                        ModuleId = moduleId,
                        PartNo = partNo,
                        PartNo_Id = partNoId,
                        ProcessId = standardInfo.ProcessId,
                        ProcessOrder = standardInfo.ListOrder,
                        ProcessNeedMinutes = standardInfo.ProcessNeedMinutes,
                        StatusId = standardInfo.ListOrder == 1 ? "Ready" : "Pending",
                        NeedProduct = 0,
                        Priority = 100
                    };

                    EnsurePartProcessNotExists(processInfo, rejectWhenExists);

                    if (CreatePartProcess(processInfo) <= 0)
                    {
                        throw new InvalidOperationException(
                            "生成零件工序失败：" + partNoId + ", " + standardInfo.ProcessId);
                    }

                    createdCount++;
                }
            }

            return createdCount;
        }

        public int CreateByStandard(
            ModuleWorkFlow.Model.PartInfo partInfo,
            string standardProcessName,
            bool rejectWhenExists = true)
        {
            if (partInfo == null)
                throw new ArgumentNullException("partInfo");
            if (string.IsNullOrWhiteSpace(standardProcessName))
                throw new ArgumentException("标准工艺名称不能为空", "standardProcessName");
            if (partInfo.PartCount <= 0)
                throw new ArgumentException("零件数量必须大于零", "partInfo");

            IList standardProcesses = new StandProcess()
                .GetStandProcessByName(standardProcessName);
            if (standardProcesses == null || standardProcesses.Count == 0)
                throw new InvalidOperationException("未找到标准工艺：" + standardProcessName);

            int createdCount = 0;
            foreach (StandProcessInfo standardInfo in standardProcesses)
            {
                for (int i = 1; i <= partInfo.PartCount; i++)
                {
                    PartProcessInfo processInfo = new PartProcessInfo
                    {
                        ModuleId = partInfo.ModuleId,
                        PartNo = partInfo.PartNo,
                        PartNo_Id = partInfo.PartNo + "-" + i,
                        ProcessId = standardInfo.ProcessId,
                        ProcessOrder = standardInfo.ListOrder,
                        ProcessNeedMinutes = standardInfo.ProcessNeedMinutes,
                        StatusId = standardInfo.ListOrder == 1 ? "Ready" : "Pending",
                        NeedProduct = 0,
                        Priority = partInfo.Priority == 0 ? 100 : partInfo.Priority
                    };

                    EnsurePartProcessNotExists(processInfo, rejectWhenExists);
                    if (CreatePartProcess(processInfo) <= 0)
                    {
                        throw new InvalidOperationException(
                            "生成零件工序失败：" + processInfo.PartNo_Id + ", "
                            + standardInfo.ProcessId);
                    }

                    createdCount++;
                }
            }

            return createdCount;
        }

        private static void ValidateCreateArguments(
            string moduleId,
            string partNo,
            string standardProcessName)
        {
            if (string.IsNullOrWhiteSpace(moduleId))
                throw new ArgumentException("模块编号不能为空", "moduleId");
            if (string.IsNullOrWhiteSpace(partNo))
                throw new ArgumentException("零件编号不能为空", "partNo");
            if (string.IsNullOrWhiteSpace(standardProcessName))
                throw new ArgumentException("标准工艺名称不能为空", "standardProcessName");
        }

        private static void EnsurePartProcessNotExists(
            PartProcessInfo processInfo,
            bool rejectWhenExists)
        {
            if (!rejectWhenExists)
                return;

            string queryString =
                "select id from tb_partprocess "
                + "where moduleid=@moduleid and partno_id=@partno_id "
                + "and processorder=@processorder";
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = processInfo.ModuleId;
            pars[1] = new SqlParameter("@partno_id", SqlDbType.NVarChar, 50);
            pars[1].Value = processInfo.PartNo_Id;
            pars[2] = new SqlParameter("@processorder", SqlDbType.Int);
            pars[2].Value = processInfo.ProcessOrder;

            DataRow existing = Data.getDataRow(queryString, pars);
            if (existing != null)
            {
                throw new InvalidOperationException(
                    "零件工序已存在：" + processInfo.PartNo_Id + ", "
                    + processInfo.ProcessId + ", 工序号 " + processInfo.ProcessOrder);
            }
        }

        private static int CreatePartProcess(PartProcessInfo processInfo)
        {
            string queryString =
                "insert into tb_partprocess "
                + "(ModuleId,PartNo,ProcessId,PartNo_Id,StatusId,"
                + "ProcessNeedMinutes,ProcessOrder,priority) "
                + "values (@moduleid,@partno,@processid,@partno_id,@statusid,"
                + "@processneedminutes,@processorder,@priority); "
                + "declare @newPartProcessId int; "
                + "set @newPartProcessId=convert(int,scope_identity()); "
                + "update tb_partprocess set processno=@newPartProcessId "
                + "where id=@newPartProcessId; "
                + "select @@ROWCOUNT as AffectedRows";

            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = processInfo.ModuleId;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = processInfo.PartNo;
            pars[2] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
            pars[2].Value = processInfo.ProcessId;
            pars[3] = new SqlParameter("@partno_id", SqlDbType.NVarChar, 50);
            pars[3].Value = processInfo.PartNo_Id;
            pars[4] = new SqlParameter("@statusid", SqlDbType.NVarChar, 50);
            pars[4].Value = processInfo.StatusId;
            pars[5] = new SqlParameter("@processneedminutes", SqlDbType.Int);
            pars[5].Value = processInfo.ProcessNeedMinutes;
            pars[6] = new SqlParameter("@processorder", SqlDbType.Int);
            pars[6].Value = processInfo.ProcessOrder;
            pars[7] = new SqlParameter("@priority", SqlDbType.Int);
            pars[7].Value = processInfo.Priority;

            object result = Data.getDataScalar(queryString, pars);
            return result == null ? -1 : Convert.ToInt32(result);
        }

        private static void ApplyReplacementQRCode(
            PartPartProcessInfo process,
            string originalPartNoId,
            string replacementQRCode)
        {
            if (process == null || replacementQRCode.Length == 0)
                return;

            process.PartNo_Id = replacementQRCode;
            process.SameProcessNo = originalPartNoId + ",";
        }

        private static PartPartProcessInfo CloneProcess(PartPartProcessInfo source)
        {
            return new PartPartProcessInfo
            {
                ModuleId = source.ModuleId,
                PartNo = source.PartNo,
                PartNo_Id = source.PartNo_Id,
                ProcessId = source.ProcessId,
                ProcessNeedMinutes = source.ProcessNeedMinutes,
                Priority = source.Priority,
                StartDate = source.StartDate,
                SelfStartDate = source.SelfStartDate,
                SelfEndDate = source.SelfEndDate,
                isConfirm = source.isConfirm,
                ProcessComment = source.ProcessComment,
                ProcessMachineId = source.ProcessMachineId,
                IsUnnormal = source.IsUnnormal,
                UnNormalReasonsId = source.UnNormalReasonsId,
                DepartMentId = source.DepartMentId,
                NeedProduct = source.NeedProduct,
                CustomerProcessId = source.CustomerProcessId,
                RelationPartNo = source.RelationPartNo,
                RelationPartNoId = source.RelationPartNoId,
                SameProcessNo = source.SameProcessNo,
                Ispcnc = source.Ispcnc
            };
        }

        private static PartPartProcessInfo ToPartProcessInfo(XHSPartPartProcessInfo source)
        {
            return new PartPartProcessInfo
            {
                ModuleId = source.ModuleId,
                PartNo = source.PartNo,
                PartNo_Id = source.PartNo_Id,
                ProcessNo = source.ProcessNo ?? 0,
                ProcessId = source.ProcessId,
                ProcessNeedMinutes = source.ProcessNeedMinutes ?? 0,
                ProcessOrder = source.ProcessOrder ?? 0,
                StatusId = source.StatusId,
                ProcessComment = source.ProcessComment,
                ProcessMachineId = source.ProcessMachineId,
                IsUnnormal = source.IsUnnormal ?? 0,
                UnNormalReasonsId = source.UnNormalReasonsId ?? 0,
                DepartMentId = source.DepartmentId ?? 0,
                NeedProduct = source.NeedProduct ?? 0,
                CustomerProcessId = source.CustomerProcessId,
                RelationPartNo = source.RelationPartNo,
                RelationPartNoId = source.RelationPartNoId
            };
        }

        private static string NormalizeValue(string value)
        {
            return (value ?? string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();
        }
    }
}
