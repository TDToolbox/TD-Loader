using Microsoft.WindowsAPICodePack.Dialogs;
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
        public static string BrowseForDirectory(string title, string startDir)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = title;
            dialog.Multiselect = false;
            dialog.InitialDirectory = startDir;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            else
                return null;
        }
    }
}
