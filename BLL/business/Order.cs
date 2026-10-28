using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using ModuleWorkFlow.BLL;
using ModuleWorkFlow.Model;
using ModuleWorkFlow.Model.NewOrder;
using Utility;

namespace ModuleWorkFlow.business
{
	/// <summary>
	/// Summary description for Order.
	/// </summary>
	public class Order
	{
		private decimal Price;
		private string ModuleId;
		private string CustomerId;
		private string ProductType;
		private string ProductName;
		private string Material;
		private string Ton;
		private string FinishPicture;
		private string Holes;
		private DateTime ReferencePicturedate;
		private DateTime LastPicturedate;
		private DateTime Trydate0;
		private DateTime Trydate1;
		private DateTime Trydate2;
		private DateTime TryDate3;
		private string Responsible;
		private int priority;
		private int design;
		private int assemble;

		private DateTime _DTReview;
		private DateTime _DTBasicTool;
		private DateTime _DT3DMTool;
		private DateTime _DTMainParts;
		private DateTime _DTPartDraw;

		private DateTime _TryDateF;
		private DateTime _FlashCheckDate;
		private DateTime _HotBuyDate;
		private DateTime _SkeletonBuyDate;

		private string _productid;
		private string _dandangzhe;
		private string _yingyedandang;
		private string _gongchengdandang;
		private string _shejidandang;
		private string _zulidandang;

		private string _statusid;
		//MYD070822

		private int _alertday;

		public Order()
		{
			//
			// TODO: Add constructor logic here
			//
		}


		public decimal getPrice()
		{
			return this.Price;
		}

		public string getModuleId()
		{
			return this.ModuleId;
		}

		public string getCustomerId()
		{
			return this.CustomerId;
		}

		public string getProductType()
		{
			return this.ProductType;
		}

		public string getProductName()
		{
			return this.ProductName;
		}

		public string getMaterial()
		{
			return this.Material;
		}

		public string getTon()
		{
			return this.Ton;
		}

		public string getFinishPicture()
		{
			return this.FinishPicture;
		}

		public string getHoles()
		{
			return this.Holes;
		}

		public DateTime getReferencePicturedate()
		{
			return this.ReferencePicturedate;
		}

		public DateTime getLastPicturedate()
		{
			return this.LastPicturedate;
		}

		public DateTime getTrydate0()
		{
			return this.Trydate0;
		}

		public DateTime getTrydate1()
		{
			return this.Trydate1;
		}

		public DateTime getTrydate2()
		{
			return this.Trydate2;
		}

		public DateTime getTryDate3()
		{
			return this.TryDate3;
		}

		public string getResponsible()
		{
			return this.Responsible;
		}

		public int getPriority()
		{
			return priority;
		}

		public int getDesign()
		{
			return this.design;
		}

		public int getAssemble()
		{
			return assemble;
		}

		public DateTime getDTReview()
		{
			return _DTReview;
		}

		public DateTime getDTBasicTool()
		{
			return _DTBasicTool;
		}

		public DateTime getDT3DMTool()
		{
			return _DT3DMTool;
		}

		public DateTime getDTMainParts()
		{
			return _DTMainParts;
		}

		public DateTime getDTPartDraw()
		{
			return _DTPartDraw;
		}

		public DateTime getTryDateF()
		{
			return _TryDateF;
		}

		public DateTime getFlashCheckDate()
		{
			return _FlashCheckDate;
		}

		public DateTime getHotBuyDate()
		{
			return _HotBuyDate;
		}

		public DateTime getSkeletonBuyDate()
		{
			return _SkeletonBuyDate;
		}

		public string getProductId()
		{
			return _productid;
		}

		public string getDanDangZhe()
		{
			return _dandangzhe;
		}

		public string getYingYeDanDang()
		{
			return _yingyedandang;
		}

		public string getGongChengDanDang()
		{
			return _gongchengdandang;
		}

		public string getSheJiDanDang()
		{
			return _shejidandang;
		}

		public string getZuLiDanDang()
		{
			return _zulidandang;
		}

		public string getStatus()
		{
			return _statusid;
		}
		/// <summary>
		/// set part
		/// </summary>
		/// <param name="price"></param>

		public void setPrice(decimal price)
		{
			this.Price=price;
		} 
                
		public void setModuleId(String moduleId)
		{
			this.ModuleId=moduleId;
		}              

		public void setCustomerId(String customerId)
		{
			this.CustomerId=customerId;
		}            

		public void setProductType(String productType)
		{
			this.ProductType=productType;
		}           

		public void setProductName(String productName)
		{
			this.ProductName=productName;
		}           

		public void setMaterial(String material)
		{
			this.Material=material;
		}              

		public void setTon(String ton)
		{
			this.Ton=ton;
		}                
   
		public void setFinishPicture(String finishPicture)
		{
			this.FinishPicture=finishPicture;
		}         

		public void setHoles(String holes)
		{
			this.Holes=holes;
		}                 

		public void setReferencePicturedate(DateTime referencePicturedate)
		{
			this.ReferencePicturedate=referencePicturedate;
		}  

		public void setLastPicturedate(DateTime lastPicturedate)
		{
			this.LastPicturedate=lastPicturedate;
		}       

		public void setTrydate0(DateTime tryDate0)
		{
			this.Trydate0=tryDate0;
		}

		public void setTrydate1(DateTime tryDate1)
		{
			this.Trydate1=tryDate1;
		}              

		public void setTrydate2(DateTime tryDate2)
		{
			this.Trydate2=tryDate2;
		}              

		public void setTryDate3(DateTime tryDate3)
		{
			this.TryDate3=tryDate3;
		}              

		public void setResponsible(String responsible)
		{
			this.Responsible=responsible;
		} 

		public void setPriority(int priority)
		{
			this.priority=priority;
		} 


		public void setDesign(int design)
		{
			this.design=design;
		} 

		public void setAssemble(int assemble)
		{
			this.assemble=assemble;
		} 

		public void setDTReview(DateTime DTReview)
		{
			_DTReview=DTReview;
		}

		public void setDTBasicTool(DateTime DTBasicTool)
		{
			_DTBasicTool=DTBasicTool;
		}

		public void setDT3DMTool(DateTime DT3DMTool)
		{
			_DT3DMTool=DT3DMTool;
		}

		public void setDTMainParts(DateTime DTMainParts)
		{
			_DTMainParts=DTMainParts;
		}

		public void setDTPartDraw(DateTime DTPartDraw)
		{
			_DTPartDraw=DTPartDraw;
		}

		public void setTryDateF(DateTime TryDateF)
		{
			_TryDateF=TryDateF;
		}

		public void setFlashCheckDate(DateTime FlashCheckDate)
		{
			_FlashCheckDate=FlashCheckDate;
		}

		public void setHotBuyDate(DateTime HotBuyDate)
		{
			_HotBuyDate=HotBuyDate;
		}

		public void setSkeletonBuyDate(DateTime SkeletonBuyDate)
		{
			_SkeletonBuyDate=SkeletonBuyDate;
		}

		public void setProductId(string productid)
		{
			_productid=productid;
		}

		public void setDanDangZhe(string dandangzhe)
		{
			_dandangzhe=dandangzhe;
		}

		public void setYingYeDanDang(string yingyedandang)
		{
			_yingyedandang=yingyedandang;
		}

		public void setGongChengDanDang(string gongchengdandang)
		{
			_gongchengdandang=gongchengdandang;
		}

		public void setSheJiDanDang(string shejidandang)
		{
			_shejidandang=shejidandang;
		}

		public void setZuLiDanDang(string zulidandang)
		{
			_zulidandang=zulidandang;
		}

		public void setStatus(string statusid)
		{
			_statusid=statusid;
		}

		//MYD070822
		public void setAlertDay(int alertday)
		{
			_alertday = alertday;
		}

		//1
		private string _totalModuleId;
		public string TotalModuleId
		{
			set { _totalModuleId = value;}
			get { return _totalModuleId;}
		}

		private string _moduleType;
		public string ModuleType
		{
			set { _moduleType = value;}
			get { return _moduleType;}
		}
		
		private int _noId;
		public int NoId
		{
			set { _noId = value;}
			get { return _noId;}
		}
		public string CreateModuleID()
		{
			string fabCode = "YD";
			string querySring = "select top 1 moduleid from tb_order where moduleid like ('"+fabCode+"'+left(Convert(nvarchar(10),getDate(),12),2)+'%') order by moduleid desc";
			Object o = Data.getDataScalar(querySring);
			string moduleid = null;
			if (o == null)
			{
				moduleid = fabCode + string.Format("{0:yy}",(DateTime.Now)) + "-001";
			}
			else
			{
				int maxValue = Convert.ToInt32(((string)o).Substring(5,3))+1;
				moduleid = fabCode + string.Format("{0:yy}",(DateTime.Now)) + "-" +string.Format("{0:000}",(maxValue));
			}
			return moduleid;
		}
        
		public int DuplicateKeys()
		{
			string queryString="select count(*) as recordnum from tb_order where customerid=@customerid and moduleid=@moduleid";
			SqlParameter[] pars = new SqlParameter[2];
			pars[0] = new SqlParameter("@customerid", SqlDbType.NVarChar,50);
			pars[0].Value = this.CustomerId;
			pars[1] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[1].Value = this.ModuleId;
			return Convert.ToInt32(Data.getDataScalar(queryString,pars).ToString()); 
		} 

		public static DataSet getOrderView()
		{
			string queryString="select tb_order.*,tb_priority.priorityname from tb_order inner join tb_priority on (tb_priority.priorityid=tb_order.priority) where overdealt=0 order by tb_order.moduleId desc";
			return Data.getDataSet(queryString);
		}

		//□□MYB061226-1
		public static DataSet getOrderViewForEdit()
		{
			string queryString="select tb_order.*,tb_priority.priorityname from tb_order inner join tb_priority on (tb_priority.priorityid=tb_order.priority) order by tb_order.moduleId desc";
			return Data.getDataSet(queryString);
		}

        public static DataSet getOrderViewForEditONe()
        {
            string queryString = "select tb_order.* from tb_order order by tb_order.moduleId desc";
            return Data.getDataSet(queryString);
        }


		public static DataSet getOrderViewBySearch(string key)
		{
			string queryString="select tb_order.*,tb_priority.priorityname from tb_order inner join tb_priority on (tb_priority.priorityid=tb_order.priority) where overdealt=0 and moduleid like '%"+key+"%' ";
			//???場□家,?場□家□惠□Θ??
			//MYD070604
			//queryString = queryString+" and not ( len(moduleid)>8 and SUBSTRING (REVERSE(moduleid),CHARINDEX ('-',REVERSE(moduleid))-1,1)='2')";
			queryString = queryString + " order by tb_order.moduleId ";
			return Data.getDataSet(queryString);
		}

		//MYD070615
        public static IList getOrderAndProductViewBySearch(string key)
        {
            string queryString = "select moduleid,productname,customerid,trydate0,CreateDate,StatusId from tb_order where overdealt=0 and moduleid like '%" + key + "%' COLLATE database_default  group by moduleid,productname,customerid,trydate0,CreateDate,StatusId UNION select moduleid,productname,customerid,trydate0,CreateDate,StatusId from tb_order where overdealt=0 and moduleid like '%" + key + "%'";
            queryString = queryString + " order by CreateDate desc ,moduleId ";
            DataSet ds = Data.getDataSet(queryString);
            DataTable dt = ds.Tables[0];
            IList orders = new ArrayList();
            ModuleWorkFlow.BLL.Alert.WorkStatus ws = new ModuleWorkFlow.BLL.Alert.WorkStatus();
            foreach (DataRow row in dt.Rows)
            {
                OrderDesignInfo odi = new OrderDesignInfo();
                odi.ModuleId = row["moduleid"].ToString();
                odi.ProductName = row["productname"].ToString();
                odi.CustomerId = Convert.ToInt32(row["customerid"]);
                odi.TryDate0 = Convert.ToDateTime(row["trydate0"]);
                if (!Convert.IsDBNull(row["CreateDate"]))
                {
                    odi.CreateDate = Convert.ToDateTime(row["CreateDate"]);
                }
                odi.StatusId = row["StatusId"].ToString();

                IList workstatus = ws.getModuleDonePercent(odi.ModuleId);
                if (workstatus.Count > 0)
                {
                    ModuleWorkStatusInfo mwsi = workstatus[0] as ModuleWorkStatusInfo;
                    odi.ShouldPercent = mwsi.ShouldPercent;
                    odi.ShouldPercentColor = mwsi.ShouldPercentColor;
                    odi.DonePercente = mwsi.DoingPercent;
                    odi.PercentColor = mwsi.PercentColor;
                }


                orders.Add(odi);
            }

            return orders;
        }

		public static DataSet getBusinessOrderViewByCustomerId(string customerid,int mode)
		{
			// mode :: 0:all; 1: new; 2:modify;
//			string submodeNew = " and charindex('-',moduleid) <= 6 and charindex('-',moduleid,charindex('-',moduleid)+1) = 0 ";
//			string submodeModify = " and charindex('-',moduleid) <= 6 and charindex('-',moduleid,charindex('-',moduleid)+1) > 0";
			string substr = "";
//			if (mode == 1) substr = submodeNew;
//			if (mode == 2) substr = submodeModify;


			string queryString="select * from tb_order where  overdealt=0 and customerid=@customerid "+substr+" order by moduleId desc";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@customerid", SqlDbType.NVarChar,50);
			pars[0].Value = customerid;
			return Data.getDataSet(queryString,pars);
		}

		public static DataRow getOrderViewByModuleId(string moduleid)
		{
			string queryString="select tb_order.*,tb_producttype.typename,tb_usergroup.name usergroupname from tb_order left join tb_producttype on (tb_order.ProductTypeId=tb_producttype.id) left join tb_usergroup on (tb_usergroup.id=tb_order.responsible) where moduleId=@moduleid order by customerId,moduleId desc";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = moduleid;
			return Data.getDataRow(queryString,pars);
		}

		public static IList getOrderViewForAdd(string customerid)
		{
			ModuleWorkFlow.BLL.Order order = new ModuleWorkFlow.BLL.Order();
			IList alAll = order.GetOrderByCustomerId(customerid);

			string queryString="select moduleId from tb_order";
			DataSet ds = Data.getDataSet(queryString);
			DataTable dt = ds.Tables[0];
			string existModule="";
			for(int i=0;i<dt.Rows.Count;i++)
			{
				existModule += "["+dt.Rows[i]["moduleid"].ToString()+"]";
			}

			for(int i=0;i<alAll.Count;i++)
			{
				OrderInfo oi = (OrderInfo)alAll[i];
				if (existModule.IndexOf("["+oi.Id+"]") > -1)
				{
					alAll.RemoveAt(i);
					i--;
				}
			}

			return alAll;
		}

		public static DataSet getOrderViewExceptIsOver()
		{
			string queryString="select * from tb_order where isover=0 order by moduleId desc";
			return Data.getDataSet(queryString);
		}

		public static DataSet getOrderViewByNotOverDealt()
		{
			string queryString="select * from tb_order where isover=1 and overdealt=0 order by moduleId desc";
			return Data.getDataSet(queryString);
		}

		public static DataSet getOrderPrepareView()
		{
			string queryString="select * from tb_order where isstart = 1 order by customerId,moduleId desc";
			return Data.getDataSet(queryString);
		}

		public static DataSet getOrderByCondition(string condition)
		{
			string queryString="select * from tb_order where "+condition+" order by customerId,moduleId desc";
			return Data.getDataSet(queryString);
		}

		public static DataRow getOrderByModuleId(string moduleid)
		{
//			string queryString="select tb_order.*,tb_producttype.typename from tb_order inner join tb_producttype on (tb_order.ProductTypeId=tb_producttype.id)  where moduleId=@moduleid order by customerId,moduleId desc";
			string queryString="select tb_order.* from tb_order  where moduleId=@moduleid order by customerId,moduleId desc";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = moduleid;
			return Data.getDataRow(queryString,pars);
		}

		public static DateTime getTryDate0(string moduleid)
		{
			string queryString="select trydate0 from tb_order where moduleId=@moduleid";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = moduleid;
			return Convert.ToDateTime(Data.getDataScalar(queryString,pars));
		}

		public static DataSet getBusinessOrderByCustomerId(string customerid,int mode)
		{
			// mode :: 0:all; 1: new; 2:modify;
			string submodeNew = " and charindex('-',moduleid) <= 6 and charindex('-',moduleid,charindex('-',moduleid)+1) = 0 ";
			string submodeModify = " and charindex('-',moduleid) <= 6 and charindex('-',moduleid,charindex('-',moduleid)+1) > 0";
			string substr = "";
			if (mode == 1) substr = submodeNew;
			if (mode == 2) substr = submodeModify;

			string queryString="select * from tb_order where overdealt=0 and CustomerId=@customerid "+substr+" order by moduleid";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@customerid", SqlDbType.NVarChar,50);
			pars[0].Value = customerid;
			return Data.getDataSet(queryString,pars);
		}

		public static DataSet getBusinessOrderByCustomerIdNotOverdealt(string customerid,int mode)
		{
			// mode :: 0:all; 1: new; 2:modify;
			string submodeNew = " and charindex('-2',moduleid,charindex('-',moduleid)+1) = 0 ";
			string submodeModify = " and charindex('-2',moduleid,charindex('-',moduleid)+1) > 0";
			string substr = "";
			if (mode == 0) substr = submodeNew;
			if (mode == 2) substr = submodeModify;

			string queryString="select * from tb_order where CustomerId=@customerid "+substr+" order by moduleid";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@customerid", SqlDbType.NVarChar,50);
			pars[0].Value = customerid;
			return Data.getDataSet(queryString,pars);
		}

		public static DateTime getOutPutDate(string moduleid)
		{
			string queryString="select trydate1 from tb_order where moduleid=@moduleid";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = moduleid;
			return Convert.ToDateTime(Data.getDataScalar(queryString,pars));
		}


		public int Create()
		{
			//get a max+1 module Id
			//setModuleId(CreateModuleID());
			
//			string procedureString="sp_CreateOrder";
			string querystring = "Insert into tb_order (Price,ModuleId,CustomerId,"+
								"ProductTypeId,ProductName,Material,Ton,FinishPicture,"+
								"Holes,ReferencePicturedate,LastPicturedate,TryDate0,Trydate1,"+
								"Trydate2,Trydate3,Responsible,Priority,design,assemble,DTReview,DTBasicTool,DT3DMTool,"+
								"DTMainParts,DTPartDraw,TryDateF ,FlashCheckDate ,HotBuyDate,SkeletonBuyDate,productid,"+
								"dandangzhe,yingyedandang,gongchengdandang,shejidandang,zulidandang,statusid,alertday,TotalModuleId,ModuleTypeId) values ("+
								"@Price,@ModuleId,@CustomerId,"+
								"@ProductTypeId,@ProductName,@Material,@Ton,@FinishPicture,"+
								"@Holes,@ReferencePicturedate,@LastPicturedate,@Trydate0,@Trydate1,"+
								"@Trydate2,@Trydate3,@Responsible,@Priority,@design,@assemble,"+
								"@DTReview,@DTBasicTool,@DT3DMTool,@DTMainParts,@DTPartDraw,"+
								"@TryDateF ,@FlashCheckDate ,@HotBuyDate ,@SkeletonBuyDate ,"+
								"@productid,@dandangzhe,@yingyedandang,@gongchengdandang,@shejidandang,@zulidandang,@statusid,@alertday,@TotalModuleId,@ModuleTypeId)"+
								"select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[38];
			pars[0] = new SqlParameter("@Price", SqlDbType.Decimal,50);
			if (this.Price == -1)
			{
				pars[0].Value = null;
			}
			else
			{
				pars[0].Value = this.Price;
			}

			pars[1] = new SqlParameter("@ModuleId",SqlDbType.NVarChar,50);
			pars[1].Value=this.ModuleId;
			pars[2] = new SqlParameter("@CustomerId",SqlDbType.NVarChar,50);
			pars[2].Value=this.CustomerId;
			pars[3] = new SqlParameter("@ProductTypeId",SqlDbType.NVarChar,50);
			pars[3].Value=this.ProductType;
			pars[4] = new SqlParameter("@ProductName",SqlDbType.NVarChar,50);
			pars[4].Value=this.ProductName;
			pars[5] = new SqlParameter("@Material",SqlDbType.NVarChar,50);
			pars[5].Value=this.Material;
			pars[6] = new SqlParameter("@Ton",SqlDbType.NVarChar,50);
			pars[6].Value=this.Ton;
            pars[7] = new SqlParameter("@FinishPicture",SqlDbType.NVarChar,50);
			pars[7].Value=this.FinishPicture;
			pars[8] = new SqlParameter("@Holes",SqlDbType.NVarChar,50);
			pars[8].Value=this.Holes;
			pars[9] = new SqlParameter("@ReferencePicturedate",SqlDbType.DateTime);
			if (this.ReferencePicturedate.Ticks == new DateTime().Ticks)
			{
				pars[9].Value=null;
			}
			else
			{
				pars[9].Value=this.ReferencePicturedate;
			}

			pars[10] = new SqlParameter("@LastPicturedate",SqlDbType.DateTime);
			if (this.LastPicturedate.Ticks == new DateTime().Ticks)
			{
				pars[10].Value=null;
			}
			else
			{
				pars[10].Value=this.LastPicturedate;
			}

			pars[11] = new SqlParameter("@Trydate0",SqlDbType.DateTime);
			if (this.Trydate0.Ticks == new DateTime().Ticks)
			{
				pars[11].Value=null;
			}
			else
			{
				pars[11].Value=this.Trydate0;
			}

			pars[12] = new SqlParameter("@Trydate1",SqlDbType.DateTime);
			if (this.Trydate1.Ticks == new DateTime().Ticks)
			{
				pars[12].Value=null;
			}
			else
			{
				pars[12].Value=this.Trydate1;
			}

			pars[13] = new SqlParameter("@Trydate2",SqlDbType.DateTime);
			if (this.Trydate2.Ticks == new DateTime().Ticks)
			{
				pars[13].Value=null;
			}
			else
			{
				pars[13].Value=this.Trydate2;
			}
			pars[14] = new SqlParameter("@Trydate3",SqlDbType.DateTime);
			if (this.TryDate3.Ticks == new DateTime().Ticks)
			{
				pars[14].Value=null;
			}
			else
			{
				pars[14].Value=this.TryDate3;
			}

			pars[15] = new SqlParameter("@Responsible",SqlDbType.NVarChar,50);
			pars[15].Value=this.Responsible;
			pars[16] = new SqlParameter("@priority",SqlDbType.Int);
			pars[16].Value=this.priority;
			pars[17] = new SqlParameter("@design",SqlDbType.Int);
			pars[17].Value=this.design;
			pars[18] = new SqlParameter("@assemble",SqlDbType.Int);
			pars[18].Value=this.assemble;
			pars[19] = new SqlParameter("@DTReview",SqlDbType.DateTime);
			if (_DTReview.Ticks == new DateTime().Ticks)
			{
				pars[19].Value=null;
			}
			else
			{
				pars[19].Value=_DTReview;
			}
			pars[20] = new SqlParameter("@DTBasicTool",SqlDbType.DateTime);
			if (_DTBasicTool.Ticks == new DateTime().Ticks)
			{
				pars[20].Value=null;
			}
			else
			{
				pars[20].Value=_DTBasicTool;
			}
			pars[21] = new SqlParameter("@DT3DMTool",SqlDbType.DateTime);
			if (_DT3DMTool.Ticks == new DateTime().Ticks)
			{
				pars[21].Value=null;
			}
			else
			{
				pars[21].Value=_DT3DMTool;
			}
			pars[22] = new SqlParameter("@DTMainParts",SqlDbType.DateTime);
			if (_DTMainParts.Ticks == new DateTime().Ticks)
			{
				pars[22].Value=null;
			}
			else
			{
				pars[22].Value=_DTMainParts;
			}
			pars[23] = new SqlParameter("@DTPartDraw",SqlDbType.DateTime);
			if (_DTPartDraw.Ticks == new DateTime().Ticks)
			{
				pars[23].Value=null;
			}
			else
			{
				pars[23].Value=_DTPartDraw;
			}
			pars[24] = new SqlParameter("@TryDateF",SqlDbType.DateTime);
			if (_TryDateF.Ticks == new DateTime().Ticks)
			{
				pars[24].Value=null;
			}
			else
			{
				pars[24].Value=_TryDateF;
			}
			pars[25] = new SqlParameter("@FlashCheckDate",SqlDbType.DateTime);
			if (_FlashCheckDate.Ticks == new DateTime().Ticks)
			{
				pars[25].Value=null;
			}
			else
			{
				pars[25].Value=_FlashCheckDate;
			}
			pars[26] = new SqlParameter("@HotBuyDate",SqlDbType.DateTime);
			if (_HotBuyDate.Ticks == new DateTime().Ticks)
			{
				pars[26].Value=null;
			}
			else
			{
				pars[26].Value=_HotBuyDate;
			}
			pars[27] = new SqlParameter("@SkeletonBuyDate",SqlDbType.DateTime);
			if (_SkeletonBuyDate.Ticks == new DateTime().Ticks)
			{
				pars[27].Value=null;
			}
			else
			{
				pars[27].Value=_SkeletonBuyDate;
			}

			pars[28] = new SqlParameter("@productid",SqlDbType.NVarChar,50);
			pars[28].Value=this._productid;
			pars[29] = new SqlParameter("@dandangzhe",SqlDbType.NVarChar,250);
			pars[29].Value=this._dandangzhe;
			pars[30] = new SqlParameter("@yingyedandang",SqlDbType.NVarChar,250);
			pars[30].Value=this._yingyedandang;
			pars[31] = new SqlParameter("@gongchengdandang",SqlDbType.NVarChar,50);
			pars[31].Value=this._gongchengdandang;
			pars[32] = new SqlParameter("@shejidandang",SqlDbType.NVarChar,50);
			pars[32].Value=this._shejidandang;
			pars[33] = new SqlParameter("@zulidandang",SqlDbType.NVarChar,50);
			pars[33].Value=this._zulidandang;
			pars[34] = new SqlParameter("@statusid",SqlDbType.NVarChar,50);
			pars[34].Value=this._statusid;
			pars[35] = new SqlParameter("@alertday",SqlDbType.Int);
			pars[35].Value = this._alertday;

			pars[36] = new SqlParameter("@TotalModuleId", SqlDbType.NVarChar,50);
			pars[36].Value = this.TotalModuleId;
			pars[37] = new SqlParameter("@ModuleTypeId",SqlDbType.Int );
			pars[37].Value = this.ModuleType;


			Object o=Data.getDataScalar(querystring,pars);
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
			string procedureString="sp_RemoveOrder";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@ModuleId",SqlDbType.NVarChar,50);
			pars[0].Value=this.ModuleId;
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
			string querystring="update tb_order set Price=@Price,customerid=@customerid,alertday=@alertday,"+
				"ProductTypeId=@ProductTypeId,ProductName=@ProductName,Material=@Material,Ton=@Ton,FinishPicture=@FinishPicture,"+
				"Holes=@Holes,ReferencePicturedate=@ReferencePicturedate,LastPicturedate=@LastPicturedate,Trydate0=@Trydate0,Trydate1=@Trydate1,"+
				"Trydate2=@Trydate2,Trydate3=@Trydate3,Responsible=@Responsible,Priority=@Priority,design=@design,assemble=@assemble ,"+
				"DTReview=@DTReview,DTBasicTool=@DTBasicTool,DT3DMTool=@DT3DMTool,DTMainParts=@DTMainParts,DTPartDraw=@DTPartDraw,"+
				"TryDateF=@TryDateF,FlashCheckDate=@FlashCheckDate,HotBuyDate=@HotBuyDate,SkeletonBuyDate=@SkeletonBuyDate,"+
				"productid=@productid,dandangzhe=@dandangzhe,yingyedandang=@yingyedandang,gongchengdandang=@gongchengdandang,shejidandang=@shejidandang,zulidandang=@zulidandang,statusid=@statusid,"+
				"TotalModuleId=@TotalModuleId,ModuleTypeId=@ModuleTypeId where moduleid=@moduleid "+
				"select @@ROWCOUNT as AffectedRows";
			SqlParameter[] pars = new SqlParameter[38];
			pars[0] = new SqlParameter("@Price", SqlDbType.Decimal,50);
			if (this.Price == -1)
			{
				pars[0].Value = null;
			}
			else
			{
				pars[0].Value = this.Price;
			}

			pars[1] = new SqlParameter("@ModuleId",SqlDbType.NVarChar,50);
			pars[1].Value=this.ModuleId;
			pars[2] = new SqlParameter("@CustomerId",SqlDbType.NVarChar,50);
			pars[2].Value=this.CustomerId;
			pars[3] = new SqlParameter("@ProductTypeId",SqlDbType.NVarChar,50);
			pars[3].Value=this.ProductType;
			pars[4] = new SqlParameter("@ProductName",SqlDbType.NVarChar,50);
			pars[4].Value=this.ProductName;
			pars[5] = new SqlParameter("@Material",SqlDbType.NVarChar,50);
			pars[5].Value=this.Material;
			pars[6] = new SqlParameter("@Ton",SqlDbType.NVarChar,50);
			pars[6].Value=this.Ton;
			pars[7] = new SqlParameter("@FinishPicture",SqlDbType.NVarChar,50);
			pars[7].Value=this.FinishPicture;
			pars[8] = new SqlParameter("@Holes",SqlDbType.NVarChar,50);
			pars[8].Value=this.Holes;
			pars[9] = new SqlParameter("@ReferencePicturedate",SqlDbType.DateTime);
			if (this.ReferencePicturedate.Ticks == new DateTime().Ticks)
			{
				pars[9].Value=null;
			}
			else
			{
				pars[9].Value=this.ReferencePicturedate;
			}

			pars[10] = new SqlParameter("@LastPicturedate",SqlDbType.DateTime);
			if (this.LastPicturedate.Ticks == new DateTime().Ticks)
			{
				pars[10].Value=null;
			}
			else
			{
				pars[10].Value=this.LastPicturedate;
			}

			pars[11] = new SqlParameter("@Trydate0",SqlDbType.DateTime);
			if (this.Trydate0.Ticks == new DateTime().Ticks)
			{
				pars[11].Value=null;
			}
			else
			{
				pars[11].Value=this.Trydate0;
			}

			pars[12] = new SqlParameter("@Trydate1",SqlDbType.DateTime);
			if (this.Trydate1.Ticks == new DateTime().Ticks)
			{
				pars[12].Value=null;
			}
			else
			{
				pars[12].Value=this.Trydate1;
			}

			pars[13] = new SqlParameter("@Trydate2",SqlDbType.DateTime);
			if (this.Trydate2.Ticks == new DateTime().Ticks)
			{
				pars[13].Value=null;
			}
			else
			{
				pars[13].Value=this.Trydate2;
			}
			pars[14] = new SqlParameter("@Trydate3",SqlDbType.DateTime);
			if (this.TryDate3.Ticks == new DateTime().Ticks)
			{
				pars[14].Value=null;
			}
			else
			{
				pars[14].Value=this.TryDate3;
			}

			pars[15] = new SqlParameter("@Responsible",SqlDbType.NVarChar,50);
			pars[15].Value=this.Responsible;
			pars[16] = new SqlParameter("@priority",SqlDbType.Int);
			pars[16].Value=this.priority;
			pars[17] = new SqlParameter("@design",SqlDbType.Int);
			pars[17].Value=this.design;
			pars[18] = new SqlParameter("@assemble",SqlDbType.Int);
			pars[18].Value=this.assemble;
			pars[19] = new SqlParameter("@DTReview",SqlDbType.DateTime);
			if (_DTReview.Ticks == new DateTime().Ticks)
			{
				pars[19].Value=null;
			}
			else
			{
				pars[19].Value=_DTReview;
			}
			pars[20] = new SqlParameter("@DTBasicTool",SqlDbType.DateTime);
			if (_DTBasicTool.Ticks == new DateTime().Ticks)
			{
				pars[20].Value=null;
			}
			else
			{
				pars[20].Value=_DTBasicTool;
			}
			pars[21] = new SqlParameter("@DT3DMTool",SqlDbType.DateTime);
			if (_DT3DMTool.Ticks == new DateTime().Ticks)
			{
				pars[21].Value=null;
			}
			else
			{
				pars[21].Value=_DT3DMTool;
			}
			pars[22] = new SqlParameter("@DTMainParts",SqlDbType.DateTime);
			if (_DTMainParts.Ticks == new DateTime().Ticks)
			{
				pars[22].Value=null;
			}
			else
			{
				pars[22].Value=_DTMainParts;
			}
			pars[23] = new SqlParameter("@DTPartDraw",SqlDbType.DateTime);
			if (_DTPartDraw.Ticks == new DateTime().Ticks)
			{
				pars[23].Value=null;
			}
			else
			{
				pars[23].Value=_DTPartDraw;
			}
			pars[24] = new SqlParameter("@TryDateF",SqlDbType.DateTime);
			if (_TryDateF.Ticks == new DateTime().Ticks)
			{
				pars[24].Value=null;
			}
			else
			{
				pars[24].Value=_TryDateF;
			}
			pars[25] = new SqlParameter("@FlashCheckDate",SqlDbType.DateTime);
			if (_FlashCheckDate.Ticks == new DateTime().Ticks)
			{
				pars[25].Value=null;
			}
			else
			{
				pars[25].Value=_FlashCheckDate;
			}
			pars[26] = new SqlParameter("@HotBuyDate",SqlDbType.DateTime);
			if (_HotBuyDate.Ticks == new DateTime().Ticks)
			{
				pars[26].Value=null;
			}
			else
			{
				pars[26].Value=_HotBuyDate;
			}
			pars[27] = new SqlParameter("@SkeletonBuyDate",SqlDbType.DateTime);
			if (_SkeletonBuyDate.Ticks == new DateTime().Ticks)
			{
				pars[27].Value=null;
			}
			else
			{
				pars[27].Value=_SkeletonBuyDate;
			}

			pars[28] = new SqlParameter("@productid",SqlDbType.NVarChar,50);
			pars[28].Value=this._productid;
			pars[29] = new SqlParameter("@dandangzhe",SqlDbType.NVarChar,50);
			pars[29].Value=this._dandangzhe;
			pars[30] = new SqlParameter("@yingyedandang",SqlDbType.NVarChar,50);
			pars[30].Value=this._yingyedandang;
			pars[31] = new SqlParameter("@gongchengdandang",SqlDbType.NVarChar,50);
			pars[31].Value=this._gongchengdandang;
			pars[32] = new SqlParameter("@shejidandang",SqlDbType.NVarChar,50);
			pars[32].Value=this._shejidandang;
			pars[33] = new SqlParameter("@zulidandang",SqlDbType.NVarChar,50);
			pars[33].Value=this._zulidandang;
			pars[34] = new SqlParameter("@statusid",SqlDbType.NVarChar,50);
			pars[34].Value=this._statusid;
			pars[35] = new SqlParameter("@alertday",SqlDbType.Int);
			pars[35].Value = this._alertday;


			pars[36] = new SqlParameter("@TotalModuleId", SqlDbType.NVarChar,50);
			pars[36].Value = this.TotalModuleId;
			pars[37] = new SqlParameter("@ModuleTypeId",SqlDbType.Int );
			pars[37].Value = this.ModuleType;


			Object o=Data.getDataScalar(querystring,pars);
			if (o==null)
			{
				return -1;
			}
			else
			{
				return (int) o;
			}

		}

		public int ExistRelationData()
		{
			string queryString="select count(*) as recordnum from tb_partprocess where ModuleId=@ModuleId";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@ModuleId", SqlDbType.NVarChar,50);
			pars[0].Value = this.ModuleId;
			return Convert.ToInt32(Data.getDataScalar(queryString,pars).ToString()); 
		}

		//public bool

		public int CheckOrderForBom(string moduleid)
		{
			string queryString="select Count(*) as bomcount from tb_bomdesign where moduleid=@moduleid ";
			SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@moduleid", SqlDbType.NVarChar,50);
			pars[0].Value = moduleid;
			return Convert.ToInt32(Data.getDataScalar(queryString,pars).ToString());
		}
	}
}
