using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ionic.Zip;
using System.Windows.Forms;
using System.Diagnostics;

namespace TD_Loader.Classes
{
    class UpdateHandler
    {
        WebClient client = new WebClient();
        WebHandler reader;

        public bool reinstall { get; set; }
        string toolbox_updater_zipName = "TD Loader Updater.zip";
        string gitURL = "https://raw.githubusercontent.com/TDToolbox/BTDToolbox-2019_LiveFIles/master/Version";

        public void HandleUpdates()
        {
            if (!reinstall)
            {
                if (CheckForUpdates())
                {
                    if (AskToUpdate())
                    {
                        DownloadUpdate();
                        ExtractUpdate();
                        LaunchUpdate();
                    }
                    else
                    { Log.Output("Update cancelled..."); }
                }
                else
                { Log.Output("TD Loader is up to date!"); }
            }
            else
            {
                DownloadUpdate();
                ExtractUpdate();
                LaunchUpdate();
            }

        }
        private bool CheckForUpdates()
        {
            reader = new WebHandler();
            try
            {
                return reader.CheckForUpdate(gitURL, "TDLoader: ", 5, Settings.settings.TDLoaderVersion);
            }
            catch
            {
                Log.OutputNotice("Something went wrong when checking for updates.. Failed to check for updates");
                return false;
            }

        }
        private void DownloadUpdate()
        {
            reader = new WebHandler();
            string git_Text = reader.WaitOn_URL(gitURL);
            string updaterURL = reader.processGit_Text(git_Text, "toolbox2019_updater: ", 1);

            Log.Output("Downloading TD Loader Updater");
            client.DownloadFile(updaterURL, "Update");

            if (File.Exists(toolbox_updater_zipName))
                File.Delete(toolbox_updater_zipName);
            File.Move("Update", toolbox_updater_zipName);
            Log.Output("Updater successfully downloaded!");
        }
        private void ExtractUpdate()
        {
            string zipPath = Environment.CurrentDirectory + "\\" + toolbox_updater_zipName;
            string extractedFilePath = Environment.CurrentDirectory;
            Log.Output("Extracting updater...");
            ZipFile archive = new ZipFile(zipPath);
            foreach (ZipEntry e in archive)
            {
                e.Extract(extractedFilePath, ExtractExistingFileAction.DoNotOverwrite);
            }
            archive.Dispose();
            Log.Output("TD Loader updater has been successfully downloaded and extracted...");
        }
        private bool AskToUpdate()
        {
            Log.Output("There is a new update availible for TD Loader! Do you want to download it?");
            DialogResult result = MessageBox.Show("There is a new update availible for TD Loader! Do you want to download it?", "Update TD Loader?", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                return true;
            }
            else
                return false;
        }

        private void LaunchUpdate()
        {
            Log.Output("TD Loader needs to close in order to update..");
            MessageBox.Show("Closing TD Loader to continue update...");

            //save config real quick
            Settings.settings.RecentUpdate = true;
            Settings.SaveSettings();

            Process p = new Process();
            p.StartInfo.Arguments = "-lineNumber:2 -url:https://raw.githubusercontent.com/TDToolbox/BTDToolbox-2019_LiveFIles/master/Updater_launch%20parameters";
            //p.StartInfo.Arguments = "-fileName:BTD_Toolbox -processName:BTDToolbox -exeName:BTDToolbox.exe -updateZip_Name:BTDToolbox_Updater.zip -ignoreFiles:BTDToolbox_Updater,Backups,DotNetZip,.json -deleteFiles:BTDToolbox_Updater.zip,Update -url:https://raw.githubusercontent.com/TDToolbox/BTDToolbox-2019_LiveFIles/master/Version -replaceText:toolbox2019: -lineNumber:0";
            p.StartInfo.FileName = Environment.CurrentDirectory + "\\BTDToolbox_Updater.exe";
            p.Start();
            Process.GetCurrentProcess().Kill();
        }
    }
}
