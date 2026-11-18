<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register TagPrefix="cc1" Namespace="ModuleWorkFlow.WebView" Assembly="WebView" %>

<%@ Page Language="c#" CodePage="65001" CodeBehind="PartModifyEdit.aspx.cs" MasterPageFile="~/DefaultSub.Master" AutoEventWireup="false" Inherits="ModuleWorkFlow.PartModifyEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
    <div id="Wrapper">
        <div id="Header">
            <div class="headbox">
                <div class="linebox">
                    <a href="#"><%=Utility.Translate.translateString("生產管理")%></a>
                    <img src="images/arrow.png" />
                    <a href="#"><%=menuname %></a>
                </div>
                <div class="logout">
                    <a href="login.aspx" target="_parent"><%=Utility.Translate.translateString("登出")%></a>
                </div>
                <div class="clearbox"></div>
            </div>
        </div>
        
        <div id="Container">
            <div id="Content">
                <div id="Menu">
                    <div class="menubox">
                       
                        <div class="mod1">
                            <ul>
                                <li class="btn5">
                                    <asp:LinkButton ID="lnkbutton_view" runat="server"
                                        ToolTip="檢視/view" OnClick="lnkbutton_view_Click">檢視/view</asp:LinkButton>
                                </li>
                               
                            </ul>
                        </div>
                         <div class="mod2">
                             <ul>
                                 <li class="btn3">
                                     <asp:LinkButton ID="lnkbutton_edit" runat="server" ToolTip="儲存/save" OnClick="lnkbutton_save_Click">儲存/save</asp:LinkButton>
                                 </li>
                                 </ul>
                             </div>
                        <div class="clearbox"></div>
                    </div>
                </div>
                <div class="space1"></div>
                <div class="container mt-3 border border-primary" >
                   <div class="container mt-3">
                         <div class="row mb-3">
                            <div class="col-lg-4  d-flex">
                                <asp:Label ID="Label4" runat="server" CssClass="me-10">客户名称</asp:Label>
                                <asp:Label ID="Label_CustomerName" runat="server"></asp:Label>
                                <asp:Label ID="Label_ModuleId" runat="server" Visible="false"></asp:Label>
                            </div>
                             <div class="col-lg-4  d-flex">
                                <asp:Label ID="Label1" runat="server" CssClass="me-10">零件编号</asp:Label>
                                <asp:Label ID="Label_PartNo" runat="server"></asp:Label>
                            </div>
                             <div class="col-lg-4  d-flex">
                                <asp:Label ID="Label2" runat="server" CssClass="me-10">零件编码</asp:Label>
                                <asp:Label ID="Label_PartNoId" runat="server"></asp:Label>
                            </div>
                        </div>
                         <div class="row mb-3">
                             <div class="col-lg-6 d-flex d-none">
                                <asp:Label ID="Label3" runat="server" CssClass="me-10">等級</asp:Label>
                                <asp:DropDownList ID="ListBox_Priority" runat="server"  CssClass="form-select custom-heighter-width  text-start border-primary me-1"></asp:DropDownList>
                            </div>
                         
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-12  d-flex">
                            <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true" EnableScriptLocalization="true"></asp:ScriptManager>
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                <div class="container mt-3">
                                    <div class="row g-0 mb-3">
                                        <div class="col-lg-3  d-flex">
                                           <div class="overflow-auto border border-primary" style="height: 360px; width:220px;">
                                                <div class="mt-3 ml-10 "> <!-- Bootstrap class for margin-top -->
                                                    <asp:CheckBoxList ID="CheckBoxList_Process" runat="server" AutoPostBack="True" RepeatDirection="Vertical"  repeatcolumns="1" Width="200px"  RepeatLayout="Table" ></asp:CheckBoxList>
                                                </div>
                                            </div>
                                        </div>
                                         <div class="col-lg-9  d-flex">
                                            <asp:DataGrid ID="MainDataGrid" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered table-hover table-sm" style="height: 40px; overflow-y: auto;" >
                                               <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="table-primary"  Wrap="true"></HeaderStyle>
                                                <Columns>
                                                    <asp:ButtonColumn Text="選擇" ButtonType="PushButton" CommandName="Select"></asp:ButtonColumn>
                                                    <asp:TemplateColumn HeaderText="">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label_OrderNo" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ListOrder") %>'></asp:Label>
                                                            <asp:Label ID="Label_processno" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.processno") %>' Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>

                                                    <asp:TemplateColumn Visible="false">
                                                        <HeaderTemplate>
                                                            <asp:Label ID="lab_cbxrelation" runat="server" Text='<%# Utility.Translate.translateString("掛靠勾選") %>'></asp:Label>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="cbx_relation" Text="" runat="server"></asp:CheckBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>


                                                    <asp:TemplateColumn Visible="false" HeaderText="工序編號">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="Label_ProcessID" Text='<%# DataBinder.Eval(Container, "DataItem.ProcessID") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn Visible="false" HeaderText="customerProcessId">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="Label_CustomerProcessID" Text='<%# DataBinder.Eval(Container, "DataItem.CustomerProcessID") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="Label_CustomerProcessName" Text='<%# DataBinder.Eval(Container, "DataItem.CustomerProcessName") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="Label_StatusID" Text='<%# DataBinder.Eval(Container, "DataItem.StatusID") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                     <asp:TemplateColumn HeaderText="天" HeaderStyle-CssClass="d-none" ItemStyle-CssClass="d-none">
                                                        <ItemTemplate>
                                                            <asp:DropDownList runat="server" ID="Dropdownlist_Day"></asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                     <asp:TemplateColumn HeaderText="小時" HeaderStyle-CssClass="d-none" ItemStyle-CssClass="d-none">
                                                        <ItemTemplate>
                                                            <asp:DropDownList runat="server" ID="DropDownList_Hour"></asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                     <asp:TemplateColumn HeaderText="分鐘" HeaderStyle-CssClass="d-none" ItemStyle-CssClass="d-none">
                                                        <ItemTemplate>
                                                            <asp:DropDownList runat="server" ID="Dropdownlist_Minute"></asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="計劃開始時間" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txt_startDate" runat="server"></asp:TextBox>
                                                            <ajaxToolkit:CalendarExtender ID="cal_startDate" runat="server" TargetControlID="txt_startDate"></ajaxToolkit:CalendarExtender>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn Visible="false">
                                                        <ItemTemplate>
                                                            <asp:DropDownList runat="server" ID="dpl_startHour"></asp:DropDownList>
                                                            <asp:Label ID="lab_startHour" runat="server" Text="時"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="計劃結束時間" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txt_endDate" runat="server"></asp:TextBox>
                                                            <ajaxToolkit:CalendarExtender ID="cal_endDate" runat="server" TargetControlID="txt_endDate"></ajaxToolkit:CalendarExtender>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn Visible="false">
                                                        <ItemTemplate>
                                                            <asp:DropDownList runat="server" ID="dpl_endHour"></asp:DropDownList>
                                                            <asp:Label ID="lab_endHour" runat="server" Text="小時"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="實際開始日期" Visible="false"> 
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="Label_FactStartDate" Text='<%# string.Format("{0:yyyy-MM-dd}",DataBinder.Eval(Container, "DataItem.FactStartDate")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="實際結束日期" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="Label_FactEndDate" Text='<%# string.Format("{0:yyyy-MM-dd}",DataBinder.Eval(Container, "DataItem.FactEndDate")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="注意事項" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txt_processComment" runat="server"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="加工機台" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="dpl_processMachineid" runat="server"></asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="程式" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chk_Ispcnc" runat="server"></asp:CheckBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>

                                                     <asp:TemplateColumn HeaderText="返修" HeaderStyle-CssClass="d-none" ItemStyle-CssClass="d-none">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="cbl_isunnormal" runat="server"></asp:CheckBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                     <asp:TemplateColumn HeaderText="返修原因" HeaderStyle-CssClass="d-none" ItemStyle-CssClass="d-none">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="dpl_unnormalReasons" runat="server"></asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>

                                                     <asp:TemplateColumn HeaderText="責任部門" HeaderStyle-CssClass="d-none" ItemStyle-CssClass="d-none">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="dpl_DepartMent" runat="server"></asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                     <asp:TemplateColumn HeaderText="責任人" HeaderStyle-CssClass="d-none" ItemStyle-CssClass="d-none">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="dpl_Supply" runat="server"></asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>


                                                    <asp:ButtonColumn Text="上移" ButtonType="PushButton" HeaderText="功能" CommandName="Upper" ItemStyle-HorizontalAlign="Center"></asp:ButtonColumn>
                                                    <asp:ButtonColumn Text="下移" ButtonType="PushButton" HeaderText="功能" CommandName="Lower" ItemStyle-HorizontalAlign="Center"></asp:ButtonColumn>
                                                    <asp:ButtonColumn Text="刪除" ButtonType="PushButton" HeaderText="功能" CommandName="Delete"></asp:ButtonColumn>
                                                    <asp:TemplateColumn Visible="false" HeaderText="工序編號">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="dg_lab_StatusID" Text='<%# DataBinder.Eval(Container, "DataItem.StatusID") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn Visible="false" HeaderText="工序類型">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="dg_lab_pricetype" Text='<%# DataBinder.Eval(Container, "DataItem.pricetype") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn Visible="false" HeaderText="关联零件">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="dg_lab_relationpartno" Text='<%# DataBinder.Eval(Container, "DataItem.RelationPartNo") %>'></asp:Label>
                                                            <asp:Label runat="server" ID="dg_lab_relationpartnoId" Text='<%# DataBinder.Eval(Container, "DataItem.RelationPartNoId") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>

                                                </Columns>
                                                <PagerStyle Mode="NumericPages" CssClass="tbPager"></PagerStyle>
                                            </asp:DataGrid>
                                        </div>
                                    </div>
                                </div>
                                </ContentTemplate>
                                <Triggers>
                                       <asp:AsyncPostBackTrigger ControlID="MainDataGrid" EventName="ItemCommand" />
                                       <asp:AsyncPostBackTrigger ControlID="CheckBoxList_Process" EventName="SelectedIndexChanged" />
                                </Triggers>
                                </asp:UpdatePanel>

                            </div>
                        </div>
                    </div>
                </div>
                <div class="container mt-3 border border-primary">
                    <table width="100%" align="center" class="tbMessage">
                        <tr valign="middle">
                            <td width="15%" height="28">
                                <div align="center">
                                    <b>
                                        <div align="center">
                                            <b>
                                                <asp:Label ID="Label5" runat="server">提示</asp:Label></b>
                                        </div>
                                    </b>
                                </div>
                            </td>
                            <td class="msg" width="85%">&nbsp;&nbsp;<asp:Label ID="Label_Message" runat="server" Font-Bold="True" ForeColor="RED"></asp:Label>
                                <asp:Label ID="Label_DeleteProcessNo" runat="server" ForeColor="Red" Visible="False"></asp:Label>
                                <asp:Label ID="Label_HiddenSelectRow" runat="server" ForeColor="Red" Visible="False"></asp:Label>
                                <asp:Label ID="Label_DisableMaxOrder" runat="server" ForeColor="Red" Visible="False"></asp:Label>
                                <asp:Label ID="Label_HiddenPriority" runat="server" ForeColor="Red" Visible="False"></asp:Label>
                                <asp:Label ID="Label_PageIndex" runat="server" ForeColor="Red" Visible="False"></asp:Label>
                                <asp:Label ID="lab_zuli" runat="server" Visible="False"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

