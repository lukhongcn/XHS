using System;


namespace Utility
{
    /// <summary>
    /// Summary description for Lang.
    /// </summary>
    public class Lang
    {
        public static string TXT_CUSTOMERNUMBER_NO_EXISTED = "客户别不存在";
        public static string TXT_PARTMODULEID_NO_EXISTED = "厂批不存在";
        public static string TXT_PARTMODULEID_NO_CREATED = "厂批建立失败";

        public static string TXT_MODULEID = "模具编号";
        public static string TXT_INPUTTRYTIMEERROR = "设定日期的工序没有设定相应的时间!";
        public static string TXT_DROPDOWNLISTTWO = "零件大类的第二个类必须填写";
        public static string TXT_INPUTTRYTIMES = "请输入试模次数!";
        public static string TXT_INPUTTRYTIME = "请输入试模时间!";
        public static string TXT_MODIFYIDHASEXISTED = "修模编号已经存在！";
        public static string TXT_INPUTTRYDATE = "请输入试模日期!";
        public static string TXT_MODIFYIDFORMATERR = "您输入的修模编号不正确，修模编号规则为：模具编号+'-2**'";
        public static string TXT_SELECTMODIFYID = "请选择有效的修模编号";
        public static string TXT_SELECTPROJECT = "请选择工程项目！";
        public static string TXT_SETTINGSUCCESS = "镜面机台设定成功！";
        public static string TXT_DONETOSCHEDULE = "重新排程已经完成！";
        public static string TXT_SELECTCUSTOMIDMODULEIDPARTNO = "请选择有效的客户名称，模具编号，零件编号";
        public static string TXT_DATELOGICALERROR = "您输入的日期有逻辑错误，请检查!";
        public static string TXT_HINTERROR = "无效";
        public static string TXT_SELECTMODULEID = "请选择有效的模具编号";
        public static string TXT_ContainSpecCode = "零件编号含有非法特殊字符，如'+'等";
        public static string TXT_CAN_NOT_DELETED_PRODUCTING_PROCESS = "您不能设定零件已经生产的工序的所需时间为0";
        public static string TXT_TIMERANGEERROR = "时间范围设定错误，请重新设定！";
        public static string TXT_CREATEEXCELFAILED = "创建文件失败，请检查输入参数的合法性！";
        public static string TXT_CREATEEXCELSUCCESS = "创建文件成功！";
        public static string TXT_CONTACTERNOTNULL = "姓名必须填写";
        public static string TXT_CONTACTERNOTPHONE = "电话或者手机号码必须填写一个";
        public static string TXT_PRICEINPUTERROR = "价格输入错误";
        public static string TXT_SHOWSTATUSADD = " (添加)";
        public static string TXT_SHOWSTATUSEDIT = " (编辑)";
        public static string TXT_TYPECONVERTERROR = "类型转换错误，请检查您输入数据的是否为正确的类型！";
        public static string TXT_AUTOCREATE = "(自动产生)";
        public static string TXT_KEYWORDS = "关键字";
        public static string TXT_SELECT = "选择";
        public static string TXT_CANCEL = "取消";
        public static string TXT_ADD = "保存";
        public static string TXT_EDIT = "保存";
        public static string TXT_YES = "是";
        public static string TXT_NO = "否";
        public static string TXT_EXISTRELATIONDATA = "该模具由于有关联零件信息数据存在,您不能执行此操作,请先删除零件信息!";
        public static string TXT_NOPRIVILEGE = "您无权执行此项操作!";
        public static string TXT_LOGINFAILED = "员工编号或口令错误!";
        public static string TXT_NOREGISTED = "软件没有注册，请先注册!";
        public static string TXT_OPERATIONERROR = "这是一个非法错误,请联系开发人员!";
        public static string TXT_ADDSUCCESS = "添加成功!";
        public static string TXT_EDITSUCCESS = "编辑成功!";
        public static string TXT_DELETESUCCESS = "删除成功!";
        public static string TXT_DELETECONFIRM = "您确定要删除这条纪录吗?";
        public static string TXT_SCHEDULESUCCESS = "创建排程成功!";
        public static string TXT_SCHEDULEFAILURE = "创建排程失败!";
        public static string TXT_STARTDATEERROR = "请输入正确的开始日期!";
        public static string TXT_DATEERROR = "请输入正确的日期!";
        public static string TXT_STARTENDDATEERROR = "结束日期必须大于开始日期!";
        public static string TXT_PARTNOIDERROR = "输入的零件编号有误！";
        public static string TXT_ELECTRODENOERROR = "请输入合法的电极编号,电极编号为'模仁编号'+'M'+'电极编号'。如：101M01";
        public static string TXT_NOTADDCOMPLATE = "请完整填写信息";
        public static string TXT_UNIQUE = "请确认ShiftAbbr的唯一性";
        public static string TXT_SELECTNAME = "  ";
        public static string TXT_SELECTVALUE = "select";
        public static string TXT_SELECTALLNAME = "全部";
        public static string TXT_SELECTALLVALUE = "all";

        public static string MODULEID_PARTNO_NOT_EXIST = "模具或零件编号或工序不存在，请录入模具，零件信息";

        // 这组讯息用于改成“厂批...”的需求，避免修改原本常数字串
        public static string MODULEID_PARTNO_NOT_EXIST_FACTORY = "厂批或零件编号或工序不存在";

        public static string MACHINE_EMPTY = "机体空闲，不可在此机体选择结束或暂停";

        public static string TXT_STARTDATE_NOSPACE = "请输入计划开始时间";
        public static string TXT_ENDDATE_NOSPACE = "请输入计划完成时间";
        public static string TXT_TIME = "开始时间和完成时间设定不统一";
        public static string SPEC_FORMAT_ERROR = "规格格式为 A*B*C ,ABC都只能为数字";


        public static string TXT_MACHINESTATUSADD_SelectModuleid = "请选择模具编号!";
        public static string TXT_MACHINESTATUSADD_SelectPartnoid = "请选择零件编号!";
        public static string TXT_MACHINESTATUSADD_SelectProcessorder = "请选择工序号!";
        public static string TXT_MACHINESTATUSADD_SelectDate = "请选择日期！";
        public static string TXT_MACHINESTATUSADD_SettingTime = "请设定加工时间！";
        public static string TXT_MACHINESTATUSADD_NotOverOneDay = "修改时，不允许设定加工工时大于机台有效一天加工工时！";
        public static string TXT_MACHINESTATUSADD_OverMachineTime = "您设定的加工时间和机台现有的加工时间累计超过24小时，请重新设定！";

        public static string TXT__PARTMIDDLESCHEDULEPART_Level0 = "模具当前无中排程!";
        public static string TXT__PARTMIDDLESCHEDULEPART_Level1 = "模具当前为模具级中排程!";
        public static string TXT__PARTMIDDLESCHEDULEPART_Level2 = "模具当前为零件级中排程!";
        public static string TXT__PARTMIDDLESCHEDULEPART_Level3 = "模具当前为零件个数级中排程!";

        public static string TXT__PARTMODULEMIDDLESCHEDULE_ModuleNull = "请选择模具编号!";
        public static string TXT__PARTMODULEMIDDLESCHEDULE_CreateBigScheduleFailed = "自动创建模具中日程失败!";
        public static string TXT__PARTMODULEMIDDLESCHEDULE_SetBigScheduleSucc = "设定中日程成功!";
        public static string TXT__PARTMODULEMIDDLESCHEDULE_SetBigScheduleFailed = "设定中日程失败!请联系开发人员！";
        public static string TXT__PARTMODULEMIDDLESCHEDULE_SelectProcess = "请选择工序！";
        public static string TXT__PARTMODULEMIDDLESCHEDULE_SetStartEndOrNot = "请选择开始日期和结束日期或者都不设定！";
        public static string TXT__PARTMODULEMIDDLESCHEDULE_DateTimeFormatError = "开始日期和结束日期格式设定有误！";
        public static string TXT__PARTMODULEMIDDLESCHEDULE_DateTimeInputError = "开始日期大于结束日期，设定有误！";

        public static string TXT_MODULEMIDDLESCHEDULE_SelectProcess = "请先选择工序！";
        public static string TXT_MODULEMIDDLESCHEDULE_SelectStartDate = "请先设定此工序中排程开始日期！";
        public static string TXT_MODULEMIDDLESCHEDULE_SelectEndDate = "请先设定此工序中排程结束日期！";
        public static string TXT_MODULEMIDDLESCHEDULE_StartDateSmallerThanEndDate = "开始日期大于结束日期，请重新设定！";

        public static string TXT_SCANBARCODE_BarcodeEmpty = "请扫描或输入条形码!";
        public static string TXT_SCANBARCODE_BarcodeError = "条形码不合法,请从新扫描或输入!";
        public static string TXT_SCANBARCODE_SelectProcess = "此零件无这道工序，请重新选择工序或条形码!";

        public static string TXT_PROJECTADD_ModifyidError = "您输入的修模编号有误，请检查！";
        public static string TXT_PROJECTADD_DisProjectIdError = "您输入的指示书编号有误，请检查！";
        public static string TXT_PROJECTADD_DisProjectIdExisted = "您输入的指示书编号已经存在，请检查！";

        public static string TXT_PROJECTLIST_DeleteInfo = "删除操作将要删除此指示书对应的订单及排程信息，您确定要执行这个操作吗?";

        public static string TXT_PROCESSNAME = "工序名称";
        public static string TXT_PROCESSORDER = "工序序号";
        public static string TXT_STARTTIME = "开始日期";
        public static string TXT_ENDTIME = "结束日期";
        public static string TXT_PROCESSTIME = "工时（小时）";
        public static string TXT_NEEDMINUTESZERO = "您有工序设定的工时为零，请重新设定！";
        public static string TXT_QRCODENEED = "必须设定二维码";
        public static string TXT_PARTADDEDIT_NoSelected = "请选择工序！";
        public static string NO_User = "不存在此用户";

        public static string TXT_SCANBARCODE2_BarcodeContent = "输入的条码内容为 ";
        public static string TXT_SCANBARCODE2_INPUTACTIONFIRST = "输入的条码内容为 ";
        public static string TXT_SCANBARCODE2_BarcodeContentError = "输入的条码内容有误! ";
        public static string TXT_SCANBARCODE2_BarcodeContentError2 = "请先设定工序和零件编码! ";
        public static string TXT_SCANBARCODE2_PARTALREADYINPUT = "结束或暂停零件以机台加工为准，无需刷人零件";
        public static string TXT_SCANBARCODE2_BarcodeContentError3 = "该零件此工序都已完成! ";
        public static string TXT_SCANBARCODE2_BarcodeContentError4 = "您的操作动作不正确，零件的当前状态为 ";
        public static string TXT_SCANBARCODE2_BarcodeContentError5 = "此零件没有这道工序，或者这个零件不存在！";
        public static string TXT_SCANBARCODE2_BarcodeContentError6 = "请先设定工序! ";
        public static string TXT_SCANBARCODE2_BarcodeContentError7 = "您的操作动作不正确，零件已经开始加工，零件的当前状态为 ";
        public static string TXT_SCANBARCODE2_BarcodeContentError8 = "该零件已经有开始的工艺,该工艺信息如下：";
        public static string TXT_SCANBARCODE2_BarcodeContentError9 = "请输入零件编号。如：1 或1,2 或1-3";
        public static string TXT_SCANBARCODE2_BarcodeContentError10 = "请先输入用户编号、零件编号、动作信息！";
        public static string TXT_SCANBARCODE2_BarcodeContentError101 = "请先输入动作信息！";
        public static string TXT_SCANBARCODE2_BarcodeContentError11 = "工序不能为组立！";
        public static string TXT_SCANBARCODE2_BarcodeContentError12 = "由于输入的是模号，所以工序请选择组立！";
        public static string TXT_SCANBARCODE2_ReadyStart = "零件已经开始，无法继续开始！";
        public static string TXT_SCANBARCODE2_ReadyHoldOnOrEnd = "零件不是开始状态，无法继续暂停或结束！";
        public static string TXT_ModifyPart_ReadyChange = "零件工序状态可能在其他电脑已经更改，无法继续变更！";
        //MYD070615
        public static string TXT_SCANBARCODE2_BarcodeContentError13 = "工件并没有完成，因此无法进行返工操作！";
        public static string TXT_SCANBARCODE2_BarcodeContentError14 = "员工不存在";
        public static string TXT_SCANBARCODE2_BarcodeContentError30 = "请先输入模具编号、用户编号、动作信息！";
        public static string TXT_SCANBARCODE2_BarcodeContentError31 = "此模具不存在，或者这个模具没有结案！";

        public static string TXT_ORDERLIST_OrderCount = "当前模具数量为";

        public static string TXT_OUTSOURCINGLIST_PartCount = "当前零件数量为";

        public static string TXT_USERLIST_InputSearch = "请输入搜索的内容！";
        public static string TXT_USERLIST_SaveSucc = "保存信息成功！";
        public static string TXT_USERLIST_Savefailed = "保存信息失败，请联系相关人员！";
        public static string TXT_USERLIST_InputUserName = "请输入员工编号！";
        public static string TXT_USERLIST_InputName = "请输入员工姓名！";
        public static string TXT_USERLIST_KeyDuplicated = "该员工编号已经存在，请重新输入！";
        public static string TXT_USERLIST_ErrorURL = "非法连接！";
        public static string TXT_DELETEUSERBYORDER = "定单表有此用户不允许删除";
        public static string TXT_MODULEMIDDLESCHEDULEVIEW_ModuleCountZero = "当前查询到的模具数量为0！";

        public static string TXT_PARTUNNORMALADD_SelectPartNo = "请选择零件!";
        public static string TXT_PARTUNNORMALADD_DiscardSucc = "该零件曾经报废[{0}]次，第[{1}]次报废设定成功！零件报废新编号为[{2}]!";
        public static string TXT_PARTUNNORMALADD_DiscardFailed = "保存信息失败，请联系相关人员！";
        public static string TXT_PARTUNNORMALADD_Newpartno = "该零件所有工序都没有开始加工，为新零件，不能报废！";
        public static string TXT_PARTUNNORMALADD_ConfirmDiscard = "您确定要报废这个零件吗？";

        public static string TXT_OUTSOURCINGADD_SelectModule = "请选择模具编号！";
        public static string TXT_OUTSOURCINGADD_SelectPartNo = "请选择零件编号！";
        public static string TXT_OUTSOURCINGADD_SelectPartNoId = "请选择零件ID编号！";

        public static string MetalView_Delete = "请先删除此类零件";
        public static string BomType_Delete = "请先删除与此相关的零件";
        public static string TXT_NOPRIVILIGE = "您无权限进行此操作";

        public static string OUTPUTCOUNT_BIGER_STOCK = "出库数量大于库存数量";
        public static string INPUTCOUNT_BIGER_PARTCOUNT = "入库数量大于零件数量";


        /* popupdiscardinfo.aspx */
        public static string TXT_POPUPDISCARDINFO_ErrorInput = "错误的模具编号！";

        public static string TXT_PARTMODIFYPROCESSORDER_saveError = "保存信息发生错误！";

        //订单设定修改页面
        public static string TXT_ORDERADDEDIT_SetAssembleTime = "请设定组立的加工工时！";

        public static string TXT_SCHEDULEREPORT_InputStartDateEndDate = "请选择开始日期和结束日期！";

        //电子邮件列表
        public static string TXT_EMAILGROUP_TXT0 = "模具完成进度警示信息";
        public static string TXT_EMAILGROUP_VALUE0 = "0";
        public static string TXT_EMAILGROUP_TXT1 = "零件正在超时加工警示信息";
        public static string TXT_EMAILGROUP_VALUE1 = "1";

        public static string TXT_EMAILGROUPEDIT_SelectUser = "请选择用户！";

        //上传文件
        public static string TXT_UPLOADIMAGE_SELECTFile = "请选择文件！";

        //电机修改页面
        public static string TXT_ELECTRODEADDEDIT_NoInput = "请输入最少需要数量/个！";

        //条码输入页面处理修模部分
        public static string TXT_PROJECTADD_INPUTModifyModuleFailed = "导入修模订单失败！";

        public static string UserView_DuplicateName = "用户名已存在！！！";

        public static string MetalVeiw_DuplicateName = "五金件名已存在";
        public static string MetalView_RelationBom = "Bom中相关五金件存在,无法删除";

        public static string BomPartLoad_Length_Error = "列内容缺失！！!";
        public static string BomPartView_PartExist = "零件编号已存在";
        public static string BOMEXISTT = "BOM已经存在";



        public static string MODULEID_PARTNO_NEED = "请输入模具，零件编号";

        public static string TXT_DATEFORMAT = "日期格式输入错误!!!";
        public static string TXT_PART_NOTEXIST_ELEEXIST = "此零件不存在或电极已存在";
        public static string TXT_MODULEINPUT = "请输入模具编号";
        public static string TXT_TO = "请输入T0时间";
        public static string TXT_START = "请输入开始时间";
        public static string TXT_MACHINEGROUP_SELECT = "请选择机台组";
        public static string TXT_START_BEFORE_NOW = "开始时间不能小于当前日期";

        public static string MACHINE_BUSY = "机台已使用";

        public static string NO_MACHINE_EXIST = "机台不存在";

        public static string NO_NEED_MACHINE = "不需要机台生产";

        public static string No_delete_factdatestart = "零件工序已经开始,不能删除";

        public static string DEPART_MUST_SELECT = "请选择部门!";
        public static string CUST_MODULE_MUST_SELECT = "请选择客户及相应模具!";

        public static string DELETECUSTOMER = "项目预算里有该客户不允许删除";
        public static string DELETEUSERS = "项目预算里有该用户不允许删除";

        //外包报工
        public static string TXT_SCANBARCODE2_BarcodeContentError26 = "出厂数量不能超过总数";
        public static string TXT_SCANBARCODE2_BarcodeContentError15 = "该零件此工序还未完成!";
        public static string TXT_SCANBARCODE2_BarcodeContentError27 = "回厂数量不能超过出厂数";
        public static string TXT_SCANBARCODE2_BarcodeContentError28 = "返回数量不能超过出厂数";
        public static string TXT_SCANBARCODE2_OutSourceContentError1 = "无外包工序或者未通过审核";


        public static string NOOUTSOURCE = "此零件无外包工序";
        //whd080901
        public static string TXT_SCAN_NEXT = "就绪";
        public static string TXT_NO_PROCESS_PRIVILIGE = "对于此工序您无此权限";

        public static string FILEMANAGE_CHECKOUT = "要上传文件请先CHECKOUT";
        public static string FILEMANAGE_USENAME = "用户不正确";

        public static string TXT_DATANOTNULL = "日期不能为空";
        public static string TXT_OUTNONOTNULL = "请输入领用单号";
        public static string TXT_MATERIALNONOTNULL = "请输入原料编号";
        public static string TXT_SESSIONTIMEOUT = "闲置时间超时";

        public static string TXT_EMAILGROUP_TXT2 = "计划但未加工零件信息";
        public static string TXT_EMAILGROUP_VALUE2 = "2";
        public static string TXT_EMAILGROUP_TXT3 = "零件已加工但未在计划中信息";
        public static string TXT_EMAILGROUP_VALUE3 = "3";
        public static string TXT_EMAILGROUP_TXT4 = "新增模具";
        public static string TXT_EMAILGROUP_VALUE4 = "4";
        public static string TXT_EMAILGROUP_TXT5 = "CNC计划";
        public static string TXT_EMAILGROUP_VALUE5 = "5";

        public static string TXT_NO_Merchise = "无采购主档，无法提交";
        public static string TXT_Must_Schedule = "保存排程必须填写日期";
        public static string TXT_Have_Schedule = "此模具类型已经存在";

        public static string NUMBER_INPUT = "请输入数字";



        public static string INPUT_NUMBER = "请输入数字";
        public static string DENSITY_INPUT_NUMBER = "价格,密度请输入数字";
        public static string SAVE_SUCCESS = "保存成功";
        public static string SAVE_FAIL = "保存失败";
        public static string DELETE_SUCCESS = "删除成功";
        public static string DELETE_FAIL = "删除失败";
        public static string Edit_SUCCESS = "编辑成功";
        public static string Edit_FALL = "编辑失败";
        public static string NAME_DUPLICATE = "名称已存在";
        public static string NO_PROCESS = "请设定工序";
        public static string PROCESS_CHANGED = "工序已单独编辑过，请使用异常处理方法修改";
        public static string NO_MERCHINDISE = "采购编号重复";
        public static string OUT_INPUT = "退货数量不能大于进货数量,请重新输入";
        public static string DO_PROCESS = "工序正在使用中，无法删除";
        public static string NO_PRODUCTNO = "无材料编号";

        public static string NO_ERROR = "请设定正确的编码";
        public static string NO_DUPLICATE = "编码重复";
        public static string APPLY_NO_EXISTS = "不存在委外申请单";

        public static string INPUT_MODULE_PART = "请输入模具编号及零件编号";

        public static string NO_NOT_UNIQUE = "编码重复，请确认其唯一性。";
        public static string MERCHINDISE_NO_DEFAULT = "采购单号前12位为自动产生";
        public static string MERCHINDISE_NO_MODULE = "采购单号-后必须为存在的模具编号";

        public static string SHEET_FIRST = "请先输入申请单";
        public static string OUT_TOO_BIG = "出库量不能大于库存量";
        public static string NO_REJECT_PROJECT = "无退货项目";

        public static string NO_Product = "有库存不能删除";

        public static string FILENAME_FORMAT = "文件名不能包含_";

        public static string CHECKER_INPUT = "必须输入审核者";

        public static string MERCHINDISE_BIG_APPLY = "申购数量大于请购数量";

        public static string NO_DELETE = "已审核不能删除";

        public static string MERCHINDISE_SHEET_FIRST = "请先输入采购单号";

        public static string NO_ENGLISH = "不是纯英文，请重新输入";

        public static string PART_OR_MODULE_NOT_EXIST = "零件,模具不存在或库存数量大于工艺数量";

        public static string SHLF_NO_ERROR = "货架横向，纵向设定格式为 x-x";

        public static string CHECK_DATE = "请输入审核日期";

        public static string NODELDEPARTMENT = "因管理员存在,该部门不能被删除";

        public static DateTime DEFAULTDATE = Convert.ToDateTime("1960-01-01");

        //order over
        public static string OVER_MODULEID_ERROR = "请输入正确的结案编号";


        public static string APPROV_PASS = "核准通过";

        public static string APPROV_FAIL = "核准失败";

        public static string NO_REQUESTION_NO_ADD = "无申请单无法添加";

        public static string OUTSOURCE_OUTSOURCECOUNT = "外包数量必须小于可外包数量且必须选择至少一个工序";
        public static string OUTSOURCE_PROCESS_CONTINUED = "一次外包的工序必须连续";
        public static string OUTSOURCE_OUTSOURCEEXIST = "此零件此工序已外包";

        public static string OUTSOURCE_EXIST = "工序正在加工中";

        public static string PartNoIdNull = "零件小编号不能为空";

        public static string NO_MERCHISE = "此采购单已经不存在，此数据已报废，不能进行任何操作，请删除";
        public static string MERCHINDISE_ERROR = "无采购单,不能进行此操作";

        public static string NO_Customer = "客户正在使用，不能删除";

        public static string NO_DETAIL = "有明细，不能删除";

        public static string X0_X1 = "横向编码必须X0<X1";
        public static string Y0_Y1 = "纵向编码必须Y0<Y1";

        public static string DATE1_DATE2 = "两个日期不能相差60天";
        public static string DATE1DATE2 = "结束日期必须大于开始日期";

        public static string INPUT_FIRST = "必须输入";

        public static string MODULE_CUSTOMER_EXISTED = "该客户已经在订单中存在，请先删除订单";
        public static string MODULE_CUSTOMERMODEL_EXISTED = "该客户已经在机种中存在，请先删除订单";
        public static string CUSTOMERMODEL_EXISTED = "该客户已经在机种中存在，请先删除机种";

        public static string DATE_TYPE_CHANGE_ERROR = "请输入正确的日期格式";
        public static string NO_FIND = "所查找的记录不存在";
        public static string NO_SPACES = "不能为空";
        //public static string INPUT_DATETIME = "请输入正确时间");
        public static string INPUT_MACHINE = "请输入几台号";

        public static string INPUT_PROGRAMNO = "请输入程式编号";
        public static string INPUT_CountERROR = "超出库存数量";
        public static string DUPLICATE = "不存在";
        public static string PRODUCTPARTNO_NOT_EXIST = "工令单不存在";

        public static string INPUT_DATETIME = "请输入日期";
        public static string NO_HAVE = "不能为空";
        public static string NO_REJECT = "存在退货单不能删除";

        public static string No_ModuleIdINorder = "模具编号不能为空";
        public static string SELECT_PART = "请选择零件";
        public static string QRCODE_BOUNDED = "二维码已经绑定";
        public static string PART_EXIST = "零件已存在";
        public static string PART_DELETED = "零件已删除";

        public static string DELAYTIME = "未确定";

        public static string COUNTRY_IS_USING = "国家使用中，不可删除。";

        public static string ORDER_IS_USING = "订单使用中，不可删除。";
        //
        //		public static string NO_PRODUCTNO = "没有项目号");

        public static string MACHINE_MULTUPLE = "机台编号重复";

        public static string MACHINE_IPADDRESS_MULTUPLE = "机台IP地址重复";

        public static string NO_RETURN = "同一物料不能入库两次";

        public static string DONOT_ADD_DESIGNPARTPROCESS = "此模具相同设计类别的工序已存在";

        public static string SINGLE_NO_SPACES = "工令单号不能为空";

        public static string SPEC_FORMAT_ERRORTWO = "格式为 A*B*C 或 A*B ,ABC都只能为数字";

        public static string ORDER_DUPLICATE = "模具编号已存在";

        public static string NO_ORDER = "没有模具编号";

        public static string INPUT_ORDERID = "模具编号不能为空";

        public static string FORMATETIMEERROR = "需要使用正确格式 ：(00:00)";

        public static string TRYCHANGERFALL = "领用数量修改会使仓库数量成负数,操作失败";

        public static string No_TryScheduleNo = "试模编号不能为空";

        public static string No_UserNo = "此员工编号不存在";

        public static string NO_PARTNOID = "工件名称不能为空";

        public static string NO_T0 = "试模日期不能为空";

        public static string NO_CREATEDATE = "开单日期不能为空";

        public static string NO_UNORMALREASONS = "返修原因不能为空";

        public static string NO_USER = "员工不存在";

        public static string ProductMaterialNo_Repeat = "新增的原物料编号重复";

        public static string ProductMaterialNmae_Repeat = "新增的原物料名称重复";

        public static string Transsatciton_No_Exist = "交易信息不存在";

        public static string Customer_No_Exist = "客户信息不存在";

        public static string HeatProcess_No_Exist = "热处理方式不存在";

        public static string ContractNo_No_Exist = "合同编号没有填写";

        public static string Material_No_Exist = "材质不存在";

        public static string No_Number = "非数字";

        public static string ConstractNo_No_Equals = "合同编号不符合";

        public static string PROCESSINGSHEET_EXIST = "加工单存在,请先删除加工单";

        public static string TOTAL = "总计";

        //Quote
        public static string QUOTE_PROCESS_DUPLICATE = "工序编号已存在";
        public static string QUOTE_PROCESS_INPUT = "请输入工序编号";
        public static string QUOTE_PRICE_NUMBER = "价格请输入数字";

        public static string DuplicateKeys(string val)
        {
            return val + " 已经存在,请重新输入!";
        }

        public static string DuplicateKeys()
        {
            return " 此笔录入数据已经存在,请重新输入!";
        }

        public static string DuplicateKeysCNC(string val)
        {
            return val + " 机台编号在CNC普通机,CNC高速机,或CNC电极机中已经存在,请重新输入!";
        }

        public static string EditKeyError(string val)
        {
            return val + " 非法执行此项操作!";
        }

        public static string PrevProcessNotDone(string val)
        {
            return "该零件前一道工序" + "[" + val + "]" + "还没有完成，并且这道工序不允许暂停!";
        }

        public static string MiddleScheduleAlarmMessage(int int_oldlevel, int int_newlevel)
        {
            string retmsg = null;
            if (int_oldlevel > int_newlevel)
            {
                if (int_newlevel == 1)
                {
                    retmsg = "您当前的操作会使原有的零件中排程设定无效，您确定执行此操作吗？";
                }

                if (int_newlevel == 2)
                {
                    retmsg = "您当前的操作会使原有的零件中排程设定无效，您确定执行此操作吗？";
                }
            }

            return retmsg;
        }

        public static string ErrorSettingStatus(string processname, string statusname)
        {
            return "零件当前的" + processname + "工序的状态已经为" + statusname + "," + "您不能设置此状态！";
        }


    public class MsSQL_ViewLang
    {
        public static string TXT_NORMAL = "正常";
        public static string TXT_DELAY = "延迟";
        //public static string TXT_EARLY = "提前";
        public static string TXT_FINISH = "结束";
    }

    public Lang()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}
