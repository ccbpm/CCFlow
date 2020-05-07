namespace FluentFTP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class FtpReflection
    {
        public static object CallMethod(this object obj, string methodName, params object[] prm)
        {
            return (from f in GetAllMethods(obj.GetType())
                where (f.Name == methodName) && (f.GetParameters().Length == prm.Length)
                select f).Single<MethodInfo>().Invoke(obj, prm);
        }

        private static IEnumerable<FieldInfo> GetAllFields(Type t)
        {
            if (t == null)
            {
                return Enumerable.Empty<FieldInfo>();
            }
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            return t.GetFields(bindingAttr).Concat<FieldInfo>(GetAllFields(t.BaseType));
        }

        private static IEnumerable<MethodInfo> GetAllMethods(Type t)
        {
            if (t == null)
            {
                return Enumerable.Empty<MethodInfo>();
            }
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            return t.GetMethods(bindingAttr).Concat<MethodInfo>(GetAllMethods(t.BaseType));
        }

        private static IEnumerable<PropertyInfo> GetAllProperties(Type t)
        {
            if (t == null)
            {
                return Enumerable.Empty<PropertyInfo>();
            }
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            return t.GetProperties(bindingAttr).Concat<PropertyInfo>(GetAllProperties(t.BaseType));
        }

        public static object GetEnumValue(this Assembly assembly, string typeName, int value)
        {
            return Enum.ToObject(assembly.GetType(typeName), value);
        }

        public static object GetField(this object obj, string fieldName)
        {
            return (from f in GetAllFields(obj.GetType())
                where f.Name == fieldName
                select f).Single<FieldInfo>().GetValue(obj);
        }

        public static object GetProperty(this object obj, string propertyName)
        {
            return (from f in GetAllProperties(obj.GetType())
                where f.Name == propertyName
                select f).Single<PropertyInfo>().GetValue(obj, null);
        }

        public static object GetStaticField(this Assembly assembly, string typeName, string fieldName)
        {
            return (from f in GetAllFields(assembly.GetType(typeName)).Where<FieldInfo>(<>c.<>9__2_0 ?? (<>c.<>9__2_0 = new Func<FieldInfo, bool>(<>c.<>9.<GetStaticField>b__2_0)))
                where f.Name == fieldName
                select f).Single<FieldInfo>().GetValue(null);
        }

        public static object InvokeStaticMethod(this Assembly assembly, string typeName, string methodName, params object[] prm)
        {
            return (from f in GetAllMethods(assembly.GetType(typeName)).Where<MethodInfo>(<>c.<>9__6_0 ?? (<>c.<>9__6_0 = new Func<MethodInfo, bool>(<>c.<>9.<InvokeStaticMethod>b__6_0)))
                where (f.Name == methodName) && (f.GetParameters().Length == prm.Length)
                select f).Single<MethodInfo>().Invoke(null, prm);
        }

        public static object NewInstance(this Assembly assembly, string typeName, params object[] prm)
        {
            return (from f in assembly.GetType(typeName).GetConstructors()
                where f.GetParameters().Length == prm.Length
                select f).Single<ConstructorInfo>().Invoke(prm);
        }

        public static void SetField(this object obj, string fieldName, object value)
        {
            (from f in GetAllFields(obj.GetType())
                where f.Name == fieldName
                select f).Single<FieldInfo>().SetValue(obj, value);
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly FtpReflection.<>c <>9 = new FtpReflection.<>c();
            public static Func<FieldInfo, bool> <>9__2_0;
            public static Func<MethodInfo, bool> <>9__6_0;

            internal bool <GetStaticField>b__2_0(FieldInfo f)
            {
                return f.IsStatic;
            }

            internal bool <InvokeStaticMethod>b__6_0(MethodInfo f)
            {
                return f.IsStatic;
            }
        }
    }
}

