<%@ Page Language="c#" CodeBehind="ShippingGoodsPrintList.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.ShippingGoodsPrintList" MasterPageFile="~/DefaultSub.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
    <div id="Wrapper">
        <div id="Header">
            <div class="headbox">
                <div class="linebox">
                    <a href="Default.aspx">生产管理</a>
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
                        <div class="mod2">
                            <ul>
                                <li class="btn9"><asp:LinkButton ID="lnkbutton_print" runat="server" ToolTip="打印/print" OnClick="lnkbutton_print_Click" OnClientClick="return preparePrintClientId();">打印/print</asp:LinkButton></li>
                            </ul>
                        </div>
                        <div class="clearbox"></div>
                    </div>
                </div>

                <div class="space1"></div>
                <div class="container mt-3 border border-primary">
                    <div class="container mt-3 mb-3">
                        <div class="row mb-3">
                            <div class="col-lg-3 d-flex">
                                <asp:Label runat="server" CssClass="me-10 shipping-goods-search-label" AssociatedControlID="TextBox_PartNo">零件编号</asp:Label>
                                <asp:TextBox ID="TextBox_PartNo" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                            <div class="col-lg-3 d-flex">
                                <asp:Label runat="server" CssClass="me-10 shipping-goods-search-label" AssociatedControlID="TextBox_PartName">零件名称</asp:Label>
                                <asp:TextBox ID="TextBox_PartName" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                            <div class="col-lg-3 d-flex">
                                <asp:Label runat="server" CssClass="me-10 shipping-goods-search-label" AssociatedControlID="TextBox_SupplyBatchNo">批次</asp:Label>
                                <asp:TextBox ID="TextBox_SupplyBatchNo" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                            <div class="col-lg-3 d-flex">
                                <asp:Label runat="server" CssClass="me-10 shipping-goods-search-label" AssociatedControlID="DropDownList_PrinterName">打印机名</asp:Label>
                                <asp:DropDownList ID="DropDownList_PrinterName" runat="server" CssClass="form-select custom-heighter-width text-start border-primary"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="row mb-3">
                            <div class="col-lg-3 d-flex align-items-center">
                                <asp:CheckBox ID="CheckBox_ShowAll" runat="server" AutoPostBack="true" Text="全部列出" OnCheckedChanged="CheckBox_ShowAll_CheckedChanged" />
                                <asp:Button ID="Button_Search" runat="server" Text="搜索" CssClass="btn btn-primary btn-sm ml-10" OnClick="Button_Search_Click" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="container mt-3 border border-primary">
                    <div class="container mt-3">
                        <asp:DataGrid ID="MainDataGrid" runat="server" PageSize="20" AutoGenerateColumns="False" AllowPaging="True" CssClass="table table-striped table-bordered table-hover table-sm" OnPageIndexChanged="MainDataGrid_PageIndexChanged">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="table-primary"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true" CssClass="wrap-text" />
                            <Columns>
                                <asp:TemplateColumn HeaderText="选择">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="checkall" runat="server" onclick="javascript:SelectAll(this,'chk_datagrid');" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" ID="chk_datagrid" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="Id" HeaderText="Id" Visible="False"></asp:BoundColumn>
                                <asp:BoundColumn DataField="SupplierCode" HeaderText="供应商代码"></asp:BoundColumn>
                                <asp:BoundColumn DataField="PartNo" HeaderText="零件编号"></asp:BoundColumn>
                                <asp:BoundColumn DataField="PartChineseName" HeaderText="零件中文名称"></asp:BoundColumn>
                                <asp:BoundColumn DataField="PartEnglishName" HeaderText="零件英文名称"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Quantity" HeaderText="数量"></asp:BoundColumn>
                                <asp:BoundColumn DataField="SupplyBatchNo" HeaderText="供货批次号"></asp:BoundColumn>
                                <asp:BoundColumn DataField="CartonNo" HeaderText="纸箱编号"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Status" HeaderText="状态"></asp:BoundColumn>
                                <asp:BoundColumn DataField="PrintCount" HeaderText="打印次数"></asp:BoundColumn>
                                <asp:BoundColumn DataField="SingleBoxGrossWeight" HeaderText="单箱毛重"></asp:BoundColumn>
                                <asp:BoundColumn DataField="ProductionDate" HeaderText="生产日期" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundColumn>
                                <asp:BoundColumn DataField="InspectionConfirmDate" HeaderText="检验确认日期" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundColumn>
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
                <asp:HiddenField ID="hid_ClientId" runat="server" />
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
    </style>
    <script type="text/javascript">
        function generateClientId() {
            return 'xxxx-xxxx-4xxx-yxxx'.replace(/[xy]/g, function (c) {
                let r = Math.random() * 16 | 0;
                let v = c === 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        }

        function preparePrintClientId() {
            var hiddenField = document.getElementById('<%=hid_ClientId.ClientID%>');
            if (hiddenField) {
                hiddenField.value = generateClientId();
            }

            return true;
        }

        function SelectAll(control, itemIdSuffix) {
            var form = document.forms[0];
            if (!form) {
                return;
            }

            for (var i = 0; i < form.elements.length; i++) {
                var element = form.elements[i];
                if (element && element.type === 'checkbox' && element.id && element.id.indexOf(itemIdSuffix) >= 0) {
                    element.checked = control.checked;
                }
            }
        }
    </script>
</asp:Content>
