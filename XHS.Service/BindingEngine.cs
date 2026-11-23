using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XHS.service
{
    public sealed class ExpressionResolver
    {
        public object Resolve(object expression, IDictionary<string, object> scopes)
        {
            string text = expression as string;
            if (text == null || !text.StartsWith("$", StringComparison.Ordinal))
            {
                return expression;
            }

            return ResolvePath(text, scopes);
        }

        private object ResolvePath(string expression, IDictionary<string, object> scopes)
        {
            int position = 1;
            string rootName = ReadName(expression, ref position);
            object current;
            if (rootName.Length == 0 || !TryGetDictionaryValue(scopes, rootName, out current))
            {
                throw new ApiExecutionException("表达式根节点不存在：" + expression);
            }

            while (position < expression.Length)
            {
                if (expression[position] == '.')
                {
                    position++;
                    string memberName = ReadName(expression, ref position);
                    if (memberName.Length == 0)
                    {
                        throw new ApiExecutionException("表达式成员名称为空：" + expression);
                    }

                    current = ReadMember(current, memberName, expression);
                }
                else if (expression[position] == '[')
                {
                    int close = FindBracketClose(expression, position);
                    string token = expression.Substring(position + 1, close - position - 1).Trim();
                    object key = ResolveBracketToken(token, scopes);
                    current = ReadIndexedValue(current, key, expression);
                    position = close + 1;
                }
                else
                {
                    throw new ApiExecutionException("表达式格式无效：" + expression);
                }
            }

            return current;
        }

        private object ResolveBracketToken(string token, IDictionary<string, object> scopes)
        {
            if (token.StartsWith("$", StringComparison.Ordinal))
            {
                return ResolvePath(token, scopes);
            }

            if ((token.StartsWith("\"", StringComparison.Ordinal) && token.EndsWith("\"", StringComparison.Ordinal))
                || (token.StartsWith("'", StringComparison.Ordinal) && token.EndsWith("'", StringComparison.Ordinal)))
            {
                return token.Substring(1, token.Length - 2);
            }

            int index;
            return int.TryParse(token, out index) ? (object)index : token;
        }

        private static int FindBracketClose(string expression, int start)
        {
            int depth = 0;
            for (int i = start; i < expression.Length; i++)
            {
                if (expression[i] == '[') depth++;
                if (expression[i] == ']')
                {
                    depth--;
                    if (depth == 0) return i;
                }
            }

            throw new ApiExecutionException("表达式缺少右方括号：" + expression);
        }

        private static string ReadName(string expression, ref int position)
        {
            int start = position;
            while (position < expression.Length
                && expression[position] != '.'
                && expression[position] != '[')
            {
                position++;
            }

            return expression.Substring(start, position - start);
        }

        private static object ReadMember(object source, string memberName, string expression)
        {
            if (source == null)
            {
                throw new ApiExecutionException("表达式读取到空对象：" + expression);
            }

            object value;
            IDictionary<string, object> genericDictionary = source as IDictionary<string, object>;
            if (genericDictionary != null && TryGetDictionaryValue(genericDictionary, memberName, out value))
            {
                return value;
            }

            IDictionary dictionary = source as IDictionary;
            if (dictionary != null)
            {
                foreach (DictionaryEntry entry in dictionary)
                {
                    if (string.Equals(Convert.ToString(entry.Key), memberName, StringComparison.OrdinalIgnoreCase))
                    {
                        return entry.Value;
                    }
                }
            }

            PropertyInfo property = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(item => item.CanRead
                    && string.Equals(item.Name, memberName, StringComparison.OrdinalIgnoreCase)
                    && item.GetIndexParameters().Length == 0);
            if (property != null) return property.GetValue(source, null);

            throw new ApiExecutionException("表达式成员不存在：" + expression);
        }

        private static object ReadIndexedValue(object source, object key, string expression)
        {
            if (source == null)
            {
                throw new ApiExecutionException("表达式索引对象为空：" + expression);
            }

            int index;
            IList list = source as IList;
            if (list != null && int.TryParse(Convert.ToString(key), out index))
            {
                if (index < 0 || index >= list.Count)
                {
                    throw new ApiExecutionException("表达式数组索引越界：" + expression);
                }

                return list[index];
            }

            object value;
            IDictionary<string, object> genericDictionary = source as IDictionary<string, object>;
            if (genericDictionary != null
                && TryGetDictionaryValue(genericDictionary, Convert.ToString(key), out value))
            {
                return value;
            }

            IDictionary dictionary = source as IDictionary;
            if (dictionary != null)
            {
                foreach (DictionaryEntry entry in dictionary)
                {
                    if (string.Equals(
                        Convert.ToString(entry.Key),
                        Convert.ToString(key),
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return entry.Value;
                    }
                }
            }

            throw new ApiExecutionException("表达式索引不存在：" + expression);
        }

        internal static bool TryGetDictionaryValue(
            IDictionary<string, object> dictionary,
            string key,
            out object value)
        {
            if (dictionary.TryGetValue(key, out value)) return true;
            foreach (KeyValuePair<string, object> item in dictionary)
            {
                if (string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    value = item.Value;
                    return true;
                }
            }

            value = null;
            return false;
        }
    }

    public sealed class ConditionEvaluator
    {
        private readonly ExpressionResolver resolver;

        public ConditionEvaluator(ExpressionResolver resolver)
        {
            if (resolver == null) throw new ArgumentNullException("resolver");
            this.resolver = resolver;
        }

        public bool Evaluate(ConditionDefinition condition, IDictionary<string, object> scopes)
        {
            if (condition == null) return true;
            string operation = !string.IsNullOrWhiteSpace(condition.Op)
                ? condition.Op.Trim()
                : (condition.Operator ?? string.Empty).Trim();
            if (operation.Length == 0)
            {
                throw new ApiConfigurationException("条件缺少 operator。");
            }

            if (string.Equals(operation, "and", StringComparison.OrdinalIgnoreCase))
            {
                return RequireConditions(condition).All(item => Evaluate(item, scopes));
            }

            if (string.Equals(operation, "or", StringComparison.OrdinalIgnoreCase))
            {
                return RequireConditions(condition).Any(item => Evaluate(item, scopes));
            }

            if (string.Equals(operation, "not", StringComparison.OrdinalIgnoreCase))
            {
                List<ConditionDefinition> conditions = RequireConditions(condition);
                if (conditions.Count != 1) throw new ApiConfigurationException("not 条件只能包含一个子条件。");
                return !Evaluate(conditions[0], scopes);
            }

            object leftExpression = condition.Left ?? condition.Value;
            object left = resolver.Resolve(leftExpression, scopes);
            if (string.Equals(operation, "isNull", StringComparison.OrdinalIgnoreCase))
            {
                return left == null || left == DBNull.Value;
            }

            if (string.Equals(operation, "notNull", StringComparison.OrdinalIgnoreCase))
            {
                return left != null && left != DBNull.Value;
            }

            if (string.Equals(operation, "notEmpty", StringComparison.OrdinalIgnoreCase))
            {
                return left != null && left != DBNull.Value && GetCount(left) > 0;
            }

            object right = resolver.Resolve(condition.Right, scopes);
            if (string.Equals(operation, "equals", StringComparison.OrdinalIgnoreCase))
            {
                return AreEqual(left, right);
            }

            if (string.Equals(operation, "notEquals", StringComparison.OrdinalIgnoreCase))
            {
                return !AreEqual(left, right);
            }

            if (string.Equals(operation, "greaterThan", StringComparison.OrdinalIgnoreCase))
            {
                return Compare(left, right) > 0;
            }

            if (string.Equals(operation, "countEquals", StringComparison.OrdinalIgnoreCase))
            {
                return GetCount(left) == Convert.ToInt32(right);
            }

            throw new ApiConfigurationException("不支持的条件 operator：" + operation);
        }

        private static List<ConditionDefinition> RequireConditions(ConditionDefinition condition)
        {
            if (condition.Conditions == null || condition.Conditions.Count == 0)
            {
                throw new ApiConfigurationException(condition.Operator + " 条件缺少子条件。");
            }

            return condition.Conditions;
        }

        private static bool AreEqual(object left, object right)
        {
            if (left == null || left == DBNull.Value) return right == null || right == DBNull.Value;
            if (right == null || right == DBNull.Value) return false;
            decimal leftNumber;
            decimal rightNumber;
            if (decimal.TryParse(Convert.ToString(left), out leftNumber)
                && decimal.TryParse(Convert.ToString(right), out rightNumber))
            {
                return leftNumber == rightNumber;
            }

            return string.Equals(
                Convert.ToString(left),
                Convert.ToString(right),
                StringComparison.OrdinalIgnoreCase);
        }

        private static int Compare(object left, object right)
        {
            decimal leftNumber;
            decimal rightNumber;
            if (decimal.TryParse(Convert.ToString(left), out leftNumber)
                && decimal.TryParse(Convert.ToString(right), out rightNumber))
            {
                return leftNumber.CompareTo(rightNumber);
            }

            return string.Compare(
                Convert.ToString(left),
                Convert.ToString(right),
                StringComparison.OrdinalIgnoreCase);
        }

        private static int GetCount(object value)
        {
            if (value == null) return 0;
            ICollection collection = value as ICollection;
            if (collection != null) return collection.Count;
            IEnumerable enumerable = value as IEnumerable;
            if (enumerable == null) throw new ApiExecutionException("countEquals 的左值不是集合。");
            int count = 0;
            foreach (object ignored in enumerable) count++;
            return count;
        }
    }

    public sealed class BindingExecutionContext
    {
        public BindingExecutionContext()
        {
            Scopes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            TrustedContext = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, object> Scopes { get; private set; }
        public IDictionary<string, object> TrustedContext { get; private set; }
        public IApiPermissionEvaluator PermissionEvaluator { get; set; }
    }

    public sealed class ActionDispatchResult
    {
        internal ActionDispatchResult(
            string action,
            IDictionary<string, object> outputs,
            IDictionary<string, object> steps)
        {
            Action = action;
            Outputs = outputs;
            Steps = steps;
        }

        public string Action { get; private set; }
        public IDictionary<string, object> Outputs { get; private set; }
        public IDictionary<string, object> Steps { get; private set; }

        public T Get<T>(string name)
        {
            object value;
            if (!Outputs.TryGetValue(name, out value) || value == null) return default(T);
            if (value is T) return (T)value;
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
