<%@ Page language="c#" Codebehind="UserPasswordChange.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.admin.UserPasswordChange" MasterPageFile="~/DefaultSub.Master" %>
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
<li class="btn3"><asp:LinkButton ID="lnkbutton_edit" runat="server" ToolTip="保存/save" OnClick="lnkbutton_save_Click">保存/save</asp:LinkButton>
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
      <div class="col-lg-6 d-flex">
        <asp:Label ID="label1" runat="server" CssClass="me-10">员工编号</asp:Label>
        <asp:Label Runat="server" ID="lab_UserName" CssClass="me-10" />
      </div>
      <div class="col-lg-6 d-flex">
        <asp:Label ID="label2" runat="server" CssClass="me-10">员工姓名</asp:Label>
        <asp:Label Runat="server" ID="lab_Name" CssClass="me-10" />
      </div>
    </div>
    <div class="row mb-3">
      <div class="col-lg-6 d-flex">
        <asp:Label ID="label3" runat="server" CssClass="me-10">电子邮件</asp:Label>
        <asp:TextBox Runat="server" ID="txt_Email" CssClass="form-control custom-heighter-width text-start border-primary" />
      </div>
      <div class="col-lg-6 d-flex">
        <asp:Label ID="label4" runat="server" CssClass="me-10">密码</asp:Label>
        <asp:TextBox Runat="server" ID="txt_password" TextMode="Password" CssClass="form-control custom-heighter-width text-start border-primary" />
      </div>
    </div>
  </div>
</div>
<div class="container mt-3 border border-warning"><table width="100%" align="center" class="tbMessage"><TR vAlign="middle">
<TD width="10%" height="28"><DIV align="center"><B><DIV align="center"><B>提示</B>
</DIV>
</B>
</DIV>
</TD>
<TD class="msg" width="85%">
&nbsp;&nbsp;<asp:label id="Label_Message" runat="server" Font-Bold="True" ForeColor="RED"></asp:label>
</TD>
</TR>
</table>
</div>
</div>
</div>
</div>
</asp:Content>
