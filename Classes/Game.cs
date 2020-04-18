using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD_Loader.Classes
{
    class Game
    {
        public static void CreateModsDir(string path, string game)
        {
            if (path == "")
                path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TD Loader\\" + game;

            Directory.CreateDirectory(path);
        }
        /*public string GetVersion(string game)
        {
            using (ZipArchive zip = ZipFile.Open("test.zip", ZipArchiveMode.Create))
            {

            }
        }*/
    }
}
