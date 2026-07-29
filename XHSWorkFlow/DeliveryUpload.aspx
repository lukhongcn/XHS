<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Page Language="c#" CodeBehind="DeliveryUpload.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.DeliveryUpload" MasterPageFile="~/DefaultSub.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true"></asp:ScriptManager>
    <div id="Wrapper">
        <div id="Header"><div class="headbox"><div class="linebox"><a href="#">生产管理</a><img src="images/arrow.png" /><a href="#"><%=menuname %></a></div><div class="logout"><a href="login.aspx" target="_parent">登出</a></div><div class="clearbox"></div></div></div>
        <div id="Container"><div id="Content">
            <div id="Menu"><div class="menubox"><div class="mod1"><ul><li class="btn5"><asp:LinkButton ID="lnk_view" runat="server" OnClick="lnk_view_Click" ToolTip="浏览">浏览</asp:LinkButton></li></ul></div><div class="clearbox"></div></div></div>
            <div class="space1"></div>
            <div class="container mt-3 border border-primary"><div class="container mt-3">
                <div class="row mb-3"><div class="col-lg-12 d-flex"><asp:Label ID="Label_ScanQRCode" runat="server" CssClass="me-10 delivery-label" AssociatedControlID="txt_ScanQRCode">扫描二维码</asp:Label><asp:TextBox ID="txt_ScanQRCode" runat="server" CssClass="form-control text-start border-primary delivery-scan" AutoPostBack="true" OnTextChanged="txt_ScanQRCode_TextChanged"></asp:TextBox></div></div>
                <div class="row mb-3"><div class="col-lg-12 d-flex delivery-scan-type-row"><asp:RadioButtonList ID="rblScanType" runat="server" RepeatDirection="Horizontal" CssClass="delivery-scan-type"><asp:ListItem Value="KD" Selected="True">KD 标签</asp:ListItem><asp:ListItem Value="Packing">配送单</asp:ListItem></asp:RadioButtonList><asp:Button ID="btn_upload" runat="server" Text="上传" OnClick="btn_upload_Click" CssClass="delivery-upload-button" Enabled="false" /></div></div>
                <h6><asp:Label ID="Label_KdTitle" runat="server" CssClass="delivery-section-title">KD 标签信息</asp:Label></h6>
                <div class="row mb-3"><div class="col-lg-6 d-flex"><asp:Label ID="Label_KdSupplyBatchNo" runat="server" CssClass="me-10 delivery-label" AssociatedControlID="txt_KdSupplyBatchNo">供货批次号</asp:Label><asp:TextBox ID="txt_KdSupplyBatchNo" runat="server" CssClass="form-control text-start border-primary delivery-readonly" ReadOnly="true"></asp:TextBox></div><div class="col-lg-6 d-flex"><asp:Label ID="Label_KdPartNo" runat="server" CssClass="me-10 delivery-label" AssociatedControlID="txt_KdPartNo">零件编号</asp:Label><asp:TextBox ID="txt_KdPartNo" runat="server" CssClass="form-control text-start border-primary delivery-readonly" ReadOnly="true"></asp:TextBox></div></div>
                <div class="row mb-3"><div class="col-lg-6 d-flex"><asp:Label ID="Label_KdCartonNo" runat="server" CssClass="me-10 delivery-label" AssociatedControlID="txt_KdCartonNo">箱号</asp:Label><asp:TextBox ID="txt_KdCartonNo" runat="server" CssClass="form-control text-start border-primary delivery-readonly" ReadOnly="true"></asp:TextBox></div><div class="col-lg-6 d-flex"><asp:Label ID="Label_KdQty" runat="server" CssClass="me-10 delivery-label" AssociatedControlID="txt_KdQty">计划数量</asp:Label><asp:TextBox ID="txt_KdQty" runat="server" CssClass="form-control text-start border-primary delivery-readonly" ReadOnly="true"></asp:TextBox></div></div>
                <div class="row mb-3"><div class="col-lg-6 d-flex"><asp:Label ID="Label_KdPartName" runat="server" CssClass="me-10 delivery-label" AssociatedControlID="txt_KdPartName">零件名称</asp:Label><asp:TextBox ID="txt_KdPartName" runat="server" CssClass="form-control text-start border-primary delivery-readonly" ReadOnly="true"></asp:TextBox></div><div class="col-lg-6 d-flex"><asp:Label ID="Label_PackageName" runat="server" CssClass="me-10 delivery-label" AssociatedControlID="txt_PackageName">包装名称</asp:Label><asp:TextBox ID="txt_PackageName" runat="server" CssClass="form-control text-start border-primary delivery-readonly" ReadOnly="true"></asp:TextBox></div></div>
                <h6><asp:Label ID="Label_DeliveryTitle" runat="server" CssClass="delivery-section-title">配送单信息</asp:Label></h6>
                <div class="row mb-3"><div class="col-lg-6 d-flex"><asp:Label ID="Label_DeliverySupplyBatchNo" runat="server" CssClass="me-10 delivery-label" AssociatedControlID="txt_DeliverySupplyBatchNo">供货批次号</asp:Label><asp:TextBox ID="txt_DeliverySupplyBatchNo" runat="server" CssClass="form-control text-start border-primary delivery-readonly" ReadOnly="true"></asp:TextBox></div><div class="col-lg-6 d-flex"><asp:Label ID="Label_DeliveryPartNo" runat="server" CssClass="me-10 delivery-label" AssociatedControlID="txt_DeliveryPartNo">零件编号</asp:Label><asp:TextBox ID="txt_DeliveryPartNo" runat="server" CssClass="form-control text-start border-primary delivery-readonly" ReadOnly="true"></asp:TextBox></div></div>
                <div class="row mb-3"><div class="col-lg-6 d-flex"><asp:Label ID="Label_DeliveryQty" runat="server" CssClass="me-10 delivery-label" AssociatedControlID="txt_DeliveryQty">数量</asp:Label><asp:TextBox ID="txt_DeliveryQty" runat="server" CssClass="form-control text-start border-primary delivery-readonly" ReadOnly="true"></asp:TextBox></div><div class="col-lg-6 d-flex"><asp:Label ID="Label_DeliveryCardNo" runat="server" CssClass="me-10 delivery-label" AssociatedControlID="txt_DeliveryCardNo">随箱卡号</asp:Label><asp:TextBox ID="txt_DeliveryCardNo" runat="server" CssClass="form-control text-start border-primary delivery-readonly" ReadOnly="true"></asp:TextBox></div></div>
                <div class="row mb-3"><div class="col-lg-6 d-flex"><asp:Label ID="Label_DeliveryPackageCode" runat="server" CssClass="me-10 delivery-label" AssociatedControlID="txt_DeliveryPackageCode">包装单编号</asp:Label><asp:TextBox ID="txt_DeliveryPackageCode" runat="server" CssClass="form-control text-start border-primary packing-required"></asp:TextBox></div><div class="col-lg-6 d-flex"><asp:Label ID="Label_DeliveryPackageName" runat="server" CssClass="me-10 delivery-label" AssociatedControlID="txt_DeliveryPackageName">包装名称</asp:Label><asp:TextBox ID="txt_DeliveryPackageName" runat="server" CssClass="form-control text-start border-primary packing-required"></asp:TextBox></div></div>
                <div class="row mb-3"><div class="col-lg-6 d-flex"><asp:Label ID="Label_DeliveryNo" runat="server" CssClass="me-10 delivery-label" AssociatedControlID="txt_DeliveryNo">配送单号</asp:Label><asp:TextBox ID="txt_DeliveryNo" runat="server" CssClass="form-control text-start border-primary packing-required"></asp:TextBox></div></div>
                <div class="row mb-3"><div class="col-lg-6 d-flex"><asp:Label ID="Label_DeliveryMatch" runat="server" CssClass="me-10 delivery-label" AssociatedControlID="txt_DeliveryMatch">核验结果</asp:Label><asp:TextBox ID="txt_DeliveryMatch" runat="server" CssClass="form-control text-start border-primary delivery-readonly" ReadOnly="true"></asp:TextBox></div></div>
            </div></div>
            <div class="container mt-3 border border-warning"><table width="100%" align="center" class="tbMessage"><tr valign="middle"><td width="10%" height="28"><div align="center"><b>提示</b></div></td><td class="msg" width="85%">&nbsp;&nbsp;<asp:Label ID="Label_Message" runat="server"></asp:Label></td></tr></table></div>
        </div></div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="JSHolder" runat="server">
    <style>.delivery-label{min-width:110px;line-height:30px}.delivery-scan{width:100%}.delivery-readonly{background-color:#f3f3f3!important}.delivery-scan-type label{margin-right:24px}.delivery-radio-disabled{color:gray!important}.delivery-radio-enabled{color:black!important}.delivery-section-title{color:#0d6efd;margin-top:8px;margin-bottom:8px;padding-bottom:4px;border-bottom:1px solid #dee2e6}.delivery-scan-type-row{align-items:center;gap:20px}.delivery-upload-button{margin:0;padding:6px 16px;border:1px solid #007bff;border-radius:4px;background:#fff;color:#007bff;font-size:16px;line-height:1.5;white-space:nowrap}.delivery-upload-button:hover,.delivery-upload-button:focus{background:#eaf3ff;color:#0056b3}.delivery-upload-button:disabled{border-color:#c8c8c8;background:#f3f3f3;color:#b3b3b3;cursor:not-allowed}</style>
    <script type="text/javascript">
        function updateDeliveryRadioStyles() {
            var radioList = document.getElementById('<%=rblScanType.ClientID%>');
            if (!radioList) return;
            var labels = radioList.getElementsByTagName('label');
            var inputs = radioList.getElementsByTagName('input');
            for (var i = 0; i < inputs.length && i < labels.length; i++) {
                if (inputs[i].disabled) {
                    labels[i].className = 'delivery-radio-disabled';
                } else {
                    labels[i].className = 'delivery-radio-enabled';
                }
            }
        }

        function focusDeliveryScan() {
            var input = document.getElementById('<%=txt_ScanQRCode.ClientID%>');
            if (input && !input.disabled) {
                input.focus();
                input.select();
            }
        }

        function onXhsMessageModalHidden() {
            window.setTimeout(focusDeliveryScan, 0);
        }

        function deliveryScanKeyDown(event) {
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

        window.setTimeout(focusDeliveryScan, 0);
    </script>
</asp:Content>
