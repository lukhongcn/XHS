<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Page Language="c#" CodeBehind="Packing.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.Packing" MasterPageFile="~/DefaultSub.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true"></asp:ScriptManager>
    <div id="Wrapper">
        <div id="Header"><div class="headbox"><div class="linebox"><a href="#">生产管理</a><img src="images/arrow.png" /><a href="#"><%=menuname %></a></div><div class="logout"><a href="login.aspx" target="_parent">登出</a></div><div class="clearbox"></div></div></div>
        <div id="Container"><div id="Content">
            <div id="Menu"><div class="menubox"><div class="mod1"><ul><li class="btn5"><asp:LinkButton ID="lnk_view" runat="server" OnClick="lnk_view_Click" ToolTip="浏览">浏览</asp:LinkButton></li><li class="btn2"><asp:LinkButton ID="lnkbutton_packing_complete" runat="server" OnClick="lnkbutton_packing_complete_Click" ToolTip="装箱完成">装箱完成</asp:LinkButton></li></ul></div><div class="mod2"><ul><li class="btn3"><asp:LinkButton ID="lnkbutton_save" runat="server" ToolTip="保存/save" OnClick="lnkbutton_save_Click">保存/save</asp:LinkButton></li></ul></div><div class="clearbox"></div></div></div>
            <div class="space1"></div>
            <asp:Panel ID="pnlLocked" runat="server" CssClass="packing-lock-panel" Visible="true" Style="display:none" role="alertdialog" aria-modal="true"><div class="packing-lock-card"><strong>装箱异常已暂停</strong><br /><asp:Label ID="Label_LockMessage" runat="server"></asp:Label><br />等待主管审核</div></asp:Panel>
            <div class="container mt-3 border border-primary"><div class="container mt-3">
                <div class="row mb-3"><div class="col-lg-12 d-flex"><asp:Label ID="Label_ScanQRCode" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_ScanQRCode">扫描二维码</asp:Label><asp:TextBox ID="txt_ScanQRCode" runat="server" CssClass="form-control text-start border-primary packing-scan" AutoPostBack="true" OnTextChanged="txt_ScanQRCode_TextChanged"></asp:TextBox></div></div>
                <div class="row mb-3"><div class="col-lg-12 d-flex"><asp:RadioButtonList ID="rblScanType" runat="server" RepeatDirection="Horizontal" CssClass="packing-scan-type"><asp:ListItem Value="KD" Selected="True">KD 标签</asp:ListItem><asp:ListItem Value="Material">零件标签</asp:ListItem><asp:ListItem Value="Packing">随箱码</asp:ListItem></asp:RadioButtonList></div></div>
                <div class="row mb-3"><div class="col-lg-6 d-flex"><asp:Label ID="Label_SupplyBatchNo" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_SupplyBatchNo">供货批次号</asp:Label><asp:TextBox ID="txt_SupplyBatchNo" runat="server" CssClass="form-control text-start border-primary packing-required"></asp:TextBox></div><div class="col-lg-6 d-flex"><asp:Label ID="Label_PartNo" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_PartNo">零件编号</asp:Label><asp:TextBox ID="txt_PartNo" runat="server" CssClass="form-control text-start border-primary packing-required"></asp:TextBox></div></div>
                <div class="row mb-3"><div class="col-lg-6 d-flex"><asp:Label ID="Label_CartonNo" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_CartonNo">箱号</asp:Label><asp:TextBox ID="txt_CartonNo" runat="server" CssClass="form-control text-start border-primary packing-required"></asp:TextBox></div><div class="col-lg-6 d-flex"><asp:Label ID="Label_PlanQty" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_PlanQty">计划数量</asp:Label><asp:TextBox ID="txt_PlanQty" runat="server" CssClass="form-control text-start border-primary packing-required"></asp:TextBox></div></div>
                <div class="row mb-3"><div class="col-lg-6 d-flex"><asp:Label ID="Label_MaterialNo" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_MaterialNo">物料号</asp:Label><asp:TextBox ID="txt_MaterialNo" runat="server" CssClass="form-control text-start border-primary"></asp:TextBox></div><div class="col-lg-6 d-flex"><asp:Label ID="Label_Qty" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_Qty">装箱数量</asp:Label><asp:TextBox ID="txt_Qty" runat="server" Text="0" CssClass="form-control text-start border-primary packing-readonly" ReadOnly="true"></asp:TextBox></div></div>
                <div class="row mb-3"><div class="col-lg-6 d-flex"><asp:Label ID="Label_Status" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_Status">装箱状态</asp:Label><asp:TextBox ID="txt_Status" runat="server" CssClass="form-control text-start border-primary packing-readonly" ReadOnly="true"></asp:TextBox></div><div class="col-lg-6 d-flex"><asp:Label ID="Label_ExceptionStatus" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_ExceptionStatus">异常状态</asp:Label><asp:TextBox ID="txt_ExceptionStatus" runat="server" CssClass="form-control text-start border-primary packing-readonly" ReadOnly="true"></asp:TextBox></div></div>
                <div class="row mb-3"><div class="col-lg-12 d-flex"><asp:Label ID="Label_LockToken" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_LockToken">当前令牌</asp:Label><asp:TextBox ID="txt_LockToken" runat="server" CssClass="form-control text-start border-primary packing-readonly" ReadOnly="true"></asp:TextBox></div></div>
                <asp:HiddenField ID="hidPackingId" runat="server" /><asp:HiddenField ID="hidLockToken" runat="server" /><asp:HiddenField ID="hidPackingCompleteMode" runat="server" />
            </div></div>
            <div class="container mt-3 border border-primary"><div class="container mt-3"><h5>扫描明细</h5><asp:GridView ID="gvScanRecords" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="false" EmptyDataText="暂无扫描记录。" ShowHeaderWhenEmpty="true"><Columns><asp:BoundField DataField="QRCodeType" HeaderText="二维码类型" /><asp:BoundField DataField="QRCode" HeaderText="二维码" /><asp:BoundField DataField="MaterialNo" HeaderText="物料号" /><asp:BoundField DataField="Qty" HeaderText="数量" /><asp:BoundField DataField="ScanUser" HeaderText="扫描人" /><asp:BoundField DataField="ScanTime" HeaderText="扫描时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" /></Columns></asp:GridView></div></div>
            <div class="container mt-3 border border-warning"><table width="100%" align="center" class="tbMessage"><tr valign="middle"><td width="10%" height="28"><div align="center"><b>提示</b></div></td><td class="msg" width="85%">&nbsp;&nbsp;<asp:Label ID="Label_Message" runat="server"></asp:Label></td></tr></table></div>
        </div></div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="JSHolder" runat="server">
    <style>.packing-label{min-width:110px;line-height:30px}.packing-scan{width:100%}.packing-required{background-color:#fff59d!important}.packing-readonly{background-color:#f3f3f3!important}.packing-scan-type label{margin-right:24px}.packing-radio-disabled{color:gray!important}.packing-radio-enabled{color:black!important}.packing-lock-panel{position:fixed;inset:0;z-index:10000;align-items:center;justify-content:center;background:rgba(0,0,0,.45);color:#842029;padding:16px;font-size:18px}.packing-lock-card{min-width:360px;max-width:720px;padding:32px;text-align:center;border:1px solid #dc3545;border-radius:6px;background:#fff3cd;box-shadow:0 8px 24px rgba(0,0,0,.25);line-height:1.8}</style>
    <script type="text/javascript">
        function updatePackingRadioStyles() {
            var radioList = document.getElementById('<%=rblScanType.ClientID%>');
            if (!radioList) return;
            var labels = radioList.getElementsByTagName('label');
            var inputs = radioList.getElementsByTagName('input');
            for (var i = 0; i < inputs.length && i < labels.length; i++) {
                if (inputs[i].disabled) {
                    labels[i].className = 'packing-radio-disabled';
                } else {
                    labels[i].className = 'packing-radio-enabled';
                }
            }
        }

        function focusPackingScan() {
            var input = document.getElementById('<%=txt_ScanQRCode.ClientID%>');
            if (input && !input.disabled) {
                input.focus();
                input.select();
            }
        }

        function onXhsMessageModalHidden() {
            window.setTimeout(focusPackingScan, 0);
        }

        function packingScanKeyDown(event) {
            event = event || window.event;
            var keyCode = event.which || event.keyCode;
            if (keyCode === 13) {
                if (event.preventDefault) {
                    event.preventDefault();
                }
                __doPostBack('<%=txt_ScanQRCode.UniqueID%>', '');
                return false;
            }
            return true;
        }

        window.setTimeout(focusPackingScan, 0);
    </script>
</asp:Content>
