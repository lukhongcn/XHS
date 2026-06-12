using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
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
