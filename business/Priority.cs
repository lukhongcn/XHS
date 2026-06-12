using System;
using System.Data;
using System.Data.SqlClient;

using Utility;
namespace ModuleWorkFlow.business
{
	/// <summary>
	/// Summary description for Priority.
	/// </summary>
	public class Priority
	{
		public static DataSet getPriorityView()
		{
			string queryString="select * from tb_priority order by priorityid desc";
			return Data.getDataSet(queryString);
		}

        public static DataSet getPriorityViewTop()
        {
            string queryString = "select top 3 * from tb_priority";
            return Data.getDataSet(queryString);
        }


		public static string getPrioriName(int priorityid)
		{
			if (priorityid == 0)
				priorityid = 60;

			string queryString="select priorityname from tb_priority where priorityid=@priorityid";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@priorityid", SqlDbType.Int);
			pars[0].Value = priorityid;
			return Data.getDataScalar(queryString,pars).ToString();
		}
	}
}
