#region 代碼文件說明
/* 
	Title	BookingSystem
	Author	湯建棟(jandon)
	Create	Sep.24,2004
	Version 1.0  
 */
#endregion

using System;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using System.Xml;
using Microsoft.ApplicationBlocks.Data;		//需添加文件SQLHelper.cs(微軟提供的數據訪問模塊)
using System.Web.UI.WebControls;

using System.Drawing;
using System.Linq;
namespace Utility
{
	#region 命名空間描述
	/// <summary>
	/// 命名空間描述
	/// </summary>
	public class Info
	{
		public const string Title="復旦學生網";
		public const string URL="http://stu.fudan.edu.cn";
		public const string	Address="葉耀珍樓503室";
		public const string Tel="021-55664425";
	}
	#endregion

	#region 全局設置
	/// <summary>
	/// 全局設置
	/// </summary>
	public class Settings
	{
		#region 參數
		public static string[] Channel={"校園頻道","新聞頻道","娛樂頻道","生活頻道","學習頻道", "專題頻道" ,"其他"};
		public static string[] ChannelColor={"#f7df9b", "#f7df9b", "#f7df9b", "#f7df9b", "#f7df9b", "#f7df9b", "#f7df9b"};
		public static string RobotID = "SYSTEM_NewsRobo";
		public static int	HotLimit = 20;	//HotNews點擊率下限值
		public static int	StuBoardID = 65;	//學生網信息發佈版塊
		public static int	ForumBoardID = 1;	//論壇根版塊
		#endregion

		#region enum 定義
		public enum BoardTypes : byte {PlainText=0,Html=1,FileLink=2,FileInDatabase=3,Reference=4}
		public static string[] Grades={"2000級本科","2001級本科","2002級本科","2003級本科","2004級本科","研究生、博士生","其他復旦學生","復旦老師","復旦大學校友","非復旦人員"};
		#endregion
	}
	#endregion

	#region 用戶認證
	/// <summary>
	/// 用戶認證
	/// </summary>
	public class Identity
	{
		#region 角色定義
		/// <summary>
		/// 角色
		/// </summary>
		public struct Roles
		{
			public const string Unknown="Unknown";
			public const string Visitor="Visitor";
		}
		#endregion

		#region Logon
		public static void UserLogon(string UserID,string IP)
		{	
			System.Data.SqlClient.SqlParameter[] pars = new SqlParameter[2];
			pars[0] = new SqlParameter("@UserID",SqlDbType.VarChar,15);
			pars[0].Value = UserID;
			pars[1] = new SqlParameter("@LastLogonIp",SqlDbType.VarChar,15);
			pars[1].Value = IP;
			Data.ExecuteNonQuery("UserLogon",CommandType.StoredProcedure,pars);
		}
		#endregion

		#region Logonout
		public static void UserLogonout(string UserID)
		{	
			System.Data.SqlClient.SqlParameter[] pars = new SqlParameter[1];
			pars[0] = new SqlParameter("@UserID",SqlDbType.VarChar,15);
			pars[0].Value = UserID;
			Data.ExecuteNonQuery("UserLogonout",CommandType.StoredProcedure,pars);
		}
		#endregion


		#region 角色認證方法
		/// <summary>
		/// 驗證用戶
		/// </summary>
		/// <param name="UserID">用戶名</param>
		/// <param name="Password">密碼</param>
		/// <returns></returns> 
		public static bool Authenticate(string UserID,string Password)
		{
			SqlConnection con=new SqlConnection(Data.ReadConnectionStr());

			SqlCommand com=new SqlCommand("UserAuthenticate",con);
			com.CommandType=CommandType.StoredProcedure;
			com.Parameters.Add("@UserID",SqlDbType.VarChar,15);
			com.Parameters.Add("@Password",SqlDbType.VarChar,15);
			com.Parameters[0].Value=UserID;
			com.Parameters[1].Value=Password;

			int count;

			try
			{
				con.Open();
				count=(int)com.ExecuteScalar();
			}
			finally
			{
				con.Close();
			}

			return (count>0);
		}
		
		/// <summary>
		/// 判斷用戶是否存在
		/// </summary>
		/// <param name="UserID">用戶名</param>
		/// <returns></returns>
		public static bool UserExists(string UserID)
		{
			SqlConnection con=new SqlConnection(Data.ReadConnectionStr());

			SqlCommand com=new SqlCommand("UserExists",con);
			com.CommandType=CommandType.StoredProcedure;
			com.Parameters.Add("@UserID",SqlDbType.VarChar,15);
			com.Parameters[0].Value=UserID;

			int count;
			try
			{
				con.Open();
				count=(int)com.ExecuteScalar();
			}
			finally
			{
				con.Close();
			}

			return (count>0);
		}	
		
		/// <summary>
		/// 判斷在指定版塊該用戶是否存在(涉及UserRole表,現在可能不用這張表)
		/// </summary>
		/// <param name="UserID">用戶名</param>
		/// <param name="BoardID">版塊ID</param>
		/// <returns></returns>
		public static bool UserExists(string UserID,int BoardID)
		{
			SqlCommand com=Data.getSqlComOfStrdPrcdr("UserExists_Board");
			com.Parameters.Add("@UserID",SqlDbType.VarChar,15).Value=UserID;
			com.Parameters.Add("@BoardID",SqlDbType.Int).Value=BoardID;

			int count;
			try
			{
				com.Connection.Open();
				count=(int)com.ExecuteScalar();
			}
			finally
			{
				com.Connection.Close();
			}
			return (count>0);
		}	
		
		/// <summary>
		/// 獲取角色權限值(涉及Roles表,現在可能不用這張表)
		/// </summary>
		/// <param name="RoleID">角色ID</param>
		/// <returns></returns> 
		public static int getRolePrivilliage(string RoleID)
		{
			SqlCommand com=Data.getSqlComOfStrdPrcdr("getRolePrivilliage");
			com.Parameters.Add("@RoleID",SqlDbType.VarChar,50).Value=RoleID;
			object RolePrivilliage=Data.getDataScalar(com);
			if(RolePrivilliage==null)
				return 0;
			return int.Parse(RolePrivilliage.ToString());
		}
		#endregion

		#region 權限描述結構
		/// <summary>
		/// 讀權限
		/// </summary>
		public enum ReadLevel {CannotRead=0,ReadTitle=1,ReadContent=2}	
		/// <summary>
		/// 改權限
		/// </summary>	
		public enum EditLevel {CannotEdit=0,EditSelf,EditAll,EditProperty /*置頂等屬性*/}
		/// <summary>
		/// 刪除權限
		/// </summary>
		public enum DeleteLevel {CannontDelete=0,DeleteSelf,DeleteAll}

		/// <summary>
		/// 權限描述結構
		/// </summary>
		public struct BoardAccessLevel
		{
			/// <summary>
			/// 讀公開信息
			/// </summary>
			public ReadLevel ReadPassed;	
			/// <summary>
			/// 讀非公開信息
			/// </summary>
			public ReadLevel ReadNotPassed;	
			/// <summary>
			/// 改公開信息
			/// </summary>
			public EditLevel EditPassed;		
			/// <summary>
			/// 改非公開信息
			/// </summary>
			public EditLevel EditNotPassed;	
			/// <summary>
			/// 刪除公開信息
			/// </summary>
			public DeleteLevel DeletePassed;	
			/// <summary>
			/// 刪除非公開信息
			/// </summary>
			public DeleteLevel DeleteNotPassed;	
			/// <summary>
			/// 是否具有審核信息的權限
			/// </summary>
			public bool Pass;
			/// <summary>
			/// 是否具有發佈信息的權限
			/// </summary>
			public bool Append;
			/// <summary>
			/// 是否具有管理權限
			/// </summary>
			public bool Manage;
			//public bool Search;

			//默認構造全部0，無任何權限
			/*public BoardAccessLevel() 
			{
				
				this.ReadPassed=CannotRead;
				this.ReadNotPassed=CannotRead;
				this.EditPassed=CannotEdit;
				this.EditNotPassed=CannotEdit;
				this.DeleteNotPassed=CannontDelete;
				this.DeletePassed=CannontDelete;
				this.Pass=false;
				this.Append=false;
				this.Search=false;
				
			}*/

			/// <summary>
			/// 構造函數，全參數
			/// </summary>
			/// <param name="aReadPassed">讀公開信息的權限值</param>
			/// <param name="aReadNotPassed">讀非公開信息的權限值</param>
			/// <param name="aEditPassed">改公開信息的權限值</param>
			/// <param name="aEditNotPassed">改非公開信息的權限值</param>
			/// <param name="aDeletePassed">刪除公開信息的權限值</param>
			/// <param name="aDeleteNotPassed">刪除非公開信息的權限值</param>
			/// <param name="aPass">是否具有審核信息的權限</param>
			/// <param name="aAppend">是否具有發佈信息的權限</param>
			/// <param name="aManage">是否具有管理權限</param>
			/// 
			public BoardAccessLevel(
				ReadLevel aReadPassed,
				ReadLevel aReadNotPassed,
				EditLevel aEditPassed,
				EditLevel aEditNotPassed,
				DeleteLevel aDeletePassed,
				DeleteLevel aDeleteNotPassed,
				bool aPass,
				bool aAppend,
				bool aManage)
				//bool aSearch 
			{
				this.ReadPassed=aReadPassed;
				this.ReadNotPassed=aReadNotPassed;
				this.EditPassed=aEditPassed;
				this.EditNotPassed=aEditNotPassed;
				this.DeleteNotPassed=aDeleteNotPassed;
				this.DeletePassed=aDeletePassed;
				this.Pass=aPass;
				this.Append=aAppend;
				this.Manage=aManage;
			}

			/// <summary>
			/// 拷貝構造函數
			/// </summary>
			/// <param name="aClone"></param>
			public BoardAccessLevel(BoardAccessLevel aClone)
			{
				this.ReadPassed=aClone.ReadPassed;
				this.ReadNotPassed=aClone.ReadNotPassed;
				this.EditPassed=aClone.EditPassed;
				this.EditNotPassed=aClone.EditNotPassed;
				this.DeletePassed=aClone.DeletePassed;
				this.DeleteNotPassed=aClone.DeleteNotPassed;
				this.Pass=aClone.Pass;
				this.Append=aClone.Append;
				this.Manage=aClone.Manage;
				//this.Search=aClone.Search;
			}


			//常用BoardAccessLevel模式，直接調用
			//public static BoardAccessLevel PublicRead = new BoardAccessLevel();
			//public static BoardAccessLevel 
		}
		#endregion		
	}
	#endregion

	#region 頁面設置	
	namespace Pages
	{
		#region 頁面位置
		/// <summary>
		/// 頁面位置
		/// </summary>
		public class Location
		{
			//public const string Home_Stu = "http://10.107.0.49";
			//public const string Home_Stu ="http://stu.fudan.edu.cn";
			public const string Home_Stu = "..";

			/// <summary>
			/// 註冊頁 
			/// </summary>
			public const string Page_Register = Home_Stu+"/register.aspx";		
			
			/// <summary>
			/// 新聞單個欄目文章列表
			/// </summary>
			public const string Page_List = Home_Stu+"/list.aspx";		
			
			/// <summary>
			/// 參數錯誤
			/// </summary>
			public const string Page_Error_Para = Home_Stu + "/error/para.aspx";
	
			///<summary>
			/// 權限不足
			/// </summary>
			public const string Page_Error_Limit = Home_Stu + "/error/limit.aspx";	
			
			/// <summary>
			/// 頁面不存在
			/// </summary>
			public const string Page_Error_None = Home_Stu + "/error/none.aspx";
	
			/// <summary>
			/// 建設中
			/// </summary>
			public const string Page_Error_Construction = Home_Stu + "/error/construction.aspx";

			/// <summary>
			/// 通用錯誤 	
			/// </summary>		
			public const string Page_Error = Home_Stu + "/error/error.aspx";		
			
			/// <summary>
			/// 選擇轉向頁面
			/// </summary>
			public const string Name_Switch = "Switch.aspx";
		}
		#endregion

		#region Switch (Front Controller 模式)
		/// <summary>
		/// Switch (Front Controller 模式)
		/// </summary>
		public class Switch
		{
			/*菜單命令模式：
			 * Transfer直接將本頁面QueryString傳到下一個頁面；
			 * List在Settings.Xml中指定傳輸的Key
			 * Custom自定義標籤，需解析
			 * None無參數
			 * 默認值Transfer（不需再Settings.Xml中指定）
			 * */
			public enum ParaMode {Transfer=0,List,Custom,None};	
			public struct MenuItem
			{
				public string Text;
				public string Command;
				public ParaMode CommandMode;
				//public MenuItem ParentMenuItem; 
				public string CssClass;
			}
		}
		#endregion
		
		#region 公用動態生成方法
		/// <summary>
		/// 公用動態生成方法
		/// </summary>
		public class GenerateHtml
		{
        }
		#endregion
	}

    #endregion

    #region 數據訪問
    /// <summary>
    /// 數據訪問
    /// </summary>
    /// 

    public class PTSetting
    {
        public static int DAYS = 30;
        public static int HOURS = 24;

        public static string[] DEFAULTPROCESS = { "PrePare", "Grind", "Milling", "Cut", "CNC", "EDM", "Flash" };
        public static string[] DEFAULTELECTRODEPROCESS = { "Cut", "CNCE", "QC", "Flash" };

        public static string[] PROCESSID = { "Design", "Merchandise", "Lathe", "Drill", "BefroeCNC", "CNCHeat", "AfterCNC", "Engraving", "QC", "Texture", "Grind", "Milling", "PrePare", "Cut", "SlowCut", "CNC", "CNCN", "EDM", "Flash", "Assemble", "CNCE", "PlaneEDM", "CAM", "burn" };
        public static string[] PROCESSNAME = { "設計", "外購", "車床", "鑽床", "CNC熱前", "熱處理", "CNC熱後", "雕刻", "品管", "咬花", "磨床", "銑床", "備料", "快絲", "慢絲", "CNC加工中心", "CNC普通機", "放電", "打光", "組立", "CNC電極機", "鏡面放電", "CAM", "燒焊" };
        public static string[] PROCESSBARCODE = { "SHEJI", "WAIGOU", "CHECHUANG", "ZUANCHUANG", "CNCREQIAN", "RECHULI", "CNCREHOU", "DIAOKE", "PINGUAN", "YAOHUA", "MOCHUANG", "XICHUANG", "BEILIAO", "KUAISI", "MANSI", "CNC", "CNCPUTONGJI", "FANGDIAN", "DAGUANG", "ZULI", "CNCDIANJIJI", "JINGMIANFANGDIAN", "CAM", "SHAOHAN" };

        public static string[] MATERIALTYPEID = { "Production", "Part", "Try" };
        public static string[] MATERIALTYPENAME = { "產品", "零件", "試模" };

        public static string[] STATUSID = { "Implementation", "Working", "Pending", "Rejected", "Cancelled", "Reopened", "Delay", "Ready", "Holdon", "JIEDAN", "XIAODAN", "CHUCHANG", "HUICHANG", "FANHUI", "Rezuli" };
        public static string[] STATUSNAME = { "完成", "進行中", "未就緒", "拒絕", "取消", "重新開始", "延遲", "就緒", "暫停", "接單", "消單", "出廠", "回廠", "返回", "組立返修" };
        public static string[] STATUSCODE = { "WANCHENG", "JINXINGZHONG", "WEIJIUXU", "JUJUE", "QUXIAO", "CHONGXINKAISHI", "YANCHI", "JIUXU", "ZANTING", "JIEDAN", "XIAODAN", "CHUCHANG", "HUICHANG", "FANHUI", "ZULIFANXIU" };
        public static string[] STATUSCOLOR = { "&H00FF00 ", "&H99ff66", "&HFFFF00", "&HFF0000", "&H787878", "&HCC0000", "&HFF0000", "&H00FFFF", "&HB7B7B7", "&HFFB300", "&HB0B9FF  ", "&HCCFF00", "&H00FF00", "&HB7B7B7", "&HFF3FD7" };


        //		MYD070615
        public static string[] ACTIONBARCODE = { "KAISHI", "ZANTING", "JIESHU", "FANGONG", "RUKU", "CHUKU", "CHUCHANG", "HUICHANG", "FANHUI", "XIAODAN", "JIEDAN", "CHUHUO", "DAIPAN", "BULIANG" };
        public static string[] ACTIONNAME = { "開始", "暫停", "結束", "返工", "入庫", "出庫", "出廠", "回廠", "返回", "消單", "接單", "出貨", "待判", "不良" };

        #region 條形碼命令規則
        //模式
        public static string[] BARCODE_CODE = { "A", "M", "C", "D", "Z", "B" };
		public static string PART_CODE = "M";
        public static string[] BARCODE_COMMAND = { "USERNO", "MODULEPART", "PROCESS", "ACTION", "MODULE", "MACHINE" };

        #endregion


        public static Color SELECTCOLOR = Color.PowderBlue;
        public static Color FACTDATECOLOR = Color.DimGray;
        public static Color NormalColor = Color.White;
        public static Color DelayColor = Color.Red;
        public static Color FinishColor = Color.Green;

        public static string ALLValue = "all";
        public static string ALLText = "全部";

        public static string STATUSCOLOR_DONE = "GREEN";
        public static string STATUSCOLOR_DOING = "BLUE";
        public static string STATUSCOLOR_READY = "YELLOW";

        public static string INFILENAME = "Sample.xls";

        public static string TreeViewRoot = "webctrl_client/1_0/treeimages/";
    }

    public class Data
	{
		public static string constr ;
		#region 連接字符串
		/// <summary>
		/// 讀帳號連接字符串
		/// </summary>
		/// <returns></returns>
		public static string ReadConnectionStr()
		{
			//MYD070313
			//return "server=.;database=ModuleWorkFlow;uid=workflow;pwd=workflow";	
			return System.Configuration.ConfigurationSettings.AppSettings["MsSQLConnString"];
	
		}

		/// <summary>
		/// 寫帳號連接字符串
		/// </summary>
		/// <returns></returns>
		public static string WriteConnectionStr()
		{
			////MYD070313
			//return "server=.;database=ModuleWorkFlow;uid=workflow;pwd=workflow";
			return System.Configuration.ConfigurationSettings.AppSettings["MsSQLConnString"];

		}

		#endregion

		#region 執行命令,返回受影響的行數
		/// <summary>
		/// 執行命令,返回受影響的行數
		/// </summary>
		/// <param name="sql">Sql查詢語句</param>
		/// <returns></returns>
		public static int ExecuteNonQuery(string sql)
		{
			return SqlHelper.ExecuteNonQuery(WriteConnectionStr(),CommandType.Text,sql);
		}
		/// <summary>
		/// 執行命令,返回受影響的行數
		/// </summary>
		/// <param name="sql">Sql查詢語句</param>
		/// <param name="commandParameters">參數數組</param>
		/// <returns></returns>
		public static int ExecuteNonQuery(string sql, params SqlParameter[] commandParameters)
		{
			return SqlHelper.ExecuteNonQuery(WriteConnectionStr(),CommandType.Text,sql,commandParameters);
		}

        public static int ExecuteNonQueryWithConnectionStr(string sql,string connectionStr, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteNonQuery(connectionStr, CommandType.Text, sql, commandParameters);
        }
		/// <summary>
		/// 執行命令,返回受影響的行數
		/// </summary>
		/// <param name="StrdPrcdrName">存儲過程名</param>
		/// <param name="CommandType">填CommandType.StoredProcedure</param>
		/// <returns></returns>
		public static int ExecuteNonQuery(string StrdPrcdrName,CommandType commandType)
		{
			return SqlHelper.ExecuteNonQuery(WriteConnectionStr(),commandType,StrdPrcdrName);
		}
		/// <summary>
		/// 執行命令,返回受影響的行數
		/// </summary>
		/// <param name="StrdPrcdrName">存儲過程名</param>
		/// <param name="commandType">填CommandType.StoredProcedure</param>
		/// <param name="commandParameters">參數數組</param>
		/// <returns></returns>
		public static int ExecuteNonQuery(string StrdPrcdrName,CommandType commandType, params SqlParameter[] commandParameters)
		{
			return SqlHelper.ExecuteNonQuery(WriteConnectionStr(),commandType,StrdPrcdrName,commandParameters);
		}

        public static int ExecuteNonQueryWithConnectionStr(string StrdPrcdrName, string connectionStr, CommandType commandType, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteNonQuery(connectionStr, commandType, StrdPrcdrName, commandParameters);
        }	
		#endregion
		
		#region 獲取數據讀取器
		/// <summary>
		/// 獲取數據讀取器
		/// </summary>
		/// <param name="sql">Sql查詢語句</param>
		/// <returns></returns>
		public static SqlDataReader ExecuteReader(string sql)
		{
			return SqlHelper.ExecuteReader(ReadConnectionStr(),CommandType.Text,sql);
		}

		/// <summary>
		/// 獲取數據讀取器
		/// </summary>
		/// <param name="sql">Sql查詢語句</param>
		/// <param name="commandParameters">參數數組</param>
		/// <returns></returns>
		public static SqlDataReader ExecuteReader(string sql, params SqlParameter[] commandParameters)
		{
			return SqlHelper.ExecuteReader(ReadConnectionStr(),CommandType.Text,sql,commandParameters);
		}

		/// <summary>
		/// 獲取數據讀取器
		/// </summary>
		/// <param name="StrdPrcdrName">存儲過程名</param>
		/// <param name="CommandType">填CommandType.StoredProcedure</param>
		/// <returns></returns>
		public static SqlDataReader ExecuteReader(string StrdPrcdrName,CommandType commandType)
		{
			return SqlHelper.ExecuteReader(WriteConnectionStr(),commandType,StrdPrcdrName);
		}

		/// <summary>
		/// 獲取數據讀取器
		/// </summary>
		/// <param name="StrdPrcdrName">填CommandType.StoredProcedure</param>
		/// <param name="commandType">存儲過程名</param>
		/// <param name="commandParameters">參數數組</param>
		/// <returns></returns>
		public static SqlDataReader ExecuteReader(string StrdPrcdrName,CommandType commandType, params SqlParameter[] commandParameters)
		{
			return SqlHelper.ExecuteReader(Data.WriteConnectionStr(),commandType,StrdPrcdrName,commandParameters);		
		}

		#endregion
		
		#region 獲取單個值
		/// <summary>
		/// 獲取單個值,無結果則返回null
		/// </summary>
		/// <param name="sql">Sql查詢語句</param>
		/// <returns></returns>
		public static object getDataScalar(string sql)
		{
			return SqlHelper.ExecuteScalar(ReadConnectionStr(),CommandType.Text,sql);
		}

		/// <summary>
		/// 獲取單個值,無結果則返回null
		/// </summary>
		/// <param name="sql">Sql查詢語句</param>
		/// <param name="commandParameters">參數數組</param>
		/// <returns></returns>
		public static object getDataScalar(string sql, params SqlParameter[] commandParameters)
		{
            return SqlHelper.ExecuteScalar(ReadConnectionStr(),CommandType.Text,sql,commandParameters);
		}

		/// <summary>
		/// 獲取單個值,無結果則返回null
		/// </summary>
		/// <param name="StrdPrcdrName">存儲過程名</param>
		/// <param name="commandType">填CommandType.StoredProcedure</param>
		/// <returns></returns>
		public static object getDataScalar(string StrdPrcdrName,CommandType commandType)
		{
			return SqlHelper.ExecuteScalar(WriteConnectionStr(),commandType,StrdPrcdrName);
		}

		/// <summary>
		/// 獲取單個值,無結果則返回null
		/// </summary>
		/// <param name="StrdPrcdrName">存儲過程名</param>
		/// <param name="commandType">填CommandType.StoredProcedure</param>
		/// <param name="commandParameters">參數數組</param>
		/// <returns></returns>
		public static object getDataScalar(string StrdPrcdrName,CommandType commandType, params SqlParameter[] commandParameters)
		{
			return SqlHelper.ExecuteScalar(WriteConnectionStr(),commandType,StrdPrcdrName, commandParameters);
		}
		
		#endregion
		
		#region 獲取數據行
		/// <summary>
		/// 獲取一行數據,無結果返回null
		/// </summary>
		/// <param name="sql">Sql查詢語句</param>
		/// <returns></returns>
		public static DataRow getDataRow(string sql)
		{
			DataTable dt = getDataTable(sql);
			if (dt.Rows.Count != 0)
				return dt.Rows[0];
			return null;
		}
		
		/// <summary>
		/// 獲取一行數據,無結果返回null
		/// </summary>
		/// <param name="sql">Sql查詢語句</param>
		/// <param name="commandParameters">參數數組</param>
		/// <returns></returns>
		public static DataRow getDataRow(string sql, params SqlParameter[] commandParameters)
		{
			DataTable dt = getDataTable(sql,commandParameters);
			if (dt.Rows.Count != 0)
				return dt.Rows[0];
			return null;
		}

		/// <summary>
		/// 獲取一行數據,無結果返回null
		/// </summary>
		/// <param name="StrdPrcdrName">存儲過程名</param>
		/// <param name="commandType">填CommandType.StoredProcedure</param>
		/// <returns></returns>
		public static DataRow getDataRow(string StrdPrcdrName,CommandType commandType)
		{
			DataTable dt = getDataTable(StrdPrcdrName,commandType);
			if (dt.Rows.Count != 0)
				return dt.Rows[0];
			return null;
		}

		/// <summary>
		/// 獲取一行數據,無結果返回null
		/// </summary>
		/// <param name="StrdPrcdrName">存儲過程名</param>
		/// <param name="commandType">填CommandType.StoredProcedure</param>
		/// <param name="commandParameters">參數數組</param>
		/// <returns></returns>
		public static DataRow getDataRow(string StrdPrcdrName,CommandType commandType, params SqlParameter[] commandParameters)
		{
			DataTable dt = getDataTable(StrdPrcdrName,commandType,commandParameters);
			if (dt.Rows.Count != 0)
				return dt.Rows[0];
			return null;
		}

		#endregion
				
		#region 獲取數據表
		/// <summary>
		/// 獲取一個數據表格,無結果返回空表
		/// </summary>
		/// <param name="sql">Sql查詢語句</param>
		/// <returns></returns>
		public static DataTable getDataTable(string sql)
		{
			DataSet  ds = getDataSet(sql);
			if (ds.Tables.Count != 0)
				return ds.Tables[0]; 			
			return new DataTable();
		}

		/// <summary>
		/// 獲取一個數據表格,無結果返回空表
		/// </summary>
		/// <param name="sql">Sql查詢語句</param>
		/// <param name="commandParameters">參數數組</param>
		/// <returns></returns>
		public static DataTable getDataTable(string sql, params SqlParameter[] commandParameters)
		{
			DataSet  ds = getDataSet(sql,commandParameters);
			if (ds.Tables.Count != 0)
				return ds.Tables[0]; 			
			return new DataTable();
		}
		
		/// <summary>
		/// 獲取一個數據表格,無結果返回空表
		/// </summary>
		/// <param name="StrdPrcdrName">存儲過程名</param>
		/// <param name="commandType">填CommandType.StoredProcedure</param>
		/// <returns></returns>
		public static DataTable getDataTable(string StrdPrcdrName,CommandType commandType)
		{
			DataSet  ds = getDataSet(StrdPrcdrName,commandType);
			if (ds.Tables.Count != 0)
				return ds.Tables[0]; 			
			return new DataTable();
		}

		/// <summary>
		/// 獲取一個數據表格,無結果返回空表
		/// </summary>
		/// <param name="StrdPrcdrName">存儲過程名</param>
		/// <param name="commandType">填CommandType.StoredProcedure</param>
		/// <param name="commandParameters">參數數組</param>
		/// <returns></returns>
		public static DataTable getDataTable(string StrdPrcdrName,CommandType commandType, params SqlParameter[] commandParameters)
		{
			DataSet  ds = getDataSet(StrdPrcdrName,commandType,commandParameters);
			if (ds.Tables.Count != 0)
				return ds.Tables[0]; 			
			return new DataTable();
		}	

		#endregion
						
		#region 獲取數據集
		/// <summary>
		/// 獲取一個數據集,無結果返回空的數據集
		/// </summary>
		/// <param name="sql">Sql查詢語句</param>
		/// <returns></returns>
		public static DataSet getDataSet(string sql)
		{
			return SqlHelper.ExecuteDataset(WriteConnectionStr(), CommandType.Text, sql);
		}
		/// <summary>
		/// 獲取一個數據集,無結果返回空的數據集
		/// </summary>
		/// <param name="sql">Sql查詢語句</param>
		/// <param name="commandParameters">參數數組</param>
		/// <returns></returns>
		public static DataSet getDataSet(string sql, params SqlParameter[] commandParameters)
		{
			return SqlHelper.ExecuteDataset(WriteConnectionStr(), CommandType.Text,sql,commandParameters);
		}

        public static DataSet getDataSetWithConnectionStr(string sql, string connectionString,params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteDataset(connectionString, CommandType.Text, sql, commandParameters);
        }	
		
		/// <summary>
		/// 獲取一個數據集,無結果返回空的數據集
		/// </summary>
		/// <param name="StrdPrcdrName">存儲過程名</param>
		/// <param name="commandType">填CommandType.StoredProcedure</param>
		/// <returns></returns>
		public static DataSet getDataSet(string StrdPrcdrName,CommandType commandType)
		{
			return SqlHelper.ExecuteDataset(WriteConnectionStr(), commandType, StrdPrcdrName);
		}	

		/// <summary>
		/// 獲取一個數據集,無結果返回空的數據集
		/// </summary>
		/// <param name="StrdPrcdrName">存儲過程名</param>
		/// <param name="commandType">填CommandType.StoredProcedure</param>
		/// <param name="commandParameters">參數數組</param>
		/// <returns></returns>
		public static DataSet getDataSet(string StrdPrcdrName,CommandType commandType, params SqlParameter[] commandParameters)
		{
			return SqlHelper.ExecuteDataset(WriteConnectionStr(), commandType, StrdPrcdrName,commandParameters);
		}	

		#endregion

		#region 執行事務,成功返回真,失敗返回假
		/// <summary>
		/// 在事務中執行不帶參數的sql語句
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static bool excuteTrans(ArrayList alSql)
		{
			SqlConnection conn = new SqlConnection();
			conn.ConnectionString = WriteConnectionStr();
			conn.Open();

			SqlCommand myCommand = conn.CreateCommand();
			SqlTransaction myTrans;

			// Start a local transaction
			myTrans = conn.BeginTransaction();
			// Must assign both transaction object and connection
			// to Command object for a pending local transaction
			myCommand.Connection = conn;
			myCommand.Transaction = myTrans;

			try
			{
				for (int i=0;i<alSql.Count;i++)
				{
					myCommand.CommandText = alSql[i].ToString();
					myCommand.ExecuteNonQuery();
				}
				myTrans.Commit();
				return true;
			}
			catch(Exception e)
			{
				try
				{
					myTrans.Rollback();
				}
				catch (SqlException ex)
				{
					if (myTrans.Connection != null)
					{
						Console.WriteLine("An exception of type " + ex.GetType() +
							" was encountered while attempting to roll back the transaction.");
					}
				}
    
				Console.WriteLine("An exception of type " + e.GetType() +
					" was encountered while inserting the data.");
				Console.WriteLine("Neither record was written to database.");
				return false;
			}
			finally 
			{
				conn.Close();
			}			
		}

        public static bool excuteTransWithConnectionStr(ArrayList alSql, ArrayList Parameters, ArrayList alCommandType, string ConnectionStr)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConnectionStr;
            conn.Open();

            SqlCommand myCommand = conn.CreateCommand();
            SqlTransaction myTrans;

            // Start a local transaction
            myTrans = conn.BeginTransaction();
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            myCommand.Connection = conn;
            myCommand.Transaction = myTrans;

            try
            {
                for (int i = 0; i < alSql.Count; i++)
                {
                    myCommand.CommandText = alSql[i].ToString();
                    // Set the command type
                    myCommand.CommandType = (CommandType)alCommandType[i];
                    object pars = Parameters[i];

                    // Attach the command parameters if they are provided
                    if (pars != null)
                    {
                        myCommand.Parameters.Clear();
                        SqlHelper.AttachParameters(myCommand, (SqlParameter[])pars);
                    }
                    myCommand.ExecuteNonQuery();
                }
                myTrans.Commit();
                return true;
            }
            catch (Exception e)
            {
                try
                {
                    myTrans.Rollback();
                }
                catch (SqlException ex)
                {
                    if (myTrans.Connection != null)
                    {
                        Console.WriteLine("An exception of type " + ex.GetType() +
                            " was encountered while attempting to roll back the transaction.");

                    }
                }

                Console.WriteLine("An exception of type " + e.GetType() +
                    " was encountered while inserting the data.");
                Console.WriteLine("Neither record was written to database.");
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 在事務中執行帶參數的sql語句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static bool excuteTrans(ArrayList alSql, ArrayList Parameters, ArrayList alCommandType)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = WriteConnectionStr();
            conn.Open();

            SqlCommand myCommand = conn.CreateCommand();
            SqlTransaction myTrans;

            // Start a local transaction
            myTrans = conn.BeginTransaction();
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            myCommand.Connection = conn;
            myCommand.Transaction = myTrans;

            try
            {
                for (int i = 0; i < alSql.Count; i++)
                {
                    myCommand.CommandText = alSql[i].ToString();
                    // Set the command type
                    myCommand.CommandType = (CommandType)alCommandType[i];
                    object pars = Parameters[i];

                    // Attach the command parameters if they are provided
                    if (pars != null)
                    {
                        myCommand.Parameters.Clear();
                        SqlHelper.AttachParameters(myCommand, (SqlParameter[])pars);
                    }
                    myCommand.ExecuteNonQuery();
                }
                myTrans.Commit();
                return true;
            }
            catch (Exception e)
            {
                try
                {
                    myTrans.Rollback();
                }
                catch (SqlException ex)
                {
                    if (myTrans.Connection != null)
                    {
                        Console.WriteLine("An exception of type " + ex.GetType() +
                            " was encountered while attempting to roll back the transaction.");
                        Log.WriteLog("log.txt", "An exception of type " + ex.GetType() +
                            " was encountered while attempting to roll back the transaction.");
                    }
                }

                Console.WriteLine("An exception of type " + e.GetType() +
                    " was encountered while inserting the data.");
                Log.WriteLog("log.txt", "An exception of type " + e.GetType() +
                    " was encountered while inserting the data.");
                Console.WriteLine("Neither record was written to database.");
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

		/// <summary>
		/// 在事務中執行帶參數的sql語句
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="commandParameters"></param>
		/// <returns></returns>
		public static bool excuteTrans(ArrayList alSql, ArrayList Parameters)
		{
			SqlConnection conn = new SqlConnection();
			conn.ConnectionString = WriteConnectionStr();
			conn.Open();

			SqlCommand myCommand = conn.CreateCommand();
			SqlTransaction myTrans;

			// Start a local transaction
			myTrans = conn.BeginTransaction();
			// Must assign both transaction object and connection
			// to Command object for a pending local transaction
			myCommand.Connection = conn;
			myCommand.Transaction = myTrans;

			try
			{
				for (int i=0;i<alSql.Count;i++)
				{
					myCommand.CommandText = alSql[i].ToString();
					// Set the command type
					myCommand.CommandType = CommandType.Text;
					object pars = Parameters[i];

					// Attach the command parameters if they are provided
					if (pars != null)
					{
						myCommand.Parameters.Clear();
						SqlHelper.AttachParameters(myCommand, (SqlParameter[])pars);
					}
					myCommand.ExecuteNonQuery();
				}
				myTrans.Commit();
				return true;
			}
			catch(Exception e)
			{
				try
				{
					myTrans.Rollback();
				}
				catch (SqlException ex)
				{
					if (myTrans.Connection != null)
					{
						Console.WriteLine("An exception of type " + ex.GetType() +
							" was encountered while attempting to roll back the transaction.");
						
					}
				}
    
				Console.WriteLine("An exception of type " + e.GetType() +
					" was encountered while inserting the data.");
				Console.WriteLine("Neither record was written to database.");
				return false;
			}
			finally 
			{
				conn.Close();
			}			
		}


		#endregion

		#region 新聞特殊元素的處理
		/// <summary>
		/// 獲取制定新聞特殊元素的定義信息及內容
		/// </summary>
		/// <param name="NewsID">新聞ID</param>
		/// <param name="strXMLPath">存放版塊定義信息的XML文件路徑</param>
		/// <returns></returns>
		/*
		public static DataTable GetSpecialData_NewsID(int NewsID, string strXMLPath)
		{
			DataTable tabSpecialData = new DataTable();
			string strSQL = string.Format("SELECT NewsBoardID, Special FROM [News]WHERE NewsID = {0}", NewsID);
			SqlDataReader dr = Fudan.Stu.Data.ExecuteReader(strSQL);
			if (dr.Read())
			{
				int BoardID = int.Parse(dr["NewsBoardID"].ToString());
				string strSpecial = dr["Special"].ToString();
				tabSpecialData = GetSpecialData_BoardID(BoardID, strXMLPath, strSpecial);
			}
			return tabSpecialData;			
		}
		*/
		
		/// <summary>
		/// 獲取制定新聞特殊元素的定義信息及內容
		/// </summary>
		/// <param name="BoardID">該新聞所屬版塊</param>
		/// <param name="strXMLPath">存放版塊定義信息的XML文件路徑</param>
		/// <param name="strSpecial">該新聞Special字段的內容</param>
		/// <returns></returns>
		public static DataTable GetSpecialData_BoardID(int BoardID, string strXMLPath, string strSpecial)
		{
			DataTable tabSpecialData = new DataTable();
			tabSpecialData.Columns.Add("Title");
			tabSpecialData.Columns.Add("Tag");
				
			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(strXMLPath);

				XmlNodeList nodeList;
				nodeList=doc.SelectNodes("/NewDataSet/Board[BoardID='" + BoardID.ToString() + "']/Special"); 
				
				foreach (XmlNode SpecialData in nodeList)
				{                
					DataRow newRow  = tabSpecialData.NewRow(); 
					newRow["Title"] = SpecialData.ChildNodes[0].InnerText;
					newRow["Tag"] = GetStrInTag(strSpecial, SpecialData.ChildNodes[1].InnerText);
					tabSpecialData.Rows.Add(newRow);
				}
			}
			catch
			{
			}
			return tabSpecialData;
		}		

		/// <summary>
		/// 獲取指定標籤內的文本
		/// </summary>
		/// <param name="Str">文本源</param>
		/// <param name="Tag">標籤</param>
		/// <returns></returns>
		public static string GetStrInTag(string Str,string Tag)
		{
			int intStart = Str.IndexOf("<" + Tag + ">");
			int intEnd	 = Str.IndexOf("</" + Tag + ">");
			if(intStart == -1 || intEnd == -1)
				return "";
			return Str.Substring(intStart + Tag.Length + 2, intEnd - intStart - Tag.Length - 2);
		}
		#endregion
		
		#region 這部分函數不要用
		/// <summary>
		/// 獲取單個值,無結果則返回null(暫時不要用這個函數)
		/// </summary>
		/// <param name="com">Sql命令對像</param>
		/// <returns></returns>
		public static object getDataScalar(SqlCommand com)
		{
			object result=null;
			try
			{
				com.Connection.Open();
				result=com.ExecuteScalar();
			}
			finally
			{
				com.Connection.Close();
			}
			return result;
		}

		/// <summary>
		/// 獲取一行數據,無結果返回空行(暫時不要用這個函數)
		/// </summary>
		/// <param name="com">Sql命令對像</param>
		/// <returns></returns>
		public static DataRow getDataRow(SqlCommand com)
		{
			DataTable dt = getDataTable(com);
			if (dt.Rows.Count != 0)
				return dt.Rows[0];
			return dt.NewRow();
		}

		/// <summary>
		/// 獲取一個數據表格,無結果返回空表(暫時不要用這個函數)
		/// </summary>
		/// <param name="sql">Sql命令對像</param>
		/// <returns></returns>
		public static DataTable getDataTable(SqlCommand com)
		{
			DataSet  ds = getDataSet(com);
			if (ds.Tables.Count != 0)
				return ds.Tables[0]; 			
			return new DataTable();
		}
				
		/// <summary>
		/// 獲取一個數據集,無結果返回空的數據集(暫時不要用這個函數)
		/// </summary>
		/// <param name="sql">Sql命令對像</param>
		/// <returns></returns>
		public static DataSet getDataSet(SqlCommand com)
		{
			SqlDataAdapter ad=new SqlDataAdapter(com);
			DataSet ds=new DataSet();
			ad.Fill(ds);
			return ds;
		}
			
		/// <summary>
		/// 執行一條Sql查詢語句(暫時不要用這個函數,用ExecuteNonQuery)
		/// </summary>
		/// <param name="sql">Sql查詢語句</param>
		public static void ExecuteSQL(string sql)
		{
			SqlConnection con=new SqlConnection(WriteConnectionStr());
			SqlCommand com=new SqlCommand(sql,con);
			try
			{
				con.Open();
				com.ExecuteNonQuery();
			}
			finally
			{
				con.Close();
			}
		}	
	
		/// <summary>
		/// 執行Sql命令對像(暫時不要用這個函數,用ExecuteNonQuery)
		/// </summary>
		/// <param name="com">Sql命令對像</param>
		public static void ExecuteCommand(SqlCommand com)
		{
			try
			{
				com.Connection.Open();
				com.ExecuteNonQuery();
			}
			finally
			{
				com.Connection.Close();
			}
		}			
			
		/// <summary>
		/// 獲取一個Sql命令對像,該命令對像類型為存儲過程(暫時不要用這個函數)
		/// </summary>
		/// <param name="StrdPrcdrName">存儲過程名</param>
		/// <returns></returns>		
		public static SqlCommand getSqlComOfStrdPrcdr(string StrdPrcdrName)
		{
			SqlConnection con=new SqlConnection(Data.WriteConnectionStr());
			SqlCommand com=new SqlCommand(StrdPrcdrName,con);
			com.CommandType=CommandType.StoredProcedure;
			return com;
		}		
		
		/// <summary>
		/// 執行存儲過程(暫時不要用這個函數,用ExecuteNonQuery)
		/// </summary>
		/// <param name="com">存儲過程名</param>
		public static void ExecuteStrdPrcdr(string StrdPrcdrName)
		{
			SqlCommand com = Data.getSqlComOfStrdPrcdr(StrdPrcdrName);
			try
			{
				com.Connection.Open();
				com.ExecuteNonQuery();
			}
			finally
			{
				com.Connection.Close();
			}
		}
		#endregion
	}

	#endregion

	#region 應用方法庫
	/// <summary>
	/// 應用方法庫
	/// </summary>
	public class Methods
	{
        private static Hashtable hprocess;
        private static Hashtable hcustomerProcess;
		#region Jeason添加方法
		/// <summary>
		/// 設定dropdownlist控件顯示設定項
		/// </summary>
		/// <param name="DDL"></param>
		/// <param name="ChangeVal"></param>
		public static void DropDownListChange(DropDownList DDL,string ChangeVal)
		{
			for(int i=0;i<DDL.Items.Count;i++)
			{
				DDL.Items[i].Selected = false;
			}

			for(int i=0;i<DDL.Items.Count;i++)
			{
				if (DDL.Items[i].Value.Equals(ChangeVal))
				{
					DDL.Items[i].Selected = true;
					break;
				}
			}
		}
		#endregion
		#region 常用判斷函數
		/// <summary>
		/// 判斷文章是否是公開的(Article表)
		/// </summary>
		/// <param name="ArticleID"></param>
		/// <returns></returns>
		public static bool IsPublished_Article(int ArticleID)
		{
			SqlCommand com=Data.getSqlComOfStrdPrcdr("IsPublished_Article");
			com.Parameters.Add("@ArticleID",SqlDbType.BigInt).Value=ArticleID;
			object Published = Data.getDataScalar(com);
			if (Published == null)
				return false;
			return bool.Parse(Published.ToString());
		}

		/// <summary>
		/// 判斷文章是否是當前用戶發表的(Article表)
		/// </summary>
		/// <param name="UserID"></param>
		/// <param name="ArticleID"></param>
		/// <returns></returns>
		public static bool IsSelf_Article(string UserID,int NewsID)
		{
			SqlCommand com=Data.getSqlComOfStrdPrcdr("IsSelf_Article");
			com.Parameters.Add("@ArticleID",SqlDbType.BigInt).Value=NewsID;
			com.Parameters.Add("@SubmitterID",SqlDbType.VarChar,15).Value=UserID;
			
			int count = 0;
			count = (int)Data.getDataScalar(com);
			return count>0;
		}
		
		/// <summary>
		/// 獲取文章所屬版塊ID(News表)
		/// </summary>
		/// <param name="NewsID"></param>
		/// <returns></returns>
		public static int GetBoardID(int NewsID)
		{
			SqlCommand com=Data.getSqlComOfStrdPrcdr("GetBoardID");
			com.Parameters.Add("@NewsID",SqlDbType.BigInt).Value=NewsID;
			object BoardID=Data.getDataScalar(com);
			if(BoardID==null)
				return -1;
			return int.Parse(BoardID.ToString());
		}

		/// <summary>
		/// 獲取版塊所屬頻道(表)
		/// </summary>
		/// <param name="BoardID"></param>
		/// <returns></returns>
		public static int GetChannelID(int BoardID)
		{
			SqlCommand com=Data.getSqlComOfStrdPrcdr("GetChannelID");
			com.Parameters.Add("@BoardID",SqlDbType.BigInt).Value=BoardID;
			object ChannelID=Data.getDataScalar(com);
			if(ChannelID==null)
				return -1;
			return int.Parse(ChannelID.ToString());
		}

		/// <summary>
		/// 判斷文章是否存在(Article表)
		/// </summary>
		/// <param name="ArticleID"></param>
		/// <returns></returns>
		public static bool Exists_Article(int ArticleID)
		{
			SqlCommand com=Data.getSqlComOfStrdPrcdr("Exists_Article");
			com.Parameters.Add("@ArticleID",SqlDbType.BigInt).Value=ArticleID;
			
			int count = 0;
			count = (int)Data.getDataScalar(com);
			return count>0;
		}		

		/// <summary>
		/// 判斷信息是否存在(News表)
		/// </summary>
		/// <param name="NewsID"></param>
		/// <returns></returns>
		public static bool Exists_News(int NewsID)
		{
			SqlCommand com=Data.getSqlComOfStrdPrcdr("AppInfo_Exists_News");
			com.Parameters.Add("@NewsID",SqlDbType.BigInt).Value=NewsID;
			
			int count = 0;
			count = (int)Data.getDataScalar(com);
			return count>0;
		}	

		/// <summary>
		/// 判斷評論是否存在(NewsComment表)
		/// </summary>
		/// <param name="NewsCommentID"></param>
		/// <returns></returns>
		public static bool Exists_NewsComment(int NewsCommentID)
		{
			SqlCommand com=Data.getSqlComOfStrdPrcdr("AppInfo_Exists_NewsComment");
			com.Parameters.Add("@NewsCommentID",SqlDbType.BigInt).Value=NewsCommentID;
			
			int count = 0;
			count = (int)Data.getDataScalar(com);
			return count>0;
		}	
	
		/// <summary>
		/// 判斷信息是否是已審核的(News表)
		/// </summary>
		/// <param name="NewsID"></param>
		/// <returns></returns>
		public static bool IsPublished_News(int NewsID)
		{
			SqlCommand com=Data.getSqlComOfStrdPrcdr("AppInfo_IsPublished_News");
			com.Parameters.Add("@NewsID",SqlDbType.BigInt).Value=NewsID;
			object Published = Data.getDataScalar(com);
			if(Published == null)
				return false;
			else 
				return bool.Parse(Published.ToString());
		}

		/// <summary>
		/// 判斷信息是否是當前用戶發表的(News表)
		/// </summary>
		/// <param name="UserID"></param>
		/// <param name="NewsID"></param>
		/// <returns></returns>
		public static bool IsSelf_News(string UserID,int NewsID)
		{
			SqlCommand com=Data.getSqlComOfStrdPrcdr("AppInfo_IsSelf_News");
			com.Parameters.Add("@NewsID",SqlDbType.BigInt).Value=NewsID;
			com.Parameters.Add("@SubmitterID",SqlDbType.VarChar,15).Value=UserID;
			
			int count = 0;
			count = (int)Data.getDataScalar(com);
			return count>0;
		}
		
		#endregion			
		
		//yuding
		#region zq
		public static IList GetSmallPartNo(string txt)
		{
			IList alstr = new ArrayList();
			if (txt.IndexOf("-") > -1)
			{
				string first = txt.Substring(0, txt.IndexOf("-"));
				string second = txt.Substring(txt.IndexOf("-") + 1, txt.Length - txt.IndexOf("-") - 1);
				for (int i = Convert.ToInt32(first);i <= Convert.ToInt32(second); i++)
				{
					alstr.Add(i.ToString());
				}
			}
			
			if (txt.IndexOf(",") > -1)
			{
				string[] nos = txt.Split(',');
				for(int i = 0; i < nos.Length; i++)
				{
					alstr.Add(nos[i]);
				}
			}

			if (txt.IndexOf("-") == -1 && txt.IndexOf(",") == -1)
			{
				alstr.Add(txt);
			}

			return alstr;
		}
		#endregion
		

		

       
        public static string FormatMinuteToDayHourMinute(int minutes, int hoursperday)
        {
            int Day = (int)(minutes / (60 * hoursperday));
            int Hour = (int)(minutes % (60 * hoursperday) / 60);
            int Minutes = (int)(minutes % (60 * hoursperday) % 60);
            return string.Format("{0}d{1}:{2}", Day, Hour, Minutes);
        }


        public static string TranslateStatusName(string statusid)
        {
            int locate = -1;
            for (int i = 0; i < PTSetting.STATUSID.Length; i++)
            {
                if (statusid.Equals(PTSetting.STATUSID[i]))
                { locate = i; }
            }
            if (locate != -1)
            {
                return PTSetting.STATUSNAME[locate];
            }
            else
            {
                return "";
            }
        }

        public static string TranslateStatusColor(string statusid)
        {
            int locate = -1;
            for (int i = 0; i < PTSetting.STATUSID.Length; i++)
            {
                if (statusid.Equals(PTSetting.STATUSID[i]))
                { locate = i; }
            }
            if (locate != -1)
            {
                return PTSetting.STATUSCOLOR[locate];
            }
            else
            {
                return "";
            }
        }

        public static string GetSeriesNumber(string NoId, string prefix)
        {
            if (string.IsNullOrEmpty(NoId) || string.IsNullOrEmpty(prefix))
                return null;

            int prefixPos = NoId.LastIndexOf(prefix);

            if (prefixPos >= 0 && prefixPos + prefix.Length < NoId.Length)
            {
                string seriesStr = NoId.Substring(prefixPos + prefix.Length);
                // 检查提取的字符串是否全部是数字
                if (seriesStr.All(char.IsDigit))
                {
                    return seriesStr; // 返回原始格式（保持前导0）
                }
            }
            return null; // 返回null而不是原始NoId，更明确表达无法提取序列号的情况
        }

        public static string GetNextSeriesNumber(string currentNoId, string prefix)
        {
        

            string seriesStr = GetSeriesNumber(currentNoId, prefix);

            if (seriesStr == null)
                return currentNoId;

            int currentNumber = int.Parse(seriesStr);
            int nextNumber = currentNumber + 1;

            // 使用与原始序列号相同的位数格式
            string nextSeriesStr = nextNumber.ToString().PadLeft(seriesStr.Length, '0');

            // 找到前缀的位置
            int prefixPos = currentNoId.LastIndexOf(prefix);

            // 构建新的PartNo_Id
            string newPartNoId = currentNoId.Substring(0, prefixPos) + prefix + nextSeriesStr;

            return newPartNoId;
        }
    }
	#endregion

	
}