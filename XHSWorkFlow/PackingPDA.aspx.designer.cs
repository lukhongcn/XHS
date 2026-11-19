namespace ModuleWorkFlow
{
    public partial class PackingPDA
    {
        protected global::System.Web.UI.WebControls.Panel pnlLocked;
        protected global::System.Web.UI.WebControls.Label Label_LockMessage;
        protected global::System.Web.UI.WebControls.Label Label_PdaState;
        protected global::System.Web.UI.WebControls.TextBox txt_ScanQRCode;
        protected global::System.Web.UI.WebControls.RadioButtonList rblScanType;
        protected global::System.Web.UI.WebControls.LinkButton lnk_continue_packing;
        protected global::System.Web.UI.WebControls.LinkButton lnk_unbind;
        protected global::System.Web.UI.WebControls.TextBox txt_SupplyBatchNo;
        protected global::System.Web.UI.WebControls.TextBox txt_PartNo;
        protected global::System.Web.UI.WebControls.TextBox txt_DeliveryNo;
        protected global::System.Web.UI.WebControls.TextBox txt_CartonNo;
        protected global::System.Web.UI.WebControls.TextBox txt_PlanQty;
        protected global::System.Web.UI.WebControls.TextBox txt_MaterialNo;
        protected global::System.Web.UI.WebControls.TextBox txt_Qty;
        protected global::System.Web.UI.WebControls.TextBox txt_Status;
        protected global::System.Web.UI.WebControls.TextBox txt_ExceptionStatus;
        protected global::System.Web.UI.WebControls.TextBox txt_LockToken;
        protected global::System.Web.UI.WebControls.HiddenField hidPackingId;
        protected global::System.Web.UI.WebControls.HiddenField hidTaskId;
        protected global::System.Web.UI.WebControls.HiddenField hidLockToken;
        protected global::System.Web.UI.WebControls.HiddenField hidPackingCompleteMode;
        protected global::System.Web.UI.WebControls.HiddenField hidDeleteScanId;
        protected global::System.Web.UI.WebControls.HiddenField hidResetKdScanId;
        protected global::System.Web.UI.WebControls.HiddenField hidPendingMessage;
        protected global::System.Web.UI.WebControls.GridView gvScanRecords;
        protected global::System.Web.UI.WebControls.Panel pnlDeleteModal;
        protected global::System.Web.UI.WebControls.Panel pnlResetKdModal;
        protected global::System.Web.UI.WebControls.LinkButton lnkbutton_delete;
        protected global::System.Web.UI.WebControls.TextBox txt_ResetKd;
        protected global::System.Web.UI.WebControls.LinkButton lnkbutton_reset_kd;
        protected global::System.Web.UI.WebControls.Panel pnlUnbindModal;
        protected global::System.Web.UI.WebControls.LinkButton lnkbutton_unbind;
        protected global::System.Web.UI.WebControls.Label Label_Message;
    }
}
