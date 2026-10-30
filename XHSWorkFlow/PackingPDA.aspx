<%@ Page Language="C#" CodePage="65001" CodeBehind="PackingPDA.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.PackingPDA" MasterPageFile="~/DefaultPDASub.Master" %>
<asp:Content ID="PdaContent" ContentPlaceHolderID="contentHolder" runat="server">
    <asp:ScriptManager ID="PdaScriptManager" runat="server" />
    <asp:UpdatePanel ID="upPdaContent" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <style type="text/css">
                .packing-pda-section{margin:8px;padding:10px;border-radius:6px;background:#fff;border:1px solid #d8dee5}.packing-pda-section h2{margin:0 0 8px;font-size:18px}
                .packing-pda-dock{position:sticky;top:0;z-index:10;margin:8px;padding:10px;background:#f4f6f8}.packing-pda-scan{width:100%;min-height:58px;padding:8px;font-size:20px;border:3px solid #1769aa;border-radius:5px;background:#fffbd9}.packing-pda-scan:focus{outline:3px solid #ffb000;outline-offset:1px}.packing-pda-scan:disabled{background:#d9dee3;color:#6b7280;border-color:#9aa4af;opacity:1}
                .packing-pda-type{display:flex;flex-wrap:wrap;align-items:center;gap:12px;margin-top:8px;font-size:16px}.packing-pda-type label{margin-right:10px}.packing-pda-qty{display:inline-flex;align-items:center;gap:6px;margin-left:auto;font-weight:700}.packing-pda-qty .packing-pda-readonly{width:90px;min-height:34px;padding:4px;text-align:center}.packing-pda-continue{min-height:34px;padding:4px 12px;border:0;border-radius:5px;background:#1769aa;color:#fff;font-size:15px;font-weight:700}.packing-pda-unbind{background:#d9534f}.packing-pda-continue:disabled,.packing-pda-continue.aspNetDisabled,.packing-pda-type span.packing-pda-continue{background:#aeb8c2;color:#eef2f5;cursor:not-allowed}.packing-pda-state{display:block;margin-bottom:8px;padding:10px;border-radius:6px;color:#fff;text-align:center;font-size:20px;font-weight:800;background:#1769aa}
                .packing-pda-message{display:block;margin:8px 0 0;padding:4px 0;font-weight:700;color:#c62828;background:transparent}.packing-pda-field{margin:7px 0}.packing-pda-field label{display:block;margin-bottom:3px;font-weight:700}.packing-pda-readonly{width:100%;min-height:40px;padding:7px;border:1px solid #c9d2dc;border-radius:4px;background:#f5f7f9;word-break:break-all}.packing-pda-required{background:#fffbd9}.packing-pda-summary{display:grid;grid-template-columns:repeat(2,1fr);gap:5px}.packing-pda-summary div{padding:7px 4px;border-radius:4px;background:#eef2f5}.packing-pda-grid{width:100%;border-collapse:collapse;font-size:13px}.packing-pda-grid th{background:#e8edf2;white-space:nowrap}.packing-pda-grid th,.packing-pda-grid td{padding:6px 4px;border:1px solid #cbd5df;vertical-align:top;word-break:break-all}.packing-pda-button{width:100%;min-height:48px;margin-top:8px;border:0;border-radius:5px;font-size:17px;font-weight:700}.packing-pda-primary{background:#1769aa;color:#fff}.packing-pda-modal-mask{position:fixed;inset:0;z-index:1000;display:flex;align-items:center;justify-content:center;padding:18px;background:rgba(0,0,0,.62)}.packing-pda-modal{width:100%;max-width:460px;padding:18px;border-radius:8px;background:#fff;box-shadow:0 8px 24px rgba(0,0,0,.35)}.packing-pda-modal-header{display:flex;justify-content:space-between;align-items:center;padding-bottom:10px;border-bottom:1px solid #dee2e6;font-size:20px}.packing-pda-close{border:0;background:transparent;font-size:26px}.packing-pda-modal-body{padding:14px 0}.packing-pda-modal-footer{display:flex;gap:8px}.packing-pda-modal-footer .btn{flex:1;min-height:44px}
            </style>
            <div class="packing-pda-dock">
                <asp:Label ID="Label_PdaState" runat="server" CssClass="packing-pda-state" Text="等待 KD 标签"></asp:Label>
                <asp:TextBox ID="txt_ScanQRCode" runat="server" CssClass="packing-pda-scan" AutoPostBack="true" OnTextChanged="txt_ScanQRCode_TextChanged"></asp:TextBox>
                <div class="packing-pda-type"><asp:RadioButtonList ID="rblScanType" runat="server" RepeatDirection="Horizontal"><asp:ListItem Value="KD" Selected="True" Text="KD 标签" /><asp:ListItem Value="Material" Text="零件标签" /><asp:ListItem Value="Packing" Text="随箱卡" /></asp:RadioButtonList><asp:LinkButton ID="lnk_continue_packing" runat="server" CssClass="packing-pda-continue" Text="继续装箱" Enabled="false" OnClick="lnk_continue_packing_Click" /><asp:LinkButton ID="lnk_unbind" runat="server" CssClass="packing-pda-continue packing-pda-unbind" Text="解除绑定" OnClick="lnk_unbind_Click" OnClientClick="return showPackingPdaUnbindModal();" Visible="false" /></div>
                <asp:Label ID="Label_Message" runat="server" CssClass="packing-pda-message"></asp:Label>
            </div>
            <asp:Panel ID="pnlLocked" runat="server" CssClass="packing-pda-modal-mask" Visible="true" Style="display:none" role="alertdialog" aria-modal="true"><div class="packing-pda-modal"><div class="packing-pda-modal-header"><strong>装箱异常已暂停</strong></div><div class="packing-pda-modal-body"><asp:Label ID="Label_LockMessage" runat="server" /><br />等待主管审核</div></div></asp:Panel>
             <div class="packing-pda-section"><h2>装箱任务</h2><div class="packing-pda-field"><label>供货批次号</label><asp:TextBox ID="txt_SupplyBatchNo" runat="server" ReadOnly="true" CssClass="packing-pda-readonly" /></div><div class="packing-pda-field"><label>零件编号</label><asp:TextBox ID="txt_PartNo" runat="server" ReadOnly="true" CssClass="packing-pda-readonly" /></div><div class="packing-pda-field"><label>配送单号</label><asp:TextBox ID="txt_DeliveryNo" runat="server" CssClass="packing-pda-readonly packing-pda-required" /></div><div class="packing-pda-summary"><div><strong><asp:Literal ID="litCartonNoTitle" runat="server" Text="箱号" /></strong><asp:TextBox ID="txt_CartonNo" runat="server" ReadOnly="true" CssClass="packing-pda-readonly" /></div><div><strong>装箱数量</strong><asp:TextBox ID="txt_Qty" runat="server" Text="0" ReadOnly="true" CssClass="packing-pda-readonly" /></div></div></div>
            <div class="packing-pda-section"><h2>装箱状态</h2><div class="packing-pda-field"><label>物料号</label><asp:TextBox ID="txt_MaterialNo" runat="server" ReadOnly="true" CssClass="packing-pda-readonly" /></div><div class="packing-pda-summary"><div><strong>计划数量</strong><asp:TextBox ID="txt_PlanQty" runat="server" ReadOnly="true" CssClass="packing-pda-readonly" /></div><div><strong>装箱状态</strong><asp:TextBox ID="txt_Status" runat="server" ReadOnly="true" CssClass="packing-pda-readonly" /></div></div><div class="packing-pda-field"><label>异常状态</label><asp:TextBox ID="txt_ExceptionStatus" runat="server" ReadOnly="true" CssClass="packing-pda-readonly" /></div></div>
            <div class="packing-pda-section"><h2>扫描明细</h2><asp:GridView ID="gvScanRecords" runat="server" CssClass="packing-pda-grid" AutoGenerateColumns="false" EmptyDataText="暂无扫描记录。" ShowHeaderWhenEmpty="true"><Columns><asp:BoundField DataField="QRCodeType" HeaderText="二维码类型" /><asp:BoundField DataField="QRCode" HeaderText="二维码" /><asp:BoundField DataField="MaterialNo" HeaderText="物料号" /><asp:BoundField DataField="Qty" HeaderText="数量" /><asp:BoundField DataField="ScanUser" HeaderText="扫描人" /><asp:BoundField DataField="ScanTime" HeaderText="扫描时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" /><asp:TemplateField HeaderText="操作"><ItemTemplate><asp:LinkButton ID="lnk_ResetKd" runat="server" CssClass="btn btn-warning" Text="重置KD" ToolTip="替换此 KD 标签" CommandArgument='<%# Eval("Id") %>' OnClientClick='<%# GetResetKdClientClick(Eval("Id")) %>' Visible='<%# IsKdScanRecord(Eval("QRCodeType")) %>' /><asp:LinkButton ID="lnk_Delete" runat="server" CommandName="Delete" CommandArgument='<%# Eval("Id") %>' Text="删除" ToolTip="删除此零件" OnClientClick='<%# GetDeleteClientClick(Eval("Id")) %>' Visible='<%# IsMaterialScanRecord(Eval("QRCodeType")) %>' /></ItemTemplate></asp:TemplateField></Columns></asp:GridView></div>
            <asp:Panel ID="pnlDeleteModal" runat="server" CssClass="packing-pda-modal-mask" Style="display:none" role="alertdialog" aria-modal="true"><div class="packing-pda-modal" onclick="event.stopPropagation();"><div class="packing-pda-modal-header"><strong>确认删除</strong></div><div class="packing-pda-modal-body">确认删除这条零件扫描明细吗？删除后装箱数量会重新计算。</div><div class="packing-pda-modal-footer"><button type="button" class="btn btn-secondary" onclick="hidePackingPdaDeleteModal();">取消</button><asp:LinkButton ID="lnkbutton_delete" runat="server" CssClass="btn btn-danger" OnClick="lnkbutton_delete_Click" Text="确定删除" /></div></div></asp:Panel>
            <asp:Panel ID="pnlResetKdModal" runat="server" CssClass="packing-pda-modal-mask" Style="display:none" role="dialog" aria-modal="true"><div class="packing-pda-modal" onclick="event.stopPropagation();"><div class="packing-pda-modal-header"><strong>重置 KD</strong></div><div class="packing-pda-modal-body"><label for="txt_ResetKd">请输入新的 KD 标签</label><asp:TextBox ID="txt_ResetKd" runat="server" CssClass="packing-pda-scan" MaxLength="200" /></div><div class="packing-pda-modal-footer"><button type="button" class="btn btn-secondary" onclick="hidePackingPdaResetKdModal();">取消</button><asp:LinkButton ID="lnkbutton_reset_kd" runat="server" CssClass="btn btn-primary" OnClick="lnkbutton_reset_kd_Click" Text="检查并替换" /></div></div></asp:Panel>
            <asp:Panel ID="pnlUnbindModal" runat="server" CssClass="packing-pda-modal-mask" Style="display:none" role="alertdialog" aria-modal="true"><div class="packing-pda-modal" onclick="event.stopPropagation();"><div class="packing-pda-modal-header"><strong>确认解除绑定</strong></div><div class="packing-pda-modal-body">解除后当前 PackId 的未上传零件绑定会清理，页面将返回 KD 输入状态；已上传记录不会被解除。</div><div class="packing-pda-modal-footer"><button type="button" class="btn btn-secondary" onclick="hidePackingPdaUnbindModal();">取消</button><asp:LinkButton ID="lnkbutton_unbind" runat="server" CssClass="btn btn-danger" OnClick="lnk_unbind_Click" Text="确认解除" /></div></div></asp:Panel>
            <asp:HiddenField ID="hidPackingId" runat="server" /><asp:HiddenField ID="hidTaskId" runat="server" /><asp:HiddenField ID="hidLockToken" runat="server" /><asp:HiddenField ID="hidPackingCompleteMode" runat="server" /><asp:HiddenField ID="hidDeleteScanId" runat="server" /><asp:HiddenField ID="hidResetKdScanId" runat="server" /><asp:HiddenField ID="hidPendingMessage" runat="server" /><asp:TextBox ID="txt_LockToken" runat="server" Style="display:none"></asp:TextBox>
        </ContentTemplate>
        <Triggers><asp:AsyncPostBackTrigger ControlID="txt_ScanQRCode" EventName="TextChanged" /><asp:AsyncPostBackTrigger ControlID="lnkbutton_delete" EventName="Click" /><asp:AsyncPostBackTrigger ControlID="lnkbutton_reset_kd" EventName="Click" /><asp:AsyncPostBackTrigger ControlID="lnkbutton_unbind" EventName="Click" /><asp:AsyncPostBackTrigger ControlID="lnk_continue_packing" EventName="Click" /></Triggers>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="PdaScripts" ContentPlaceHolderID="JSHolder" runat="server"><script type="text/javascript">
 (function(){
    var busy=false;
    var loadWired=false;

    function scan(){return document.getElementById('<%=txt_ScanQRCode.ClientID%>')}

    function focusScan(){
        var e=scan();
        if(e&&!e.disabled){
            e.focus();
            try{e.select()}catch(x){}
            if(e.scrollIntoView){e.scrollIntoView(false)}
        }
    }

    function focusScanSoon(){
        window.setTimeout(focusScan,50);
        window.setTimeout(focusScan,200);
    }

    function wire(){
        var e=scan();
        if(!e||e.getAttribute('data-pda-wired')==='1')return;
        e.setAttribute('data-pda-wired','1');
        e.setAttribute('autocomplete','off');
        e.setAttribute('autocorrect','off');
        e.setAttribute('autocapitalize','off');
        e.setAttribute('spellcheck','false');
        e.onkeydown=function(ev){
            ev=ev||window.event;
            if((ev.which||ev.keyCode)===13){
                ev.preventDefault();
                if(busy)return false;
                busy=true;
                __doPostBack('<%=txt_ScanQRCode.UniqueID%>','');
                return false
            }
        }
    }

    function ready(){
        wire();
        focusScanSoon()
    }

    window.showPackingPdaDeleteModal=function(scanId){
        var hidden=document.getElementById('<%=hidDeleteScanId.ClientID%>');
        if(hidden)hidden.value=scanId||'';
        var e=document.getElementById('<%=pnlDeleteModal.ClientID%>');
        if(e)e.style.display='flex';
        var button=e?e.querySelector('.btn-danger'):null;
        if(button)button.focus();
        return false;
    };
    window.hidePackingPdaDeleteModal=function(){
        var e=document.getElementById('<%=pnlDeleteModal.ClientID%>');
        if(e)e.style.display='none';
        focusScanSoon();
    };
    window.showPackingPdaResetKdModal=function(scanId){
        var hidden=document.getElementById('<%=hidResetKdScanId.ClientID%>');
        if(hidden)hidden.value=scanId||'';
        var input=document.getElementById('<%=txt_ResetKd.ClientID%>');
        var e=document.getElementById('<%=pnlResetKdModal.ClientID%>');
        if(e)e.style.display='flex';
        if(input){input.value='';input.focus();}
        return false;
    };
    window.hidePackingPdaResetKdModal=function(){
        var e=document.getElementById('<%=pnlResetKdModal.ClientID%>');
        if(e)e.style.display='none';
        focusScanSoon();
    };
    window.showPackingPdaUnbindModal=function(){
        var e=document.getElementById('<%=pnlUnbindModal.ClientID%>');
        if(e)e.style.display='flex';
        return false;
    };
    window.hidePackingPdaUnbindModal=function(){
        var e=document.getElementById('<%=pnlUnbindModal.ClientID%>');
        if(e)e.style.display='none';
        focusScanSoon();
    };

    function wireAjaxLifecycle(){
        if(loadWired||!window.Sys||!Sys.Application)return;
        loadWired=true;
        Sys.Application.add_load(function(){busy=false;ready()});
        if(Sys.WebForms){
            var p=Sys.WebForms.PageRequestManager.getInstance();
            p.add_beginRequest(function(){busy=true});
        }
    }

    function start(){
        wireAjaxLifecycle();
        ready();
    }

    if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',start);else start();
    window.setTimeout(start,120)
 })();
</script></asp:Content>
