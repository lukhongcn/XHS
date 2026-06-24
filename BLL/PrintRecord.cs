using System.Collections;
using System.Collections.Generic;
using ModuleWorkFlow.business;
using XHS.IDAL;
using XHS.Model;

namespace XHS.BLL
{
    /// <summary>
    /// 打印记录业务层。
    /// </summary>
    public class PrintRecord
    {
        private readonly IPrintRecord dal;

        public PrintRecord()
        {
            dal = XHS.DALFactory.PrintRecord.Create();
        }

        public List<PrintRecordInfo> GetPrintRecords()
        {
            return dal.GetPrintRecords();
        }

        public List<PrintRecordInfo> GetPrintRecords(string supplyBatchNo, string partNo, string cartonNo, string printType)
        {
            return dal.GetPrintRecords(supplyBatchNo, partNo, cartonNo, printType);
        }

        public List<PrintRecordInfo> GetPrintRecordsByBusinessKey(string supplyBatchNo, string partNo, string cartonNo, string printType)
        {
            return dal.GetPrintRecordsByBusinessKey(supplyBatchNo, partNo, cartonNo, printType);
        }

        public List<PrintRecordInfo> GetPrintRecordsByTaskId(System.Guid taskId)
        {
            return dal.GetPrintRecordsByTaskId(taskId);
        }

        public List<PrintRecordInfo> LockPendingPrintRecords(string machineId, int maxCount)
        {
            return dal.LockPendingPrintRecords(machineId, maxCount);
        }

        public string InsertPrintRecord(List<PrintRecordInfo> printRecordInfos)
        {
            NormalizePrintRecords(printRecordInfos);
            ParamterInfo paramterInfo = dal.InsertPrintRecord(printRecordInfos);
            IList source = new ArrayList();
            source.Add(paramterInfo);

            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string UpdatePrintRecord(List<PrintRecordInfo> printRecordInfos)
        {
            NormalizePrintRecords(printRecordInfos);
            ParamterInfo paramterInfo = dal.UpdatePrintRecord(printRecordInfos);
            IList source = new ArrayList();
            source.Add(paramterInfo);

            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string DeletePrintRecord(List<PrintRecordInfo> printRecordInfos)
        {
            ParamterInfo paramterInfo = dal.DeletePrintRecord(printRecordInfos);
            IList source = new ArrayList();
            source.Add(paramterInfo);

            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string CompletePrintRecordByTaskId(System.Guid taskId, string printUser)
        {
            List<PrintRecordInfo> printRecordInfos = dal.GetPrintRecordsByTaskId(taskId);
            if (printRecordInfos == null || printRecordInfos.Count == 0)
            {
                return "未找到对应的打印记录。";
            }

            System.DateTime now = System.DateTime.Now;
            string safePrintUser = string.IsNullOrWhiteSpace(printUser) ? string.Empty : printUser.Trim();
            foreach (PrintRecordInfo printRecordInfo in printRecordInfos)
            {
                if (printRecordInfo == null)
                {
                    continue;
                }

                printRecordInfo.Status = PrintRecordStatusInfo.Completed;
                printRecordInfo.PrintCount = 1;
                printRecordInfo.PrintUser = safePrintUser;
                printRecordInfo.PrintTime = now;
                if (string.IsNullOrWhiteSpace(printRecordInfo.FirstPrintUser))
                {
                    printRecordInfo.FirstPrintUser = safePrintUser;
                }
                if (!printRecordInfo.FirstPrintTime.HasValue)
                {
                    printRecordInfo.FirstPrintTime = now;
                }

                printRecordInfo.LastPrintUser = safePrintUser;
                printRecordInfo.LastPrintTime = now;
                printRecordInfo.UpdateUser = safePrintUser;
                printRecordInfo.UpdateTime = now;
                printRecordInfo.LockTime = null;
            }

            return UpdatePrintRecord(printRecordInfos);
        }

        private static void NormalizePrintRecords(List<PrintRecordInfo> printRecordInfos)
        {
            if (printRecordInfos == null)
            {
                return;
            }

            foreach (PrintRecordInfo printRecordInfo in printRecordInfos)
            {
                if (printRecordInfo == null)
                {
                    continue;
                }

                if (!printRecordInfo.TaskId.HasValue || printRecordInfo.TaskId == System.Guid.Empty)
                {
                    printRecordInfo.TaskId = System.Guid.NewGuid();
                }

                if (!printRecordInfo.Status.HasValue)
                {
                    printRecordInfo.Status = PrintRecordStatusInfo.Pending;
                }

                if (!printRecordInfo.PrintCount.HasValue)
                {
                    printRecordInfo.PrintCount = 0;
                }
            }
        }
    }
}
