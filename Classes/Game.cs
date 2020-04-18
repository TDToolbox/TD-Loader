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
                
            if(path != "")
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
    }
}
