using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Utility;

namespace ModuleWorkFlow.business
{
	/// <summary>
	/// Summary description for RoleProcess.
	/// </summary>
	public class RoleProcess
	{
		private string _username;
		private string _processid;

		public RoleProcess(){}
		
		public void setUserName(string val)
		{
			_username = val;
		}
		public string getUserName()
		{
			return _username;
		}

		public void setProcessId(string val)
		{
			_processid = val;
		}
		public string getProcessId()
		{
			return _processid;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		
		public static DataSet getRoleProcessView(string username)
		{
			string queryString="select tb_roleprocess.*,tb_process.processname from tb_roleprocess inner join tb_process on (tb_roleprocess.processid=tb_process.processid) where tb_roleprocess.username=@username order by tb_process.listorder";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@username", SqlDbType.NVarChar,50);
			pars[0].Value = username;

			return Data.getDataSet(queryString,pars);
		}

		public static bool UpdateRoleProcess(string username,IList process)
		{
			ArrayList alSql = new ArrayList();
			ArrayList alPars = new ArrayList();

			string querystring="delete from tb_roleprocess where username=@username";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@username", SqlDbType.NVarChar,50);
			pars[0].Value = username;
			
			alSql.Add(querystring);
			alPars.Add(pars);

			for (int i=0;i<process.Count;i++)
			{
				querystring="insert into tb_roleprocess (username,processid) values(@username,@processid)";
				pars = new SqlParameter[2];
				pars[0] = new SqlParameter("@username", SqlDbType.NVarChar,50);
				pars[0].Value = username;
				pars[1] = new SqlParameter("@processid", SqlDbType.NVarChar,50);
				pars[1].Value = process[i].ToString();

				alSql.Add(querystring);
				alPars.Add(pars);
			}

			return Data.excuteTrans(alSql,alPars);
		}
	}
}
