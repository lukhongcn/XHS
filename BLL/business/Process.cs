using System;
using System.Data;
using System.Data.SqlClient;
using Utility;
using System.Collections;

namespace ModuleWorkFlow.business
{
	/// <summary>
	/// Summary description for Process.
	/// </summary>
	public class Process
	{
		private string ProcessId;
		private string ProcessName;
		private int ListOrder;

		public void setProcessId(string val)
		{
			this.ProcessId = val;
		}
		public string getProcessId()
		{
			return this.ProcessId;
		}

		public void setProcessName(string val)
		{
			this.ProcessName = val;
		}
		public string getProcessName()
		{
			return this.ProcessName;
		}

		public void setListOrder(int val)
		{
			this.ListOrder = val;
		}
		public int getListOrder()
		{
			return this.ListOrder;
		}


		public Process()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static DataSet getProcessView()
		{
			string queryString="select * from tb_process order by ListOrder";
			return Data.getDataSet(queryString);
		}

		public static DataSet getProcessViewOnlyMachine()
		{
			string queryString="select distinct customerprocessid,customerprocessname,CustomerListOrder,processName from tb_process where NeedSchedule=1 order by CustomerListOrder";
			return Data.getDataSet(queryString);
		}

        public static DataSet getProcessIdViewOnlyMachine()
        {
            string queryString = "select distinct processid,processname,CustomerListOrder,processName from tb_process where NeedSchedule=1 order by CustomerListOrder";
            return Data.getDataSet(queryString);
        }

        public static DataSet getProcessViewByNeedSchedule(int needschedule)
        {
            //needschedule = -1,then select all record
            string queryString = "";
            if (needschedule == -1)
            {
                queryString = "select * from tb_process where pricetype<>'design' order by ListOrder";
            }
            else
            {
                queryString = "select * from tb_process where needschedule=" + needschedule + " order by ListOrder";
            }
            return Data.getDataSet(queryString);
        }

		public static DataSet getProcessViewExceptAssmbleDesign()
		{
			string queryString="select * from tb_process where processid<>'assemble' and processid<>'design'  order by ListOrder";
			return Data.getDataSet(queryString);
		}

		public static DataSet getProcessViewExceptAssmbleDesignCAM()
		{
			string queryString="select * from tb_process where processid<>'assemble' and processid<>'design' and (NOT processid like 'CAM%') order by ListOrder";
			return Data.getDataSet(queryString);
		}

		public static DataSet getProcessViewForElectrode()
		{
			string queryString="select * from tb_process where processid='cut' or processid='cnce' or processid='qc' or processid='flash' order by ListOrder";
			return Data.getDataSet(queryString);
		}

		public static DataSet getProcessViewOnlyHasMachine()
		{
			string queryString="select * from tb_process where needschedule=1 order by ListOrder";
			return Data.getDataSet(queryString);
		}

		public static DataSet getProcessViewByProcessList(IList processList)
		{
			string cond = "";
			for (int i=0;i<processList.Count;i++)
			{
				cond+="'"+processList[i]+"',";
			}
			cond = cond.Substring(0,cond.Length-1);
			string queryString="select * from tb_process where processid in ("+cond+") order by ListOrder";
			return Data.getDataSet(queryString);
		}

		public static DataRow getProcessDataRow(string processid)
		{
			string queryString="select * from tb_process where processid=@processid";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@processid", SqlDbType.NVarChar,50);
			pars[0].Value = processid;
			return Data.getDataRow(queryString,pars);
		}

		public static Hashtable getProcessHoursPerDay()
		{
			Hashtable ht = new Hashtable();
			string queryString="select * from tb_process order by ListOrder";
			DataSet ds = Data.getDataSet(queryString);
			DataTable dt = ds.Tables[0];
			foreach(DataRow dr in dt.Rows)
			{
				ht.Add(dr["processid"].ToString().ToUpper(),dr["hoursperday"]);
			}
			return ht;
		}

	}
}
