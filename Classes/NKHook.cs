using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BTD_Backend;
using BTD_Backend.Web;

namespace TD_Loader.Classes
{
    class NKHook
    {
        public static bool enableNKH = false;
        public static string nkhDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\NKHook5";
        public static string nkhEXE = nkhDir + "\\NKHook5-Injector.exe";
        public static string pathTowerLoadPlugin = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\NKHook5\\Plugins\\NewTowerLoader.dll";
        public string versionsURL = "https://raw.githubusercontent.com/TDToolbox/BTDToolbox-2019_LiveFIles/master/Version";
        
        public void DoInitialNKH()
        {
            if (!File.Exists(Environment.CurrentDirectory + "\\NKHook5-Injector.exe"))
            {
                if (File.Exists(NKHook.nkhEXE))
                    File.Copy(NKHook.nkhEXE, Environment.CurrentDirectory + "\\NKHook5-Injector.exe");
            }

            NKHook nkh = new NKHook();
            nkh.DoUpdateNKH();
            nkh.DoUpdateTowerPlugin();
        }
        public static bool DoesNkhExist()
        {
            if (File.Exists(nkhEXE))
            {
                return true;
            }
            return false;
        }
        public static bool CanUseNKH()
        {
            if (!enableNKH || Settings.game.GameName != "BTD5" || Settings.game == null)
                return false;

            if (!DoesNkhExist())
                return false;

            return true;
        }
        public static void LaunchNKH()
        {
            if (!DoesNkhExist())
            {
                Log.Output("Unable to find NKHook5-Injector.exe. Failed to launch...");
                return;
            }
            Log.Output("Launching NKHook");
            Process.Start(nkhEXE);
        }

        public static void OpenNkhGithub()
        {
            Log.Output("Opening NKHook Github page...");
            Process.Start("https://github.com/DisabledMallis/NKHook5");
        }
        public static void OpenMainWebsite()
        {
            string url = "https://nkhook.pro/";
            Log.Output("Opening NKHook's website...");
            Process.Start(url);
        }
        public void DoUpdateNKH()
        {
            new Thread(() =>
            {
                WebHandler web = new WebHandler();
                string nkhfolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\NKHook5";
                string savePath = nkhfolder + "\\NKHook5.zip";

                if (!web.CheckForUpdate(versionsURL, "NKHook5: ", 3, Settings.settings.NKHookVersion) && Directory.Exists(nkhfolder)
                && File.Exists(nkhfolder + "\\NKHook5.dll") && File.Exists(nkhfolder + "\\NKHook5-CLR.dll"))
                    return;

                if (File.Exists(savePath))
                    File.Delete(savePath);

                if (!Directory.Exists(nkhfolder))
                    Directory.CreateDirectory(nkhfolder);

                web.DownloadFile("NKHook5", versionsURL, savePath, "NKHook5: ", 3);


                string extractPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\NKHook5";
                ZipFile archive = new ZipFile(savePath);
                archive.ExtractAll(extractPath, ExtractExistingFileAction.OverwriteSilently);
                archive.Dispose();

                if (File.Exists(savePath))
                    File.Delete(savePath);

                if (File.Exists(Environment.CurrentDirectory + "\\NKHook5-Injector.exe"))
                    File.Delete(Environment.CurrentDirectory + "\\NKHook5-Injector.exe");

                File.Copy(nkhEXE, Environment.CurrentDirectory + "\\NKHook5-Injector.exe");
                Settings.settings.NKHookVersion = web.LatestVersionNumber;
                Settings.SaveSettings();
            }).Start();
        }
        public void DoUpdateTowerPlugin()
        {
            new Thread(() =>
            {
                WebHandler web = new WebHandler();
                string pluginPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\NKHook5\\Plugins\\NewTowerLoader.dll";
                string altPluginPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\NKHook5\\UnloadedPlugins\\NewTowerLoader.dll";
                string nkhfolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\NKHook5";

                if (!web.CheckForUpdate(versionsURL, "NKHookTowerLoader: ", 4, Settings.settings.TowerLoadNKPluginVersion) && (File.Exists(pluginPath) || File.Exists(altPluginPath)))
                    return;

                if (File.Exists(pluginPath))
                    File.Delete(pluginPath);

                if (!Directory.Exists(nkhfolder + "\\Plugins"))
                    Directory.CreateDirectory(nkhfolder + "\\Plugins");

                web.DownloadFile("NewTowerLoader.dll", versionsURL, pluginPath, "NKHookTowerLoader: ", 4);
                Settings.settings.TowerLoadNKPluginVersion = web.LatestVersionNumber;
                Settings.SaveSettings();
            }).Start();
        }
    }
}
