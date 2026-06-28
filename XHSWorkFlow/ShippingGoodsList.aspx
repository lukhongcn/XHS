<%@ Page Language="c#" CodeBehind="ShippingGoodsList.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.ShippingGoodsList" MasterPageFile="~/DefaultSub.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
    <div id="Wrapper">
        <div id="Header">
            <div class="headbox">
                <div class="linebox">
                    <a href="Default.aspx">生产管理</a>
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
                                <li class="btn1"><a href="ShippingGoodsView.aspx" runat="server" title="新增/ add">新增/add</a></li>
                                <li class="btn2">
                                    <asp:LinkButton ID="lnkbutton_edit" runat="server" OnClick="lnkbutton_edit_Click" ToolTip="编辑/edit">编辑/edit</asp:LinkButton>
                                </li>
                                <li class="btn13">
                                    <asp:LinkButton ID="lnk_upload" runat="server" OnClick="lnk_upload_Click" ToolTip="上传">上传</asp:LinkButton>
                                </li>
                                <li class="btn14">
                                    <asp:LinkButton ID="lnkbutton_upload_edit" runat="server" OnClick="lnkbutton_upload_edit_Click" ToolTip="上传修改">上传修改</asp:LinkButton>
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
                            <div class="col-lg-4 d-flex">
                                <asp:Label runat="server" CssClass="me-10 shipping-goods-search-label">零件编号</asp:Label>
                                <asp:TextBox ID="TextBox_PartNo" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                            <div class="col-lg-4 d-flex">
                                <asp:Label runat="server" CssClass="me-10 shipping-goods-search-label">零件名称</asp:Label>
                                <asp:TextBox ID="TextBox_PartName" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                            <div class="col-lg-4 d-flex">
                                <asp:Label runat="server" CssClass="me-10 shipping-goods-search-label">批次</asp:Label>
                                <asp:TextBox ID="TextBox_SupplyBatchNo" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="container mt-3 border border-primary">
                    <div class="container mt-3">
                        <asp:DataGrid ID="MainDataGrid" runat="server" PageSize="20" AutoGenerateColumns="False" AllowPaging="True" CssClass="table table-striped table-bordered table-hover table-sm" OnPageIndexChanged="MainDataGrid_PageIndexChanged" OnItemDataBound="MainDataGrid_ItemDataBound">
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="table-primary"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true" CssClass="wrap-text" />
                            <Columns>
                                <asp:TemplateColumn>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chk_datagrid" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="SupplierCode" HeaderText="供应商代码"></asp:BoundColumn>
                                <asp:BoundColumn DataField="PartNo" HeaderText="零件编号"></asp:BoundColumn>
                                <asp:BoundColumn DataField="PartChineseName" HeaderText="零件中文名称"></asp:BoundColumn>
                                <asp:BoundColumn DataField="PartEnglishName" HeaderText="零件英文名称"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Quantity" HeaderText="数量"></asp:BoundColumn>
                                <asp:BoundColumn DataField="SupplyBatchNo" HeaderText="供货批次号"></asp:BoundColumn>
                                <asp:BoundColumn DataField="CartonNo" HeaderText="纸箱编号"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Status" HeaderText="状态"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="打印次数">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="HyperLink_PrintCount" runat="server"></asp:HyperLink>
                                        <asp:Label ID="Label_PrintCount" runat="server"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="SingleBoxGrossWeight" HeaderText="单箱毛重"></asp:BoundColumn>
                                <asp:BoundColumn DataField="ProductionDate" HeaderText="生产日期" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundColumn>
                                <asp:BoundColumn DataField="InspectionConfirmDate" HeaderText="检验确认日期" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Creater" HeaderText="创建人"></asp:BoundColumn>
                                <asp:BoundColumn DataField="CreatDate" HeaderText="创建时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}"></asp:BoundColumn>
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
                <div id="printRecordModalOverlay" class="print-record-modal-overlay" style="display: none;" onclick="closePrintRecordModal();"></div>
                <div id="printRecordModal" class="print-record-modal" style="display: none;">
                    <div class="print-record-modal-header">
                        <span>打印记录</span>
                        <a href="javascript:void(0);" class="print-record-modal-close" onclick="closePrintRecordModal();">关闭</a>
                    </div>
                    <div id="printRecordModalBody" class="print-record-modal-body"></div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JSHolder" runat="server">
    <style>
        .shipping-goods-search-label {
            min-width: 70px;
            line-height: 30px;
        }

        .btn14 {
            width: 48px;
            height: 53px;
            padding: 0px;
        }

        .btn14 a {
            width: 48px;
            height: 53px;
            margin: 0px;
            padding: 0px;
            display: block;
            text-decoration: none;
            text-indent: -9999px;
            background: url(images/up-edit.jpeg) no-repeat;
        }

        .btn14 a:hover {
            background: url(images/up-edit-a.jpeg) no-repeat;
        }

        .print-record-link {
            color: #1d5db5;
            text-decoration: underline;
            cursor: pointer;
        }

        .print-record-modal-overlay {
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: rgba(0, 0, 0, 0.35);
            z-index: 9998;
        }

        .print-record-modal {
            position: fixed;
            top: 50%;
            left: 50%;
            width: 1100px;
            max-width: calc(100vw - 40px);
            height: 720px;
            max-height: calc(100vh - 40px);
            transform: translate(-50%, -50%);
            background-color: #ffffff;
            border: 1px solid #7f9db9;
            z-index: 9999;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
        }

        .print-record-modal-header {
            height: 42px;
            line-height: 42px;
            padding: 0 14px;
            background-color: #d9e7f7;
            border-bottom: 1px solid #b5cde8;
            font-weight: bold;
        }

        .print-record-modal-close {
            float: right;
            color: #333333;
            text-decoration: none;
            font-weight: normal;
        }

        .print-record-modal-body {
            height: calc(100% - 42px);
            overflow: auto;
            padding: 12px;
            background-color: #ffffff;
        }

    </style>
    <script type="text/javascript">
        function openPrintRecordModal(url) {
            var overlay = document.getElementById('printRecordModalOverlay');
            var modal = document.getElementById('printRecordModal');
            var body = document.getElementById('printRecordModalBody');
            if (!overlay || !modal || !body) {
                return false;
            }

            body.innerHTML = '加载中...';
            overlay.style.display = '';
            modal.style.display = '';
            loadPrintRecordDetail(url, body);
            return false;
        }

        function closePrintRecordModal() {
            var overlay = document.getElementById('printRecordModalOverlay');
            var modal = document.getElementById('printRecordModal');
            var body = document.getElementById('printRecordModalBody');
            if (body) {
                body.innerHTML = '';
            }

            if (overlay) {
                overlay.style.display = 'none';
            }

            if (modal) {
                modal.style.display = 'none';
            }
        }

        function loadPrintRecordDetail(url, container) {
            var request = new XMLHttpRequest();
            request.open('GET', url, true);
            request.onreadystatechange = function () {
                if (request.readyState !== 4) {
                    return;
                }

                if (request.status >= 200 && request.status < 300) {
                    container.innerHTML = request.responseText;
                    return;
                }

                container.innerHTML = '<div style="padding:8px;color:#cc0000;">加载打印记录失败。</div>';
            };

            request.send(null);
        }
    </script>
</asp:Content>
