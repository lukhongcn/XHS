using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModuleWorkFlow.Model;
using WorkFlow.BLL.Standard;
using WorkFlow.Model.Standard;

namespace XHS.BLL
{
    /// <summary>
    /// 根据标准工艺为零件生成零件工序。
    /// 该服务只负责整合现有标准工艺 DLL 和 XHS 原有 PartProcess 方法。
    /// </summary>
    public class PartProcessService
    {
        /// <summary>
        /// 按已生成的条码标签为零件生成全部标准工序。
        /// </summary>
        public int CreateByStandardForLabels(
            string moduleId,
            string partNo,
            IEnumerable<string> partNoIds,
            string standardProcessName,
            bool rejectWhenExists = true)
        {
            if (string.IsNullOrWhiteSpace(moduleId))
                throw new ArgumentException("模块编号不能为空", "moduleId");

            if (string.IsNullOrWhiteSpace(partNo))
                throw new ArgumentException("零件编号不能为空", "partNo");

            if (string.IsNullOrWhiteSpace(standardProcessName))
                throw new ArgumentException("标准工艺名称不能为空", "standardProcessName");

            List<string> labels = (partNoIds ?? Enumerable.Empty<string>())
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (labels.Count == 0)
                throw new ArgumentException("至少需要一个条码标签", "partNoIds");

            IList standardProcesses = new StandProcess()
                .GetStandProcessByName(standardProcessName);
            if (standardProcesses == null || standardProcesses.Count == 0)
                throw new InvalidOperationException("未找到标准工艺：" + standardProcessName);

            int createdCount = 0;
            foreach (StandProcessInfo standardInfo in standardProcesses)
            {
                foreach (string partNoId in labels)
                {
                    ModuleWorkFlow.BLL.PartProcess partProcess =
                        new ModuleWorkFlow.BLL.PartProcess();
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

                    if (rejectWhenExists &&
                        partProcess.getPartProcessInfo(
                            processInfo.ModuleId,
                            processInfo.PartNo_Id,
                            processInfo.ProcessOrder) != null)
                    {
                        throw new InvalidOperationException(
                            "零件工序已存在：" + partNoId + ", "
                            + standardInfo.ProcessId + ", 工序号 "
                            + standardInfo.ListOrder);
                    }

                    if (partProcess.AddPartProcess(processInfo) <= 0)
                    {
                        throw new InvalidOperationException(
                            "生成零件工序失败：" + partNoId + ", "
                            + standardInfo.ProcessId);
                    }

                    createdCount++;
                }
            }

            return createdCount;
        }

        /// <summary>
        /// 按标准工艺生成指定零件的全部工序。
        /// </summary>
        /// <param name="partInfo">已经生成的零件信息。</param>
        /// <param name="standardProcessName">标准工艺名称。</param>
        /// <param name="rejectWhenExists">发现相同零件工序时是否拒绝继续生成。</param>
        /// <returns>生成的工序数量。</returns>
        public int CreateByStandard(
            PartInfo partInfo,
            string standardProcessName,
            bool rejectWhenExists = true)
        {
            if (partInfo == null)
                throw new ArgumentNullException("partInfo");

            if (string.IsNullOrWhiteSpace(standardProcessName))
                throw new ArgumentException("标准工艺名称不能为空", "standardProcessName");

            if (partInfo.PartCount <= 0)
                throw new ArgumentException("零件数量必须大于零", "partInfo");

            StandProcess standProcess = new StandProcess();
            IList standardProcesses =
                standProcess.GetStandProcessByName(standardProcessName);

            if (standardProcesses == null || standardProcesses.Count == 0)
                throw new InvalidOperationException(
                    "未找到标准工艺：" + standardProcessName);

            int createdCount = 0;

            foreach (StandProcessInfo standardInfo in standardProcesses)
            {
                for (int i = 1; i <= partInfo.PartCount; i++)
                {
                    ModuleWorkFlow.BLL.PartProcess partProcess =
                        new ModuleWorkFlow.BLL.PartProcess();
                    PartProcessInfo processInfo = new PartProcessInfo();
                    string partNoId = partInfo.PartNo + "-" + i;

                    processInfo.ModuleId = partInfo.ModuleId;
                    processInfo.PartNo = partInfo.PartNo;
                    processInfo.PartNo_Id = partNoId;
                    processInfo.ProcessId = standardInfo.ProcessId;
                    processInfo.ProcessOrder = standardInfo.ListOrder;
                    processInfo.ProcessNeedMinutes =
                        standardInfo.ProcessNeedMinutes;
                    processInfo.StatusId =
                        standardInfo.ListOrder == 1
                            ? "Ready"
                            : "Pending";
                    processInfo.NeedProduct = 0;
                    processInfo.Priority =
                        partInfo.Priority == 0 ? 100 : partInfo.Priority;

                    if (rejectWhenExists &&
                        partProcess.getPartProcessInfo(
                            processInfo.ModuleId,
                            processInfo.PartNo_Id,
                            processInfo.ProcessOrder) != null)
                    {
                        throw new InvalidOperationException(
                            "零件工序已存在：" + partNoId + ", " +
                            standardInfo.ProcessId + ", 工序号 " +
                            standardInfo.ListOrder);
                    }

                    if (partProcess.AddPartProcess(processInfo) <= 0)
                    {
                        throw new InvalidOperationException(
                            "生成零件工序失败：" + partNoId + ", " +
                            standardInfo.ProcessId);
                    }

                    createdCount++;
                }
            }

            return createdCount;
        }
    }
}
