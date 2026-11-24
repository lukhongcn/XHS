using System.Collections.Generic;
using ModuleWorkFlow.Model;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 零件工序查询数据访问接口。
    /// </summary>
    public interface IXHSPartPartProcess
    {
        List<XHSPartPartProcessInfo> GetPartProcesses(
            string customerAbbr,
            string partNo,
            string barcode,
            IList<string> qrDateCodes);

        List<XHSPartPartProcessInfo> GetPartProcessesByPartNoId(string partNoId);

        List<XHSPartPartProcessInfo> GetPartProcessesByPartNoIdAndProcessIdAndStatusId(
            string partNoId,
            string processId,
            string statusId);

        XHSPartPartProcessInfo GetBarcodePartProcess(
            string partNoId,
            string processId,
            string statusId);

        ModuleWorkFlow.Model.ParamterInfo UpdatePartProcessFromModifyEdit(
            List<PartPartProcessInfo> source);

        ModuleWorkFlow.Model.ParamterInfo updateDiscount(string partNoId);

        ModuleWorkFlow.Model.ParamterInfo UpdatePartNoId(
            string partNoId,
            string replacementPartNoId);
    }
}
