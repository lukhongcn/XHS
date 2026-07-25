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

        private static void AssertEqual<T>(T expected, T actual, string message)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new InvalidOperationException(message + " Expected: " + expected + ", Actual: " + actual);
            }
        }
    }
}
