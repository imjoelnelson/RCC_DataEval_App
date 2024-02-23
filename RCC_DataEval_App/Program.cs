﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RCC_DataEval_App
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //var test = new TestClass();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RawDataView dataView = MvpFactory.GetRawDataView();
            Application.Run(dataView);
        }
    }
}
