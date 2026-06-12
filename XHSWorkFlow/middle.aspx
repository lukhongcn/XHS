<%@ Page language="c#" Codebehind="middle.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.middle" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
		<title>middle</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<style> #navPoint { COLOR: white; CURSOR: hand; FONT-FAMILY: Webdings; FONT-SIZE: 9pt }
	</style>
		<script language='javaScript' type='text/javaScript'>
var displayBar=true;
function switchBar(o) {
    if (displayBar) {
        parent.document.getElementById("left").parentNode.cols="0,12,*";
        displayBar=false;
        o.value=">"
    } else {
        parent.document.getElementById("left").parentNode.cols="220,12,*";
        displayBar=true;
        o.value="<"
    }
}
</script>
</HEAD>
	<body MS_POSITIONING="GridLayout" leftmargin="0" topmargin=0  >
	
			<table border=0 onclick="switchBar(this);">
				<tr>
					<td width="6" bgcolor="#E3E3E4" height="800"><span id="navPoint">3</span></td>
				</tr>
			</table>
		
	</body>
</HTML>
