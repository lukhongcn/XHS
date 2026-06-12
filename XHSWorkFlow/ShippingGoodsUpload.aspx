<%@ Page Language="c#" CodeBehind="ShippingGoodsUpload.aspx.cs" AutoEventWireup="false" Inherits="ModuleWorkFlow.ShippingGoodsUpload" MasterPageFile="~/DefaultSub.Master" %>

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
                                <li class="btn2">
                                    <asp:LinkButton ID="lnkbutton_upload" runat="server" OnClick="lnkbutton_upload_Click" ToolTip="上传/upload">上传/upload</asp:LinkButton>
                                </li>
                            </ul>
                        </div>
                        <div class="clearbox"></div>
                    </div>
                </div>

                <div class="container mt-3 border border-primary">
                    <div class="container mt-3 mb-3">
                        <asp:FileUpload ID="FileUploadShippingGoods" runat="server" Style="display: none;" />
                        <div id="dropArea" class="shipping-upload-drop border border-primary text-center">
                            <div class="shipping-upload-title">拖拉出货单到这里</div>
                            <div class="shipping-upload-subtitle">或点击选择 Excel 文件</div>
                            <asp:Label ID="Label_FileName" runat="server" CssClass="shipping-upload-file"></asp:Label>
                        </div>
                        <div class="mt-3 d-flex">
                            <asp:Button ID="btn_select" runat="server" Text="选择文件" CssClass="btn btn-outline-primary me-10" OnClientClick="openShippingGoodsFile(); return false;" />
                            <asp:Button ID="btn_upload" runat="server" Text="上传" CssClass="btn btn-primary" OnClick="btn_upload_Click" />
                        </div>
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
        .shipping-upload-drop {
            min-height: 180px;
            padding: 42px 20px;
            background-color: #f8fbff;
            cursor: pointer;
        }

        .shipping-upload-drop.dragover {
            background-color: #eaf3ff;
            border-style: dashed !important;
        }

        .shipping-upload-title {
            font-size: 18px;
            font-weight: 600;
            color: #0d6efd;
        }

        .shipping-upload-subtitle {
            margin-top: 8px;
            color: #666;
        }

        .shipping-upload-file {
            display: block;
            margin-top: 14px;
            color: #333;
        }
    </style>
    <script type="text/javascript">
        function openShippingGoodsFile() {
            document.getElementById('<%=FileUploadShippingGoods.ClientID%>').click();
        }

        (function () {
            var dropArea = document.getElementById('dropArea');
            var fileInput = document.getElementById('<%=FileUploadShippingGoods.ClientID%>');
            var fileName = document.getElementById('<%=Label_FileName.ClientID%>');

            function setFile(files) {
                if (!files || files.length === 0) {
                    return;
                }

                var dataTransfer = new DataTransfer();
                dataTransfer.items.add(files[0]);
                fileInput.files = dataTransfer.files;
                fileName.innerText = files[0].name;
            }

            dropArea.addEventListener('click', openShippingGoodsFile);
            fileInput.addEventListener('change', function () {
                setFile(fileInput.files);
            });

            dropArea.addEventListener('dragover', function (event) {
                event.preventDefault();
                dropArea.classList.add('dragover');
            });

            dropArea.addEventListener('dragleave', function () {
                dropArea.classList.remove('dragover');
            });

            dropArea.addEventListener('drop', function (event) {
                event.preventDefault();
                dropArea.classList.remove('dragover');
                setFile(event.dataTransfer.files);
            });
        })();
    </script>
</asp:Content>
