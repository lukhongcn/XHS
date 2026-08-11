<%@ Page Language="c#" CodeBehind="PackingList.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.PackingList" MasterPageFile="~/DefaultSub.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
    <div id="Wrapper">
        <div id="Header"><div class="headbox"><div class="linebox"><a href="Default.aspx">生产管理</a><img src="images/arrow.png" /><a href="#"><%=menuname %></a></div><div class="logout"><a href="login.aspx" target="_parent">登出</a></div><div class="clearbox"></div></div></div>
        <div id="Container"><div id="Content">
            <div id="Menu"><div class="menubox"><div class="mod1"><ul>
                <li class="btn2"><asp:LinkButton ID="lnkbutton_edit" runat="server" OnClick="lnkbutton_edit_Click" ToolTip="编辑/edit">编辑/edit</asp:LinkButton></li>
            </ul></div><div class="mod2"><ul>
                <li class="btn8"><asp:LinkButton ID="lnkbutton_search" runat="server" OnClick="lnkbutton_search_Click" ToolTip="搜索/search">搜索/search</asp:LinkButton></li>
            </ul></div><div class="clearbox"></div></div></div>
            <div class="space1"></div>

            <div class="container mt-3 border border-primary"><div class="container mt-3 mb-3">
                <div class="row mb-3">
                    <div class="col-lg-4 d-flex"><asp:Label ID="Label_KDQRCode" runat="server" CssClass="packing-list-search-label" AssociatedControlID="TextBox_KDQRCode">KD码</asp:Label><asp:TextBox ID="TextBox_KDQRCode" runat="server" CssClass="form-control text-start border-primary"></asp:TextBox></div>
                    <div class="col-lg-4 d-flex"><asp:Label ID="Label_PartNo" runat="server" CssClass="packing-list-search-label" AssociatedControlID="TextBox_PartNo">零件编号</asp:Label><asp:TextBox ID="TextBox_PartNo" runat="server" CssClass="form-control text-start border-primary"></asp:TextBox></div>
                    <div class="col-lg-4 d-flex"><asp:Label ID="Label_PartCode" runat="server" CssClass="packing-list-search-label" AssociatedControlID="TextBox_PartCode">零件码</asp:Label><asp:TextBox ID="TextBox_PartCode" runat="server" CssClass="form-control text-start border-primary"></asp:TextBox></div>
                </div>
            </div></div>

            <div class="container mt-3 border border-primary"><div class="container mt-3">
                <asp:DataGrid ID="MainDataGrid" runat="server" PageSize="20" AutoGenerateColumns="False" AllowPaging="True" CssClass="table table-striped table-bordered table-hover table-sm" OnPageIndexChanged="MainDataGrid_PageIndexChanged">
                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="table-primary"></HeaderStyle>
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true" CssClass="wrap-text" />
                    <Columns>
                        <asp:TemplateColumn HeaderText="箱号"><ItemTemplate><span class="packing-code" title='<%# Server.HtmlEncode(Convert.ToString(Eval("CartonNo"))) %>'><%# GetShortCode(Eval("CartonNo")) %></span></ItemTemplate></asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="KD码"><ItemTemplate><span class="packing-code" title='<%# Server.HtmlEncode(Convert.ToString(Eval("KDQRCode"))) %>'><%# GetShortCode(Eval("KDQRCode")) %></span></ItemTemplate></asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="零件编号"><ItemTemplate><span class="packing-code" title='<%# Server.HtmlEncode(Convert.ToString(Eval("PartNo"))) %>'><%# GetShortCode(Eval("PartNo")) %></span></ItemTemplate></asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="供货批次号"><ItemTemplate><span class="packing-code" title='<%# Server.HtmlEncode(Convert.ToString(Eval("SupplyBatchNo"))) %>'><%# GetShortCode(Eval("SupplyBatchNo")) %></span></ItemTemplate></asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="随箱码"><ItemTemplate><span class="packing-code" title='<%# Server.HtmlEncode(Convert.ToString(Eval("PackingQRCode"))) %>'><%# GetShortCode(Eval("PackingQRCode")) %></span></ItemTemplate></asp:TemplateColumn>
                        <asp:BoundColumn DataField="PlanQty" HeaderText="计划数量"></asp:BoundColumn>
                        <asp:BoundColumn DataField="PackingQty" HeaderText="装箱数量"></asp:BoundColumn>
                        <asp:BoundColumn DataField="PackingStage" HeaderText="装箱阶段"></asp:BoundColumn>
                        <asp:BoundColumn DataField="PackingUser" HeaderText="装箱人"></asp:BoundColumn>
                        <asp:BoundColumn DataField="PackingTime" HeaderText="装箱时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}"></asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="操作"><ItemTemplate><a href="#" class="packing-detail-link" data-packing-id="<%# Eval("Id") %>" title="明细">明细</a></ItemTemplate></asp:TemplateColumn>
                    </Columns>
                    <PagerStyle Mode="NumericPages" CssClass="table-primary"></PagerStyle>
                </asp:DataGrid>
            </div></div>

            <div class="container mt-3 border border-warning"><table width="100%" align="center" class="tbMessage"><tr valign="middle"><td width="10%" height="28"><div align="center"><b>提示</b></div></td><td class="msg" width="85%">&nbsp;&nbsp;<asp:Label ID="Label_Message" runat="server"></asp:Label></td></tr></table></div>

            <div class="packing-detail-mask" id="packingDetailModalMask" role="presentation">
                <div class="packing-detail-window" role="dialog" aria-modal="true" aria-labelledby="packingDetailModalTitle">
                    <div class="packing-detail-header"><h5 id="packingDetailModalTitle">装箱明细</h5><button type="button" class="packing-detail-close" id="packingDetailClose" aria-label="关闭">×</button></div>
                    <div class="packing-detail-body" id="packingDetailBody">正在加载明细...</div>
                </div>
            </div>
        </div></div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JSHolder" runat="server">
    <style>
        .packing-list-search-label { min-width: 75px; line-height: 30px; }
        .packing-code { display: inline-block; max-width: 180px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; vertical-align: bottom; cursor: help; }
        .packing-code-full { word-break: break-all; word-wrap: break-word; overflow-wrap: anywhere; }
        .packing-detail-table { width: 100%; table-layout: fixed; }
        .packing-detail-table th, .packing-detail-table td { vertical-align: middle; }
        .packing-detail-table td .packing-code { display: block; max-width: 100%; }
        .packing-detail-link { cursor: pointer; }
        .packing-detail-mask { position: fixed !important; top: 0 !important; right: 0 !important; bottom: 0 !important; left: 0 !important; z-index: 12000 !important; display: none; background-color: rgba(0, 0, 0, .45) !important; }
        .packing-detail-mask.packing-detail-open { display: block !important; }
        .packing-detail-window { position: absolute !important; top: 20px !important; left: 50% !important; z-index: 12001 !important; display: block !important; visibility: visible !important; width: calc(100% - 40px) !important; max-width: 1140px !important; max-height: calc(100% - 40px) !important; overflow: hidden !important; border: 1px solid #b8c8d8 !important; border-radius: 6px; background-color: #fff !important; box-shadow: 0 12px 36px rgba(0, 0, 0, .3); opacity: 1 !important; -webkit-transform: translateX(-50%); transform: translateX(-50%); }
        .packing-detail-header { display: flex; align-items: center; justify-content: space-between; padding: 12px 16px; border-bottom: 1px solid #dee2e6; background: #eef5ff; }
        .packing-detail-header h5 { margin: 0; }
        .packing-detail-close { width: 32px; height: 32px; border: 0; background: transparent; color: #333; font-size: 26px; line-height: 28px; cursor: pointer; }
        .packing-detail-body { max-height: calc(100vh - 115px); overflow: auto; padding: 16px; }
    </style>
    <script type="text/javascript">
        (function () {
            function findDetailLink(element) {
                while (element && element !== document) {
                    if ((' ' + element.className + ' ').indexOf(' packing-detail-link ') >= 0) return element;
                    element = element.parentNode;
                }
                return null;
            }

            function closeDetail() {
                document.getElementById('packingDetailModalMask').className = 'packing-detail-mask';
            }

            function addValue(container, label, value, code) {
                var wrapper = document.createElement('div');
                wrapper.className = 'col-lg-6 mb-2';
                var strong = document.createElement('b');
                strong.textContent = label + '：';
                wrapper.appendChild(strong);
                var span = document.createElement('span');
                span.textContent = value || '';
                if (code === 'full') {
                    span.className = 'packing-code-full';
                    span.title = value || '';
                } else if (code) {
                    span.className = 'packing-code';
                    span.title = value || '';
                }
                wrapper.appendChild(span);
                container.appendChild(wrapper);
            }

            function renderDetail(data) {
                var body = document.getElementById('packingDetailBody');
                body.textContent = '';
                var summary = document.createElement('div');
                summary.className = 'row';
                addValue(summary, 'KD码', data.Record.KDQRCode, 'full');
                addValue(summary, '箱号', data.Record.CartonNo, true);
                addValue(summary, '零件编号', data.Record.PartNo, true);
                addValue(summary, '供货批次号', data.Record.SupplyBatchNo, true);
                addValue(summary, '计划数量', data.Record.PlanQty, false);
                addValue(summary, '装箱数量', data.Record.PackingQty, false);
                body.appendChild(summary);

                var title = document.createElement('h6');
                title.textContent = '扫描明细';
                body.appendChild(title);
                var table = document.createElement('table');
                table.className = 'table table-bordered table-striped table-sm packing-detail-table';
                var columnWidths = ['12%', '34%', '16%', '8%', '12%', '18%'];
                var colgroup = document.createElement('colgroup');
                columnWidths.forEach(function (width) {
                    var col = document.createElement('col');
                    col.style.width = width;
                    colgroup.appendChild(col);
                });
                table.appendChild(colgroup);
                var headers = ['二维码类型', '二维码', '物料号', '数量', '扫描人', '扫描时间'];
                var thead = document.createElement('thead');
                var headerRow = document.createElement('tr');
                headers.forEach(function (header) { var cell = document.createElement('th'); cell.textContent = header; headerRow.appendChild(cell); });
                thead.appendChild(headerRow);
                table.appendChild(thead);
                var tbody = document.createElement('tbody');
                (data.ScanRecords || []).forEach(function (item) {
                    var row = document.createElement('tr');
                    [item.QRCodeType, item.QRCode, item.MaterialNo, item.Qty, item.ScanUser, item.ScanTime].forEach(function (value, index) {
                        var cell = document.createElement('td');
                        var text = value == null ? '' : value;
                        if (index === 1 || index === 2) {
                            var codeSpan = document.createElement('span');
                            codeSpan.className = 'packing-code';
                            codeSpan.textContent = text;
                            codeSpan.title = text;
                            cell.appendChild(codeSpan);
                        } else {
                            cell.textContent = text;
                        }
                        row.appendChild(cell);
                    });
                    tbody.appendChild(row);
                });
                table.appendChild(tbody);
                body.appendChild(table);
            }

            document.addEventListener('click', function (event) {
                var link = findDetailLink(event.target);
                if (!link) return;
                event.preventDefault();
                var modalMask = document.getElementById('packingDetailModalMask');
                var body = document.getElementById('packingDetailBody');
                body.textContent = '正在加载明细...';
                if (modalMask.parentNode !== document.body) document.body.appendChild(modalMask);
                modalMask.className = 'packing-detail-mask packing-detail-open';
                fetch('PackingList.aspx/GetPackingDetail', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    body: JSON.stringify({ packingId: parseInt(link.getAttribute('data-packing-id'), 10) })
                }).then(function (response) { return response.json(); }).then(function (payload) {
                    var data = payload.d || payload;
                    if (!data.Success) { body.textContent = data.Message || '加载明细失败。'; return; }
                    renderDetail(data);
                }).catch(function () { body.textContent = '加载明细失败，请稍后重试。'; });
            });

            document.getElementById('packingDetailClose').addEventListener('click', function () {
                closeDetail();
            });

            document.getElementById('packingDetailModalMask').addEventListener('click', function (event) {
                if (event.target === this) closeDetail();
            });
        }());
    </script>
</asp:Content>
