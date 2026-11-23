using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using ModuleWorkFlow.BLL;
using XHS.Model;

namespace ModuleWorkFlow.business
{
	/// <summary>
	/// Summary description for Common.
	/// </summary>
	public class Common
	{
		#region ���^dropdownlist�N���������������a����������������������
		public static void DDL_BindData_CustomerList(DropDownList DDL)
		{
			ModuleWorkFlow.BLL.Customer customer = new ModuleWorkFlow.BLL.Customer();
			DDL.DataTextField = "ChineseName";
			DDL.DataValueField = "Id";
			DDL.DataSource = customer.GetCustomer();
			DDL.DataBind();

			//insert all
			ListItem li = new ListItem(lang.TXT_SELECTNAME,lang.TXT_SELECTVALUE);
			DDL.Items.Insert(0,li);
		}

		public static void DDL_BindData_ModuleList(DropDownList DDL, string customerid)
		{
			ModuleWorkFlow.BLL.Order order = new ModuleWorkFlow.BLL.Order();
			DDL.DataTextField = "id";
			DDL.DataValueField = "id";
			if (!customerid.Equals(lang.TXT_SELECTVALUE))
			{
				DDL.DataSource = order.GetMsListOrderInfo(customerid);
			}
			else
			{
				DDL.DataSource = order.GetMsListOrderInfo();
			}
			DDL.DataBind();

			//insert all
			ListItem li = new ListItem(lang.TXT_SELECTNAME,lang.TXT_SELECTVALUE);
			DDL.Items.Insert(0,li);
		}

		public static void DDL_BindData_PartNoList(DropDownList DDL, string moduleid)
		{
			ModuleWorkFlow.BLL.Part part = new ModuleWorkFlow.BLL.Part();
			DDL.DataTextField = "PartNo";
			DDL.DataValueField = "PartNo";

			if (!moduleid.Equals(lang.TXT_SELECTVALUE))
			{
				//MYD070604
				//DDL.DataSource = part.getPartInfo(moduleid);
				DDL.DataSource = part.getPartAndElecInfo(moduleid);
				DDL.DataBind();
			}
			else
			{
				// do nothing
				DDL.Items.Clear();
			}

			//insert all
			ListItem li = new ListItem(lang.TXT_SELECTNAME,lang.TXT_SELECTVALUE);
			DDL.Items.Insert(0,li);
		}

		public static void DDL_BindData_DesignPartNoList(DropDownList DDL, string moduleid)
		{
			ModuleWorkFlow.BLL.Part part = new ModuleWorkFlow.BLL.Part();
			DDL.DataTextField = "PartNo";
			DDL.DataValueField = "PartNo";

			if (!moduleid.Equals(lang.TXT_SELECTVALUE))
			{
				DDL.DataSource = part.getDesignPart(moduleid);
				DDL.DataBind();
			}
			else
			{
				DDL.Items.Clear();
			}

			ListItem li = new ListItem(lang.TXT_SELECTNAME,lang.TXT_SELECTVALUE);
			DDL.Items.Insert(0,li);
		}


		public static void DDL_BindData_PartNoIdList(DropDownList DDL, string moduleid, string partno)
		{
			ModuleWorkFlow.BLL.PartProcess pp = new ModuleWorkFlow.BLL.PartProcess();
			if (!partno.Equals(lang.TXT_SELECTVALUE))
			{	
				DDL.DataSource = pp.getListPartNoIdList(moduleid, partno);
				DDL.DataBind();
			}
			else
			{
				// do nothing
				DDL.Items.Clear();
			}

			//insert all
			ListItem li = new ListItem(lang.TXT_SELECTNAME,lang.TXT_SELECTVALUE);
			DDL.Items.Insert(0,li);
		}

		public static void DDL_BindData_YearList(DropDownList DDL)
		{
			Methods.YearDropDownListInit(DDL);
			//insert all
			ListItem li = new ListItem(lang.TXT_SELECTNAME,lang.TXT_SELECTVALUE);
			DDL.Items.Insert(0,li);
		}

		public static void DDL_BindData_MonthList(DropDownList DDL)
		{
			Methods.MonthDropDownListInit(DDL);
			//insert all
			ListItem li = new ListItem(lang.TXT_SELECTNAME,lang.TXT_SELECTVALUE);
			DDL.Items.Insert(0,li);
		}

	

		public static void DDL_BindData_EmailGroup(DropDownList DDL)
		{
			DDL.Items.Clear();
			ListItem li = new ListItem(lang.TXT_EMAILGROUP_TXT0,lang.TXT_EMAILGROUP_VALUE0);
			DDL.Items.Insert(0,li);
			li = new ListItem(lang.TXT_EMAILGROUP_TXT1,lang.TXT_EMAILGROUP_VALUE1);
			DDL.Items.Insert(1,li);
			li = new ListItem(lang.TXT_EMAILGROUP_TXT2,lang.TXT_EMAILGROUP_VALUE2);
			DDL.Items.Insert(2,li);
			li = new ListItem(lang.TXT_EMAILGROUP_TXT3,lang.TXT_EMAILGROUP_VALUE3);
			DDL.Items.Insert(3,li);
			li = new ListItem(lang.TXT_EMAILGROUP_TXT4,lang.TXT_EMAILGROUP_VALUE4);
			DDL.Items.Insert(4,li);
            li = new ListItem(lang.TXT_EMAILGROUP_TXT5, lang.TXT_EMAILGROUP_VALUE5);
            DDL.Items.Insert(5, li);
		}

		public static void DDL_BindData_ProcessList(DropDownList DDL)
		{
			ModuleWorkFlow.BLL.Process process = new ModuleWorkFlow.BLL.Process();
			DDL.DataValueField = "ProcessId";
			DDL.DataTextField = "ProcessName";
			DDL.DataSource = process.GetProcessInfo();
			DDL.DataBind();

			//insert all
			ListItem li = new ListItem(lang.TXT_SELECTALLNAME,lang.TXT_SELECTVALUE);
			DDL.Items.Insert(0,li);
		}

		public static void DDL_BindData_DesignProcessList(DropDownList DDL)
		{
			ModuleWorkFlow.BLL.DesignProcess process = new ModuleWorkFlow.BLL.DesignProcess();
			DDL.DataValueField = "ProcessId";
			DDL.DataTextField = "ProcessName";
			DDL.DataSource = process.GetDesignProcessInfo();
			DDL.DataBind();

			//insert all
			ListItem li = new ListItem(lang.TXT_SELECTALLNAME,lang.TXT_SELECTVALUE);
			DDL.Items.Insert(0,li);
		}


		public static void DDL_BindData_StatusList(DropDownList DDL)
		{
			ModuleWorkFlow.BLL.Status status = new ModuleWorkFlow.BLL.Status();
			DDL.DataValueField = "StatusId";
			DDL.DataTextField = "StatusDesc";
			DDL.DataSource = status.getStatusInfo();
			DDL.DataBind();

			//insert all
			ListItem li = new ListItem(lang.TXT_SELECTALLNAME,lang.TXT_SELECTVALUE);
			DDL.Items.Insert(0,li);
		}

		#endregion
		#region JG-071128
		public static void DDL_BindData_DepartMentList(DropDownList DDL)
		{
			ModuleWorkFlow.BLL.DepartMent department = new ModuleWorkFlow.BLL.DepartMent();
			DDL.DataTextField = "DepartmentName";
			DDL.DataValueField = "DepartmentId";
			IList departments = department.GetAllDepartment();
			ModuleWorkFlow.Model.DepartMentInfo dpmi = new ModuleWorkFlow.Model.DepartMentInfo();
			dpmi.DepartmentId = 0;
			dpmi.DepartmentName = "�Ҧ�";//�s�V
			departments.Insert(0,dpmi);
			DDL.DataSource = departments;
			DDL.DataBind();

			//insert all
			ListItem li = new ListItem(lang.TXT_SELECTNAME,lang.TXT_SELECTVALUE);
			DDL.Items.Insert(0,li);
		}
		#endregion

		public static void DDL_BindData_DesignProcess(DropDownList DDL)
		{
			ModuleWorkFlow.BLL.DesignProcess designProcess=new DesignProcess();
			DDL.DataValueField = "ProcessId";
			DDL.DataTextField = "ProcessName";
			DDL.DataSource = designProcess.GetDesignProcessInfo();
			DDL.DataBind();

			//insert all
			ListItem li = new ListItem(lang.TXT_SELECTALLNAME,lang.TXT_SELECTVALUE);
			DDL.Items.Insert(0,li);
		}


        public static bool Save(IList ilist)
        {
            ArrayList alsql = new ArrayList();
            ArrayList alpars = new ArrayList();
            ArrayList alcom = new ArrayList();
            foreach (ParamterInfo pi in ilist)
            {
                if (pi.AlSQL != null && pi.AlSQL.Count > 0)
                {
                    for (int i = 0; i < pi.AlSQL.Count; i++)
                    {
                        alsql.Add(pi.AlSQL[i]);
                        alpars.Add(pi.AlPAR[i]);
                        alcom.Add(pi.AlCOM[i]);
                    }
                }
                else
                {
                    alsql.Add(pi.Sql);
                    alpars.Add(pi.Pars);
                    alcom.Add(pi.Type);
                }
            }
            return Data.excuteTrans(alsql, alpars, alcom);
        }

        public static bool Save(List<ParamterInfo> ilist)
        {
            return Save((IList)ilist);
        }

        public static bool SingleSave(ParamterInfo pi)
        {
            ArrayList alsql = new ArrayList();
            ArrayList alpars = new ArrayList();
            ArrayList alcom = new ArrayList();
            
            alsql.Add(pi.Sql);
            alpars.Add(pi.Pars);
            alcom.Add(pi.Type);
            
            return Data.excuteTrans(alsql, alpars, alcom);
        }
	}
}
