<%@ Page Language="c#" CodeBehind="UserPrivilegeAddEdit.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.admin.UserPrivilegeAddEdit" MasterPageFile="~/DefaultSub.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <div id="Wrapper">
            <div id="Header">
                <div class="headbox">
                    <div class="linebox">
                        <a href="#"><%=Utility.Translate.translateString("生產管理")%></a>
                        <img src="images/arrow.png" />
                        <a href="#">><%=menuname %></a>
                    </div>
                    <div class="logout">
                        <a href="../login.aspx" target="_parent"><%=Utility.Translate.translateString("登出")%></a>
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
                                     <li class="btn5"><a href="UserPrivilegeList.aspx" title="檢視 / view">檢視/view</a>
                                     </li>
                                     <li class="btn3">
                                         <asp:LinkButton ID="lnkbutton_edit" runat="server" ToolTip="儲存/save" OnClick="lnkbutton_save_Click">儲存/save</asp:LinkButton>
                                     </li>

                                 </ul>
                             </div>
                             <div class="mod2">
                                 <ul>
                                     <li class="btn8">
                                         <asp:LinkButton ID="lnkbutton_search" runat="server" ToolTip="搜尋/search" OnClick="lnkbutton_search_Click">搜尋/search</asp:LinkButton>
                                     </li>
                                 </ul>
                             </div>
                             <div class="clearbox"></div>
                         </div>
                     </div>
                    <div class="space1"></div>
                      <asp:Label ID="Label_HiddenUserId" runat="server" Visible="False" ForeColor="Red"></asp:Label>
                     <asp:Label ID="Label_HiddenFuncMode" runat="server" Visible="False" ForeColor="Red"></asp:Label>
                    <div class="container mt-3 border border-primary">
                        <div class="container mt-3">
                                <div class="row mb-3">
                                     <div class="col-lg-12  d-flex">
                                        <asp:Label ID="Label2" runat="server" CssClass="me-10">員工編號:</asp:Label>
                                        <asp:TextBox ID="Textbox_UserId" name="Textbox_UserId" runat="server" CssClass="form-control custom-large-width text-start border-primary"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="Textbox_UserId" ErrorMessage="請輸入員工編號"></asp:RequiredFieldValidator>
                                        <asp:Label ID="Label3" runat="server" CssClass="me-10">口令:</asp:Label>
                                        <asp:TextBox ID="Textbox_Password" name="Textbox_Password" runat="server" CssClass="form-control custom-large-width text-start border-primary"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="Requiredfieldvalidator3" runat="server" ControlToValidate="Textbox_Password" ErrorMessage="請輸入口令"></asp:RequiredFieldValidator>
                                        <asp:DropDownList ID="DropDownList_Role" runat="server" Visible="False"></asp:DropDownList>
                                    </div>
                                </div>
                             <div class="row mb-3">
                                 <div class="col-lg-12  d-flex">
                                        <asp:Label ID="Label4" runat="server"  CssClass="me-10">模塊:</asp:Label>
                                        <asp:DropDownList ID="drp_menu" runat="server" OnSelectedIndexChanged="drp_menu_SelectedIndexChanged" CssClass="form-select custom-heighter-width  text-start border-primary me-1" AutoPostBack="true"></asp:DropDownList>
                                        <asp:Label ID="Label5" runat="server"  CssClass="me-10">子模塊:</asp:Label>
                                        <asp:DropDownList ID="drp_subMenu" runat="server"  CssClass="form-select custom-heighter-width  text-start border-primary me-1"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="row mb-3">
                                   
                                    <div class="col-lg-12  d-flex">
                                        <asp:DataGrid ID="DataGridPrivate" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered table-hover table-sm" style="table-layout:fixed;width: 100%;">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="table-primary" ></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true" CssClass="wrap-text"/>
                                            <Columns>
                                                <asp:BoundColumn DataField="menuid" HeaderText="功能項"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="menuname" HeaderText="功能項描述"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="url" HeaderText="url" Visible="False"></asp:BoundColumn>
                                                <asp:TemplateColumn HeaderText="瀏覽">
                                                    <ItemTemplate>
                                                        <asp:CheckBox runat="server" ID="Checkbox_PQuery"></asp:CheckBox>
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="添加">
                                                    <ItemTemplate>
                                                        <asp:CheckBox runat="server" ID="CheckBox_PAdd"></asp:CheckBox>
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="編輯">
                                                    <ItemTemplate>
                                                        <asp:CheckBox runat="server" ID="Checkbox_PEdit"></asp:CheckBox>
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="刪除">
                                                    <ItemTemplate>
                                                        <asp:CheckBox runat="server" ID="Checkbox_PDel"></asp:CheckBox>
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                </div>
                                </div>
                              <asp:Panel ID="pan_processgroup" runat="server" >
                              <asp:UpdatePanel ID="UpdatePanel_ProcessGroup" runat="server">
                            <ContentTemplate>
                            <div class="row mb-3">
                                <div class="col-lg-6 d-flex">
                                    <asp:DropDownList ID="dpl_processGroup" runat="server" CssClass="form-select custom-heighter-width text-start border-primary me-1" OnSelectedIndexChanged="dpl_processGroup_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mb-3">
                                    <div class="col-lg-12 d-flex">
                                        <div class="overflow-auto border border-primary" style="overflow-y: scroll; width: 100%; border-top-style: ridge; border-right-style: ridge; border-left-style: ridge; height: 200px; border-bottom-style: ridge"  onscroll="saveScrollPosition()">
                                            <asp:DataGrid ID="MainDataGrid" runat="server" AutoGenerateColumns="False"  CssClass="table table-striped table-bordered table-hover table-sm" style="table-layout:fixed;width: 100%;">
                                               <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="table-primary" ></HeaderStyle>
                                               <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true" CssClass="wrap-text"/>
                                                <Columns>
                                                    <asp:TemplateColumn HeaderText="選擇">
                                                        <HeaderTemplate>
                                                            <asp:CheckBox ID="checkall" runat="server" AutoPostBack="false" onclick="javascript:SelectAll(this);"></asp:CheckBox>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox runat="server" ID="CheckBox_Select" Checked="False"></asp:CheckBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:BoundColumn DataField="processid" HeaderText="工序" Visible="False"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="processname" HeaderText="工序"></asp:BoundColumn>
                                                </Columns>
                                            </asp:DataGrid>
                                        </div>
                                     </div>
                                    </div>
                                  </ContentTemplate>
                                      <Triggers>
                                              <asp:AsyncPostBackTrigger ControlID="dpl_processGroup" EventName="SelectedIndexChanged" />
                                          </Triggers>
                                </asp:UpdatePanel>
                                       </asp:Panel>
                                <asp:Panel ID="pan_columnPriviage" runat="server" Visible="false" class="row mb-3">
                                    <div class="col-lg-6 d-flex">
                                        <asp:DataGrid ID="dg_column" runat="server" AutoGenerateColumns="False" OnItemDataBound="dg_column_DataBinding" CssClass="table table-striped table-bordered table-hover table-sm">
                                           <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="table-primary"></HeaderStyle>
                                           <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true" CssClass="wrap-text" />
                                           <Columns>
                                               <asp:TemplateColumn HeaderStyle-Width="10px">
                                                   <HeaderTemplate>
                                                       <asp:CheckBox ID="checkall" runat="server" AutoPostBack="false" onclick="javascript:SelectAll(this);"></asp:CheckBox>
                                                   </HeaderTemplate>
                                                   <ItemTemplate>
                                                       <asp:CheckBox runat="server" ID="CheckBox_Select" Checked="False"></asp:CheckBox>
                                                   </ItemTemplate>
                                               </asp:TemplateColumn>
                                               <asp:BoundColumn DataField="MenuName" HeaderText="模塊"></asp:BoundColumn>
                                               <asp:BoundColumn DataField="TableValue" HeaderText="內容"></asp:BoundColumn>
                                           </Columns>
                                       </asp:DataGrid>
                                    </div>

                                </asp:Panel>
                               
                                <div class="row mb-3">
                                     <div class="col-lg-1 d-flex">
                                        <asp:Label ID="Label9" runat="server" CssClass="me-10">系統管理員</asp:Label>
                                     </div>
                                    <div class="col-lg-11 d-flex">
                                        <asp:RadioButtonList ID="RadioButtonList_IsAdmin" runat="server"></asp:RadioButtonList>
                                    </div>
                                </div>
                        </div>
                    </div>
                    <div class="container mt-3 border border-warning">
                        <table id="Table3" width="100%" align="center" class="tbMessage" >
                            <tr valign="middle">
                                <td width="10%" height="28">
                                    <div align="center">
                                        <b>
                                            <div align="center">
                                                <b>
                                                    <asp:Label ID="Label10" runat="server">提示</asp:Label></b>
                                            </div>
                                        </b>
                                    </div>
                                </td>
                                <td class="msg" width="85%">&nbsp;&nbsp;<asp:Label ID="Label_Message" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
</asp:Content>
  <asp:Content ID="Content2" ContentPlaceHolderID="JSHolder" runat="server">
   
    <!-- Select2 -->
    <script src="plugins/select2/select2.full.min.js"></script>
    <link rel="stylesheet" href="plugins/select2/select2.min.css">
    <script>
        $(document).ready(function () {

            $('.relationButton').click(function () {
                var $this = $(this);
                var row = $this.parents('tr');
                alert("clicked " + row.index());
            });
        });

        $(function () {
            //Initialize Select2 Elements
            $(".select2").select2();


        });

        // 保存滚动位置
        function saveScrollPosition() {
            sessionStorage.setItem('scrollPosition', document.querySelector('.overflow-auto').scrollTop);
        }

        // 恢复滚动位置
        function restoreScrollPosition() {
            var scrollPos = sessionStorage.getItem('scrollPosition');
            if (scrollPos) {
                document.querySelector('.overflow-auto').scrollTop = scrollPos;
            }
        }

        // 页面加载后恢复滚动位置
        Sys.Application.add_load(function () {
            restoreScrollPosition();
        });
    </script>
</asp:Content>

