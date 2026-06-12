<%@ Page language="c#" Codebehind="UserView.aspx.cs" AutoEventWireup="false" MasterPageFile="~/DefaultSub.Master"  Inherits="ModuleWorkFlow.UserView" %>
<%--<%@ Register TagPrefix="uc1" TagName="header" Src="controls/header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="controls/footer.ascx" %>--%>
<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
<div id="Wrapper">
<div id="Header"><div class="headbox">
<div class="linebox">
<a href="#"><%=Utility.Translate.translateString("生產管理")%></a>
<img src="images/arrow.png" />
<a href="#"><%=menuname %></a>
</div>
<div class="logout"><a href="../login.aspx" TARGET="_parent"><%=Utility.Translate.translateString("登出")%></a>
</div>
<div class="clearbox"></div>
</div>
</div>
<div id="Container"><div id="Content">
<div id="Menu"><div class="menubox">
<div class="mod1"><ul>
<li class="btn1"><a href="UserView.aspx" runat="server" title="新增/ add">新增/add</a>
</li>
<li class="btn3"><asp:LinkButton ID="lnkbutton_edit" runat="server" ToolTip="儲存/save" OnClick="lnkbutton_save_Click">儲存/save</asp:LinkButton>
</li>
<li class="btn5"><a href="UserList.aspx" runat="server" title="檢視/ view">檢視/view</a>
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
    <div class="col-lg-6 d-flex">  
        <asp:Label ID="lbl_employeeId" runat="server" CssClass="me-10"><%=Utility.Translate.translateString("員工編號")%></asp:Label>
          
            <asp:textbox id="txt_username" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:textbox>  
            <asp:CustomValidator id="val_username" runat="server" ErrorMessage="CustomValidator" ControlToValidate="txt_username"></asp:CustomValidator>  
        
    </div>  
    <div class="col-lg-6 d-flex">  
        <asp:Label ID="lbl_employeeRole" runat="server" CssClass="me-10"><%=Utility.Translate.translateString("員工職能")%></asp:Label>
        <asp:DropDownList id="ddl_department" runat="server"  CssClass="form-select custom-heighter-width  text-start border-primary me-1"></asp:DropDownList>
    </div>  
</div>
<div class="row mb-3">  
    <div class="col-lg-6 d-flex">  
<asp:Label ID="Label1" runat="server" CssClass="me-10"><%=Utility.Translate.translateString("員工姓名")%></asp:Label>
<asp:textbox id="txt_name" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:textbox>
</div>
<div class="col-lg-6 d-flex"> <asp:Label ID="Label3" runat="server" CssClass="me-10"> <%=Utility.Translate.translateString("員工卡號")%></asp:Label>
<asp:textbox id="txt_cardid" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:textbox>
</div>
</div>
<div class="row mb-3">  
    <div class="col-lg-6 d-flex">  
<asp:Label ID="Label4" runat="server" CssClass="me-10">Email</asp:Label>
<asp:textbox id="txt_email" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:textbox>

    </div>
     <div class="col-lg-6 d-flex">  
<asp:Label ID="Label5" runat="server" CssClass="me-10"><%=Utility.Translate.translateString("班別")%></asp:Label>
              <asp:DropDownList id="ddl_shift" runat="server" CssClass="form-select custom-heighter-width  text-start border-primary me-1"></asp:DropDownList>
                             
</div>
</div>

           <div class="row mb-3">  
    <div class="col-lg-6 d-flex">  
        <asp:Label ID="Label8" runat="server" CssClass="me-10"><%=Utility.Translate.translateString("密碼")%></asp:Label>
        <asp:TextBox ID="txt_password" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
</div>
        </div>
</div>
</div>
 <div class="container mt-3 border border-warning"><table width="100%" align="center" class="tbMessage"><TR vAlign="middle">
<TD width="10%" height="28"><DIV align="center"><B>
<asp:Label ID="Label2" runat="server">提示</asp:Label><DIV align="center"><B></B>
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
</div>
</asp:Content>

