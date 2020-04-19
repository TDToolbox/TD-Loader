using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TD_Loader.Classes
{
    class FileIO
    {
        public static string BrowseForGame()
        {
            Log.Output("Failed to automatically find " + Settings.settings.GameName);
            if(MessageBox.Show("Failed to automatically find " + Settings.settings.GameName + " . Please browse for the game's .exe file to set the game directory", "Couldn't find game .exe", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                return FileIO.BrowseForFile("Browse for .exe", "exe", "Exe files (*.exe)|*.exe|All files (*.*)|*.*", "");
            }
            else
            {
                MessageBox.Show("You will not be able to use the mod loader for " + Settings.settings.GameName + " without the exe!");
                return "";
            }

        }
        public static string BrowseForFile(string title, string defaultExt, string filter, string startDir)
        {
            OpenFileDialog fileDiag = new OpenFileDialog();
            fileDiag.Title = title;
            fileDiag.DefaultExt = defaultExt;
            fileDiag.Filter = filter;
            fileDiag.Multiselect = false;
            fileDiag.InitialDirectory = startDir;

            if (fileDiag.ShowDialog() == DialogResult.OK)
            {
                return fileDiag.FileName;
            }
            else
                return null;
        }
        public static List<string> BrowseForFiles(string title, string defaultExt, string filter, string startDir)
        {
            OpenFileDialog fileDiag = new OpenFileDialog();
            fileDiag.Title = title;
            fileDiag.DefaultExt = defaultExt;
            fileDiag.Filter = filter;
            fileDiag.Multiselect = true;
            fileDiag.InitialDirectory = startDir;

            if (fileDiag.ShowDialog() == DialogResult.OK)
            {
                List<string> files = new List<string>();
                files.AddRange(fileDiag.FileNames);
                return files;
            }
            else
                return null;
        }
    }
}
