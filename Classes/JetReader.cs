using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace TD_Loader.Classes
{
    class JetReader
    {
        /// <summary>
        /// This class combines operations to read jet file and return strings or lists of modded files, etc
        /// </summary>

        #region Constructor
        Game game;
        public JetReader()
        {
            var game = new Game();
        }

        #endregion

        #region Properties
        public static event EventHandler FinishedStagingMods;

        public List<string> Passwords { get; set; }
        public string GameName { get; set; } = Settings.settings.GameName;

        #endregion


        public void DoWork()
        {
            string backupJet = "";
            if (Settings.game.GameName == "BTD5")
                backupJet = "BTD5.jet";
            else
                backupJet = "data.jet";

            if (Directory.Exists(Settings.settings.StagingDir))
                Directory.Delete(Settings.settings.StagingDir, true);
            Directory.CreateDirectory(Settings.settings.StagingDir);

            if (!File.Exists(Settings.game.GameBackupDir + "\\Assets\\" + backupJet))
            {
                Log.Output("Failed to locate backup...");
                return;
            }
            if (!Directory.Exists(Settings.settings.StagingDir)) //Checking again incase of an error. Happens if user had this dir open when it was deleted
                Directory.CreateDirectory(Settings.settings.StagingDir);

            if (!Directory.Exists(Settings.settings.StagingDir)) //If it still doesnt exist, return.
            {
                Log.Output("Failed to find staging directory....");
                return;
            }

            File.Copy(Settings.game.GameBackupDir + "\\Assets\\" + backupJet, Settings.settings.StagingDir + "\\" + backupJet);

            if (!File.Exists(Settings.game.GameBackupDir + "\\Assets\\" + backupJet))
            {
                Log.Output("Failed to located backup file.... Returning");
                return;
            }
            
            
            List<string> reverseOrder = new List<string>();
            for (int i = Settings.game.LoadedMods.Count; i > 0; i--)
                reverseOrder.Add(Settings.game.LoadedMods[i - 1]);

            string originalPass = "";
            string moddedPass = "";
            foreach (string mod in reverseOrder)
            {
                Zip original = new Zip(Settings.game.GameBackupDir + "\\Assets\\" + backupJet);
                Zip modded = new Zip(mod);

                if (Settings.game.GameName != "BTDB")
                {
                    originalPass = Settings.game.Password;
                    moddedPass = Settings.game.Password;
                }
                else
                {
                    originalPass = original.GetPassword();
                    moddedPass = modded.GetPassword();
                }

                original.CurrentPassword = originalPass;
                original.Archive.Password = originalPass;
                modded.CurrentPassword = moddedPass;
                modded.Archive.Password = moddedPass;

                List<string> moddedFiles = new List<string>();
                moddedFiles = GetAllModdedFiles(original, modded);
                Zip staging = new Zip(Settings.settings.StagingDir + "\\" + backupJet, original.CurrentPassword);
                foreach(string file in moddedFiles)
                    staging.CopyFilesBetweenZips(modded.Archive, staging.Archive, file);
            }


            var files = new DirectoryInfo(Settings.settings.StagingDir).GetFiles("*", SearchOption.AllDirectories);
            foreach(var file in files)
            {
                if(file.Name == backupJet)
                {
                    if (File.Exists(Settings.game.GameDir + "\\Assets\\" + backupJet))
                        File.Delete(Settings.game.GameDir + "\\Assets\\" + backupJet);
                    File.Copy(file.FullName, Settings.game.GameDir + "\\Assets\\" + backupJet);
                }
                else
                {
                    if (File.Exists(Settings.game.GameDir + "\\" + file.FullName.Replace(Settings.settings.StagingDir, "")))
                        File.Delete(Settings.game.GameDir + "\\" + file.FullName.Replace(Settings.settings.StagingDir, ""));

                    File.Copy(file.FullName, Settings.game.GameDir + "\\" + file.FullName.Replace(Settings.settings.StagingDir,""));
                }
            }

            MessageBox.Show("Done staging");
            MainWindow.doingWork = false;
            
            if(FinishedStagingMods != null)
                FinishedStagingMods.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// Reads the text from the file in both zip's and compares if they are the same or different. Returns true if modded
        /// </summary>
        /// <param name="original">An object of the Zip class that is made from the original backup jet file</param>
        /// <param name="modded">An object of the Zip class that is made from the modded jet file</param>
        /// <param name="filepathInZip">the filepath to the file you want to compare. It will be the same for both jets</param>
        /// <returns>Returns whether or not file is modded</returns>
        public bool CompareFiles(Zip original, Zip modded, string filepathInZip)
        {
            if(original == null || modded == null)
            {
                Log.Output("One of the zip files you are trying to compare is invalid");
                return false;
            }

            string originalText = Regex.Replace(original.ReadFileInZip(filepathInZip, original.CurrentPassword), @"^\s*$(\n|\r|\r\n)", "", RegexOptions.Multiline).ToLower().Trim().Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\r\n", "");
            string modText = Regex.Replace(modded.ReadFileInZip(filepathInZip, modded.CurrentPassword), @"^\s*$(\n|\r|\r\n)", "", RegexOptions.Multiline).ToLower().Trim().Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\r\n", "");

            /*MessageBox.Show("Original file: " + originalText);
            MessageBox.Show("Modded file: " + modText);*/

            bool isModded = false;
            if (modText != originalText)
                isModded = true;
            
            return isModded;
        }


        public List<string> GetAllModdedFiles(Zip original, Zip modded)
        {
            List<string> moddedFiles = new List<string>();
            
            foreach(var file in modded.Archive.Entries)
            {
                if(!file.IsDirectory)
                {
                    if (CompareFiles(original, modded, file.FileName))
                        moddedFiles.Add(file.FileName);
                }
            }           

            return moddedFiles;
        }

        /// <summary>
        /// Gets password list from raw github link. Uses methods from other classes
        /// </summary>
        /// <returns>A string list of passwords</returns>
        /*public async System.Threading.Tasks.Task<List<string>> GetPasswordsList()
        {
            if (game == null)
                game = new Game();
            string result = await game.GetPasswordsListAsync();
            List<string> passwords = game.CreatePasswordsList(result);

            this.Passwords = passwords;
            return passwords;
        }*/
    }
}
