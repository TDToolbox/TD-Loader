using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD_Loader.Classes
{
    class Settings
    {
        public static SettingsFile settings;

        public static string settingsFileName = "settings.json";
        public static string settingsPath = Environment.CurrentDirectory + "\\" + settingsFileName;

        public class SettingsFile
        {
            public string TDLoaderVersion { get; set; }
            public string GameName { get; set; }


            public string BTD5Dir { get; set; }
            public string BTD5Version { get; set; }
            public string BTD5BackupDir { get; set; }
            public string BTD5ModsDir { get; set; }
            public List<string> BTD5LoadedMods { get; set; }


            public string BTDBDir { get; set; }
            public string BTDBVersion { get; set; }
            public string BTDBBackupDir { get; set; }
            public string BTDBModsDir { get; set; }
            public List<string> BTDBLoadedMods { get; set; }


            public string BMCDir { get; set; }
            public string BMCVersion { get; set; }
            public string BMCBackupDir { get; set; }
            public string BMCModsDir { get; set; }
            public List<string> BMCLoadedMods { get; set; }


            public bool AdvancedConflictMenu { get; set; }
        }
        public class GameFile
        {
            public string GameName { get; set; }
            public string GameVersion { get; set; }
            public string GameDir { get; set; }
            public string GameBackupDir { get; set; }
            public string ModsDir { get; set; }
            public List<string> LoadedMods { get; set; }
        }
        public static void CreateSettings()
        {
            settings = new SettingsFile();
            settings.TDLoaderVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
            settings.GameName = "";


            settings.BTD5Dir = "";
            settings.BTD5Version = "";
            settings.BTD5BackupDir = "";
            settings.BTD5ModsDir = "";
            settings.BTD5LoadedMods = new List<string>();


            settings.BTDBDir = "";
            settings.BTDBVersion = "";
            settings.BTDBBackupDir = "";
            settings.BTDBModsDir = "";
            settings.BTDBLoadedMods = new List<string>();


            settings.BMCDir = "";
            settings.BMCVersion = "";
            settings.BMCBackupDir = "";
            settings.BMCModsDir = "";
            settings.BMCLoadedMods = new List<string>();


            settings.AdvancedConflictMenu = false;

            SaveSettings();
        }
        public static SettingsFile LoadSettings()
        {
            if (!File.Exists(settingsPath))
                CreateSettings();
            string json = File.ReadAllText(settingsPath);
            if (JSON.IsJsonValid(json))
            {
                settings = JsonConvert.DeserializeObject<SettingsFile>(json);
            }
            else
            {
                Log.Output("Settings file has invalid json, generating a new settings file.");
                CreateSettings();
            }
            return settings;
        }
        public static void SaveSettings()
        {
            string output = JsonConvert.SerializeObject(settings, Formatting.Indented);
            
            if (settingsPath != "" && settingsPath != null)
            {
                StreamWriter serialize = new StreamWriter(settingsPath, false);
                serialize.Write(output);
                serialize.Close();
            }
            else
            {
                Log.OutputNotice("Unknown error occured... Path to settings is invalid...");
            }
        }
        public static void SetGameDir(string game, string path)
        {
            switch (game)
            {
                case "BTD5":
                    settings.BTD5Dir = path;
                    break;
                case "BTDB":
                    settings.BTDBDir = path;
                    break;
                case "BMC":
                    settings.BMCDir = path;
                    break;
            }
            SaveSettings();
        }
        public static string GetGameDir(string game)
        {
            string gameDir = "";
            switch (game)
            {
                case "BTD5":
                    gameDir = settings.BTD5Dir;
                    break;
                case "BTDB":
                    gameDir = settings.BTDBDir;
                    break;
                case "BMC":
                    gameDir = settings.BMCDir;
                    break;
            }
            return gameDir;
        }

        public static void SetBackupDir(string game, string path)
        {
            switch (game)
            {
                case "BTD5":
                    settings.BTD5BackupDir = path;
                    break;
                case "BTDB":
                    settings.BTDBBackupDir = path;
                    break;
                case "BMC":
                    settings.BMCBackupDir = path;
                    break;
            }
            SaveSettings();
        }
        public static string GetBackupDir(string game)
        {
            string backupDir = "";
            switch (game)
            {
                case "BTD5":
                    backupDir = settings.BTD5BackupDir;
                    break;
                case "BTDB":
                    backupDir = settings.BTDBBackupDir;
                    break;
                case "BMC":
                    backupDir = settings.BMCBackupDir;
                    break;
            }
            return backupDir;
        }
        /*public static void CreateGameFile()
        {
            game = new GameFile();
            game.GameName = "";
            game.GameVersion = "";
            game.GameDir = "";
            game.GameBackupDir = "";
            game.ModsDir = "";
            game.LoadedMods = new List<string>();

            SaveGameFile();
        }
        public static GameFile LoadGameFile(string path)
        {
            //if(!File.Exists(path))
            string json = File.ReadAllText(path);
            if (JSON.IsJsonValid(json))
            {
                game = JsonConvert.DeserializeObject<GameFile>(json);
            }
            else
            {
                Log.Output("Game file has invalid json, generating a new Game file.");
                CreateGameFile();
            }
            return game;
        }
        public static void SaveGameFile()
        {
            string output = JsonConvert.SerializeObject(game, Formatting.Indented);
            string GameFilePath = Environment.CurrentDirectory + "\\" + game.GameName + ".profile";
            if (GameFilePath != "" && GameFilePath != null)
            {
                StreamWriter serialize = new StreamWriter(GameFilePath, false);
                serialize.Write(output);
                serialize.Close();
            }
            else
            {
                Log.OutputNotice("Unknown error occured... Path to game profile is invalid...");
            }
        }*/
    }
}
