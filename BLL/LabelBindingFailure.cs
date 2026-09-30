using System;
using XHS.IDAL;
using XHS.Model;

namespace BLL
{
    /// <summary>
    /// 光束标签绑定失败记录业务层。
    /// </summary>
    public class LabelBindingFailure
    {
        private readonly ILabelBindingFailure dal;

        public LabelBindingFailure()
        {
            dal = XHS.DALFactory.LabelBindingFailure.Create();
        }

        /// <summary>保存光束标签绑定失败记录。</summary>
        public bool Save(LabelBindingPdaScanFailureInfo info)
        {
            try
            {
                return dal.Insert(info);
            }
            catch (Exception ex)
            {
                Utility.Log.WriteLog("log.txt", "LabelBindingPDA 失败记录保存失败：" + ex);
                return false;
            }
        }
    }
}
