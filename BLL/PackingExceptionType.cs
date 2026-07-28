using System.Collections.Generic;
using XHS.IDAL;
using XHS.Model;

namespace BLL
{
    /// <summary>
    /// 装箱异常类型业务层。
    /// </summary>
    public class PackingExceptionType
    {
        private readonly IPackingExceptionType dal;

        public PackingExceptionType()
        {
            dal = XHS.DALFactory.PackingExceptionType.Create();
        }

        public List<PackingExceptionTypeInfo> GetExPackingExceptionTypes()
        {
            return dal.GetExPackingExceptionTypes();
        }
    }
}
