<%@ Page Language="c#" CodeBehind="XWDPartMasterList.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.XWDPartMasterList" MasterPageFile="~/DefaultSub.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
    <div id="Wrapper">
        <div id="Header">
            <div class="headbox">
                <div class="linebox">
                    <a href="Default.aspx">标签管理</a>
                    <img src="images/arrow.png" />
                    <a href="#"><%=menuname %></a>
                </div>
                <div class="logout">
                    <a href="login.aspx" target="_parent">登出</a>
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
                                <li class="btn2">
                                    <asp:LinkButton ID="lnkbutton_edit" runat="server" OnClick="lnkbutton_edit_Click" ToolTip="编辑/edit">编辑/edit</asp:LinkButton>
                                </li>
                            </ul>
                        </div>
                        <div class="mod2">
                            <ul>
                                <li class="btn8">
                                    <asp:LinkButton ID="lnkbutton_search" runat="server" OnClick="lnkbutton_search_Click" ToolTip="搜索/search">搜索/search</asp:LinkButton>
                                </li>
                            </ul>
                        </div>
                        <div class="clearbox"></div>
                    </div>
                </div>

                <div class="space1"></div>
                <div class="container mt-3 border border-primary">
                    <div class="container mt-3 mb-3">
                        <div class="row mb-3">
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_JHSPartNo" runat="server" AssociatedControlID="TextBox_JHSPartNo" CssClass="me-10 gs-part-master-search-label">本厂零件编号</asp:Label>
                                <asp:TextBox ID="TextBox_JHSPartNo" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_CustomerMaterialNo" runat="server" AssociatedControlID="TextBox_CustomerMaterialNo" CssClass="me-10 gs-part-master-search-label">客户零件编号</asp:Label>
                                <asp:TextBox ID="TextBox_CustomerMaterialNo" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="container mt-3 border border-primary">
                    <div class="container mt-3">
                        <asp:DataGrid ID="MainDataGrid" runat="server" PageSize="20" AutoGenerateColumns="False" AllowPaging="True" CssClass="table table-striped table-bordered table-hover table-sm" OnPageIndexChanged="MainDataGrid_PageIndexChanged" OnEditCommand="MainDataGrid_EditCommand" OnUpdateCommand="MainDataGrid_UpdateCommand" OnCancelCommand="MainDataGrid_CancelCommand">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="table-primary"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true" CssClass="wrap-text" />
                            <Columns>
                                <asp:TemplateColumn HeaderText="本厂编号">
                                    <ItemTemplate><asp:Label ID="Label_JHSPartNoValue" runat="server" Text='<%# Eval("JHSPartNo") %>'></asp:Label></ItemTemplate>
                                    <EditItemTemplate><asp:Label ID="Label_JHSPartNoEditValue" runat="server" Text='<%# Eval("JHSPartNo") %>'></asp:Label></EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="客户编号">
                                    <ItemTemplate><asp:Label ID="Label_CustomerMaterialNoValue" runat="server" Text='<%# Eval("CustomerMaterialNo") %>'></asp:Label></ItemTemplate>
                                    <EditItemTemplate><asp:Label ID="Label_CustomerMaterialNoEditValue" runat="server" Text='<%# Eval("CustomerMaterialNo") %>'></asp:Label></EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="产品名称">
                                    <ItemTemplate><asp:Label ID="Label_MaterialNameValue" runat="server" Text='<%# Eval("MaterialName") %>'></asp:Label></ItemTemplate>
                                    <EditItemTemplate><asp:Label ID="Label_MaterialNameEditValue" runat="server" Text='<%# Eval("MaterialName") %>'></asp:Label></EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="版本号">
                                    <ItemTemplate>
                                        <%# Eval("Remark") %>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox_Remark" runat="server" Text='<%# Bind("Remark") %>' CssClass="form-control"></asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:EditCommandColumn HeaderText="操作" EditText="编辑" UpdateText="保存" CancelText="取消"></asp:EditCommandColumn>
                            </Columns>
                            <PagerStyle Mode="NumericPages" CssClass="table-primary"></PagerStyle>
                        </asp:DataGrid>
                    </div>
                </div>

                <div class="container mt-3 border border-warning">
                    <table width="100%" align="center" class="tbMessage">
                        <tr valign="middle">
                            <td width="10%" height="28">
                                <div align="center"><b><asp:Label ID="Label2" runat="server">提示</asp:Label></b></div>
                            </td>
                            <td class="msg" width="85%">&nbsp;&nbsp;<asp:Label ID="Label_Message" runat="server"></asp:Label></td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JSHolder" runat="server">
    <style type="text/css">
        .gs-part-master-search-label {
            min-width: 110px;
            line-height: 30px;
        }
    </style>
</asp:Content>
