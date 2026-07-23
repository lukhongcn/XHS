using System.Collections.Generic;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 零件主数据数据访问接口。
    /// </summary>
    public interface IPartMaster
    {
        List<PartMasterInfo> GetPartMasters();

        PartMasterInfo GetPartMaster(int partMasterId);

        PartMasterInfo GetPartMasterByJHSPartNo(string jhsPartNo);

        ParamterInfo InsertPartMasters(List<PartMasterInfo> infos);

        ParamterInfo UpdatePartMasters(List<PartMasterInfo> infos);

        ParamterInfo DeletePartMasters(List<PartMasterInfo> infos);
    }
}
