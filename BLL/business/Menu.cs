using System;
using System.Data;
using System.Data.SqlClient;

using Utility;
namespace ModuleWorkFlow.business
{
	/// <summary>
	/// Summary description for Menu.
	/// </summary>
	public class Menu
	{
		public Menu()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static DataSet getMenuView(int parentId)
		{
            string queryString = $"select * from tb_menu where len(menuid) <> 1 and menuclass like '%{System.Configuration.ConfigurationSettings.AppSettings["menu"]}%' ";
            if (parentId > 0)
            {
                queryString += " and parentId=@parentId ";
            }
            queryString += " order by menuid";
            SqlParameter[] pars = new SqlParameter[1];
            pars[0] = new SqlParameter("@parentId", SqlDbType.Int, 4);
            pars[0].Value = parentId;
            
			return Data.getDataSet(queryString,pars);
		}

		public static DataSet getMenuView(bool adminpart)
		{
			if (adminpart == true)
			{
				string queryString="select * from tb_menu where isadmin = 1 and len(menuid) <> 1 order by menuid";
				return Data.getDataSet(queryString);
			}
			else
			{
				string queryString="select * from tb_menu where isadmin = 0 and len(menuid) <> 1 order by menuid";
				return Data.getDataSet(queryString);
			}
		}
	}
}
