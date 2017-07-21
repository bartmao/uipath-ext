using InvokeMethodEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ActivitiesEx
{
    public class AssemblyReader
    {
        public string Env { get; set; }

        public string DllPath { get; set; }

        public AssemblyReader(string env, string dllPath)
        {
            Env = env.Trim('\"');
            DllPath = dllPath.Trim('\"');
        }

        public MyAssemblyInfo Parse() {
            var info = new MyAssemblyInfo()
            {
                Methods = new List<MyMethodInfo>()
            };

            if (!Path.IsPathRooted(DllPath))
            {
                DllPath = Path.Combine(Env, DllPath);
            }

            try
            {
                var lib = Assembly.LoadFrom(DllPath);
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
            }
            catch (Exception)
            {

            }

            return info;
        }
    }
}
