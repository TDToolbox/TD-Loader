using BTD_Backend;
using BTD_Backend.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TD_Loader.Classes
{
    class BTD6_CrashHandler
    {
        string btd6_bootlog_path = UserData.Instance.BTD6Dir + "\\BloonsTD6_Data\\boot.config";
        string crash_report_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp\\Ninja Kiwi\\BloonsTD6\\Crashes\\";

        public void CheckForCrashes()
        {
            var checkForCrash_start = DateTime.Now.TimeOfDay;
            MessageBox.Show(checkForCrash_start.ToString("h:mm:ss tt"));
            return;
            Thread t = new Thread(() =>
            {
                Process btd6Proc;
                while (BTD_Backend.Natives.Windows.IsProgramRunning(BTD_Backend.Game.GameInfo.GetGame(BTD_Backend.Game.GameType.BTD6).ProcName, out btd6Proc))
                {
                    if (!BTD_Backend.Natives.Windows.IsProgramRunning("UnityCrashHandler64.exe", out Process crashProc))
                    {
                        Log.Output("BTD6 has crashed");
                        var result = MessageBox.Show("BTD6 has crashed! Do you want to open the crash log?", "Open crash log?", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                            OpenCrashLog();
                        break;
                    }
                    Thread.Sleep(100);
                }
            });
            BgThread.AddToQueue(t);
        }

        public void EnableCrashLog()
        {
            if (!File.Exists(btd6_bootlog_path))
            {
                Log.Output("Error! BTD6 boot.config file not found!");
                return;
            }

            string newCrashLog = "";
            var lines = File.ReadAllLines(btd6_bootlog_path);
            foreach (var line in lines)
            {
                if (!line.Contains("nolog"))
                    newCrashLog += line + "\n";
            }

            File.WriteAllText(btd6_bootlog_path, newCrashLog);
        }

        public void OpenCrashLog()
        {
            if (!Directory.Exists(crash_report_path))
            {
                Log.Output("Error! The crash folder doesn't exist!");
                return;
            }

            string dest = "";
            DateTime mostRecent = new DateTime();
            var files = Directory.GetDirectories(crash_report_path);
            foreach (var item in files)
            {
                var info = new DirectoryInfo(item);

                if (info.LastWriteTime > mostRecent)
                {
                    mostRecent = info.LastWriteTime;
                    dest = info.FullName;
                }
            }
            Process.Start(dest + "\\error.log"); 
        }
    }
}
