<%@ Page Language="C#" CodeBehind="ScanFlowStep.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.ScanFlowStepPage" MasterPageFile="~/DefaultSub.Master" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-3">
        <h4>扫描流程步骤</h4>
        <div class="mb-3">
            <asp:Label ID="labFlowId" runat="server" AssociatedControlID="txtFlowId" Text="FlowId"></asp:Label>
            <asp:TextBox ID="txtFlowId" runat="server" CssClass="form-control" />
            <asp:Button ID="btnSearch" runat="server" Text="查询" CssClass="btn btn-primary mt-2" OnClick="btnSearch_Click" />
        </div>
        <asp:Label ID="labMessage" runat="server" CssClass="text-danger"></asp:Label>
        <asp:GridView ID="gvSteps" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="false" EmptyDataText="暂无流程步骤。">
            <Columns>
                <asp:BoundField DataField="StepNo" HeaderText="步骤序号" />
                <asp:BoundField DataField="StepCode" HeaderText="步骤编码" />
                <asp:BoundField DataField="StepName" HeaderText="步骤名称" />
                <asp:BoundField DataField="ScanType" HeaderText="扫描类型" />
                <asp:BoundField DataField="RuleName" HeaderText="规则名称" />
                <asp:BoundField DataField="CustomerId" HeaderText="客户" />
                <asp:BoundField DataField="LabelType" HeaderText="标签类型" />
                <asp:CheckBoxField DataField="AllowRepeat" HeaderText="允许重复扫描" />
                <asp:BoundField DataField="EndControlId" HeaderText="结束条件控件" />
                <asp:BoundField DataField="EndCompareControlId" HeaderText="结束比对控件" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
