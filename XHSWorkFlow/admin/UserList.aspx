<%@ Page language="c#" Codebehind="UserList.aspx.cs" AutoEventWireup="false" MasterPageFile="~/DefaultSub.Master" Inherits="ModuleWorkFlow.admin.UserList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
<div id="Wrapper">
<div id="Header"><div class="headbox">
<div class="linebox">
<a href="../defaultmain.aspx">生产管理</a>
<img src="images/arrow.png" />
<a href="#"><%=menuname %></a>
</div>
<div class="logout"><a href="../login.aspx" TARGET="_parent">登出</a>
</div>
<div class="clearbox"></div>
</div>
</div>
<div id="Container">
<div id="Content">
<div id="Menu"><div class="menubox">
<div class="mod1"><ul>
<li class="btn1"><a href="UserView.aspx" runat="server" title="新增/ add">新增/add</a>
</li>
<li class="btn2"><asp:LinkButton ID="lnkbutton_edit" runat="server" OnClick="lnkbutton_edit_Click" ToolTip="编辑/edit">编辑/edit</asp:LinkButton>
</li>
<li class="btn8"><asp:LinkButton ID="lnkbutton_search" runat="server" ToolTip="搜索/search" OnClick="lnkbutton_search_Click">搜索/search</asp:LinkButton>
</li>
</ul>
</div>

<div class="clearbox"></div>
</div>
</div>
 <div class="space1"></div>
 <div class="container mt-3 border border-primary">
   <div class="container mt-3">
  <div class="row mb-3">
 <div class="col-lg-6  d-flex">
<asp:Label runat="server" CssClass="me-10">部门 </asp:Label>

<asp:DropDownList ID="dpl_department" runat="server" AutoPostBack="True" CssClass="form-select custom-heighter-width  text-start border-primary me-1"></asp:DropDownList>
</div>
 <div class="col-lg-6  d-flex">
    <asp:Label runat="server" CssClass="me-10">关键字查询</asp:Label>
<asp:textbox id="TextBox_key" Runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:textbox>

&nbsp;&nbsp;<asp:button id="Button_Search" Runat="server" Text="琩高" visible="false"></asp:button>
<asp:label id="lab_remerber" Runat="server" Visible="False"></asp:label>
</div>
</div>

 <div class="row mb-3">
 <div class="col-lg-6  d-flex">
        <asp:Label runat="server" CssClass="me-10">已离职</asp:Label>
          <asp:CheckBox runat="server" ID="chk_resignation" />
</div>
</div>

</div>
</div>
<div class="container mt-3 border border-primary">
       <div class="container mt-3">
    <asp:datagrid id="MainDataGrid" runat="server" AutoGenerateColumns="False"  AllowPaging="True" CssClass="table table-striped table-bordered table-hover table-sm">
  <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="table-primary" ></HeaderStyle>
  <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true" CssClass="wrap-text" />
<Columns>
<asp:TemplateColumn>
<ItemTemplate>
<asp:CheckBox runat="server" ID="chk_datagrid" />
</ItemTemplate>
</asp:TemplateColumn>
<asp:BoundColumn DataField="UserName" HeaderText="员工编号"></asp:BoundColumn>
<asp:BoundColumn DataField="CardID" HeaderText="员工卡号"></asp:BoundColumn>
<asp:BoundColumn DataField="Name" HeaderText="员工姓名"></asp:BoundColumn>
<asp:BoundColumn DataField="Email" HeaderText="电子邮件"></asp:BoundColumn>
<asp:BoundColumn DataField="Comment" HeaderText="备注"></asp:BoundColumn>
<asp:TemplateColumn HeaderText="功能" visible="false"><ItemTemplate><asp:HyperLink id="HyperLink_Edit" runat="server" Text="编辑" NavigateUrl="&lt;%# &quot;UserEdit.aspx?username=&quot;+DataBinder.Eval(Container, &quot;DataItem.username&quot;) %&gt;"></asp:HyperLink>
</ItemTemplate>
</asp:TemplateColumn>
<asp:ButtonColumn Text="删除" CommandName="Delete" Visible="false"></asp:ButtonColumn>
</Columns>
<PagerStyle Mode="NumericPages" CssClass="table-primary" ></PagerStyle>
</asp:datagrid>
</div>
</div>
<div class="container mt-3 border border-warning"><table width="100%" align="center" class="tbMessage" border="0"><TR vAlign="middle">
<TD width="10%" height="28"><DIV align="center"><B><DIV align="center"><B><asp:Label ID="Label2" runat="server">提示</asp:Label></B>
</DIV>
</B>
</DIV>
</TD>
<TD class="msg" width="85%">
&nbsp;&nbsp;<asp:label id="Label_Message" runat="server" ></asp:label>
<asp:Label id="Label1" runat="server" Visible="False"></asp:Label>
</TD>
</TR>
</table>
</div>
</div>
</div>
</div>
 </asp:Content>
