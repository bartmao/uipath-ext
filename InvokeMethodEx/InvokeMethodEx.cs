using InvokeMethodEx;
using System;
using System.Activities;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ActivitiesEx
{
    [Designer(typeof(InvokeMethodExDesigner))]
    public class InvokeMethodEx : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> AssemblyFile { get; set; }

        public OutArgument Result { get; set; }

        [Browsable(false)]
        public string DesignTimeAssemblyFile { get; set; }

        [Browsable(false)]
        public MyAssemblyInfo MyAssemblyInfo { get; set; }

        [Browsable(false)]
        public string SelectedMethodName { get; set; }

        [Browsable(false)]
        public MyMethodInfo SelectedMethod { get; set; }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);

            if (SelectedMethod != null)
            {
                var parameters = SelectedMethod.Parameters;
                for (var i = 0; i < parameters.Count; ++i)
                {

                    var ra = new RuntimeArgument("p" + i, parameters[i].RealType, ArgumentDirection.In);
                    metadata.Bind(parameters[i].Binding, ra);
                    metadata.AddArgument(ra);
                }

                var ra1 = new RuntimeArgument("r", typeof(string), ArgumentDirection.Out);
                metadata.Bind(Result, ra1);
                metadata.AddArgument(ra1);
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            var realLibPath = context.GetValue(AssemblyFile);
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var dir = Path.GetDirectoryName(realLibPath);
                var probeFile = Path.Combine(dir, args.Name.Split(',')[0] + ".dll");
                if (File.Exists(probeFile))
                    return Assembly.LoadFile(probeFile);
                else {
                    probeFile = Path.Combine(dir, args.Name.Split(',')[0] + ".exe");
                    if (File.Exists(probeFile))
                        return Assembly.LoadFile(probeFile);
                }
                return null;
            };

            var lib = Assembly.LoadFile(realLibPath);
            var method = lib.GetType(SelectedMethod.MyType.TypeName)?.GetMethod(SelectedMethodName);
            if (method == null)
            {
                throw new Exception("Couldn't find the right Type or Method, check the Runtime Assemly matching the one using in designer");
            }
            var rst = method.Invoke(null, SelectedMethod.Parameters.Select(p => context.GetValue(p.Binding)).ToArray());

            if (Result != null && rst != null)
                context.SetValue((Argument)Result, rst);
        }
    }
}
