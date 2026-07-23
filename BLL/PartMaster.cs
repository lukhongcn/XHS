using System.Collections;
using System.Collections.Generic;
using ModuleWorkFlow.business;
using XHS.IDAL;
using XHS.Model;

namespace BLL
{
    /// <summary>
    /// 零件主数据业务层。
    /// </summary>
    public class PartMaster
    {
        private readonly IPartMaster dal;

        public PartMaster()
        {
            dal = XHS.DALFactory.PartMaster.Create();
        }

        public List<PartMasterInfo> GetPartMasters()
        {
            return dal.GetPartMasters();
        }

        public PartMasterInfo GetPartMaster(int partMasterId)
        {
            return dal.GetPartMaster(partMasterId);
        }

        public PartMasterInfo GetPartMasterByJHSPartNo(string jhsPartNo)
        {
            return dal.GetPartMasterByJHSPartNo(jhsPartNo);
        }

        public string InsertPartMasters(List<PartMasterInfo> infos)
        {
            string validateMessage = Validate(infos);
            if (!string.IsNullOrWhiteSpace(validateMessage))
            {
                return validateMessage;
            }

            ParamterInfo paramterInfo = dal.InsertPartMasters(infos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string UpdatePartMasters(List<PartMasterInfo> infos)
        {
            string validateMessage = Validate(infos);
            if (!string.IsNullOrWhiteSpace(validateMessage))
            {
                return validateMessage;
            }

            ParamterInfo paramterInfo = dal.UpdatePartMasters(infos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string DeletePartMasters(List<PartMasterInfo> infos)
        {
            ParamterInfo paramterInfo = dal.DeletePartMasters(infos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        private static string Validate(List<PartMasterInfo> infos)
        {
            if (infos == null)
            {
                return string.Empty;
            }

            foreach (PartMasterInfo info in infos)
            {
                if (info == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(info.JHSPartNo))
                {
                    return "JHS 品号不能为空。";
                }
            }

            return string.Empty;
        }
    }
}
