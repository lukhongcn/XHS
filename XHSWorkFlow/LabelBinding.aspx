<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Page Language="c#" CodeBehind="LabelBinding.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.LabelBinding" MasterPageFile="~/DefaultSub.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true"></asp:ScriptManager>

    <div id="Wrapper" class="label-binding-page">
        <div id="Header">
            <div class="headbox">
                <div class="linebox">
                    <a href="#">标签管理</a>
                    <img src="images/arrow.png" />
                    <a href="#"><%=menuname %></a>
                </div>
                <div class="logout"><a href="login.aspx" target="_parent">登出</a></div>
                <div class="clearbox"></div>
            </div>
        </div>

        <div id="Container">
            <div id="Content">
                <div id="Menu">
                    <div class="menubox">
                        <div class="mod1">
                            <ul>
                                <li class="btn5">
                                    <asp:LinkButton ID="lnk_view" runat="server" OnClick="lnk_view_Click" ToolTip="浏览">浏览</asp:LinkButton>
                                </li>
                            </ul>
                        </div>
                        <div class="mod2">
                            <ul>
                                <li class="btn3">
                                    <asp:LinkButton ID="lnkbutton_save" runat="server" ToolTip="保存绑定" OnClick="lnkbutton_save_Click">保存绑定</asp:LinkButton>
                                </li>
                                <li class="btn4">
                                    <asp:LinkButton ID="lnkbutton_clear" runat="server" ToolTip="清空当前绑定" OnClick="lnkbutton_clear_Click">清空</asp:LinkButton>
                                </li>
                            </ul>
                        </div>
                        <div class="clearbox"></div>
                    </div>
                </div>

                <div class="space1"></div>

                <asp:Panel ID="pnlCompleted" runat="server" CssClass="binding-completed-panel" Visible="false">
                    <strong>客户标签数量已绑定完成</strong><br />
                    当前客户标签剩余数量为 0，请保存后扫描下一张客户标签。
                </asp:Panel>

                <!-- 客户 QRCode -->
                <div class="container mt-3 border border-primary binding-section">
                    <div class="container mt-3 mb-3">
                        <div class="binding-title">
                            <span class="binding-step">1</span>
                            扫描客户 QRCode
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-12 d-flex">
                                <asp:Label ID="Label_CustomerQRCode" runat="server" CssClass="binding-label" AssociatedControlID="txt_CustomerQRCode">客户 QRCode</asp:Label>
                                <asp:TextBox ID="txt_CustomerQRCode" runat="server"
                                    CssClass="form-control text-left border-primary binding-scan binding-required"
                                    AutoPostBack="true"
                                    OnTextChanged="txt_CustomerQRCode_TextChanged"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_CustomerLabelNo" runat="server" CssClass="binding-label" AssociatedControlID="txt_CustomerLabelNo">客户标签号</asp:Label>
                                <asp:TextBox ID="txt_CustomerLabelNo" runat="server" CssClass="form-control text-left border-primary binding-readonly" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_SourceCode" runat="server" CssClass="binding-label" AssociatedControlID="txt_SourceCode">来源代码</asp:Label>
                                <asp:TextBox ID="txt_SourceCode" runat="server" CssClass="form-control text-left border-primary binding-readonly" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_CustomerMaterialNo" runat="server" CssClass="binding-label" AssociatedControlID="txt_CustomerMaterialNo">客户物料号</asp:Label>
                                <asp:TextBox ID="txt_CustomerMaterialNo" runat="server" CssClass="form-control text-left border-primary binding-readonly binding-emphasis" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_ExpectedPartNo" runat="server" CssClass="binding-label" AssociatedControlID="txt_ExpectedPartNo">本厂品号</asp:Label>
                                <asp:TextBox ID="txt_ExpectedPartNo" runat="server" CssClass="form-control text-left border-primary binding-readonly binding-emphasis" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_PartName" runat="server" CssClass="binding-label" AssociatedControlID="txt_PartName">零件名称</asp:Label>
                                <asp:TextBox ID="txt_PartName" runat="server" CssClass="form-control text-left border-primary binding-readonly" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_CustomerBatchNo" runat="server" CssClass="binding-label" AssociatedControlID="txt_CustomerBatchNo">客户批次</asp:Label>
                                <asp:TextBox ID="txt_CustomerBatchNo" runat="server" CssClass="form-control text-left border-primary binding-readonly" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-4 d-flex">
                                <asp:Label ID="Label_CustomerQty" runat="server" CssClass="binding-label-small" AssociatedControlID="txt_CustomerQty">客户数量</asp:Label>
                                <asp:TextBox ID="txt_CustomerQty" runat="server" CssClass="form-control text-center border-primary binding-readonly binding-qty-total" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="col-lg-4 d-flex">
                                <asp:Label ID="Label_BoundQty" runat="server" CssClass="binding-label-small" AssociatedControlID="txt_BoundQty">已绑定</asp:Label>
                                <asp:TextBox ID="txt_BoundQty" runat="server" CssClass="form-control text-center border-primary binding-readonly binding-qty-bound" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="col-lg-4 d-flex">
                                <asp:Label ID="Label_RemainingQty" runat="server" CssClass="binding-label-small" AssociatedControlID="txt_RemainingQty">剩余数量</asp:Label>
                                <asp:TextBox ID="txt_RemainingQty" runat="server" CssClass="form-control text-center border-primary binding-readonly binding-qty-remaining" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-12 d-flex">
                                <asp:Label ID="Label_Unit" runat="server" CssClass="binding-label" AssociatedControlID="txt_Unit">单位</asp:Label>
                                <asp:TextBox ID="txt_Unit" runat="server" CssClass="form-control text-left border-primary binding-readonly binding-unit" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="binding-progress-wrap">
                            <div class="binding-progress-text">
                                绑定进度：<asp:Label ID="Label_BindingProgressText" runat="server" Text="0 / 0"></asp:Label>
                            </div>
                            <div class="progress binding-progress">
                                <div id="bindingProgressBar" runat="server" class="progress-bar" role="progressbar" style="width:0%">0%</div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- 本厂标签 -->
                <div class="container mt-3 border border-success binding-section">
                    <div class="container mt-3 mb-3">
                        <div class="binding-title">
                            <span class="binding-step binding-step-green">2</span>
                            扫描本厂条码
                        </div>

                        <div class="row mb-2">
                            <div class="col-lg-12 d-flex">
                                <asp:Label ID="Label_FactoryBarcode" runat="server" CssClass="binding-label" AssociatedControlID="txt_FactoryBarcode">本厂条码</asp:Label>
                                <asp:TextBox ID="txt_FactoryBarcode" runat="server"
                                    CssClass="form-control text-left border-success binding-scan binding-required"
                                    AutoPostBack="true"
                                    OnTextChanged="txt_FactoryBarcode_TextChanged"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-12">
                                <div class="binding-scan-hint">
                                    系统会自动识别：<strong>上方工单码</strong> 或 <strong>下方包装码</strong>。实际绑定优先使用下方包装码。
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_FactoryCodeType" runat="server" CssClass="binding-label" AssociatedControlID="txt_FactoryCodeType">识别类型</asp:Label>
                                <asp:TextBox ID="txt_FactoryCodeType" runat="server" CssClass="form-control text-left border-success binding-readonly binding-code-type" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_WorkOrderNo" runat="server" CssClass="binding-label" AssociatedControlID="txt_WorkOrderNo">本厂工单号</asp:Label>
                                <asp:TextBox ID="txt_WorkOrderNo" runat="server" CssClass="form-control text-left border-success binding-readonly" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_FactoryPartNo" runat="server" CssClass="binding-label" AssociatedControlID="txt_FactoryPartNo">本厂品号</asp:Label>
                                <asp:TextBox ID="txt_FactoryPartNo" runat="server" CssClass="form-control text-left border-success binding-readonly binding-emphasis" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_FactoryPartName" runat="server" CssClass="binding-label" AssociatedControlID="txt_FactoryPartName">零件名称</asp:Label>
                                <asp:TextBox ID="txt_FactoryPartName" runat="server" CssClass="form-control text-left border-success binding-readonly" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_FactoryBatchNo" runat="server" CssClass="binding-label" AssociatedControlID="txt_FactoryBatchNo">本厂批次</asp:Label>
                                <asp:TextBox ID="txt_FactoryBatchNo" runat="server" CssClass="form-control text-left border-success binding-readonly" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_FactoryLabelQty" runat="server" CssClass="binding-label" AssociatedControlID="txt_FactoryLabelQty">标签数量</asp:Label>
                                <asp:TextBox ID="txt_FactoryLabelQty" runat="server" CssClass="form-control text-center border-success binding-readonly binding-qty-label" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3 align-items-center">
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_BindQty" runat="server" CssClass="binding-label" AssociatedControlID="txt_BindQty">本次绑定数量</asp:Label>
                                <asp:TextBox ID="txt_BindQty" runat="server"
                                    CssClass="form-control text-center border-success binding-required binding-bind-qty"
                                    AutoPostBack="true"
                                    OnTextChanged="txt_BindQty_TextChanged"
                                    oninput="validateBindQtyClient();"></asp:TextBox>
                            </div>
                            <div class="col-lg-3">
                                <asp:Button ID="btnUseRemainingQty" runat="server" Text="使用剩余数量" CssClass="btn btn-outline-warning w-100" OnClick="btnUseRemainingQty_Click" />
                            </div>
                            <div class="col-lg-3">
                                <asp:Button ID="btnConfirmBind" runat="server" Text="确认加入绑定" CssClass="btn btn-success w-100" OnClick="btnConfirmBind_Click" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-12 d-flex">
                                <asp:Label ID="Label_ValidationStatus" runat="server" CssClass="binding-label" AssociatedControlID="txt_ValidationStatus">校验结果</asp:Label>
                                <asp:TextBox ID="txt_ValidationStatus" runat="server" CssClass="form-control text-left border-success binding-readonly" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- 已绑定明细 -->
                <div class="container mt-3 border border-primary binding-section">
                    <div class="container mt-3 mb-3">
                        <div class="binding-title">
                            <span class="binding-step">3</span>
                            已绑定本厂标签
                        </div>

                        <asp:GridView ID="gvBindingRecords" runat="server"
                            CssClass="table table-bordered table-striped binding-grid"
                            AutoGenerateColumns="false"
                            EmptyDataText="当前客户标签尚未绑定本厂标签。"
                            DataKeyNames="BindingDetailId"
                            OnRowCommand="gvBindingRecords_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="SeqNo" HeaderText="序号" />
                                <asp:BoundField DataField="FactoryCodeTypeName" HeaderText="条码类型" />
                                <asp:BoundField DataField="FactoryBarcode" HeaderText="本厂原始码" />
                                <asp:BoundField DataField="WorkOrderNo" HeaderText="工单号" />
                                <asp:BoundField DataField="FactoryPartNo" HeaderText="本厂品号" />
                                <asp:BoundField DataField="BatchNo" HeaderText="批次" />
                                <asp:BoundField DataField="LabelQty" HeaderText="标签数量" DataFormatString="{0:0.####}" />
                                <asp:BoundField DataField="BindQty" HeaderText="绑定数量" DataFormatString="{0:0.####}" />
                                <asp:BoundField DataField="ScanTime" HeaderText="扫描时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                                <asp:TemplateField HeaderText="操作">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkDelete" runat="server"
                                            CommandName="DeleteBinding"
                                            CommandArgument='<%# Eval("BindingDetailId") %>'
                                            CssClass="binding-delete-link"
                                            OnClientClick="return confirm('确定删除这条绑定明细吗？');">删除</asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>

                        <div class="binding-summary">
                            <span>客户数量：<strong><asp:Label ID="Label_SummaryCustomerQty" runat="server" Text="0"></asp:Label></strong></span>
                            <span>已绑定：<strong><asp:Label ID="Label_SummaryBoundQty" runat="server" Text="0"></asp:Label></strong></span>
                            <span>剩余数量：<strong><asp:Label ID="Label_SummaryRemainingQty" runat="server" Text="0"></asp:Label></strong></span>
                        </div>
                    </div>
                </div>

                <div class="container mt-3 border border-warning">
                    <table width="100%" align="center" class="tbMessage">
                        <tr valign="middle">
                            <td width="10%" height="28"><div align="center"><b>提示</b></div></td>
                            <td class="msg" width="85%">&nbsp;&nbsp;<asp:Label ID="Label_Message" runat="server"></asp:Label></td>
                        </tr>
                    </table>
                </div>

                <asp:HiddenField ID="hidCustomerRuleId" runat="server" />
                <asp:HiddenField ID="hidFactoryRuleId" runat="server" />
                <asp:HiddenField ID="hidCustomerQty" runat="server" Value="0" />
                <asp:HiddenField ID="hidBoundQty" runat="server" Value="0" />
                <asp:HiddenField ID="hidRemainingQty" runat="server" Value="0" />
                <asp:HiddenField ID="hidFactoryLabelQty" runat="server" Value="0" />
                <asp:HiddenField ID="hidFactoryAvailableQty" runat="server" Value="0" />
                <asp:HiddenField ID="hidCustomerLabelRaw" runat="server" />
                <asp:HiddenField ID="hidFactoryLabelRaw" runat="server" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JSHolder" runat="server">
    <style type="text/css">
        .binding-section { background: #fff; }
        .binding-title { font-size: 18px; font-weight: bold; margin-bottom: 16px; color: #333; }
        .binding-step { display: inline-block; width: 28px; height: 28px; line-height: 28px; border-radius: 50%; background: #0d6efd; color: #fff; text-align: center; margin-right: 8px; }
        .binding-step-green { background: #198754; }
        .binding-label { min-width: 120px; line-height: 32px; margin-right: 10px; }
        .binding-label-small { min-width: 88px; line-height: 32px; margin-right: 10px; }
        .binding-scan { width: 100%; font-size: 17px; font-weight: bold; }
        .binding-required { background-color: #fff59d !important; }
        .binding-readonly { background-color: #f3f3f3 !important; }
        .binding-emphasis { font-weight: bold; color: #0d47a1; }
        .binding-code-type { font-weight: bold; color: #198754; }
        .binding-qty-total { font-size: 20px; font-weight: bold; color: #0d47a1; }
        .binding-qty-bound { font-size: 20px; font-weight: bold; color: #198754; }
        .binding-qty-remaining { font-size: 20px; font-weight: bold; color: #dc3545; }
        .binding-qty-label { font-size: 18px; font-weight: bold; }
        .binding-bind-qty { font-size: 20px; font-weight: bold; }
        .binding-unit { max-width: 160px; }
        .binding-scan-hint { padding: 8px 12px; background: #eef7ff; border-left: 4px solid #0d6efd; color: #444; }
        .binding-completed-panel { margin: 15px; border: 1px solid #198754; background: #d1e7dd; color: #0f5132; padding: 16px; font-size: 16px; }
        .binding-progress-wrap { margin-top: 4px; }
        .binding-progress-text { margin-bottom: 6px; font-weight: bold; }
        .binding-progress { height: 24px; }
        .binding-progress .progress-bar { font-weight: bold; }
        .binding-grid th { white-space: nowrap; text-align: center; }
        .binding-grid td { vertical-align: middle; }
        .binding-delete-link { color: #dc3545; font-weight: bold; }
        .binding-summary { margin-top: 12px; padding: 12px; background: #eef7ff; border: 1px solid #b8daff; font-size: 17px; text-align: right; }
        .binding-summary span { display: inline-block; margin-left: 28px; }
        .binding-summary strong { font-size: 20px; }
        .binding-invalid { background-color: #f8d7da !important; border-color: #dc3545 !important; }
        .binding-valid { background-color: #d1e7dd !important; border-color: #198754 !important; }
        .label-binding-page .binding-grid { display: block; overflow-x: auto; }
        .label-binding-page .d-flex { min-width: 0; }
        .label-binding-page .d-flex .form-control { min-width: 0; }
    </style>

    <script type="text/javascript">
        function getNumberValue(id) {
            var input = document.getElementById(id);
            if (!input) { return 0; }
            var value = parseFloat(input.value);
            return isNaN(value) ? 0 : value;
        }

        function focusCustomerScan() {
            var customerCode = document.getElementById('<%=txt_CustomerQRCode.ClientID%>');
            var factoryCode = document.getElementById('<%=txt_FactoryBarcode.ClientID%>');
            var expectedPartNo = document.getElementById('<%=txt_ExpectedPartNo.ClientID%>');

            if (expectedPartNo && expectedPartNo.value && factoryCode && !factoryCode.disabled) {
                factoryCode.focus();
                factoryCode.select();
                return;
            }

            if (customerCode && !customerCode.disabled) {
                customerCode.focus();
                customerCode.select();
            }
        }

        function validateBindQtyClient() {
            var bindQtyInput = document.getElementById('<%=txt_BindQty.ClientID%>');
            if (!bindQtyInput) { return true; }

            var bindQty = parseFloat(bindQtyInput.value);
            var remainingQty = getNumberValue('<%=hidRemainingQty.ClientID%>');
            var factoryAvailableQty = getNumberValue('<%=hidFactoryAvailableQty.ClientID%>');
            var valid = !isNaN(bindQty)
                && bindQty > 0
                && bindQty <= remainingQty
                && bindQty <= factoryAvailableQty;

            bindQtyInput.className = bindQtyInput.className
                .replace(/\bbinding-invalid\b/g, '')
                .replace(/\bbinding-valid\b/g, '');
            bindQtyInput.className += valid ? ' binding-valid' : ' binding-invalid';
            return valid;
        }

        window.setTimeout(focusCustomerScan, 0);
    </script>
</asp:Content>
