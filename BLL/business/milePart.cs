using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
 

using ModuleWorkFlow.Model;

namespace ModuleWorkFlow.business
{
	/// <summary>
	/// Summary description for Part.
	/// </summary>
	public class milePart
	{
		private string ModuleId;
		private string PartNo;
		private string Process;
		private string PartPicture;
		private string Comment;
		private int Material;
		private int PartCount;
		private int NeedProduct;
		private int Priority;
		private float jiancun1;
		private float jiancun2;
		private float jiancun3;
		private float jiancun4;
		private int count1;
		private int count2;
		private int count3;
		private int count4;
		private int leastcount;
		private string PartName;
		private int PartType1;
		private int PartType2;
		private int PartType3;
		private int PartType4;
		private int PartType5;
		private DateTime EndDate;
		private string DesignProcess;
		private string RelationPart;
		private string RelationprocessId;
		private string RelationPartNo;
		private string Spec;

		//Property

		public void setEndDate(DateTime val)
		{
			this.EndDate=val;
		}

		public void setPartName(string val)
		{
			this.PartName=val;
		}

		public void setPartType1(int val)
		{
			this.PartType1= val;
		}

		public void setPartType2(int val)
		{
			this.PartType2 =val;
		}

		public void setPartType3(int val)
		{
			this.PartType3= val;
		}

		public void setPartType4(int val)
		{
			this.PartType4= val;
		}

		public void setPartType5(int val)
		{
			this.PartType5= val;
		}

		public int LeastCount
		{
			set { leastcount = value; }
			get { return leastcount; }
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

		public void setProcess(string val)
		{
			this.Process = val;
		}
		public string getProcess()
		{
			return this.Process;
		}
		public void setSpec(string val)
		{
			this.Spec = val;
		}
		public string getSpec()
		{
			return this.Spec;
		}

		public void setPartPicture(string val)
		{
			this.PartPicture = val;
		}
		public string getPartPicture()
		{
			return this.PartPicture;
		}

		public void setComment(string val)
		{
			this.Comment = val;
		}
		public string getComment()
		{
			return this.Comment;
		}

		public void setMaterial(int val)
		{
			this.Material = val;
		}
		public int getMaterial()
		{
			return this.Material;
		}

		public void setPartCount(int val)
		{
			this.PartCount = val;
		}
		public int getPartCount()
		{
			return this.PartCount;
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
			this.Priority = val;
		}
		public int getPriority()
		{
			return this.Priority;
		}

		public void setJiancun1(float val)
		{
			this.jiancun1 = val;
		}
		public void setJiancun1(string val)
		{
			try
			{
				this.jiancun1 = Convert.ToSingle(val);
			}
			catch
			{
				this.jiancun1 = 0;
			}
		}
		public float getJiancun1()
		{
			return this.jiancun1;
		}

		public void setJiancun2(float val)
		{
			this.jiancun2 = val;
		}
		public void setJiancun2(string val)
		{
			try
			{
				this.jiancun2 = Convert.ToSingle(val);
			}
			catch
			{
				this.jiancun2 = 0;
			}
		}
		public float getJiancun2()
		{
			return this.jiancun2;
		}

		public void setCount1(int val)
		{
			this.count1 = val;
		}
		public void setCount1(string val)
		{
			try
			{
				this.count1 = Convert.ToInt32(val);
			}
			catch
			{
				this.count1 = 0;
			}
		}
		public float getCount1()
		{
			return this.count1;
		}

		public void setCount2(int val)
		{
			this.count2 = val;
		}
		public void setCount2(string val)
		{
			try
			{
				this.count2 = Convert.ToInt32(val);
			}
			catch
			{
				this.count2 = 0;
			}
		}
		public float getCount2()
		{
			return this.count2;
		}

		
		//whd081107  longfull ÄÝ©Ê
		#region  longfull
		public void setCount3(int val)
		{
			this.count3 = val;
		}
		public void setCount3(string val)
		{
			try
			{
				this.count3 = Convert.ToInt32(val);
			}
			catch
			{
				this.count3 = 0;
			}
		}
		public float getCount3()
		{
			return this.count3;
		}

		public void setCount4(int val)
		{
			this.count4 = val;
		}
		public void setCount4(string val)
		{
			try
			{
				this.count4 = Convert.ToInt32(val);
			}
			catch
			{
				this.count4 = 0;
			}
		}
		public float getCount4()
		{
			return this.count4;
		}


		public void setJiancun3(float val)
		{
			this.jiancun3 = val;
		}
		public void setJiancun3(string val)
		{
			try
			{
				this.jiancun3 = Convert.ToSingle(val);
			}
			catch
			{
				this.jiancun3 = 0;
			}
		}
		public float getJiancun3()
		{
			return this.jiancun3;
		}

		public void setJiancun4(float val)
		{
			this.jiancun4 = val;
		}
		public void setJiancun4(string val)
		{
			try
			{
				this.jiancun4 = Convert.ToSingle(val);
			}
			catch
			{
				this.jiancun4 = 0;
			}
		}
		public float getJiancun4()
		{
			return this.jiancun4;
		}
		#endregion
		//endwhd081107

		//
		private int _totalminutes;
		public int TotalMinutes
		{
			set { _totalminutes = value;}
			get { return _totalminutes;}
		}

		public void setDesignProcess(string val)
		{
			this.DesignProcess = val;
		}

		public void setRelationPart(string val)
		{
			this.RelationPart=val;
		}

		public string getRelationPart()
		{
			return this.RelationPart;
		}
		
		public void setRelationprocessId(string val)
		{
			this.RelationprocessId =val;
		}

		public string getRelationprocessId()
		{
			return this.RelationprocessId;
		}

		public void setRelationPartNo(string val)
		{
			this.RelationPartNo = val;
		}

		public int ResponsibleDepartmentId { set; get; }


        public string getRelationPartNo()
		{
			return this.RelationPartNo;
		}

		public milePart()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static DataSet getPartView()
		{
			string queryString="select * from tb_part where iselectrode=0 order by moduleId,partno";
			DataSet ds = Data.getDataSet(queryString);
			DataTable dt=ds.Tables[0];
			dt.PrimaryKey = new DataColumn[]{dt.Columns["ModuleID"],dt.Columns["PartNO"]};
			return ds;
		}

		public static DataSet GetPart(string moduleid)
		{
			string querystring = "select tb_part.*,tb_partprocess.EndDate from tb_part inner join tb_partprocess on (tb_partprocess.partno=tb_part.partno) where tb_part.moduleid=@moduleid AND tb_part.iselectrode=0 ";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = moduleid;
			return Data.getDataSet(querystring,pars);
		}

		public static DataSet getDesignPart(string moduleid)
		{
			//string querystring = "select tb_part.*,tb_DesignPartProcess.EndDate from tb_part inner join tb_DesignPartProcess on (tb_DesignPartProcess.partno=tb_part.partno) where tb_part.moduleid=@moduleid AND tb_part.iselectrode=0 and tb_part.DesignProcess IS NOT NULL ";
			string querystring ="select  distinct tb_part.partno,tb_part.ModuleId,tb_part.PartType1,tb_part.Material,tb_part.PartCount,tb_part.PartName,tb_part.PartPicture,tb_part.DesignProcess,  tb_DesignPartProcess.EndDate from tb_part  inner join tb_DesignPartProcess    on (tb_DesignPartProcess.partno=tb_part.partno and tb_DesignPartProcess.ModuleId = tb_part.ModuleId) where tb_part.moduleid=@moduleid AND tb_part.iselectrode=0 and tb_part.DesignProcess IS NOT NULL   order by tb_part.partno";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = moduleid;
			return Data.getDataSet(querystring,pars);
		}

		public static DataSet GetElectrode(string moduleid)
		{
			string querystring = "select tb_part.*,tb_partprocess.EndDate from tb_part inner join tb_partprocess on (tb_partprocess.partno=tb_part.partno) where tb_part.moduleid=@moduleid AND tb_part.iselectrode=1 ";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = moduleid;
			return Data.getDataSet(querystring,pars);
		}

		public static DataRow getPartDataRow(string moduleid,string partno)
		{
			string queryString= "select tb_part.*,tb_order.customerid,tb_order.orderSingle from tb_part inner join tb_order on tb_order.moduleid=tb_part.moduleid COLLATE database_default where tb_part.moduleid=@moduleid and tb_part.partno=@partno order by tb_part.moduleid";
			SqlParameter[] pars = new SqlParameter[2];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = moduleid;
			pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar,50);
			pars[1].Value = partno;
			return Data.getDataRow(queryString,pars);
		}

		public static int removeProcessByModuleId(string moduleid,string processid)
		{
			string queryString="update tb_part set process=replace(process,'"+processid+"/','') where moduleid=@moduleid;select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = moduleid;
			return Convert.ToInt32(Data.getDataScalar(queryString,pars).ToString()); 
		}

		public static int AddDesignProcessId(string moduleid)
		{
			string queryString="update tb_part set process='Design/'+process where moduleid=@moduleid;select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = moduleid;
			return Convert.ToInt32(Data.getDataScalar(queryString,pars).ToString()); 
		}

		public static int AddAssembleProcessId(string moduleid)
		{
			string queryString="update tb_part set process=process+'/Assemble' where moduleid=@moduleid;select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = moduleid;
			return Convert.ToInt32(Data.getDataScalar(queryString,pars).ToString()); 
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public int DuplicateKeys()
		{
			string queryString="select count(*) as recordnum from tb_part where moduleid=@moduleid and partno=@partno";
			SqlParameter[] pars = new SqlParameter[2];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
			return Convert.ToInt32(Data.getDataScalar(queryString,pars).ToString()); 
		}

		public static Hashtable getHPartProcess(string moduleId)
		{
			string process;
			string queryString="select * from tb_part where moduleid=@moduleid order by partno";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = moduleId;
			DataSet dataSet=Data.getDataSet(queryString,pars);
			Hashtable hPartProcess=new Hashtable();
			DataTable dt = new DataTable();
			dt = dataSet.Tables[0];
			for(int i=0;i<dt.Rows.Count;i++)
			{	
				DataRow dr = dt.Rows[i];
				process=Convert.ToString(dr["process"]);
				process=process.Substring(0,process.Length-1);
				hPartProcess.Add(Convert.ToString(dr["PartNo"]),process);
			}

			return hPartProcess;
		}


		public static ArrayList getLNeedAssembleParts(string moduleId)
		{
			string process;
			string queryString="select * from tb_part where (moduleid=@moduleid and process like '%Assemble%')";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = moduleId;
			DataSet dataSet=Data.getDataSet(queryString,pars);
			ArrayList lNeedAssembleParts=new ArrayList();
			DataTable dt = new DataTable();
			dt = dataSet.Tables[0];
			for(int i=0;i<dt.Rows.Count;i++)
			{	
				DataRow dr = dt.Rows[i];
				process=Convert.ToString(dr["process"]);
				process=process.Substring(0,process.Length-1);
				lNeedAssembleParts.Add(Convert.ToString(dr["PartNo"]));
			}

			return lNeedAssembleParts;
		}


		public int Create()
		{
			string procedureString= "Insert into tb_part (ModuleId,PartNo,Process,PartPicture,comment,material,PartCount,Priority,importantid,totalminutes,PartName,PartType1,PartType2,DesignProcess,Spec,PartType3,PartType4,PartType5,ResponsibleDepartmentId) " +
                                    " values (@ModuleId,@PartNo,@Process,@PartPicture,@comment,@material,@PartCount,@Priority,@importantid,@totalminutes,@PartName,@PartType1,@PartType2,@DesignProcess,@Spec,@PartType3,@PartType4,@PartType5,@ResponsibleDepartmentId)" +
									" select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[19];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
			pars[2] = new SqlParameter("@process", SqlDbType.NVarChar,100);
			pars[2].Value = this.Process;
			pars[3]=new SqlParameter("@partpicture",SqlDbType.NVarChar,200);
			pars[3].Value=this.PartPicture;
			pars[4] = new SqlParameter("@comment", SqlDbType.Text);
			pars[4].Value = this.Comment;
			pars[5]=new SqlParameter("@material",SqlDbType.Int);
			pars[5].Value=this.Material;
			pars[6] = new SqlParameter("@partcount", SqlDbType.SmallInt,2);
			pars[6].Value = this.PartCount;
			pars[7] = new SqlParameter("@priority", SqlDbType.SmallInt,2);
			pars[7].Value = this.Priority;
			pars[8] = new SqlParameter("@importantid", SqlDbType.Int);
			pars[8].Value = Part.getPartImportantId(this.PartNo);
			pars[9] = new SqlParameter("@totalminutes", SqlDbType.Int);
			pars[9].Value = this._totalminutes;
			pars[10] = new SqlParameter("@PartName", SqlDbType.NVarChar,50);
			pars[10].Value = this.PartName;
			pars[11] = new SqlParameter("@PartType1", SqlDbType.Int);
			pars[11].Value = this.PartType1;
			pars[12] = new SqlParameter("@PartType2", SqlDbType.Int);
			pars[12].Value = this.PartType2;
			pars[13] = new SqlParameter("@DesignProcess",SqlDbType.NVarChar,100);
			pars[13].Value = this.DesignProcess;
			pars[14] = new SqlParameter("@Spec",SqlDbType.NVarChar,50);
			pars[14].Value=this.Spec;
			pars[15] = new SqlParameter("@PartType3", SqlDbType.Int);
			pars[15].Value = this.PartType3;
			pars[16] = new SqlParameter("@PartType4", SqlDbType.Int);
			pars[16].Value = this.PartType4;
			pars[17] = new SqlParameter("@PartType5", SqlDbType.Int);
			pars[17].Value = this.PartType5;
            pars[18] = new SqlParameter("@ResponsibleDepartmentId", SqlDbType.Int);
            pars[18].Value = this.ResponsibleDepartmentId;




            Object o=Data.getDataScalar(procedureString,pars);
			if (o==null)
			{
				return -1;
			}
			else
			{
				return (int) o;
			}
		}

		public int CreateElectrode()
		{
			string strsql=" Insert into tb_part (ModuleId,PartNo,Process,PartPicture,comment,material,PartCount,Priority,isElectrode,jiancun1,jiancun2,count1,count2,leastcount,PartName,PartType1,PartType2,RelationPart,RelationPartNo,RelationprocessId) "+
                          " values (@ModuleId,@PartNo,@Process,@PartPicture,@comment,@material,@PartCount,@Priority,1,@jiancun1,@jiancun2,@count1,@count2,@leastcount,@PartName,@PartType1,@PartType2,@RelationPart,@RelationPartNo,@RelationprocessId) "+
                          " select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[19];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
			pars[2] = new SqlParameter("@process", SqlDbType.NVarChar,100);
			pars[2].Value = this.Process;
			pars[3]=new SqlParameter("@partpicture",SqlDbType.NVarChar,200);
			pars[3].Value=this.PartPicture;
			pars[4] = new SqlParameter("@comment", SqlDbType.Text);
			pars[4].Value = this.Comment;
			pars[5]=new SqlParameter("@material",SqlDbType.Int);
			pars[5].Value=this.Material;
			pars[6] = new SqlParameter("@partcount", SqlDbType.SmallInt,2);
			pars[6].Value = this.PartCount;
			pars[7] = new SqlParameter("@priority", SqlDbType.SmallInt,2);
			pars[7].Value = this.Priority;

			pars[8] = new SqlParameter("@jiancun1", SqlDbType.Float);
			pars[8].Value = this.jiancun1;
			pars[9] = new SqlParameter("@jiancun2", SqlDbType.Float);
			pars[9].Value = this.jiancun2;
			pars[10] = new SqlParameter("@count1", SqlDbType.SmallInt,2);
			pars[10].Value = this.count1;
			pars[11] = new SqlParameter("@count2", SqlDbType.SmallInt,2);
			pars[11].Value = this.count2;
			pars[12] = new SqlParameter("@leastcount",SqlDbType.Int);
			pars[12].Value = this.LeastCount;
			pars[13] = new SqlParameter("@PartName", SqlDbType.NVarChar,50);
			pars[13].Value = this.PartName;
			pars[14] = new SqlParameter("@PartType1", SqlDbType.Int);
			pars[14].Value = this.PartType1;
			pars[15] = new SqlParameter("@PartType2", SqlDbType.Int);
			pars[15].Value = this.PartType2;
			pars[16] = new SqlParameter("@RelationPart",SqlDbType.NVarChar,50);
			pars[16].Value = this.RelationPart;
			pars[17] = new SqlParameter("@RelationprocessId",SqlDbType.NVarChar,50);
			pars[17].Value = this.RelationprocessId;
			pars[18] = new SqlParameter("@RelationPartNo",SqlDbType.NVarChar,50);
			pars[18].Value = this.RelationPartNo;


			Object o=Data.getDataScalar(strsql,pars);
			if (o==null)
			{
				return -1;
			}
			else
			{
				return (int) o;
			}
		}

		public int UpdateElectrode()
		{
			string strsql=" update tb_part set comment =@comment ,material=@material,Priority=@Priority,jiancun1=@jiancun1,jiancun2=@jiancun2,count1=@count1,count2=@count2,leastcount=@leastcount,RelationPart=@RelationPart,RelationPartNo=@RelationPartNo,RelationprocessId=@RelationprocessId  "+
				" where moduleid=@moduleid and partno=@partno"+
				" select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[13];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
			pars[2] = new SqlParameter("@comment", SqlDbType.Text);
			pars[2].Value = this.Comment;
			pars[3]=new SqlParameter("@material",SqlDbType.Int);
			pars[3].Value=this.Material;
			pars[4] = new SqlParameter("@priority", SqlDbType.SmallInt,2);
			pars[4].Value = this.Priority;
			pars[5] = new SqlParameter("@jiancun1", SqlDbType.Float);
			pars[5].Value = this.jiancun1;
			pars[6] = new SqlParameter("@jiancun2", SqlDbType.Float);
			pars[6].Value = this.jiancun2;
			pars[7] = new SqlParameter("@count1", SqlDbType.SmallInt,2);
			pars[7].Value = this.count1;
			pars[8] = new SqlParameter("@count2", SqlDbType.SmallInt,2);
			pars[8].Value = this.count2;
			pars[9] = new SqlParameter("@leastcount",SqlDbType.Int);
			pars[9].Value = this.LeastCount;
			pars[10] = new SqlParameter("@RelationPart",SqlDbType.NVarChar,50);
			pars[10].Value = this.RelationPart;
			pars[11] = new SqlParameter("@RelationprocessId",SqlDbType.NVarChar,50);
			pars[11].Value = this.RelationprocessId;
			pars[12] = new SqlParameter("@RelationPartNo", SqlDbType.NVarChar,50);
			pars[12].Value = this.RelationPartNo;


			Object o=Data.getDataScalar(strsql,pars);
			if (o==null)
			{
				return -1;
			}
			else
			{
				return (int) o;
			}
		}

		public int UpdateElectrodeProcess(string moduleid,string partno,string process)
		{
			string strsql=" update tb_part set process =@process  "+
				" where moduleid=@moduleid and partno=@partno"+
				" select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[3];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
			pars[2] = new SqlParameter("@process", SqlDbType.NVarChar,50);
			pars[2].Value = this.Process;
			


			Object o=Data.getDataScalar(strsql,pars);
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
			string procedureString="update tb_part set Process=@Process,PartPicture=@PartPicture,"+
									" comment=@comment,material=@material,PartCount=@PartCount, Priority=@Priority,totalminutes=@totalminutes,Spec = @Spec ,PartType1=@PartType1,PartType2=@PartType2 "+
									" where ModuleId=@ModuleId and PartNo=@PartNo"+
									" select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[12];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
			pars[2] = new SqlParameter("@process", SqlDbType.NVarChar,100);
			pars[2].Value = this.Process;
			pars[3]=new SqlParameter("@partpicture",SqlDbType.NVarChar,200);
			pars[3].Value=this.PartPicture;
			pars[4] = new SqlParameter("@comment", SqlDbType.Text);
			pars[4].Value = this.Comment;
			pars[5]=new SqlParameter("@material",SqlDbType.Int);
			pars[5].Value=this.Material;
			pars[6] = new SqlParameter("@partcount", SqlDbType.SmallInt,2);
			pars[6].Value = this.PartCount;
			pars[7] = new SqlParameter("@priority", SqlDbType.SmallInt,2);
			pars[7].Value = this.Priority;
			pars[8] = new SqlParameter("@totalminutes",SqlDbType.Int);
			pars[8].Value = this._totalminutes;
			pars[9] = new SqlParameter("@Spec",SqlDbType.NVarChar,50);
			pars[9].Value=this.Spec;
			pars[10] = new SqlParameter("@PartType1",SqlDbType.Int,4);
			pars[10].Value = this.PartType1;
			pars[11] = new SqlParameter("@PartType2",SqlDbType.Int,4);
			pars[11].Value = this.PartType2;

			Object o=Data.getDataScalar(procedureString,pars);
			if (o==null)
			{
				return -1;
			}
			else
			{
				return (int) o;
			}
		}

		public int StoreDesignPartProcess()
		{
			string procedureString="update tb_part set DesignProcess=@DesignProcess,PartPicture=@PartPicture,"+
				" comment=@comment,material=@material,PartCount=@PartCount, Priority=@Priority,totalminutes=@totalminutes,PartName=@PartName"+
				" where ModuleId=@ModuleId and PartNo=@PartNo"+
				" select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[10];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
			pars[2] = new SqlParameter("@DesignProcess", SqlDbType.NVarChar,100);
			pars[2].Value = this.DesignProcess;
			pars[3]=new SqlParameter("@partpicture",SqlDbType.NVarChar,200);
			pars[3].Value=this.PartPicture;
			pars[4] = new SqlParameter("@comment", SqlDbType.Text);
			pars[4].Value = this.Comment;
			pars[5]=new SqlParameter("@material",SqlDbType.Int);
			pars[5].Value=this.Material;
			pars[6] = new SqlParameter("@partcount", SqlDbType.SmallInt,2);
			pars[6].Value = this.PartCount;
			pars[7] = new SqlParameter("@priority", SqlDbType.SmallInt,2);
			pars[7].Value = this.Priority;
			pars[8] = new SqlParameter("@totalminutes",SqlDbType.Int);
			pars[8].Value = this._totalminutes;
			pars[9] = new SqlParameter("@PartName",SqlDbType.NVarChar,50);
			pars[9].Value = this.PartName;

			Object o=Data.getDataScalar(procedureString,pars);
			if (o==null)
			{
				return -1;
			}
			else
			{
				return (int) o;
			}
		}

		public int StoreOthers()
		{
			string querystr=" update tb_part set "+
							" PartPicture=@partpicture,comment=@comment,material=@material,PartCount=@partcount, Priority=@priority,PartName=@PartName,PartType1=@PartType1,PartType2=@PartType2,Spec=@Spec,PartType3=@PartType3,PartType4=@PartType4,PartType5=@PartType5 "+
							" where ModuleId=@moduleid and PartNo=@partno"+
							" select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[15];
			pars[0] = new SqlParameter("@ModuleId", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
			pars[2] = new SqlParameter("@process", SqlDbType.NVarChar,100);
			pars[2].Value = this.Process;
			pars[3]=new SqlParameter("@partpicture",SqlDbType.NVarChar,200);
			pars[3].Value=this.PartPicture;
			pars[4] = new SqlParameter("@comment", SqlDbType.Text);
			pars[4].Value = this.Comment;
			pars[5]=new SqlParameter("@material",SqlDbType.Int);
			pars[5].Value=this.Material;
			pars[6] = new SqlParameter("@partcount", SqlDbType.SmallInt,2);
			pars[6].Value = this.PartCount;
			pars[7] = new SqlParameter("@priority", SqlDbType.SmallInt,2);
			pars[7].Value = this.Priority;
			pars[8]=new SqlParameter("@PartName",SqlDbType.NVarChar,50);
			pars[8].Value=this.PartName;
			pars[9]=new SqlParameter("@PartType1",SqlDbType.Int,4);
			pars[9].Value=this.PartType1;
			pars[10]=new SqlParameter("@PartType2",SqlDbType.Int,4);
			pars[10].Value=this.PartType2;
			pars[11]=new SqlParameter("@Spec",SqlDbType.NVarChar,50);
			pars[11].Value=this.Spec;
			pars[12]=new SqlParameter("@PartType3",SqlDbType.Int,4);
			pars[12].Value=this.PartType3;
			pars[13]=new SqlParameter("@PartType4",SqlDbType.Int,4);
			pars[13].Value=this.PartType4;
			pars[14]=new SqlParameter("@PartType5",SqlDbType.Int,4);
			pars[14].Value=this.PartType5;



			Object o=Data.getDataScalar(querystr,pars);
			if (o==null)
			{
				return -1;
			}
			else
			{
				return (int) o;
			}
		}

		public int StoreProcess()
		{
			string querystr=" update tb_part set "+
				" process=@process"+
				" where ModuleId=@moduleid and PartNo=@partno"+
				" select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[3];
			pars[0] = new SqlParameter("@ModuleId", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
			pars[2] = new SqlParameter("@process", SqlDbType.NVarChar,100);
			pars[2].Value = this.Process;
			Object o=Data.getDataScalar(querystr,pars);
			if (o==null)
			{
				return -1;
			}
			else
			{
				return (int) o;
			}
		}

		public int StoreElectrodeOthers()
		{
			string strsql=" update tb_part set process=@process,comment=@comment,material=@material,PartCount=@PartCount, Priority=@Priority,jiancun1=@jiancun1, jiancun2=@jiancun2, count1=@count1,count2=@count2,PartName=@PartName,PartType1=@PartType1,PartType2=@PartType2,RelationPart=@RelationPart,RelationPartNo=@RelationPartNo,RelationprocessId=@RelationprocessId  "+
				          " where ModuleId=@ModuleId and PartNo=@PartNo select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[19];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
			pars[2] = new SqlParameter("@process", SqlDbType.NVarChar,100);
			pars[2].Value = this.Process;
			pars[3]=new SqlParameter("@partpicture",SqlDbType.NVarChar,200);
			pars[3].Value=this.PartPicture;
			pars[4] = new SqlParameter("@comment", SqlDbType.Text);
			pars[4].Value = this.Comment;
			pars[5]=new SqlParameter("@material",SqlDbType.Int);
			pars[5].Value=this.Material;
			pars[6] = new SqlParameter("@partcount", SqlDbType.SmallInt,2);
			pars[6].Value = this.PartCount;
			pars[7] = new SqlParameter("@priority", SqlDbType.SmallInt,2);
			pars[7].Value = this.Priority;

			pars[8] = new SqlParameter("@jiancun1", SqlDbType.Float);
			pars[8].Value = this.jiancun1;
			pars[9] = new SqlParameter("@jiancun2", SqlDbType.Float);
			pars[9].Value = this.jiancun2;
			pars[10] = new SqlParameter("@count1", SqlDbType.SmallInt,2);
			pars[10].Value = this.count1;
			pars[11] = new SqlParameter("@count2", SqlDbType.SmallInt,2);
			pars[11].Value = this.count2;
			pars[12]=new SqlParameter("@PartName",SqlDbType.NVarChar,50);
			pars[12].Value=this.PartName;
			pars[13]=new SqlParameter("@PartType1",SqlDbType.NVarChar,50);
			pars[13].Value=this.PartType1;
			pars[14]=new SqlParameter("@PartType2",SqlDbType.NVarChar,50);
			pars[14].Value=this.PartType2;
			pars[15] = new SqlParameter("@RelationPart",SqlDbType.NVarChar,50);
			pars[15].Value = this.RelationPart;
			pars[16] = new SqlParameter("@RelationprocessId",SqlDbType.NVarChar,50);
			pars[16].Value = this.RelationprocessId;
			pars[17] = new SqlParameter("@RelationPartNo",SqlDbType.NVarChar,50);
			pars[17].Value = this.RelationPartNo;

			Object o=Data.getDataScalar(strsql,pars);
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
			string procedureString="sp_RemovePart";
			SqlParameter[] pars = new SqlParameter[2];
			pars[0] = new SqlParameter("@ModuleId",SqlDbType.NVarChar,50);
			pars[0].Value=this.ModuleId;
			pars[1] = new SqlParameter("@PartNo",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
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

		public static string GetNextProcessId(PartProcessInfo ppi)
		{
			string moduleid = ppi.ModuleId;
			string partno = ppi.PartNo;
			string OldProcessId = ppi.ProcessId;
			DataRow dr = getPartDataRow(moduleid,partno);
			if (dr != null)
			{
				string process = dr["process"].ToString();
				string[] processList = process.Split('/');
				string NewProcessId = "";
				for(int i=0;i<processList.Length;i++)
				{
					if (processList[i] != null && processList[i].Equals(OldProcessId))
					{
						if (i<processList.Length-1)
						{
							NewProcessId = processList[i+1];
							break;
						}
					}
				}

				if (!NewProcessId.Equals(""))
				{
					return NewProcessId;
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}

		

		public static int getPartImportantId(string partnoid)
		{
			int ret = 5;
//			int partno = Convert.ToInt32(partnoid.Substring(0,3));
//			if ((partno >= 100 && partno <=199) || (partno >= 300 && partno <= 399)) ret = 1;
//			if ((partno >= 200 && partno <=299) || (partno >= 400 && partno <= 499)) ret = 2;
//			if (partno == 511 || partno == 512) ret = 3;
//			if ((partno >= 700 && partno <=799) || (partno >= 800 && partno <= 899)) ret = 4;
//			if ((partno >= 500 && partno <=599 && partno != 511 && partno != 512) || (partno >= 900 && partno <= 999)) ret = 5;
			return ret;
		}


		//whd081107 longfull ¤èªk
		public int longfullStoreElectrodeOthers()
		{
			string strsql=" update tb_part set comment=@comment,material=@material,PartCount=@PartCount, Priority=@Priority,jiancun1=@jiancun1, jiancun2=@jiancun2,jiancun3=@jiancun3, jiancun4=@jiancun4,  count1=@count1,count2=@count2, count3=@count3,count4=@count4,PartName=@PartName,PartType1=@PartType1,PartType2=@PartType2,RelationPart=@RelationPart,RelationPartNo=@RelationPartNo,RelationprocessId=@RelationprocessId  "+
				" where ModuleId=@ModuleId and PartNo=@PartNo select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[22];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
			pars[2] = new SqlParameter("@process", SqlDbType.NVarChar,100);
			pars[2].Value = this.Process;
			pars[3]=new SqlParameter("@partpicture",SqlDbType.NVarChar,200);
			pars[3].Value=this.PartPicture;
			pars[4] = new SqlParameter("@comment", SqlDbType.Text);
			pars[4].Value = this.Comment;
			pars[5]=new SqlParameter("@material",SqlDbType.Int);
			pars[5].Value=this.Material;
			pars[6] = new SqlParameter("@partcount", SqlDbType.SmallInt,2);
			pars[6].Value = this.PartCount;
			pars[7] = new SqlParameter("@priority", SqlDbType.SmallInt,2);
			pars[7].Value = this.Priority;

			pars[8] = new SqlParameter("@jiancun1", SqlDbType.Float);
			pars[8].Value = this.jiancun1;
			pars[9] = new SqlParameter("@jiancun2", SqlDbType.Float);
			pars[9].Value = this.jiancun2;
			pars[10] = new SqlParameter("@count1", SqlDbType.SmallInt,2);
			pars[10].Value = this.count1;
			pars[11] = new SqlParameter("@count2", SqlDbType.SmallInt,2);
			pars[11].Value = this.count2;
			pars[12]=new SqlParameter("@PartName",SqlDbType.NVarChar,50);
			pars[12].Value=this.PartName;
			pars[13]=new SqlParameter("@PartType1",SqlDbType.NVarChar,50);
			pars[13].Value=this.PartType1;
			pars[14]=new SqlParameter("@PartType2",SqlDbType.NVarChar,50);
			pars[14].Value=this.PartType2;
			pars[15] = new SqlParameter("@RelationPart",SqlDbType.NVarChar,50);
			pars[15].Value = this.RelationPart;
			pars[16] = new SqlParameter("@RelationprocessId",SqlDbType.NVarChar,50);
			pars[16].Value = this.RelationprocessId;
			pars[17] = new SqlParameter("@jiancun3", SqlDbType.Float);
			pars[17].Value = this.jiancun3;
			pars[18] = new SqlParameter("@jiancun4", SqlDbType.Float);
			pars[18].Value = this.jiancun4;
			pars[19] = new SqlParameter("@count3", SqlDbType.SmallInt,2);
			pars[19].Value = this.count3;
			pars[20] = new SqlParameter("@count4", SqlDbType.SmallInt,2);
			pars[20].Value = this.count4;
			pars[21] = new SqlParameter("@RelationPartNo",SqlDbType.NVarChar,50);
			pars[21].Value = this.RelationPartNo;

			Object o=Data.getDataScalar(strsql,pars);
			if (o==null)
			{
				return -1;
			}
			else
			{
				return (int) o;
			}
		}

		public int longfullUpdateElectrode()
		{
			string strsql=" update tb_part set comment =@comment ,material=@material,Priority=@Priority,jiancun1=@jiancun1,jiancun2=@jiancun2,jiancun3=@jiancun3, jiancun4=@jiancun4,count1=@count1,count2=@count2, count3=@count3,count4=@count4,leastcount=@leastcount,RelationPart=@RelationPart,RelationPartNo=@RelationPartNo,RelationprocessId=@RelationprocessId  "+
				" where moduleid=@moduleid and partno=@partno"+
				" select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[17];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
			pars[2] = new SqlParameter("@comment", SqlDbType.Text);
			pars[2].Value = this.Comment;
			pars[3]=new SqlParameter("@material",SqlDbType.Int);
			pars[3].Value=this.Material;
			pars[4] = new SqlParameter("@priority", SqlDbType.SmallInt,2);
			pars[4].Value = this.Priority;
			pars[5] = new SqlParameter("@jiancun1", SqlDbType.Float);
			pars[5].Value = this.jiancun1;
			pars[6] = new SqlParameter("@jiancun2", SqlDbType.Float);
			pars[6].Value = this.jiancun2;
			pars[7] = new SqlParameter("@count1", SqlDbType.SmallInt,2);
			pars[7].Value = this.count1;
			pars[8] = new SqlParameter("@count2", SqlDbType.SmallInt,2);
			pars[8].Value = this.count2;
			pars[9] = new SqlParameter("@leastcount",SqlDbType.Int);
			pars[9].Value = this.LeastCount;
			pars[10] = new SqlParameter("@RelationPart",SqlDbType.NVarChar,50);
			pars[10].Value = this.RelationPart;
			pars[11] = new SqlParameter("@RelationprocessId",SqlDbType.NVarChar,50);
			pars[11].Value = this.RelationprocessId;
			pars[12] = new SqlParameter("@jiancun3", SqlDbType.Float);
			pars[12].Value = this.jiancun3;
			pars[13] = new SqlParameter("@jiancun4", SqlDbType.Float);
			pars[13].Value = this.jiancun4;
			pars[14] = new SqlParameter("@count3", SqlDbType.SmallInt,2);
			pars[14].Value = this.count3;
			pars[15] = new SqlParameter("@count4", SqlDbType.SmallInt,2);
			pars[15].Value = this.count4;
			pars[16] = new SqlParameter("@RelationPartNo",SqlDbType.NVarChar,50);
			pars[16].Value = this.RelationPartNo;


			Object o=Data.getDataScalar(strsql,pars);
			if (o==null)
			{
				return -1;
			}
			else
			{
				return (int) o;
			}
		}

		public int longfullCreateElectrode()
		{
			string strsql=" Insert into tb_part (ModuleId,PartNo,Process,PartPicture,comment,material,PartCount,Priority,isElectrode,jiancun1,jiancun2,count1,count2,jiancun3,jiancun4,count3,count4,leastcount,PartName,PartType1,PartType2,RelationPart,RelationPartNo,RelationprocessId) "+
				" values (@ModuleId,@PartNo,@Process,@PartPicture,@comment,@material,@PartCount,@Priority,1,@jiancun1,@jiancun2,@count1,@count2,@jiancun3,@jiancun4,@count3,@count4,@leastcount,@PartName,@PartType1,@PartType2,@RelationPart,@RelationPartNo,@RelationprocessId) "+
				" select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[23];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
			pars[2] = new SqlParameter("@process", SqlDbType.NVarChar,100);
			pars[2].Value = this.Process;
			pars[3]=new SqlParameter("@partpicture",SqlDbType.NVarChar,200);
			pars[3].Value=this.PartPicture;
			pars[4] = new SqlParameter("@comment", SqlDbType.Text);
			pars[4].Value = this.Comment;
			pars[5]=new SqlParameter("@material",SqlDbType.Int);
			pars[5].Value=this.Material;
			pars[6] = new SqlParameter("@partcount", SqlDbType.SmallInt,2);
			pars[6].Value = this.PartCount;
			pars[7] = new SqlParameter("@priority", SqlDbType.SmallInt,2);
			pars[7].Value = this.Priority;

			pars[8] = new SqlParameter("@jiancun1", SqlDbType.Float);
			pars[8].Value = this.jiancun1;
			pars[9] = new SqlParameter("@jiancun2", SqlDbType.Float);
			pars[9].Value = this.jiancun2;
			pars[10] = new SqlParameter("@count1", SqlDbType.SmallInt,2);
			pars[10].Value = this.count1;
			pars[11] = new SqlParameter("@count2", SqlDbType.SmallInt,2);
			pars[11].Value = this.count2;
			pars[12] = new SqlParameter("@leastcount",SqlDbType.Int);
			pars[12].Value = this.LeastCount;
			pars[13] = new SqlParameter("@PartName", SqlDbType.NVarChar,50);
			pars[13].Value = this.PartName;
			pars[14] = new SqlParameter("@PartType1", SqlDbType.Int);
			pars[14].Value = this.PartType1;
			pars[15] = new SqlParameter("@PartType2", SqlDbType.Int);
			pars[15].Value = this.PartType2;
			pars[16] = new SqlParameter("@RelationPart",SqlDbType.NVarChar,50);
			pars[16].Value = this.RelationPart;
			pars[17] = new SqlParameter("@RelationprocessId",SqlDbType.NVarChar,50);
			pars[17].Value = this.RelationprocessId;
			pars[18] = new SqlParameter("@jiancun3", SqlDbType.Float);
			pars[18].Value = this.jiancun3;
			pars[19] = new SqlParameter("@jiancun4", SqlDbType.Float);
			pars[19].Value = this.jiancun4;
			pars[20] = new SqlParameter("@count3", SqlDbType.SmallInt,2);
			pars[20].Value = this.count3;
			pars[21] = new SqlParameter("@count4", SqlDbType.SmallInt,2);
			pars[21].Value = this.count4;
			pars[22] = new SqlParameter("@RelationPartNo",SqlDbType.NVarChar,50);
			pars[22].Value = this.RelationPartNo;

			Object o=Data.getDataScalar(strsql,pars);
			if (o==null)
			{
				return -1;
			}
			else
			{
				return (int) o;
			}
		}
		//endwhd081107

		public static DataRow getNewMilePartDataRow(string moduleid,string partno)
		{
			string queryString="select tb_part.*,tb_order.customerid,tb_order.yingyedandang,tb_order.designhead,tb_order.makehead,tb_order.procurementhead from tb_part inner join tb_order on tb_order.moduleid=tb_part.moduleid COLLATE database_default where tb_part.moduleid=@moduleid and tb_part.partno=@partno order by tb_part.moduleid";
			SqlParameter[] pars = new SqlParameter[2];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = moduleid;
			pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar,50);
			pars[1].Value = partno;
			return Data.getDataRow(queryString,pars);
		}
	}
}
