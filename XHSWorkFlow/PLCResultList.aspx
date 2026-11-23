<%@ Page Language="C#" CodePage="65001" CodeBehind="PLCResultList.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.PLCResultList" MasterPageFile="~/DefaultSub.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
    <div id="Wrapper">
        <div id="Header"><div class="headbox"><div class="linebox"><a href="Default.aspx">生产管理</a><img src="images/arrow.png" /><a href="#"><%=menuname %></a></div><div class="logout"><a href="login.aspx" target="_parent">登出</a></div><div class="clearbox"></div></div></div>
        <div id="Container"><div id="Content">
            <div id="Menu"><div class="menubox"><div class="mod2"><ul><li class="btn8"><asp:LinkButton ID="Button_Search" runat="server" OnClick="Button_Search_Click">搜索/search</asp:LinkButton></li></ul></div><div class="clearbox"></div></div></div>
            <div class="space1"></div>
            <div class="container mt-3 border border-primary"><div class="container mt-3 mb-3">
                <div class="row mb-3">
                    <div class="col-lg-6 d-flex"><asp:Label ID="Label_StartDate" runat="server" CssClass="me-10 record-search-label" AssociatedControlID="TextBox_StartDate">检测日期起</asp:Label><asp:TextBox ID="TextBox_StartDate" runat="server" TextMode="Date" CssClass="form-control custom-heighter-width text-start border-primary" /></div>
                    <div class="col-lg-6 d-flex"><asp:Label ID="Label_EndDate" runat="server" CssClass="me-10 record-search-label" AssociatedControlID="TextBox_EndDate">检测日期止</asp:Label><asp:TextBox ID="TextBox_EndDate" runat="server" TextMode="Date" CssClass="form-control custom-heighter-width text-start border-primary" /></div>
                </div>
                <div class="row mb-3">
                    <div class="col-lg-6 d-flex"><asp:Label ID="Label_PartNo" runat="server" CssClass="me-10 record-search-label" AssociatedControlID="TextBox_PartNo">零件编号</asp:Label><asp:TextBox ID="TextBox_PartNo" runat="server" CssClass="form-control custom-heighter-width text-start border-primary" /></div>
                    <div class="col-lg-6 d-flex"><asp:Label ID="Label_QRCode" runat="server" CssClass="me-10 record-search-label" AssociatedControlID="TextBox_QRCode">二维码</asp:Label><asp:TextBox ID="TextBox_QRCode" runat="server" CssClass="form-control custom-heighter-width text-start border-primary" /></div>
                </div>
            </div></div>
            <div class="container mt-3 border border-primary"><div class="container mt-3"><asp:GridView ID="MainDataGrid" runat="server" PageSize="20" AutoGenerateColumns="False" AllowPaging="True" ShowHeaderWhenEmpty="True" CssClass="table table-striped table-bordered table-hover table-sm" OnPageIndexChanging="MainDataGrid_PageIndexChanging"><HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="table-primary" /><RowStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true" CssClass="wrap-text" /><Columns><asp:BoundField DataField="PartNo" HeaderText="零件编号" /><asp:BoundField DataField="QRCode" HeaderText="二维码" /><asp:BoundField DataField="CreateDate" HeaderText="检测日期" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" /><asp:BoundField DataField="ResultValue" HeaderText="检测值" /><asp:BoundField DataField="Result" HeaderText="检测结果" /><asp:BoundField DataField="ProcessNo" HeaderText="工序号" /></Columns><PagerSettings Mode="Numeric" /><PagerStyle CssClass="table-primary" /></asp:GridView></div></div>
            <div class="container mt-3 border border-warning"><table width="100%" align="center" class="tbMessage"><tr><td class="msg">&nbsp;&nbsp;<asp:Label ID="Label_Message" runat="server" /></td></tr></table></div>
        </div></div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="JSHolder" runat="server"><style type="text/css">.record-search-label{min-width:85px;line-height:30px}.wrap-text{word-break:break-all}</style></asp:Content>
