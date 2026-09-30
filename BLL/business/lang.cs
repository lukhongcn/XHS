using System;

namespace ModuleWorkFlow.business
{
	/// <summary>
	/// Summary description for lang.
	/// </summary>
	public class lang
	{
		public static string TXT_MODULEID = "模具編號";
		public static string TXT_INPUTTRYTIMEERROR = "設定日期的工序沒有設定相應的時間!";
		public static string TXT_DROPDOWNLISTTWO="零件大類的第二個類必須填寫";
		public static string TXT_INPUTTRYTIMES = "請輸入試模次數!";
		public static string TXT_INPUTTRYTIME = "請輸入試模時間!";
		public static string TXT_MODIFYIDHASEXISTED = "修模編號已經存在！";
		public static string TXT_INPUTTRYDATE = "請輸入試模日期!";
		public static string TXT_MODIFYIDFORMATERR = "您輸入的修模編號不正確，修模編號規則為：模具編號+'-2**'";
		public static string TXT_SELECTMODIFYID = "請選擇有效的修模編號";
		public static string TXT_SELECTPROJECT = "請選擇工程項目！";
		public static string TXT_SETTINGSUCCESS = "鏡面機台設定成功！";
		public static string TXT_DONETOSCHEDULE = "重新排程已經完成！";
		public static string TXT_SELECTCUSTOMIDMODULEIDPARTNO = "請選擇有效的客戶名稱，模具編號，零件編號";
		public static string TXT_DATELOGICALERROR = "您輸入的日期有邏輯錯誤，請檢查!";
		public static string TXT_HINTERROR = "無效";
		public static string TXT_SELECTMODULEID = "請選擇有效的模具編號";
		public static string TXT_ContainSpecCode = "零件編號含有非法特殊字符，如'+'等";
		public static string TXT_CAN_NOT_DELETED_PRODUCTING_PROCESS = "您不能設定零件已經生產的工序的所需時間為0";
		public static string TXT_TIMERANGEERROR = "時間範圍設定錯誤，請重新設定！";
		public static string TXT_CREATEEXCELFAILED = "創建文件失敗，請檢查輸入參數的合法性！";
		public static string TXT_CREATEEXCELSUCCESS = "創建文件成功！";
		public static string TXT_CONTACTERNOTNULL="姓名必須填寫";
		public static string TXT_CONTACTERNOTPHONE="電話或者手機號碼必須填寫一個";
		public static string TXT_PRICEINPUTERROR = "價格輸入錯誤";
		public static string TXT_SHOWSTATUSADD = " (添加)";
		public static string TXT_SHOWSTATUSEDIT = " (編輯)";
		public static string TXT_TYPECONVERTERROR = "類型轉換錯誤，請檢查您輸入數據的是否為正確的類型！";
		public static string TXT_AUTOCREATE = "(自動產生)";
		public static string TXT_KEYWORDS = "關鍵字";
		public static string TXT_SELECT = "選擇";
		public static string TXT_CANCEL = "取消";
		public static string TXT_ADD = "保存";
		public static string TXT_EDIT = "保存";
		public static string TXT_YES = "是";
		public static string TXT_NO  = "否";
		public static string TXT_EXISTRELATIONDATA = "該模具由於有關聯零件信息數據存在,您不能執行此操作,請先刪除零件信息!";
		public static string TXT_NOPRIVILEGE = "您無權執行此項操作!";
		public static string TXT_LOGINFAILED = "員工編號或口令錯誤!";
		public static string TXT_NOREGISTED = "軟件沒有註冊，請先註冊!";
		public static string TXT_OPERATIONERROR = "這是一個非法錯誤,請聯繫開發人員!";
		public static string TXT_ADDSUCCESS = "添加成功!";
		public static string TXT_EDITSUCCESS = "編輯成功!";
		public static string TXT_DELETESUCCESS = "刪除成功!";
		public static string TXT_DELETECONFIRM = "您確定要刪除這條紀錄嗎?";
		public static string TXT_SCHEDULESUCCESS = "創建排程成功!";
		public static string TXT_SCHEDULEFAILURE = "創建排程失敗!";
		public static string TXT_STARTDATEERROR = "請輸入正確的開始日期!";
		public static string TXT_DATEERROR = "請輸入正確的日期!";
		public static string TXT_STARTENDDATEERROR = "結束日期必須大於開始日期!";
		public static string TXT_PARTNOIDERROR = "輸入的零件編號有誤！";
		public static string TXT_ELECTRODENOERROR = "請輸入合法的電極編號,電極編號為'模仁編號'+'M'+'電極編號'。如：101M01";
		public static string TXT_NOTADDCOMPLATE="請完整填寫信息";
		public static string TXT_UNIQUE="請確認ShiftAbbr的唯一性";
		public static string TXT_SELECTNAME = "  ";
		public static string TXT_SELECTVALUE = "select";
		public static string TXT_SELECTALLNAME = "全部";
		public static string TXT_SELECTALLVALUE = "all";

		public static string MODULEID_PARTNO_NOT_EXIST = "模具或零件編號或工序不存在，請錄入模具，零件信息";

		public static string MACHINE_EMPTY = "機體空閒，不可在此機體選擇結束或暫停";

		public static string TXT_STARTDATE_NOSPACE = "請輸入計劃開始時間";
		public static string TXT_ENDDATE_NOSPACE = "請輸入計劃完成時間";
		public static string TXT_TIME = "開始時間和完成時間設定不統一";
		public static string SPEC_FORMAT_ERROR="規格格式為 A*B*C ,ABC都只能為數字";
		

		public static string TXT_MACHINESTATUSADD_SelectModuleid = "請選擇模具編號!";
		public static string TXT_MACHINESTATUSADD_SelectPartnoid = "請選擇零件編號!";
		public static string TXT_MACHINESTATUSADD_SelectProcessorder = "請選擇工序號!";
		public static string TXT_MACHINESTATUSADD_SelectDate = "請選擇日期！";
		public static string TXT_MACHINESTATUSADD_SettingTime = "請設定加工時間！";
		public static string TXT_MACHINESTATUSADD_NotOverOneDay = "修改時，不允許設定加工工時大於機台有效一天加工工時！";
		public static string TXT_MACHINESTATUSADD_OverMachineTime = "您設定的加工時間和機台現有的加工時間纍計超過24小時，請重新設定！";

		public static string TXT__PARTMIDDLESCHEDULEPART_Level0 = "模具當前無中排程!";
		public static string TXT__PARTMIDDLESCHEDULEPART_Level1 = "模具當前為模具級中排程!";
		public static string TXT__PARTMIDDLESCHEDULEPART_Level2 = "模具當前為零件級中排程!";
		public static string TXT__PARTMIDDLESCHEDULEPART_Level3 = "模具當前為零件個數級中排程!";

		public static string TXT__PARTMODULEMIDDLESCHEDULE_ModuleNull = "請選擇模具編號!";
		public static string TXT__PARTMODULEMIDDLESCHEDULE_CreateBigScheduleFailed = "自動創建模具中日程失敗!";
		public static string TXT__PARTMODULEMIDDLESCHEDULE_SetBigScheduleSucc = "設定中日程成功!";
		public static string TXT__PARTMODULEMIDDLESCHEDULE_SetBigScheduleFailed = "設定中日程失敗!請聯繫開發人員！";
		public static string TXT__PARTMODULEMIDDLESCHEDULE_SelectProcess = "請選擇工序！";
		public static string TXT__PARTMODULEMIDDLESCHEDULE_SetStartEndOrNot = "請選擇開始日期和結束日期或者都不設定！";
		public static string TXT__PARTMODULEMIDDLESCHEDULE_DateTimeFormatError = "開始日期和結束日期格式設定有誤！";
		public static string TXT__PARTMODULEMIDDLESCHEDULE_DateTimeInputError = "開始日期大於結束日期，設定有誤！";

		public static string TXT_MODULEMIDDLESCHEDULE_SelectProcess = "請先選擇工序！";
		public static string TXT_MODULEMIDDLESCHEDULE_SelectStartDate = "請先設定此工序中排程開始日期！";
		public static string TXT_MODULEMIDDLESCHEDULE_SelectEndDate = "請先設定此工序中排程結束日期！";
		public static string TXT_MODULEMIDDLESCHEDULE_StartDateSmallerThanEndDate = "開始日期大於結束日期，請重新設定！";

		public static string TXT_SCANBARCODE_BarcodeEmpty = "請掃瞄或輸入條形碼!";
		public static string TXT_SCANBARCODE_BarcodeError = "條形碼不合法,請從新掃瞄或輸入!";
		public static string TXT_SCANBARCODE_SelectProcess = "此零件無這道工序，請重新選擇工序或條形碼!";

		public static string TXT_PROJECTADD_ModifyidError = "您輸入的修模編號有誤，請檢查！";
		public static string TXT_PROJECTADD_DisProjectIdError = "您輸入的指示書編號有誤，請檢查！";
		public static string TXT_PROJECTADD_DisProjectIdExisted = "您輸入的指示書編號已經存在，請檢查！";

		public static string TXT_PROJECTLIST_DeleteInfo = "刪除操作將要刪除此指示書對應的訂單及排程信息，您確定要執行這個操作嗎?";

		public static string TXT_PROCESSNAME = "工序名稱";
		public static string TXT_PROCESSORDER = "工序序號";
		public static string TXT_STARTTIME = "開始日期";
		public static string TXT_ENDTIME = "結束日期";
		public static string TXT_PROCESSTIME = "工時（小時）";
		public static string TXT_NEEDMINUTESZERO = "您有工序設定的工時為零，請重新設定！";
        public static string TXT_QRCODENEED = "必須設定二維碼";
		public static string TXT_PARTADDEDIT_NoSelected = "請選擇工序！";
		public static string NO_User = "不存在此用戶";

		public static string TXT_SCANBARCODE2_BarcodeContent = "輸入的條碼內容為 ";
        public static string TXT_SCANBARCODE2_INPUTACTIONFIRST = "輸入的條碼內容為 ";
		public static string TXT_SCANBARCODE2_BarcodeContentError = "輸入的條碼內容有誤! ";
		public static string TXT_SCANBARCODE2_BarcodeContentError2 = "請先設定工序和零件編碼! ";
        public static string TXT_SCANBARCODE2_PARTALREADYINPUT = "結束或暫停零件以機台加工爲準，無需刷人零件";
		public static string TXT_SCANBARCODE2_BarcodeContentError3 = "該零件此工序都已完成! ";
		public static string TXT_SCANBARCODE2_BarcodeContentError4 = "您的操作動作不正確，零件的當前狀態為 ";
		public static string TXT_SCANBARCODE2_BarcodeContentError5 = "此零件沒有這道工序，或者這個零件不存在！";
		public static string TXT_SCANBARCODE2_BarcodeContentError6 = "請先設定工序! ";
		public static string TXT_SCANBARCODE2_BarcodeContentError7 = "您的操作動作不正確，零件已經開始加工，零件的當前狀態為 ";
		public static string TXT_SCANBARCODE2_BarcodeContentError8 = "該零件已經有開始的工藝,該工藝信息如下：";
		public static string TXT_SCANBARCODE2_BarcodeContentError9 = "請輸入零件編號。如：1 或1,2 或1-3";
		public static string TXT_SCANBARCODE2_BarcodeContentError10 = "請先輸入用戶編號、零件編號、動作信息！";
        public static string TXT_SCANBARCODE2_BarcodeContentError101 = "請先輸入動作信息！";
		public static string TXT_SCANBARCODE2_BarcodeContentError11 = "工序不能為組立！";
		public static string TXT_SCANBARCODE2_BarcodeContentError12 = "由於輸入的是模號，所以工序請選擇組立！";
        public static string TXT_SCANBARCODE2_ReadyStart = "零件已經開始，無法繼續開始！";
        public static string TXT_SCANBARCODE2_ReadyHoldOnOrEnd = "零件不是開始狀態，無法繼續暫停或結束！";
        public static string TXT_ModifyPart_ReadyChange = "零件工序狀態可能在其他電腦已經更改，無法繼續變更！";
		//MYD070615
		public static string TXT_SCANBARCODE2_BarcodeContentError13 = "工件並沒有完成，因此無法進行返工操作！";
		public static string TXT_SCANBARCODE2_BarcodeContentError14 = "員工不存在";
		public static string TXT_SCANBARCODE2_BarcodeContentError30 ="請先輸入模具編號、用戶編號、動作信息！";
		public static string TXT_SCANBARCODE2_BarcodeContentError31= "此模具不存在，或者這個模具沒有結案！";

		public static string TXT_ORDERLIST_OrderCount = "當前模具數量為";

		public static string TXT_OUTSOURCINGLIST_PartCount = "當前零件數量為";

		public static string TXT_USERLIST_InputSearch = "請輸入搜索的內容！";
		public static string TXT_USERLIST_SaveSucc = "保存信息成功！";
		public static string TXT_USERLIST_Savefailed = "保存信息失敗，請聯繫相關人員！";
		public static string TXT_USERLIST_InputUserName = "請輸入員工編號！";
		public static string TXT_USERLIST_InputName = "請輸入員工姓名！";
		public static string TXT_USERLIST_KeyDuplicated = "該員工編號已經存在，請重新輸入！";
		public static string TXT_USERLIST_ErrorURL = "非法連接！";
		public static string TXT_DELETEUSERBYORDER="定單表有此用戶不允許刪除";
		public static string TXT_MODULEMIDDLESCHEDULEVIEW_ModuleCountZero = "當前查詢到的模具數量為0！";

		public static string TXT_PARTUNNORMALADD_SelectPartNo = "請選擇零件!";
		public static string TXT_PARTUNNORMALADD_DiscardSucc = "該零件曾經報廢[{0}]次，第[{1}]次報廢設定成功！零件報廢新編號為[{2}]!";
		public static string TXT_PARTUNNORMALADD_DiscardFailed = "保存信息失敗，請聯繫相關人員！";
		public static string TXT_PARTUNNORMALADD_Newpartno = "該零件所有工序都沒有開始加工，為新零件，不能報廢！";
		public static string TXT_PARTUNNORMALADD_ConfirmDiscard = "您確定要報廢這個零件嗎？";

		public static string TXT_OUTSOURCINGADD_SelectModule = "請選擇模具編號！";
		public static string TXT_OUTSOURCINGADD_SelectPartNo = "請選擇零件編號！";
		public static string TXT_OUTSOURCINGADD_SelectPartNoId = "請選擇零件ID編號！";

		public static string MetalView_Delete = "請先刪除此類零件";
		public static string BomType_Delete = "請先刪除與此相關的零件";
		public static string TXT_NOPRIVILIGE = "您無權限進行此操作";

		public static string OUTPUTCOUNT_BIGER_STOCK = "出庫數量大於庫存數量";
		public static string INPUTCOUNT_BIGER_PARTCOUNT ="入庫數量大於零件數量";
		

		/* popupdiscardinfo.aspx */
		public static string TXT_POPUPDISCARDINFO_ErrorInput = "錯誤的模具編號！";

		public static string TXT_PARTMODIFYPROCESSORDER_saveError = "保存信息發生錯誤！";

		//訂單設定修改頁面
		public static string TXT_ORDERADDEDIT_SetAssembleTime = "請設定組立的加工工時！";

		public static string TXT_SCHEDULEREPORT_InputStartDateEndDate = "請選擇開始日期和結束日期！";

		//電子郵件列表
		public static string TXT_EMAILGROUP_TXT0 = "模具完成進度警示信息";
		public static string TXT_EMAILGROUP_VALUE0 = "0";
		public static string TXT_EMAILGROUP_TXT1 = "零件正在超時加工警示信息";
		public static string TXT_EMAILGROUP_VALUE1 = "1";

		public static string TXT_EMAILGROUPEDIT_SelectUser = "請選擇用戶！";

		//上傳文件
		public static string TXT_UPLOADIMAGE_SELECTFile = "請選擇文件！";

		//電機修改頁面
		public static string TXT_ELECTRODEADDEDIT_NoInput = "請輸入最少需要數量/個！";

		//條碼輸入頁面處理修模部分
		public static string TXT_PROJECTADD_INPUTModifyModuleFailed = "導入修模訂單失敗！";

		public static string UserView_DuplicateName = "用戶名已存在！！！";

		public static string MetalVeiw_DuplicateName = "五金件名已存在";
		public static string MetalView_RelationBom = "Bom中相關五金件存在,無法刪除";

		public static string BomPartLoad_Length_Error ="列?容缺失！！!";
		public static string BomPartView_PartExist="零件編號已存在";

		public static string MODULEID_PARTNO_NEED = "請輸入模具，零件編號";

		public static string TXT_DATEFORMAT = "日期格式輸入錯誤!!!";
		public static string TXT_PART_NOTEXIST_ELEEXIST = "此零件不存在或電極已存在";
		public static string TXT_MODULEINPUT = "請輸入模具編號";
		public static string TXT_TO ="請輸入T0時間";
		public static string TXT_START = "請輸入開始時間";
		public static string TXT_MACHINEGROUP_SELECT = "請選擇機台組";
		public static string TXT_START_BEFORE_NOW = "開始時間不能小於當前日期";

		public static string MACHINE_BUSY = "機台已使用";

		public static string NO_MACHINE_EXIST = "機台不存在";

		public static string NO_NEED_MACHINE = "不需要機台生產";

		public static string No_delete_factdatestart ="零件工序已經開始,不能刪除";

		public static string DEPART_MUST_SELECT = "請選擇部門!";
		public static string CUST_MODULE_MUST_SELECT = "請選擇客戶及相應模具!";

		public static string DELETECUSTOMER="項目預算裡有該客戶不允許刪除";
		public static string DELETEUSERS="項目預算裡有該用戶不允許刪除";

		//外包報工
		public static string TXT_SCANBARCODE2_BarcodeContentError26 = "出廠數量不能超過總數";
		public static string TXT_SCANBARCODE2_BarcodeContentError15 = "該零件此工序還未完成!";
		public static string TXT_SCANBARCODE2_BarcodeContentError27 = "回廠數量不能超過出廠數";
		public static string TXT_SCANBARCODE2_BarcodeContentError28 = "返回數量不能超過出廠數";
        public static string TXT_SCANBARCODE2_OutSourceContentError1 = "無外包工序或者未通過審核";


		public static string NOOUTSOURCE="此零件無外包工序";
		//whd080901
		public static string TXT_SCAN_NEXT = "就緒";
		public static string TXT_NO_PROCESS_PRIVILIGE ="對於此工序您無此權限";
		
		public static string FILEMANAGE_CHECKOUT ="要上傳文件請先CHECKOUT";
		public static string FILEMANAGE_USENAME ="用戶不正確";

		public static string TXT_DATANOTNULL="日期不能為空";
		public static string TXT_OUTNONOTNULL="請輸入領用單號";
		public static string TXT_MATERIALNONOTNULL="請輸入原料編號";
        public static string TXT_SESSIONTIMEOUT = "閒置時間超時";

		public static string TXT_EMAILGROUP_TXT2 = "計劃但未加工零件信息";
		public static string TXT_EMAILGROUP_VALUE2 = "2";
		public static string TXT_EMAILGROUP_TXT3 = "零件已加工但未在計劃中信息";
		public static string TXT_EMAILGROUP_VALUE3 = "3";
		public static string TXT_EMAILGROUP_TXT4 = "新增模具";
		public static string TXT_EMAILGROUP_VALUE4 = "4";
        public static string TXT_EMAILGROUP_TXT5 = "CNC計劃";
        public static string TXT_EMAILGROUP_VALUE5 = "5";

        public static string TXT_NO_Merchise = "無采購主檔，無法提交";
        public static string TXT_Must_Schedule = "保存排程必須填寫日期";
        public static string TXT_Have_Schedule = "此模具類型已經存在";

        public static string NUMBER_INPUT = "請輸入數字";
        public static string[] Init_Report = new string[92] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ", "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN" };

		public static string DuplicateKeys(string val)
		{
			return val+" 已經存在,請重新輸入!";
		}

		public static string DuplicateKeys()
		{
			return " 此筆錄入數據已經存在,請重新輸入!";
		}

		public static string DuplicateKeysCNC(string val)
		{
			return val+" 機台編號在CNC普通機,CNC高速機,或CNC電極機中已經存在,請重新輸入!";
		}

		public static string EditKeyError(string val)
		{
			return val+" 非法執行此項操作!";
		}

		public static string PrevProcessNotDone(string val)
		{
			return "該零件前一道工序[" + val + "]還沒有完成，並且這道工序不允許暫停!";
		}

		public static string MiddleScheduleAlarmMessage(int int_oldlevel, int int_newlevel)
		{
			string retmsg = null;
			if (int_oldlevel > int_newlevel)
			{
				if (int_newlevel == 1)
				{
					retmsg = "您當前的操作會使原有的零件中排程設定無效，您確定執行此操作嗎？";
				}

				if (int_newlevel == 2)
				{
					retmsg = "您當前的操作會使原有的零件中排程設定無效，您確定執行此操作嗎？";
				}
			}

			return retmsg;
		}

		public static string ErrorSettingStatus(string processname, string statusname)
		{
			return "零件當前的"+processname+"工序的狀態已經為"+statusname+",您不能設置此狀態！";
		}

	}

	public class MsSQL_ViewLang
	{
		public static string TXT_NORMAL = "正常";
		public static string TXT_DELAY = "延遲";
		//public static string TXT_EARLY = "提前";
		public static string TXT_FINISH = "結束";
	}
}
