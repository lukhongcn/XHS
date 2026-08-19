using System;
using System.Collections;
using System.Collections.Generic;
using ModuleWorkFlow.business;
using XHS.IDAL;
using XHS.Model;

namespace BLL
{
    /// <summary>
    /// 扫描流程记录业务层。
    /// </summary>
    public class ScanFlowScanRecord
    {
        private readonly IScanFlowScanRecord dal;
        private readonly ILabelBindingFailure failureDal;

        public ScanFlowScanRecord()
        {
            dal = XHS.DALFactory.ScanFlowScanRecord.Create();
            failureDal = XHS.DALFactory.LabelBindingFailure.Create();
        }

        public bool IsFactoryBarcodeRecorded(int flowId, string scanContent)
        {
            return flowId > 0 && !string.IsNullOrWhiteSpace(scanContent)
                && dal.IsFactoryBarcodeRecorded(flowId, scanContent.Trim());
        }

        public string SaveRecords(List<ScanFlowScanRecordInfo> infos)
        {
            if (infos == null || infos.Count == 0)
            {
                return "请至少提供一条扫描记录。";
            }

            ParamterInfo parameterInfo = dal.InsertRecords(infos);
            IList source = new ArrayList();
            source.Add(parameterInfo);
            return Common.Save(source) ? string.Empty : "扫描记录保存失败。";
        }

        public bool SavePdaScanFailure(LabelBindingPdaScanFailureInfo info)
        {
            try
            {
                return failureDal.Insert(info);
            }
            catch (Exception ex)
            {
                Utility.Log.WriteLog("log.txt", "LabelBindingPDA 失败记录保存失败：" + ex);
                return false;
            }
        }
    }
}
