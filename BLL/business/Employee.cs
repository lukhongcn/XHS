using System;
using System.Data;
using System.Data.SqlClient;

using Utility;

namespace ModuleWorkFlow.business
{
	/// <summary>
	/// Summary description for User.
	/// </summary>
	public class Employee
	{
		private string UserName;
		private string Password;
		private string RoleId;
		private string Name;
		private string CardId;
		private string DepartId;
		private string Email;
		private int IsAdmin;

		public void setUserName(string val)
		{
			this.UserName = val;
		}
		public string getUserName()
		{
			return this.UserName;
		}

		public void setPassword(string val)
		{
			this.Password = val;
		}
		public string getPassword()
		{
			return this.Password;
		}

		public void setRoleId(string val)
		{
			this.RoleId = val;
		}
		public string getRoleId()
		{
			return this.RoleId;
		}

		public void setName(string val)
		{
			this.Name = val;
		}
		public string getName()
		{
			return this.Name;
		}

		public void setCardId(string val)
		{
			this.CardId = val;
		}
		public string getCardId()
		{
			return this.CardId;
		}

		public void setDepartId(string val)
		{
			this.DepartId = val;
		}
		public string getDepartId()
		{
			return this.DepartId;
		}

		public void setEmail(string val)
		{
			this.Email = val;
		}
		public string getEmail()
		{
			return this.Email;
		}

		public void setIsAdmin(int val)
		{
			this.IsAdmin = val;
		}
		public int getIsAdmin()
		{
			return this.IsAdmin;
		}

		public Employee()
		{
		}

		public static DataSet getEmplView()
		{
			string queryString="select tb_user.*,tb_user.username+'['+tb_user.name+']' as displayname,tb_role.rolename from tb_user inner join tb_role on (tb_role.roleid=tb_user.roleid) where tb_user.username<>'admin' order by username";
			return Data.getDataSet(queryString);
		}

		public static DataSet getEmplView(string name_card_like)
		{
			string queryString="select tb_user.*,tb_user.username+'['+tb_user.name+']' as displayname,tb_role.rolename from tb_user inner join tb_role on (tb_role.roleid=tb_user.roleid) "
                                + "where isResignation=0  and tb_user.username<>'admin' and tb_user.Name like '%" + name_card_like + "%' or tb_user.username like '%" + name_card_like + "%'  order by username";
			return Data.getDataSet(queryString);
		}

		public static DataRow getEmplDataRow(string username)
		{
			string queryString="select tb_user.*,tb_user.username+'['+tb_user.name+']' as displayname,tb_role.rolename from tb_user inner join tb_role on (tb_role.roleid=tb_user.roleid) where tb_user.username=@username order by username";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@username", SqlDbType.NVarChar,50);
			pars[0].Value = username;
			return Data.getDataRow(queryString,pars);
		}

		public int Store()
		{
			string procedureString="sp_StoreUser";
			SqlParameter[] pars = new SqlParameter[2];
			pars[0] = new SqlParameter("@username", SqlDbType.NVarChar,50);
			pars[0].Value = this.UserName;
			pars[1]=new SqlParameter("@email",SqlDbType.NVarChar,50);
			pars[1].Value=this.Email;
			Object o=Data.getDataScalar(procedureString,CommandType.StoredProcedure,pars);
			if (o==null)
			{
				return -1;
			}
			else
			{
				return (int) o;
			}
		}

		public int StorePrivilege()
		{
			string procedureString="sp_StoreUserPrivilege";
			SqlParameter[] pars = new SqlParameter[4];
			pars[0] = new SqlParameter("@username", SqlDbType.NVarChar,50);
			pars[0].Value = this.UserName;
			pars[1]=new SqlParameter("@password",SqlDbType.NVarChar,50);
			pars[1].Value=this.Password;
			pars[2] = new SqlParameter("@roleid", SqlDbType.NVarChar,50);
			pars[2].Value = this.RoleId;
			pars[3]=new SqlParameter("@isadmin",SqlDbType.Int);
			pars[3].Value=this.IsAdmin;

			Object o=Data.getDataScalar(procedureString,CommandType.StoredProcedure,pars);
			if (o==null)
			{
				return -1;
			}
			else
			{
				return (int) o;
			}
		}
	}
}
