<%@ Page Language="C#" CodePage="65001" CodeBehind="LabelBindingRecordList.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.LabelBindingRecordList" MasterPageFile="~/DefaultSub.Master" %>

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
                                <li class="btn8"><asp:LinkButton ID="Button_Search" runat="server" OnClick="Button_Search_Click" ToolTip="搜索/search">搜索/search</asp:LinkButton></li>
                            </ul>
                        </div>
                        <div class="clearbox"></div>
                    </div>
                </div>

                <div class="space1"></div>
                <div class="container mt-3 border border-primary">
                    <div class="container mt-3 mb-3">
                        <div class="row mb-3">
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_CustomerProductNo" runat="server" CssClass="me-10 record-search-label" AssociatedControlID="TextBox_CustomerProductNo">工单号</asp:Label>
                                <asp:TextBox ID="TextBox_CustomerProductNo" runat="server" CssClass="form-control custom-heighter-width text-start border-primary" />
                            </div>
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_ProductNo" runat="server" CssClass="me-10 record-search-label" AssociatedControlID="TextBox_ProductNo">本厂品号</asp:Label>
                                <asp:TextBox ID="TextBox_ProductNo" runat="server" CssClass="form-control custom-heighter-width text-start border-primary" />
                            </div>
                            </div>
                             <div class="row mb-3">
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_StartTime" runat="server" CssClass="me-10 record-search-label" AssociatedControlID="TextBox_StartTime">开始时间</asp:Label>
                                <asp:TextBox ID="TextBox_StartTime" runat="server" TextMode="DateTimeLocal" CssClass="form-control custom-heighter-width text-start border-primary" />
                            </div>
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_EndTime" runat="server" CssClass="me-10 record-search-label" AssociatedControlID="TextBox_EndTime">结束时间</asp:Label>
                                <asp:TextBox ID="TextBox_EndTime" runat="server" TextMode="DateTimeLocal" CssClass="form-control custom-heighter-width text-start border-primary" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="container mt-3 border border-primary">
                    <div class="container mt-3">
                        <asp:GridView ID="MainDataGrid" runat="server" PageSize="20" AutoGenerateColumns="False" AllowPaging="True" ShowHeaderWhenEmpty="True" CssClass="table table-striped table-bordered table-hover table-sm" OnPageIndexChanging="MainDataGrid_PageIndexChanging" OnRowDataBound="MainDataGrid_RowDataBound">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="table-primary"></HeaderStyle>
                            <RowStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true" CssClass="wrap-text"></RowStyle>
                            <Columns>
                                <asp:BoundField DataField="WorkOrderNo" HeaderText="工单号"></asp:BoundField>
                                <asp:BoundField DataField="FactoryBarcode" HeaderText="本厂条码"></asp:BoundField>
                                <asp:BoundField DataField="JHSPartNo" HeaderText="本厂品号"></asp:BoundField>
                                <asp:BoundField DataField="JHSBatchNo" HeaderText="批号"></asp:BoundField>
                                <asp:BoundField DataField="LabelQtyText" HeaderText="包装数量"></asp:BoundField>
                                <asp:BoundField DataField="ScanTimeText" HeaderText="扫描时间"></asp:BoundField>
                                <asp:BoundField DataField="ScanUser" HeaderText="操作者"></asp:BoundField>
                            </Columns>
                            <PagerSettings Mode="Numeric" />
                            <PagerStyle CssClass="table-primary"></PagerStyle>
                        </asp:GridView>
                    </div>
                </div>

                <div class="container mt-3 border border-warning">
                    <table width="100%" align="center" class="tbMessage">
                        <tr><td class="msg">&nbsp;&nbsp;<asp:Label ID="Label_Message" runat="server" /></td></tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JSHolder" runat="server">
    <style type="text/css">
        .record-search-label { min-width: 85px; line-height: 30px; }
        .wrap-text { word-break: break-all; }
    </style>
</asp:Content>
