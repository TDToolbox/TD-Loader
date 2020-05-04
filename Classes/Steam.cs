using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace TD_Loader.Classes
{
    class Steam
    {

        bool done = false;

        

        public const UInt64 BTD6AppID = 960090;
        public const string BTD6Name = "BloonsTD6";

        public const UInt64 BTD5AppID = 306020;
        public const string BTD5Name = "BloonsTD5";

        public const UInt64 BTDBAppID = 444640;
        public const string BTDBName = "Bloons TD Battles";

        public const UInt64 BMCAppID = 1252780;
        public const string BMCName = "Bloons Monkey City";

        private static Dictionary<UInt64, string> steamGames = new Dictionary<UInt64, string>
        {{BTD6AppID, BTD6Name}, {BTD5AppID, BTD5Name}, {BTDBAppID, BTDBName}, {BMCAppID, BMCName}};


        private class Utils
        {
            // Takes any quotation marks out of a string.
            public static string StripQuotes(string str)
            {
                return str.Replace("\"", "");
            }

            public static string UnixToWindowsPath(string UnixPath)
            {
                return UnixPath.Replace("/", "\\");
            }
        }

        public static bool IsGameRunning(UInt64 appid)
        {
            int isGameRunning = (int)Registry.GetValue(Registry.CurrentUser + 
                "\\Software\\Valve\\Steam\\Apps\\" + appid, "Running", null);
            if (isGameRunning == 1) // Cant type true because its of type System.Bool.
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetGameDir(UInt64 appid)
        {
            return GetGameDir(appid, steamGames[appid]);
        }

        public static string GetGameDir(UInt64 appid, string gameName)
        {


            string steamDir = (string)Registry.GetValue(Registry.CurrentUser + 
                "\\Software\\Valve\\Steam", "SteamPath", null);
            if (steamDir == null)
            {
                Log.Output("Failed to find steam in registry!");
                return null;
            }

            //
            // Check if game is installed first
            //

            int isGameInstalled = (int)Registry.GetValue(Registry.CurrentUser + 
                "\\Software\\Valve\\Steam\\Apps\\" + appid, "Installed", null);
            if (isGameInstalled != 1)
            {
                Log.Output(gameName + " is not installed!");
                return null;
            }

            //
            // Get game Directory...
            //

            string configFileDir = steamDir + "\\steamapps\\libraryfolders.vdf";
            List<string> SteamLibDirs = new List<string>();
            SteamLibDirs.Add(Utils.UnixToWindowsPath(steamDir)); // This steam Directory is always here.
            string[] configFile = File.ReadAllLines(configFileDir);
            for (int i = 0; i < configFile.Length; i++)
            {
                // To Example lines are
                // 	"ContentStatsID"		"-4535501642230800231"
                // "1"     "C:\\SteamLibrary"
                // So, we scan for the items in quotes, if the first one is numeric, 
                // then the second one will be a steam library.
                Regex reg = new Regex("\".*?\"");
                MatchCollection matches = reg.Matches(configFile[i]);
                for (int match = 0; match < matches.Count; match++)
                {
                    if (match == 0)
                    {

                        if (int.TryParse(Utils.StripQuotes(matches[match].Value.ToString()), out int n))
                        {
                            // We dont actually need N, we just need to check if the value is an integer.
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (match == 1)
                    {
                        SteamLibDirs.Add(Utils.UnixToWindowsPath(Utils.StripQuotes(matches[match].Value.ToString())));
                    }
                }
            }
            for (int i = 0; i < SteamLibDirs.Count; i++)
            {
                string GameFolder = SteamLibDirs[i] + "\\steamapps\\common\\" + gameName;
                if (Directory.Exists(GameFolder))
                {
                    Log.Output("Found " + gameName + " directory at: " + GameFolder);
                    return GameFolder;
                }
            }

            Log.Output(gameName + "'s Directory not found!");
            return null;
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
            Process validationProc = Process.Start(url);

            Thread thread = new Thread(delegate () { SteamValidate("stop"); });
            thread.Start();
            while (!done)
                await Task.Delay(250);

            done = false;
            Log.Output("Validation finished.");
            //validationProc.Kill();
            Windows.CloseWindow("100%");    //For all the Mallis's out there whose PC is in Italian

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
        private void SteamValidate(string op)
        {
            bool result = false;
            if (op == "stop")
                result = GetValidatorProc("Validating Steam files - ", "100% complete");
            else
                result = GetValidatorProc("Validating Steam files - ", "");

            while(!result)
            {
                Thread.Sleep(250);
                SteamValidate(op);
            }
            if (result)
                done = true;
        }
    }
}
