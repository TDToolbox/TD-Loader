using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TD_Loader.Classes
{
    class Zip
    {
        public static void ExtractFile(string path)
        {

        }
        public static void ExtractAll(string path)
        {

        }
        public static void Compile()
        {

        }
        public static string ReadFileInZip(string pathToZip, string filePathInZip, string password)
        {
            string returnText = "";
            var zip = ZipFile.Read(pathToZip);
            
            if (zip.ContainsEntry(filePathInZip))
            {
                foreach (var entry in zip.Entries)
                {
                    if (entry.FileName.Replace("/", "\\").Contains(filePathInZip))
                    {
                        Stream s;
                        if (password != "")
                        {
                            entry.Password = password;
                            s = entry.OpenReader(password);
                        }
                        else
                            s = entry.OpenReader();

                        StreamReader sr = new StreamReader(s);
                        returnText = sr.ReadToEnd();
                    }
                }
            }
            else
            {
                    System.Windows.MessageBox.Show("Not found\n\n" + filePathInZip + "\n\nCount " + zip.Count() );
                Log.Output("Unable to read " + filePathInZip.Replace(zip.Name,"") + "  because it wasnt found");
            }
            return returnText;
        }
    }
}
