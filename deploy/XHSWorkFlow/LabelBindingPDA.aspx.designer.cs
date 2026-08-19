namespace ModuleWorkFlow
{
    public partial class LabelBindingPDA
    {
        protected global::System.Web.UI.ScriptManager PdaScriptManager;
        protected global::System.Web.UI.UpdatePanel upPdaContent;
        protected global::System.Web.UI.WebControls.Label labScanState;
        protected global::System.Web.UI.WebControls.TextBox txtPdaScan;
        protected global::System.Web.UI.WebControls.Label labMessage;
        protected global::System.Web.UI.WebControls.Label labCustomerRaw;
        protected global::System.Web.UI.WebControls.Label labCustomerMaterial;
        protected global::System.Web.UI.WebControls.Label labExpectedPart;
        protected global::System.Web.UI.WebControls.Label labCustomerBatch;
        protected global::System.Web.UI.WebControls.Label labCustomerUnit;
        protected global::System.Web.UI.WebControls.Label labCustomerQty;
        protected global::System.Web.UI.WebControls.Label labBoundQty;
        protected global::System.Web.UI.WebControls.Label labRemainingQty;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl progressBar;
        protected global::System.Web.UI.WebControls.GridView gvBindingRecords;
        protected global::System.Web.UI.WebControls.Button btnClear;

        protected global::System.Web.UI.WebControls.Button btnEndStep;

        protected global::System.Web.UI.WebControls.Button btnCompleteBinding;
        protected global::System.Web.UI.WebControls.Panel pnlFailureModal;
        protected global::System.Web.UI.WebControls.Label labFailureTitle;
        protected global::System.Web.UI.WebControls.Label labFailureMessage;
        protected global::System.Web.UI.WebControls.Button btnFailureOk;
        protected global::System.Web.UI.WebControls.HiddenField hidScanStage;
        protected global::System.Web.UI.WebControls.HiddenField hidCustomerQty;
        protected global::System.Web.UI.WebControls.HiddenField hidBoundQty;
        protected global::System.Web.UI.WebControls.HiddenField hidRemainingQty;
    }
}
