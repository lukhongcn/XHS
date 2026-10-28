using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using XHS.IDAL;
using XHS.Model;

namespace XHS.BLL
{
    /// <summary>配送单平台上传记录业务层。</summary>
    public class DeliveryUploadRecord
    {
        private readonly IDeliveryUploadRecord dal;

        public DeliveryUploadRecord()
        {
            dal = XHS.DALFactory.DeliveryUploadRecord.Create();
        }

        public long CreateUploadRecord(DeliveryUploadRecordInfo info)
        {
            if (info == null) return 0L;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["MsSQLConnString"]))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            long id = dal.InsertUploadRecord(info, connection, transaction);
                            transaction.Commit();
                            return id;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch
            {
                return 0L;
            }
        }

        public bool CompleteUploadRecord(DeliveryUploadRecordInfo info)
        {
            if (info == null || !info.Id.HasValue || info.Id.Value <= 0) return false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["MsSQLConnString"]))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            bool result = dal.UpdateUploadResult(info, connection, transaction);
                            transaction.Commit();
                            return result;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public List<DeliveryUploadRecordInfo> GetUploadRecords(long packingId)
        {
            if (packingId <= 0) return new List<DeliveryUploadRecordInfo>();
            try
            {
                return dal.GetUploadRecords(packingId) ?? new List<DeliveryUploadRecordInfo>();
            }
            catch
            {
                return new List<DeliveryUploadRecordInfo>();
            }
        }
    }
}
