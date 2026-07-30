<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Page Language="c#" CodeBehind="ShippingGoodsClose.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.ShippingGoodsClose" MasterPageFile="~/DefaultSub.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    <div id="Wrapper">
        <div id="Header">
            <div class="headbox">
                <div class="linebox">
                    <a href="#">生产管理</a>
                    <img src="images/arrow.png" />
                    <a href="#"><%=menuname %></a>
                </div>
                <div class="logout">
                    <a href="login.aspx" target="_parent">登出</a>
                </div>
                <div class="clearbox"></div>
            </div>
        </div>

        <div id="Container">
            <div id="Content">
                <div id="Menu">
                    <div class="menubox">
                        <div class="mod1">
                            <ul>
                                <li class="btn5"><asp:LinkButton ID="lnk_view" runat="server" OnClick="lnk_view_Click" ToolTip="浏览">浏览</asp:LinkButton></li>
                            </ul>
                        </div>
                        <div class="mod2">
                            <ul>
                                <li class="btn3"><asp:LinkButton ID="lnkbutton_save" runat="server" ToolTip="保存/save" OnClick="lnkbutton_save_Click">保存/save</asp:LinkButton></li>
                            </ul>
                        </div>
                        <div class="clearbox"></div>
                    </div>
                </div>

                <div class="space1"></div>
                <div class="container mt-3 border border-primary">
                    <div class="container mt-3 mb-3">
                        <div class="row mb-3">
                            <div class="col-lg-6 d-flex shipping-goods-close-input-row">
                                <asp:Label ID="Label_SupplyBatchNo" runat="server" CssClass="me-10 shipping-goods-search-label" AssociatedControlID="TextBox_SupplyBatchNo">批次</asp:Label>
                                <asp:TextBox ID="TextBox_SupplyBatchNo" runat="server" CssClass="form-control shipping-goods-close-input text-start border-primary shipping-goods-scan" AutoPostBack="true" OnTextChanged="TextBox_SupplyBatchNo_TextChanged"></asp:TextBox>
                                <ajaxToolkit:AutoCompleteExtender
                                    ID="AutoCompleteExtender_SupplyBatchNo"
                                    runat="server"
                                    TargetControlID="TextBox_SupplyBatchNo"
                                    ServiceMethod="GetSupplyBatchNoSuggestions"
                                    MinimumPrefixLength="1"
                                    CompletionInterval="100"
                                    EnableCaching="false"
                                    CompletionSetCount="12" />
                            </div>
                            <div class="col-lg-6 d-flex shipping-goods-close-input-row">
                                <asp:Label ID="Label_PartNo" runat="server" CssClass="me-10 shipping-goods-search-label" AssociatedControlID="TextBox_PartNo">零件编号</asp:Label>
                                <asp:TextBox ID="TextBox_PartNo" runat="server" CssClass="form-control shipping-goods-close-input text-start border-primary shipping-goods-scan" AutoPostBack="true" OnTextChanged="TextBox_PartNo_TextChanged"></asp:TextBox>
                                <ajaxToolkit:AutoCompleteExtender
                                    ID="AutoCompleteExtender_PartNo"
                                    runat="server"
                                    TargetControlID="TextBox_PartNo"
                                    ServiceMethod="GetPartNoSuggestions"
                                    MinimumPrefixLength="1"
                                    CompletionInterval="100"
                                    EnableCaching="false"
                                    CompletionSetCount="12"
                                    UseContextKey="true"
                                    OnClientPopulating="shippingGoodsClosePreparePartNoContext" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="container mt-3 border border-primary">
                    <div class="container mt-3">
                        <asp:DataGrid ID="MainDataGrid" runat="server" PageSize="20" AutoGenerateColumns="False" AllowPaging="False" CssClass="table table-striped table-bordered table-hover table-sm">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="table-primary"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true" CssClass="wrap-text" />
                            <Columns>
                                <asp:BoundColumn DataField="Id" HeaderText="Id" Visible="False"></asp:BoundColumn>
                                <asp:BoundColumn DataField="SupplierCode" HeaderText="供应商代码"></asp:BoundColumn>
                                <asp:BoundColumn DataField="PartNo" HeaderText="零件编号"></asp:BoundColumn>
                                <asp:BoundColumn DataField="PartChineseName" HeaderText="零件中文名称"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Quantity" HeaderText="数量"></asp:BoundColumn>
                                <asp:BoundColumn DataField="SupplyBatchNo" HeaderText="供货批次号"></asp:BoundColumn>
                                <asp:BoundColumn DataField="CartonNo" HeaderText="纸箱编号"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Status" HeaderText="状态"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Closer" HeaderText="结案人"></asp:BoundColumn>
                                <asp:BoundColumn DataField="CloseDate" HeaderText="结案时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Creater" HeaderText="创建人"></asp:BoundColumn>
                                <asp:BoundColumn DataField="CreatDate" HeaderText="创建时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}"></asp:BoundColumn>
                            </Columns>
                            <PagerStyle Mode="NumericPages" CssClass="table-primary"></PagerStyle>
                        </asp:DataGrid>
                    </div>
                </div>

                <div class="container mt-3 border border-warning">
                    <table width="100%" align="center" class="tbMessage">
                        <tr valign="middle">
                            <td width="10%" height="28">
                                <div align="center"><b><asp:Label ID="Label2" runat="server">提示</asp:Label></b></div>
                            </td>
                            <td class="msg" width="85%">&nbsp;&nbsp;<asp:Label ID="Label_Message" runat="server"></asp:Label></td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JSHolder" runat="server">
    <style>
        .shipping-goods-search-label {
            min-width: 70px;
            line-height: 30px;
        }

        .shipping-goods-scan {
            background-color: #f8fbff;
        }

        .shipping-goods-close-input-row {
            align-items: center;
        }

        .shipping-goods-close-input {
            width: min(100%, 260px);
            height: 30px;
            padding: 6px 12px;
            font-size: 14px;
        }
    </style>
    <script type="text/javascript">
        function shippingGoodsCloseKeyDown(event, nextClientId) {
            event = event || window.event;
            var keyCode = event.keyCode || event.which;
            if (keyCode !== 13) {
                return true;
            }

            if (event.preventDefault) {
                event.preventDefault();
            } else {
                event.returnValue = false;
            }

            var source = event.target || event.srcElement;
            if (source && source.blur) {
                source.blur();
            }

            if (nextClientId) {
                window.setTimeout(function () {
                    var next = document.getElementById(nextClientId);
                    if (next && next.focus) {
                        next.focus();
                    }
                }, 0);
            }

            return false;
        }

        function focusShippingGoodsCloseBatch() {
            var batchTextBox = document.getElementById('<%=TextBox_SupplyBatchNo.ClientID%>');
            if (batchTextBox && batchTextBox.focus) {
                batchTextBox.focus();
            }
        }

        function shippingGoodsClosePreparePartNoContext(sender, args) {
            var batchTextBox = document.getElementById('<%=TextBox_SupplyBatchNo.ClientID%>');
            var supplyBatchNo = batchTextBox ? batchTextBox.value : '';
            sender.set_contextKey(supplyBatchNo || '');
        }

        function initializeShippingGoodsCloseSensors() {
            var batchTextBox = document.getElementById('<%=TextBox_SupplyBatchNo.ClientID%>');
            var partTextBox = document.getElementById('<%=TextBox_PartNo.ClientID%>');
            if (!batchTextBox || !partTextBox) {
                return;
            }

            batchTextBox.onkeydown = function (event) {
                return shippingGoodsCloseKeyDown(event, '<%=TextBox_PartNo.ClientID%>');
            };
            partTextBox.onkeydown = function (event) {
                return shippingGoodsCloseKeyDown(event, '');
            };
        }

        $(function () {
            initializeShippingGoodsCloseSensors();
            window.setTimeout(focusShippingGoodsCloseBatch, 0);
        });
    </script>
</asp:Content>
