using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace TD_Loader.Classes
{
    class Log
    {
        public string lastMessage = "";
        private static Log log;

        public static void Output(string output) => Print(output, false, false);

        public static void Print(string output, bool canRepeat, bool Notice)
        {
            if (log == null)
                log = new Log();

            if (!canRepeat && output == log.lastMessage)
                return;


            MainWindow.instance.OutputLog.Dispatcher.BeginInvoke((Action)(() =>
            {
                    MainWindow.instance.OutputLog.AppendText(">> " + output + "\n");
            }));
        }

        public static void OutputCanRepeat(string output) => Print(output, true, false);

        public static void OutputNotice(string output) => Print(output, false, true);

        public static void OutputMgbBox(string output)
        {
            MessageBox.Show(">> " + output);
        }
    }
}
