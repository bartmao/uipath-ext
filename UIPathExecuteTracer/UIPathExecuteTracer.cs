using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UIPathExecuteTracer
{
    public class DefaultTracer : TraceListener
    {
        private Stack<string> stack = new Stack<string>();
        private int level = 0;
        private string fname;

        public DefaultTracer()
        {
            fname = Environment.CurrentDirectory + "//" + DateTime.Now.ToString("HH-mm-ss") + ".txt";
            File.WriteAllText(fname, string.Empty);
        }

        public override void Write(string message)
        {
            WriteInternal(message);
        }

        public override void WriteLine(string message)
        {
            WriteInternal(message);
        }

        private void WriteInternal(string msg)
        {
            var match = Regex.Match(msg, "\"message\":\"((?:\\w| )+) (Executing|Closed)\"", RegexOptions.Multiline);
            if (match != null && match.Groups.Count >= 2)
            {
                var v = match.Groups[1].Value;
                if (stack.Count > 0 && stack.Peek() == v)
                {
                    level--;
                    stack.Pop();
                }
                else
                {
                    //Console.WriteLine(new string('\t', level) + v);
                    File.AppendAllText(fname, new string('\t', level) + v + "\r\n");
                    level++;
                    stack.Push(v);
                }
            }
        }
    }
}
