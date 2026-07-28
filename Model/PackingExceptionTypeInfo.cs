using System;

namespace XHS.Model
{
    public class PackingExceptionTypeInfo
    {
        public int Id { get; set; }

        public string ExceptionCode { get; set; }

        public string ExceptionName { get; set; }

        public bool IsLock { get; set; }

        public bool Enable { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
