using System;
using System.Reflection;
//using System.Linq.Expressions;
namespace Utility
{
	/// <summary>
	/// Summary description for Reflector.
	/// </summary>
	public class Reflector
	{
		public Reflector()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private const BindingFlags CommonFlags = BindingFlags.Public | BindingFlags.NonPublic;

		/// <summary>
		/// 
		/// </summary>
		public static object CreateInstance(Type type, params object[] args)
		{
			return Reflector.InvokeMember(
				type, null, null, 
				Reflector.CommonFlags | BindingFlags.CreateInstance | BindingFlags.Instance, args);
		}

		/// <summary>
		/// 
		/// </summary>
		public static void SetField(object target, string fieldName, object value)
		{
			Reflector.InvokeMember(
				target.GetType(), target, fieldName, 
				Reflector.CommonFlags | BindingFlags.SetField | BindingFlags.Instance, value);
		}

		/// <summary>
		/// 
		/// </summary>
		public static object GetField(object target, string fieldName)
		{
			return Reflector.InvokeMember(
				target.GetType(), target, fieldName, 
				Reflector.CommonFlags | BindingFlags.GetField | BindingFlags.Instance);
		}

		/// <summary>
		/// 
		/// </summary>
		public static void SetProperty(object target, string propertyName, object value)
		{
           
			Reflector.InvokeMember(
				target.GetType(), target, propertyName, 
				Reflector.CommonFlags | BindingFlags.SetProperty | BindingFlags.Instance, value);
		}

		/// <summary>
		/// 
		/// </summary>
		public static object GetProperty(object target, string propertyName)
		{
			return Reflector.InvokeMember(
				target.GetType(), target, propertyName, 
				Reflector.CommonFlags | BindingFlags.GetProperty | BindingFlags.Instance);
		}

		/// <summary>
		/// 
		/// </summary>
		public static void StaticSetField(Type type, string fieldName, object value)
		{
			Reflector.InvokeMember(
				type, null, fieldName, 
				Reflector.CommonFlags | BindingFlags.SetField | BindingFlags.Static, value);
		}

		/// <summary>
		/// 
		/// </summary>
		public static object StaticGetField(Type type, string fieldName)
		{
			return Reflector.InvokeMember(
				type, null, fieldName, 
				Reflector.CommonFlags | BindingFlags.GetField | BindingFlags.Static);
		}

		/// <summary>
		/// 
		/// </summary>
		public static void StaticSetProperty(Type type, string propertyName, object value)
		{
			Reflector.InvokeMember(
				type, null, propertyName, 
				Reflector.CommonFlags | BindingFlags.SetProperty | BindingFlags.Static, value);
		}

		/// <summary>
		/// 
		/// </summary>
		public static object StaticGetProperty(Type type, string propertyName)
		{
			return Reflector.InvokeMember(
				type, null, propertyName, 
				Reflector.CommonFlags | BindingFlags.GetProperty | BindingFlags.Static);
		}

		/// <summary>
		/// 
		/// </summary>
		public static object CallMethod(object target, string methodName, params object[] args)
		{
			return Reflector.InvokeMember(
				target.GetType(), target, methodName, 
				Reflector.CommonFlags | BindingFlags.InvokeMethod | BindingFlags.Instance, args);
		}

		/// <summary>
		/// 
		/// </summary>
		public static object StaticCallMethod(Type type, string memberName, params object[] args)
		{
			return Reflector.InvokeMember(
				type, null, null, 
				Reflector.CommonFlags | BindingFlags.InvokeMethod | BindingFlags.Static, args);
		}

		/// <summary>
		/// 
		/// </summary>
		private static object InvokeMember(
			Type type, object target, string memberName, BindingFlags flags, params object[] args)
		{
			return type.InvokeMember(memberName, flags, null, target, args);
		}

        public static PropertyInfo[] getPropertyMember(object obj)
        {
            Type type = obj.GetType();
            return type.GetProperties();
        }

        public static Type getPropertyType(object obj, string propertyName)
        {
            Type objType = obj.GetType();
            PropertyInfo pi = objType.GetProperty(propertyName);

            return pi.PropertyType;
        }

        public static bool hasPropertyMember(object obj, string propertyNam)
        {
            Type objType = obj.GetType();
            PropertyInfo p = objType.GetProperty(propertyNam);
            if (p == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }



       

        //public static string PropertyName<T>(Expression<Func<T, object>> expression)
        //{
        //    var body = expression.Body as MemberExpression;

        //    if (body == null)
        //    {
        //        body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
        //    }

        //    return body.Member.Name;
        //}
	}
}
