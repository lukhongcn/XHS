
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using XHS.service;

namespace ModuleWorkFlow
{
    /// <summary>
    /// 通用 API 执行器。
    /// 不包含 PDA 专用业务逻辑。
    /// 目标方法必须由可信的 API 注册器预先授权。
    /// </summary>
    internal sealed class PDAWorkFlowApiHandlers : IApiHandler
    {
        private readonly object service;
        private readonly MethodInfo method;
        private readonly ParameterInfo[] parameters;
        private readonly string outputName;
        private readonly MethodInfo getRequiredMethod;

        public PDAWorkFlowApiHandlers(
            object service,
            MethodInfo method,
            string outputName)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            if (method == null)
                throw new ArgumentNullException("method");

            if (!method.IsPublic ||
                method.IsStatic ||
                method.ContainsGenericParameters ||
                method.DeclaringType == null ||
                !method.DeclaringType.IsInstanceOfType(service))
            {
                throw new ArgumentException(
                    "API 方法签名不受支持。");
            }

            this.service = service;
            this.method = method;
            this.outputName = string.IsNullOrWhiteSpace(outputName)
                ? "result"
                : outputName;

            parameters = method.GetParameters();

            if (parameters.Any(p =>
                p.IsOut ||
                p.ParameterType.IsByRef ||
                p.ParameterType.IsPointer ||
                p.IsOptional))
            {
                throw new ArgumentException(
                    "API 包含不支持的参数类型。");
            }

            getRequiredMethod = typeof(ApiExecutionContext)
                .GetMethods(BindingFlags.Public |
                            BindingFlags.Instance)
                .Single(m =>
                    m.Name == "GetRequired" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType ==
                        typeof(string));
        }

        public ApiExecutionResult Execute(
            ApiExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            object[] args = new object[parameters.Length];

            // 自动根据 BLL 方法签名读取输入
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];

                MethodInfo getter = getRequiredMethod
                    .MakeGenericMethod(parameter.ParameterType);

                try
                {
                    args[i] = getter.Invoke(
                        context,
                        new object[] { parameter.Name });
                }
                catch (TargetInvocationException ex)
                {
                    RethrowInner(ex);
                    throw;
                }
            }

            object result;

            try
            {
                // 调用预先注册、已授权的 BLL 方法
                result = method.Invoke(service, args);
            }
            catch (TargetInvocationException ex)
            {
                RethrowInner(ex);
                throw;
            }

            return new ApiExecutionResult()
                .Add(outputName, result);
        }

        private static void RethrowInner(
            TargetInvocationException ex)
        {
            if (ex.InnerException != null)
            {
                ExceptionDispatchInfo
                    .Capture(ex.InnerException)
                    .Throw();
            }
        }
    }
}
