using InvokeMethodEx;
using System;
using System.Activities.Presentation;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Linq;
using MessageBox = System.Windows.MessageBox;

namespace ActivitiesEx
{
    public partial class InvokeMethodExDesigner : ActivityDesigner
    {
        dynamic vm;

        private void ActivityDesigner_Loaded(object sender, EventArgs e)
        {
            vm = ModelItem;
        }

        private void comboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBox.Items.Count > 0)
            {
                RefreshParameters();
            }
        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("waht");
            var dlg = new OpenFileDialog();
            dlg.Filter = "Assebmly File|*.dll|Executable File|*.exe";
            var rst = dlg.ShowDialog();
            if (rst == DialogResult.OK)
            {
                ModelItem.Properties["DesignTimeAssemblyFile"].SetValue(dlg.FileName);
                ReloadAssembly();
            }
        }

        private void ReloadAssembly()
        {
            if (string.IsNullOrEmpty(vm.DesignTimeAssemblyFile))
            {
                MessageBox.Show("Please specify Assembly File");
                return;
            }

            var domain = AppDomain.CreateDomain("readInfoDomain");
            try
            {
                domain.SetData("Read_Env", ".");
                domain.SetData("Read_DllPath", "\"" + vm.DesignTimeAssemblyFile + "\"");
                // Load current assembly into the new domain
                var pathToSelf = new Uri(GetType().Assembly.CodeBase).LocalPath;
                domain.CreateInstanceFrom(pathToSelf, typeof(AssemblyLoadHelper).FullName);

                domain.DoCallBack(DomainProxy.DoCallback);
                var assemblyInfo = domain.GetData("Assembly_Info") as MyAssemblyInfo;
                if (assemblyInfo == null)
                {
                    MessageBox.Show(string.Format("Error on parsing the assembly {0}", vm.AssemblyFile));
                    return;
                }

                vm.MyAssemblyInfo = assemblyInfo;
                if (comboBox.Items.Count > 0)
                {
                    comboBox.SelectedIndex = 0;
                    RefreshParameters();
                }
            }
            finally {
                AppDomain.Unload(domain);
            }
        }

        private void RefreshParameters()
        {
            vm.SelectedMethodName = comboBox.SelectedValue.ToString();
            foreach (var m in vm.MyAssemblyInfo.Methods)
            {
                if (m.Name == vm.SelectedMethodName)
                {
                    vm.SelectedMethod = m;
                    break;
                }
            }
        }

        private class DomainProxy : MarshalByRefObject
        {
            public static void DoCallback()
            {
                try
                {
                    var curDomain = AppDomain.CurrentDomain;
                    var reader = new AssemblyReader(curDomain.GetData("Read_Env").ToString(), curDomain.GetData("Read_DllPath").ToString());
                    var info = reader.Parse();
                    curDomain.SetData("Assembly_Info", info);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
