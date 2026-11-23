<%@ Page Language="C#" CodePage="65001" CodeBehind="FXWorkFlow.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.FXWorkFlow" MasterPageFile="~/DefaultPDASub.Master" %>
<asp:Content ID="PdaContent" ContentPlaceHolderID="contentHolder" runat="server">
    <asp:ScriptManager ID="PdaScriptManager" runat="server" />
    <asp:UpdatePanel ID="upPdaContent" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <style type="text/css">
                .workflow-section{margin:8px;padding:10px;border-radius:6px;background:#fff;border:1px solid #d8dee5}.workflow-section h2{margin:0 0 8px;font-size:18px}
                .workflow-scan-dock{position:sticky;top:0;z-index:10;margin:8px;padding:10px;background:#f4f6f8}.workflow-state{display:block;padding:13px 10px;border-radius:6px;color:#fff;text-align:center;font-size:22px;font-weight:800;background:#1769aa}
                .workflow-scan{width:100%;min-height:58px;padding:8px;font-size:20px;border:3px solid #1769aa;border-radius:5px;background:#fffbd9}.workflow-scan:focus{outline:3px solid #ffb000;outline-offset:1px}
                .workflow-status{display:block;margin:8px;padding:10px;border-radius:4px;font-weight:700}.workflow-ok{color:#126b2e;background:#e5f6ea}.workflow-error{color:#a51d18;background:#fde9e7}
                .workflow-grid{width:100%;table-layout:fixed;border-collapse:collapse;font-size:13px}.workflow-grid th{background:#e8edf2;white-space:nowrap}.workflow-grid th,.workflow-grid td{padding:7px 4px;border:1px solid #cbd5df;vertical-align:top;word-break:break-all}
                .workflow-modal-mask{position:fixed;inset:0;z-index:1000;display:flex;align-items:center;justify-content:center;padding:18px;background:rgba(0,0,0,.62)}.workflow-modal{width:100%;max-width:460px;padding:18px;border-radius:8px;background:#fff;box-shadow:0 8px 24px rgba(0,0,0,.35)}
                .workflow-modal-message{display:block;margin-bottom:12px;padding:12px;border-radius:5px;color:#17324d;background:#eef4fb;font-size:20px;font-weight:700;text-align:center}
                .workflow-action{width:100%;min-height:52px;margin-top:8px;border:0;border-radius:5px;font-size:18px;font-weight:700;color:#fff}.workflow-recheck{background:#4CAF50}.workflow-scrap{background:#F44336}.workflow-pack{background:#2196F3}
                .workflow-replace,.workflow-action-list{box-sizing:border-box;width:100%;min-height:38px;padding:5px;border:1px solid #9aa4af;border-radius:4px;background:#fff;font-size:16px}
            </style>
            <div class="workflow-scan-dock">
                <asp:Label ID="labScanState" runat="server" CssClass="workflow-state" Text="等待扫描 QRCode"></asp:Label>
                <asp:TextBox ID="txtQRCode" runat="server" CssClass="workflow-scan" AutoPostBack="true" OnTextChanged="txtQRCode_TextChanged"></asp:TextBox>
            </div>
            <asp:Label ID="labMessage" runat="server" CssClass="workflow-status" Visible="false" />
            <div class="workflow-section">
                <h2>待处理资料</h2>
                <asp:GridView ID="gvRecords" runat="server" CssClass="workflow-grid" AutoGenerateColumns="false"
                    EmptyDataText="暂无待处理资料。" ShowHeaderWhenEmpty="true">
                    <Columns>
                        <asp:BoundField DataField="RowNumber" HeaderText="序号" />
                        <asp:BoundField DataField="PartNoId" HeaderText="零件编号" />
                        <asp:BoundField DataField="ProcessNo" HeaderText="ProcessNo" />
                        <asp:TemplateField HeaderText="替换QRCode">
                            <ItemTemplate>
                                <asp:TextBox ID="txtReplaceQRCode" runat="server" CssClass="workflow-replace"
                                    Text='<%# Eval("ReplaceQRCode") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="操作">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlAction" runat="server" CssClass="workflow-action-list"
                                    SelectedValue='<%# Eval("ActionCode") %>'>
                                    <asp:ListItem Text="重检" Value="RECHECK" />
                                    <asp:ListItem Text="报废" Value="SCRAP" />
                                    <asp:ListItem Text="装箱" Value="PACK" />
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
            <asp:Panel ID="pnlActionModal" runat="server" CssClass="workflow-modal-mask" Visible="false" role="dialog" aria-modal="true">
                <div class="workflow-modal">
                    <asp:Label ID="labModalMessage" runat="server" CssClass="workflow-modal-message" Text="请选择处理动作" />
                    <asp:Button ID="btnRecheck" runat="server" Text="重检" CommandName="RECHECK"
                        CssClass="workflow-action workflow-recheck" OnClick="ActionButton_Click" />
                    <asp:Button ID="btnScrap" runat="server" Text="报废" CommandName="SCRAP"
                        CssClass="workflow-action workflow-scrap" OnClick="ActionButton_Click" />
                    <asp:Button ID="btnPack" runat="server" Text="装箱" CommandName="PACK"
                        CssClass="workflow-action workflow-pack" OnClick="ActionButton_Click" />
                </div>
            </asp:Panel>
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
