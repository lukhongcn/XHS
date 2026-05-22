<%@ Page Language="c#" CodeBehind="newLeft.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.controls.newLeft" %>

<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>left</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../css/ehour.css" type="text/css" rel="stylesheet">
</head>
<script type="text/javascript">
    function postBackByObject(e) {
        var evt = e || window.event;
        var o = evt.target || evt.srcElement;
        if (o.tagName == "INPUT" && o.type == "checkbox") {
            __doPostBack("", "");
        }
</script>
<body topmargin="0" ms_positioning="GridLayout">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <div class="GreyNavFrame" style="margin-top: 0em">

            <div class="GreyNavFrameHead">
                <h2 style="height: 15px">
                    <asp:Label ID="lab_menu" runat="server" Visible="false"></asp:Label>
                     
                </h2>
                <asp:Label ID="lab_name" runat="server" Font-Size="Medium"></asp:Label>
             


            </div>
            <div class="GreyNavBody">
                <asp:Label ID="Label_MenuDisplay" runat="server"></asp:Label>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <iewc:TreeView ID="tv_menu" runat="server" NodeIndent="0" ShowExpandCollapse="False" BackColor="Transparent" CssClass="common.css" BorderColor="Transparent"
                            DefaultStyle="font-family:Verdana,Arial,Helvetica,sans-serif;font-size:11px;"></iewc:TreeView></IEWC:TREEVIEW>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="GreyNavFrameFooter">
                <p>
                </p>
            </div>
        </div>
    </form>
</body>
</html>