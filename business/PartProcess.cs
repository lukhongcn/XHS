using System;
using System.Collections;
using System.Data;


using System.Text;
using System.Data.SqlClient;
using ModuleWorkFlow.Model;
using ModuleWorkFlow.BLL;
using ModuleWorkFlow.BLL.System;
using ModuleWorkFlow.Model.System;
using Utility;
using System.Collections.Generic;

namespace ModuleWorkFlow.business
{
    /// <summary>
    /// Summary description for PartProcess.
    /// </summary>
    public class PartProcess
    {
        private string ModuleId;
        private string PartNo;
        private string ProcessId;
        private string PartNo_Id;
        private string SatusId;
        private int ProcessNeedMinutes;
        private int ProcessOrder;
        private DateTime StartDate;
        private DateTime EndDate;
        private string Responsible;
        private int NeedProduct;
        private int priority;
        private DateTime selfStartDate;
        private DateTime selfEndDate;

        private DateTime middlestartdate;
        private DateTime middleenddate;
        private string RelationPartNo;
        private string RelationProcessNo;
        private string _assigned;
        private string _responsPercent;
        //whd080901
        private int isUnnormal;

        private string PartProcessId;

        public string Assigned
        {
            set { _assigned = value; }
            get { return _assigned; }
        }
        public void setisUnnormal(int val)
        {
            this.isUnnormal = val;
        }

        public int getisUnnormal()
        {
            return this.isUnnormal;
        }
        //endwhd080901

        //


        public void setSelfStartDate(DateTime val)
        {
            this.selfStartDate = val;
        }
        public DateTime getSelfStartDate()
        {
            return this.selfStartDate;
        }

        public void setSelfEndDate(DateTime val)
        {
            this.selfEndDate = val;
        }
        public DateTime getSelfEndDate()
        {
            return this.selfEndDate;
        }

        public void setModuleId(string val)
        {
            this.ModuleId = val;
        }
        public string getModuleId()
        {
            return this.ModuleId;
        }

        public void setPartNo(string val)
        {
            this.PartNo = val;
        }
        public string getPartNo()
        {
            return this.PartNo;
        }
        private int RelationOrder;
        public void setRelationOrder(int val)
        {
            this.RelationOrder = val;
        }
        public int getRelationOrder()
        {
            return this.RelationOrder;
        }

        public void setProcessId(string val)
        {
            this.ProcessId = val;
        }
        public string getProcessId()
        {
            return this.ProcessId;
        }

       public string Category { set; get; }

        public int DesignReasonId { set; get; }

        public void setPartNo_Id(string val)
        {
            this.PartNo_Id = val;
        }
        public string getPartNo_Id()
        {
            return this.PartNo_Id;
        }

        public void setSatusId(string val)
        {
            this.SatusId = val;
        }
        public string getSatusId()
        {
            return this.SatusId;
        }

        public void setProcessNeedMinutes(int val)
        {
            this.ProcessNeedMinutes = val;
        }
        public int getProcessNeedMinutes()
        {
            return this.ProcessNeedMinutes;
        }

        public void setProcessOrder(int val)
        {
            this.ProcessOrder = val;
        }
        public int getProcessOrder()
        {
            return this.ProcessOrder;
        }

        public void setStartDate(DateTime val)
        {
            this.StartDate = val;
        }
        public DateTime getStartDate()
        {
            return this.StartDate;
        }

        public void setEndDate(DateTime val)
        {
            this.EndDate = val;
        }
        public DateTime getEndDate()
        {
            return this.EndDate;
        }

        public void setResponsible(string val)
        {
            this.Responsible = val;
        }
        public string getResponsible()
        {
            return this.Responsible;
        }

        public void setNeedProduct(int val)
        {
            this.NeedProduct = val;
        }
        public int getNeedProduct()
        {
            return this.NeedProduct;
        }

        public void setPriority(int val)
        {
            priority = val;
        }
        public int getPriority()
        {
            return priority;
        }

        public void setMiddleStartDate(DateTime val)
        {
            middlestartdate = val;
        }
        public DateTime getMiddleStartDate()
        {
            return middlestartdate;
        }

        public void setMiddleEndDate(DateTime val)
        {
            middleenddate = val;
        }
        public DateTime getMiddleEndDate()
        {
            return middleenddate;
        }

        private int _totalminutes;
        public int TotalMinutes
        {
            set { _totalminutes = value; }
            get { return _totalminutes; }
        }

        private int _isDesign;
        public int IsDesign
        {
            set { _isDesign = value; }
            get { return _isDesign; }
        }



        public void setRelationPartNo(string val)
        {
            this.RelationPartNo = val;
        }

        public string getRelationPartNo()
        {
            return RelationPartNo;

        }

        public void setRelationProcessNo(string val)
        {
            this.RelationProcessNo = val;
        }

        public string getRelationProcessNo()
        {
            return RelationProcessNo;
        }

        public void setPartProcessId(string val)
        {
            this.PartProcessId = val;
        }
        public string getPartProcessId()
        {
            return PartProcessId;
        }

        private int IsConfirm;
        public void setIsConfirm(int val)
        {
            this.IsConfirm = val;
        }
        public int getIsConfirm()
        {
            return IsConfirm;
        }

        private string ProcessComment;
        public void setProcessComment(string val)
        {
            this.ProcessComment = val;
        }
        public string getProcessComment()
        {
            return ProcessComment;
        }

        private string ProcessMachineid;
        public void setProcessMachineid(string val)
        {
            this.ProcessMachineid = val;
        }
        public string getProcessMachineid()
        {
            return ProcessMachineid;
        }

        private double finishedPercent;
        public void setfinishedPercent(double val)
        {
            this.finishedPercent = val;
        }
        public double getfinishedPercent()
        {
            return finishedPercent;
        }

        public void setResponsPercent(string val)
        {
            _responsPercent = val;
        }

        public string getResponsPercent()
        {
            return _responsPercent;
        }

        public string DesignReasonAbbr { set; get; }



        public PartProcess()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static DataSet getModulePartProcessView(string moduleid, string partnoId)
        {
            string queryString = "select tb_partprocess.processorder listorder," +
                                      "tb_partprocess.processid processid," +
                                      "tb_partprocess.statusid statusid," +
                                      "tb_process.processname processname," +
                                      "tb_partprocess.factstartdate factstartdate," +
                                      "tb_partprocess.factenddate factenddate," +
                                      "tb_partprocess.processno processno,tb_partprocess.NeedProduct NeedProduct,tb_partprocess.unNormalReasonsId," +
                                      "tb_partprocess.processneedminutes processneedminutes,totalminutes,selfStartDate,selfEndDate," +
                                      "tb_partprocess.isUnnormal,tb_partprocess.processComment,tb_partprocess.processMachineid,tb_process.pricetype " +
                               " from tb_partprocess " +
                               " inner join tb_process on (tb_partprocess.processid=tb_process.processid)" +
                               " where tb_partprocess.ModuleId=@moduleId and tb_partprocess.partno_id=@partno_id" +
                               " and tb_partprocess.ProcessId not in ('Design','Assemble') " +
                               " order by tb_partprocess.processOrder ";

            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleId", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno_id", SqlDbType.NVarChar, 50);
            pars[1].Value = partnoId;
            return Data.getDataSet(queryString, pars);
        }

        public static DataSet getModuleDesignPartProcessView(string moduleid, string partnoId)
        {
            string queryString = "select tb_DesignPartProcess.processorder listorder,category,partno='',DesignReasonId as Reason," +
                "tb_DesignPartProcess.processid processid," +
                "tb_DesignPartProcess.statusid statusid," +
                "tb_DesignProcess.processname processname," +
                "tb_DesignPartProcess.factstartdate factstartdate," +
                "tb_DesignPartProcess.factenddate factenddate," +
                "tb_DesignPartProcess.processno processno," +
                "tb_DesignPartProcess.finishedPercent finishedPercent," +
                "tb_DesignPartProcess.processneedminutes processneedminutes,totalminutes,selfStartDate,selfEndDate,assigned,responsPercent " +
                " from tb_DesignPartProcess " +
                " inner join tb_DesignProcess on (tb_DesignPartProcess.processid=tb_DesignProcess.processid)" +
                " where tb_DesignPartProcess.ModuleId=@moduleId and tb_DesignPartProcess.partno_id=@partno_id" +
                " order by tb_DesignPartProcess.processOrder ";

            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleId", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno_id", SqlDbType.NVarChar, 50);
            pars[1].Value = partnoId;
            return Data.getDataSet(queryString, pars);
        }

        //MYB061226-3-5
        public static DataSet getAllModulePartProcessView(string moduleid, string partnoId)
        {
            ProcessCustomer processCustomer = new ProcessCustomer();
            string queryString = "select tb_partprocess.processorder listorder,partno,tb_partprocess.customerProcessid,customerprocessname='',supplyId ," +
                "tb_partprocess.processid processid," +
                "tb_partprocess.statusid statusid," +
                "tb_process.processname processname," +
                "tb_partprocess.factstartdate factstartdate," +
                "tb_partprocess.factenddate factenddate," +
                "tb_partprocess.processno processno,tb_partprocess.NeedProduct NeedProduct,tb_partprocess.unNormalReasonsId," +
                "tb_partprocess.departMentId,tb_partprocess.processneedminutes processneedminutes,totalminutes,selfStartDate,selfEndDate," +
                "tb_partprocess.isUnnormal,tb_partprocess.processComment,tb_partprocess.processMachineid,tb_process.pricetype,RelationPartNo,RelationPartNoId " +
                " from tb_partprocess " +
                " inner join tb_process on (tb_partprocess.processid=tb_process.processid)" +
                " where tb_partprocess.ModuleId=@moduleId and tb_partprocess.partno_id=@partno_id" +
                " order by tb_partprocess.processOrder ";

            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleId", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno_id", SqlDbType.NVarChar, 50);
            pars[1].Value = partnoId;
            DataSet ds = Data.getDataSet(queryString, pars);
            DataTable dt = ds.Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                string processid = dr["processid"].ToString();
                string processCustomerId = dr["customerProcessid"].ToString();
                if (processCustomerId != null && !processCustomerId.Equals("") && !processid.Equals(processCustomerId))
                {
                    ProcessCustomerInfo pci = processCustomer.findbykey(processCustomerId);
                    if (pci != null)
                    {
                        dr["CustomerProcessName"] = pci.CustomerProcessName;
                    }

                }
                else
                {
                    dr["CustomerProcessName"] = dr["processname"];
                    if (processCustomerId != null || !processCustomerId.Equals(""))
                    {
                        dr["CustomerProcessId"] = processid;
                    }
                }

            }

            return ds;
        }


        public static DataSet getAllProductPartProcessView(string moduleid, string partnoId)
        {
            string queryString = "select tb_partprocess.processorder listorder,tb_partprocess.partno,tb_productPartNo.finishedCount+tb_productPartNo.workintCount dealcount,tb_productPartNo.finishedCount-tb_productPartNo.discardCount-tb_productPartNo.returnCount validatedCount,tb_productPartNo.discardCount+tb_productPartNo.returnCount unValidatedCount," +
                "tb_partprocess.processid processid," +
                "tb_partprocess.statusid statusid," +
                "tb_process.processname processname," +
                "tb_partprocess.factstartdate factstartdate," +
                "tb_partprocess.factenddate factenddate," +
                "tb_partprocess.processno processno,tb_partprocess.NeedProduct NeedProduct,tb_productPartNo.canReady," +
                "tb_partprocess.processneedminutes processneedminutes,totalminutes,tb_partprocess.selfStartDate,tb_partprocess.selfEndDate," +
                "tb_partprocess.processComment,tb_partprocess.processMachineid,tb_process.pricetype,id productPartnoId " +
                " from tb_partprocess " +
                " inner join tb_process on (tb_partprocess.processid=tb_process.processid)" +
                " inner join tb_productPartNo on (tb_partprocess.partNo_Id=tb_productPartNo.partNo_Id and  tb_partprocess.processorder = tb_productPartNo.processorder) " +
                " where tb_partprocess.ModuleId=@moduleId and tb_partprocess.partno_id=@partno_id" +
                " order by tb_partprocess.processOrder ";

            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleId", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno_id", SqlDbType.NVarChar, 50);
            pars[1].Value = partnoId;
            return Data.getDataSet(queryString, pars);
        }

        public static DataSet getLowsetModulePartProcessView(string moduleid, string partno)
        {
            string queryString = "select tb_partprocess.processorder listorder," +
                "tb_partprocess.processid processid," +
                "tb_partprocess.statusid statusid," +
                "tb_process.processname processname," +
                "tb_partprocess.factstartdate factstartdate," +
                "tb_partprocess.factenddate factenddate," +
                "tb_partprocess.partno_id partno_id," +
                "tb_partprocess.processno processno,tb_partprocess.NeedProduct NeedProduct,tb_partprocess.unNormalReasonsId," +
                "tb_partprocess.processneedminutes processneedminutes,totalminutes,selfStartDate,selfEndDate," +
                "tb_partprocess.isUnnormal,tb_partprocess.processComment,tb_partprocess.processMachineid,tb_process.pricetype " +
                " from tb_partprocess " +
                " inner join tb_process on (tb_partprocess.processid=tb_process.processid)" +
                " where tb_partprocess.ModuleId=@moduleId and tb_partprocess.partno_id like '" + partno + "%' " +
                " order by tb_partprocess.partno_id,tb_partprocess.processOrder ";

            SqlParameter[] pars = new SqlParameter[1];
            pars[0] = new SqlParameter("@moduleId", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;

            DataSet ds = Data.getDataSet(queryString, pars);

            DataTable dt = ds.Tables[0];
            string partnoId = "";

            foreach (DataRow dr in dt.Rows)
            {
                partnoId = dr["partno_id"].ToString();
            }

            DataRow[] removeRow = dt.Select("partno_id <> '" + partnoId + "'");
            foreach (DataRow dr in removeRow)
            {
                dt.Rows.Remove(dr);
            }

            return ds;
        }

        public static DataSet getModulePartProcessViewWithFactDateNULL(string moduleid, string partno)
        {
            string queryString = "select processorder from tb_partprocess " +
                " where ModuleId=@moduleid and partno=@partno and (factstartdate is null and factenddate is null) " +
                " group by processorder order by processorder ";

            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = partno;
            return Data.getDataSet(queryString, pars);
        }

        public static DataSet getProcessView(string moduleid, string partnoid)
        {
            string queryString = "select processid,processname from tb_process where processid in (select distinct processid from tb_partprocess where moduleid=@moduleid and partno=@partno) order by listorder ";

            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleId", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = partnoid;
            return Data.getDataSet(queryString, pars);
        }

        public static DataSet getProcessView(string moduleid)
        {
            string queryString = "select processid,processname from tb_process where processid in (select distinct processid from tb_partprocess where moduleid=@moduleid) order by listorder ";

            SqlParameter[] pars = new SqlParameter[1];
            pars[0] = new SqlParameter("@moduleId", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            return Data.getDataSet(queryString, pars);
        }

        public static DataSet getProcessViewByModuleIdPartNo(string moduleid, string partnoid)
        {
            //MYD070528-1
            string queryString = "select *,processname from tb_partprocess join tb_process on tb_partprocess.processid=tb_process.processid where moduleid=@moduleid and partno=@partno order by partno,processorder ";

            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleId", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = partnoid;
            return Data.getDataSet(queryString, pars);
        }

        public static DataSet getProcessAssemble(string condition)
        {
            string queryString = "select moduleid,startdate,enddate from tb_partprocess where " + condition + " AND processid='assemble' group by moduleid,startdate,enddate ";
            return Data.getDataSet(queryString);
        }

        public static Hashtable getNeedProduct(string moduleId, string processId)
        {


            string queryString = "select * from tb_partprocess where ModuleId=@moduleId and processId=@processId and needProduct=1";
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleId", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleId;
            pars[1] = new SqlParameter("@processId", SqlDbType.NVarChar, 50);
            pars[1].Value = processId;
            DataSet dataSet = Data.getDataSet(queryString, pars);
            Hashtable hNeedMinutes = new Hashtable();
            DataTable dt = new DataTable();
            dt = dataSet.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                hNeedMinutes.Add(Convert.ToString(dr["PartNo_Id"]).Trim(), Convert.ToInt32(dr["ProcessNeedMinutes"]));
            }

            return hNeedMinutes;

        }

        public static Hashtable getPartProcessByPrimaryKey()
        {
            string queryString = "select * from tb_partprocess";
            DataSet dataSet = Data.getDataSet(queryString);
            Hashtable hPartProcess = new Hashtable();
            DataTable dt = new DataTable();
            dt = dataSet.Tables[0];

            DataColumn col1 = dt.Columns["moduleid"];
            DataColumn col2 = dt.Columns["partno"];
            DataColumn col3 = dt.Columns["partno_id"];
            DataColumn col4 = dt.Columns["processneedminutes"];
            DataColumn col5 = dt.Columns["processid"];
            DataColumn col6 = dt.Columns["statusid"];
            DataColumn col7 = dt.Columns["processorder"];
            DataColumn col8 = dt.Columns["startdate"];
            DataColumn col9 = dt.Columns["enddate"];
            DataColumn col10 = dt.Columns["needproduct"];
            DataColumn col11 = dt.Columns["priority"];
            DataColumn col12 = dt.Columns["factstartdate"];
            DataColumn col13 = dt.Columns["factenddate"];
            DataColumn col14 = dt.Columns["factprocessneedminutes"];
            DataColumn col15 = dt.Columns["machineid"];

            foreach (DataRow row in dt.Rows)
            {
                PartProcessInfo partprocessinfo = new PartProcessInfo();
                partprocessinfo.ModuleId = row[col1].ToString();
                partprocessinfo.PartNo = row[col2].ToString();
                partprocessinfo.PartNo_Id = row[col3].ToString();
                partprocessinfo.ProcessNeedMinutes = Convert.ToInt32(row[col4]);
                partprocessinfo.ProcessId = row[col5].ToString();
                partprocessinfo.StatusId = row[col6].ToString();
                partprocessinfo.ProcessOrder = Convert.ToInt32(row[col7]);
                partprocessinfo.StartDate = Convert.IsDBNull(row[col8]) ? DateTime.MinValue : Convert.ToDateTime(row[col8]);
                partprocessinfo.EndDate = Convert.IsDBNull(row[col9]) ? DateTime.MinValue : Convert.ToDateTime(row[col9]);
                partprocessinfo.NeedProduct = Convert.ToInt32(row[col10]);
                partprocessinfo.Priority = Convert.ToInt32(row[col11]);
                partprocessinfo.FactStartDate = Convert.IsDBNull(row[col12]) ? DateTime.MinValue : Convert.ToDateTime(row[col12]);
                partprocessinfo.FactEndDate = Convert.IsDBNull(row[col13]) ? DateTime.MinValue : Convert.ToDateTime(row[col13]);
                partprocessinfo.FactProcessNeedMinutes = Convert.IsDBNull(row[col14]) ? 0 : Convert.ToInt32(row[col14]);
                partprocessinfo.MachineId = Convert.IsDBNull(row[col15]) ? null : row[col15].ToString();
                partprocessinfo.ProcessNo = Convert.ToInt32(row["processno"]);
                string key = "[" + partprocessinfo.ModuleId + "][" + partprocessinfo.PartNo_Id + "][" + partprocessinfo.ProcessId + "][" + partprocessinfo.ProcessNo + "]";
                hPartProcess.Add(key, partprocessinfo);
            }

            return hPartProcess;
        }

        public static Hashtable getPartProcessByPrimaryKey(string processid)
        {
            string queryString = "select * from tb_partprocess where processid='" + processid + "'";
            DataSet dataSet = Data.getDataSet(queryString);
            Hashtable hPartProcess = new Hashtable();
            DataTable dt = new DataTable();
            dt = dataSet.Tables[0];

            DataColumn col1 = dt.Columns["moduleid"];
            DataColumn col2 = dt.Columns["partno"];
            DataColumn col3 = dt.Columns["partno_id"];
            DataColumn col4 = dt.Columns["processneedminutes"];
            DataColumn col5 = dt.Columns["processid"];
            DataColumn col6 = dt.Columns["statusid"];
            DataColumn col7 = dt.Columns["processorder"];
            DataColumn col8 = dt.Columns["startdate"];
            DataColumn col9 = dt.Columns["enddate"];
            DataColumn col10 = dt.Columns["needproduct"];
            DataColumn col11 = dt.Columns["priority"];
            DataColumn col12 = dt.Columns["factstartdate"];
            DataColumn col13 = dt.Columns["factenddate"];
            DataColumn col14 = dt.Columns["factprocessneedminutes"];
            DataColumn col15 = dt.Columns["machineid"];

            foreach (DataRow row in dt.Rows)
            {
                PartProcessInfo partprocessinfo = new PartProcessInfo();
                partprocessinfo.ModuleId = row[col1].ToString();
                partprocessinfo.PartNo = row[col2].ToString();
                partprocessinfo.PartNo_Id = row[col3].ToString();
                partprocessinfo.ProcessNeedMinutes = Convert.ToInt32(row[col4]);
                partprocessinfo.ProcessId = row[col5].ToString();
                partprocessinfo.StatusId = row[col6].ToString();
                partprocessinfo.ProcessOrder = Convert.ToInt32(row[col7]);
                partprocessinfo.StartDate = Convert.IsDBNull(row[col8]) ? DateTime.MinValue : Convert.ToDateTime(row[col8]);
                partprocessinfo.EndDate = Convert.IsDBNull(row[col9]) ? DateTime.MinValue : Convert.ToDateTime(row[col9]);
                partprocessinfo.NeedProduct = Convert.ToInt32(row[col10]);
                partprocessinfo.Priority = Convert.ToInt32(row[col11]);
                partprocessinfo.FactStartDate = Convert.IsDBNull(row[col12]) ? DateTime.MinValue : Convert.ToDateTime(row[col12]);
                partprocessinfo.FactEndDate = Convert.IsDBNull(row[col13]) ? DateTime.MinValue : Convert.ToDateTime(row[col13]);
                partprocessinfo.FactProcessNeedMinutes = Convert.IsDBNull(row[col14]) ? 0 : Convert.ToInt32(row[col14]);
                partprocessinfo.MachineId = Convert.IsDBNull(row[col15]) ? null : row[col15].ToString();
                partprocessinfo.ProcessNo = Convert.ToInt32(row["processno"]);
                string key = "[" + partprocessinfo.ProcessNo + "]";
                hPartProcess.Add(key, partprocessinfo);
            }

            return hPartProcess;
        }


        public static ArrayList getLParts(string moduleId)
        {
            ArrayList lParts = new ArrayList();
            string queryString = "select partno_id from tb_partprocess where ModuleId=@moduleId group by partno_id";
            SqlParameter[] pars = new SqlParameter[1];
            pars[0] = new SqlParameter("@moduleId", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleId;
            DataSet dataSet = Data.getDataSet(queryString, pars);
            DataTable dt = new DataTable();
            dt = dataSet.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                lParts.Add(Convert.ToString(dr["PartNo_Id"]).Trim());
            }

            return lParts;
        }

        public static DataSet getPartsForMachineStatusAdd(string moduleId, string processid)
        {
            string queryString = "select moduleid,partno_id,Priority,priorityname from tb_partprocess inner join tb_priority on (tb_priority.priorityid=tb_partprocess.priority) " +
                " where ModuleId=@moduleId and processid=@processid and (factenddate is null and factouttime is null) group by moduleid,partno_id,Priority,priorityname order by partno_id";
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleId", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleId;
            pars[1] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
            pars[1].Value = processid;
            DataSet dset = Data.getDataSet(queryString, pars);
            DataTable dtable = dset.Tables[0];
            return Data.getDataSet(queryString, pars);
        }

        public static DataSet getPartsExceptDesign(string moduleId, string partnoid)
        {
             

            string queryString = "select tb_partprocess.moduleid,tb_partprocess.Priority,partno_id,priorityname,LeastCount=0,ProcessOrder=0 from tb_partprocess inner join tb_priority on (tb_priority.priorityid=tb_partprocess.priority) "
                    + "inner join tb_part on (tb_partprocess.moduleId=tb_part.moduleId and tb_partprocess.partno=tb_part.partno)  where tb_partprocess.ModuleId like '%" + moduleId + "%' and DesignProcess is null and tb_partprocess.eachBatchCount>0  and process is not null ";
                if (partnoid != null && !partnoid.Equals(""))
                    queryString = queryString + " and tb_partprocess.partno_id like '%" + partnoid + "%'";

                queryString = queryString + " group by tb_partprocess.moduleid,partno_id,tb_partprocess.Priority,priorityname order by partno_id";
            
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleId", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleId;
            pars[1] = new SqlParameter("@partnoid", SqlDbType.NVarChar, 50);
            pars[1].Value = partnoid;

            DataSet dset = Data.getDataSet(queryString, pars);
            DataTable dtable = dset.Tables[0];
            return Data.getDataSet(queryString, pars);
        }

        public static DataSet getPartsDesign(string moduleId, string partnoid)
        {
            string queryString = "select tb_partprocess.moduleid,partno_id,tb_partprocess.Priority,priorityname from tb_partprocess inner join tb_priority on (tb_priority.priorityid=tb_partprocess.priority) "
                    + "inner join tb_part on (tb_partprocess.moduleId=tb_part.moduleId and tb_partprocess.partno=tb_part.partno)  where tb_partprocess.ModuleId like '%" + moduleId + "%' and DesignProcess is not null";
            if (partnoid != null && !partnoid.Equals(""))
                queryString = queryString + " and tb_partprocess.partno_id like '%" + partnoid + "%'";

            queryString = queryString + " group by tb_partprocess.moduleid,partno_id,tb_partprocess.Priority,priorityname order by partno_id";

            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleId", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleId;
            pars[1] = new SqlParameter("@partnoid", SqlDbType.NVarChar, 50);
            pars[1].Value = partnoid;

            DataSet dset = Data.getDataSet(queryString, pars);
            DataTable dtable = dset.Tables[0];
            return Data.getDataSet(queryString, pars);
        }

        public static DataSet getProcessOrderByID(string moduleid, string partnoid, string processid)
        {
            string querystr = "select processorder from tb_partprocess where moduleid=@moduleid and partno_id=@partnoid and processid=@processid and factenddate is null";
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@moduleId", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partnoid", SqlDbType.NVarChar, 50);
            pars[1].Value = partnoid;
            pars[2] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
            pars[2].Value = processid;

            return Data.getDataSet(querystr, pars);
        }


        public static Hashtable getNeedTime(string moduleId, string processId)
        {
            string queryString = "select * from tb_partprocess where ModuleId=@moduleId and processId=@processId";
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleId", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleId;
            pars[1] = new SqlParameter("@processId", SqlDbType.NVarChar, 50);
            pars[1].Value = processId;
            DataSet dataSet = Data.getDataSet(queryString, pars);
            Hashtable hNeedMinutes = new Hashtable();
            DataTable dt = new DataTable();
            dt = dataSet.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                hNeedMinutes.Add(Convert.ToString(dr["PartNo_Id"]).Trim(), Convert.ToInt32(dr["ProcessNeedMinutes"]));
            }

            return hNeedMinutes;

        }

        public static ArrayList getPartDatas(string moduleId, string partno, string processId)
        {
            string queryString = "select * from tb_partprocess where ModuleId=@moduleId and partno=@partno and processId=@processId";
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@moduleId", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleId;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = partno;
            pars[2] = new SqlParameter("@processId", SqlDbType.NVarChar, 50);
            pars[2].Value = processId;
            DataSet dataSet = Data.getDataSet(queryString, pars);
            ArrayList LPartDatas = new ArrayList();
            DataTable dt = new DataTable();
            dt = dataSet.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                LPartDatas.Add(new PartData(Convert.ToString(dr["PartNo_Id"]).Trim(), Convert.ToInt32(dr["ProcessNeedMinutes"]), Convert.ToInt32(dr["NeedProduct"])));
            }

            return LPartDatas;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int DuplicateKeys()
        {
            string queryString = "select count(*) as recordnum from tb_partprocess where moduleid=@moduleid and partno=@partno and processid=@processid";
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = this.ModuleId;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = this.PartNo;
            pars[2] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
            pars[2].Value = this.ProcessId;
            return Convert.ToInt32(Data.getDataScalar(queryString, pars).ToString());
        }

        public int Create()
        {
            string querystr = " Insert into tb_partprocess (ModuleId,PartNo,ProcessId,PartNo_Id,StatusId,ProcessNeedMinutes,ProcessOrder,priority,startdate,enddate, middlestartdate,middleenddate,importantid,totalminutes,selfStartDate,selfEndDate,DesignFinish,isConfirm,processComment,processMachineid) " +
                            " values (@ModuleId,@PartNo,@ProcessId,@PartNo_Id,@StatusId,@ProcessNeedMinutes,@ProcessOrder,@priority,@startdate,@enddate,@middlestartdate,@middleenddate,@importantid,@totalminutes,@selfStartDate,@selfEndDate,@DesignFinish,@isConfirm,@processComment,@processMachineid)" +
                            " select @@ROWCOUNT as AffectedRows";
            SqlParameter[] pars = new SqlParameter[20];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = this.ModuleId;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = this.PartNo;
            pars[2] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
            pars[2].Value = this.ProcessId;
            pars[3] = new SqlParameter("@partno_id", SqlDbType.NVarChar, 50);
            pars[3].Value = this.PartNo_Id;
            pars[4] = new SqlParameter("@statusid", SqlDbType.NVarChar, 50);
            pars[4].Value = this.SatusId;
            pars[5] = new SqlParameter("@processneedminutes", SqlDbType.Int, 4);
            pars[5].Value = this.ProcessNeedMinutes;
            pars[6] = new SqlParameter("@processorder", SqlDbType.Int, 4);
            pars[6].Value = this.ProcessOrder;
            pars[7] = new SqlParameter("@priority", SqlDbType.Int, 4);
            pars[7].Value = priority;
            pars[8] = new SqlParameter("@startdate", SqlDbType.DateTime);
            pars[8].Value = null;
            if (this.StartDate.Ticks != 0) pars[8].Value = this.StartDate;
            pars[9] = new SqlParameter("@enddate", SqlDbType.DateTime);
            pars[9].Value = null;
            if (this.EndDate.Ticks != 0) pars[9].Value = this.EndDate;
            pars[10] = new SqlParameter("@middlestartdate", SqlDbType.DateTime);
            pars[10].Value = null;
            if (this.middlestartdate.Ticks != 0) pars[10].Value = this.middlestartdate;
            pars[11] = new SqlParameter("@middleenddate", SqlDbType.DateTime);
            pars[11].Value = null;
            if (this.middleenddate.Ticks != 0) pars[11].Value = this.middleenddate;

            pars[12] = new SqlParameter("@importantid", SqlDbType.Int);
            pars[12].Value = ModuleWorkFlow.business.Part.getPartImportantId(this.PartNo);
            pars[13] = new SqlParameter("@totalminutes", SqlDbType.Int);
            pars[13].Value = this._totalminutes;
            pars[14] = new SqlParameter("@selfStartDate", SqlDbType.DateTime);
            pars[14].Value = null;
            if (this.selfStartDate.Ticks != 0) pars[14].Value = this.selfStartDate;

            pars[15] = new SqlParameter("@selfEndDate", SqlDbType.DateTime);
            pars[15].Value = null;
            if (this.selfEndDate.Ticks != 0)
                pars[15].Value = this.selfEndDate;

            pars[16] = new SqlParameter("@DesignFinish", SqlDbType.Int);
            pars[16].Value = this.IsDesign;

            pars[17] = new SqlParameter("@isConfirm", SqlDbType.Int);
            pars[17].Value = this.IsConfirm;

            pars[18] = new SqlParameter("@processComment", SqlDbType.NVarChar);
            pars[18].Value = this.ProcessComment;

            pars[19] = new SqlParameter("@processMachineid", SqlDbType.NVarChar);
            pars[19].Value = this.ProcessMachineid;

            Object o = Data.getDataScalar(querystr, pars);
            if (o == null)
            {
                return -1;
            }
            else
            {
                return (int)o;
            }
        }

        public int CreateDFMDesignPartProcess()
        {
            string querystr = " Insert into tb_DesignPartProcess (ModuleId,PartNo,ProcessId,PartNo_Id,StatusId,ProcessNeedMinutes,ProcessOrder,priority,startdate,enddate,totalminutes,selfStartDate,selfEndDate,partprocessid,DFMModuleId,category,DesignReasonId,assigned ) " +
                " values (@ModuleId,@PartNo,@ProcessId,@PartNo_Id,@StatusId,@ProcessNeedMinutes,@ProcessOrder,@priority,@startdate,@enddate,@totalminutes,@selfStartDate,@selfEndDate,@partprocessid,@DFMModuleId,@category,@DesignReasonId,@assigned )" +
                " select @@ROWCOUNT as AffectedRows";
            SqlParameter[] pars = new SqlParameter[18];


            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = this.ModuleId;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = this.PartNo;
            pars[2] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
            pars[2].Value = this.ProcessId;
            pars[3] = new SqlParameter("@partno_id", SqlDbType.NVarChar, 50);
            pars[3].Value = this.PartNo_Id;
            pars[4] = new SqlParameter("@statusid", SqlDbType.NVarChar, 50);
            pars[4].Value = this.SatusId;
            pars[5] = new SqlParameter("@processneedminutes", SqlDbType.Int, 4);
            pars[5].Value = this.ProcessNeedMinutes;
            pars[6] = new SqlParameter("@processorder", SqlDbType.Int, 4);
            pars[6].Value = this.ProcessOrder;
            pars[7] = new SqlParameter("@priority", SqlDbType.Int, 4);
            pars[7].Value = priority;
            pars[8] = new SqlParameter("@startdate", SqlDbType.DateTime);
            pars[8].Value = null;
            if (this.StartDate.Ticks != 0) pars[8].Value = this.StartDate;
            pars[9] = new SqlParameter("@enddate", SqlDbType.DateTime);
            pars[9].Value = null;
            if (this.EndDate.Ticks != 0) pars[9].Value = this.EndDate;
            pars[10] = new SqlParameter("@totalminutes", SqlDbType.Int);
            pars[10].Value = this._totalminutes;
            pars[11] = new SqlParameter("@selfStartDate", SqlDbType.DateTime);
            pars[11].Value = null;
            if (this.selfStartDate.Ticks != 0) pars[11].Value = this.selfStartDate;

            pars[12] = new SqlParameter("@selfEndDate", SqlDbType.DateTime);
            pars[12].Value = null;
            if (this.selfEndDate.Ticks != 0) pars[12].Value = this.selfEndDate;
            pars[13] = new SqlParameter("@partprocessid", SqlDbType.NVarChar, 50);
            pars[13].Value = this.PartProcessId;

            pars[14] = new SqlParameter("@DFMModuleId", SqlDbType.NVarChar, 50);
            pars[14].Value = this.ModuleId;

            pars[15] = new SqlParameter("@category", SqlDbType.NVarChar, 200);
            pars[15].Value = this.Category;

            pars[16] = new SqlParameter("@DesignReasonId", SqlDbType.Int, 4);
            pars[16].Value = this.DesignReasonId;

            pars[17] = new SqlParameter("@assigned", SqlDbType.NVarChar, 200);
            pars[17].Value = this.Assigned;

            Object o = Data.getDataScalar(querystr, pars);
            if (o == null)
            {
                return -1;
            }
            else
            {
                return (int)o;
            }
        }



        public int CreateDesignPartProcess()
        {
            string querystr = " Insert into tb_DesignPartProcess (ModuleId,PartNo,ProcessId,PartNo_Id,StatusId,ProcessNeedMinutes,ProcessOrder,priority,startdate,enddate,totalminutes,selfStartDate,selfEndDate,partprocessid,finishedPercent,assigned,responsPercent ) " +
                " values (@ModuleId,@PartNo,@ProcessId,@PartNo_Id,@StatusId,@ProcessNeedMinutes,@ProcessOrder,@priority,@startdate,@enddate,@totalminutes,@selfStartDate,@selfEndDate,@partprocessid,@finishedPercent,@assigned,@responsPercent )" +
                " select @@ROWCOUNT as AffectedRows";
            SqlParameter[] pars = new SqlParameter[17];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = this.ModuleId;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = this.PartNo;
            pars[2] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
            pars[2].Value = this.ProcessId;
            pars[3] = new SqlParameter("@partno_id", SqlDbType.NVarChar, 50);
            pars[3].Value = this.PartNo_Id;
            pars[4] = new SqlParameter("@statusid", SqlDbType.NVarChar, 50);
            pars[4].Value = this.SatusId;
            pars[5] = new SqlParameter("@processneedminutes", SqlDbType.Int, 4);
            pars[5].Value = this.ProcessNeedMinutes;
            pars[6] = new SqlParameter("@processorder", SqlDbType.Int, 4);
            pars[6].Value = this.ProcessOrder;
            pars[7] = new SqlParameter("@priority", SqlDbType.Int, 4);
            pars[7].Value = priority;
            pars[8] = new SqlParameter("@startdate", SqlDbType.DateTime);
            pars[8].Value = null;
            if (this.StartDate.Ticks != 0) pars[8].Value = this.StartDate;
            pars[9] = new SqlParameter("@enddate", SqlDbType.DateTime);
            pars[9].Value = null;
            if (this.EndDate.Ticks != 0) pars[9].Value = this.EndDate;
            pars[10] = new SqlParameter("@totalminutes", SqlDbType.Int);
            pars[10].Value = this._totalminutes;
            pars[11] = new SqlParameter("@selfStartDate", SqlDbType.DateTime);
            pars[11].Value = null;
            if (this.selfStartDate.Ticks != 0) pars[11].Value = this.selfStartDate;

            pars[12] = new SqlParameter("@selfEndDate", SqlDbType.DateTime);
            pars[12].Value = null;
            if (this.selfEndDate.Ticks != 0) pars[12].Value = this.selfEndDate;
            pars[13] = new SqlParameter("@partprocessid", SqlDbType.NVarChar, 50);
            pars[13].Value = this.PartProcessId;
            pars[14] = new SqlParameter("@finishedPercent", SqlDbType.Float, 8);
            pars[14].Value = this.finishedPercent;
            pars[15] = new SqlParameter("@assigned", SqlDbType.NVarChar, 50);
            pars[15].Value = this.Assigned;
            pars[16] = new SqlParameter("@responsPercent", SqlDbType.NVarChar, 250);
            pars[16].Value = this._responsPercent;
            Object o = Data.getDataScalar(querystr, pars);
            if (o == null)
            {
                return -1;
            }
            else
            {
                return (int)o;
            }
        }

        public int CreateElectrode()
        {
            string strsql = " Insert into tb_partprocess (ModuleId,PartNo,ProcessId,PartNo_Id,StatusId,StartDate,ProcessNeedMinutes,ProcessOrder,priority,isElectrode,selfStartDate,selfEndDate,RelationPartNo,RelationProcessNo) " +
                          " values (@ModuleId,@PartNo,@ProcessId,@PartNo_Id,@StatusId,@StartDate,@ProcessNeedMinutes,@ProcessOrder,@priority,1,@selfStartDate,@selfEndDate,@RelationPartNo,@RelationProcessNo)" +
                          " select @@ROWCOUNT as AffectedRows";
            SqlParameter[] pars = new SqlParameter[13];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = this.ModuleId;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = this.PartNo;
            pars[2] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
            pars[2].Value = this.ProcessId;
            pars[3] = new SqlParameter("@partno_id", SqlDbType.NVarChar, 50);
            pars[3].Value = this.PartNo_Id;
            pars[4] = new SqlParameter("@statusid", SqlDbType.NVarChar, 50);
            pars[4].Value = this.SatusId;
            pars[5] = new SqlParameter("@processneedminutes", SqlDbType.Int, 4);
            pars[5].Value = this.ProcessNeedMinutes;
            pars[6] = new SqlParameter("@processorder", SqlDbType.Int, 4);
            pars[6].Value = this.ProcessOrder;
            pars[7] = new SqlParameter("@priority", SqlDbType.Int, 4);
            pars[7].Value = priority;
            pars[8] = new SqlParameter("@startDate", SqlDbType.DateTime);
            if (this.StartDate.Ticks != 0) pars[8].Value = StartDate;
            pars[9] = new SqlParameter("@selfStartDate", SqlDbType.DateTime);
            pars[9].Value = null;
            if (this.selfStartDate.Ticks != 0) pars[9].Value = this.selfStartDate;

            pars[10] = new SqlParameter("@selfEndDate", SqlDbType.DateTime);
            pars[10].Value = null;
            if (this.selfEndDate.Ticks != 0) pars[10].Value = this.selfEndDate;
            pars[11] = new SqlParameter("@RelationPartNo", SqlDbType.NVarChar, 50);
            pars[11].Value = this.RelationPartNo;
            pars[12] = new SqlParameter("@RelationProcessNo", SqlDbType.NVarChar, 50);
            pars[12].Value = this.RelationProcessNo;

            Object o = Data.getDataScalar(strsql, pars);
            if (o == null)
            {
                return -1;
            }
            else
            {
                return (int)o;
            }
        }

        public int UpdatePartProcessRelation()
        {
            string strsql = " update tb_partprocess set RelationPartNo=RelationPartNo+','+@RelationPartNo,RelationProcessNo=@RelationProcessNo where processid=@processid and partno=@partno and processorder=@processorder and moduleid=@moduleid ";
            string querystring = " select * from tb_partprocess where processid='" + this.RelationProcessNo + "' and partno='" + this.RelationPartNo + "' and processorder=" + this.RelationOrder + " and RelationPartNo is null and moduleid='" + this.ModuleId + "' ";
            DataSet ds = Data.getDataSet(querystring);

            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                strsql = " update tb_partprocess set RelationPartNo=@RelationPartNo,RelationProcessNo=@RelationProcessNo where processid=@processid and partno=@partno and processorder=@processorder and moduleid=@moduleid ";
            }
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
            pars[0].Value = this.RelationProcessNo;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = this.RelationPartNo;
            pars[2] = new SqlParameter("@RelationPartNo", SqlDbType.NVarChar, 50);
            pars[2].Value = this.PartNo;
            pars[3] = new SqlParameter("@RelationProcessNo", SqlDbType.NVarChar, 50);
            pars[3].Value = this.ProcessId;
            pars[4] = new SqlParameter("@processorder", SqlDbType.Int, 4);
            pars[4].Value = this.RelationOrder;
            pars[5] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[5].Value = this.ModuleId;

            Object o = Data.getDataScalar(strsql, pars);
            if (o == null)
            {
                return -1;
            }
            else
            {
                return (int)o;
            }
        }

        public int Remove()
        {
            string procedureString = "sp_RemovePartProcess";
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = this.ModuleId;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = this.PartNo;
            pars[2] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
            pars[2].Value = this.ProcessId;
            Object o = Data.getDataScalar(procedureString, CommandType.StoredProcedure, pars);
            if (o == null)
            {
                return -1;
            }
            else
            {
                return (int)o;
            }

        }

        public void initOutSourcing()
        {
            string procedureString = "update tb_partprocess set needproduct = 1 where moduleid=@moduleid";
            SqlParameter[] pars = new SqlParameter[1];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = this.ModuleId;
            Data.ExecuteNonQuery(procedureString, pars);
        }

        public int updateProcessMins()
        {
            string querystr = " update tb_partprocess set " +
                            " ProcessNeedMinutes=@ProcessNeedMinutes " +
                            " where ModuleId=@ModuleId and ProcessOrder=@ProcessOrder and PartNo_Id=@PartNo_Id" +
                            " select @@ROWCOUNT as AffectedRows";
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = this.ModuleId;
            pars[1] = new SqlParameter("@partno_id", SqlDbType.NVarChar, 50);
            pars[1].Value = this.PartNo_Id;
            pars[2] = new SqlParameter("@processneedminutes", SqlDbType.Int, 4);
            pars[2].Value = this.ProcessNeedMinutes;
            pars[3] = new SqlParameter("@processorder", SqlDbType.Int, 4);
            pars[3].Value = this.ProcessOrder;


            Object o = Data.getDataScalar(querystr, pars);
            if (o == null)
            {
                return -1;
            }
            else
            {
                return (int)o;
            }
        }

        public int updateDesignProcessMins()
        {
            string querystr = " update tb_DesignPartProcess set " +
                " ProcessNeedMinutes=@ProcessNeedMinutes ," +
                " PartProcessId=@partprocessid ,SelfStartDate=@SelfStartDate,SelfEndDate=@SelfEndDate " +
                " where ModuleId=@ModuleId and ProcessOrder=@ProcessOrder and PartNo_Id=@PartNo_Id" +
                " select @@ROWCOUNT as AffectedRows";
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = this.ModuleId;
            pars[1] = new SqlParameter("@partno_id", SqlDbType.NVarChar, 50);
            pars[1].Value = this.PartNo_Id;
            pars[2] = new SqlParameter("@processneedminutes", SqlDbType.Int, 4);
            pars[2].Value = this.ProcessNeedMinutes;
            pars[3] = new SqlParameter("@processorder", SqlDbType.Int, 4);
            pars[3].Value = this.ProcessOrder;
            pars[4] = new SqlParameter("@partprocessid", SqlDbType.NVarChar, 50);
            pars[4].Value = this.PartProcessId;

            pars[5] = new SqlParameter("@SelfStartDate", SqlDbType.DateTime);
            pars[5].Value = this.selfStartDate;
            if (this.selfStartDate.Ticks == 0) pars[5].Value = null;
            pars[6] = new SqlParameter("@SelfEndDate", SqlDbType.DateTime);
            pars[6].Value = this.selfEndDate;
            if (this.selfEndDate.Ticks == 0) pars[6].Value = null;
            Object o = Data.getDataScalar(querystr, pars);
            if (o == null)
            {
                return -1;
            }
            else
            {
                return (int)o;
            }
        }

        public int updatePriority(string moduleid, string partnoid, int priority)
        {
            string querystr = " update tb_partprocess set " +
                " priority=@priority " +
                " where ModuleId=@ModuleId and PartNo_Id=@PartNo_Id" +
                " select @@ROWCOUNT as AffectedRows";
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno_id", SqlDbType.NVarChar, 50);
            pars[1].Value = partnoid;
            pars[2] = new SqlParameter("@priority", SqlDbType.Int, 4);
            pars[2].Value = priority;


            Object o = Data.getDataScalar(querystr, pars);
            if (o == null)
            {
                return -1;
            }
            else
            {
                return (int)o;
            }
        }

        public static PartProcessInfo getPartProcessInfo(PartProcessInfo ppi)
        {
            string moduleid = ppi.ModuleId;
            string partno_id = ppi.PartNo_Id;
            int processorder = ppi.ProcessOrder;
            string queryString = "select * from tb_partprocess where moduleid=@moduleid and partno_id=@partno_id and processorder=@processorder";
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno_id", SqlDbType.NVarChar, 50);
            pars[1].Value = partno_id;
            pars[2] = new SqlParameter("@processorder", SqlDbType.Int);
            pars[2].Value = processorder;

            DataRow dr = Data.getDataRow(queryString, pars);
            PartProcessInfo nextppi = null;
            if (dr != null)
            {
                nextppi = new PartProcessInfo();
                nextppi.ModuleId = ppi.ModuleId;
                nextppi.ProcessId = dr["processid"].ToString();
                nextppi.PartNo = dr["partno"].ToString();
                nextppi.PartNo_Id = dr["partno_id"].ToString();
                nextppi.StatusId = dr["statusid"].ToString();
                nextppi.ProcessNeedMinutes = Convert.ToInt32(dr["ProcessNeedMinutes"]);
                if (!Convert.IsDBNull(dr["startDate"]))
                {
                    nextppi.StartDate = Convert.ToDateTime(dr["startDate"]);
                }
                if (!Convert.IsDBNull(dr["endDate"]))
                {
                    nextppi.EndDate = Convert.ToDateTime(dr["endDate"]);
                }
                if (!Convert.IsDBNull(dr["factstartDate"]))
                {
                    nextppi.FactStartDate = Convert.ToDateTime(dr["factstartdate"]);
                }
                if (!Convert.IsDBNull(dr["factenddate"]))
                {
                    nextppi.FactEndDate = Convert.ToDateTime(dr["factenddate"]);
                }

                nextppi.ProcessOrder = processorder;
            }

            return nextppi;
        }

        public int StoreStartDate()
        {
            string procedureString = "sp_StoreStartDatePartProcess";
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = this.ModuleId;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = this.PartNo;
            pars[2] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
            pars[2].Value = this.ProcessId;
            pars[3] = new SqlParameter("@startdate", SqlDbType.DateTime);
            pars[3].Value = this.StartDate;

            Object o = Data.getDataScalar(procedureString, CommandType.StoredProcedure, pars);
            if (o == null)
            {
                return -1;
            }
            else
            {
                return (int)o;
            }
        }

        public static DataSet getPartProcessViewForOutSourcing(string moduleid)
        {
            if (moduleid.Equals(""))
            {
                string queryString = "select * from tb_partprocess where needproduct=0 order by moduleid,partno,partno_id,processorder";
                return Data.getDataSet(queryString);
            }
            else
            {
                string queryString = "select * from tb_partprocess where needproduct=0 and moduleid=@moduleid order by moduleid,partno,partno_id,processorder";
                SqlParameter[] pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                pars[0].Value = moduleid;
                return Data.getDataSet(queryString, pars);
            }
        }

        public static DataSet getPartProcessViewForOutSourcingByCondition(string key)
        {
            string queryString = "select * from tb_partprocess where needproduct=0 and moduleid like '%" + key + "%' order by moduleid desc";
            return Data.getDataSet(queryString);
        }

        public static DataSet getPartProcessView(string moduleid)
        {
            if (moduleid.Equals(""))
            {
                string queryString = "select * from tb_partprocess order by moduleid,partno,partno_id,processorder";
                return Data.getDataSet(queryString);
            }
            else
            {
                string queryString = "select * from tb_partprocess where moduleid=@moduleid order by moduleid,partno,partno_id,processorder";
                SqlParameter[] pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                pars[0].Value = moduleid;
                return Data.getDataSet(queryString, pars);
            }
        }

        public static DataSet getPartProcessView(string moduleid, string partno)
        {
            string queryString = "select * from tb_partprocess where moduleid=@moduleid and partno=@partno order by processorder,partno_id";
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = partno;
            return Data.getDataSet(queryString, pars);
        }

        public static DataSet getPartProcessView(string moduleid, string partno, string partnoid)
        {
            string queryString = "select * from tb_partprocess where moduleid=@moduleid and partno=@partno and partno_id=@partnoid order by processorder";
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = partno;
            pars[2] = new SqlParameter("@partnoid", SqlDbType.NVarChar, 50);
            pars[2].Value = partnoid;
            return Data.getDataSet(queryString, pars);
        }

        public static DataSet getPartProcessViewByProcess(string moduleid, string partno, string partnoid, string processid)
        {
            string condstr = "";
            if (partnoid == null)
            {
                condstr = " where pp.moduleid=@moduleid and pp.partno=@partno ";
            }
            else
            {
                condstr = " where pp.moduleid=@moduleid and pp.partno_id=@partnoid ";
            }

            if (processid != null)
            {
                condstr += " and pp.processid=@processid ";
            }

            string queryString = "select pp.*, p.processname from tb_partprocess pp inner join tb_process p on (pp.processid=p.processid) " + condstr + " order by p.listorder, pp.partno_id";
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = partno;
            pars[2] = new SqlParameter("@partnoid", SqlDbType.NVarChar, 50);
            pars[2].Value = partnoid;
            pars[3] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
            pars[3].Value = processid;
            return Data.getDataSet(queryString, pars);
        }

        public static DataSet getPartProcessByProcessView(string moduleid, string partno)
        {
            string queryString = " select processorder,processid,min(startdate) startdate,max(enddate) enddate,processneedminutes processneedminutes from tb_partprocess " +
                               " where moduleid=@moduleid and partno=@partno and iselectrode=0 group by  processorder,processid,processneedminutes order by processorder";
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = partno;
            return Data.getDataSet(queryString, pars);
        }

        public static DataSet getDesignPartProcessByProcessView(string moduleid, string partno)
        {
            string queryString = " select processorder,processid,min(startdate) startdate,max(enddate) enddate,processneedminutes processneedminutes from tb_DesignPartProcess " +
                " where moduleid=@moduleid and partno=@partno and iselectrode=0 group by  processorder,processid,processneedminutes order by processorder";
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = partno;
            return Data.getDataSet(queryString, pars);
        }

        public static DataSet getPartIDProcessByProcessView(string moduleid, string partnoid)
        {
            string queryString = " select processorder,processid,startdate,enddate,processneedminutes from tb_partprocess " +
                " where moduleid=@moduleid and partno_id=@partnoid and iselectrode=0 order by processorder";
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partnoid", SqlDbType.NVarChar, 50);
            pars[1].Value = partnoid;
            return Data.getDataSet(queryString, pars);
        }

        public static DataSet getElectrodeByProcessView(string moduleid, string partno)
        {
            string queryString = " select tb_partprocess.processorder,tb_partprocess.processid,min(tb_partprocess.factstartdate) startdate,max(tb_partprocess.factenddate) enddate,sum(tb_partprocess.processneedminutes) processneedminutes from tb_partprocess " +
                " join tb_part on tb_part.moduleid = tb_partprocess.moduleid and tb_part.partno = tb_partprocess.partno " +
                " where tb_partprocess.moduleid=@moduleid and tb_partprocess.partno=@partno and tb_partprocess.iselectrode=1 group by  tb_partprocess.processorder,tb_partprocess.processid,tb_partprocess.processneedminutes order by tb_partprocess.processorder";
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = partno;
            return Data.getDataSet(queryString, pars);
        }

        public static DataSet getPartNoIdView(string moduleid, string partno)
        {
            string queryString = "select partno_id from tb_partprocess where moduleid=@moduleid and partno=@partno group by partno_id order by partno_id";
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = partno;
            return Data.getDataSet(queryString, pars);
        }

        public DataSet getPartNoIdViews(string moduleid, string partno)
        {
            string queryString = "select partno_id from tb_partprocess where moduleid=@moduleid and partno=@partno group by partno_id order by partno_id";
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = partno;
            return Data.getDataSet(queryString, pars);
        }

        public static DateTime getAssembleDate(ArrayList assembleParts, string moduleid)
        {
            string parts = "";
            for (int i = 0; i < assembleParts.Count; i++)
            {
                parts = "'" + assembleParts[i] + "',";
            }
            parts = parts.Substring(0, parts.Length - 1);
            string querystring = " select DateAdd(d,1,max(enddate)) as startdate from tb_partprocess " +
                                 " where moduleid='" + moduleid + "' and processid<> 'Assemble' and partno_id in (" + parts + ")";
            return Convert.ToDateTime(Data.getDataScalar(querystring));
        }

        public static void UpdateNextProcessFactStartDate(PartProcessInfo ppi)
        {
            string querystring = " update tb_partprocess set FactStartDate=@FactStartDate "
                + " where ModuleId=@ModuleId and ProcessId=@ProcessId and PartNo_Id=@PartNo_Id;";
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@FactStartDate", SqlDbType.DateTime);
            pars[0].Value = ppi.FactStartDate;

            pars[1] = new SqlParameter("@ModuleId", SqlDbType.NVarChar, 50);
            pars[1].Value = ppi.ModuleId;
            pars[2] = new SqlParameter("@ProcessId", SqlDbType.NVarChar, 50);
            pars[2].Value = ppi.ProcessId;
            pars[3] = new SqlParameter("@PartNo_Id", SqlDbType.NVarChar, 50);
            pars[3].Value = ppi.PartNo_Id;

            Data.getDataScalar(querystring, pars);
        }

        public static void UpdateNextProcess(PartProcessInfo ppi, DateTime prevenddate, int processminuts)
        {

            if (ppi.FactStartDate.Ticks == 0)
            {
                string changestatus = " ,statusid='Ready' ";
                int days = processminuts / (18 * 60);
                string querystring = " update tb_partprocess set StartDate=@StartDate,EndDate=@EndDate " + changestatus
                    + " where ModuleId=@ModuleId and PartNo_Id=@PartNo_Id and processorder=@processorder;";

                //edit by jeason 2005-07-12
                string partnoid = ppi.PartNo_Id;
                int num = Convert.ToInt32(partnoid.Substring(0, 3));
                if (num >= 721 && num <= 739 && !ppi.ProcessId.Equals("Assemble"))
                {
                    querystring = " update tb_partprocess set StartDate=@StartDate,EndDate=@EndDate " + changestatus
                        + " where ModuleId=@ModuleId and processorder=@processorder and cast(left(partno_id,3) as int) =" + num + ";"
                        + " select @@ROWCOUNT as AffectedRows";
                }

                SqlParameter[] pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@StartDate", SqlDbType.DateTime);
                //pars[0].Value = prevenddate.AddDays(1);
                pars[0].Value = DateTime.Now;
                pars[1] = new SqlParameter("@ModuleId", SqlDbType.NVarChar, 50);
                pars[1].Value = ppi.ModuleId;
                pars[2] = new SqlParameter("@PartNo_Id", SqlDbType.NVarChar, 50);
                pars[2].Value = ppi.PartNo_Id;
                pars[3] = new SqlParameter("@processorder", SqlDbType.Int);
                pars[3].Value = ppi.ProcessOrder + 1;
                pars[4] = new SqlParameter("@EndDate", SqlDbType.DateTime);
                //pars[4].Value=prevenddate.AddDays(1+days);
                pars[4].Value = DateTime.Now.AddDays(days);
                Data.getDataScalar(querystring, pars);
            }
        }

        public static int updateNeedMinutesByModuleIdAssemble(string moduleid, int needminutes)
        {
            //			int retVal = 0;
            //
            //			SqlParameter[] pars = new SqlParameter[2];
            //			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
            //			pars[0].Value = moduleid;
            //			pars[1] = new SqlParameter("@needminutes", SqlDbType.Int);
            //			pars[1].Value = needminutes;
            //
            //			//clear tb_part table assemble process
            //			string strsql = "update tb_part set process=REPLACE(process,'/Assemble','') where moduleid=@moduleid";
            //			Data.ExecuteNonQuery(strsql,pars);
            //			//clear tb_partprocess table assemble process
            //			strsql = "delete from tb_partprocess where moduleid=@moduleid and processid='Assemble'";
            //			Data.ExecuteNonQuery(strsql,pars);
            //
            //
            //			if (needminutes > 0)
            //			{
            //				strsql = "update tb_part set process=process +'/Assemble' where moduleid=@moduleid";
            //				Data.ExecuteNonQuery(strsql,pars);
            //
            //				DataSet ds = getMaxListOrder(moduleid);
            //				DataTable dt = ds.Tables[0];
            //
            //				ArrayList alsql = new ArrayList();
            //				ArrayList alpar = new ArrayList();
            //
            //				string queryString=" Insert into tb_partprocess (ModuleId,PartNo,ProcessId,PartNo_Id,StatusId,ProcessNeedMinutes,ProcessOrder,priority) "+
            //					" values (@ModuleId,@PartNo,@processid,@PartNo_Id,@StatusId,@ProcessNeedMinutes,@ProcessOrder,@priority)"+
            //					" select @@ROWCOUNT as AffectedRows";
            //				foreach(DataRow dr in dt.Rows)
            //				{
            //					pars = new SqlParameter[8];
            //					pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
            //					pars[0].Value = dr["moduleid"];
            //					pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
            //					pars[1].Value=dr["partno"];
            //					pars[2] = new SqlParameter("@processid", SqlDbType.NVarChar,50);
            //					pars[2].Value = "Assemble";
            //					pars[3]=new SqlParameter("@partno_id",SqlDbType.NVarChar,50);
            //					pars[3].Value=dr["partno_id"];
            //					pars[4] = new SqlParameter("@statusid", SqlDbType.NVarChar,50);
            //					pars[4].Value = "Pending";
            //					pars[5]=new SqlParameter("@processneedminutes",SqlDbType.Int,4);
            //					pars[5].Value=needminutes;
            //					pars[6] = new SqlParameter("@processorder", SqlDbType.Int,4);
            //					pars[6].Value = Convert.ToInt32(dr["processorder"])+1;
            //					pars[7] = new SqlParameter("@priority", SqlDbType.Int,4);
            //					pars[7].Value = Convert.ToInt32(dr["priority"]);
            //					alsql.Add(queryString);
            //					alpar.Add(pars);
            //				}
            //	            
            //				retVal = Convert.ToInt32(Data.excuteTrans(alsql,alpar));
            //			}
            //			return retVal;

            int retVal = 0;

            string querystr = "update tb_partprocess set ProcessNeedMinutes=@needminutes where moduleid=@moduleid and processid='Assemble'; select @@ROWCOUNT as AffectedRows";
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@needminutes", SqlDbType.Int);
            pars[1].Value = needminutes;

            retVal = Convert.ToInt32(Data.getDataScalar(querystr, pars));
            return retVal;
        }

        private static bool existedProcess(string moduleid, string processid)
        {
            string queryString = "select * from tb_partprocess where moduleid=@moduleid and processid=@processid;";
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
            pars[1].Value = processid;
            DataSet ds = Data.getDataSet(queryString, pars);

            return ds.Tables[0].Rows.Count > 0;
        }

        /// <summary>
        /// update partprocess ���ǡ�������̡�������š�����
        /// </summary>
        /// <param name="ppi"></param>
        public static void UpdatePartProcessOthers(PartProcessInfo ppi)
        {
            string querystring = " update tb_partprocess set priority=@priority,RelationPartNo =@RelationPartNo,RelationProcessNo=@RelationProcessNo "
                              + " where ModuleId=@moduleid and PartNo=@partno;";
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = ppi.ModuleId;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = ppi.PartNo;
            pars[2] = new SqlParameter("@priority", SqlDbType.Int);
            pars[2].Value = ppi.Priority;

            #region 080611 ����RelationProcessNo,RelationPartNo �ֶ�
            pars[3] = new SqlParameter("@RelationPartNo", SqlDbType.NVarChar, 50);
            pars[3].Value = ppi.RelationPartNo;
            pars[4] = new SqlParameter("@RelationProcessNo", SqlDbType.NVarChar, 50);
            pars[4].Value = ppi.RelationProcessNo;
            #endregion

            Data.getDataScalar(querystring, pars);
        }

        //�ж���������Ѿ���ʼ�����
        public bool getPartProcessfacdateisstart(string moduleid, string partno)
        {

            string querystring = "select * from tb_partprocess  where moduleid=@moduleid and partno =@partno and factStartDate is not null";
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = partno;

            DataSet ds = Data.getDataSet(querystring, pars);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private static DataSet getMaxListOrder(string moduleid)
        {
            string queryString = "select moduleid,partno,partno_id,priority, max(processorder) as processorder from tb_partprocess group by moduleid,partno,partno_id,priority";
            SqlParameter[] pars = new SqlParameter[1];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;

            return Data.getDataSet(queryString, pars);
        }

        public static DataSet getPartProcessWithNoMachineProcessForModule(string moduleid, string customerid, string processid)
        {
            if (moduleid != null)
            {
                string queryString = "select moduleid,min(startdate) as startdate,max(enddate) as enddate from tb_partprocess where processid=@processid and moduleid=@moduleid group by moduleid order by moduleid";
                SqlParameter[] pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                pars[0].Value = moduleid;
                pars[1] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                pars[1].Value = processid;
                return Data.getDataSet(queryString, pars);
            }
            else
            {
                if (customerid != null)
                {
                    string queryString = "select pp.moduleid,min(pp.startdate) as startdate,max(pp.enddate) as enddate from tb_partprocess pp inner join tb_order od on (pp.moduleid=od.moduleid) where pp.processid=@processid and od.customerid=@customerid group by pp.moduleid order by pp.moduleid";
                    SqlParameter[] pars = new SqlParameter[2];
                    pars[0] = new SqlParameter("@customerid", SqlDbType.NVarChar, 50);
                    pars[0].Value = customerid;
                    pars[1] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                    pars[1].Value = processid;
                    return Data.getDataSet(queryString, pars);
                }
                else
                {
                    string queryString = "select moduleid,min(startdate) as startdate,max(enddate) as enddate from tb_partprocess where processid=@processid group by moduleid order by moduleid";
                    SqlParameter[] pars = new SqlParameter[1];
                    pars[0] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                    pars[0].Value = processid;
                    return Data.getDataSet(queryString, pars);
                }
            }

        }

        public static DataSet getPartProcessWithNoMachineProcessForModulePartNoId(string moduleid, string customerid, string processid)
        {
            if (moduleid != null)
            {
                string queryString = "select moduleid,partno_id,min(startdate) as startdate,max(enddate) as enddate from tb_partprocess where processid=@processid and moduleid=@moduleid group by moduleid,partno_id order by moduleid";
                SqlParameter[] pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                pars[0].Value = moduleid;
                pars[1] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                pars[1].Value = processid;
                return Data.getDataSet(queryString, pars);
            }
            else
            {
                if (customerid != null)
                {
                    string queryString = "select pp.moduleid,pp.partno_id,min(pp.startdate) as startdate,max(pp.enddate) as enddate from tb_partprocess pp inner join tb_order od on (pp.moduleid=od.moduleid) where pp.processid=@processid and od.customerid=@customerid group by pp.moduleid,pp.partno_id order by pp.moduleid";
                    SqlParameter[] pars = new SqlParameter[2];
                    pars[0] = new SqlParameter("@customerid", SqlDbType.NVarChar, 50);
                    pars[0].Value = customerid;
                    pars[1] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                    pars[1].Value = processid;
                    return Data.getDataSet(queryString, pars);
                }
                else
                {
                    string queryString = "select moduleid,partno_id,min(startdate) as startdate,max(enddate) as enddate from tb_partprocess where processid=@processid group by moduleid,partno_id order by moduleid";
                    SqlParameter[] pars = new SqlParameter[1];
                    pars[0] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                    pars[0].Value = processid;
                    return Data.getDataSet(queryString, pars);
                }
            }

        }

        public static DataSet getPartProcessWithNoMachineProcessNoGroupBy(string moduleid, string customerid, string processid)
        {
            if (moduleid != null)
            {
                string queryString = "select * from tb_partprocess where processid=@processid and moduleid=@moduleid order by moduleid";
                SqlParameter[] pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                pars[0].Value = moduleid;
                pars[1] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                pars[1].Value = processid;
                return Data.getDataSet(queryString, pars);
            }
            else
            {
                if (customerid != null)
                {
                    string queryString = "select pp.* from tb_partprocess pp inner join tb_order od on (pp.moduleid=od.moduleid) where pp.processid=@processid and od.customerid=@customerid order by pp.moduleid";
                    SqlParameter[] pars = new SqlParameter[2];
                    pars[0] = new SqlParameter("@customerid", SqlDbType.NVarChar, 50);
                    pars[0].Value = customerid;
                    pars[1] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                    pars[1].Value = processid;
                    return Data.getDataSet(queryString, pars);
                }
                else
                {
                    string queryString = "select * from tb_partprocess where processid=@processid order by moduleid";
                    SqlParameter[] pars = new SqlParameter[1];
                    pars[0] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                    pars[0].Value = processid;
                    return Data.getDataSet(queryString, pars);
                }
            }

        }
        #region getProcessWithMachineProcess
        public static DataSet getProcessWithMachineProcess(string moduleid, string customerid, string processid, string customerprocessid, string machineid, DateTime dtStartDate, int days)
        {
            if (moduleid != null)
            {
                if (dtStartDate == DateTime.MinValue)
                {
                    if (customerprocessid != null)
                    {
                        string queryString = "select * from tb_process" + processid + " where processid=@processid and moduleid=@moduleid and " + processid + "ID=@machineid order by processdate,moduleid,partno_id";
                        SqlParameter[] pars = new SqlParameter[3];
                        pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                        pars[0].Value = moduleid;
                        pars[1] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                        pars[1].Value = machineid;
                        pars[2] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                        pars[2].Value = customerprocessid;
                        return Data.getDataSet(queryString, pars);
                    }
                    else
                    {
                        string queryString = "select * from tb_process" + processid + " where moduleid=@moduleid and " + processid + "ID=@machineid order by processdate,moduleid,partno_id";
                        SqlParameter[] pars = new SqlParameter[2];
                        pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                        pars[0].Value = moduleid;
                        pars[1] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                        pars[1].Value = machineid;
                        return Data.getDataSet(queryString, pars);
                    }
                }
                else
                {
                    if (customerprocessid != null)
                    {
                        string queryString = "select * from tb_process" + processid + " where processid=@processid and moduleid=@moduleid and processdate>=@startdate and processdate<=@enddate and " + processid + "ID=@machineid order by processdate,moduleid,partno_id";
                        SqlParameter[] pars = new SqlParameter[5];
                        pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                        pars[0].Value = moduleid;
                        pars[1] = new SqlParameter("@startdate", SqlDbType.DateTime);
                        pars[1].Value = dtStartDate;
                        pars[2] = new SqlParameter("@enddate", SqlDbType.DateTime);
                        pars[2].Value = dtStartDate.AddDays(days);
                        pars[3] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                        pars[3].Value = machineid;
                        pars[4] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                        pars[4].Value = customerprocessid;
                        return Data.getDataSet(queryString, pars);
                    }
                    else
                    {
                        string queryString = "select * from tb_process" + processid + " where moduleid=@moduleid and processdate>=@startdate and processdate<=@enddate and " + processid + "ID=@machineid order by processdate,moduleid,partno_id";
                        SqlParameter[] pars = new SqlParameter[4];
                        pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                        pars[0].Value = moduleid;
                        pars[1] = new SqlParameter("@startdate", SqlDbType.DateTime);
                        pars[1].Value = dtStartDate;
                        pars[2] = new SqlParameter("@enddate", SqlDbType.DateTime);
                        pars[2].Value = dtStartDate.AddDays(days);
                        pars[3] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                        pars[3].Value = machineid;
                        return Data.getDataSet(queryString, pars);
                    }
                }
            }
            else
            {
                if (customerid != null)
                {
                    if (dtStartDate == DateTime.MinValue)
                    {
                        if (customerprocessid != null)
                        {
                            string queryString = "select pp.* from tb_process" + processid + " pp left join tb_order od on (pp.moduleid=od.moduleid) where pp.processid=@processid and od.customerid=@customerid and pp." + processid + "ID=@machineid order by pp.processdate,pp.moduleid,pp.partno_id";
                            SqlParameter[] pars = new SqlParameter[3];
                            pars[0] = new SqlParameter("@customerid", SqlDbType.NVarChar, 50);
                            pars[0].Value = customerid;
                            pars[1] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                            pars[1].Value = machineid;
                            pars[2] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                            pars[2].Value = customerprocessid;
                            return Data.getDataSet(queryString, pars);
                        }
                        else
                        {
                            string queryString = "select pp.* from tb_process" + processid + " pp left join tb_order od on (pp.moduleid=od.moduleid) where od.customerid=@customerid and pp." + processid + "ID=@machineid order by pp.processdate,pp.moduleid,pp.partno_id";
                            SqlParameter[] pars = new SqlParameter[2];
                            pars[0] = new SqlParameter("@customerid", SqlDbType.NVarChar, 50);
                            pars[0].Value = customerid;
                            pars[1] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                            pars[1].Value = machineid;
                            return Data.getDataSet(queryString, pars);
                        }
                    }
                    else
                    {
                        if (customerprocessid != null)
                        {
                            string queryString = "select pp.* from tb_process" + processid + " pp left join tb_order od on (pp.moduleid=od.moduleid) where pp.processid=@processid and od.customerid=@customerid and pp.processdate>=@startdate and pp.processdate<=@enddate and pp." + processid + "ID=@machineid order by pp.processdate,pp.moduleid,pp.partno_id";
                            SqlParameter[] pars = new SqlParameter[5];
                            pars[0] = new SqlParameter("@customerid", SqlDbType.NVarChar, 50);
                            pars[0].Value = customerid;
                            pars[1] = new SqlParameter("@startdate", SqlDbType.DateTime);
                            pars[1].Value = dtStartDate;
                            pars[2] = new SqlParameter("@enddate", SqlDbType.DateTime);
                            pars[2].Value = dtStartDate.AddDays(days);
                            pars[3] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                            pars[3].Value = machineid;
                            pars[4] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                            pars[4].Value = customerprocessid;
                            return Data.getDataSet(queryString, pars);
                        }
                        else
                        {
                            string queryString = "select pp.* from tb_process" + processid + " pp left join tb_order od on (pp.moduleid=od.moduleid) where od.customerid=@customerid and pp.processdate>=@startdate and pp.processdate<=@enddate and pp." + processid + "ID=@machineid order by pp.processdate,pp.moduleid,pp.partno_id";
                            SqlParameter[] pars = new SqlParameter[4];
                            pars[0] = new SqlParameter("@customerid", SqlDbType.NVarChar, 50);
                            pars[0].Value = customerid;
                            pars[1] = new SqlParameter("@startdate", SqlDbType.DateTime);
                            pars[1].Value = dtStartDate;
                            pars[2] = new SqlParameter("@enddate", SqlDbType.DateTime);
                            pars[2].Value = dtStartDate.AddDays(days);
                            pars[3] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                            pars[3].Value = machineid;
                            return Data.getDataSet(queryString, pars);
                        }
                    }
                }
                else
                {
                    if (dtStartDate == DateTime.MinValue)
                    {
                        if (customerprocessid != null)
                        {
                            string queryString = "select * from tb_process" + processid + " where processid=@processid and " + processid + "ID=@machineid order by processdate,moduleid,partno_id";
                            SqlParameter[] pars = new SqlParameter[2];
                            pars[0] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                            pars[0].Value = machineid;
                            pars[1] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                            pars[1].Value = customerprocessid;
                            return Data.getDataSet(queryString, pars);
                        }
                        else
                        {
                            string queryString = "select * from tb_process" + processid + " where " + processid + "ID=@machineid order by processdate,moduleid,partno_id";
                            SqlParameter[] pars = new SqlParameter[1];
                            pars[0] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                            pars[0].Value = machineid;
                            return Data.getDataSet(queryString, pars);
                        }
                    }
                    else
                    {
                        if (customerprocessid != null)
                        {
                            string queryString = "select * from tb_process" + processid + " where processid=@processid and processdate>=@startdate and processdate<=@enddate and " + processid + "ID=@machineid order by processdate,moduleid,partno_id";
                            SqlParameter[] pars = new SqlParameter[4];
                            pars[0] = new SqlParameter("@startdate", SqlDbType.DateTime);
                            pars[0].Value = dtStartDate;
                            pars[1] = new SqlParameter("@enddate", SqlDbType.DateTime);
                            pars[1].Value = dtStartDate.AddDays(days);
                            pars[2] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                            pars[2].Value = machineid;
                            pars[3] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                            pars[3].Value = customerprocessid;
                            return Data.getDataSet(queryString, pars);
                        }
                        else
                        {
                            string queryString = "select * from tb_process" + processid + " where processdate>=@startdate and processdate<=@enddate and " + processid + "ID=@machineid order by processdate,moduleid,partno_id";
                            SqlParameter[] pars = new SqlParameter[3];
                            pars[0] = new SqlParameter("@startdate", SqlDbType.DateTime);
                            pars[0].Value = dtStartDate;
                            pars[1] = new SqlParameter("@enddate", SqlDbType.DateTime);
                            pars[1].Value = dtStartDate.AddDays(days);
                            pars[2] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                            pars[2].Value = machineid;
                            return Data.getDataSet(queryString, pars);
                        }
                    }
                }
            }

        }

        #endregion

        public static bool saveUnDoInfo(IList pp)
        {
            ArrayList alsqls = new ArrayList();
            ArrayList alpars = new ArrayList();
            for (int i = 0; i < pp.Count; i++)
            {
                PartProcessInfo ppi = (PartProcessInfo)pp[i];
                /*string querystring = "update tb_partprocess set factstartdate=@factstartdate,factenddate=@factenddate where moduleid=@moduleid and partno_id=@partno_id and processorder=@processorder";
				SqlParameter[] pars = new SqlParameter[6];
				pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
				pars[0].Value = ppi.ModuleId;
				pars[1] = new SqlParameter("@partno_id", SqlDbType.NVarChar,50);
				pars[1].Value = ppi.PartNo_Id;
				pars[2] = new SqlParameter("@processorder", SqlDbType.Int);
				pars[2].Value = ppi.ProcessOrder;
				pars[3] = new SqlParameter("@processneedminutes", SqlDbType.Int);
				pars[3].Value = ppi.RemaintneedMinutes;

				pars[4] = new SqlParameter("@factstartdate", SqlDbType.DateTime);
				pars[4].Value = null;
				if (ppi.FactStartDate.Ticks != DateTime.MinValue.Ticks) pars[4].Value = ppi.FactStartDate;

				pars[5] = new SqlParameter("@factenddate", SqlDbType.DateTime);
				pars[5].Value = null;
				if (ppi.FactEndDate.Ticks != DateTime.MinValue.Ticks) pars[5].Value = ppi.FactEndDate;
				*/
                string querystring = "update tb_partprocess set factstartdate=null,factenddate=null,statusid='Ready',factprocessneedminutes=0 where moduleid=@moduleid and partno_id=@partno_id and processorder=@processorder";
                SqlParameter[] pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                pars[0].Value = ppi.ModuleId;
                pars[1] = new SqlParameter("@partno_id", SqlDbType.NVarChar, 50);
                pars[1].Value = ppi.PartNo_Id;
                pars[2] = new SqlParameter("@processorder", SqlDbType.Int);
                pars[2].Value = ppi.ProcessOrder;
                pars[3] = new SqlParameter("@processneedminutes", SqlDbType.Int);
                pars[3].Value = ppi.RemaintneedMinutes;

                pars[4] = new SqlParameter("@factstartdate", SqlDbType.DateTime);
                pars[4].Value = null;
                if (ppi.FactStartDate.Ticks != DateTime.MinValue.Ticks) pars[4].Value = ppi.FactStartDate;

                pars[5] = new SqlParameter("@factenddate", SqlDbType.DateTime);
                pars[5].Value = null;
                if (ppi.FactEndDate.Ticks != DateTime.MinValue.Ticks) pars[5].Value = ppi.FactEndDate;

                alsqls.Add(querystring);
                alpars.Add(pars);
            }
            return Data.excuteTrans(alsqls, alpars);
        }

        #region ��z����ʾ���������X
        public static DataSet getMiddleScheduleViewWithModuleLevel(string moduleid, string processid)
        {
            string condstr = "";
            if (processid != null) condstr = " and pp.processid=@processid ";
            string querystr = " select pp.moduleid, pp.processid, p.processname, min(pp.middlestartdate) middlestartdate, max(pp.middleenddate) middleenddate,p.listorder" +
                              " from tb_partprocess pp inner join tb_process p on (pp.processid = p.processid) " +
                              " where pp.moduleid=@moduleid and pp.iselectrode=0 " + condstr +
                              " group by pp.moduleid,pp.processid,p.processname,p.listorder " +
                              " order by p.listorder";

            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
            pars[1].Value = processid;
            return Data.getDataSet(querystr, pars);
        }

        /// <summary>
        /// ���l��partprocess������middlestartdate,middleenddate����������������
        /// </summary>
        /// <param name="moduleid"></param>
        public static void initMiddleScheduleWithModuleLevel(string moduleid)
        {
            string querystr = "update tb_partprocess set middlestartdate=null, middleenddate=null where moduleid=@moduleid";
            SqlParameter[] pars = new SqlParameter[1];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            Data.getDataScalar(querystr, pars);
        }

        public static void updateOrderMiddleScheduledFlag(string moduleid, int int_flag)
        {
            string querystr = "update tb_order set middlescheduled = @flag where moduleid=@moduleid";
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@flag", SqlDbType.Int);
            pars[1].Value = int_flag;
            Data.getDataScalar(querystr, pars);
        }

        /// <summary>
        /// ���l��partprocess������middlestartdate,middleenddate����������������
        /// </summary>
        /// <param name="moduleid"></param>
        public static void initMiddleScheduleWithPartLevel(string moduleid, string partno)
        {
            string querystr = "update tb_partprocess set middlestartdate=null, middleenddate=null where moduleid=@moduleid and partno=@partno";
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = partno;
            Data.getDataScalar(querystr, pars);
        }

        public static void updateMiddleSchduleWithModuleLevel(string moduleid, string processid, DateTime middlestartdate, DateTime middleenddate)
        {
            string querystr = "update tb_partprocess set middlestartdate=@middlestartdate, middleenddate=@middleenddate where moduleid=@moduleid and processid=@processid";
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
            pars[1].Value = processid;
            pars[2] = new SqlParameter("@middlestartdate", SqlDbType.DateTime);
            pars[2].Value = middlestartdate;
            pars[3] = new SqlParameter("@middleenddate", SqlDbType.DateTime);
            pars[3].Value = middleenddate;

            Data.getDataScalar(querystr, pars);
        }

        public static void updateMiddleSchduleWithPartLevel(string moduleid, string processid, string partno, DateTime middlestartdate, DateTime middleenddate)
        {
            string querystr = "update tb_partprocess set middlestartdate=@middlestartdate, middleenddate=@middleenddate where moduleid=@moduleid and processid=@processid and partno=@partno";
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
            pars[1].Value = processid;
            pars[2] = new SqlParameter("@middlestartdate", SqlDbType.DateTime);
            pars[2].Value = middlestartdate;
            pars[3] = new SqlParameter("@middleenddate", SqlDbType.DateTime);
            pars[3].Value = middleenddate;
            pars[4] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[4].Value = partno;

            Data.getDataScalar(querystr, pars);
        }

        public static void updateMiddleSchduleWithPartLevel(string moduleid, string processid, string partno, string partnoid, DateTime middlestartdate, DateTime middleenddate)
        {
            string querystr = "update tb_partprocess set middlestartdate=@middlestartdate, middleenddate=@middleenddate where moduleid=@moduleid and processid=@processid and partno=@partno and partno_id=@partnoid";
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
            pars[1].Value = processid;
            pars[2] = new SqlParameter("@middlestartdate", SqlDbType.DateTime);
            pars[2].Value = middlestartdate;
            pars[3] = new SqlParameter("@middleenddate", SqlDbType.DateTime);
            pars[3].Value = middleenddate;
            pars[4] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[4].Value = partno;
            pars[5] = new SqlParameter("@partnoid", SqlDbType.NVarChar, 50);
            pars[5].Value = partnoid;

            Data.getDataScalar(querystr, pars);
        }

        #endregion

        #region ��z�s�����ơ�ɢ�������
        /// <summary>
        /// �s������ɢ��
        /// </summary>
        /// <param name="moduleid"></param>
        /// <param name="partno"></param>
        /// <param name="int_oldcount"></param>
        /// <param name="int_newcount"></param>
        /// <param name="modelevel"></param>
        /// <returns></returns>
        public static bool partcountAdd(string moduleid, string partno, int int_oldcount, int int_newcount, int modelevel)
        {
            bool retval = false;
            DataSet dspart = getPartProcessView(moduleid, partno, partno + "-1");
            DataTable dtpart = dspart.Tables[0];

            ArrayList alSql = new ArrayList();
            ArrayList alPars = new ArrayList();

            if (int_oldcount < int_newcount)
            {
                string querystrpart = "update tb_part set partcount=@partcount where moduleid=@moduleid and partno=@partno";
                SqlParameter[] parspart = new SqlParameter[3];
                parspart[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                parspart[0].Value = moduleid;
                parspart[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
                parspart[1].Value = partno;
                parspart[2] = new SqlParameter("@partcount", SqlDbType.Int);
                parspart[2].Value = int_newcount;
                alSql.Add(querystrpart);
                alPars.Add(parspart);
            }

            for (int i = int_oldcount + 1; i <= int_newcount; i++)
            {
                for (int n = 0; n < dtpart.Rows.Count; n++)
                {
                    string partnoid = partno + "-" + i;
                    string processid = dtpart.Rows[n]["processid"].ToString();
                    string statusid = "Pending";
                    if (n == 0)
                        statusid = "Ready";
                    int processneedminutes = Convert.ToInt32(dtpart.Rows[n]["processneedminutes"]);
                    int processorder = Convert.ToInt32(dtpart.Rows[n]["processorder"]);
                    DateTime startdate = DateTime.Now;
                    DateTime enddate = DateTime.Now;
                    int priority = Convert.ToInt32(dtpart.Rows[n]["priority"]);
                    int iselectrode = Convert.ToInt32(dtpart.Rows[n]["iselectrode"]);
                    DateTime middlestartdate = DateTime.MinValue;
                    DateTime middleenddate = DateTime.MinValue;
                    if (modelevel == 1 || modelevel == 2)
                    {
                        if (!Convert.IsDBNull(dtpart.Rows[n]["middlestartdate"])) middlestartdate = Convert.ToDateTime(dtpart.Rows[n]["middlestartdate"]);
                        if (!Convert.IsDBNull(dtpart.Rows[n]["middleenddate"])) middleenddate = Convert.ToDateTime(dtpart.Rows[n]["middleenddate"]);
                    }

                    //�����ݿ�
                    string querystr = "insert into tb_partprocess " +
                        " (moduleid,partno,processid,partno_id,statusid,processneedminutes,processorder,priority,startdate,enddate,iselectrode,middlestartdate,middleenddate) " +
                        " values (@moduleid,@partno,@processid,@partno_id,@statusid,@processneedminutes,@processorder,@priority,@startdate,@enddate,@iselectrode,@middlestartdate,@middleenddate)" +
                        " select @@ROWCOUNT as AffectedRows";

                    SqlParameter[] pars = new SqlParameter[13];
                    pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                    pars[0].Value = moduleid;
                    pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
                    pars[1].Value = partno;
                    pars[2] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                    pars[2].Value = processid;
                    pars[3] = new SqlParameter("@partno_id", SqlDbType.NVarChar, 50);
                    pars[3].Value = partnoid;
                    pars[4] = new SqlParameter("@statusid", SqlDbType.NVarChar, 50);
                    pars[4].Value = statusid;
                    pars[5] = new SqlParameter("@processneedminutes", SqlDbType.Int, 4);
                    pars[5].Value = processneedminutes;
                    pars[6] = new SqlParameter("@processorder", SqlDbType.Int, 4);
                    pars[6].Value = processorder;
                    pars[7] = new SqlParameter("@priority", SqlDbType.Int, 4);
                    pars[7].Value = priority;
                    pars[8] = new SqlParameter("@startdate", SqlDbType.DateTime);
                    pars[8].Value = null;
                    if (startdate.Ticks != 0) pars[8].Value = startdate;
                    pars[9] = new SqlParameter("@enddate", SqlDbType.DateTime);
                    pars[9].Value = null;
                    if (enddate.Ticks != 0) pars[9].Value = enddate;
                    pars[10] = new SqlParameter("@iselectrode", SqlDbType.Int, 4);
                    pars[10].Value = iselectrode;
                    pars[11] = new SqlParameter("@middlestartdate", SqlDbType.DateTime);
                    pars[11].Value = null;
                    if (middlestartdate.Ticks != 0) pars[11].Value = middlestartdate;
                    pars[12] = new SqlParameter("@middleenddate", SqlDbType.DateTime);
                    pars[12].Value = null;
                    if (middleenddate.Ticks != 0) pars[12].Value = middleenddate;

                    alSql.Add(querystr);
                    alPars.Add(pars);
                }
            }

            retval = Data.excuteTrans(alSql, alPars);

            return retval;
        }

        /// <summary>
        /// �s�����ƴ��
        /// </summary>
        /// <param name="moduleid"></param>
        /// <param name="partno"></param>
        /// <param name="int_oldcount"></param>
        /// <param name="int_newcount"></param>
        /// <returns></returns>
        public static bool partcountDel(string moduleid, string partno, int int_oldcount, int int_newcount)
        {
            bool retval = false;
            ArrayList alSql = new ArrayList();
            ArrayList alPars = new ArrayList();

            //if (int_oldcount < int_newcount)
            {
                string querystrpart = "update tb_part set partcount=@partcount where moduleid=@moduleid and partno=@partno";
                SqlParameter[] parspart = new SqlParameter[3];
                parspart[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                parspart[0].Value = moduleid;
                parspart[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
                parspart[1].Value = partno;
                parspart[2] = new SqlParameter("@partcount", SqlDbType.Int);
                parspart[2].Value = int_newcount;
                alSql.Add(querystrpart);
                alPars.Add(parspart);
            }

            for (int i = int_newcount + 1; i <= int_oldcount; i++)
            {
                //�����ݿ�
                string querystr = "delete from tb_partprocess " +
                    " where moduleid=@moduleid and partno_id=@partnoid";

                SqlParameter[] pars = new SqlParameter[13];
                pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                pars[0].Value = moduleid;
                pars[1] = new SqlParameter("@partnoid", SqlDbType.NVarChar, 50);
                pars[1].Value = partno + "-" + i;

                alSql.Add(querystr);
                alPars.Add(pars);
            }

            retval = Data.excuteTrans(alSql, alPars);

            return retval;
        }

        /// <summary>
        /// �ˬd�s�����ƬO���������
        /// </summary>
        /// <param name="moduleid"></param>
        /// <param name="partno"></param>
        /// <param name="int_oldcount"></param>
        /// <param name="int_newcount"></param>
        /// <returns></returns>
        public static bool canDelpartcount(string moduleid, string partno, int int_oldcount, int int_newcount)
        {
            bool retval = true;
            string partnoidtotal = "''";
            for (int i = int_oldcount; i > int_newcount; i--)
            {
                partnoidtotal += ",'" + partno + "-" + i + "'";
            }
            string querystr = "select distinct factstartdate,factenddate from tb_partprocess where moduleid=@moduleid and partno_id in (" + partnoidtotal + ")";
            SqlParameter[] pars = new SqlParameter[1];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            DataSet ds = Data.getDataSet(querystr, pars);
            DataTable dt = ds.Tables[0];
            retval = dt.Rows.Count <= 1;

            if (int_newcount < 1) retval = false;

            return retval;
        }
        #endregion

        public static DataSet xuxiangGetModuleDesignPartProcessView(string moduleid, string partnoId)
        {
            string queryString = "select tb_DesignPartProcess.processorder listorder," +
                "tb_DesignPartProcess.processid processid," +
                "tb_DesignPartProcess.statusid statusid," +
                "tb_DesignProcess.processname processname," +
                "tb_DesignPartProcess.factstartdate factstartdate," +
                "tb_DesignPartProcess.factenddate factenddate," +
                "tb_DesignPartProcess.processno processno," +
                "tb_DesignPartProcess.processneedminutes processneedminutes,totalminutes,selfStartDate,selfEndDate,tb_DesignPartProcess.partprocessid " +
                " from tb_DesignPartProcess " +
                " inner join tb_DesignProcess on (tb_DesignPartProcess.processid=tb_DesignProcess.processid)" +
                " where tb_DesignPartProcess.ModuleId=@moduleId and tb_DesignPartProcess.partno_id=@partno_id" +
                " and tb_DesignPartProcess.ProcessId not in ('Design','Assemble') " +
                " order by tb_DesignPartProcess.processOrder ";

            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleId", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno_id", SqlDbType.NVarChar, 50);
            pars[1].Value = partnoId;
            return Data.getDataSet(queryString, pars);
        }

        #region getProcessWithMachineProcess
        public static DataSet getProcessWithMachineProcess(string moduleid, string customerid, string processid, string customerprocessid, string machineid, DateTime dtStartDate, int days, int[] statuslist)
        {
            string instatus = "(";
            for (int i = 0; i < statuslist.Length; i++)
            {
                if (i == 0)
                {
                    instatus += statuslist[i];
                }
                else
                {
                    instatus += "," + statuslist[i];
                }
            }
            instatus += ")";


            if (moduleid != null)
            {
                if (dtStartDate == DateTime.MinValue)
                {
                    if (customerprocessid != null)
                    {
                        string queryString = "select * from tb_process" + processid + " where processid=@processid and moduleid=@moduleid and " + processid + "ID=@machineid and (Designated in" + instatus + " or Designated=1) and factusedendtime is  null order by processdate,moduleid,partno_id";
                        SqlParameter[] pars = new SqlParameter[3];
                        pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                        pars[0].Value = moduleid;
                        pars[1] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                        pars[1].Value = machineid;
                        pars[2] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                        pars[2].Value = customerprocessid;
                        return Data.getDataSet(queryString, pars);
                    }
                    else
                    {
                        string queryString = "select * from tb_process" + processid + " where moduleid=@moduleid and " + processid + "ID=@machineid and Designated in " + instatus + " and factusedendtime is  null order by processdate,moduleid,partno_id";
                        SqlParameter[] pars = new SqlParameter[2];
                        pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                        pars[0].Value = moduleid;
                        pars[1] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                        pars[1].Value = machineid;
                        return Data.getDataSet(queryString, pars);
                    }
                }
                else
                {
                    if (customerprocessid != null)
                    {
                        string queryString = "select * from tb_process" + processid + " where processid=@processid and ("
                                            + "(processdate>=@startdate and processdate<=@enddate) "
                                            + "or (DATEADD(mi,ProcessMinutes,ProcessDate)>=@startdate and DATEADD(mi,ProcessMinutes,ProcessDate)<=@enddate) "
                                            + "or (processdate>@startdate and  DATEADD(mi,ProcessMinutes,ProcessDate)<@enddate) "
                                            + "or (processdate<@startdate and  DATEADD(mi,ProcessMinutes,ProcessDate)>@enddate) "
                                            + ")  and " + processid + "ID=@machineid and Designated in " + instatus + " and factusedendtime is  null order by processdate,moduleid,partno_id ";
                        SqlParameter[] pars = new SqlParameter[5];
                        pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                        pars[0].Value = moduleid;
                        pars[1] = new SqlParameter("@startdate", SqlDbType.DateTime);
                        pars[1].Value = dtStartDate;
                        pars[2] = new SqlParameter("@enddate", SqlDbType.DateTime);
                        pars[2].Value = dtStartDate.AddDays(days);
                        pars[3] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                        pars[3].Value = machineid;
                        pars[4] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                        pars[4].Value = customerprocessid;
                        return Data.getDataSet(queryString, pars);
                    }
                    else
                    {
                        string queryString = "select * from tb_process" + processid + " where moduleid=@moduleid and ("
                                             + "(processdate>=@startdate and processdate<=@enddate) "
                                             + "or (DATEADD(mi,ProcessMinutes,ProcessDate)>=@startdate and DATEADD(mi,ProcessMinutes,ProcessDate)<=@enddate) "
                                             + "or (processdate>@startdate and  DATEADD(mi,ProcessMinutes,ProcessDate)<@enddate) "
                                             + "or (processdate<@startdate and  DATEADD(mi,ProcessMinutes,ProcessDate)>@enddate) "
                                             + ") and " + processid + "ID=@machineid and Designated in" + instatus + " and factusedendtime is  null order by processdate,moduleid,partno_id";
                        SqlParameter[] pars = new SqlParameter[4];
                        pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                        pars[0].Value = moduleid;
                        pars[1] = new SqlParameter("@startdate", SqlDbType.DateTime);
                        pars[1].Value = dtStartDate;
                        pars[2] = new SqlParameter("@enddate", SqlDbType.DateTime);
                        pars[2].Value = dtStartDate.AddDays(days);
                        pars[3] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                        pars[3].Value = machineid;
                        return Data.getDataSet(queryString, pars);
                    }
                }
            }
            else
            {
                if (customerid != null)
                {
                    if (dtStartDate == DateTime.MinValue)
                    {
                        if (customerprocessid != null)
                        {
                            string queryString = "select pp.* from tb_process" + processid + " pp left join tb_order od on (pp.moduleid=od.moduleid) where pp.processid=@processid and od.customerid=@customerid and pp." + processid + "ID=@machineid and pp.Designated in " + instatus + " and factusedendtime is  null order by pp.processdate,pp.moduleid,pp.partno_id";
                            SqlParameter[] pars = new SqlParameter[3];
                            pars[0] = new SqlParameter("@customerid", SqlDbType.NVarChar, 50);
                            pars[0].Value = customerid;
                            pars[1] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                            pars[1].Value = machineid;
                            pars[2] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                            pars[2].Value = customerprocessid;
                            return Data.getDataSet(queryString, pars);
                        }
                        else
                        {
                            string queryString = "select pp.* from tb_process" + processid + " pp left join tb_order od on (pp.moduleid=od.moduleid) where od.customerid=@customerid and pp." + processid + "ID=@machineid and pp.Designated in " + instatus + " and factusedendtime is  null order by pp.processdate,pp.moduleid,pp.partno_id";
                            SqlParameter[] pars = new SqlParameter[2];
                            pars[0] = new SqlParameter("@customerid", SqlDbType.NVarChar, 50);
                            pars[0].Value = customerid;
                            pars[1] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                            pars[1].Value = machineid;
                            return Data.getDataSet(queryString, pars);
                        }
                    }
                    else
                    {
                        if (customerprocessid != null)
                        {
                            string queryString = "select pp.* from tb_process" + processid + " pp left join tb_order od on (pp.moduleid=od.moduleid) where pp.processid=@processid and od.customerid=@customerid and ("
                                                 + "(pp.processdate>=@startdate and pp.processdate<=@enddate) "
                                                 + " or (DATEADD(mi,pp.ProcessMinutes,pp.ProcessDate)>=@startdate  and DATEADD(mi,ProcessMinutes,ProcessDate)<=@enddate)"
                                                 + "or (processdate>@startdate and  DATEADD(mi,ProcessMinutes,ProcessDate)<@enddate) "
                                                 + "or (processdate<@startdate and  DATEADD(mi,ProcessMinutes,ProcessDate)>@enddate) "
                                                 + ")  and pp." + processid + "ID=@machineid and pp.Designated in " + instatus + " and factusedendtime is  null order by pp.processdate,pp.moduleid,pp.partno_id";
                            SqlParameter[] pars = new SqlParameter[5];
                            pars[0] = new SqlParameter("@customerid", SqlDbType.NVarChar, 50);
                            pars[0].Value = customerid;
                            pars[1] = new SqlParameter("@startdate", SqlDbType.DateTime);
                            pars[1].Value = dtStartDate;
                            pars[2] = new SqlParameter("@enddate", SqlDbType.DateTime);
                            pars[2].Value = dtStartDate.AddDays(days);
                            pars[3] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                            pars[3].Value = machineid;
                            pars[4] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                            pars[4].Value = customerprocessid;
                            return Data.getDataSet(queryString, pars);
                        }
                        else
                        {
                            string queryString = "select pp.* from tb_process" + processid + " pp left join tb_order od on (pp.moduleid=od.moduleid) where od.customerid=@customerid and ("
                                                 + " (pp.processdate>=@startdate and pp.processdate<=@enddate) "
                                                 + " or (DATEADD(mi,pp.ProcessMinutes,pp.ProcessDate)>=@startdate and DATEADD(mi,pp.ProcessMinutes,pp.ProcessDate)<=@enddate)"
                                                 + "or (processdate>@startdate and  DATEADD(mi,ProcessMinutes,ProcessDate)<@enddate) "
                                                 + "or (processdate<@startdate and  DATEADD(mi,ProcessMinutes,ProcessDate)>@enddate) "
                                                 + ")  and pp." + processid + "ID=@machineid and pp.Designated in " + instatus + " and factusedendtime is  null order by pp.processdate,pp.moduleid,pp.partno_id";
                            SqlParameter[] pars = new SqlParameter[4];
                            pars[0] = new SqlParameter("@customerid", SqlDbType.NVarChar, 50);
                            pars[0].Value = customerid;
                            pars[1] = new SqlParameter("@startdate", SqlDbType.DateTime);
                            pars[1].Value = dtStartDate;
                            pars[2] = new SqlParameter("@enddate", SqlDbType.DateTime);
                            pars[2].Value = dtStartDate.AddDays(days);
                            pars[3] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                            pars[3].Value = machineid;
                            return Data.getDataSet(queryString, pars);
                        }
                    }
                }
                else
                {
                    if (dtStartDate == DateTime.MinValue)
                    {
                        if (customerprocessid != null)
                        {
                            string queryString = "select * from tb_process" + processid + " where processid=@processid and " + processid + "ID=@machineid and Designated in " + instatus + " and factusedendtime is  null order by processdate,moduleid,partno_id";
                            SqlParameter[] pars = new SqlParameter[2];
                            pars[0] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                            pars[0].Value = machineid;
                            pars[1] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                            pars[1].Value = customerprocessid;
                            return Data.getDataSet(queryString, pars);
                        }
                        else
                        {
                            string queryString = "select * from tb_process" + processid + " where " + processid + "ID=@machineid and Designated in " + instatus + " and factusedendtime is  null order by processdate,moduleid,partno_id";
                            SqlParameter[] pars = new SqlParameter[1];
                            pars[0] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                            pars[0].Value = machineid;
                            return Data.getDataSet(queryString, pars);
                        }
                    }
                    else
                    {
                        if (customerprocessid != null)
                        {
                            string queryString = "select * from tb_process" + processid + " where ("
                                                 + "(processdate>=@startdate and processdate<=@enddate) "
                                                 + "or (DATEADD(mi,ProcessMinutes,ProcessDate)>=@startdate and DATEADD(mi,ProcessMinutes,ProcessDate)<=@enddate)"
                                                 + "or (processdate>@startdate and  DATEADD(mi,ProcessMinutes,ProcessDate)<@enddate) "
                                                 + "or (processdate<@startdate and  DATEADD(mi,ProcessMinutes,ProcessDate)>@enddate) "
                                                 + ") and " + processid + "ID=@machineid and Designated in " + instatus + " and factusedendtime is  null order by processdate,moduleid,partno_id";
                            SqlParameter[] pars = new SqlParameter[4];
                            pars[0] = new SqlParameter("@startdate", SqlDbType.DateTime);
                            pars[0].Value = dtStartDate;
                            pars[1] = new SqlParameter("@enddate", SqlDbType.DateTime);
                            pars[1].Value = dtStartDate.AddDays(days);
                            pars[2] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                            pars[2].Value = machineid;
                            pars[3] = new SqlParameter("@processid", SqlDbType.NVarChar, 50);
                            pars[3].Value = customerprocessid;
                            return Data.getDataSet(queryString, pars);
                        }
                        else
                        {
                            string queryString = "select * from tb_process" + processid + " where ("
                                                 + "(processdate>=@startdate and processdate<=@enddate)"
                                                 + " or (DATEADD(mi,ProcessMinutes,ProcessDate)>=@startdate and DATEADD(mi,ProcessMinutes,ProcessDate)<=@enddate) "
                                                 + "or (processdate>@startdate and  DATEADD(mi,ProcessMinutes,ProcessDate)<@enddate) "
                                                 + "or (processdate<@startdate and  DATEADD(mi,ProcessMinutes,ProcessDate)>@enddate) "
                                                 + ") and " + processid + "ID=@machineid and Designated in " + instatus + " and factusedendtime is  null order by processdate,moduleid,partno_id";
                            SqlParameter[] pars = new SqlParameter[3];
                            pars[0] = new SqlParameter("@startdate", SqlDbType.DateTime);
                            pars[0].Value = dtStartDate;
                            pars[1] = new SqlParameter("@enddate", SqlDbType.DateTime);
                            pars[1].Value = dtStartDate.AddDays(days);
                            pars[2] = new SqlParameter("@machineid", SqlDbType.NVarChar, 50);
                            pars[2].Value = machineid;
                            return Data.getDataSet(queryString, pars);
                        }
                    }
                }
            }

        }

        #endregion

        public IList _OutSourcePartProcess(IList source, string moduleid, string partno)//�����\��^���Ƥu��
        {
            bool flag = false;
            IList ilist = new ArrayList();
            Model.System.ApproveSystemInfo asi = new BLL.System.ApproveSystem().getApproveSystemByApproveAbbr("OutSource");
            DataTable dt = null;
            if (!asi.ApproveText.Equals("N"))
            {
                dt = new BLL.Outsource.OutSourceApplyPartDetail().GetOutSource_PartDetail_ByApproveToWork(moduleid, partno, "CHECK_PASS");
            }
            Hashtable ht = new Hashtable();
            foreach (PartProcessInfo ppi in source)
            {
                BLL.Process process = new BLL.Process();
                //if(process.GetProcessInfoById(ppi.ProcessId).PriceType.Equals("OUTSOURCE"))
                if (!asi.ApproveText.Equals("N"))
                {
                    string selectsearch = "processno=" + ppi.ProcessNo;
                    if (ppi.NeedProduct == 0 && dt != null && dt.Select(selectsearch).Length > 0)
                    {
                        if (!ht.ContainsKey(ppi.ProcessId))
                        {
                            ppi.ProcessName = process.GetProcessInfoById(ppi.ProcessId).ProcessName;
                            ilist.Add(ppi);
                            ht.Add(ppi.ProcessId, ppi);
                            flag = true;
                        }

                    }
                    else
                    {
                        //throw new Exception(lang.TXT_SCANBARCODE2_OutSourceContentError1) ;
                    }
                }
                else
                {
                    if (ppi.NeedProduct == 0)
                    {
                        if (!ht.ContainsKey(ppi.ProcessId))
                        {
                            ppi.ProcessName = process.GetProcessInfoById(ppi.ProcessId).ProcessName;
                            ilist.Add(ppi);
                            ht.Add(ppi.ProcessId, ppi);
                            flag = true;
                        }

                    }
                    else
                    {
                        //throw new Exception(lang.NOOUTSOURCE);
                    }
                }
            }
            if (!asi.ApproveText.Equals("N"))
            {
                if (!flag)
                {
                    throw new Exception(Lang.TXT_SCANBARCODE2_OutSourceContentError1);
                }
            }
            else
            {
                if (!flag)
                {
                    throw new Exception(Lang.NOOUTSOURCE);
                }
            }
            return ilist;
        }

        public static DataSet getPartIDProcessByProcessViewNew(string moduleid, string partnoid)
        {
            string queryString = " select processorder,processid,selfstartdate,selfenddate,processneedminutes from tb_partprocess " +
                " where moduleid=@moduleid and partno_id=@partnoid order by processorder";
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partnoid", SqlDbType.NVarChar, 50);
            pars[1].Value = partnoid;
            return Data.getDataSet(queryString, pars);
        }

        private DataTable CheckPartProcessStatusId(IList source)
        {
            if (source.Count > 0)
            {
                StringBuilder querystring = new StringBuilder("select statusid,processno,processorder from tb_partprocess where ");

                PartProcessInfo ppi = (PartProcessInfo)source[0];

                querystring.Append("  ModuleId='" + ppi.ModuleId + "' and PartNo_Id= '" + ppi.PartNo_Id + "'");
                querystring.Append(" order by processorder");
                return Data.getDataTable(querystring.ToString());
            }
            return null;
        }

        private DataTable CheckPartPartProcessStatusId(List<PartPartProcessInfo> source)
        {
            if (source.Count > 0)
            {
                StringBuilder querystring = new StringBuilder("select statusid,processno,processorder from tb_partprocess where ");

                PartPartProcessInfo ppi = source[0];

                querystring.Append("  ModuleId='" + ppi.ModuleId + "' and PartNo_Id= '" + ppi.PartNo_Id + "'");
                querystring.Append(" order by processorder");
                return Data.getDataTable(querystring.ToString());
            }
            return null;
        }

        public bool CheckModifyPartPartProcess(List<PartPartProcessInfo> source)
        {
            DataTable dt = CheckPartPartProcessStatusId(source);
            if (dt != null)
            {
                for (int i = 0; i < source.Count; i++)
                {

                    PartPartProcessInfo ppi = (PartPartProcessInfo)source[i];
                    if (ppi.ProcessNo > 0)
                    {
                        string query = "processno=" + ppi.ProcessNo;
                        DataRow[] rows = dt.Select(query);
                        if (rows.Length > 0)
                        {
                            if (!rows[0]["statusid"].ToString().Equals(ppi.StatusId))
                            {
                                if (rows[0]["statusid"].ToString().Equals("Ready") && ppi.StatusId.Equals("Pending") && ppi.ProcessOrder > Convert.ToInt32(rows[0]["processorder"]))
                                {
                                    //return true;
                                }
                                else
                                {
                                    if (rows[0]["statusid"].ToString().Equals("Pending") && ppi.StatusId.Equals("Ready") && ppi.ProcessOrder < Convert.ToInt32(rows[0]["processorder"]))
                                    {
                                        //return true;
                                    }
                                    else
                                    {
                                        throw new System.Exception(ppi.PartNo_Id + " " + Lang.TXT_ModifyPart_ReadyChange);
                                    }
                                }
                            }
                        }
                    }




                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckModifyPartProcess(List<PartPartProcessInfo> source)
        {
            DataTable dt = CheckPartPartProcessStatusId(source);
            if (dt != null)
            {
                for (int i = 0; i < source.Count; i++)
                {

                    PartProcessInfo ppi = (PartProcessInfo)source[i];
                    if (ppi.ProcessNo > 0)
                    {
                        string query = "processno=" + ppi.ProcessNo;
                        DataRow[] rows = dt.Select(query);
                        if (rows.Length > 0)
                        {
                            if (!rows[0]["statusid"].ToString().Equals(ppi.StatusId))
                            {
                                if (rows[0]["statusid"].ToString().Equals("Ready") && ppi.StatusId.Equals("Pending") && ppi.ProcessOrder > Convert.ToInt32(rows[0]["processorder"]))
                                {
                                    //return true;
                                }
                                else
                                {
                                    if (rows[0]["statusid"].ToString().Equals("Pending") && ppi.StatusId.Equals("Ready") && ppi.ProcessOrder < Convert.ToInt32(rows[0]["processorder"]))
                                    {
                                        //return true;
                                    }
                                    else
                                    {
                                        throw new System.Exception(ppi.PartNo_Id + " " + Lang.TXT_ModifyPart_ReadyChange);
                                    }
                                }
                            }
                        }
                    }




                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckScanBarPartProcess(IList source, string actionid, string processname, bool startDirectly)
        {
            DataTable dt = CheckPartProcessStatusId(source);
            if (dt != null)
            {
                switch (actionid)
                {
                    case "JIEDAN":
                        for (int i = 0; i < source.Count; i++)
                        {
                            PartProcessInfo ppi = (PartProcessInfo)source[i];
                            string query = "(processno=" + ppi.ProcessNo + " and statusid='Ready')";
                            if (dt.Select(query).Length == 0) throw new System.Exception(ppi.PartNo_Id + " " + processname + " " + Lang.TXT_SCANBARCODE2_ReadyStart);
                        }
                        break;
                    case "KAISHI":
                        for (int i = 0; i < source.Count; i++)
                        {
                            PartProcessInfo ppi = (PartProcessInfo)source[i];
                            string query = "(processno=" + ppi.ProcessNo + " and statusid='Holdon') or (processno=" + ppi.ProcessNo + " and statusid='JIEDAN')";
                            if (startDirectly)
                                query = "(processno=" + ppi.ProcessNo + " and statusid='Holdon') or (processno=" + ppi.ProcessNo + " and statusid='Ready')";

                            if (dt.Select(query).Length == 0) throw new System.Exception(ppi.PartNo_Id + " " + processname + " " + Lang.TXT_SCANBARCODE2_ReadyStart);
                        }
                        break;
                    case "ZANTING":
                        for (int i = 0; i < source.Count; i++)
                        {
                            PartProcessInfo ppi = (PartProcessInfo)source[i];
                            string query = "processno=" + ppi.ProcessNo + " and statusid='Working'";
                            if (dt.Select(query).Length == 0) throw new System.Exception(ppi.PartNo_Id + " " + processname + " " + Lang.TXT_SCANBARCODE2_ReadyHoldOnOrEnd);
                        }
                        break;
                    case "JIESHU":
                        for (int i = 0; i < source.Count; i++)
                        {
                            PartProcessInfo ppi = (PartProcessInfo)source[i];
                            string query = "processno=" + ppi.ProcessNo + " and statusid='Working'";
                            if (dt.Select(query).Length == 0) throw new System.Exception(ppi.PartNo_Id + " " + processname + " " + Lang.TXT_SCANBARCODE2_ReadyHoldOnOrEnd);
                        }
                        break;
                    case "XIAODAN":
                        for (int i = 0; i < source.Count; i++)
                        {
                            PartProcessInfo ppi = (PartProcessInfo)source[i];
                            string query = "processno=" + ppi.ProcessNo + " and statusid='Implementation'";
                            if (dt.Select(query).Length == 0) throw new System.Exception(ppi.PartNo_Id + " " + processname + " " + Lang.TXT_SCANBARCODE2_ReadyHoldOnOrEnd);
                        }
                        break;
                }
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
