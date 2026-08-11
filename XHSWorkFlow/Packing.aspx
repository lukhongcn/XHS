<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Page Language="c#" CodeBehind="Packing.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.Packing" MasterPageFile="~/DefaultSub.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true"></asp:ScriptManager>
    <div id="Wrapper">
        <div id="Header"><div class="headbox"><div class="linebox"><a href="#">生产管理</a><img src="images/arrow.png" /><a href="#"><%=menuname %></a></div><div class="logout"><a href="login.aspx" target="_parent">登出</a></div><div class="clearbox"></div></div></div>
        <div id="Container"><div id="Content">
            <div id="Menu"><div class="menubox"><div class="mod1"><ul><li class="btn5"><asp:LinkButton ID="lnk_view" runat="server" OnClick="lnk_view_Click" ToolTip="浏览">浏览</asp:LinkButton></li></ul></div><div class="clearbox"></div></div></div>
            <div class="space1"></div>
            <asp:Panel ID="pnlLocked" runat="server" CssClass="packing-lock-panel" Visible="true" Style="display:none" role="alertdialog" aria-modal="true"><div class="packing-lock-card"><strong>装箱异常已暂停</strong><br /><asp:Label ID="Label_LockMessage" runat="server"></asp:Label><br />等待主管审核</div></asp:Panel>
            <div class="container mt-3 border border-primary"><div class="container mt-3">
                <div class="row mb-3"><div class="col-lg-12 d-flex"><asp:Label ID="Label_ScanQRCode" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_ScanQRCode">扫描二维码</asp:Label><asp:TextBox ID="txt_ScanQRCode" runat="server" CssClass="form-control text-start border-primary packing-scan" AutoPostBack="true" OnTextChanged="txt_ScanQRCode_TextChanged"></asp:TextBox></div></div>
                <div class="row mb-3"><div class="col-lg-12 d-flex packing-scan-type-row"><asp:RadioButtonList ID="rblScanType" runat="server" RepeatDirection="Horizontal" CssClass="packing-scan-type"><asp:ListItem Value="KD" Selected="True">KD 标签</asp:ListItem><asp:ListItem Value="Material">零件标签</asp:ListItem><asp:ListItem Value="Packing">配送单</asp:ListItem></asp:RadioButtonList><asp:Button ID="btn_packing_complete" runat="server" Text="装箱完成" OnClick="btn_packing_complete_Click" CssClass="packing-complete-button" /></div></div>
                <div class="row mb-3"><div class="col-lg-6 d-flex"><asp:Label ID="Label_SupplyBatchNo" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_SupplyBatchNo">供货批次号</asp:Label><asp:TextBox ID="txt_SupplyBatchNo" runat="server" CssClass="form-control text-start border-primary packing-required"></asp:TextBox></div><div class="col-lg-6 d-flex"><asp:Label ID="Label_PartNo" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_PartNo">零件编号</asp:Label><asp:TextBox ID="txt_PartNo" runat="server" CssClass="form-control text-start border-primary packing-required"></asp:TextBox></div></div>
                <div class="row mb-3"><div class="col-lg-6 d-flex"><asp:Label ID="Label_CartonNo" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_CartonNo">箱号</asp:Label><asp:TextBox ID="txt_CartonNo" runat="server" CssClass="form-control text-start border-primary packing-required"></asp:TextBox></div><div class="col-lg-6 d-flex"><asp:Label ID="Label_PlanQty" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_PlanQty">计划数量</asp:Label><asp:TextBox ID="txt_PlanQty" runat="server" CssClass="form-control text-start border-primary packing-required"></asp:TextBox></div></div>
                <div class="row mb-3"><div class="col-lg-6 d-flex"><asp:Label ID="Label_MaterialNo" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_MaterialNo">物料号</asp:Label><asp:TextBox ID="txt_MaterialNo" runat="server" CssClass="form-control text-start border-primary"></asp:TextBox></div><div class="col-lg-6 d-flex"><asp:Label ID="Label_Qty" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_Qty">装箱数量</asp:Label><asp:TextBox ID="txt_Qty" runat="server" Text="0" CssClass="form-control text-start border-primary packing-readonly" ReadOnly="true"></asp:TextBox></div></div>
                <div class="row mb-3"><div class="col-lg-6 d-flex"><asp:Label ID="Label_Status" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_Status">装箱状态</asp:Label><asp:TextBox ID="txt_Status" runat="server" CssClass="form-control text-start border-primary packing-readonly" ReadOnly="true"></asp:TextBox></div><div class="col-lg-6 d-flex"><asp:Label ID="Label_ExceptionStatus" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_ExceptionStatus">异常状态</asp:Label><asp:TextBox ID="txt_ExceptionStatus" runat="server" CssClass="form-control text-start border-primary packing-readonly" ReadOnly="true"></asp:TextBox></div></div>
                <div class="row mb-3"><div class="col-lg-12 d-flex"><asp:Label ID="Label_LockToken" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_LockToken">当前令牌</asp:Label><asp:TextBox ID="txt_LockToken" runat="server" CssClass="form-control text-start border-primary packing-readonly" ReadOnly="true"></asp:TextBox></div></div>
                <asp:HiddenField ID="hidPackingId" runat="server" /><asp:HiddenField ID="hidLockToken" runat="server" /><asp:HiddenField ID="hidPackingCompleteMode" runat="server" />
            </div></div>
            <div class="container mt-3 border border-primary"><div class="container mt-3"><h5>扫描明细</h5><asp:GridView ID="gvScanRecords" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="false" EmptyDataText="暂无扫描记录。" ShowHeaderWhenEmpty="true" OnRowCommand="gvScanRecords_RowCommand"><Columns><asp:BoundField DataField="QRCodeType" HeaderText="二维码类型" /><asp:BoundField DataField="QRCode" HeaderText="二维码" /><asp:BoundField DataField="MaterialNo" HeaderText="物料号" /><asp:BoundField DataField="Qty" HeaderText="数量" /><asp:BoundField DataField="ScanUser" HeaderText="扫描人" /><asp:BoundField DataField="ScanTime" HeaderText="扫描时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" /><asp:TemplateField HeaderText="操作"><ItemTemplate><asp:LinkButton ID="lnk_Repack" runat="server" CommandName="Repack" CommandArgument='<%# Eval("Id") %>' Text='<%# GetRepackLinkText(Eval("Id")) %>' ToolTip='<%# GetRepackLinkToolTip(Eval("Id")) %>' OnClientClick='<%# GetRepackClientClick(Eval("Id")) %>' Enabled='<%# CanUseRepackLink(Eval("Id")) %>' Visible='<%# IsMaterialScanRecord(Eval("QRCodeType")) %>'></asp:LinkButton></ItemTemplate></asp:TemplateField></Columns></asp:GridView></div></div>
            <asp:HiddenField ID="hidRepackScanId" runat="server" />
            <asp:Panel ID="pnlRepackModal" runat="server" CssClass="packing-repack-mask" Style="display:none" role="dialog" aria-modal="true" aria-labelledby="packingRepackTitle">
                <div class="packing-repack-window">
                    <div class="packing-repack-header"><strong id="packingRepackTitle">重新装箱</strong><button type="button" class="packing-repack-close" onclick="hidePackingRepackModal();">×</button></div>
                    <div class="packing-repack-body"><asp:Label ID="Label_RepackQRCode" runat="server" CssClass="packing-label" AssociatedControlID="txt_RepackQRCode">产品二维码</asp:Label><asp:TextBox ID="txt_RepackQRCode" runat="server" CssClass="form-control border-primary" autocomplete="off" /></div>
                    <div class="packing-repack-footer"><button type="button" class="btn btn-secondary" onclick="hidePackingRepackModal();">取消</button><asp:LinkButton ID="lnkbutton_repack_save" runat="server" CssClass="btn btn-primary" OnClick="lnkbutton_repack_save_Click">保存</asp:LinkButton></div>
                </div>
            </asp:Panel>
            <div class="container mt-3 border border-warning"><table width="100%" align="center" class="tbMessage"><tr valign="middle"><td width="10%" height="28"><div align="center"><b>提示</b></div></td><td class="msg" width="85%">&nbsp;&nbsp;<asp:Label ID="Label_Message" runat="server"></asp:Label></td></tr></table></div>
        </div></div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="JSHolder" runat="server">
    <style>.packing-label{min-width:110px;line-height:30px}.packing-scan{width:100%}.packing-required{background-color:#fff59d!important}.packing-readonly{background-color:#f3f3f3!important}.packing-scan-type-row{align-items:center;gap:20px}.packing-scan-type label{margin-right:24px}.packing-scan-type input[type='radio']:disabled + label,.packing-radio-disabled{display:inline-block;padding:0;border:0;background-color:transparent!important;color:#b3b3b3!important;font-weight:700;opacity:1;cursor:not-allowed}.packing-scan-type input[type='radio']:disabled{filter:grayscale(1);opacity:.65}.packing-radio-enabled{color:black!important}.packing-complete-button{margin:0;padding:6px 16px;border:1px solid #007bff;border-radius:4px;background:#fff;color:#007bff;font-size:16px;line-height:1.5;white-space:nowrap}.packing-complete-button:hover,.packing-complete-button:focus{background:#eaf3ff;color:#0056b3}.packing-complete-button:disabled{border-color:#c8c8c8;background:#f3f3f3;color:#b3b3b3;cursor:not-allowed}.packing-lock-panel{position:fixed;inset:0;z-index:10000;align-items:center;justify-content:center;background:rgba(0,0,0,.45);color:#842029;padding:16px;font-size:18px}.packing-lock-card{min-width:360px;max-width:720px;padding:32px;text-align:center;border:1px solid #dc3545;border-radius:6px;background:#fff3cd;box-shadow:0 8px 24px rgba(0,0,0,.25);line-height:1.8}.packing-repack-mask{position:fixed;inset:0;z-index:11000;align-items:center;justify-content:center;background:rgba(0,0,0,.45);padding:16px}.packing-repack-window{width:460px;max-width:calc(100% - 32px);background:#fff;border:1px solid #b8c8d8;border-radius:6px;box-shadow:0 8px 24px rgba(0,0,0,.25)}.packing-repack-header{display:flex;justify-content:space-between;align-items:center;padding:12px 16px;border-bottom:1px solid #dee2e6;background:#eef5ff}.packing-repack-close{border:0;background:transparent;font-size:24px;cursor:pointer}.packing-repack-body{display:flex;align-items:center;gap:12px;padding:20px 16px}.packing-repack-body .form-control{flex:1}.packing-repack-footer{display:flex;justify-content:flex-end;gap:8px;padding:12px 16px;border-top:1px solid #dee2e6}</style>
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

        function showPackingRepackModal() {
            var modal = document.getElementById('<%=pnlRepackModal.ClientID%>');
            if (modal) { modal.style.display = 'flex'; document.getElementById('<%=txt_RepackQRCode.ClientID%>').focus(); }
        }

        function openApprovedRepackModal(scanRecordId) {
            var target = document.getElementById('<%=hidRepackScanId.ClientID%>');
            var input = document.getElementById('<%=txt_RepackQRCode.ClientID%>');
            if (target) target.value = scanRecordId;
            if (input) input.value = '';
            showPackingRepackModal();
            return false;
        }

        function hidePackingRepackModal() {
            var modal = document.getElementById('<%=pnlRepackModal.ClientID%>');
            if (modal) modal.style.display = 'none';
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

        function packingRepackKeyDown(event) {
            event = event || window.event;
            var keyCode = event.which || event.keyCode;
            if (keyCode === 13) {
                if (event.preventDefault) event.preventDefault();
                __doPostBack('<%=lnkbutton_repack_save.UniqueID%>', '');
                return false;
            }
            return true;
        }

        var packingStatusPollTimer = null;

        function startPackingStatusPolling() {
            if (packingStatusPollTimer !== null) return;
            packingStatusPollTimer = window.setTimeout(pollPackingPageStatus, 1000);
        }

        function schedulePackingStatusPolling() {
            packingStatusPollTimer = window.setTimeout(pollPackingPageStatus, 3000);
        }

        function pollPackingPageStatus() {
            packingStatusPollTimer = null;
            var taskId = new URLSearchParams(window.location.search).get('taskId');
            if (!taskId) return;

            var lockTokenElement = document.getElementById('<%=hidLockToken.ClientID%>');
            var lockToken = lockTokenElement ? lockTokenElement.value : '';
            fetch('Packing.aspx/GetPackingPageStatus', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                body: JSON.stringify({ taskId: taskId, lockToken: lockToken })
            }).then(function (response) {
                return response.json();
            }).then(function (payload) {
                var data = payload.d || payload;
                if (data.Success && data.ShouldReload) {
                    window.location.reload();
                    return;
                }
                schedulePackingStatusPolling();
            }).catch(function () {
                schedulePackingStatusPolling();
            });
        }

        window.setTimeout(focusPackingScan, 0);
    </script>
</asp:Content>
