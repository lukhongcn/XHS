<%@ Page Language="c#" CodePage="65001" CodeBehind="PartProcessList.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.PartProcessList" MasterPageFile="~/DefaultSub.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
    <div id="Wrapper">
        <div id="Header">
            <div class="headbox">
                <div class="linebox">
                    <a href="Default.aspx">生产管理</a>
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
                        <div class="mod2">
                            <ul>
                                <li class="btn8">
                                    <asp:LinkButton ID="lnkbutton_search" runat="server" OnClick="lnkbutton_search_Click" ToolTip="搜索/search">搜索/search</asp:LinkButton>
                                </li>
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
                                <asp:Label ID="Label_PartNo" runat="server" AssociatedControlID="DropDownList_PartNo" CssClass="part-process-search-label">零件编号</asp:Label>
                                <asp:DropDownList ID="DropDownList_PartNo" runat="server" CssClass="form-control custom-heighter-width text-start border-primary" />
                            </div>
                            <div class="col-lg-3 d-flex">
                                <asp:Label ID="Label_StartDate" runat="server" AssociatedControlID="TextBox_StartDate" CssClass="part-process-search-label">生产日期起</asp:Label>
                                <asp:TextBox ID="TextBox_StartDate" runat="server" TextMode="Date" CssClass="form-control custom-heighter-width text-start border-primary" />
                            </div>
                            <div class="col-lg-3 d-flex">
                                <asp:Label ID="Label_EndDate" runat="server" AssociatedControlID="TextBox_EndDate" CssClass="part-process-search-label">生产日期止</asp:Label>
                                <asp:TextBox ID="TextBox_EndDate" runat="server" TextMode="Date" CssClass="form-control custom-heighter-width text-start border-primary" />
                            </div>
                        </div>
                        <div class="row mb-3">
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_Barcode" runat="server" AssociatedControlID="TextBox_Barcode" CssClass="part-process-search-label">二维码</asp:Label>
                                <asp:TextBox ID="TextBox_Barcode" runat="server" CssClass="form-control text-start border-primary" Style="flex: 1;" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="container mt-3 border border-primary">
                    <div class="container mt-3">
                        <asp:DataGrid ID="MainDataGrid" runat="server" PageSize="20" AutoGenerateColumns="False" AllowPaging="True" CssClass="table table-striped table-bordered table-hover table-sm" OnPageIndexChanged="MainDataGrid_PageIndexChanged" OnItemDataBound="MainDataGrid_ItemDataBound">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="table-primary" />
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true" CssClass="wrap-text" />
                            <Columns>
                                <asp:BoundColumn DataField="ModuleId" HeaderText="客户编号" />
                                <asp:BoundColumn DataField="PartNo" HeaderText="零件编号" />
                                <asp:BoundColumn DataField="PartNo_Id" HeaderText="二维码" />
                                <asp:TemplateColumn HeaderText="打印日期">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_PrintDate" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="ProcessName" HeaderText="工序名称" />
                                <asp:BoundColumn DataField="ProcessOrder" HeaderText="工序顺序" />
                                <asp:BoundColumn DataField="StatusName" HeaderText="状态" />
                            </Columns>
                            <PagerStyle Mode="NumericPages" CssClass="table-primary" />
                        </asp:DataGrid>
                    </div>
                </div>

                <div class="container mt-3 border border-warning">
                    <table width="100%" align="center" class="tbMessage">
                        <tr valign="middle">
                            <td width="10%" height="28"><div align="center"><b>提示</b></div></td>
                            <td class="msg" width="85%">&nbsp;&nbsp;<asp:Label ID="Label_Message" runat="server" /></td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JSHolder" runat="server">
    <style type="text/css">
        .part-process-search-label { min-width: 78px; line-height: 30px; }
    </style>
</asp:Content>
