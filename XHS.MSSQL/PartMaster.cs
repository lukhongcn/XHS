using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Utility;
using XHS.IDAL;
using XHS.Model;

namespace XHS.MSSQL
{
    /// <summary>
    /// 零件主数据 SQL Server 数据访问实现。
    /// </summary>
    public class PartMaster : IPartMaster
    {
        private const string SelectColumns =
            "PartMasterId,JHSPartNo,MaterialNo,MaterialName,ProcessType,LabelInfo,HasSteelStamp,LabelFormat,SortOrder,Remark";

        public List<PartMasterInfo> GetPartMasters()
        {
            DataSet dataSet = Data.getDataSet(
                "select " + SelectColumns + " from tb_PartMaster order by SortOrder asc");
            return BuildPartMasters(dataSet);
        }

        public PartMasterInfo GetPartMaster(int partMasterId)
        {
            List<PartMasterInfo> result = BuildPartMasters(Data.getDataSet(
                "select " + SelectColumns + " from tb_PartMaster where PartMasterId=" + partMasterId));
            return result.Count == 0 ? null : result[0];
        }

        public PartMasterInfo GetPartMasterByJHSPartNo(string jhsPartNo)
        {
            string safePartNo = (jhsPartNo ?? string.Empty).Trim().Replace("'", "''");
            List<PartMasterInfo> result = BuildPartMasters(Data.getDataSet(
                "select " + SelectColumns + " from tb_PartMaster where JHSPartNo='" + safePartNo + "'"));
            return result.Count == 0 ? null : result[0];
        }

        public ParamterInfo InsertPartMasters(List<PartMasterInfo> infos)
        {
            const string sql =
                "insert into tb_PartMaster (JHSPartNo,MaterialNo,MaterialName,ProcessType,LabelInfo,HasSteelStamp,LabelFormat,SortOrder,Remark) " +
                "values (@JHSPartNo,@MaterialNo,@MaterialName,@ProcessType,@LabelInfo,@HasSteelStamp,@LabelFormat,@SortOrder,@Remark)";
            return BuildParamterInfo(infos, sql, BuildInsertParameters);
        }

        public ParamterInfo UpdatePartMasters(List<PartMasterInfo> infos)
        {
            const string sql =
                "update tb_PartMaster set JHSPartNo=@JHSPartNo,MaterialNo=@MaterialNo,MaterialName=@MaterialName," +
                "ProcessType=@ProcessType,LabelInfo=@LabelInfo,HasSteelStamp=@HasSteelStamp,LabelFormat=@LabelFormat," +
                "SortOrder=@SortOrder,Remark=@Remark where PartMasterId=@PartMasterId";
            return BuildParamterInfo(infos, sql, BuildUpdateParameters);
        }

        public ParamterInfo DeletePartMasters(List<PartMasterInfo> infos)
        {
            const string sql = "delete from tb_PartMaster where PartMasterId=@PartMasterId";
            return BuildParamterInfo(infos, sql, info => new[]
            {
                new SqlParameter("@PartMasterId", SqlDbType.Int) { Value = ToDbValue(info.PartMasterId) }
            });
        }

        private static List<PartMasterInfo> BuildPartMasters(DataSet dataSet)
        {
            List<PartMasterInfo> result = new List<PartMasterInfo>();
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new PartMasterInfo
                {
                    PartMasterId = row.IsNull("PartMasterId") ? (int?)null : Convert.ToInt32(row["PartMasterId"]),
                    JHSPartNo = row.IsNull("JHSPartNo") ? null : Convert.ToString(row["JHSPartNo"]),
                    MaterialNo = row.IsNull("MaterialNo") ? null : Convert.ToString(row["MaterialNo"]),
                    MaterialName = row.IsNull("MaterialName") ? null : Convert.ToString(row["MaterialName"]),
                    ProcessType = row.IsNull("ProcessType") ? null : Convert.ToString(row["ProcessType"]),
                    LabelInfo = row.IsNull("LabelInfo") ? null : Convert.ToString(row["LabelInfo"]),
                    HasSteelStamp = row.IsNull("HasSteelStamp") ? null : Convert.ToString(row["HasSteelStamp"]),
                    LabelFormat = row.IsNull("LabelFormat") ? null : Convert.ToString(row["LabelFormat"]),
                    SortOrder = row.IsNull("SortOrder") ? (int?)null : Convert.ToInt32(row["SortOrder"]),
                    Remark = row.IsNull("Remark") ? null : Convert.ToString(row["Remark"])
                });
            }

            return result;
        }

        private static ParamterInfo BuildParamterInfo(
            List<PartMasterInfo> infos,
            string sql,
            Func<PartMasterInfo, SqlParameter[]> parameterBuilder)
        {
            ParamterInfo paramterInfo = new ParamterInfo
            {
                Sql = sql,
                Type = CommandType.Text,
                AlSQL = new ArrayList(),
                AlPAR = new ArrayList(),
                AlCOM = new ArrayList()
            };

            if (infos == null)
            {
                return paramterInfo;
            }

            foreach (PartMasterInfo info in infos)
            {
                if (info == null)
                {
                    continue;
                }

                paramterInfo.AlSQL.Add(sql);
                paramterInfo.AlPAR.Add(parameterBuilder(info));
                paramterInfo.AlCOM.Add(CommandType.Text);
            }

            return paramterInfo;
        }

        private static SqlParameter[] BuildInsertParameters(PartMasterInfo info)
        {
            return new[]
            {
                new SqlParameter("@JHSPartNo", SqlDbType.VarChar, 20) { Value = ToDbValue(info.JHSPartNo) },
                new SqlParameter("@MaterialNo", SqlDbType.VarChar, 50) { Value = ToDbValue(info.MaterialNo) },
                new SqlParameter("@MaterialName", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.MaterialName) },
                new SqlParameter("@ProcessType", SqlDbType.NVarChar, 20) { Value = ToDbValue(info.ProcessType) },
                new SqlParameter("@LabelInfo", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.LabelInfo) },
                new SqlParameter("@HasSteelStamp", SqlDbType.VarChar, 10) { Value = ToDbValue(info.HasSteelStamp) },
                new SqlParameter("@LabelFormat", SqlDbType.NVarChar, 10) { Value = ToDbValue(info.LabelFormat) },
                new SqlParameter("@SortOrder", SqlDbType.Int) { Value = ToDbValue(info.SortOrder) },
                new SqlParameter("@Remark", SqlDbType.NVarChar, 200) { Value = ToDbValue(info.Remark) }
            };
        }

        private static SqlParameter[] BuildUpdateParameters(PartMasterInfo info)
        {
            return new[]
            {
                new SqlParameter("@PartMasterId", SqlDbType.Int) { Value = ToDbValue(info.PartMasterId) },
                new SqlParameter("@JHSPartNo", SqlDbType.VarChar, 20) { Value = ToDbValue(info.JHSPartNo) },
                new SqlParameter("@MaterialNo", SqlDbType.VarChar, 50) { Value = ToDbValue(info.MaterialNo) },
                new SqlParameter("@MaterialName", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.MaterialName) },
                new SqlParameter("@ProcessType", SqlDbType.NVarChar, 20) { Value = ToDbValue(info.ProcessType) },
                new SqlParameter("@LabelInfo", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.LabelInfo) },
                new SqlParameter("@HasSteelStamp", SqlDbType.VarChar, 10) { Value = ToDbValue(info.HasSteelStamp) },
                new SqlParameter("@LabelFormat", SqlDbType.NVarChar, 10) { Value = ToDbValue(info.LabelFormat) },
                new SqlParameter("@SortOrder", SqlDbType.Int) { Value = ToDbValue(info.SortOrder) },
                new SqlParameter("@Remark", SqlDbType.NVarChar, 200) { Value = ToDbValue(info.Remark) }
            };
        }

        private static object ToDbValue(object value)
        {
            return value ?? DBNull.Value;
        }
    }
}
