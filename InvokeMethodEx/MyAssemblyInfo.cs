using System;
using System.Activities;
using System.Collections.Generic;

namespace InvokeMethodEx
{
    public class MyAssemblyInfo
    {
        public List<MyMethodInfo> Methods { get; set; }
    }

    public class MyMethodInfo
    {
        public string Name { get; set; }

        public MyTypeInfo MyType { get; set; }

        public List<MyParameterInfo> Parameters { get; set; }

        public MyTypeInfo ReturnValue { get; set; }
    }

    public class MyParameterInfo
    {
        public string Name { get; set; }

        public MyTypeInfo MyTypeInfo { get; set; }

        public Type RealType {
            get {
                return Type.GetType(MyTypeInfo.AssemblyQualifiedName)??typeof(object);
            }
        }

        public string Hint {
            get {
                return string.Format("{0}({1})", Name, MyTypeInfo.TypeName);
            }
        }
        public InArgument Binding { get; set; }
    }

    public class MyTypeInfo
    {
        public string TypeName { get; set; }

        public string AssemblyQualifiedName { get; set; }

        public static MyTypeInfo GetInstance(Type type)
        {
            return new MyTypeInfo()
            {
                TypeName = type.FullName,
                AssemblyQualifiedName = type.AssemblyQualifiedName
            };
        }
    }
}
