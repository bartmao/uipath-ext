using InvokeMethodEx;
using System;
using System.Activities.Presentation;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Forms;
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
            if (!File.Exists("c:\\windows\\AssemblyReader.exe"))
            {
                MessageBox.Show("Not found AssemblyReader under Windows folder");
                return;
            }

            var psf = new ProcessStartInfo();
            psf.FileName = @"c:\\windows\\AssemblyReader.exe";
            psf.Arguments = ". \"" + vm.DesignTimeAssemblyFile + "\"";
            psf.UseShellExecute = false;
            psf.CreateNoWindow = true;
            psf.WindowStyle = ProcessWindowStyle.Hidden;
            psf.RedirectStandardOutput = true;
            var p = Process.Start(psf);
            var json = p.StandardOutput.ReadToEnd();

            if (string.IsNullOrEmpty(json))
            {
                MessageBox.Show(string.Format("Error on parsing the assembly {0}", vm.AssemblyFile));
                return;
            }

            MyAssemblyInfo assemblyInfo = null;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(MyAssemblyInfo));
                assemblyInfo = serializer.ReadObject(stream) as MyAssemblyInfo;
            }
            vm.MyAssemblyInfo = assemblyInfo;
            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
                RefreshParameters();
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
    }
}
