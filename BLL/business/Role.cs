using System;
using System.Data;
using System.Data.SqlClient;

using Utility;

namespace ModuleWorkFlow.business
{
	/// <summary>
	/// Summary description for role.
	/// </summary>
	public class Role
	{
		private string RoleId;
		private string RoleName;

		public void setRoleId(string val)
		{
			this.RoleId = val;
		}
		public string getRoleId()
		{
			return this.RoleId;
		}

		public void setRoleName(string val)
		{
			this.RoleName = val;
		}
		public string getRoleName()
		{
			return this.RoleName;
		}

		public Role()
		{
			
		}

		public int DuplicateKeys()
		{
			string queryString="select count(*) as recordnum from tb_role where roleid=@roleid";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@roleid", SqlDbType.NVarChar,50);
			pars[0].Value = this.RoleId;
			return Convert.ToInt32(Data.getDataScalar(queryString,pars).ToString()); 
		} 

		public int Create()
		{
			string procedureString="sp_CreateRole";
			SqlParameter[] pars = new SqlParameter[2];
			pars[0] = new SqlParameter("@RoleId", SqlDbType.NVarChar,50);
			pars[0].Value = this.RoleId;
			pars[1] = new SqlParameter("@RoleName",SqlDbType.NVarChar,50);
			pars[1].Value=this.RoleName;
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

		public int Remove()
		{
			string procedureString="sp_RemoveRole";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@RoleId",SqlDbType.NVarChar,50);
			pars[0].Value=this.RoleId;
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

		public int Store()
		{
			string procedureString="sp_StoreRole";
			SqlParameter[] pars = new SqlParameter[2];
			pars[0] = new SqlParameter("@RoleId", SqlDbType.NVarChar,50);
			pars[0].Value = this.RoleId;
			pars[1] = new SqlParameter("@RoleName",SqlDbType.NVarChar,50);
			pars[1].Value=this.RoleName;
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

		public static DataSet getRoleView()
		{
			string queryString="select * from tb_role order by roleid";
			return Data.getDataSet(queryString);
		}

		public static DataRow getRoleRow(string roleid)
		{
			string queryString="select * from tb_role where roleid=@roleid order by roleid";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@roleid", SqlDbType.NVarChar,50);
			pars[0].Value = roleid;
			return Data.getDataRow(queryString,pars);
		}

		public int ExistRelationData()
		{
			string queryString="select count(*) as recordnum from tb_user where roleid=@roleid";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@roleid", SqlDbType.NVarChar,50);
			pars[0].Value = this.RoleId;
			return Convert.ToInt32(Data.getDataScalar(queryString,pars).ToString()); 
		}

	}
}
