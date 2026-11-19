<%@ Page Language="C#" CodeBehind="DeliveryUploadPDA.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.DeliveryUploadPDA" MasterPageFile="~/DefaultPDASub.Master" %>
<asp:Content ID="PdaContent" ContentPlaceHolderID="contentHolder" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <asp:UpdatePanel ID="upPdaContent" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <style type="text/css">
                .delivery-pda-section{margin:8px;padding:10px;border-radius:6px;background:#fff;border:1px solid #d8dee5}.delivery-pda-section h2{margin:0 0 8px;font-size:18px}.delivery-pda-dock{position:sticky;top:0;z-index:10;margin:8px;padding:10px;background:#f4f6f8}.delivery-pda-scan{width:100%;min-height:58px;padding:8px;font-size:20px;border:3px solid #1769aa;border-radius:5px;background:#fffbd9}.delivery-pda-scan:focus{outline:3px solid #ffb000;outline-offset:1px}.delivery-pda-type{display:flex;flex-wrap:wrap;gap:12px;margin-top:8px;font-size:16px}.delivery-pda-type label{margin-right:10px}.delivery-pda-state{display:block;margin-bottom:8px;padding:10px;border-radius:6px;color:#fff;text-align:center;font-size:20px;font-weight:800;background:#1769aa}.delivery-pda-message{display:block;margin:8px;padding:10px;border-radius:4px;font-weight:700;background:#eef2f5}.delivery-pda-field{margin:7px 0}.delivery-pda-field label{display:block;margin-bottom:3px;font-weight:700}.delivery-pda-readonly,.delivery-pda-required{width:100%;min-height:40px;padding:7px;border:1px solid #c9d2dc;border-radius:4px;word-break:break-all}.delivery-pda-readonly{background:#f5f7f9}.delivery-pda-required{background:#fffbd9}.delivery-pda-summary{display:grid;grid-template-columns:repeat(2,1fr);gap:5px}.delivery-pda-summary>div{padding:7px 4px;border-radius:4px;background:#eef2f5}.delivery-pda-button{width:100%;min-height:48px;margin-top:8px;border:0;border-radius:5px;font-size:17px;font-weight:700;background:#1769aa;color:#fff}.delivery-pda-button:disabled{background:#9aa4af;color:#e5e7eb}.delivery-pda-radio-disabled{color:gray!important}.delivery-pda-radio-enabled{color:black!important}
            </style>
            <div class="delivery-pda-dock">
                <asp:Label ID="Label_PdaState" runat="server" CssClass="delivery-pda-state" Text="请先扫描 KD 标签" />
                <asp:TextBox ID="txt_ScanQRCode" runat="server" CssClass="delivery-pda-scan" AutoPostBack="true" OnTextChanged="txt_ScanQRCode_TextChanged" />
                <div class="delivery-pda-type"><asp:RadioButtonList ID="rblScanType" runat="server" RepeatDirection="Horizontal"><asp:ListItem Value="KD" Selected="True">KD 标签</asp:ListItem><asp:ListItem Value="Packing">配送单</asp:ListItem></asp:RadioButtonList></div>
                <asp:Button ID="btn_upload" runat="server" Text="上传配送单" OnClick="btn_upload_Click" CssClass="delivery-pda-button" Enabled="false" />
            </div>
            <asp:Label ID="Label_Message" runat="server" CssClass="delivery-pda-message" />
            <div class="delivery-pda-section"><h2>KD 标签信息</h2>
                <div class="delivery-pda-field"><label>供货批次号</label><asp:TextBox ID="txt_KdSupplyBatchNo" runat="server" ReadOnly="true" CssClass="delivery-pda-readonly" /></div>
                <div class="delivery-pda-field"><label>零件编号</label><asp:TextBox ID="txt_KdPartNo" runat="server" ReadOnly="true" CssClass="delivery-pda-readonly" /></div>
                <div class="delivery-pda-summary"><div><label>箱号</label><asp:TextBox ID="txt_KdCartonNo" runat="server" ReadOnly="true" CssClass="delivery-pda-readonly" /></div><div><label>计划数量</label><asp:TextBox ID="txt_KdQty" runat="server" ReadOnly="true" CssClass="delivery-pda-readonly" /></div></div>
                <div class="delivery-pda-field"><label>零件名称</label><asp:TextBox ID="txt_KdPartName" runat="server" ReadOnly="true" CssClass="delivery-pda-readonly" /></div>
            </div>
            <div class="delivery-pda-section"><h2>配送单信息</h2>
                <div class="delivery-pda-summary"><div><label>供货批次号</label><asp:TextBox ID="txt_DeliverySupplyBatchNo" runat="server" ReadOnly="true" CssClass="delivery-pda-readonly" /></div><div><label>零件编号</label><asp:TextBox ID="txt_DeliveryPartNo" runat="server" ReadOnly="true" CssClass="delivery-pda-readonly" /></div></div>
                <div class="delivery-pda-summary"><div><label>数量</label><asp:TextBox ID="txt_DeliveryQty" runat="server" ReadOnly="true" CssClass="delivery-pda-readonly" /></div><div><label>随箱卡流水号</label><asp:TextBox ID="txt_SxCardSeq" runat="server" CssClass="delivery-pda-required" AutoPostBack="true" OnTextChanged="txt_SxCardSeq_TextChanged" /></div></div>
                <div class="delivery-pda-field"><label>包装单编号</label><asp:TextBox ID="txt_DeliveryPackageCode" runat="server" CssClass="delivery-pda-required" /></div>
                <div class="delivery-pda-field"><label>包装名称</label><asp:TextBox ID="txt_DeliveryPackageName" runat="server" CssClass="delivery-pda-required" /></div>
                <div class="delivery-pda-field"><label>配送单号</label><asp:TextBox ID="txt_DeliveryNo" runat="server" CssClass="delivery-pda-required" /></div>
                <div class="delivery-pda-field"><label>核验结果</label><asp:TextBox ID="txt_DeliveryMatch" runat="server" ReadOnly="true" CssClass="delivery-pda-readonly" /></div>
                <asp:DropDownList ID="dpl_baseno" runat="server" Style="display:none"><asp:ListItem Value="" Text="" /></asp:DropDownList>
            </div>
        </ContentTemplate>
        <Triggers><asp:AsyncPostBackTrigger ControlID="txt_ScanQRCode" EventName="TextChanged" /><asp:AsyncPostBackTrigger ControlID="txt_SxCardSeq" EventName="TextChanged" /><asp:AsyncPostBackTrigger ControlID="btn_upload" EventName="Click" /></Triggers>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="PdaScripts" ContentPlaceHolderID="JSHolder" runat="server"><script type="text/javascript">
function deliveryScanKeyDown(event){event=event||window.event;var keyCode=event.which||event.keyCode;if(keyCode===13){if(event.preventDefault)event.preventDefault();__doPostBack('<%=txt_ScanQRCode.UniqueID%>','');return false}return true}
function updateDeliveryRadioStyles(){var e=document.getElementById('<%=rblScanType.ClientID%>');if(!e)return;var l=e.getElementsByTagName('label'),i=e.getElementsByTagName('input');for(var n=0;n<i.length&&n<l.length;n++)l[n].className=i[n].disabled?'delivery-pda-radio-disabled':'delivery-pda-radio-enabled'}
function focusDeliveryScan(){var e=document.getElementById('<%=txt_ScanQRCode.ClientID%>');if(e&&!e.disabled){e.focus();try{e.select()}catch(x){}}}
(function(){function ready(){var e=document.getElementById('<%=txt_ScanQRCode.ClientID%>');if(e){e.setAttribute('autocomplete','off');e.setAttribute('onkeydown','return deliveryScanKeyDown(event);')}updateDeliveryRadioStyles();window.setTimeout(focusDeliveryScan,30)}if(window.Sys&&Sys.WebForms){Sys.WebForms.PageRequestManager.getInstance().add_endRequest(ready)}if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',ready);else ready()})();
</script></asp:Content>
