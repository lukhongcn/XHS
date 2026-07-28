using XHS.Model;

namespace XHS.BLL
{
    /// <summary>
    /// 装箱操作统一返回结果。
    /// </summary>
    public class PackingOperationResult
    {
        /// <summary>操作是否成功。</summary>
        public bool Success { get; set; }

        /// <summary>状态：NORMAL / LOCK / STALE / COMPLETED / ERROR。</summary>
        public string Status { get; set; }

        /// <summary>提示消息。</summary>
        public string Message { get; set; }

        /// <summary>操作后生成的新 Token。</summary>
        public string NewToken { get; set; }

        /// <summary>操作后的装箱记录。</summary>
        public PackingRecordInfo PackingRecord { get; set; }

        /// <summary>成功结果。</summary>
        public static PackingOperationResult Ok(string message, string newToken, PackingRecordInfo record)
        {
            return new PackingOperationResult
            {
                Success = true,
                Status = "NORMAL",
                Message = message,
                NewToken = newToken,
                PackingRecord = record
            };
        }

        /// <summary>已完成结果。</summary>
        public static PackingOperationResult Completed(string message, string newToken, PackingRecordInfo record)
        {
            return new PackingOperationResult
            {
                Success = true,
                Status = "COMPLETED",
                Message = message,
                NewToken = newToken,
                PackingRecord = record
            };
        }

        /// <summary>异常锁定结果。</summary>
        public static PackingOperationResult Lock(string message, string newToken, PackingRecordInfo record)
        {
            return new PackingOperationResult
            {
                Success = false,
                Status = "LOCK",
                Message = message,
                NewToken = newToken,
                PackingRecord = record
            };
        }

        /// <summary>Token 过期结果。</summary>
        public static PackingOperationResult Stale(string message)
        {
            return new PackingOperationResult
            {
                Success = false,
                Status = "STALE",
                Message = message
            };
        }

        /// <summary>错误结果。</summary>
        public static PackingOperationResult Error(string message)
        {
            return new PackingOperationResult
            {
                Success = false,
                Status = "ERROR",
                Message = message
            };
        }
    }
}
