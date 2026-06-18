<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Page Language="c#" CodeBehind="ShippingGoodsView.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.ShippingGoodsView" MasterPageFile="~/DefaultSub.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true"></asp:ScriptManager>
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
                                <li class="btn5">
                                    <asp:LinkButton ID="lnk_view" runat="server" OnClick="lnk_view_Click" ToolTip="浏览">浏览</asp:LinkButton>
                                </li>
                            </ul>
                        </div>
                        <div class="mod2">
                            <ul>
                                <li class="btn3">
                                    <asp:LinkButton ID="lnkbutton_save" runat="server" ToolTip="保存/save" OnClick="lnkbutton_save_Click">保存/save</asp:LinkButton>
                                </li>
                            </ul>
                        </div>
                        <div class="clearbox"></div>
                    </div>
                </div>

                <div class="space1"></div>
                <div class="container mt-3 border border-primary">
                    <div class="container mt-3">
                        <div class="row mb-3">
                            <div class="col-lg-12 d-flex">
                                <asp:Label ID="Label_Barcode" runat="server" CssClass="me-10 shipping-goods-label" AssociatedControlID="txt_barcode">扫描条码</asp:Label>
                                <asp:TextBox ID="txt_barcode" runat="server" CssClass="form-control text-start border-primary shipping-goods-barcode" AutoPostBack="true" OnTextChanged="txt_barcode_TextChanged"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_SupplierCode" runat="server" CssClass="me-10 shipping-goods-label" AssociatedControlID="txt_SupplierCode">供应商代码</asp:Label>
                                <asp:TextBox ID="txt_SupplierCode" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_PartNo" runat="server" CssClass="me-10 shipping-goods-label" AssociatedControlID="txt_PartNo">零件编号</asp:Label>
                                <asp:TextBox ID="txt_PartNo" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_PartChineseName" runat="server" CssClass="me-10 shipping-goods-label" AssociatedControlID="txt_PartChineseName">零件中文名称</asp:Label>
                                <asp:TextBox ID="txt_PartChineseName" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_PartEnglishName" runat="server" CssClass="me-10 shipping-goods-label" AssociatedControlID="txt_PartEnglishName">零件英文名称</asp:Label>
                                <asp:TextBox ID="txt_PartEnglishName" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_Quantity" runat="server" CssClass="me-10 shipping-goods-label" AssociatedControlID="txt_Quantity">数量</asp:Label>
                                <asp:TextBox ID="txt_Quantity" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_SupplyBatchNo" runat="server" CssClass="me-10 shipping-goods-label" AssociatedControlID="txt_SupplyBatchNo">供货批次号</asp:Label>
                                <asp:TextBox ID="txt_SupplyBatchNo" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_StackLayerCount" runat="server" CssClass="me-10 shipping-goods-label" AssociatedControlID="txt_StackLayerCount">码放层数</asp:Label>
                                <asp:TextBox ID="txt_StackLayerCount" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_BoxCount" runat="server" CssClass="me-10 shipping-goods-label" AssociatedControlID="txt_BoxCount">纸箱数量</asp:Label>
                                <asp:TextBox ID="txt_BoxCount" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_ProductionDate" runat="server" CssClass="me-10 shipping-goods-label" AssociatedControlID="txt_ProductionDate">生产日期</asp:Label>
                                <asp:TextBox ID="txt_ProductionDate" runat="server" TextMode="DateTimeLocal" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_InspectionConfirmDate" runat="server" CssClass="me-10 shipping-goods-label" AssociatedControlID="txt_InspectionConfirmDate">检验确认日期</asp:Label>
                                <asp:TextBox ID="txt_InspectionConfirmDate" runat="server" TextMode="DateTimeLocal" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-6 d-flex">
                                <asp:Label ID="Label_SingleBoxGrossWeight" runat="server" CssClass="me-10 shipping-goods-label" AssociatedControlID="txt_SingleBoxGrossWeight">单箱毛重</asp:Label>
                                <asp:TextBox ID="txt_SingleBoxGrossWeight" runat="server" CssClass="form-control custom-heighter-width text-start border-primary"></asp:TextBox>
                            </div>
                            <div class="col-lg-6 d-flex"></div>
                        </div>

                        <asp:HiddenField ID="hid_Id" runat="server" />
                        <asp:HiddenField ID="hid_CartonNo" runat="server" />
                        <asp:HiddenField ID="hid_QrCode" runat="server" />
                        <asp:HiddenField ID="hid_Status" runat="server" />
                        <asp:HiddenField ID="hid_PrintCount" runat="server" />
                        <asp:HiddenField ID="hid_Creater" runat="server" />
                        <asp:HiddenField ID="hid_CreatDate" runat="server" />
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
        .shipping-goods-label {
            min-width: 110px;
            line-height: 30px;
        }

        .shipping-goods-barcode {
            width: 100%;
        }

        .shipping-goods-readonly {
            background-color: #f3f3f3;
        }
    </style>
    <script type="text/javascript">
        function focusShippingGoodsBarcode() {
            var barcodeInput = document.getElementById('<%=txt_barcode.ClientID%>');
            if (barcodeInput) {
                barcodeInput.focus();
                barcodeInput.select();
            }
        }

        function shippingGoodsBarcodeKeyDown(event) {
            event = event || window.event;
            var keyCode = event.which || event.keyCode;
            if (keyCode === 13) {
                if (event.preventDefault) {
                    event.preventDefault();
                }
                __doPostBack('<%=txt_barcode.UniqueID%>', '');
                return false;
            }
            return true;
        }

        window.setTimeout(focusShippingGoodsBarcode, 0);
    </script>
</asp:Content>





