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
	public class Part
	{
		private string ModuleId;
		private string PartNo;
		private string Process;
		private string PartPicture;
		private string Comment;
		private string Material;
		private int PartCount;
		private int NeedProduct;
		private int Priority;
		private float jiancun1;
		private float jiancun2;
		private int count1;
		private int count2;
		private int leastcount;
		private string PartName;
		private int PartType1;
		private int PartType2;
		private string RelationPart;
		private string RelationPartNo;
		private string RelationprocessId;

		//Property

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

		public void setMaterial(string val)
		{
			this.Material = val;
		}
		public string getMaterial()
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

		//
		private int _totalminutes;
		public int TotalMinutes
		{
			set { _totalminutes = value;}
			get { return _totalminutes;}
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

		public string getRelationPartNo()
		{
			return this.RelationPartNo;
		}


		public Part()
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
            string querystring = "select tb_part.id  as idd ,tb_part.*,tb_priority.priorityname,tb_material.name  from tb_part inner join tb_priority on (tb_priority.priorityid=tb_part.priority) " +
                                 " left join tb_material on tb_material.id = tb_part.Material " +
                                 " where tb_part.moduleid=@moduleid AND tb_part.iselectrode=0 and (tb_part.isvirtual=0 or tb_part.isvirtual is null) and DesignProcess is null";

			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = moduleid;
		
			
			return Data.getDataSet(querystring,pars);
		}

        public static DataSet GetDesignPart(string moduleid)
        {
            string querystring = "select tb_part.id  as idd ,tb_part.*,tb_priority.priorityname, name=''  from tb_part  inner join tb_priority on (tb_priority.priorityid=tb_part.priority) " +
                                 " where tb_part.moduleid=@moduleid  And DesignProcess is not null";

            SqlParameter[] pars = new SqlParameter[1];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;


            return Data.getDataSet(querystring, pars);
        }

		public static DataSet GetPartLike(string moduleid)
		{
            string querystring = " select tb_part.id  as idd,tb_part.* , tb_material.name ,tb_priority.priorityname from tb_part inner join tb_priority on (tb_priority.priorityid=tb_part.priority) " +
                                 " left join tb_material on (tb_material.id = tb_part.Material)" +
                                 " where (tb_part.moduleid like '%" + moduleid + "%' or tb_part.partno='" + moduleid + "') AND tb_part.iselectrode=0 and (tb_part.isvirtual=0 or tb_part.isvirtual is null) and DesignProcess is null";
			
			
			return Data.getDataSet(querystring,null);
		}

        public static DataSet GetDesignPartLike(string moduleid)
        {
            string querystring = " select tb_part.id  as idd,tb_part.*,tb_priority.priorityname, name=''  from tb_part  inner join tb_priority on (tb_priority.priorityid=tb_part.priority) " +
                                 " where (tb_part.moduleid like '%" + moduleid + "%' or tb_part.partno='" + moduleid + "') AND DesignProcess is not null";


            return Data.getDataSet(querystring, null);
        }

		public static DataSet GetElectrode(string moduleid)
		{
            string querystring = "select IDENTITY (int,1,1) as idd,tb_part.*,tb_priority.priorityname into tb_tempelectrodetable from tb_part inner join tb_priority on (tb_priority.priorityid=tb_part.priority) where tb_part.moduleid=@moduleid AND tb_part.iselectrode=1 and (tb_part.isvirtual=0 or tb_part.isvirtual is null)  ";
			querystring += "  select * from tb_tempelectrodetable ";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = moduleid;
			querystring += "drop table tb_tempelectrodetable"; 

			return Data.getDataSet(querystring,pars);
		}

		public static DataSet GetElectrodeLike(string moduleid)
		{
            string querystring = "select IDENTITY (int,1,1) as idd,tb_part.*,tb_priority.priorityname into tb_tempelectrodetable from tb_part inner join tb_priority on (tb_priority.priorityid=tb_part.priority) where tb_part.moduleid like '%" + moduleid + "%' AND tb_part.iselectrode=1 and (tb_part.isvirtual=0 or tb_part.isvirtual is null)  ";
			querystring += "  select * from tb_tempelectrodetable ";
//			SqlParameter[] pars = new SqlParameter[1];
//			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
//			pars[0].Value = moduleid;
			querystring += "drop table tb_tempelectrodetable"; 

			return Data.getDataSet(querystring,null);
		}

		public static DataSet GetSelectElectrode(string moduleid)
		{
			string querystring = "select tb_part.*,tb_priority.priorityname from tb_part inner join tb_priority on (tb_priority.priorityid=tb_part.priority) where tb_part.moduleid=@moduleid AND tb_part.iselectrode=1 ";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = moduleid;

			return Data.getDataSet(querystring,pars);
		}

		public static DataRow getPartDataRow(string moduleid,string partno)
		{
			string queryString="select tb_part.*,tb_order.customerid from tb_part inner join tb_order on tb_order.moduleid=tb_part.moduleid COLLATE database_default where tb_part.moduleid=@moduleid and tb_part.partno=@partno order by tb_part.moduleid";
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
			string procedureString="Insert into tb_part (ModuleId,PartNo,Process,PartPicture,comment,material,PartCount,Priority,importantid,totalminutes,PartName,PartType1,PartType2) "+
									" values (@ModuleId,@PartNo,@Process,@PartPicture,@comment,@material,@PartCount,@Priority,@importantid,@totalminutes,@PartName,@PartType1,@PartType2)"+
									" select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[13];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
			pars[2] = new SqlParameter("@process", SqlDbType.NVarChar,100);
			pars[2].Value = this.Process;
			pars[3]=new SqlParameter("@partpicture",SqlDbType.NVarChar,50);
			pars[3].Value=this.PartPicture;
			pars[4] = new SqlParameter("@comment", SqlDbType.Text);
			pars[4].Value = this.Comment;
			pars[5]=new SqlParameter("@material",SqlDbType.NVarChar,50);
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
			pars[3]=new SqlParameter("@partpicture",SqlDbType.NVarChar,50);
			pars[3].Value=this.PartPicture;
			pars[4] = new SqlParameter("@comment", SqlDbType.Text);
			pars[4].Value = this.Comment;
			pars[5]=new SqlParameter("@material",SqlDbType.NVarChar,50);
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
			string strsql=" update tb_part set comment =@comment ,material=@material,Priority=@Priority,jiancun1=@jiancun1,jiancun2=@jiancun2,count1=@count1,count2=@count2,leastcount=@leastcount,PartName =@PartName,RelationPart=@RelationPart,RelationPartNo=@RelationPartNo,RelationprocessId=@RelationprocessId  "+
				" where moduleid=@moduleid and partno=@partno"+
				" select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[14];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
			pars[2] = new SqlParameter("@comment", SqlDbType.Text);
			pars[2].Value = this.Comment;
			pars[3]=new SqlParameter("@material",SqlDbType.NVarChar,50);
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
			pars[10] = new SqlParameter("@PartName",SqlDbType.NVarChar,50);
			pars[10].Value = this.PartName;
			pars[11] = new SqlParameter("@RelationPart",SqlDbType.NVarChar,50);
			pars[11].Value = this.RelationPart;
			pars[12] = new SqlParameter("@RelationprocessId",SqlDbType.NVarChar,50);
			pars[12].Value = this.RelationprocessId;
			pars[13] = new SqlParameter("@RelationPartNo",SqlDbType.NVarChar,50);
			pars[13].Value = this.RelationPartNo;


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
									" comment=@comment,material=@material,PartCount=@PartCount, Priority=@Priority,totalminutes=@totalminutes"+
									" where ModuleId=@ModuleId and PartNo=@PartNo"+
									" select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[8];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
			pars[2] = new SqlParameter("@process", SqlDbType.NVarChar,100);
			pars[2].Value = this.Process;
			pars[3]=new SqlParameter("@partpicture",SqlDbType.NVarChar,50);
			pars[3].Value=this.PartPicture;
			pars[4] = new SqlParameter("@comment", SqlDbType.Text);
			pars[4].Value = this.Comment;
			pars[5]=new SqlParameter("@material",SqlDbType.NVarChar,50);
			pars[5].Value=this.Material;
			pars[6] = new SqlParameter("@partcount", SqlDbType.SmallInt,2);
			pars[6].Value = this.PartCount;
			pars[7] = new SqlParameter("@priority", SqlDbType.SmallInt,2);
			pars[7].Value = this.Priority;
			pars[8] = new SqlParameter("@totalminutes",SqlDbType.Int);
			pars[8].Value = this._totalminutes;

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
							" PartPicture=@partpicture,comment=@comment,material=@material,PartCount=@partcount, Priority=@priority,PartName=@PartName,PartType1=@PartType1,PartType2=@PartType2"+
							" where ModuleId=@moduleid and PartNo=@partno"+
							" select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[11];
			pars[0] = new SqlParameter("@ModuleId", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
			pars[2] = new SqlParameter("@process", SqlDbType.NVarChar,100);
			pars[2].Value = this.Process;
			pars[3]=new SqlParameter("@partpicture",SqlDbType.NVarChar,50);
			pars[3].Value=this.PartPicture;
			pars[4] = new SqlParameter("@comment", SqlDbType.Text);
			pars[4].Value = this.Comment;
			pars[5]=new SqlParameter("@material",SqlDbType.NVarChar,50);
			pars[5].Value=this.Material;
			pars[6] = new SqlParameter("@partcount", SqlDbType.SmallInt,2);
			pars[6].Value = this.PartCount;
			pars[7] = new SqlParameter("@priority", SqlDbType.SmallInt,2);
			pars[7].Value = this.Priority;
			pars[8]=new SqlParameter("@PartName",SqlDbType.NVarChar,50);
			pars[8].Value=this.PartName;
			pars[9]=new SqlParameter("@PartType1",SqlDbType.NVarChar,50);
			pars[9].Value=this.PartType1;
			pars[10]=new SqlParameter("@PartType2",SqlDbType.NVarChar,50);
			pars[10].Value=this.PartType2;



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
			string strsql=" update tb_part set comment=@comment,material=@material,PartCount=@PartCount, Priority=@Priority,jiancun1=@jiancun1, jiancun2=@jiancun2, count1=@count1,count2=@count2,PartName=@PartName,PartType1=@PartType1,PartType2=@PartType2 "+
				          " where ModuleId=@ModuleId and PartNo=@PartNo select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[15];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			pars[1]=new SqlParameter("@partno",SqlDbType.NVarChar,50);
			pars[1].Value=this.PartNo;
			pars[2] = new SqlParameter("@process", SqlDbType.NVarChar,100);
			pars[2].Value = this.Process;
			pars[3]=new SqlParameter("@partpicture",SqlDbType.NVarChar,50);
			pars[3].Value=this.PartPicture;
			pars[4] = new SqlParameter("@comment", SqlDbType.Text);
			pars[4].Value = this.Comment;
			pars[5]=new SqlParameter("@material",SqlDbType.NVarChar,50);
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
				return (int) 1;
			}

		}

		public int RemoveDesign()
		{
			string procedureString="sp_RemoveDesignPart";
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

        public static bool insertvirtualandupdateElerode(string VirtualPartNo,string moduleid,string partnoids,string virtualpartname)
        {
            ArrayList alinsert = new ArrayList();
            ArrayList alpars = new ArrayList();
            string[] processnos = partnoids.Substring(0, partnoids.Length - 1).Split(',');
            string strsql = " Insert into tb_part (ModuleId,PartNo,PartCount,PartName,isvirtual) " +
                           " values (@ModuleId,@PartNo,@PartCount,@PartName,1) " +
                           " select @@ROWCOUNT as AffectedRows";
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = VirtualPartNo;
            pars[2] = new SqlParameter("@partcount", SqlDbType.SmallInt, 2);
            pars[2].Value = processnos.Length;
            pars[3] = new SqlParameter("@PartName", SqlDbType.NVarChar, 50);
            pars[3].Value = virtualpartname;
            alinsert.Add(strsql);
            alpars.Add(pars);
            for (int i = 0; i < processnos.Length; i++)
            {
                if (processnos[i].Equals(""))
                {
                    continue;
                }
                string update = "update tb_partprocess set isvirtual=1,virtualpartno=@virtualpartno where moduleid=@moduleid and partno_id=@partnoid";
                SqlParameter[] pars1 = new SqlParameter[3];
                pars1[0] = new SqlParameter("@partnoid", SqlDbType.NVarChar,50);
                pars1[0].Value = processnos[i];
                pars1[1] = new SqlParameter("@virtualpartno", SqlDbType.NVarChar, 50);
                pars1[1].Value = VirtualPartNo;
                pars1[2] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                pars1[2].Value = moduleid;
                alinsert.Add(update);
                alpars.Add(pars1);
            }
            return Data.excuteTrans(alinsert, alpars);
        }

        public static bool updatevirtualandupdateElerode(string VirtualPartNo, string moduleid, string partnoids, string virtualpartname)
        {
            ArrayList alinsert = new ArrayList();
            ArrayList alpars = new ArrayList();
            string[] processnos = partnoids.Substring(0, partnoids.Length - 1).Split(',');
            string strsql = " update tb_part set PartCount=@partcount,PartName=@PartName " +
                           " where (moduleid=@moduleid and partno=@partno) " +
                           " select @@ROWCOUNT as AffectedRows";
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = VirtualPartNo;
            pars[2] = new SqlParameter("@partcount", SqlDbType.SmallInt, 2);
            pars[2].Value = processnos.Length;
            pars[3] = new SqlParameter("@PartName", SqlDbType.NVarChar, 50);
            pars[3].Value = virtualpartname;
            alinsert.Add(strsql);
            alpars.Add(pars);
            strsql = " update tb_partprocess set isvirtual=0,virtualpartno=null " +
                           " where (moduleid=@moduleid and virtualpartno=@partno) " +
                           " select @@ROWCOUNT as AffectedRows";
            pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
            pars[0].Value = moduleid;
            pars[1] = new SqlParameter("@partno", SqlDbType.NVarChar, 50);
            pars[1].Value = VirtualPartNo;
            alinsert.Add(strsql);
            alpars.Add(pars);
            for (int i = 0; i < processnos.Length; i++)
            {
                if (processnos[i].Equals(""))
                {
                    continue;
                }
                string update = "update tb_partprocess set isvirtual=1,virtualpartno=@virtualpartno where moduleid=@moduleid and partno_id=@partnoid";
                SqlParameter[] pars1 = new SqlParameter[3];
                pars1[0] = new SqlParameter("@partnoid", SqlDbType.NVarChar, 50);
                pars1[0].Value = processnos[i];
                pars1[1] = new SqlParameter("@virtualpartno", SqlDbType.NVarChar, 50);
                pars1[1].Value = VirtualPartNo;
                pars1[2] = new SqlParameter("@moduleid", SqlDbType.NVarChar, 50);
                pars1[2].Value = moduleid;
                alinsert.Add(update);
                alpars.Add(pars1);
            }
            return Data.excuteTrans(alinsert, alpars);
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
	}
}
