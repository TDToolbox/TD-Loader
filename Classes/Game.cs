using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TD_Loader.Classes
{
    class Game
    {
        public static string SetGameDir(string game)
        {
            string path = "";

            Steam steam = new Steam();
            path = steam.SearchForSteam(game);
            if (path == "" || path == null)
            {
                Log.Output("Failed to automatically find " + Settings.settings.GameName);
                MessageBox.Show("Failed to automatically find " + Settings.settings.GameName + " . Please browse for the game's .exe file to set the game directory");

                path = FileIO.BrowseForFile("Browse for .exe", "exe", "Exe files (*.exe)|*.exe|All files (*.*)|*.*", "");
            }
            return path;
        }
        public static void SetModsDir(string game)
        {
            string path = "";

            Log.Output("Would you like to use default mods directory?");
            DialogResult diag = MessageBox.Show("Mods Directory not set for " + game + "!\n\nWould you like to use the default mods directory \"Appdata/TD Loader/\"" + game + "/Mods? Press no to set your own.", "Use Default Directory?", MessageBoxButtons.YesNo);
            if(diag == DialogResult.Yes)
                path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TD Loader\\" + game + "\\Mods";
            else
            {
                Log.Output("User chose to manually set directory");
                path = FileIO.BrowseForDirectory("Choose a directory for your mods", Environment.CurrentDirectory);
            }
                
            if(path != "" && path != null)
            {
                Log.Output("You chose " + path + " as your mods directory for " + game);
                Directory.CreateDirectory(path);
                Settings.SetModsDir(game, path);
            }
            else
            {
                Log.Output("You didnt select a valid directory");
            }
        }
        public static string GetEXEName(string game)
        {
            string exeName = "";
            switch (game)
            {
                case "BTD5":
                    exeName = "BTD5-Win.exe";
                    break;
                case "BTDB":
                    exeName = "Battles-Win.exe";
                    break;
                case "BMC":
                    exeName = "MonkeyCity-Win.exe";
                    break;
            }
            return exeName;
        }

        //
        //Backup stuff
        //
        public static void CreateBackupDir(string game)
        {
            string path = "";

            DialogResult diag = MessageBox.Show("Backup Directory not set for " + game + "!\n\nWould you like to use the default mods directory \"Appdata/TD Loader/\"" + game + "/Backup? Press no to set your own.", "Use Default Directory?", MessageBoxButtons.YesNo);
            if (diag == DialogResult.Yes)
                path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TD Loader\\" + game + "\\Backup";
            else
            {
                Log.Output("User chose to manually set directory");
                path = FileIO.BrowseForDirectory("Choose a directory for your backup", Environment.CurrentDirectory);
            }

            if (path != "" && path != null)
            {
                Log.Output("You chose " + path + " as your backup directory for " + game);
                Directory.CreateDirectory(path);
                Settings.SetBackupDir(game, path);
            }
            else
            {
                Log.Output("You didnt select a valid directory");
            }
        }
        public static bool VerifyBackup(string game)
        {
            bool valid = false;
            string backupDir = Settings.GetBackupDir(game);
            int numFiles = 0;

            switch (game)
            {
                case "BTD5":
                    numFiles = 1454;
                    break;
                case "BTDB":
                    numFiles = 690;
                    break;
                case "BMC":
                    numFiles = 186;
                    break;
            }

            if (Directory.Exists(backupDir))
            {
                var backupFiles = Directory.GetFiles(backupDir, "*", SearchOption.AllDirectories);
                if (backupFiles.Length >= numFiles)
                    valid = true;
            }
            return valid;
        }       
        public static async Task CreateBackupAsync(string game)
        {
            Log.Output("Creating backup for " + game);
            DialogResult diag = MessageBox.Show("Creating a backup for " + game
                + "\n\nYour game NEEDS to be unmodded, otherwise it could cause issues." +
                " TD Loader will verify your game files to make sure they are unmodded, however" +
                " if you have extra files, for example new texture files, they will pass the verification" +
                "  and be included in the backup. Please note that this process will replace your modded" +
                " files with unmodded ones. Press YES if you understand and wish to continue, or press" +
                " NO if you want to save your mods/manually reinstall the game to" +
                " guarentee it is unmodded.", "Verify Game Files?", MessageBoxButtons.YesNo);
            if (diag == DialogResult.No)
            {
                Log.Output("User chose to manually verify game. Closing TD Loader");
                MessageBox.Show("You chose to do it manually. Closing TD Loader...");
                MainWindow.instance.Close();
            }
            else
            {
                Log.Output("User chose to automatically verify game files");
                Steam steam = new Steam();
                bool finished = await steam.ValidateGameAsync(game);

                if (finished)
                {
                    Log.Output("Finished validating files. Creating backup");

                    string backupDir = Settings.GetBackupDir(game);
                    string gameDir = Settings.GetGameDir(game);

                    bool error = false;

                    try
                    {
                        if (Directory.Exists(backupDir))
                            Directory.Delete(backupDir, true);
                    }
                    catch { }
                    Directory.CreateDirectory(backupDir);

                    if (gameDir == "" || gameDir == null)
                    {
                        string path = SetGameDir(game);

                        if (path == "" || path == null)
                        {
                            error = true;
                            Log.Output("Something went wrong... Failed to aquire game directory...");
                        }
                        else
                        {
                            Log.Output("You selected " + path + " for your game directory");
                            Settings.SetGameDir(game, path);
                        }
                    }
                    if (!error)
                    {
                        System.Threading.Thread thread = new System.Threading.Thread(() => FileIO.CopyDirsAndContents(gameDir, backupDir));
                        thread.Start();
                        
                        while (FileIO.done != true)
                        {
                            await MainWindow.Wait(250);
                        }
                        thread.Abort();
                        FileIO.done = false;
                    }
                }
            }
        }

        //
        //Game Version Stuff
        //
        public static string GetVersion(string game)
        {
            string gameDir = Settings.GetGameDir(game);
            string exeName = GetEXEName(game);
            string exePath = gameDir + "\\" + exeName;
            string version = "";

            if (File.Exists(exePath))
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(exePath);
                version = versionInfo.FileVersion;
            }
            else
            {
                Log.Output("Game EXE not found! unable to get version");
            }
            return version;
        }
    }
}
