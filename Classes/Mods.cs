using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD_Loader.Classes
{
    class Mods
    {
        public static List<string> AddMods()
        {
            return FileIO.BrowseForFiles("Browse for mods", "", "Jet files (*.jet)|*.jet|Zip files (*.zip)|*.zip|Rar files (*.rar)|*.rar|7z files (*.7z)|*.7z", "");
        }
        public static string CopyMod(string source, string dest)
        {
            if (!File.Exists(source))
            {
                Log.Output("Unable to copy mod. Source file does not exist");
                return "";
            }

            if (File.Exists(dest))
            {
                dest = IncrementName(dest);
            }
            File.Copy(source, dest);
            return dest;
        }

        public static string IncrementName(string dest)
        {
            FileInfo f = new FileInfo(dest);
            string filename = f.Name;
            string fileExt = f.Extension;
            string destDir = dest.Replace(filename, "");

            int i = 1;
            while (File.Exists(dest))
            {
                dest = destDir + filename.Replace(fileExt, "") + " - Copy " + i + fileExt;
                i++;
            }
            return dest;
        }
    }
}
