#region 測鎢恅璃佽隴
/* 
	Title	BookingSystem
	Author	抸膘集(jandon)
	Create	Sep.24,2004
	Version 1.0  
 */
#endregion

using System;
using System.Drawing;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using System.Xml;
using ModuleWorkFlow.WebView;
using Microsoft.ApplicationBlocks.Data;


using System.Web.UI.WebControls;

using ModuleWorkFlow.BLL;
using ModuleWorkFlow.Model;
using ModuleWorkFlow.BLL.System;
using ModuleWorkFlow.Model.System;
using Utility;

namespace ModuleWorkFlow.business
{
    //系統命名規則定義
    //一、web頁面控件命名規則
    //   1、控件名稱為"???_*"，"???"表示控件類型，
    ///////textbox="tbx"
    ///////dropdownlist="drp"
    ///////label="lab"
    ///////RadioButton="rad"

    //二、類中方法變量命名規則
    /////1、方法命名格則
    ////////私有方法名稱前面有下劃線"_",首字母小寫
    ////////public方法

    public class Setting
    {
        public static int DAYS = 30;
        public static int HOURS = 24;

        public static string[] DEFAULTPROCESS = { "PrePare", "Grind", "Milling", "Cut", "CNC", "EDM", "Flash" };
        public static string[] DEFAULTELECTRODEPROCESS = { "Cut", "CNCE", "QC", "Flash" };

        public static string[] PROCESSID = { "Design", "Merchandise", "Lathe", "Drill", "BefroeCNC", "CNCHeat", "AfterCNC", "Engraving", "QC", "Texture", "Grind", "Milling", "PrePare", "Cut", "SlowCut", "CNC", "CNCN", "EDM", "Flash", "Assemble", "CNCE", "PlaneEDM", "CAM", "burn" };
        public static string[] PROCESSNAME = { Translate.translateString("設計"), Translate.translateString("外購"), Translate.translateString("車床"), Translate.translateString("鑽床"), Translate.translateString("CNC熱前"), Translate.translateString("熱處理"), Translate.translateString("CNC熱後"), Translate.translateString("雕刻"), Translate.translateString("品管"), Translate.translateString("咬花"), Translate.translateString("磨床"), Translate.translateString("銑床"), Translate.translateString("備料"), Translate.translateString("快絲"), Translate.translateString("慢絲"), Translate.translateString("CNC加工中心"), Translate.translateString("CNC普通機"), Translate.translateString("放電"), Translate.translateString("打光"), Translate.translateString("組立"), Translate.translateString("CNC電極機"), Translate.translateString("鏡面放電"), Translate.translateString("CAM"), Translate.translateString("燒焊") };
        public static string[] PROCESSBARCODE = { "SHEJI", "WAIGOU", "CHECHUANG", "ZUANCHUANG", "CNCREQIAN", "RECHULI", "CNCREHOU", "DIAOKE", "PINGUAN", "YAOHUA", "MOCHUANG", "XICHUANG", "BEILIAO", "KUAISI", "MANSI", "CNC", "CNCPUTONGJI", "FANGDIAN", "DAGUANG", "ZULI", "CNCDIANJIJI", "JINGMIANFANGDIAN", "CAM", "SHAOHAN" };

        public static string[] MATERIALTYPEID = { "Production", "Part", "Try" };
        public static string[] MATERIALTYPENAME = { Translate.translateString("產品"), Translate.translateString("零件"), Translate.translateString("試模") };

        public static string[] STATUSID = { "Implementation", "Working", "Pending", "Rejected", "Cancelled", "Reopened", "Delay", "Ready", "Holdon", "JIEDAN", "XIAODAN", "CHUCHANG", "HUICHANG", "FANHUI", "Rezuli" };
        public static string[] STATUSNAME = { Translate.translateString("完成"), Translate.translateString("進行中"), Translate.translateString("未就緒"), Translate.translateString("拒絕"), Translate.translateString("取消"), Translate.translateString("重新開始"), Translate.translateString("延遲"), Translate.translateString("就緒"), Translate.translateString("暫停"), Translate.translateString("接單"), Translate.translateString("消單"), Translate.translateString("出廠"), Translate.translateString("回廠"), Translate.translateString("返回"), Translate.translateString("組立返修") };
        public static string[] STATUSCODE = { "WANCHENG", "JINXINGZHONG", "WEIJIUXU", "JUJUE", "QUXIAO", "CHONGXINKAISHI", "YANCHI", "JIUXU", "ZANTING", "JIEDAN", "XIAODAN", "CHUCHANG", "HUICHANG", "FANHUI", "ZULIFANXIU" };
        public static string[] STATUSCOLOR = { "&H00FF00 ", "&H99ff66", "&HFFFF00", "&HFF0000", "&H787878", "&HCC0000", "&HFF0000", "&H00FFFF", "&HB7B7B7", "&HFFB300", "&HB0B9FF  ", "&HCCFF00", "&H00FF00", "&HB7B7B7", "&HFF3FD7" };


        //		MYD070615
        public static string[] ACTIONBARCODE = { "KAISHI", "ZANTING", "JIESHU", "FANGONG", "RUKU", "CHUKU", "CHUCHANG", "HUICHANG", "FANHUI", "XIAODAN", "JIEDAN", "CHUHUO", "DAIPAN", "BULIANG" };
        public static string[] ACTIONNAME = { Translate.translateString("開始"), Translate.translateString("暫停"), Translate.translateString("結束"), Translate.translateString("返工"), Translate.translateString("入庫"), Translate.translateString("出庫"), Translate.translateString("出廠"), Translate.translateString("回廠"), Translate.translateString("返回"), Translate.translateString("消單"), Translate.translateString("接單"), Translate.translateString("出貨"), Translate.translateString("待判"), Translate.translateString("不良") };

        #region 條形碼命令規則
        //模式
        public static string[] BARCODE_CODE = { "A", "B", "C", "D", "Z", "M" };
        public static string[] BARCODE_COMMAND = { "USERNO", "MODULEPART", "PROCESS", "ACTION", "MODULE", "MACHINE" };

        #endregion


        public static Color SELECTCOLOR = Color.PowderBlue;
        public static Color FACTDATECOLOR = Color.DimGray;
        public static Color NormalColor = Color.White;
        public static Color DelayColor = Color.Red;
        public static Color FinishColor = Color.Green;

        public static string ALLValue = "all";
        public static string ALLText = Translate.translateString("全部");

        public static string STATUSCOLOR_DONE = "GREEN";
        public static string STATUSCOLOR_DOING = "BLUE";
        public static string STATUSCOLOR_READY = "YELLOW";

        public static string INFILENAME = "Sample.xls";

        public static string TreeViewRoot = "webctrl_client/1_0/treeimages/";
    }

    #region 系統原有Info類
    /// <summary>
    /// 韜靡諾潔鏡扴
    /// </summary>
    public class Info
    {
        public const string Title = "葩筒悝汜厙";
        public const string URL = "http://stu.fudan.edu.cn";
        public const string Address = "珔珓湴瞼503弅";
        public const string Tel = "021-55664425";
    }
    #endregion

    #region 系統原有Settings類
    /// <summary>
    /// □擁扢離
    /// </summary>
    public class Settings
    {
        #region 統杅
        public static string[] Channel = { "苺埶□耋", "陔恓□耋", "軓氈□耋", "汜魂□耋", "悝炾□耋", "蚳枙□耋", "Ａ坻" };
        public static string[] ChannelColor = { "#f7df9b", "#f7df9b", "#f7df9b", "#f7df9b", "#f7df9b", "#f7df9b", "#f7df9b" };
        public static string RobotID = "SYSTEM_NewsRobo";
        public static int HotLimit = 20;	//HotNews萸僻薹狟癹硉
        public static int StuBoardID = 65;	//悝汜厙陓洘楷票唳輸
        public static int ForumBoardID = 1;	//蹦抭跦唳輸
        #endregion

        #region enum 隅砱
        public enum BoardTypes : byte { PlainText = 0, Html = 1, FileLink = 2, FileInDatabase = 3, Reference = 4 }
        public static string[] Grades = { "2000撰掛褪", "2001撰掛褪", "2002撰掛褪", "2003撰掛褪", "2004撰掛褪", "旃噶汜﹜痔尪汜", "Ａ坻葩筒悝汜", "葩筒橾呇", "葩筒湮悝苺衭", "准葩筒□埜" };
        #endregion
    }
    #endregion

    #region 認證方法
    /// <summary>
    /// 蚚誧□痐
    /// </summary>
    public class Identity
    {
        #region 褒伎隅砱
        /// <summary>
        /// 褒伎
        /// </summary>
        public struct Roles
        {
            public const string Unknown = "Unknown";
            public const string Visitor = "Visitor";
        }
        #endregion

        #region Logon
        public static void UserLogon(string UserID, string IP)
        {
            System.Data.SqlClient.SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@UserID", SqlDbType.VarChar, 15);
            pars[0].Value = UserID;
            pars[1] = new SqlParameter("@LastLogonIp", SqlDbType.VarChar, 15);
            pars[1].Value = IP;
            Data.ExecuteNonQuery("UserLogon", CommandType.StoredProcedure, pars);
        }
        #endregion

        #region Logonout
        public static void UserLogonout(string UserID)
        {
            System.Data.SqlClient.SqlParameter[] pars = new SqlParameter[1];
            pars[0] = new SqlParameter("@UserID", SqlDbType.VarChar, 15);
            pars[0].Value = UserID;
            Data.ExecuteNonQuery("UserLogonout", CommandType.StoredProcedure, pars);
        }
        #endregion


        #region 認證方法
        /// <summary>
        /// 桄痐蚚誧
        /// </summary>
        /// <param name="UserID">蚚誧靡</param>
        /// <param name="Password">躇鎢</param>
        /// <returns></returns> 
        public static bool Authenticate(string UserID, string Password)
        {
            SqlConnection con = new SqlConnection(Data.ReadConnectionStr());

            SqlCommand com = new SqlCommand("UserAuthenticate", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add("@UserID", SqlDbType.VarChar, 15);
            com.Parameters.Add("@Password", SqlDbType.VarChar, 15);
            com.Parameters[0].Value = UserID;
            com.Parameters[1].Value = Password;

            int count;

            try
            {
                con.Open();
                count = (int)com.ExecuteScalar();
            }
            finally
            {
                con.Close();
            }

            return (count > 0);
        }

        /// <summary>
        /// 瓚剿蚚誧岆瘁湔婓
        /// </summary>
        /// <param name="UserID">蚚誧靡</param>
        /// <returns></returns>
        public static bool UserExists(string UserID)
        {
            SqlConnection con = new SqlConnection(Data.ReadConnectionStr());

            SqlCommand com = new SqlCommand("UserExists", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add("@UserID", SqlDbType.VarChar, 15);
            com.Parameters[0].Value = UserID;

            int count;
            try
            {
                con.Open();
                count = (int)com.ExecuteScalar();
            }
            finally
            {
                con.Close();
            }

            return (count > 0);
        }

        /// <summary>
        /// 瓚剿婓硌隅唳輸蜆蚚誧岆瘁湔婓(扡摯UserRole桶,珋婓褫夔祥蚚涴桲桶)
        /// </summary>
        /// <param name="UserID">蚚誧靡</param>
        /// <param name="BoardID">唳輸ID</param>
        /// <returns></returns>
        public static bool UserExists(string UserID, int BoardID)
        {
            SqlCommand com = Data.getSqlComOfStrdPrcdr("UserExists_Board");
            com.Parameters.Add("@UserID", SqlDbType.VarChar, 15).Value = UserID;
            com.Parameters.Add("@BoardID", SqlDbType.Int).Value = BoardID;

            int count;
            try
            {
                com.Connection.Open();
                count = (int)com.ExecuteScalar();
            }
            finally
            {
                com.Connection.Close();
            }
            return (count > 0);
        }

        /// <summary>
        /// 鳳□褒伎□癹硉(扡摯Roles桶,珋婓褫夔祥蚚涴桲桶)
        /// </summary>
        /// <param name="RoleID">褒伎ID</param>
        /// <returns></returns> 
        public static int getRolePrivilliage(string RoleID)
        {
            SqlCommand com = Data.getSqlComOfStrdPrcdr("getRolePrivilliage");
            com.Parameters.Add("@RoleID", SqlDbType.VarChar, 50).Value = RoleID;
            object RolePrivilliage = Data.getDataScalar(com);
            if (RolePrivilliage == null)
                return 0;
            return int.Parse(RolePrivilliage.ToString());
        }
        #endregion

        #region □癹鏡扴賦凳
        /// <summary>
        /// 黍□癹
        /// </summary>
        public enum ReadLevel { CannotRead = 0, ReadTitle = 1, ReadContent = 2 }
        /// <summary>
        /// 蜊□癹
        /// </summary>	
        public enum EditLevel { CannotEdit = 0, EditSelf, EditAll, EditProperty /*離階脹扽俶*/}
        /// <summary>
        /// 刉壺□癹
        /// </summary>
        public enum DeleteLevel { CannontDelete = 0, DeleteSelf, DeleteAll }

        /// <summary>
        /// □癹鏡扴賦凳
        /// </summary>
        public struct BoardAccessLevel
        {
            /// <summary>
            /// 黍鼠羲陓洘
            /// </summary>
            public ReadLevel ReadPassed;
            /// <summary>
            /// 黍准鼠羲陓洘
            /// </summary>
            public ReadLevel ReadNotPassed;
            /// <summary>
            /// 蜊鼠羲陓洘
            /// </summary>
            public EditLevel EditPassed;
            /// <summary>
            /// 蜊准鼠羲陓洘
            /// </summary>
            public EditLevel EditNotPassed;
            /// <summary>
            /// 刉壺鼠羲陓洘
            /// </summary>
            public DeleteLevel DeletePassed;
            /// <summary>
            /// 刉壺准鼠羲陓洘
            /// </summary>
            public DeleteLevel DeleteNotPassed;
            /// <summary>
            /// 岆瘁撿衄機瞄陓洘腔□癹
            /// </summary>
            public bool Pass;
            /// <summary>
            /// 岆瘁撿衄楷票陓洘腔□癹
            /// </summary>
            public bool Append;
            /// <summary>
            /// 岆瘁撿衄奪燴□癹
            /// </summary>
            public bool Manage;
            //public bool Search;

            //蘇□凳婖□窒0ㄛ拸□睡□癹
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
            /// 凳婖滲杅ㄛ□統杅
            /// </summary>
            /// <param name="aReadPassed">黍鼠羲陓洘腔□癹硉</param>
            /// <param name="aReadNotPassed">黍准鼠羲陓洘腔□癹硉</param>
            /// <param name="aEditPassed">蜊鼠羲陓洘腔□癹硉</param>
            /// <param name="aEditNotPassed">蜊准鼠羲陓洘腔□癹硉</param>
            /// <param name="aDeletePassed">刉壺鼠羲陓洘腔□癹硉</param>
            /// <param name="aDeleteNotPassed">刉壺准鼠羲陓洘腔□癹硉</param>
            /// <param name="aPass">岆瘁撿衄機瞄陓洘腔□癹</param>
            /// <param name="aAppend">岆瘁撿衄楷票陓洘腔□癹</param>
            /// <param name="aManage">岆瘁撿衄奪燴□癹</param>
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
                this.ReadPassed = aReadPassed;
                this.ReadNotPassed = aReadNotPassed;
                this.EditPassed = aEditPassed;
                this.EditNotPassed = aEditNotPassed;
                this.DeleteNotPassed = aDeleteNotPassed;
                this.DeletePassed = aDeletePassed;
                this.Pass = aPass;
                this.Append = aAppend;
                this.Manage = aManage;
            }

            /// <summary>
            /// 蕭探凳婖滲杅
            /// </summary>
            /// <param name="aClone"></param>
            public BoardAccessLevel(BoardAccessLevel aClone)
            {
                this.ReadPassed = aClone.ReadPassed;
                this.ReadNotPassed = aClone.ReadNotPassed;
                this.EditPassed = aClone.EditPassed;
                this.EditNotPassed = aClone.EditNotPassed;
                this.DeletePassed = aClone.DeletePassed;
                this.DeleteNotPassed = aClone.DeleteNotPassed;
                this.Pass = aClone.Pass;
                this.Append = aClone.Append;
                this.Manage = aClone.Manage;
                //this.Search=aClone.Search;
            }


            //都蚚BoardAccessLevel耀宒ㄛ眻諉覃蚚
            //public static BoardAccessLevel PublicRead = new BoardAccessLevel();
            //public static BoardAccessLevel 
        }
        #endregion
    }
    #endregion

    #region 關於頁面的常量定義
    namespace Pages
    {
        #region 出錯文件url地址
        /// <summary>
        /// 珜醱弇離
        /// </summary>
        public class Location
        {
            //public const string Home_Stu = "http://10.107.0.49";
            //public const string Home_Stu ="http://stu.fudan.edu.cn";
            public const string Home_Stu = "..";

            /// <summary>
            /// 蛁聊珜 
            /// </summary>
            public const string Page_Register = Home_Stu + "/register.aspx";

            /// <summary>
            /// 陔恓等跺戲醴恅梒蹈桶
            /// </summary>
            public const string Page_List = Home_Stu + "/list.aspx";

            /// <summary>
            /// 統杅渣昫
            /// </summary>
            public const string Page_Error_Para = Home_Stu + "/error/para.aspx";

            ///<summary>
            /// □癹祥逋
            /// </summary>
            public const string Page_Error_Limit = Home_Stu + "/error/limit.aspx";

            /// <summary>
            /// 珜醱祥湔婓
            /// </summary>
            public const string Page_Error_None = Home_Stu + "/error/none.aspx";

            /// <summary>
            /// 膘扢笢
            /// </summary>
            public const string Page_Error_Construction = Home_Stu + "/error/construction.aspx";

            /// <summary>
            /// 籵蚚渣昫 	
            /// </summary>		
            public const string Page_Error = Home_Stu + "/error/error.aspx";

            /// <summary>
            /// 恁寁蛌砃珜醱
            /// </summary>
            public const string Name_Switch = "Switch.aspx";
        }
        #endregion

        #region Switch (Front Controller 耀宒)
        /// <summary>
        /// Switch (Front Controller 耀宒)
        /// </summary>
        public class Switch
        {
            /*粕等韜鍔耀宒ㄩ
             * Transfer眻諉蔚掛珜醱QueryString換善狟珨跺珜醱˙
             * List婓Settings.Xml笢硌隅換懷腔Key
             * Custom赻隅砱梓□ㄛ剒賤昴
             * None拸統杅
             * 蘇□硉Transferㄗ祥剒婬Settings.Xml笢硌隅ㄘ
             * */
            public enum ParaMode { Transfer = 0, List, Custom, None };
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

    }

    #endregion

    #region 關於連接數據庫的類
    /// <summary>
    /// 杅擂溼恀
    /// </summary>
    public class Data
    {
        public static string constr;
        #region 數據庫聯接字符串定義
        /// <summary>
        /// 黍梛瘍蟀諉趼睫揹
        /// </summary>
        /// <returns></returns>
        public static string ReadConnectionStr()
        {
            //MYD070805
            //return "server=.;database=ModuleWorkFlow;uid=workflow;pwd=workflow";	
            return System.Configuration.ConfigurationSettings.AppSettings["MsSQLConnString"];

        }

        /// <summary>
        /// 迡梛瘍蟀諉趼睫揹
        /// </summary>
        /// <returns></returns>
        public static string WriteConnectionStr()
        {
            //MYD070805
            //return "server=.;database=ModuleWorkFlow;uid=workflow;pwd=workflow";
            return System.Configuration.ConfigurationSettings.AppSettings["MsSQLConnString"];

        }

        #endregion

        #region 硒俴韜鍔,殿隙忳荌砒腔俴杅
        /// <summary>
        /// 硒俴韜鍔,殿隙忳荌砒腔俴杅
        /// </summary>
        /// <param name="sql">Sql脤戙逄歷</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sql)
        {
            return SqlHelper.ExecuteNonQuery(WriteConnectionStr(), CommandType.Text, sql);
        }
        /// <summary>
        /// 硒俴韜鍔,殿隙忳荌砒腔俴杅
        /// </summary>
        /// <param name="sql">Sql脤戙逄歷</param>
        /// <param name="commandParameters">統杅杅郪</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sql, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteNonQuery(WriteConnectionStr(), CommandType.Text, sql, commandParameters);
        }
        /// <summary>
        /// 硒俴韜鍔,殿隙忳荌砒腔俴杅
        /// </summary>
        /// <param name="StrdPrcdrName">湔揣徹最靡</param>
        /// <param name="CommandType">沓CommandType.StoredProcedure</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string StrdPrcdrName, CommandType commandType)
        {
            return SqlHelper.ExecuteNonQuery(WriteConnectionStr(), commandType, StrdPrcdrName);
        }
        /// <summary>
        /// 硒俴韜鍔,殿隙忳荌砒腔俴杅
        /// </summary>
        /// <param name="StrdPrcdrName">湔揣徹最靡</param>
        /// <param name="commandType">沓CommandType.StoredProcedure</param>
        /// <param name="commandParameters">統杅杅郪</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string StrdPrcdrName, CommandType commandType, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteNonQuery(WriteConnectionStr(), commandType, StrdPrcdrName, commandParameters);
        }
        #endregion

        #region 鳳□杅擂黍□□
        /// <summary>
        /// 鳳□杅擂黍□□
        /// </summary>
        /// <param name="sql">Sql脤戙逄歷</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string sql)
        {
            return SqlHelper.ExecuteReader(ReadConnectionStr(), CommandType.Text, sql);
        }

        /// <summary>
        /// 鳳□杅擂黍□□
        /// </summary>
        /// <param name="sql">Sql脤戙逄歷</param>
        /// <param name="commandParameters">統杅杅郪</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string sql, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteReader(ReadConnectionStr(), CommandType.Text, sql, commandParameters);
        }

        /// <summary>
        /// 鳳□杅擂黍□□
        /// </summary>
        /// <param name="StrdPrcdrName">湔揣徹最靡</param>
        /// <param name="CommandType">沓CommandType.StoredProcedure</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string StrdPrcdrName, CommandType commandType)
        {
            return SqlHelper.ExecuteReader(WriteConnectionStr(), commandType, StrdPrcdrName);
        }

        /// <summary>
        /// 鳳□杅擂黍□□
        /// </summary>
        /// <param name="StrdPrcdrName">沓CommandType.StoredProcedure</param>
        /// <param name="commandType">湔揣徹最靡</param>
        /// <param name="commandParameters">統杅杅郪</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string StrdPrcdrName, CommandType commandType, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteReader(Data.WriteConnectionStr(), commandType, StrdPrcdrName, commandParameters);
        }

        #endregion

        #region 鳳□等跺硉
        /// <summary>
        /// 鳳□等跺硉,拸賦別寀殿隙null
        /// </summary>
        /// <param name="sql">Sql脤戙逄歷</param>
        /// <returns></returns>
        public static object getDataScalar(string sql)
        {
            return SqlHelper.ExecuteScalar(ReadConnectionStr(), CommandType.Text, sql);
        }

        /// <summary>
        /// 鳳□等跺硉,拸賦別寀殿隙null
        /// </summary>
        /// <param name="sql">Sql脤戙逄歷</param>
        /// <param name="commandParameters">統杅杅郪</param>
        /// <returns></returns>
        public static object getDataScalar(string sql, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteScalar(ReadConnectionStr(), CommandType.Text, sql, commandParameters);
        }

        /// <summary>
        /// 鳳□等跺硉,拸賦別寀殿隙null
        /// </summary>
        /// <param name="StrdPrcdrName">湔揣徹最靡</param>
        /// <param name="commandType">沓CommandType.StoredProcedure</param>
        /// <returns></returns>
        public static object getDataScalar(string StrdPrcdrName, CommandType commandType)
        {
            return SqlHelper.ExecuteScalar(WriteConnectionStr(), commandType, StrdPrcdrName);
        }

        /// <summary>
        /// 鳳□等跺硉,拸賦別寀殿隙null
        /// </summary>
        /// <param name="StrdPrcdrName">湔揣徹最靡</param>
        /// <param name="commandType">沓CommandType.StoredProcedure</param>
        /// <param name="commandParameters">統杅杅郪</param>
        /// <returns></returns>
        public static object getDataScalar(string StrdPrcdrName, CommandType commandType, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteScalar(WriteConnectionStr(), commandType, StrdPrcdrName, commandParameters);
        }

        #endregion

        #region 鳳□杅擂俴
        /// <summary>
        /// 鳳□珨俴杅擂,拸賦別殿隙null
        /// </summary>
        /// <param name="sql">Sql脤戙逄歷</param>
        /// <returns></returns>
        public static DataRow getDataRow(string sql)
        {
            DataTable dt = getDataTable(sql);
            if (dt.Rows.Count != 0)
                return dt.Rows[0];
            return null;
        }

        /// <summary>
        /// 鳳□珨俴杅擂,拸賦別殿隙null
        /// </summary>
        /// <param name="sql">Sql脤戙逄歷</param>
        /// <param name="commandParameters">統杅杅郪</param>
        /// <returns></returns>
        public static DataRow getDataRow(string sql, params SqlParameter[] commandParameters)
        {
            DataTable dt = getDataTable(sql, commandParameters);
            if (dt.Rows.Count != 0)
                return dt.Rows[0];
            return null;
        }

        /// <summary>
        /// 鳳□珨俴杅擂,拸賦別殿隙null
        /// </summary>
        /// <param name="StrdPrcdrName">湔揣徹最靡</param>
        /// <param name="commandType">沓CommandType.StoredProcedure</param>
        /// <returns></returns>
        public static DataRow getDataRow(string StrdPrcdrName, CommandType commandType)
        {
            DataTable dt = getDataTable(StrdPrcdrName, commandType);
            if (dt.Rows.Count != 0)
                return dt.Rows[0];
            return null;
        }

        /// <summary>
        /// 鳳□珨俴杅擂,拸賦別殿隙null
        /// </summary>
        /// <param name="StrdPrcdrName">湔揣徹最靡</param>
        /// <param name="commandType">沓CommandType.StoredProcedure</param>
        /// <param name="commandParameters">統杅杅郪</param>
        /// <returns></returns>
        public static DataRow getDataRow(string StrdPrcdrName, CommandType commandType, params SqlParameter[] commandParameters)
        {
            DataTable dt = getDataTable(StrdPrcdrName, commandType, commandParameters);
            if (dt.Rows.Count != 0)
                return dt.Rows[0];
            return null;
        }

        #endregion

        #region 鳳□杅擂桶
        /// <summary>
        /// 鳳□珨跺杅擂桶跡,拸賦別殿隙諾桶
        /// </summary>
        /// <param name="sql">Sql脤戙逄歷</param>
        /// <returns></returns>
        public static DataTable getDataTable(string sql)
        {
            DataSet ds = getDataSet(sql);
            if (ds.Tables.Count != 0)
                return ds.Tables[0];
            return new DataTable();
        }

        /// <summary>
        /// 鳳□珨跺杅擂桶跡,拸賦別殿隙諾桶
        /// </summary>
        /// <param name="sql">Sql脤戙逄歷</param>
        /// <param name="commandParameters">統杅杅郪</param>
        /// <returns></returns>
        public static DataTable getDataTable(string sql, params SqlParameter[] commandParameters)
        {
            DataSet ds = getDataSet(sql, commandParameters);
            if (ds.Tables.Count != 0)
                return ds.Tables[0];
            return new DataTable();
        }

        /// <summary>
        /// 鳳□珨跺杅擂桶跡,拸賦別殿隙諾桶
        /// </summary>
        /// <param name="StrdPrcdrName">湔揣徹最靡</param>
        /// <param name="commandType">沓CommandType.StoredProcedure</param>
        /// <returns></returns>
        public static DataTable getDataTable(string StrdPrcdrName, CommandType commandType)
        {
            DataSet ds = getDataSet(StrdPrcdrName, commandType);
            if (ds.Tables.Count != 0)
                return ds.Tables[0];
            return new DataTable();
        }

        /// <summary>
        /// 鳳□珨跺杅擂桶跡,拸賦別殿隙諾桶
        /// </summary>
        /// <param name="StrdPrcdrName">湔揣徹最靡</param>
        /// <param name="commandType">沓CommandType.StoredProcedure</param>
        /// <param name="commandParameters">統杅杅郪</param>
        /// <returns></returns>
        public static DataTable getDataTable(string StrdPrcdrName, CommandType commandType, params SqlParameter[] commandParameters)
        {
            DataSet ds = getDataSet(StrdPrcdrName, commandType, commandParameters);
            if (ds.Tables.Count != 0)
                return ds.Tables[0];
            return new DataTable();
        }

        #endregion

        #region 鳳□杅擂摩
        /// <summary>
        /// 鳳□珨跺杅擂摩,拸賦別殿隙諾腔杅擂摩
        /// </summary>
        /// <param name="sql">Sql脤戙逄歷</param>
        /// <returns></returns>
        public static DataSet getDataSet(string sql)
        {
            return SqlHelper.ExecuteDataset(WriteConnectionStr(), CommandType.Text, sql);
        }
        /// <summary>
        /// 鳳□珨跺杅擂摩,拸賦別殿隙諾腔杅擂摩
        /// </summary>
        /// <param name="sql">Sql脤戙逄歷</param>
        /// <param name="commandParameters">統杅杅郪</param>
        /// <returns></returns>
        public static DataSet getDataSet(string sql, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteDataset(WriteConnectionStr(), CommandType.Text, sql, commandParameters);
        }

        /// <summary>
        /// 鳳□珨跺杅擂摩,拸賦別殿隙諾腔杅擂摩
        /// </summary>
        /// <param name="StrdPrcdrName">湔揣徹最靡</param>
        /// <param name="commandType">沓CommandType.StoredProcedure</param>
        /// <returns></returns>
        public static DataSet getDataSet(string StrdPrcdrName, CommandType commandType)
        {
            return SqlHelper.ExecuteDataset(WriteConnectionStr(), commandType, StrdPrcdrName);
        }

        /// <summary>
        /// 鳳□珨跺杅擂摩,拸賦別殿隙諾腔杅擂摩
        /// </summary>
        /// <param name="StrdPrcdrName">湔揣徹最靡</param>
        /// <param name="commandType">沓CommandType.StoredProcedure</param>
        /// <param name="commandParameters">統杅杅郪</param>
        /// <returns></returns>
        public static DataSet getDataSet(string StrdPrcdrName, CommandType commandType, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteDataset(WriteConnectionStr(), commandType, StrdPrcdrName, commandParameters);
        }

        #endregion

        #region 硒俴岈昢,傖髡殿隙淩,囮啖殿隙梁
        /// <summary>
        /// 婓岈昢笢硒俴祥湍統杅腔sql逄歷
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
                for (int i = 0; i < alSql.Count; i++)
                {
                    myCommand.CommandText = alSql[i].ToString();
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
        /// 婓岈昢笢硒俴湍統杅腔sql逄歷
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
                for (int i = 0; i < alSql.Count; i++)
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

        #region 陔恓杻忷啋匼腔揭燴
        /// <summary>
        /// 鳳□秶隅陔恓杻忷啋匼腔隅砱陓洘摯囀□
        /// </summary>
        /// <param name="NewsID">陔恓ID</param>
        /// <param name="strXMLPath">湔溫唳輸隅砱陓洘腔XML恅璃繚噤</param>
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
        /// 鳳□秶隅陔恓杻忷啋匼腔隅砱陓洘摯囀□
        /// </summary>
        /// <param name="BoardID">蜆陔恓垀扽唳輸</param>
        /// <param name="strXMLPath">湔溫唳輸隅砱陓洘腔XML恅璃繚噤</param>
        /// <param name="strSpecial">蜆陔恓Special趼僇腔囀□</param>
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
                nodeList = doc.SelectNodes("/NewDataSet/Board[BoardID='" + BoardID.ToString() + "']/Special");

                foreach (XmlNode SpecialData in nodeList)
                {
                    DataRow newRow = tabSpecialData.NewRow();
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
        /// 鳳□硌隅梓□囀腔恅掛
        /// </summary>
        /// <param name="Str">恅掛埭</param>
        /// <param name="Tag">梓□</param>
        /// <returns></returns>
        public static string GetStrInTag(string Str, string Tag)
        {
            int intStart = Str.IndexOf("<" + Tag + ">");
            int intEnd = Str.IndexOf("</" + Tag + ">");
            if (intStart == -1 || intEnd == -1)
                return "";
            return Str.Substring(intStart + Tag.Length + 2, intEnd - intStart - Tag.Length - 2);
        }
        #endregion

        #region 涴窒煦滲杅祥猁蚚
        /// <summary>
        /// 鳳□等跺硉,拸賦別寀殿隙null(婃奀祥猁蚚涴跺滲杅)
        /// </summary>
        /// <param name="com">Sql韜鍔勤砓</param>
        /// <returns></returns>
        public static object getDataScalar(SqlCommand com)
        {
            object result = null;
            try
            {
                com.Connection.Open();
                result = com.ExecuteScalar();
            }
            finally
            {
                com.Connection.Close();
            }
            return result;
        }

        /// <summary>
        /// 鳳□珨俴杅擂,拸賦別殿隙諾俴(婃奀祥猁蚚涴跺滲杅)
        /// </summary>
        /// <param name="com">Sql韜鍔勤砓</param>
        /// <returns></returns>
        public static DataRow getDataRow(SqlCommand com)
        {
            DataTable dt = getDataTable(com);
            if (dt.Rows.Count != 0)
                return dt.Rows[0];
            return dt.NewRow();
        }

        /// <summary>
        /// 鳳□珨跺杅擂桶跡,拸賦別殿隙諾桶(婃奀祥猁蚚涴跺滲杅)
        /// </summary>
        /// <param name="sql">Sql韜鍔勤砓</param>
        /// <returns></returns>
        public static DataTable getDataTable(SqlCommand com)
        {
            DataSet ds = getDataSet(com);
            if (ds.Tables.Count != 0)
                return ds.Tables[0];
            return new DataTable();
        }

        /// <summary>
        /// 鳳□珨跺杅擂摩,拸賦別殿隙諾腔杅擂摩(婃奀祥猁蚚涴跺滲杅)
        /// </summary>
        /// <param name="sql">Sql韜鍔勤砓</param>
        /// <returns></returns>
        public static DataSet getDataSet(SqlCommand com)
        {
            SqlDataAdapter ad = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            ad.Fill(ds);
            return ds;
        }

        /// <summary>
        /// 硒俴珨沭Sql脤戙逄歷(婃奀祥猁蚚涴跺滲杅,蚚ExecuteNonQuery)
        /// </summary>
        /// <param name="sql">Sql脤戙逄歷</param>
        public static void ExecuteSQL(string sql)
        {
            SqlConnection con = new SqlConnection(WriteConnectionStr());
            SqlCommand com = new SqlCommand(sql, con);
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
        /// 硒俴Sql韜鍔勤砓(婃奀祥猁蚚涴跺滲杅,蚚ExecuteNonQuery)
        /// </summary>
        /// <param name="com">Sql韜鍔勤砓</param>
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
        /// 鳳□珨跺Sql韜鍔勤砓,蜆韜鍔勤砓濬倰峈湔揣徹最(婃奀祥猁蚚涴跺滲杅)
        /// </summary>
        /// <param name="StrdPrcdrName">湔揣徹最靡</param>
        /// <returns></returns>		
        public static SqlCommand getSqlComOfStrdPrcdr(string StrdPrcdrName)
        {
            SqlConnection con = new SqlConnection(Data.WriteConnectionStr());
            SqlCommand com = new SqlCommand(StrdPrcdrName, con);
            com.CommandType = CommandType.StoredProcedure;
            return com;
        }

        /// <summary>
        /// 硒俴湔揣徹最(婃奀祥猁蚚涴跺滲杅,蚚ExecuteNonQuery)
        /// </summary>
        /// <param name="com">湔揣徹最靡</param>
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


        #region 備份數據庫,成功返回真,失敗返回假
        /// <summary>
        /// 備份數據庫
        /// </summary>
        /// <param name="databasename">要備份的數據源名稱</param>
        /// <param name="backuptodatabase">備份到的數據庫文件名稱及路徑</param>
        /// <returns></returns>
        public static bool BackUpDataBase(string databasename, string backuptodatabase)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = WriteConnectionStr();
            conn.Open();


            string procname;
            string name = databasename + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Date.Day.ToString() + DateTime.Now.Minute.ToString();

            //刪除邏輯備份設備，但不會刪掉備份的數據庫文件
            procname = "sp_dropdevice";
            SqlCommand sqlcmd1 = new SqlCommand(procname, conn);
            sqlcmd1.CommandType = CommandType.StoredProcedure;

            SqlParameter sqlpar = new SqlParameter();
            sqlpar = sqlcmd1.Parameters.Add("@logicalname", SqlDbType.VarChar, 20);
            sqlpar.Direction = ParameterDirection.Input;
            sqlpar.Value = databasename;

            //如果邏輯設備不存在，略去錯誤
            try
            {
                sqlcmd1.ExecuteNonQuery();
            }
            catch
            {
            }

            //邏輯設備
            procname = "sp_addumpdevice";
            SqlCommand sqlcmd2 = new SqlCommand(procname, conn);
            sqlcmd2.CommandType = CommandType.StoredProcedure;

            sqlpar = sqlcmd2.Parameters.Add("@devtype", SqlDbType.VarChar, 20);
            sqlpar.Direction = ParameterDirection.Input;
            sqlpar.Value = "disk";


            sqlpar = sqlcmd2.Parameters.Add("@logicalname", SqlDbType.VarChar, 20);//
            sqlpar.Direction = ParameterDirection.Input;
            sqlpar.Value = databasename;

            sqlpar = sqlcmd2.Parameters.Add("@physicalname", SqlDbType.NVarChar, 260);//
            sqlpar.Direction = ParameterDirection.Input;
            sqlpar.Value = backuptodatabase + name + ".bak";


            try
            {
                int i = sqlcmd2.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                string str = err.Message;
            }

            //
            string sql = "BACKUP DATABASE " + databasename + " TO " + databasename + " WITH INIT";
            SqlCommand sqlcmd3 = new SqlCommand(sql, conn);
            sqlcmd3.CommandType = CommandType.Text;
            try
            {
                sqlcmd3.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                string str = err.Message;
                conn.Close();

                return false;
            }

            conn.Close();//
            return true;

        }
        #endregion
    }

    #endregion

    #region 這裡含有Jeason定義的方法
    /// <summary>
    /// 茼蚚源楊踱
    /// </summary>
    public class Methods
    {
        #region Jeason添加的方法
        /// <summary>
        /// 扢隅dropdownlist諷璃珆尨扢隅砐
        /// </summary>
        /// <param name="DDL"></param>
        /// <param name="ChangeVal"></param>
        private static Hashtable hprocess;
        private static Hashtable hcustomerProcess;
        public static void DropDownListChange(DropDownList DDL, string ChangeVal)
        {
            for (int i = 0; i < DDL.Items.Count; i++)
            {
                DDL.Items[i].Selected = false;
            }

            for (int i = 0; i < DDL.Items.Count; i++)
            {
                if (DDL.Items[i].Value.Equals(ChangeVal.Trim()))
                {
                    DDL.Items[i].Selected = true;
                    break;
                }
            }
        }

        public static void DropDownTextChange(DropDownList DDL, string ChangeTxt)
        {
            for (int i = 0; i < DDL.Items.Count; i++)
            {
                DDL.Items[i].Selected = false;
            }

            for (int i = 0; i < DDL.Items.Count; i++)
            {
                if (DDL.Items[i].Text.Equals(ChangeTxt))
                {
                    DDL.Items[i].Selected = true;
                    break;
                }
            }
        }

        public static void DropDownListInit(DropDownList DDL, int minValue, int MaxValue)
        {
            DDL.Items.Clear();

            for (int i = minValue; i <= MaxValue; i++)
            {
                ListItem li = new ListItem(i.ToString(), i.ToString());
                DDL.Items.Add(li);
            }
        }

        public static void TextBoxDate(DatePicker tb, string txtValue)
        {
            tb.Text = txtValue;
        }

        public static void YearDropDownListInit(DropDownList DDL)
        {
            DDL.Items.Clear();
            int year = DateTime.Now.Year;

            for (int i = year + 2; i >= 2001; i--)
            {
                ListItem li = new ListItem(i.ToString(), i.ToString());
                DDL.Items.Add(li);
            }
        }

        public static void MonthDropDownListInit(DropDownList DDL)
        {
            DDL.Items.Clear();
            for (int i = 1; i <= 12; i++)
            {
                ListItem li = new ListItem(i.ToString(), i.ToString());
                DDL.Items.Add(li);
            }
        }

        public static void MinuteDropDownListInit(DropDownList DDL)
        {
            DDL.Items.Clear();
            for (int i = 0; i < 12; i++)
            {
                int val = i * 5;
                ListItem li = new ListItem(val.ToString(), val.ToString());
                DDL.Items.Add(li);
            }
        }

        public static void EveryMinuteDropDownListInit(DropDownList DDL)
        {
            DDL.Items.Clear();
            for (int i = 0; i < 60; i++)
            {
                int val = i;
                ListItem li = new ListItem(val.ToString(), val.ToString());
                DDL.Items.Add(li);
            }
        }

        public static string FormatMinuteToDayHourMinute(int minutes, int hoursperday)
        {
            int Day = (int)(minutes / (60 * hoursperday));
            int Hour = (int)(minutes % (60 * hoursperday) / 60);
            int Minutes = (int)(minutes % (60 * hoursperday) % 60);
            return string.Format("{0}d{1}:{2}", Day, Hour, Minutes);
        }

        public static string FormatMinuteToDDHHMM(int minutes, int hoursperday)
        {
            int Day = (int)(minutes / (60 * hoursperday));
            int Hour = (int)(minutes % (60 * hoursperday) / 60);
            int Minutes = (int)(minutes % (60 * hoursperday) % 60);
            return string.Format("{0}D{1}H{2}M", Day, Hour, Minutes);
        }

        public static string FormatMinuteToHours(int minutes)
        {
            int Hour = minutes / 60;
            int Mins = minutes % 60;
            return string.Format("{0}" + Translate.translateString("時") + "{1}"+Translate.translateString("分"), Hour, Mins);
        }

        public static string TranslateProcessListName(string processlist)
        {

            string newprocess = "";
            string[] il = processlist.Split('/');
            for (int i = 0; i < il.Length; i++)
            {
                if (il[i] != null && !il[i].Trim().Equals(""))
                {
                    newprocess += Methods.TranslateProcessName(il[i].ToString().Trim()) + "/";
                }
            }
            if (newprocess.Length > 0)
            {
                newprocess = newprocess.Substring(0, newprocess.Length - 1);
            }

            return newprocess;

        }

        public static string GenerateUniqueId()
        {
            return Guid.NewGuid().ToString();
        }
        public static string TranslateProcessName(string processid)
        {
            if (hprocess == null)
            {
                hprocess = new BLL.Process().GetProcessIdProcess();
            }

            if (hcustomerProcess == null)
            {
                hcustomerProcess = new ProcessCustomer().GetCustomerProcessIdProcess();
            }

            if (hprocess.ContainsKey(processid))
            {
                return ((ProcessInfo)hprocess[processid]).ProcessName;
            }
            else
            {
                if (hcustomerProcess.ContainsKey(processid))
                {
                    return ((ProcessCustomerInfo)hcustomerProcess[processid]).CustomerProcessName;
                }
                else
                {
                    return processid;
                }
            }
            //			int locate = -1;
            //			for(int i=0;i<Setting.PROCESSID.Length;i++)
            //			{
            //				if (processid.Equals(Setting.PROCESSID[i]))
            //				{locate = i;}
            //			}
            //			if (locate != -1)
            //			{
            //				return Setting.PROCESSNAME[locate];
            //			}
            //			else
            //			{
            //				return "";
            //			}
        }

        public static string TranslateDesignProcessName(string processid)
        {

            Hashtable hprocess = new DesignProcess().GetProcessIdProcess();

            if (hprocess.ContainsKey(processid))
            {
                return ((DesignProcessInfo)hprocess[processid]).ProcessName;
            }
            else
            {
                return processid;
            }
        }

        public static string TranslateSupplyName(int supplyid)
        {
            Hashtable hh = new Hashtable();
            hh = new ModuleWorkFlow.BLL.System.Supply().GetsupplyinfoByHashtable();
            if (hh.ContainsKey(supplyid))
            {
                return ((ModuleWorkFlow.Model.System.SupplyInfo)hh[supplyid]).SupplierName;
            }
            else
            {
                return "";
            }
        }

        public static string TransDepartmentName(int departmentid)
        {
            Hashtable hh = new Hashtable();
            hh = new ModuleWorkFlow.BLL.DepartMent().Getalldepartmenthashtable();
            if (hh.ContainsKey(departmentid))
            {
                return hh[departmentid].ToString();
            }
            else
            {
                return Convert.ToString(departmentid);
            }

        }

        public static string TransCustomerName(string CustomerID)
        {
            Hashtable hh = new Hashtable();
            hh = new ModuleWorkFlow.BLL.Customer().getCustomerByIDandall();
            if (hh.ContainsKey(CustomerID))
            {
                return hh[CustomerID].ToString();
            }
            else
            {
                return Convert.ToString(CustomerID);
            }

        }


        public static string TranslateMaterialName(string typeid)
        {
            int locate = -1;
            for (int i = 0; i < Setting.MATERIALTYPEID.Length; i++)
            {
                if (typeid.Equals(Setting.MATERIALTYPEID[i]))
                { locate = i; }
            }
            if (locate != -1)
            {
                return Setting.MATERIALTYPENAME[locate];
            }
            else
            {
                return "";
            }
        }

        public static string TranslateStatusName(string statusid)
        {
            int locate = -1;
            for (int i = 0; i < Setting.STATUSID.Length; i++)
            {
                if (statusid.Equals(Setting.STATUSID[i]))
                { locate = i; }
            }
            if (locate != -1)
            {
                return Setting.STATUSNAME[locate];
            }
            else
            {
                return "";
            }
        }

        public static string TranslateStatusColor(string statusid)
        {
            int locate = -1;
            for (int i = 0; i < Setting.STATUSID.Length; i++)
            {
                if (statusid.Equals(Setting.STATUSID[i]))
                { locate = i; }
            }
            if (locate != -1)
            {
                return Setting.STATUSCOLOR[locate];
            }
            else
            {
                return "";
            }
        }

        public static string TranslateUser_Name(string username)
        {
            Hashtable hh = new Hashtable();
            hh = new User().getUserName();
            if (hh.ContainsKey(username))
            {
                return hh[username].ToString();
            }
            else
            {
                return "";
            }
        }

        public static int getBarcodeProcessLocation(string barcode)
        {
            int ret = -1;
            barcode = barcode.ToUpper();
            for (int i = 0; i < Setting.PROCESSBARCODE.Length; i++)
            {
                if (Setting.PROCESSBARCODE[i].Equals(barcode))
                {
                    ret = i;
                }
            }
            return ret;
        }

        public static string getACTIONNAME(string actioncode)
        {
            string ret = null;
            switch (actioncode)
            {
                case "KAISHI":
                    ret = Setting.ACTIONNAME[0];
                    break;
                case "ZANTING":
                    ret = Setting.ACTIONNAME[1];
                    break;
                case "JIESHU":
                    ret = Setting.ACTIONNAME[2];
                    break;
            }
            return ret;
        }

        public static void DDL_TranslateProcessName(DataGrid dg, int index)
        {
            for (int i = 0; i < dg.Items.Count; i++)
            {
                string processid = dg.Items[i].Cells[index].Text;
                dg.Items[i].Cells[index].Text = TranslateProcessName(processid);
            }
        }

        public static void DDL_FormatMinsToHourMinute(DataGrid dg, int index)
        {
            for (int i = 0; i < dg.Items.Count; i++)
            {
                int mins = Convert.ToInt32(dg.Items[i].Cells[index].Text);
                dg.Items[i].Cells[index].Text = FormatMinuteToHours(mins);
            }
        }

        public static void DDL_FormatDateTime(DataGrid dg, int index)
        {
            for (int i = 0; i < dg.Items.Count; i++)
            {
                DateTime dt = Convert.ToDateTime(dg.Items[i].Cells[index].Text);
                if (dt.Ticks == 0)
                {
                    dg.Items[i].Cells[index].Text = "&nbsp;";
                }
            }
        }

        public static void DDL_TranslateStatusName(DataGrid dg, int index)
        {
            ColorConverter cc = new ColorConverter();
            for (int i = 0; i < dg.Items.Count; i++)
            {
                string statusid = dg.Items[i].Cells[index].Text;
                Color c = (Color)cc.ConvertFromString(Methods.TranslateStatusColor(statusid));
                dg.Items[i].Cells[index].BackColor = c;
                dg.Items[i].Cells[index].Text = Methods.TranslateStatusName(statusid);
            }
        }

        #endregion




        #region 都蚚瓚剿滲杅
        /// <summary>
        /// 瓚剿恅梒岆瘁岆鼠羲腔(Article桶)
        /// </summary>
        /// <param name="ArticleID"></param>
        /// <returns></returns>
        public static bool IsPublished_Article(int ArticleID)
        {
            SqlCommand com = Data.getSqlComOfStrdPrcdr("IsPublished_Article");
            com.Parameters.Add("@ArticleID", SqlDbType.BigInt).Value = ArticleID;
            object Published = Data.getDataScalar(com);
            if (Published == null)
                return false;
            return bool.Parse(Published.ToString());
        }

        /// <summary>
        /// 瓚剿恅梒岆瘁岆絞□蚚誧楷桶腔(Article桶)
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="ArticleID"></param>
        /// <returns></returns>
        public static bool IsSelf_Article(string UserID, int NewsID)
        {
            SqlCommand com = Data.getSqlComOfStrdPrcdr("IsSelf_Article");
            com.Parameters.Add("@ArticleID", SqlDbType.BigInt).Value = NewsID;
            com.Parameters.Add("@SubmitterID", SqlDbType.VarChar, 15).Value = UserID;

            int count = 0;
            count = (int)Data.getDataScalar(com);
            return count > 0;
        }

        /// <summary>
        /// 鳳□恅梒垀扽唳輸ID(News桶)
        /// </summary>
        /// <param name="NewsID"></param>
        /// <returns></returns>
        public static int GetBoardID(int NewsID)
        {
            SqlCommand com = Data.getSqlComOfStrdPrcdr("GetBoardID");
            com.Parameters.Add("@NewsID", SqlDbType.BigInt).Value = NewsID;
            object BoardID = Data.getDataScalar(com);
            if (BoardID == null)
                return -1;
            return int.Parse(BoardID.ToString());
        }

        /// <summary>
        /// 鳳□唳輸垀扽□耋(桶)
        /// </summary>
        /// <param name="BoardID"></param>
        /// <returns></returns>
        public static int GetChannelID(int BoardID)
        {
            SqlCommand com = Data.getSqlComOfStrdPrcdr("GetChannelID");
            com.Parameters.Add("@BoardID", SqlDbType.BigInt).Value = BoardID;
            object ChannelID = Data.getDataScalar(com);
            if (ChannelID == null)
                return -1;
            return int.Parse(ChannelID.ToString());
        }

        /// <summary>
        /// 瓚剿恅梒岆瘁湔婓(Article桶)
        /// </summary>
        /// <param name="ArticleID"></param>
        /// <returns></returns>
        public static bool Exists_Article(int ArticleID)
        {
            SqlCommand com = Data.getSqlComOfStrdPrcdr("Exists_Article");
            com.Parameters.Add("@ArticleID", SqlDbType.BigInt).Value = ArticleID;

            int count = 0;
            count = (int)Data.getDataScalar(com);
            return count > 0;
        }

        /// <summary>
        /// 瓚剿陓洘岆瘁湔婓(News桶)
        /// </summary>
        /// <param name="NewsID"></param>
        /// <returns></returns>
        public static bool Exists_News(int NewsID)
        {
            SqlCommand com = Data.getSqlComOfStrdPrcdr("AppInfo_Exists_News");
            com.Parameters.Add("@NewsID", SqlDbType.BigInt).Value = NewsID;

            int count = 0;
            count = (int)Data.getDataScalar(com);
            return count > 0;
        }

        /// <summary>
        /// 瓚剿□蹦岆瘁湔婓(NewsComment桶)
        /// </summary>
        /// <param name="NewsCommentID"></param>
        /// <returns></returns>
        public static bool Exists_NewsComment(int NewsCommentID)
        {
            SqlCommand com = Data.getSqlComOfStrdPrcdr("AppInfo_Exists_NewsComment");
            com.Parameters.Add("@NewsCommentID", SqlDbType.BigInt).Value = NewsCommentID;

            int count = 0;
            count = (int)Data.getDataScalar(com);
            return count > 0;
        }

        /// <summary>
        /// 瓚剿陓洘岆瘁岆眒機瞄腔(News桶)
        /// </summary>
        /// <param name="NewsID"></param>
        /// <returns></returns>
        public static bool IsPublished_News(int NewsID)
        {
            SqlCommand com = Data.getSqlComOfStrdPrcdr("AppInfo_IsPublished_News");
            com.Parameters.Add("@NewsID", SqlDbType.BigInt).Value = NewsID;
            object Published = Data.getDataScalar(com);
            if (Published == null)
                return false;
            else
                return bool.Parse(Published.ToString());
        }

        /// <summary>
        /// 瓚剿陓洘岆瘁岆絞□蚚誧楷桶腔(News桶)
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="NewsID"></param>
        /// <returns></returns>
        public static bool IsSelf_News(string UserID, int NewsID)
        {
            SqlCommand com = Data.getSqlComOfStrdPrcdr("AppInfo_IsSelf_News");
            com.Parameters.Add("@NewsID", SqlDbType.BigInt).Value = NewsID;
            com.Parameters.Add("@SubmitterID", SqlDbType.VarChar, 15).Value = UserID;

            int count = 0;
            count = (int)Data.getDataScalar(com);
            return count > 0;
        }

        #endregion

        #region bbs妏蚚源楊
        /// <summary>
        /// 鳳□珨沭陔恓,□別絞□蚚誧衄□脤艘腔趕(News桶)
        /// </summary>
        /// <param name="NewsID">陔恓ID</param>
        /// <param name="UserID">蚚誧靡</param>
        /// <returns></returns>
        public static string[] getNews(int NewsID, string UserID)
        {
            //News[0]:Title
            //News[1]:Content
            //News[2]:SubmitterID
            //News[3]:SubmitTime
            //News[4]:ViewCount
            string[] News = new string[5];
            string Title = "", Content = "", SubmitterID = "", SubmitTime = "", ViewCount = "";
            if (Methods.Exists_News(NewsID) == false)
            {
                Title = Content = "蜆陓洘祥湔婓!";
                News[0] = Title;
                News[1] = Content;
                News[2] = SubmitterID;
                News[3] = SubmitTime;
                News[4] = ViewCount;
                return News;
            }

            SqlCommand com = Data.getSqlComOfStrdPrcdr("AppInfo_GetNewsByNewsID");
            com.Parameters.Add("@NewsID", SqlDbType.BigInt).Value = NewsID;
            com.Connection.Open();
            SqlDataReader dr = com.ExecuteReader();

            try
            {
                if (dr.Read())
                {
                    if (Methods.IsPublished_News(NewsID))
                    {
                        #region 眒機瞄陓洘
                        Title = dr["Title"].ToString();
                        Content = dr["Content"].ToString();
                        SubmitterID = dr["SubmitterID"].ToString();
                        SubmitTime = dr["SubmitTime"].ToString();
                        ViewCount = dr["ViewCount"].ToString();
                        #endregion
                    }
                    else
                    {
                        #region 帤機瞄陓洘
                        com = Data.getSqlComOfStrdPrcdr("GetRoleID");
                        com.Parameters.Add("@UserID", SqlDbType.VarChar, 15).Value = UserID;
                        com.Parameters.Add("@BoardID", SqlDbType.Int).Value = Settings.StuBoardID;
                        object objRoleID = Data.getDataScalar(com);
                        string UserRoleID;
                        if (objRoleID != null)
                            UserRoleID = objRoleID.ToString();
                        else
                            UserRoleID = "Information_Vistor";
                        if (UserRoleID == "Information_Admin" || UserRoleID == "Information_Manager" || UserRoleID == "Information_Stuff"
                            || Methods.IsSelf_News(UserID, NewsID) || UserID == "jandon")
                        {
                            Title = dr["Title"].ToString();
                            Content = dr["Content"].ToString();
                            SubmitterID = dr["SubmitterID"].ToString();
                            SubmitTime = dr["SubmitTime"].ToString();
                            ViewCount = dr["ViewCount"].ToString();
                        }
                        else
                            //拸□舷艘蜆陓洘
                            Title = Content = "勤祥□ㄛ斕拸□舷艘蜆陓洘!";
                        #endregion
                    }
                }
            }
            catch
            {
                Title = Content = "蜆陓洘祥湔婓!";
            }
            finally
            {
                dr.Close();
                com.Connection.Close();
            }

            News[0] = Title;
            News[1] = Content;
            News[2] = SubmitterID;
            News[3] = SubmitTime;
            News[4] = ViewCount;
            return News;
        }

        /// <summary>
        /// 鳳□珨沭陔恓(News桶)
        /// </summary>
        /// <param name="NewsID">陔恓ID</param>
        /// <returns></returns>
        public static string[] getNews(int NewsID)
        {
            return getNews(NewsID, "jandon");
        }


        /// <summary>
        /// 諍□梓枙酗僅
        /// </summary>
        /// <param name="OriginalString">梓枙</param>
        /// <param name="MaxLength">郔湮酗僅</param>
        /// <returns></returns>
        public static string ShortenString(string OriginalString, int MaxLength)
        {
            //□褒趼睫蹈桶
            string qjChars = "※§‵ㄐ?ㄒㄓㄔ?ㄕㄙㄗㄘ?ㄚ??ㄠㄡㄢㄣㄤㄥㄦㄧㄨㄟ-=\\????ㄩ˙＊▲◎ㄛ﹝ˋㄞ２３４５６７８９ⅠⅡ	←↑↖♂∴▽▼◇＆∠㏑﹉∫汐汕污ㄗ〞";
            qjChars += "??????????□???????????????ㄜ〞";
            string TempString = OriginalString.Replace("&nbsp;", "∮");
            if (MaxLength > TempString.Length)
                return OriginalString;

            StringBuilder sb = new StringBuilder();
            bool flag = true;
            int i = 0;
            while (MaxLength > 0 && i < TempString.Length)
            {
                int n = (int)TempString[i];
                if ((qjChars.IndexOf(TempString[i]) != -1) || (0x4300 <= n && n <= 0x9fa0))
                {
                    sb.Append(TempString[i]);
                    i++;
                    MaxLength--;
                }
                else
                {
                    sb.Append(TempString[i]);
                    i++;
                    if (flag)
                    {
                        MaxLength--;
                    }
                    flag = !flag;
                }
            }
            return sb.ToString().Replace("∮", "&nbsp;");
        }

        /// <summary>
        /// 諍□梓枙酗僅,甜樓吽謹瘍
        /// </summary>
        /// <param name="OriginalString">梓枙</param>
        /// <param name="MaxLength">郔湮酗僅</param>
        /// <returns></returns>
        public static string ShortenString_AddSuspensionPoints(string OriginalString, int MaxLength)
        {
            string strOriginalString = OriginalString;
            string strShortString = ShortenString(OriginalString, MaxLength);
            if (strOriginalString.Length > strShortString.Length)
            {
                return strShortString + "...";
            }
            return strShortString;
        }


        /*殿隙郔輪萸腔貉*/
        public static System.Data.DataTable getRecentOrderSong()
        {
            DataTable dt = Data.getDataTable("select top 1 SongList.Title,SongList.SongID,SongPlayList.SubmitterID,SongPlayList.ToWhom,SongPlayList.HumanType,SongPlayList.Text from SongList,SongPlayList where SongList.SongID=SongPlayList.SongID and SongPlayList.Time>GetDate() order by SongPlayList.Time");
            if (dt.Rows.Count == 0)
                dt = Data.getDataTable("select top 1 SongList.Title,SongList.SongID,SongPlayList.SubmitterID,SongPlayList.ToWhom,SongPlayList.HumanType,SongPlayList.Text from SongList,SongPlayList where SongList.SongID=SongPlayList.SongID order by SongPlayList.Time");
            return dt;
        }

        #endregion

        #region 鳳□郔陔陔恓蹈桶
        /// <summary>
        /// 鳳□硌隅唳輸郔陔陔恓蹈桶(News桶)
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <param name="BoardID">唳輸ID</param>
        /// <returns></returns>
        public static DataTable GetNewsList_BoardID(int Count, int MaxLength, int BoardID)
        {
            DataTable NewsList = Data.getDataTable(string.Format("SELECT TOP {0} NewsID, Title, SubmitterID, SubmitTime FROM [News] WHERE NewsBoardID = {1} AND Published=1 ORDER BY SubmitTime DESC", Count, BoardID));
            foreach (DataRow row in NewsList.Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        /// <summary>
        /// 鳳□硌隅□耋郔陔陔恓蹈桶(News桶)
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <param name="ChannelID">□耋ID</param>
        /// <returns></returns>
        public static DataTable GetNewsList_ChannelID(int Count, int MaxLength, int ChannelID)
        {
            DataTable NewsList = Data.getDataTable(string.Format("SELECT TOP {0} [News].NewsBoardID,[NewsStructure].Title AS BoardTitle,[News].NewsID, [News].Title, [News].SubmitterID, [News].SubmitTime FROM [News],[NewsStructure] WHERE [NewsStructure].ChannelID = {1} AND [NewsStructure].NewsBoardID = [News].NewsBoardID AND [News].Published=1 ORDER BY SubmitTime DESC", Count, ChannelID));
            foreach (DataRow row in NewsList.Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        /// <summary>
        /// 鳳□郔陔陔恓蹈桶
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <returns></returns>
        public static DataTable GetNewsList(int Count, int MaxLength)
        {
            DataTable NewsList = Data.getDataTable(string.Format("SELECT TOP {0} NewsID, Title, SubmitterID, SubmitTime FROM [News] WHERE Published=1 ORDER BY SubmitTime DESC", Count));
            foreach (DataRow row in NewsList.Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        #endregion

        #region 鳳□猁恓絳黍蹈桶
        /// <summary>
        /// 鳳□硌隅唳輸猁恓絳黍蹈桶
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <param name="BoardID">唳輸ID</param>
        /// <returns></returns>
        public static DataTable GetNewsList_Important_BoardID(int Count, int MaxLength, int BoardID)
        {
            DataTable NewsList = Data.getDataTable(string.Format("SELECT TOP {0} NewsID, Title, SubmitterID, SubmitTime FROM [News] WHERE Important = 1 AND Published=1 AND NewsBoardID={1} ORDER BY SubmitTime DESC", Count, BoardID));
            foreach (DataRow row in NewsList.Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        /// <summary>
        /// 鳳□硌隅□耋猁恓絳黍蹈桶
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <param name="ChannelID">□耋ID</param>
        /// <returns></returns>
        public static DataTable GetNewsList_Important_ChannelID(int Count, int MaxLength, int ChannelID)
        {
            DataTable NewsList = Data.getDataTable(string.Format("SELECT TOP {0} [News].NewsBoardID,[NewsStructure].Title AS BoardTitle,[News].NewsID, [News].Title, [News].SubmitterID, [News].SubmitTime FROM [News],[NewsStructure] WHERE [News].Important = 1 AND [NewsStructure].ChannelID = {1} AND [NewsStructure].NewsBoardID = [News].NewsBoardID AND [News].Published=1 ORDER BY SubmitTime DESC", Count, ChannelID));
            foreach (DataRow row in NewsList.Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        /// <summary>
        /// 鳳□垀衄猁恓絳黍蹈桶
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <returns></returns>
        public static DataTable GetNewsList_Important(int Count, int MaxLength)
        {
            DataTable NewsList = Data.getDataTable(string.Format("SELECT TOP {0} NewsStructure.NewsBoardID, NewsStructure.Title as BoardTitle, News.NewsID, News.Title, News.SubmitterID, News.SubmitTime FROM [News],[NewsStructure] WHERE Important = 1 AND Published=1 and News.NewsBoardID=NewsStructure.NewsBoardID ORDER BY SubmitTime DESC", Count));
            foreach (DataRow row in NewsList.Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        #endregion

        #region 鳳□芢熱陔恓蹈桶
        /// <summary>
        /// 鳳□硌隅唳輸芢熱陔恓蹈桶
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <param name="BoardID">唳輸ID</param>
        /// <returns></returns>
        public static DataTable GetNewsList_Recommended_BoardID(int Count, int MaxLength, int BoardID)
        {
            DataTable NewsList = Data.getDataTable(string.Format("SELECT TOP {0} NewsID, Title, SubmitterID, SubmitTime FROM [News] WHERE Recommended = 1 AND Published=1 AND NewsBoardID={1} ORDER BY SubmitTime DESC", Count, BoardID));
            foreach (DataRow row in NewsList.Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        /// <summary>
        /// 鳳□硌隅□耋芢熱陔恓蹈桶
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <param name="ChannelID">□耋ID</param>
        /// <returns></returns>
        public static DataTable GetNewsList_Recommended_ChannelID(int Count, int MaxLength, int ChannelID)
        {
            DataTable NewsList = Data.getDataTable(string.Format("SELECT TOP {0} [News].NewsBoardID,[NewsStructure].Title AS BoardTitle,[News].NewsID, [News].Title, [News].SubmitterID, [News].SubmitTime FROM [News],[NewsStructure] WHERE [NewsStructure].ChannelID = {1} AND [NewsStructure].NewsBoardID = [News].NewsBoardID AND [News].Published=1 AND [News].Recommended = 1 ORDER BY SubmitTime DESC", Count, ChannelID));
            foreach (DataRow row in NewsList.Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        /// <summary>
        /// 鳳□郔陔芢熱陔恓蹈桶
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <returns></returns>
        public static DataTable GetNewsList_Recommended(int Count, int MaxLength)
        {
            DataTable NewsList = Data.getDataTable(string.Format("SELECT TOP {0} NewsID, Title, SubmitterID, SubmitTime FROM [News] WHERE Published=1 AND Recommended = 1 ORDER BY SubmitTime DESC", Count));
            foreach (DataRow row in NewsList.Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        #endregion

        #region 鳳□芢熱准猁恓絳黍蹈桶
        /// <summary>
        /// 鳳□硌隅唳輸芢熱准猁恓絳黍蹈桶
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <param name="BoardID">唳輸ID</param>
        /// <returns></returns>
        public static DataTable GetNewsList_Recommended_NotImportant_BoardID(int Count, int MaxLength, int BoardID)
        {
            DataTable NewsList = Data.getDataTable(string.Format("SELECT TOP {0} NewsID, Title, SubmitterID, SubmitTime FROM [News] WHERE Recommended = 1 AND Published=1 AND NewsBoardID={1} AND Important=0 ORDER BY SubmitTime DESC", Count, BoardID));
            foreach (DataRow row in NewsList.Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        /// <summary>
        /// 鳳□硌隅□耋芢熱准猁恓絳黍蹈桶
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <param name="ChannelID">□耋ID</param>
        /// <returns></returns>
        public static DataTable GetNewsList_Recommended_NotImportant_ChannelID(int Count, int MaxLength, int ChannelID)
        {
            DataTable NewsList = Data.getDataTable(string.Format("SELECT TOP {0} [News].NewsBoardID,[NewsStructure].Title AS BoardTitle,[News].NewsID, [News].Title, [News].SubmitterID, [News].SubmitTime FROM [News],[NewsStructure] WHERE [NewsStructure].ChannelID = {1} AND [NewsStructure].NewsBoardID = [News].NewsBoardID AND [News].Published=1 AND [News].Important=0 AND [News].Recommended = 1 ORDER BY SubmitTime DESC", Count, ChannelID));
            foreach (DataRow row in NewsList.Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        /// <summary>
        /// 鳳□郔陔芢熱准猁恓絳黍蹈桶
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <returns></returns>
        public static DataTable GetNewsList_Recommended_NotImportant(int Count, int MaxLength)
        {
            DataTable NewsList = Data.getDataTable(string.Format("SELECT TOP {0} NewsID, Title, SubmitterID, SubmitTime FROM [News] WHERE Published=1 AND Recommended = 1 AND Important=0 ORDER BY SubmitTime DESC", Count));
            foreach (DataRow row in NewsList.Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        #endregion

        #region 鳳□准芢熱准猁恓絳黍蹈桶
        /// <summary>
        /// 鳳□硌隅唳輸准芢熱准猁恓絳黍蹈桶
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <param name="BoardID">唳輸ID</param>
        /// <returns></returns>
        public static DataTable GetNewsList_NotRecommended_NotImportant_BoardID(int Count, int MaxLength, int BoardID)
        {
            DataTable NewsList = Data.getDataTable(string.Format("SELECT TOP {0} NewsID, Title, SubmitterID, SubmitTime FROM [News] WHERE Recommended = 0 AND Published=1 AND NewsBoardID={1} AND Important=0 ORDER BY SubmitTime DESC", Count, BoardID));
            foreach (DataRow row in NewsList.Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        /// <summary>
        /// 鳳□硌隅□耋准芢熱准猁恓絳黍蹈桶
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <param name="ChannelID">□耋ID</param>
        /// <returns></returns>
        public static DataTable GetNewsList_NotRecommended_NotImportant_ChannelID(int Count, int MaxLength, int ChannelID)
        {
            DataTable NewsList = Data.getDataTable(string.Format("SELECT TOP {0} [News].NewsBoardID,[NewsStructure].Title AS BoardTitle,[News].NewsID, [News].Title, [News].SubmitterID, [News].SubmitTime FROM [News],[NewsStructure] WHERE [NewsStructure].ChannelID = {1} AND [NewsStructure].NewsBoardID = [News].NewsBoardID AND [News].Published=1 AND [News].Important=0 AND [News].Recommended = 0 ORDER BY SubmitTime DESC", Count, ChannelID));
            foreach (DataRow row in NewsList.Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        /// <summary>
        /// 鳳□郔陔准芢熱准猁恓絳黍蹈桶
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <returns></returns>
        public static DataTable GetNewsList_NotRecommended(int Count, int MaxLength)
        {
            DataTable NewsList = Data.getDataTable(string.Format("SELECT TOP {0} NewsID, Title, SubmitterID, SubmitTime FROM [News] WHERE Published=1 AND Recommended = 0 AND Important=0 ORDER BY SubmitTime DESC", Count));
            foreach (DataRow row in NewsList.Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        #endregion

        #region 鳳□□藷陔恓蹈桶
        /// <summary>
        /// 鳳□硌隅唳輸□藷陔恓蹈桶(News桶)
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <param name="BoardID">唳輸ID</param>
        /// <returns></returns>
        public static DataTable GetNewsList_Hot_BoardID(int Count, int MaxLength, int BoardID)
        {
            DataTable NewsList = Data.getDataTable(string.Format("SELECT TOP {0} NewsID, Title, SubmitterID, SubmitTime FROM [News] WHERE ViewCount>{1} AND Published=1 AND NewsBoardID={2} ORDER BY SubmitTime DESC", Count, Settings.HotLimit, BoardID));
            foreach (DataRow row in NewsList.Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        /// <summary>
        /// 鳳□硌隅□耋□藷陔恓蹈桶(News桶)
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <param name="ChannelID">□耋ID</param>
        /// <returns></returns>
        public static DataTable GetNewsList_Hot_ChannelID(int Count, int MaxLength, int ChannelID)
        {
            DataTable NewsList = Data.getDataTable(string.Format("SELECT TOP {0} [News].NewsBoardID,[NewsStructure].Title AS BoardTitle,[News].NewsID, [News].Title, [News].SubmitterID, [News].SubmitTime FROM [News],[NewsStructure] WHERE [News].ViewCount>{1} AND [NewsStructure].ChannelID = {2} AND [NewsStructure].NewsBoardID = [News].NewsBoardID AND [News].Published=1 ORDER BY SubmitTime DESC", Count, Settings.HotLimit, ChannelID));
            foreach (DataRow row in NewsList.Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        /// <summary>
        /// 鳳□□藷陔恓蹈桶
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <returns></returns>
        public static DataTable GetNewsList_Hot(int Count, int MaxLength)
        {
            DataTable NewsList = Data.getDataTable(string.Format("SELECT TOP {0} NewsID, Title, SubmitterID, SubmitTime FROM [News] WHERE ViewCount>{1} AND Published=1 ORDER BY SubmitTime DESC", Count, Settings.HotLimit));
            foreach (DataRow row in NewsList.Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        #endregion

        #region 涴窒煦滲杅祥猁蚚
        /// <summary>
        /// 鳳□硌隅唳輸郔陔陔恓蹈桶(News桶)(婃奀祥猁蚚涴跺滲杅)
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <param name="BoardID">唳輸ID</param>
        /// <returns></returns>
        public static DataSet GetNewsListByBoardID(int Count, int MaxLength, int BoardID)
        {
            DataSet NewsList = Data.getDataSet(string.Format("SELECT TOP {0} NewsID, Title, SubmitterID, SubmitTime FROM [News] WHERE NewsBoardID = {1} AND Published=1 ORDER BY SubmitTime DESC", Count, BoardID));
            foreach (DataRow row in NewsList.Tables[0].Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        /// <summary>
        /// 鳳□硌隅□耋郔陔陔恓蹈桶(News桶)(婃奀祥猁蚚涴跺滲杅)
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <param name="ChannelID">□耋ID</param>
        /// <returns></returns>
        public static DataSet GetNewsListByChannelID(int Count, int MaxLength, int ChannelID)
        {
            DataSet NewsList = Data.getDataSet(string.Format("SELECT TOP {0} [News].NewsID, [News].Title, [News].SubmitterID, [News].SubmitTime FROM [News],[NewsStructure] WHERE [NewsStructure].ChannelID = {1} AND [NewsStructure].NewsBoardID = [News].NewsBoardID AND [News].Published=1 ORDER BY SubmitTime DESC", Count, ChannelID));
            foreach (DataRow row in NewsList.Tables[0].Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        /// <summary>
        /// 鳳□硌隅唳輸猁恀絳黍蹈桶(婃奀祥猁蚚涴跺滲杅)
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <param name="BoardID">唳輸ID</param>
        /// <returns></returns>
        public static DataSet GetImportantNewsList(int Count, int MaxLength, int BoardID)
        {
            DataSet NewsList = Data.getDataSet(string.Format("SELECT TOP {0} NewsID, Title, SubmitterID, SubmitTime FROM [News] WHERE Important = 1 AND Published=1 AND NewsBoardID={1} ORDER BY SubmitTime DESC", Count, BoardID));
            foreach (DataRow row in NewsList.Tables[0].Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }
        /// <summary>
        /// 鳳□垀衄猁恓絳黍蹈桶(婃奀祥猁蚚涴跺滲杅)
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <returns></returns>
        public static DataSet GetImportantNewsList(int Count, int MaxLength)
        {
            DataSet NewsList = Data.getDataSet(string.Format("SELECT TOP {0} NewsID, Title, SubmitterID, SubmitTime FROM [News] WHERE Important = 1 AND Published=1 ORDER BY SubmitTime DESC", Count));
            foreach (DataRow row in NewsList.Tables[0].Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }
        /// <summary>
        /// 鳳□硌隅唳輸芢熱陔恓蹈桶(婃奀祥猁蚚涴跺滲杅)
        /// </summary>
        /// <param name="Count">砑鳳□腔沭杅</param>
        /// <param name="MaxLength">梓枙郔湮酗僅</param>
        /// <param name="BoardID">唳輸ID</param>
        /// <returns></returns>
        public static DataSet GetRecommendedNewsList(int Count, int MaxLength, int BoardID)
        {
            DataSet NewsList = Data.getDataSet(string.Format("SELECT TOP {0} NewsID, Title, SubmitterID, SubmitTime FROM [News] WHERE Recommended = 1 AND Published=1 AND NewsBoardID={1} ORDER BY SubmitTime DESC", Count, BoardID));
            foreach (DataRow row in NewsList.Tables[0].Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        //鳳□芢熱陔恓蹈桶		
        public static DataSet GetRecommendedNewsList(int Count, int MaxLength)
        {
            DataSet NewsList = Data.getDataSet(string.Format("SELECT TOP {0} NewsID, Title, SubmitterID, SubmitTime FROM [News] WHERE Published=1 AND Recommended = 1 ORDER BY SubmitTime DESC", Count));
            foreach (DataRow row in NewsList.Tables[0].Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }

        //硌隅銜輸□藷陔恓蹈桶
        public static DataSet GetHotNewList(int Count, int MaxLength, int BoardID)
        {
            DataSet NewsList = Data.getDataSet(string.Format("SELECT TOP {0} NewsID, Title, SubmitterID, SubmitTime FROM [News] WHERE ViewCount>5 AND Published=1 AND NewsBoardID={1} ORDER BY SubmitTime DESC", Count, BoardID));
            foreach (DataRow row in NewsList.Tables[0].Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }
        //垀衄銜輸□藷陔恓蹈桶
        public static DataSet GetHotNewList(int Count, int MaxLength)
        {
            DataSet NewsList = Data.getDataSet(string.Format("SELECT TOP {0} NewsID, Title, SubmitterID, SubmitTime FROM [News] WHERE ViewCount>5 AND Published=1 ORDER BY SubmitTime DESC", Count));
            foreach (DataRow row in NewsList.Tables[0].Rows)
            {
                row["Title"] = ShortenString(row["Title"].ToString(), MaxLength);
            }
            return NewsList;
        }
        #endregion
    }
    #endregion
}