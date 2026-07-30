<%@ Page Language="C#" CodeBehind="ExceptionReview.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.ExceptionReview" MasterPageFile="~/DefaultSub.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div id="Wrapper">
        <div id="Header"><div class="headbox"><div class="linebox"><a href="#">生产管理</a><img src="images/arrow.png" /><a href="#"><%=menuname %></a></div><div class="logout"><a href="login.aspx" target="_parent">登出</a></div><div class="clearbox"></div></div></div>
        <div id="Container"><div id="Content">
            <div id="Menu"><div class="menubox">
                <div class="mod1"><ul><li class="btn5"><asp:LinkButton ID="lnk_view" runat="server" OnClick="lnk_view_Click" ToolTip="浏览">浏览</asp:LinkButton></li></ul></div>
                <div class="mod2"><ul><li class="btn8"><asp:LinkButton ID="lnk_search" runat="server" OnClick="lnk_search_Click" ToolTip="搜索/search">搜索/search</asp:LinkButton></li><li class="btn3"><asp:LinkButton ID="lnkbutton_save" runat="server" ToolTip="保存/save" OnClick="lnkbutton_save_Click">保存/save</asp:LinkButton></li></ul></div>
                <div class="clearbox"></div>
            </div></div>
            <div class="space1"></div>
            <div class="container mt-3 border border-primary"><div class="container mt-3">
                <div class="row mb-3">
                    <div class="col-lg-4 d-flex"><asp:Label ID="Label_KdCode" runat="server" CssClass="me-10 review-label" AssociatedControlID="txt_KdCode">KD标签</asp:Label><asp:TextBox ID="txt_KdCode" runat="server" CssClass="form-control text-start border-primary"></asp:TextBox></div>
                    <div class="col-lg-3 d-flex align-items-center"><asp:CheckBox ID="chk_Processed" runat="server" Text="已处理" AutoPostBack="true" CssClass="review-check" OnCheckedChanged="chk_Processed_CheckedChanged" /></div>
                </div>
            </div></div>
            <div class="container mt-3 border border-primary"><div class="container mt-3">
                <asp:GridView ID="gvExceptions" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-striped" GridLines="Both" DataKeyNames="Id,PackingId,LockToken,Status" OnRowDataBound="gvExceptions_RowDataBound" EmptyDataText="暂无异常记录" ShowHeaderWhenEmpty="true">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate><asp:CheckBox ID="chk_SelectAll" runat="server" ToolTip="全选" /></HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chk_Select" runat="server" CssClass="row-select" />
                                <asp:HiddenField ID="hid_ExceptionId" runat="server" Value='<%# Eval("Id") %>' />
                                <asp:HiddenField ID="hid_PackingId" runat="server" Value='<%# Eval("PackingId") %>' />
                                <asp:HiddenField ID="hid_LockToken" runat="server" Value='<%# Eval("LockToken") %>' />
                                <asp:HiddenField ID="hid_OriginalStatus" runat="server" Value='<%# Eval("Status") %>' />
                            </ItemTemplate>
                            <ItemStyle Width="50px" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="KDPartNo" HeaderText="KD零件编号" ItemStyle-Width="180px" />
                        <asp:BoundField DataField="Id" HeaderText="异常编号" ItemStyle-Width="70px" />
                        <asp:BoundField DataField="CartonNo" HeaderText="箱号" ItemStyle-Width="70px" />
                        <asp:BoundField DataField="ExceptionCode" HeaderText="异常代码" ItemStyle-Width="80px" />
                        <asp:BoundField DataField="ExceptionMessage" HeaderText="异常说明" ItemStyle-Width="260px" />
                        <asp:TemplateField HeaderText="状态" ItemStyle-Width="100px">
                            <ItemTemplate><asp:Label ID="lbl_Status" runat="server" Text='<%# GetStatusText(Eval("Status")) %>'></asp:Label></ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CreateUser" HeaderText="发生人" ItemStyle-Width="70px" />
                        <asp:BoundField DataField="CreateTime" HeaderText="发生时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-Width="140px" />
                        <asp:BoundField DataField="AuditUser" HeaderText="审核人" ItemStyle-Width="70px" />
                        <asp:BoundField DataField="AuditTime" HeaderText="审核时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-Width="140px" />
                        <asp:BoundField DataField="AuditRemark" HeaderText="审核备注" ItemStyle-Width="140px" />
                        <asp:TemplateField ItemStyle-Width="130px">
                            <HeaderTemplate><asp:DropDownList ID="ddl_BatchAction" runat="server" CssClass="review-batch-action"><asp:ListItem Value="" Text="--" /><asp:ListItem Value="1" Text="继续装箱" /><asp:ListItem Value="2" Text="取消" /><asp:ListItem Value="0" Text="暂停" /></asp:DropDownList></HeaderTemplate>
                            <ItemTemplate>
                                <asp:DropDownList ID="ddl_Action" runat="server" CssClass="row-action">
                                    <asp:ListItem Value="" Text="--" />
                                    <asp:ListItem Value="1" Text="继续装箱" />
                                    <asp:ListItem Value="2" Text="取消" />
                                    <asp:ListItem Value="0" Text="暂停" />
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div></div>
            <div class="container mt-3"><table width="100%" align="center" class="tbMessage"><tr valign="middle"><td width="10%" height="28"><div align="center"><b>提示</b></div></td><td class="msg" width="85%">&nbsp;&nbsp;<asp:Label ID="Label_Message" runat="server"></asp:Label></td></tr></table></div>
        </div></div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="JSHolder" runat="server">
    <style>
        .review-label{min-width:90px;line-height:30px}.review-check{margin-top:6px}.review-batch-action{padding:2px 4px;font-size:13px}.row-action{padding:2px 4px;font-size:13px}
    </style>
    <script type="text/javascript">
        function bindExceptionReviewGrid() {
            var gv = document.getElementById('<%=gvExceptions.ClientID%>');
            if (!gv) return;

            var headerCb = gv.querySelector('span[id$="chk_SelectAll"] > input') || gv.querySelector('input[id$="chk_SelectAll"]');
            var headerDdl = gv.querySelector('select[id$="ddl_BatchAction"]');

            if (headerCb) {
                headerCb.onclick = function () {
                    var rows = gv.querySelectorAll('input.row-select[id$="chk_Select"]');
                    for (var i = 0; i < rows.length; i++) {
                        rows[i].checked = headerCb.checked;
                    }
                };
            }

            if (headerDdl) {
                headerDdl.onchange = function () {
                    var val = headerDdl.value;
                    if (!val) return;
                    var ddls = gv.querySelectorAll('select.row-action[id$="ddl_Action"]');
                    for (var i = 0; i < ddls.length; i++) {
                        ddls[i].value = val;
                    }
                    headerDdl.value = '';
                };
            }
        }

        Sys.Application.add_load(bindExceptionReviewGrid);
    </script>
</asp:Content>