﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TD_Loader.Classes
{
    class Settings
    {
        public static SettingsFile settings;
        public static GameFile game;
        public static string settingsFileName = "settings.json";
        public static string settingsPath = Environment.CurrentDirectory + "\\" + settingsFileName;

        public Settings()
        {
            game = new GameFile();
        }

        public class SettingsFile
        {
            public string MainSettingsDir { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TD Loader";
            public string TDLoaderVersion { get; set; }
            public bool RecentUpdate { get; set; }
            public string GameName { get; set; }
            public string StagingDir { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TD Loader\\Mod Staging";

            public string NKHookVersion { get; set; }
            public string TowerLoadNKPluginVersion { get; set; }

            public string BTD5Dir { get; set; }
            public string BTD5Version { get; set; }
            public string BTD5BackupDir { get; set; }
            public string BTD5ModsDir { get; set; }
            public List<string> BTD5LoadedMods { get; set; }


            public bool DidBtdbUpdate { get; set; }
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
        /// <summary>
        /// An object containing info on the current game, from the settings
        /// </summary>
        public class GameFile
        {
            public string GameName { get; set; }
            public string ExeName { get; set; }
            public string Password { get; set; }
            public string GameVersion { get; set; }
            public string GameDir { get; set; }
            public string GameBackupDir { get; set; }
            public string ModsDir { get; set; }
            public List<string> LoadedMods { get; set; }
        }
        
        /// <summary>
        /// Sets the properties of the game file based on the game
        /// </summary>
        public static void SetGameFile(string gameName)
        {
            game = new GameFile();

            if (gameName == "BTD5")
            {
                game.GameName = "BTD5";
                game.Password = "Q%_{6#Px]]";
                game.ExeName = "BTD5-Win.exe";
                game.GameVersion = settings.BTD5Version;
                game.GameDir = settings.BTD5Dir;
                game.GameBackupDir = settings.BTD5BackupDir;
                game.ModsDir = settings.BTD5ModsDir;
                game.LoadedMods = settings.BTD5LoadedMods;
            }
            else if (gameName == "BTDB")
            {
                game.GameName = "BTDB";
                game.Password = "";
                game.ExeName = "Battles-Win.exe";
                game.GameVersion = settings.BTDBVersion;
                game.GameDir = settings.BTDBDir;
                game.GameBackupDir = settings.BTDBBackupDir;
                game.ModsDir = settings.BTDBModsDir;
                game.LoadedMods = settings.BTDBLoadedMods;
            }
            else if (gameName == "BMC")
            {
                game.GameName = "BMC";
                game.Password = "Q%_{6#Px]]";
                game.ExeName = "MonkeyCity-Win.exe";
                game.GameVersion = settings.BMCVersion;
                game.GameDir = settings.BMCDir;
                game.GameBackupDir = settings.BMCBackupDir;
                game.ModsDir = settings.BMCModsDir;
                game.LoadedMods = settings.BMCLoadedMods;
            }

            if(game.LoadedMods == null)
                game.LoadedMods = new List<string>();
        }
        
        /// <summary>
        /// Saves the game file values to the settings
        /// </summary>
        
        public static void SaveGameFile()
        {
            if (game == null)
            {
                //MessageBox.Show("Failed to save game object");
                return;
            }

            if (game.GameName == "BTD5")
            {
                settings.GameName = game.GameName;
                settings.BTD5Version = game.GameVersion;
                settings.BTD5Dir = game.GameDir.Replace("\\\\", "\\");
                settings.BTD5BackupDir = game.GameBackupDir;
                settings.BTD5ModsDir = game.ModsDir;
                settings.BTD5LoadedMods = game.LoadedMods;
            }
            else if (game.GameName == "BTDB")
            {
                settings.GameName = game.GameName;
                settings.BTDBVersion = game.GameVersion;
                settings.BTDBDir = game.GameDir.Replace("\\\\", "\\"); ;
                settings.BTDBBackupDir = game.GameBackupDir;
                settings.BTDBModsDir = game.ModsDir;
                settings.BTDBLoadedMods = game.LoadedMods;
            }
            else if (game.GameName == "BMC")
            {
                settings.GameName = game.GameName;
                settings.BMCVersion = game.GameVersion;
                settings.BMCDir = game.GameDir.Replace("\\\\", "\\"); ;
                settings.BMCBackupDir = game.GameBackupDir;
                settings.BMCModsDir = game.ModsDir;
                settings.BMCLoadedMods = game.LoadedMods;
            }
        }
        public static void CreateSettings()
        {
            settings = new SettingsFile();

            settings.TDLoaderVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
            settings.GameName = "";
            settings.RecentUpdate = false;

            settings.NKHookVersion = "";
            settings.TowerLoadNKPluginVersion = "";

            settings.BTD5Dir = "";
            settings.BTD5Version = Game.GetVersion("BTD5");
            settings.BTD5BackupDir = "";
            settings.BTD5ModsDir = "";
            settings.BTD5LoadedMods = new List<string>();


            settings.BTDBDir = "";
            settings.BTDBVersion = Game.GetVersion("BTDB");
            settings.BTDBBackupDir = "";
            settings.BTDBModsDir = "";
            settings.DidBtdbUpdate = false;
            settings.BTDBLoadedMods = new List<string>();


            settings.BMCDir = "";
            settings.BMCVersion = Game.GetVersion("BMC");
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
            if (!JSON.IsJsonValid(json) || json.Length <= 0)
            {
                Log.Output("Settings file has invalid json, generating a new settings file.");
                CreateSettings();
            }
            settings = JsonConvert.DeserializeObject<SettingsFile>(json);

            SetGameFile(settings.GameName);
            return settings;
        }
        public static void SaveSettings()
        {
            if (!Guard.IsStringValid(settingsPath))
                Log.OutputNotice("Unknown error occured... Path to settings is invalid...");

            string output = JsonConvert.SerializeObject(settings, Formatting.Indented);

            SaveGameFile();
            StreamWriter serialize = new StreamWriter(settingsPath, false);
            serialize.Write(output);
            serialize.Close();
        }
    }
}
