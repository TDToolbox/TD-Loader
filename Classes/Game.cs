using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace TD_Loader.Classes
{
    class Game
    {
        public static string SetGameDir(string game)
        {
            string path = "";
            
            switch (game)
            {
                case "BTD5":
                    path = Steam.GetGameDir(Steam.BTD5AppID);
                    break;
                case "BTDB":
                    path = Steam.GetGameDir(Steam.BTDBAppID);
                    break;
                case "BMC":
                    path = Steam.GetGameDir(Steam.BMCAppID);
                    break;
            }
            if (path == "" || path == null)
            {
                Log.Output("Failed to automatically find " + Settings.game.GameName);
                MessageBox.Show("Failed to automatically find " + Settings.game.GameName + " . Please browse for the game's .exe file to set the game directory");

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

                Settings.game.ModsDir = path;
                Settings.SaveGameFile();
                Settings.SaveSettings();
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
        public static string GetProcessName(string gameName)
        {
            if (gameName == "BTD5")
                return "Bloons TD5";
            if (gameName == "BTDB")
                return "Bloons TD Battles";
            if (gameName == "BMC")
                return "Bloons Monkey City";

            return "";
        }
        public static void CloseGameIfOpen(string gameName)
        {
            if (Steam.IsGameRunning(Steam.GetGameID(gameName)))
            {
                string proc = GetProcessName(gameName);
                if (!Guard.IsStringValid(proc))
                    return;

                Log.Output(gameName + " is running. Closing " + gameName + "...");
                Windows.CloseWindow(proc);
            }
        }



        //
        //Launch stuff
        //
        public static void LaunchGame()
        {
            if (!Guard.IsStringValid(Settings.game.GameName))
            {
                MessageBox.Show("Failed to get game name for game. Unable to launch");
                return;
            }

            if (NKHook.CanUseNKH())
            {
                if (MainWindow.plugin_User != null && MainWindow.plugin_User.SelectedPlugins_ListBox != null
                    && MainWindow.plugin_User.SelectedPlugins_ListBox.Items.Count > 0)
                {
                    Log.Output("Plugins are enabled. Launching NKHook");
                    Process.Start(NKHook.nkhEXE);
                    return;
                }
            }
            Process.Start(Settings.game.GameDir + "\\" + Settings.game.ExeName);
        }
        public static void DoLaunchWithMods()
        {
            if (MainWindow.mods_User.SelectedMods_ListBox.Items.Count == 0)
            {
                Log.Output("You chose to play with no mods... Launching game");
                new Thread(() =>
                {
                    if(MainWindow.resetGameFiles)
                    {
                        Thread t = new Thread(ResetGameFiles);
                        t.Start();
                        t.Join();
                    }
                    LaunchGame();
                }).Start();
                return;
            }

            MessageBox.Show("Beginning to merge mods. Please wait, this will take up to 5 seconds per mod. " +
                    "Bigger mods could take up to a minute or 2. The program is not frozen..." +
                    "\nIf you have issues with your mods, please try chaning the load order");

            MainWindow.doingWork = true;
            MainWindow.workType = "Beginning to merge mods";

            Settings.game.LoadedMods = MainWindow.mods_User.modPaths;
            Settings.SaveGameFile();
            Settings.SaveSettings();

            JetReader jet = new JetReader();
            jet.DoWork();
        }




        //
        //Backup stuff
        //
        public static void ValidateBackup()
        {
            Game game = new Game();
            if (!game.IsBackupValid())
            {
                MainWindow.workType = "Creating backup for " + Settings.game.GameName;
                CreateBackup(Settings.game.GameName);
                Log.Output("Done making backup");
            }
        }
        public bool IsBackupValid()
        {
            if (!Guard.IsStringValid(Settings.game.GameBackupDir) || !Guard.IsStringValid(Settings.game.GameName))
            {
                Settings.SetGameFile(Settings.settings.GameName);
                if (!Guard.IsStringValid(Settings.game.GameBackupDir))
                {
                    CreateBackupDir(Settings.settings.GameName);
                    if (!Guard.IsStringValid(Settings.game.GameBackupDir))
                        return false;
                }
            }

            if (!Directory.Exists(Settings.game.GameBackupDir))
                return false;


            /*var backupFiles = Directory.GetFiles(Settings.game.GameBackupDir, "*", SearchOption.AllDirectories);
            if (backupFiles.Count() < 100)  //100 is a random number. Not guarenteed but better than nothing*/
            if (!VerifyBackupOld(Settings.settings.GameName))
                return false;

            return true;
        }
        public bool VerifyBackupOld(string game)
        {
            bool valid = false;
            string backupDir = Settings.game.GameBackupDir;
            int numFiles = 0;

            switch (game)
            {
                case "BTD5":
                    numFiles = 1084;    //Originally 1454
                    break;
                case "BTDB":
                    numFiles = 689;     //Originally 690
                    break;
                case "BMC":
                    numFiles = 186;
                    break;
            }

            if (Directory.Exists(backupDir))
            {
                var backupFiles = Directory.GetFiles(backupDir, "*", SearchOption.AllDirectories);
                if (backupFiles.Count() >= numFiles)
                    valid = true;
            }
            return valid;
        }
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
                Settings.game.GameBackupDir = path;
                Settings.SaveGameFile();
                Settings.SaveSettings();
            }
            else
            {
                Log.Output("You didnt select a valid directory");
            }
        }
        
        public static async Task CreateBackupAsync(string game)
        {
            MainWindow.workType = "Creating backup";
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
                Log.Output("User chose to manually verify game. Make sure your game is un-modded or you will have issues later");
                MessageBox.Show("User chose to manually verify game. Make sure your game is un-modded or you will have issues later");
            }
            else
            {
                MainWindow.workType = "Using Steam Validator to verify game integrity (checking if the game is modded or not)";
                Log.Output("User chose to automatically verify game files");
                Steam steam = new Steam();
                bool finished = await steam.ValidateGameAsync(game);

                if (finished)
                {
                    MainWindow.workType = "Finished validating files";
                    MessageBox.Show("Finished validating files. Creating backup...");
                    Log.Output("Finished validating files. Creating backup");

                    string backupDir = Settings.game.GameBackupDir;
                    string gameDir = Settings.game.GameDir;

                    bool error = false;

                    try
                    {
                        try
                        {
                            if (Directory.Exists(backupDir))
                                Directory.Delete(backupDir, true);
                        }
                        catch { }

                        if (!Directory.Exists(backupDir))
                            Directory.CreateDirectory(backupDir);

                        if (!Guard.IsStringValid(gameDir))
                        {
                            string path = SetGameDir(game);

                            if (!Guard.IsStringValid(path))
                            {
                                error = true;
                                Log.Output("Something went wrong... Failed to aquire game directory...");
                            }
                            else
                            {
                                Log.Output("You selected " + path + " for your game directory");
                                Settings.game.GameDir = path;
                                Settings.SaveGameFile();
                            }
                        }
                        if (!error)
                        {
                            MainWindow.workType = "Copying game files to backup directory";
                            System.Threading.Thread thread = new System.Threading.Thread(() => FileIO.CopyDirsAndContents(gameDir, backupDir));
                            thread.Start();

                            while (FileIO.done != true)
                            {
                                await MainWindow.Wait(250);
                            }
                            FileIO.done = false;
                            MessageBox.Show("Done creating backup...");
                            MainWindow.workType = "";
                        }
                    }
                    catch(Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
        }
        public static bool CreateBackup(string game)
        {
            MainWindow.workType = "Creating backup";
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
                Log.Output("User chose to manually verify game. Make sure your game is un-modded or you will have issues later");
                return false;
            }
            else
            {
                MainWindow.workType = "Using Steam Validator to verify game integrity (checking if the game is modded or not)";
                Log.Output("User chose to automatically verify game files");
                Steam steam = new Steam();
                bool finished = steam.ValidateGame(game);

                if (finished)
                {
                    MainWindow.workType = "Finished validating files";
                    Log.Output("Finished validating files. Creating backup");

                    string backupDir = Settings.game.GameBackupDir;
                    string gameDir = Settings.game.GameDir;

                    bool error = false;

                    try
                    {
                        try
                        {
                            if (Directory.Exists(backupDir))
                                Directory.Delete(backupDir, true);
                        }
                        catch { }

                        if (!Directory.Exists(backupDir))
                            Directory.CreateDirectory(backupDir);

                        if (!Guard.IsStringValid(gameDir))
                        {
                            string path = SetGameDir(game);

                            if (!Guard.IsStringValid(path))
                            {
                                error = true;
                                Log.Output("Something went wrong... Failed to aquire game directory...");
                                return false;
                            }
                            else
                            {
                                Log.Output("You selected " + path + " for your game directory");
                                Settings.game.GameDir = path;
                                Settings.SaveGameFile();
                                Settings.SaveSettings();
                            }
                        }
                        if (!error)
                        {
                            MainWindow.workType = "Copying game files to backup directory";
                            System.Threading.Thread thread = new System.Threading.Thread(() => FileIO.CopyDirsAndContents(gameDir, backupDir));
                            thread.Start();

                            while (FileIO.done != true)
                            {
                                Thread.Sleep(250);
                            }
                            FileIO.done = false;
                            Log.Output("Done creating backup...");
                            MainWindow.workType = "";
                            return true;
                        }
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
                return false;
            }
        }
        public static void ResetGameFiles()
        {
            Log.Output("Resetting game directory");
            Directory.Delete(Settings.game.GameDir, true);
            FileIO.CopyDirsAndContents(Settings.game.GameBackupDir, Settings.game.GameDir);
            Log.Output("Finished cleaning game");
        }

        //
        //Game Version Stuff
        //
        /*public static string GetVersion(string game)
        {
            if(Settings.game == null)
                return "";

            string gameDir = Settings.game.GameDir;
            string exeName = GetEXEName(game);
            string exePath = gameDir + "\\" + exeName;
            string version = "";

            if (!File.Exists(exePath))
            {
                Log.Output("Game EXE not found! unable to get version");
                return ""; 
            }

            var versionInfo = FileVersionInfo.GetVersionInfo(exePath);
            version = versionInfo.FileVersion;
            
            return version;
        }*/
        public static string GetVersion(string path)
        {
            string version = "";
            if (!File.Exists(path))
            {
                Log.Output("EXE not found! unable to get version");
                return "";
            }

            var versionInfo = FileVersionInfo.GetVersionInfo(path);
            version = versionInfo.FileVersion;

            return version;
        }
        public static void WasGameUpdated()
        {
            if(!Guard.IsStringValid(Settings.game.GameDir) || !Guard.IsStringValid(Settings.game.ExeName) ||
                !Guard.IsStringValid(Settings.game.GameBackupDir))
                return;

            string gameExePath = gameExePath = Settings.game.GameDir + "\\" + Settings.game.ExeName;
            string backupExePath = Settings.game.GameBackupDir + "\\" + Settings.game.ExeName;

            string gameVersionStr = GetVersion(gameExePath);
            string backupVersionStr = GetVersion(backupExePath);

            int gameVersion = Int32.Parse(gameVersionStr.Replace(".", ""));
            int backupVersion = Int32.Parse(backupVersionStr.Replace(".", ""));

            if(backupVersion < gameVersion)
            {
                Game game = new Game();
                game.GetUpdatedGameFiles(gameVersionStr);
                return;
            }

            Settings.game.GameVersion = gameVersionStr;
            Settings.SaveGameFile();
            Settings.SaveSettings();
            return;
        }
        public void GetUpdatedGameFiles(string version)
        {
            MainWindow.workType = "Game has been updated... Reaquiring files...";
            Log.Output("Game has been updated... Reaquiring files...");

            if (!Guard.IsStringValid(Settings.game.GameBackupDir))
            {
                CreateBackupDir(Settings.game.GameName);
                return;
            }

            bool result = CreateBackup(Settings.game.GameName);
            if (!result || !IsBackupValid())
                return;

            Log.Output("Done making backup of updated game files");
            if (Settings.game.GameName == "BTDB")
            {
                Settings.settings.DidBtdbUpdate = true;
                Settings.SaveSettings();

                Zip original = new Zip(Settings.settings.BTDBBackupDir + "\\Assets\\data.jet");
                original.GetPassword();
            }

            Settings.game.GameVersion = version;
            Settings.SaveGameFile();
            Settings.SaveSettings();
        }

        //
        //Game Dir stuff
        //
        public bool DoesGameDirExist()
        {
            if (!Guard.IsStringValid(Settings.game.GameBackupDir) || !Guard.IsStringValid(Settings.game.GameDir))
                return false;

            if (Directory.Exists(Settings.game.GameDir))
                return true;

            if (!Directory.Exists(Settings.game.GameDir))
            {
                Log.Output("Game Directory Found!");
                if (Directory.Exists(Settings.game.GameBackupDir))
                {
                    Log.Output("A backup exists for the game. Copying game files to saved game directory");
                    FileIO.CopyDirsAndContents(Settings.game.GameBackupDir, Settings.game.GameDir);
                    return true;
                }
            }
            return false;
        }
        public static void ValidateGameDir()
        {
            Game game = new Game();
            if (game.DoesGameDirExist())
                return;

            string dir = SetGameDir(Settings.game.GameName);
            if (!Guard.IsStringValid(dir))
            {
                Log.Output("Something went wrong... Failed to aquire game directory...");
                MainWindow.instance.ResetGamePictures();
                return;
            }

            Settings.game.GameDir = dir;
            Settings.SaveGameFile();
            Settings.SaveSettings();
        }
    }
}
