using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD_Loader.Classes
{
    class Mods
    {
        public static List<string> AddMods(string game)
        {
            return FileIO.BrowseForFiles("Browse for mods", "", "Jet files (*.jet)|*.jet|Zip files (*.zip)|*.zip|Rar files (*.rar)|*.rar|7z files (*.7z)|*.7z|", "");
        }
        public static string IncrementName(string modname)
        {
            return "";
        }
    }
}
