<%@ Page language="c#" Codebehind="PartModifySplit.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.PartModifySplit" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" ><html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=big5" />
<title><%=title %></title>
<link href="css/amd.css" rel="stylesheet" type="text/css" />
<link rel="stylesheet" href="css/jquery.treetable.css" />
<link rel="stylesheet" href="css/jquery.treetable.theme.default.css" />
<link rel="stylesheet" href="plugins/bootstrap.5.2.3/css/bootstrap.min.css">
<link rel="stylesheet" href="plugins/jQuery-datetimepicker.2.4.5/jquery.datetimepicker.css" type="text/css" />
<script language="javascript" src="js/lib.js"></script>
<script type="text/javascript" src="plugins/JQuery.3.4.1/jquery-3.4.1.min.js"></script>
  
<script type="text/javascript" src="plugins/jQuery-datetimepicker.2.4.5/jquery.datetimepicker.js"></script>
<script type="text/Javascript" src="js/jquery.treetable.js"></script>
   
 <style>
    /* Custom styles for the date picker input */
    /*.date-picker {
        padding: 10px;
        border: 1px solid #ccc;
        border-radius: 5px;
        width: 200px;
        font-size: 16px;
    }*/
     .custom-height-width {
        height: 30px; /* Set your desired height */
        padding: 6px 12px; /* Adjust padding as necessary */
        font-size: 14px; /* Adjust font size if needed */
        width: 160px;
    }

      .custom-large-width {
            height: 30px; /* Set your desired height */
            padding: 6px 12px; /* Adjust padding as necessary */
            font-size: 14px; /* Adjust font size if needed */
            width: 230px;
        }

       .custom-htight {
           height: 20px; /* Set your desired height */
          
       }

     .custom-number-width {
        height: 30px; /* Set your desired height */
        padding: 6px 12px; /* Adjust padding as necessary */
        font-size: 14px; /* Adjust font size if needed */
        width: 60px;
    }

      .custom-heighter-width {
            height: 30px; /* Set your desired height */
            padding: 6px 12px; /* Adjust padding as necessary */
            font-size: 14px; /* Adjust font size if needed */
            width: 160px;
        }

     .me-10 {
        margin-right: 10px; /* Set your desired right margin */
      }

       .ml-10 {
             margin-left: 10px; /* Set your desired right margin */
         }

     table.treetable tbody tr td {
         position: relative;
         padding-left: 0px;
         word-break: break-all;
         word-wrap: break-word
     }

    table.treetable tbody tr td span.indenter {
         position: relative;
         left: 5px;
         word-break: break-all;
         word-wrap: break-word
     }

    .custom-warning-bold-border {
    border: 5px solid #ffc107; /* Bold warning color */
  }


</style>
</head>
<body><form id="Form1" method="post" runat="server"><div id="Wrapper">
<div id="Header"><div class="headbox">
<div class="linebox">
<a href="../defaultmain.aspx">生產管理</a>
<img src="images/arrow.png" />
<a href="#"><%=title %></a>
</div>
<div class="logout"><a href="login.aspx" TARGET="_parent">登出</a>
</div>
<div class="clearbox"></div>
</div>
</div>

<div id="Container">
    <div id="Content">
        <div id="Menu">
            <div class="menubox">
                <div class="mod2"><ul>

                <li class="btn3"><asp:LinkButton ID="lnkbutton_save" runat="server" OnClick="lnkbutton_save_Click" ToolTip="保存">保存</asp:LinkButton>
                </li>
                <li class="btn8"><asp:LinkButton ID="lnkbutton_search" runat="server" ToolTip="搜尋/search" OnClick="lnkbutton_search_Click">搜尋/search</asp:LinkButton>

                </ul>
                </div>
              
                <div class="clearbox"></div>
            </div>
        </div>
        <div class="container mt-3 border border-primary">
            <asp:label id="lab_seach_moduleid" runat="server" visible="false"></asp:label>
            <asp:label id="lab_type" Visible="False" Runat="server"></asp:label>
            <div class="container mt-3">
                <div class="row mb-3">
                    <div class="col-lg-7  d-flex">
                       <asp:Label ID="Label1" runat="server" CssClass="me-10">廠批</asp:Label>
                        <asp:dropdownlist id="Dropdownlist_ModuleId" runat="server" AutoPostBack="True" CssClass="form-select custom-large-width  text-start border-primary me-1"></asp:dropdownlist>
                        <asp:textbox id="txt_moduleid" runat="server" CssClass="form-control custom-large-width text-start border-primary"></asp:textbox>
                    </div>
                    <div class="col-lg-5  d-flex">
                        <asp:Label ID="Label2" runat="server" CssClass="me-10">圖號</asp:Label>
                        <asp:textbox id="txt_partnoid" runat="server" CssClass="form-control custom-large-width text-start border-primary"></asp:textbox>
                    </div>
                </div>
                  <div class="row mb-3">
                      <div class="col-lg-7  d-flex">
                         <asp:Label ID="Label3" runat="server" CssClass="me-10">條碼</asp:Label>
                          <asp:textbox id="txt_QRCode" runat="server" CssClass="form-control custom-large-width text-start border-primary"></asp:textbox>
                      </div>
                      <div class="col-lg-5  d-flex">
                          <asp:Label ID="Label4" runat="server" CssClass="me-10">拆分至箱號</asp:Label>
                          <asp:textbox id="txt_splitPartNoId" runat="server" CssClass="form-control custom-large-width text-start border-primary"></asp:textbox>
                      </div>
                </div>
            </div>
        </div>
        <div class="container mt-3 border border-primary" >
            <div class="table-responsive">
                <asp:datagrid id="MainDataGrid" runat="server" AllowPaging="false" AutoGenerateColumns="False"  CssClass="table table-striped table-bordered table-hover table-sm"  AllowCustomPaging="false" style="table-layout:fixed;width: 100%;">
            
                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="table-primary" Wrap="true"></HeaderStyle>
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true" />
                    <Columns>
                        <asp:TemplateColumn HeaderStyle-Width ="60px">
                            <ItemTemplate>
                            <asp:CheckBox runat="server" ID="chk_datagrid" />
                            </ItemTemplate>
                        </asp:TemplateColumn>

                        <asp:TemplateColumn  HeaderText="ModuleId" HeaderStyle-Width="100px">
                            <HeaderTemplate>
                                <asp:Label id="lab_header_ModuleId" runat="server" >廠批</asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                            <asp:Label id="lab_ModuleId" runat="server"  Text='<%# DataBinder.Eval(Container, "DataItem.ModuleId") %>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn  HeaderText="PartNo_Id" HeaderStyle-Width="100px">
                            <HeaderTemplate><asp:Label id="Label_10" runat="server">箱號</asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                            <asp:Label id="lab_PartNo_Idtemp" runat="server" Text='<%# Convert.ToString(Eval("PartNo")) %>'/>
                            </ItemTemplate>
                            </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="LeastCount" Visible="true" HeaderStyle-Width="100px">
                                      <HeaderTemplate><asp:Label id="Label_11" runat="server">可拆分數量</asp:Label></HeaderTemplate>
                              <ItemTemplate>
                                  <asp:Label ID="lab_LeastCount" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "LeastCount") %>' Visible="false" />
                                   <asp:Label ID="lab_ProcessOrder" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ProcessOrder") %>' Visible="false" />
                                  <asp:TextBox ID="txt_LeastCount" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "LeastCount") %>' Width="80px" CssClass="form-control custom-height-width text-start border-primary" />
                              </ItemTemplate>
                        </asp:TemplateColumn>
                          <asp:BoundColumn DataField="QRCode" HeaderText="條碼"  HeaderStyle-Width="100px"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Priority" HeaderText="等級" Visible="false"></asp:BoundColumn>
                  
                        <asp:TemplateColumn HeaderText="工序" HeaderStyle-Width="635px"><ItemTemplate>
                            <asp:Label runat="server" ID="Label_ProcessName" Text='<%# DataBinder.Eval(Container, "DataItem.ProcessName") %>' ></asp:Label>
                             <asp:Label runat="server" ID="dg_lnk_ProcessName" Text='' Visible="false"></asp:Label>
                        </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="priorityname" HeaderText="數量" HeaderStyle-Width ="40px"></asp:BoundColumn>
                        <asp:BoundColumn DataField="priorityname" HeaderText="等級" HeaderStyle-Width ="40px"></asp:BoundColumn>
                     
                      
                       

                        <asp:TemplateColumn  HeaderText="PartNo_Id" Visible="false">
                            <HeaderTemplate><asp:Label id="Label_101" runat="server" >箱號</asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                            <asp:Label id="lab_PartNo_Id" runat="server"  Text='<%# DataBinder.Eval(Container, "DataItem.PartNo_Id") %>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>

                        <asp:TemplateColumn HeaderText="DataTTid" Visible="false">
	                        <ItemTemplate>
		                        <asp:Label runat="server" ID="dg_lab_DataTTid" Text='<%# DataBinder.Eval(Container, "DataItem.DataTTid") %>'>
		                        </asp:Label>
	                        </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="ParentDataTTid" Visible="false">
	                        <ItemTemplate>
		                        <asp:Label runat="server" ID="dg_lab_ParentDataTTid" Text='<%# DataBinder.Eval(Container, "DataItem.ParentDataTTid") %>'>
		                        </asp:Label>
	                        </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn  HeaderText="ProcessNo" Visible="false">

                            <ItemTemplate>
                            <asp:Label id="lab_ProcessNo" runat="server" Text='<%# DataBinder.Eval(Container,"DataItem.ProcessNo") %>'/>
                            </ItemTemplate>
                        </asp:TemplateColumn>

                    </Columns>
   
   
                    <PagerStyle   CssClass="table-primary" NextPageText="" PrevPageText="" >
                    </PagerStyle>
  

                  </asp:datagrid>
            </div>
   <table style="BORDER-COLLAPSE: collapse" cellSpacing="0" width="100%"  >

</table>

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
</div>
<script>
    $(".table").treetable({ expandable: true });
</script>
</form>
</body>
</html>

