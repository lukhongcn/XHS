<%@ Page Language="c#" CodeBehind="PrintRecordList.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.PrintRecordList" MasterPageFile="~/DefaultSub.Master" %>

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
                        <div class="mod2">
                            <ul>
                                <li class="btn8"><asp:LinkButton ID="lnkbutton_search" runat="server" OnClick="lnkbutton_search_Click" ToolTip="搜索/search">搜索/search</asp:LinkButton></li>
                            </ul>
                        </div>
                        <div class="clearbox"></div>
                    </div>
                </div>

                <div class="space1"></div>
                <div class="container mt-3 border border-primary">
                    <div class="container mt-3 mb-3">
                        <div class="row mb-3">
                            <div class="col-lg-3 d-flex">
                                <asp:Label runat="server" CssClass="me-10 print-record-search-label" AssociatedControlID="TextBox_SupplyBatchNo">批次</asp:Label>
                                <asp:TextBox ID="TextBox_SupplyBatchNo" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                            <div class="col-lg-3 d-flex">
                                <asp:Label runat="server" CssClass="me-10 print-record-search-label" AssociatedControlID="TextBox_PartNo">零件编号</asp:Label>
                                <asp:TextBox ID="TextBox_PartNo" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                            <div class="col-lg-3 d-flex">
                                <asp:Label runat="server" CssClass="me-10 print-record-search-label" AssociatedControlID="TextBox_CartonNo">纸箱编号</asp:Label>
                                <asp:TextBox ID="TextBox_CartonNo" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                            <div class="col-lg-3 d-flex">
                                <asp:Label runat="server" CssClass="me-10 print-record-search-label" AssociatedControlID="TextBox_PrintType">打印类型</asp:Label>
                                <asp:TextBox ID="TextBox_PrintType" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
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
                                <asp:TemplateColumn HeaderText="">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="HiddenField_GroupRowId" runat="server" Value='<%# Eval("GroupRowId") %>' />
                                        <asp:Label ID="Label_Expand" runat="server" CssClass="print-record-toggle">+</asp:Label>
                                        <asp:Panel ID="Panel_Details" runat="server" CssClass="print-record-details" Style="display: none;">
                                            <table class="print-record-detail-table" cellspacing="0" cellpadding="0">
                                                <thead>
                                                    <tr>
                                                        <th>打印次序</th>
                                                        <th>打印人</th>
                                                        <th>打印时间</th>
                                                        <th>补打原因</th>
                                                        <th>状态</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <asp:Repeater ID="Repeater_PrintRecordDetails" runat="server">
                                                        <ItemTemplate>
                                                            <tr>
                                                                <td><%# Eval("SequenceText") %></td>
                                                                <td><%# Eval("PrintUser") %></td>
                                                                <td><%# Eval("PrintTimeText") %></td>
                                                                <td><%# Eval("ReprintReason") %></td>
                                                                <td><%# Eval("StatusText") %></td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </tbody>
                                            </table>
                                        </asp:Panel>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="SupplyBatchNo" HeaderText="批次"></asp:BoundColumn>
                                <asp:BoundColumn DataField="PartNo" HeaderText="零件编号"></asp:BoundColumn>
                                <asp:BoundColumn DataField="CartonNo" HeaderText="纸箱编号"></asp:BoundColumn>
                                <asp:BoundColumn DataField="PrintType" HeaderText="打印类型"></asp:BoundColumn>
                                <asp:BoundColumn DataField="RecordCount" HeaderText="记录数"></asp:BoundColumn>
                                <asp:BoundColumn DataField="LatestPrintCount" HeaderText="最新打印次数"></asp:BoundColumn>
                                <asp:BoundColumn DataField="LatestPrintUser" HeaderText="最新打印人"></asp:BoundColumn>
                                <asp:BoundColumn DataField="LatestPrintTimeText" HeaderText="最新打印时间"></asp:BoundColumn>
                                <asp:BoundColumn DataField="LatestReprintReason" HeaderText="最新补打原因"></asp:BoundColumn>
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
    <style>
        .print-record-search-label {
            min-width: 70px;
            line-height: 30px;
        }

        .print-record-toggle {
            display: inline-block;
            min-width: 14px;
            color: #000000;
            font-weight: normal;
            font-size: 16px;
            line-height: 1;
            cursor: pointer;
            border: none;
            background: transparent;
            text-decoration: none;
        }

        .print-record-toggle:hover {
            color: #000000;
        }

        .print-record-details {
            text-align: left;
            min-width: 320px;
            display: none;
        }

        .print-record-detail-table {
            width: 100%;
            border-collapse: collapse;
            background-color: #ffffff;
        }

        .print-record-detail-table th,
        .print-record-detail-table td {
            border: 1px solid #d9d9d9;
            padding: 6px 8px;
            text-align: center;
            white-space: nowrap;
        }

        .print-record-detail-table th {
            background-color: #d9e7f7;
            font-weight: bold;
        }

        .print-record-detail-row td {
            background-color: #f9f9f9;
            text-align: left;
            padding: 8px 12px;
        }
    </style>
    <script type="text/javascript">
        function togglePrintRecordGroup(groupRowId, control) {
            if (!control) {
                return;
            }

            var row = control;
            while (row && row.tagName !== 'TR') {
                row = row.parentNode;
            }

            if (!row) {
                return;
            }

            var details = null;
            var panels = row.getElementsByTagName('div');
            for (var i = 0; i < panels.length; i++) {
                if (panels[i].className && panels[i].className.indexOf('print-record-details') >= 0) {
                    details = panels[i];
                    break;
                }
            }

            if (!details) {
                return;
            }

            var nextRow = row.nextSibling;
            while (nextRow && nextRow.nodeType !== 1) {
                nextRow = nextRow.nextSibling;
            }

            var detailRow = null;
            if (nextRow && nextRow.getAttribute && nextRow.getAttribute('data-detail-row') === groupRowId) {
                detailRow = nextRow;
            }

            if (detailRow) {
                detailRow.parentNode.removeChild(detailRow);
                control.innerText = '+';
                return;
            }

            detailRow = document.createElement('tr');
            detailRow.className = 'print-record-detail-row';
            detailRow.setAttribute('data-detail-row', groupRowId);

            var detailCell = document.createElement('td');
            detailCell.colSpan = row.cells.length;
            detailCell.innerHTML = details.innerHTML;
            detailRow.appendChild(detailCell);

            if (row.nextSibling) {
                row.parentNode.insertBefore(detailRow, row.nextSibling);
            } else {
                row.parentNode.appendChild(detailRow);
            }

            control.innerText = '-';
        }
    </script>
</asp:Content>
