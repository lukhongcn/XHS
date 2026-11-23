using System;
using System.Collections.Generic;
using ModuleWorkFlow.Model;
using XHS.Model;

namespace XHS.BLL
{
    /// <summary>
    /// 处理未处理的 PLC 检测结果，并推进零件工序状态。
    /// </summary>
    public class PLCResultProcessService
    {
        private const string ReadyStatus = "Ready";
        private const string ImplementationStatus = "Implementation";
        private const string RepairProcessId = "QMJCPD";
        private readonly PLCResult plcResult = new PLCResult();

        public string ProcessLatestUndealt()
        {
            PLCResultInfo result = plcResult.GetLatestUndealt();
            if (result == null) return null;
            if (!result.Id.HasValue || !result.ProcessNo.HasValue)
                return "PLCResult 缺少 Id 或 ProcessNo。";
            if (string.IsNullOrWhiteSpace(result.ModuleId) || string.IsNullOrWhiteSpace(result.QRCode))
                return "PLCResult 缺少 ModuleId 或二维码。";

            // PLCResult.ProcessNo 对应的记录已经带出 PartNo_Id/二维码；再只按
            // PartNo_Id 读取该零件的全部工序，不使用模块号参与筛选。
            XHS.MSSQL.XHSPartPartProcess processReader = new XHS.MSSQL.XHSPartPartProcess();
            List<XHS.Model.XHSPartPartProcessInfo> xhsProcesses =
                processReader.GetPartProcessesByPartNoId(result.QRCode) ??
                new List<XHS.Model.XHSPartPartProcessInfo>();
            List<PartPartProcessInfo> processes = xhsProcesses.ConvertAll(ToModuleProcessInfo);
            PartPartProcessInfo current = processes.Find(item => item.ProcessNo == result.ProcessNo.Value);
            if (current == null)
                return "未找到 PLCResult 对应的工序：" + result.ProcessNo.Value;

            if (string.Equals(current.StatusId, ReadyStatus, StringComparison.OrdinalIgnoreCase))
            {
                List<PartPartProcessInfo> source = new List<PartPartProcessInfo> { current };
                current.StatusId = ImplementationStatus;

                PartPartProcessInfo next = processes.Find(item =>
                    item.ProcessOrder == current.ProcessOrder + 1);

                if (result.Result == 1)
                {
                    if (next != null)
                    {
                        next.StatusId = ReadyStatus;
                        source.Add(next);
                    }
                }
                else
                {
                    int repairOrder = current.ProcessOrder + 1;
                    if (next != null)
                    {
                        repairOrder = next.ProcessOrder;
                        next.ProcessOrder = repairOrder + 1;
                        source.Add(next);
                    }

                    PartPartProcessInfo repair = CloneProcess(next ?? current);
                    repair.ProcessNo = 0;
                    repair.ProcessId = RepairProcessId;
                    repair.ProcessOrder = repairOrder;
                    repair.StatusId = ReadyStatus;
                    source.Add(repair);
                }

                int affectedRows = ModuleWorkFlow.business.PartProcess.UpdatePartProcessFromModifyEdit(source);
                if (affectedRows <= 0)
                    return "PLCResult 对应工序更新失败：" + result.ProcessNo.Value;
            }

            if (!plcResult.MarkDeal(result.Id.Value))
                return "PLCResult deal 更新失败：" + result.Id.Value;

            return "PLCResult 工序更新成功，deal=1，Id=" + result.Id.Value;
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
                Ispcnc = source.Ispcnc
            };
        }

        private static PartPartProcessInfo ToModuleProcessInfo(XHS.Model.XHSPartPartProcessInfo source)
        {
            return new PartPartProcessInfo
            {
                ModuleId = source.ModuleId,
                PartNo = source.PartNo,
                PartNo_Id = source.PartNo_Id,
                ProcessNo = source.ProcessNo ?? 0,
                ProcessId = source.ProcessId,
                ProcessName = source.ProcessName,
                ProcessOrder = source.ProcessOrder ?? 0,
                CustomerProcessId = source.CustomerProcessId,
                StatusId = source.StatusId,
                ProcessNeedMinutes = source.ProcessNeedMinutes ?? 0,
                ProcessComment = source.ProcessComment,
                ProcessMachineId = source.ProcessMachineId,
                RelationPartNo = source.RelationPartNo,
                RelationPartNoId = source.RelationPartNoId,
                StartDate = source.StartDate ?? DateTime.MinValue,
                EndDate = source.EndDate ?? DateTime.MinValue,
                FactStartDate = source.FactStartDate ?? DateTime.MinValue,
                FactEndDate = source.FactEndDate ?? DateTime.MinValue
            };
        }
    }
}
