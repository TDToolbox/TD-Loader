using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TD_Loader.Classes
{
    class Log
    {
        public static void Output(string output)
        {

        }
        public static void ForceOutput(string output)
        {

        }
        public static void OutputNoRepeat(string output)
        {

        }
        public static void OutputNotice(string output)
        {

        }
        public static void OutputMgbBox(string output)
        {
            MessageBox.Show(">> " + output);
        }
    }
}
