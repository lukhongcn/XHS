using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Web.UI.WebControls;
using BLL;
using XHS.Model;

namespace XHS.Tests
{
    internal static class Program
    {
        private static int Main()
        {
            try
            {
                TestCustomerQRCodeTextChanged();
                TestUnRegularTableImportGetList();
                TestSearchPackingExceptions();
                TestSearchPackingExceptionsWithFilters();
                TestParseFactoryBarcodePackingPartScan();
                Console.WriteLine("All tests passed.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 1;
            }
        }

        /// <summary>
        /// 测试 txt_CustomerQRCode_TextChanged 方法，输入客户 QRCode "26J030252690"。
        /// </summary>
        private static void TestCustomerQRCodeTextChanged()
        {
            // 通过反射创建 LabelBinding 页面实例
            Assembly workflowAssembly = Assembly.Load("XHSWorkFlow");
            Type labelBindingType = workflowAssembly.GetType("ModuleWorkFlow.LabelBinding");
            object page = Activator.CreateInstance(labelBindingType);

            // 创建并设置 txt_CustomerQRCode 文本框
            TextBox txtCustomerQRCode = new TextBox();
            txtCustomerQRCode.Text = "26J030252690";

            FieldInfo txtCustomerQRCodeField = labelBindingType.GetField(
                "txt_CustomerQRCode",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            txtCustomerQRCodeField.SetValue(page, txtCustomerQRCode);

            // 确保 Label_Message 标签控件存在
            Label labelMessage = new Label();
            FieldInfo labelMessageField = labelBindingType.GetField(
                "Label_Message",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            labelMessageField.SetValue(page, labelMessage);

            // 创建其他会被 ClearFactoryScanArea 清空的控件
            FieldInfo[] fields = labelBindingType.GetFields(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (FieldInfo field in fields)
            {
                if (field.Name.StartsWith("txt_") && field.GetValue(page) == null)
                {
                    if (field.FieldType == typeof(TextBox))
                        field.SetValue(page, new TextBox());
                }

                if (field.Name.StartsWith("hid") && field.GetValue(page) == null)
                {
                    if (field.FieldType == typeof(HiddenField))
                        field.SetValue(page, new HiddenField());
                }
            }

            // 通过反射调用 protected 方法 txt_CustomerQRCode_TextChanged
            MethodInfo method = labelBindingType.GetMethod(
                "txt_CustomerQRCode_TextChanged",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            method.Invoke(page, new object[] { page, EventArgs.Empty });

            // 验证：Label_Message 应被设置
            Label actualLabel = (Label)labelMessageField.GetValue(page);
            string actualMessage = actualLabel.Text ?? string.Empty;
            AssertEqual(false, string.IsNullOrWhiteSpace(actualMessage),
                "客户 QRCode 文本变更后应设置 Label_Message。");
            Console.WriteLine("  Label_Message.Text = " + actualMessage);

            // 验证：工厂扫码区域字段应被清空
            FieldInfo factoryBarcodeField = labelBindingType.GetField(
                "txt_FactoryBarcode",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            TextBox factoryBarcode = (TextBox)factoryBarcodeField.GetValue(page);
            AssertEqual(string.Empty, factoryBarcode.Text,
                "ClearFactoryScanArea 应清空本厂条码文本框。");

            Console.WriteLine("TestCustomerQRCodeTextChanged 通过。");
        }

        private static void TestUnRegularTableImportGetList()
        {
            string rootPath = FindRootPath();
            ExecuteUpdateSql(Path.Combine(rootPath, "update.sql"));

            string fileName = Path.Combine(rootPath, "2026.6.7 7NN DX5 FL KD件标签.xlsx");
            var import = new UnRegularTableImport();
            List<string> messageList;
            List<ShippingGoodsInfo> list = import.GetList<ShippingGoodsInfo>(fileName, out messageList);

            AssertEqual(0, messageList.Count, "导入消息应为空：" + string.Join(";", messageList.ToArray()));
            AssertEqual(48, list.Count, "应解析出 48 箱数据。");

            ShippingGoodsInfo first = list[0];
            AssertEqual("7NN", first.SupplierCode, "供应商代码不正确。");
            AssertEqual("21B13A001", first.PartNo, "零件号不正确。");
            AssertEqual("后下臂总成", first.PartChineseName, "零件中文名称不正确。");
            AssertEqual("ARM COMPL,LOWER", first.PartEnglishName, "零件英文名称不正确。");
            AssertEqual(5, first.Quantity.Value, "数量不正确。");
            AssertEqual("89998-SDA141015260005", first.SupplyBatchNo, "供货批次号不正确。");
            AssertEqual(3, first.StackLayerCount.Value, "码放层数不正确。");
            AssertEqual("48-1", first.CartonNo, "第一箱纸箱编号不正确。");
            AssertEqual(15000M, first.SingleBoxGrossWeight.Value, "单箱毛重不正确。");

            ShippingGoodsInfo last = list[list.Count - 1];
            AssertEqual("48-48", last.CartonNo, "最后一箱纸箱编号不正确。");
        }

        /// <summary>
        /// 测试 PackingException.SearchPackingExceptions 两参数重载（kdCode + status）。
        /// </summary>
        private static void TestSearchPackingExceptions()
        {
            var service = new XHS.BLL.PackingException();

            // 不传任何条件，查询全部异常
            List<PackingExceptionInfo> all = service.SearchPackingExceptions(null, null);
            AssertNotNull(all, "查询全部异常不应返回 null。");
            Console.WriteLine("  SearchPackingExceptions(null, null) 返回 " + all.Count + " 条记录。");
            AssertEqual(true, all.Count >= 0, "查询全部异常应返回 0 条及以上记录。");

            // 按状态过滤：仅待处理
            List<PackingExceptionInfo> pending = service.SearchPackingExceptions(null, 0);
            AssertNotNull(pending, "按待处理状态查询不应返回 null。");
            Console.WriteLine("  SearchPackingExceptions(null, 0) 返回 " + pending.Count + " 条待处理记录。");

            // 按 KD 标签模糊搜索
            List<PackingExceptionInfo> byKd = service.SearchPackingExceptions("F26", null);
            AssertNotNull(byKd, "按 KD 标签查询不应返回 null。");
            Console.WriteLine("  SearchPackingExceptions(\"F26\", null) 返回 " + byKd.Count + " 条记录。");

            // 验证每条记录的 KDPartNo 已被解析
            foreach (PackingExceptionInfo record in all)
            {
                AssertEqual(false, record.KDPartNo == null,
                    "KDPartNo 不应为 null，记录 Id=" + (record.Id ?? -1));
            }

            Console.WriteLine("TestSearchPackingExceptions 通过。");
        }

        /// <summary>
        /// 测试 PackingException.SearchPackingExceptions 五参数重载（kdCode + partNo + dateFrom + dateTo + status）。
        /// </summary>
        private static void TestSearchPackingExceptionsWithFilters()
        {
            var service = new XHS.BLL.PackingException();

            // 无过滤条件
            List<PackingExceptionInfo> all = service.SearchPackingExceptions(null, null, null, null, null);
            AssertNotNull(all, "五参数重载：无过滤查询不应返回 null。");
            Console.WriteLine("  SearchPackingExceptions(null,null,null,null,null) 返回 " + all.Count + " 条记录。");

            // 按日期范围过滤
            DateTime from = new DateTime(2026, 8, 1);
            DateTime to = new DateTime(2026, 8, 31);
            List<PackingExceptionInfo> byDate = service.SearchPackingExceptions(null, null, from, to, null);
            AssertNotNull(byDate, "按日期范围查询不应返回 null。");
            Console.WriteLine("  SearchPackingExceptions(日期范围 2026-08-01~2026-08-31) 返回 " + byDate.Count + " 条记录。");
            // 确保日期范围内的记录 CreateTime 都在范围内
            foreach (PackingExceptionInfo record in byDate)
            {
                AssertEqual(true, record.CreateTime >= from,
                    "记录 CreateTime 应在起始日期之后，记录 Id=" + (record.Id ?? -1));
                AssertEqual(true, record.CreateTime < to.AddDays(1),
                    "记录 CreateTime 应在结束日期之前，记录 Id=" + (record.Id ?? -1));
            }

            // 按零件编号过滤
            List<PackingExceptionInfo> byPartNo = service.SearchPackingExceptions(null, "F26", null, null, null);
            AssertNotNull(byPartNo, "按零件编号查询不应返回 null。");
            Console.WriteLine("  SearchPackingExceptions(partNo=\"F26\") 返回 " + byPartNo.Count + " 条记录。");
            AssertEqual(true, byPartNo.Count > 0, "按零件编号 F26 应匹配到记录。");

            // 组合过滤：日期 + 状态
            List<PackingExceptionInfo> byDateAndStatus = service.SearchPackingExceptions(null, null, from, to, 0);
            AssertNotNull(byDateAndStatus, "组合条件查询不应返回 null。");
            Console.WriteLine("  SearchPackingExceptions(日期+待处理) 返回 " + byDateAndStatus.Count + " 条记录。");

            // 组合过滤：KD + 日期 + 状态
            List<PackingExceptionInfo> combined = service.SearchPackingExceptions("F26", "F26", from, to, null);
            AssertNotNull(combined, "KD+零件+日期组合查询不应返回 null。");
            Console.WriteLine("  SearchPackingExceptions(KD=F26, PartNo=F26, 日期范围) 返回 " + combined.Count + " 条记录。");

            Console.WriteLine("TestSearchPackingExceptionsWithFilters 通过。");
        }

        /// <summary>
        /// 测试 PackingPartScan 规则：零件标签二维码能解析，KD 标签二维码不匹配。
        /// </summary>
        private static void TestParseFactoryBarcodePackingPartScan()
        {
            var parser = new XHS.BLL.FactoryBarcodeParser();

            // 零件标签二维码：应正确解析
            string partQr = "10#F26-3301010$11#3051$12#202607290052$33#A40093051TGW005280$";
            PartInfo part = parser.ParseFactoryBarcode(partQr, "FZXHS", "PackingPartScan");
            AssertNotNull(part, "零件标签二维码应解析成功，不应返回 null。");
            AssertEqual("F26-3301010", part.JHSMaterialNo, "零件编号(MaterialNo)解析不正确。");
            AssertEqual("3051", part.SupplierCode, "供应商代码(SupplierCode)解析不正确。");
            AssertEqual("202607290052", part.ProductDate, "生产日期(ProductDate)解析不正确。");
            Console.WriteLine("  零件标签解析成功：MaterialNo=" + part.JHSMaterialNo + "，SupplierCode=" + part.SupplierCode + "，ProductDate=" + part.ProductDate);

            // 不带 33# 的零件标签二维码：33# 可选，也应正确解析
            string partQrNo33 = "10#F26-3301010$11#3051$12#202607290052$";
            PartInfo partNo33 = parser.ParseFactoryBarcode(partQrNo33, "FZXHS", "PackingPartScan");
            AssertNotNull(partNo33, "不带 33# 的零件标签二维码应解析成功，不应返回 null。");
            AssertEqual("F26-3301010", partNo33.JHSMaterialNo, "不带 33# 时零件编号(MaterialNo)解析不正确。");
            AssertEqual("3051", partNo33.SupplierCode, "不带 33# 时供应商代码(SupplierCode)解析不正确。");
            AssertEqual("202607290052", partNo33.ProductDate, "不带 33# 时生产日期(ProductDate)解析不正确。");
            Console.WriteLine("  不带 33# 的零件标签解析成功：MaterialNo=" + partNo33.JHSMaterialNo + "，SupplierCode=" + partNo33.SupplierCode + "，ProductDate=" + partNo33.ProductDate);

            // KD 标签二维码：不应匹配 PackingPartScan 规则，应返回 null
            string kdQr = "10#F26-3301010$11#3051$17#6$18#33963-CT2EV260418$19#3$20#20260706$31#10-3$230.5KG";
            PartInfo kd = parser.ParseFactoryBarcode(kdQr, "FZXHS", "PackingPartScan");
            AssertEqual(null, kd, "KD 标签二维码不应匹配 PackingPartScan 规则，应返回 null。");
            Console.WriteLine("  KD 标签二维码未匹配 PackingPartScan 规则，返回 null。");

            Console.WriteLine("TestParseFactoryBarcodePackingPartScan 通过。");
        }

        private static void ExecuteUpdateSql(string updateSqlPath)
        {
            string sql = File.ReadAllText(updateSqlPath);
            string[] batches = System.Text.RegularExpressions.Regex.Split(sql, @"(?im)^\s*GO\s*;?\s*$");
            string connectionString = ConfigurationManager.AppSettings["MsSQLConnString"];

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (string batch in batches)
                {
                    if (string.IsNullOrWhiteSpace(batch))
                    {
                        continue;
                    }

                    using (SqlCommand command = new SqlCommand(batch, connection))
                    {
                        command.CommandTimeout = 120;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static string FindRootPath()
        {
            DirectoryInfo directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "XHS.sln")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            throw new InvalidOperationException("未找到解决方案根目录。");
        }

        private static void AssertNotNull<T>(T value, string message)
        {
            if (value == null)
            {
                throw new InvalidOperationException(message + " 值为 null。");
            }
        }

        private static void AssertEqual<T>(T expected, T actual, string message)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new InvalidOperationException(message + " Expected: " + expected + ", Actual: " + actual);
            }
        }
    }
}
