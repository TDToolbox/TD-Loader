using BTD_Backend;
using BTD_Backend.Game;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD_Loader.Classes
{
    class TempSettings
    {
        private static TempSettings instance;

        public static TempSettings Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new TempSettings();
                    LoadSettings();
                }

                return instance; 
            }
            set { instance = value; }
        }

        public string settingsFileName { get; } = "settings.json";
        public string MainSettingsDir { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TD Loader";
        public bool ConsoleFlash { get; set; } = true;
        public GameType LastGame { get; set; } = GameType.None;
        public List<string> LastUsedMods { get; set; } = new List<string>();
        public string BTD6_ModsDir { get; set; }
        public string BTD5_ModsDir { get; set; }
        public string BTDB_ModsDir { get; set; }
        public string BMC_ModsDir { get; set; }
        
        public static TempSettings LoadSettings()
        {
            if (!File.Exists(Instance.MainSettingsDir + "\\" + Instance.settingsFileName))
                return Instance.CreateNewSettings();

            string json = File.ReadAllText(Instance.MainSettingsDir + "\\" + Instance.settingsFileName);
            if (!Guard.IsJsonValid(json) || json.Length <= 0)
            {
                Log.Output("Settings file has invalid json, generating a new settings file.");
                return Instance.CreateNewSettings();
            }

            return Instance = JsonConvert.DeserializeObject<TempSettings>(json);
        }

        public static void SaveSettings()
        {
            Instance.LastGame = SessionData.CurrentGame;
            Instance.LastUsedMods = SessionData.LoadedMods;
            string output = JsonConvert.SerializeObject(Instance, Formatting.Indented);

            StreamWriter serialize = new StreamWriter(Instance.MainSettingsDir + "\\" + Instance.settingsFileName, false);
            serialize.Write(output);
            serialize.Close();
        }

        public TempSettings CreateNewSettings()
        {
            instance = new TempSettings();
            Instance.BTD6_ModsDir = MainSettingsDir + "\\BTD6 Mods";
            Instance.BTD5_ModsDir = MainSettingsDir + "\\BTD5 Mods";
            Instance.BTDB_ModsDir = MainSettingsDir + "\\BTDB Mods";
            Instance.BMC_ModsDir = MainSettingsDir + "\\BMC Mods";

            SaveSettings();
            return Instance;
        }

        public string GetModsDir(GameType game)
        {
            string dir = "";
            switch (game)
            {
                case GameType.BTD6:
                    dir = TempSettings.Instance.BTD6_ModsDir;
                    break;
                case GameType.BTD5:
                    dir = TempSettings.Instance.BTD5_ModsDir;
                    break;
                case GameType.BTDB:
                    dir = TempSettings.Instance.BTDB_ModsDir;
                    break;
                case GameType.BMC:
                    dir = TempSettings.Instance.BMC_ModsDir;
                    break;
            }

            return dir;
        }

        public void SetModsDir(GameType game, string path)
        {
            switch (SessionData.CurrentGame)
            {
                case GameType.BTD6:
                    TempSettings.Instance.BTD6_ModsDir = path;
                    break;
                case GameType.BTD5:
                    TempSettings.Instance.BTD5_ModsDir = path;
                    break;
                case GameType.BTDB:
                    TempSettings.Instance.BTDB_ModsDir = path;
                    break;
                case GameType.BMC:
                    TempSettings.Instance.BMC_ModsDir = path;
                    break;
            }

            SaveSettings();
        }
    }
}
