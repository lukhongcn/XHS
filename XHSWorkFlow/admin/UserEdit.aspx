
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Page language="c#" Codebehind="UserEdit.aspx.cs" AutoEventWireup="false" MasterPageFile="~/DefaultSub.Master" Inherits="ModuleWorkFlow.admin.UserEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true"></asp:ScriptManager>
    <div id="Wrapper">
<div id="Header"><div class="headbox"> 
<div class="linebox">
<a href="#">生产管理</a>
<img src="images/arrow.png" />
<a href="#"><%=menuname %></a>
</div>
<div class="logout"><a href="../login.aspx" TARGET="_parent">登出</a>
</div>
<div class="clearbox"></div>
</div>
</div>
<div id="Container"><div id="Content">
<div id="Menu"><div class="menubox">
<div class="mod1"><ul>
<li class="btn1"><a href="UserView.aspx" runat="server" title="新增/ add">新增/add</a>
</li>
<li class="btn3"><asp:LinkButton ID="lnkbutton_edit" runat="server" ToolTip="保存/save" OnClick="lnkbutton_save_Click">保存/save</asp:LinkButton>
</li>
<li class="btn5"><a href="UserList.aspx" runat="server" title="查看/ view">查看/view</a>
</li>
</ul>
</div>
<div class="mod2"></div>
<div class="clearbox"></div>
</div>
</div>

<div class="space1"></div>
<div class="container mt-3 border border-primary">
  <div class="container mt-3">
<div class="row mb-3">
     <div class="col-lg-6  d-flex">
  <asp:Label ID="Label1" runat="server"  CssClass="me-10">员工编号</asp:Label>
  <asp:TextBox Runat="server" ID="txb_UserName" CssClass="form-control custom-heighter-width text-start border-primary"/>
</div>
  <div class="col-lg-6  d-flex">
      <asp:Label ID="Label2" runat="server"  CssClass="me-10">员工卡号</asp:Label>
      <asp:TextBox Runat="server" ID="txb_CardId" CssClass="form-control custom-heighter-width text-start border-primary"/>
</div>
</div>
<div class="row mb-3">
     <div class="col-lg-6  d-flex">
         <asp:Label ID="Label3" runat="server"  CssClass="me-10">员工姓名</asp:Label>
        <asp:TextBox Runat="server" ID="txb_Name" CssClass="form-control custom-heighter-width text-start border-primary"/>
    </div>
    <div class="col-lg-6  d-flex"></div>
</div>
<div class="row mb-3">
<div class="col-lg-6  d-flex">
    <asp:Label ID="Label6" runat="server"  CssClass="me-10">电子邮件</asp:Label>
   <asp:TextBox Runat="server" ID="txb_Email" CssClass="form-control custom-heighter-width text-start border-primary"/>
</div>
<div class="col-lg-6  d-flex"></div>
</div>

<div class="row mb-3">
<div class="col-lg-6  d-flex">
    <asp:Label ID="lbl_resignation" runat="server" CssClass="me-10">是否离职</asp:Label>
    <asp:CheckBox runat="server" ID="chk_resignation" />
</div>
<div class="col-lg-6  d-flex">
    <asp:Label ID="lbl_resignDate" runat="server" CssClass="me-10">离职时间</asp:Label>
    <asp:TextBox ID="txt_resignDate" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
        <ajaxtoolkit:calendarextender id="cal_sdate" runat="server" targetcontrolid="txt_resignDate"></ajaxtoolkit:calendarextender>
    </div>
</div>
<div class="row mb-3">
<div class="col-lg-6  d-flex">
<asp:Label ID="Label7" runat="server" CssClass="me-10">备注</asp:Label>
</div>
<div class="col-lg-6  d-flex">
    <asp:TextBox ID="txt_comment" runat="server" CssClass="form-control custom-large-width text-start border-primary"></asp:TextBox>
</div>

</div>
<asp:button id="Button_Edit" runat="server" Text="保存修改" visible="false"></asp:button>
<asp:Label id="lab_id" runat="server" Visible="False"></asp:Label>
<asp:Label id="lab_name" runat="server" Visible="False"></asp:Label>

</div>
</div>
</div>
 <div class="container mt-3 border border-warning"><table width="100%" align="center" class="tbMessage"><TR vAlign="middle">
<TD width="15%" height="28"><DIV align="center"><B><DIV align="center"><B>提示</B>
</DIV>
</B>
</DIV>
</TD>
<TD class="msg" width="85%">
&nbsp;&nbsp;<asp:label id="Label_Message" runat="server"></asp:label>
</TD>
</TR>
</table>
</div>
</div>
</div>


</asp:Content>
