using System.Collections.Generic;
using System.Data.SqlClient;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 装箱扫描记录数据访问接口。
    /// </summary>
    public interface IPackingScanRecord
    {
        List<PackingScanRecordInfo> GetExPackingScanRecords(string supplyBatchNo, string partNo, string cartonNo);

        List<PackingScanRecordInfo> GetExPackingScanRecordsByQRCodes(string kdQRCode, string packingQRCode, string materialLabelQRCode);

        List<PackingScanRecordInfo> GetExPackingScanRecordsByPartNo(string partNo);

        ParamterInfo InsertPackingScanRecord(List<PackingScanRecordInfo> packingScanRecordInfos);

        ParamterInfo UpdatePackingScanRecord(List<PackingScanRecordInfo> packingScanRecordInfos);
        ParamterInfo DeletePackingScanRecordsByBusinessKey(string supplyBatchNo, string partNo, string deliveryNo);

        /// <summary>事务内插入扫描记录。</summary>
        bool InsertScanRecord(long packingId, string qrCodeType, string qrCode, string materialNo, int qty, string scanUser, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内汇总已扫描物料数量。</summary>
        int GetScannedMaterialQty(long packingId, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内检查二维码是否重复。</summary>
        bool CheckDuplicateQRCode(long packingId, string qrCode, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内检查二维码是否被同一装箱的其他明细使用。</summary>
        bool CheckDuplicateQRCodeExceptId(long packingId, long scanRecordId, string qrCode, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内检查随箱码是否已被其他装箱任务使用。</summary>
        bool CheckPackingQRCodeUsedByOther(long packingId, string packingQRCode, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内检查当前装箱是否已经存在随箱码明细。</summary>
        bool CheckPackingQRCodeExists(long packingId, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内查询扫描记录列表。</summary>
        List<PackingScanRecordInfo> GetScanRecordsByPackingId(long packingId, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内按 Id 查询扫描记录。</summary>
        PackingScanRecordInfo GetScanRecordById(long scanRecordId, long packingId, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内删除指定扫描明细。</summary>
        bool DeleteScanRecord(long scanRecordId, long packingId, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内更新重新装箱后的扫描明细。</summary>
        bool UpdateScanRecordForRepack(long scanRecordId, long packingId, string qrCode, string materialNo, int qty, string scanUser, SqlConnection connection, SqlTransaction transaction);
    }
}
