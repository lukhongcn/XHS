<%@ Page Language="C#" CodeBehind="PackingHistory.aspx.cs" AutoEventWireup="false" OnLoad="Page_Load" Inherits="ModuleWorkFlow.PackingHistory" MasterPageFile="~/DefaultSub.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
    <div id="Wrapper">
        <div id="Header"><div class="headbox"><div class="linebox"><a href="#">生产管理</a><img src="images/arrow.png" /><a href="#"><%= menuname %></a></div><div class="logout"><a href="login.aspx" target="_parent">登出</a></div><div class="clearbox"></div></div></div>
        <div id="Container"><div id="Content">
            <div id="Menu"><div class="menubox"><div class="mod1"><ul><li class="btn5"><asp:LinkButton ID="lnk_back" runat="server" OnClick="lnk_back_Click" ToolTip="返回装箱">返回装箱</asp:LinkButton></li></ul></div><div class="clearbox"></div></div></div>
            <div class="space1"></div>
            <div class="container mt-3 border border-primary"><div class="container mt-3">
                <h5>装箱历史查询</h5>
                <div class="row mb-3"><div class="col-lg-8 d-flex"><asp:Label ID="Label_KDQRCode" runat="server" CssClass="me-10 packing-label" AssociatedControlID="txt_KDQRCode">KD标签</asp:Label><asp:TextBox ID="txt_KDQRCode" runat="server" CssClass="form-control text-start border-primary" /></div><div class="col-lg-4"><asp:Button ID="btn_Search" runat="server" Text="查询" CssClass="btn btn-primary" OnClick="btn_Search_Click" /><asp:Button ID="btn_Clear" runat="server" Text="清空" CssClass="btn btn-secondary ms-2" OnClick="btn_Clear_Click" CausesValidation="false" /></div></div>
                <asp:Label ID="Label_Message" runat="server" CssClass="text-danger"></asp:Label>
            </div></div>
            <asp:Panel ID="pnlPackingRecord" runat="server" Visible="false" CssClass="container mt-3 border border-primary"><div class="container mt-3"><h5>装箱任务</h5>
                <div class="row mb-3"><div class="col-lg-4"><strong>供货批次号</strong><asp:Label ID="litSupplyBatchNo" runat="server" CssClass="form-control packing-readonly" /></div><div class="col-lg-4"><strong>零件编号</strong><asp:Label ID="litPartNo" runat="server" CssClass="form-control packing-readonly" /></div><div class="col-lg-4"><strong>箱号</strong><asp:Label ID="litCartonNo" runat="server" CssClass="form-control packing-readonly" /></div></div>
                <div class="row mb-3"><div class="col-lg-4"><strong>KD标签</strong><asp:Label ID="litKDQRCode" runat="server" CssClass="form-control packing-readonly" /></div><div class="col-lg-4"><strong>计划数量</strong><asp:Label ID="litPlanQty" runat="server" CssClass="form-control packing-readonly" /></div><div class="col-lg-4"><strong>装箱数量</strong><asp:Label ID="litPackingQty" runat="server" CssClass="form-control packing-readonly" /></div></div>
                <div class="row mb-3"><div class="col-lg-4"><strong>装箱状态</strong><asp:Label ID="litPackingStage" runat="server" CssClass="form-control packing-readonly" /></div><div class="col-lg-4"><strong>创建人</strong><asp:Label ID="litCreateUser" runat="server" CssClass="form-control packing-readonly" /></div><div class="col-lg-4"><strong>创建时间</strong><asp:Label ID="litCreateTime" runat="server" CssClass="form-control packing-readonly" /></div></div>
            </div></asp:Panel>
            <asp:Panel ID="pnlScanRecords" runat="server" Visible="false" CssClass="container mt-3 border border-primary"><div class="container mt-3"><h5>扫描明细</h5><asp:GridView ID="gvScanRecords" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="false" EmptyDataText="暂无扫描记录。" ShowHeaderWhenEmpty="true"><Columns><asp:BoundField DataField="QRCodeType" HeaderText="二维码类型" /><asp:BoundField DataField="QRCode" HeaderText="二维码" /><asp:BoundField DataField="MaterialNo" HeaderText="物料号" /><asp:BoundField DataField="Qty" HeaderText="数量" /><asp:BoundField DataField="ScanUser" HeaderText="扫描人" /><asp:BoundField DataField="ScanTime" HeaderText="扫描时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" /></Columns></asp:GridView></div></asp:Panel>
        </div></div>
    </div>
</asp:Content>
