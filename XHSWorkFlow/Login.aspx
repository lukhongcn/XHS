<%@ Page language="c#" Codebehind="Login.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.Login" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AMDP</title>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
		<br>
			<br>
			<br>
            <br>
			<br>
			<br>
			<table border="1" align="center" cellpadding="40" cellspacing="0" >
		    <tr>
    <td>
			<table border="0" id="11" align="center" cellpadding="1" cellspacing="1">
			<tr align="center" valign="middle">
             <td colspan="2" ><img src="images/wtclogo.png" width="198" height="65" align="left"></td></tr>
				<tr align="center" valign="middle">
					<td colspan="2"><DIV align="center" class="mainTitle" ><B>系统登录</B></DIV>
					</td>
				</tr>
				<tr>
					<td align="right" class="mainTitle" >员工编号</td>
					<td width="40"  ><asp:TextBox ID="TextBox_UserName" Runat="server"></asp:TextBox></td>
				</tr>
				<tr>
					<td align="right" class="mainTitle" >密码</td>
					<td><INPUT type="password" name="HTML_Password" runat="server" id="HTML_Password"></td>
				</tr>
				<tr>
					<td colspan="2" align="center" style="WIDTH: 373px"><asp:Button ID="Button_Login" Runat="server" Text="登录"></asp:Button>&nbsp;&nbsp;<INPUT type="reset" value="重设"></td>
				</tr>
				<TR>
					<td>
                        <asp:Label ID="lab_Version" runat="server" ForeColor="White"></asp:Label></td>
					
					<td colspan="1" align="left">
						<asp:label id="Label_Message" runat="server" ForeColor="#000000"></asp:label></td>
				</TR>
			</table>
			</td>
			</tr>
			</table>
			<br>
		</form>
	</body>
</HTML>
