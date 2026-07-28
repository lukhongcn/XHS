using System.Collections.Generic;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 装箱异常类型数据访问接口。
    /// </summary>
    public interface IPackingExceptionType
    {
        List<PackingExceptionTypeInfo> GetExPackingExceptionTypes();
    }
}
