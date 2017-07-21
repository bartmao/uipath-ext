using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ActivitiesEx
{

    public class AssemblyLoadHelper : MarshalByRefObject
    {
        private readonly string _myAssemblyName;

        public AssemblyLoadHelper()
        {
            // cached for efficiency (probably not needed)
            _myAssemblyName = typeof(AssemblyLoadHelper).Assembly.FullName;

            AppDomain.CurrentDomain.AssemblyResolve += HandleAssemblyResolve;
        }

        private Assembly HandleAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name == _myAssemblyName)
            {
                return typeof(AssemblyLoadHelper).Assembly;
            }

            return null;
        }
    }
}
