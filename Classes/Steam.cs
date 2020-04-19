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
    class Steam
    {
        bool done = false;

        //
        //Autofind steam dir stuff
        //
        public static List<string> drives = new List<string>() { "A:\\", "B:\\", "C:\\","D:\\", "E:\\", "F:\\",
            "G:\\", "H:\\", "I:\\","J:\\", "K:\\", "L:\\","M:\\", "N:\\", "O:\\","P:\\", "Q:\\", "R:\\",
            "S:\\", "T:\\", "U:\\","V:\\", "W:\\", "X:\\","Y:\\", "Z:\\"
        };
        public static List<string> paths = new List<string>() { "", "Program Files", "Program Files (x86)", "Games", "Programs" };
        public string SearchForSteam(string game)
        {
            string gameFolder = "";

            switch (game)
            {
                case "BTD5":
                    gameFolder = "BloonsTD5";
                    break;
                case "BTDB":
                    gameFolder = "Bloons TD Battles";
                    break;
                case "BMC":
                    gameFolder = "Bloons Monkey City";
                    break;
            }
            return CheckDirsForSteam(gameFolder);
        }
        public string CheckDirsForSteam(string searchFolder)
        {
            string path = "";

            //Check if Steam is in the main drive
            foreach (string drive in drives)
            {
                foreach (string p in paths)
                {
                    path = drive + p + "\\Steam";
                    if (Directory.Exists(path))
                    {
                        string finalpath = path + "\\steamapps\\common\\" + searchFolder;
                        if (Directory.Exists(finalpath))
                        {
                            return finalpath;
                        }
                    }
                }
            }
            return "";
        }

        //
        //Validator stuff
        //
        public async Task<bool> ValidateGameAsync(string game)
        {
            string url = "";

            switch (game)
            {
                case "BTD5":
                    url = "steam://validate/306020";
                    break;
                case "BTDB":
                    url = "steam://validate/444640";
                    break;
                case "BMC":
                    url = "steam://validate/1252780";
                    break;
            }

            Log.Output("Validating " + game);
            Process.Start(url);

            Thread thread = new Thread(delegate () { AwaitSteamValidate("start"); });
            thread.Start();

            while (!done)
                await Task.Delay(250);
            done = false;
            thread.Abort();

            thread = new Thread(delegate () { AwaitSteamValidate("stop"); });
            thread.Start();
            while (!done)
                await Task.Delay(250);

            done = false;
            Log.Output("Validation finished.");
            Windows.CloseWindow("Validating Steam files - 100% complete");

            return true;
        }
        private bool GetValidatorProc(string proc, string percentComplete)
        {
            var openWindowProcesses = System.Diagnostics.Process.GetProcesses()
                .Where(p => p.MainWindowHandle != IntPtr.Zero && p.ProcessName != "explorer");

            foreach (var a in openWindowProcesses)
            {
                if(a.MainWindowTitle.Contains(proc) && a.MainWindowTitle.Contains(percentComplete))
                {
                    return true;
                }
            }
            return false;
        }
        private void AwaitSteamValidate(string op)
        {
            bool result = false;
            if (op == "stop")
                result = GetValidatorProc("Validating Steam files - ", "100% complete");
            else
                result = GetValidatorProc("Validating Steam files - ", "");

            while(!result)
            {
                Thread.Sleep(250);
                AwaitSteamValidate(op);
            }
            if (result)
                done = true;
        }
    }
}
