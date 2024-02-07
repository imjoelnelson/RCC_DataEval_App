using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCC_DataEval_App
{
    public class TestClass
    {
        public TestClass()
        {
            Stopwatch stop = new Stopwatch();
            stop.Start();
            string path = "C:\\Users\\Joel Nelson\\Desktop\\AAA test RCCs";
            IEnumerable<string> files = Directory.EnumerateFiles(path, "*.RCC");
            Dictionary<string, Rlf> rlfs = new Dictionary<string, Rlf>();
            List<Rcc> rccs = files.Select(x => new Rcc(x, rlfs)).ToList();
            stop.Stop();
            Console.WriteLine($"Time = {stop.ElapsedMilliseconds}\r\n");
        }
    }
}
