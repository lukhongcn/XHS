using System;
using XHS.Model;

namespace XHS.BLL
{
    public enum ScanStepHandlerType
    {
        Unknown,
        Customer,
        WorkOrder,
        Factory
    }

    /// <summary>
    /// 将流程表中的步骤配置解析为已注册的扫描处理器类型。
    /// 数据库只提供配置，不直接执行程序集中的任意方法。
    /// </summary>
    public static class ScanStepHandlerRegistry
    {
        public static ScanStepHandlerType Resolve(ScanFlowStepInfo step)
        {
            if (step == null) return ScanStepHandlerType.Unknown;

            ScanStepHandlerType byScanType = ResolveKey(step.ScanType);
            if (byScanType != ScanStepHandlerType.Unknown) return byScanType;

            return ResolveKey(step.LabelType);
        }

        private static ScanStepHandlerType ResolveKey(string key)
        {
            switch ((key ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "CUSTOMER":
                case "GSCUSTOMER":
                    return ScanStepHandlerType.Customer;
                case "WORKORDER":
                case "GSWORKORDER":
                    return ScanStepHandlerType.WorkOrder;
                case "FACTORY":
                case "GSFACTORY":
                    return ScanStepHandlerType.Factory;
                default:
                    return ScanStepHandlerType.Unknown;
            }
        }
    }
}
