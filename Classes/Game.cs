using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
            DialogResult diag = MessageBox.Show("Mods Directory not set!\n\nWould you like to use the default mods directory \"Appdata/TD Loader/\"" + game + "/Mods. Press no to set your own.", "Use Default Directory?", MessageBoxButtons.YesNo);
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

                switch (game)
                {
                    case "BTD5":
                        {
                            Settings.settings.BTD5ModsDir = path;
                            Settings.SaveSettings();
                        }
                        break;
                    case "BTDB":
                        {
                            Settings.settings.BTDBModsDir = path;
                            Settings.SaveSettings();
                        }
                        break;
                    case "BMC":
                        {
                            Settings.settings.BMCModsDir = path;
                            Settings.SaveSettings();
                        }
                        break;
                }
            }
            else
            {
                Log.Output("You didnt select a valid directory");
            }
        }
        public static void CreateBackup(string game)
        {

        }
    }
}
