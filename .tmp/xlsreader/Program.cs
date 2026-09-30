using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using ExcelDataReader;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

string filePath = @"D:\project\doc\客户\XHS\實施\光束标签整理.xlsx";
string connStr = "server=.;Pooling=false;database=XHS;uid=sa;pwd=MES2016mj";

using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
using var reader = ExcelReaderFactory.CreateReader(stream);
var dataSet = reader.AsDataSet();

var sheet1 = dataSet.Tables[0];
var sheet2 = dataSet.Tables[1];

// Build lookup from Sheet2: JHS品号 -> LabelFormat
var labelFormatMap = new Dictionary<string, string>();
for (int r = 3; r < sheet2.Rows.Count; r++)
{
    var jhsNo = sheet2.Rows[r][3]?.ToString()?.Trim();
    var labelFormat = sheet2.Rows[r][5]?.ToString()?.Trim();
    if (!string.IsNullOrWhiteSpace(jhsNo) && !string.IsNullOrWhiteSpace(labelFormat) &&
        long.TryParse(sheet2.Rows[r][0]?.ToString(), out _))
    {
        labelFormatMap[jhsNo] = labelFormat;
    }
}

using var conn = new SqlConnection(connStr);
conn.Open();

// Clear existing data
using (var cmd = new SqlCommand("DELETE FROM tb_PartMaster", conn))
{
    cmd.ExecuteNonQuery();
    Console.WriteLine("已清空旧数据。");
}

int count = 0;
for (int r = 3; r < sheet1.Rows.Count; r++)
{
    var row = sheet1.Rows[r];
    var seq = row[0]?.ToString()?.Trim();
    if (string.IsNullOrWhiteSpace(seq) || !long.TryParse(seq, out _))
        continue;

    var materialNo = row[1]?.ToString()?.Trim() ?? "";
    var materialName = row[2]?.ToString()?.Trim() ?? "";
    var jhsPartNo = row[3]?.ToString()?.Trim() ?? "";
    var processType = row[4]?.ToString()?.Trim() ?? "";
    var labelInfo = row[5]?.ToString()?.Trim() ?? "";
    var hasSteelStamp = row[6]?.ToString()?.Trim() ?? "";
    var labelFormat = labelFormatMap.TryGetValue(row[3]?.ToString()?.Trim() ?? "", out var fmt) ? fmt : "";

    if (string.IsNullOrWhiteSpace(jhsPartNo))
        continue;

    using var cmd = new SqlCommand(
        "INSERT INTO tb_PartMaster (JHSPartNo,MaterialNo,MaterialName,ProcessType,LabelInfo,HasSteelStamp,LabelFormat) " +
        "VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7)", conn);
    cmd.Parameters.AddWithValue("@p1", jhsPartNo);
    cmd.Parameters.AddWithValue("@p2", materialNo);
    cmd.Parameters.AddWithValue("@p3", materialName);
    cmd.Parameters.AddWithValue("@p4", processType);
    cmd.Parameters.AddWithValue("@p5", labelInfo);
    cmd.Parameters.AddWithValue("@p6", hasSteelStamp);
    cmd.Parameters.AddWithValue("@p7", labelFormat);
    cmd.ExecuteNonQuery();
    count++;
}

Console.WriteLine($"共导入 {count} 条记录。");
