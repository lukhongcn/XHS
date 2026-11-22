<%@ Page Language="C#" CodePage="65001" CodeBehind="PDAWorkFlow.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.PDAWorkFlow" MasterPageFile="~/DefaultPDASub.Master" %>
<asp:Content ID="PdaContent" ContentPlaceHolderID="contentHolder" runat="server">
    <asp:ScriptManager ID="PdaScriptManager" runat="server" />
    <asp:UpdatePanel ID="upPdaContent" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <style type="text/css">
                .workflow-section{margin:8px;padding:10px;border-radius:6px;background:#fff;border:1px solid #d8dee5}.workflow-section h2{margin:0 0 8px;font-size:18px}
                .workflow-scan-dock{position:sticky;top:0;z-index:10;margin:8px;padding:10px;background:#f4f6f8}.workflow-state{display:block;padding:13px 10px;border-radius:6px;color:#fff;text-align:center;font-size:22px;font-weight:800;background:#1769aa}
                .workflow-scan{width:100%;min-height:58px;padding:8px;font-size:20px;border:3px solid #1769aa;border-radius:5px;background:#fffbd9}.workflow-scan:focus{outline:3px solid #ffb000;outline-offset:1px}
                .workflow-status{display:block;margin:8px;padding:10px;border-radius:4px;font-weight:700}.workflow-ok{color:#126b2e;background:#e5f6ea}.workflow-error{color:#a51d18;background:#fde9e7}
                .workflow-fields{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:6px}.workflow-field{padding:7px;border:1px solid #c9d2dc;border-radius:4px;background:#f5f7f9;word-break:break-all}.workflow-field label{display:block;margin-bottom:3px;font-weight:700;color:#45515d}.workflow-field span{display:block;min-height:22px}.workflow-modal-fields{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:6px;margin-bottom:10px}.workflow-modal-fields>div{padding:7px;border:1px solid #c9d2dc;border-radius:4px;background:#f5f7f9;word-break:break-all}.workflow-modal-fields label{display:block;margin-bottom:3px;font-weight:700;color:#45515d}.workflow-modal-fields span{display:block;min-height:22px}
                .workflow-grid{width:100%;table-layout:fixed;border-collapse:collapse;font-size:13px}.workflow-grid th{background:#e8edf2;white-space:nowrap}.workflow-grid th,.workflow-grid td{padding:7px 4px;border:1px solid #cbd5df;vertical-align:top;word-break:break-all}
                .workflow-modal-mask{position:fixed;inset:0;z-index:1000;display:flex;align-items:center;justify-content:center;padding:18px;background:rgba(0,0,0,.62)}.workflow-modal{width:100%;max-width:460px;padding:18px;border-radius:8px;background:#fff;box-shadow:0 8px 24px rgba(0,0,0,.35)}.workflow-modal h3{margin:0 0 12px;color:#17324d;font-size:24px}.workflow-modal label{display:block;margin:10px 0 4px;font-weight:700}.workflow-reason{width:100%;min-height:70px;padding:8px;border:1px solid #c9d2dc;border-radius:4px}.workflow-action-select{width:100%;min-height:46px;padding:6px;border:1px solid #9aa4af;border-radius:4px;background:#fff;font-size:18px}.workflow-action{width:100%;min-height:52px;margin-top:8px;border:0;border-radius:5px;font-size:18px;font-weight:700;background:#1769aa;color:#fff}.workflow-cancel{background:#6b7280}.workflow-empty{padding:14px;text-align:center;color:#6b7280}
                @media(max-width:360px){.workflow-fields{grid-template-columns:1fr}}
            </style>
            <div class="workflow-scan-dock">
                <asp:Label ID="labScanState" runat="server" CssClass="workflow-state" Text="等待扫描 QRCode"></asp:Label>
                <asp:TextBox ID="txtQRCode" runat="server" CssClass="workflow-scan" AutoPostBack="true" OnTextChanged="txtQRCode_TextChanged"></asp:TextBox>
            </div>
            <asp:Label ID="labMessage" runat="server" CssClass="workflow-status" />
            <div class="workflow-section">
                <h2>待判工序</h2>
                <asp:GridView ID="gvPartProcesses" runat="server" CssClass="workflow-grid" AutoGenerateColumns="false" EmptyDataText="暂无待判工序。" ShowHeaderWhenEmpty="true">
                </asp:GridView>
            </div>
            <asp:Panel ID="pnlActionModal" runat="server" CssClass="workflow-modal-mask" Visible="false" role="dialog" aria-modal="true">
                <div class="workflow-modal">
                    <asp:GridView ID="gvActions" runat="server" AutoGenerateColumns="false" GridLines="None" CssClass="workflow-actions-grid">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Button ID="btnAction" runat="server" CssClass="workflow-action"
                                        Text='<%# Eval("Text") %>'
                                        CommandArgument='<%# Eval("Code") %>'
                                        OnClick="ActionButton_Click"
                                        style='<%# "background-color:" + Eval("Color") + ";color:" + Eval("TextColor") + ";" %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </asp:Panel>
            <asp:HiddenField ID="hidPartNoId" runat="server" />
            <asp:HiddenField ID="hidPartProcessId" runat="server" />
            <asp:HiddenField ID="hidScannedCode" runat="server" />
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="txtQRCode" EventName="TextChanged" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="PdaScripts" ContentPlaceHolderID="JSHolder" runat="server">
    <script type="text/javascript">
        (function(){
            var busy=false;
            function scan(){return document.getElementById('<%=txtQRCode.ClientID%>')}
            function focusScan(){var e=scan();if(e){e.focus();try{e.select()}catch(x){}}}
            function wire(){var e=scan();if(!e||e.getAttribute('data-workflow-wired')==='1')return;e.setAttribute('data-workflow-wired','1');e.setAttribute('autocomplete','off');e.setAttribute('autocorrect','off');e.setAttribute('autocapitalize','off');e.setAttribute('spellcheck','false');e.onkeydown=function(ev){ev=ev||window.event;if((ev.which||ev.keyCode)===13){ev.preventDefault();if(busy)return false;busy=true;__doPostBack('<%=txtQRCode.UniqueID%>','');return false}}}
            function ready(){wire();window.setTimeout(focusScan,40)}
            if(window.Sys&&Sys.WebForms){var p=Sys.WebForms.PageRequestManager.getInstance();p.add_beginRequest(function(){busy=true});p.add_endRequest(function(){busy=false;ready()})}
            if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',ready);else ready();
        })();
    </script>
</asp:Content>
