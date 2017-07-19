using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AssemblyReader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var assPath = @"C:\Users\bmao002\Documents\Visual Studio 2015\Projects\UIPathSolution\Func\bin\Debug\Func.dll";
            //args = new string[] {
            //    "\""+@"."+"\""
            //    , "\""+@"C:\Users\bmao002\Documents\Visual Studio 2015\Projects\RDPTests\Functions\bin\Debug\Functions.dll"+"\""
            //};

            if (args.Length < 2) return;
            var appPath = args[0].Trim('"');
            var assemblyPath = args[1].Trim('"');

            var info = new MyAssemblyInfo() {
                Methods = new List<MyMethodInfo>()
            };
            if (!Path.IsPathRooted(assemblyPath))
            {
                assemblyPath = Path.Combine(appPath, assemblyPath);
            }

            try
            {
                var lib = Assembly.LoadFrom(assemblyPath);
                foreach (var type in lib.GetTypes())
                {
                    foreach (var method in type.GetMethods())
                    {
                        if (method.IsStatic)
                            info.Methods.Add(new MyMethodInfo()
                            {
                                Name = method.Name,
                                MyType = MyTypeInfo.GetInstance(type),
                                ReturnValue = MyTypeInfo.GetInstance(method.ReturnType),
                                Parameters = method.GetParameters().Select(p =>
                                    new MyParameterInfo
                                    {
                                        Name = p.Name,
                                        MyTypeInfo = MyTypeInfo.GetInstance(p.ParameterType)
                                    }).ToList()
                            });
                    }
                }

                using (var stream = new MemoryStream())
                {
                    var serializer = new DataContractJsonSerializer(typeof(MyAssemblyInfo));
                    serializer.WriteObject(stream, info);
                    Console.WriteLine(UTF8Encoding.UTF8.GetString(stream.ToArray()));
                }
            }
            catch (Exception)
            {
                
            }
        }

    }

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
    }

    public class MyTypeInfo
    {
        public string TypeName { get; set; }

        public string AssemblyQualifiedName { get; set; }

        public static MyTypeInfo GetInstance(Type type) {
            return new MyTypeInfo()
            {
                TypeName = type.FullName,
                AssemblyQualifiedName = type.AssemblyQualifiedName
            };
        }
    }
}
