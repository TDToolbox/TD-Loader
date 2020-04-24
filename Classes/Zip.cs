using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TD_Loader.Classes
{
    /// <summary>
    /// Creates zip file and handles zip operations
    /// </summary>
    class Zip
    {
        #region Constructor
        /// <summary>
        /// Default Constructor. Creates blank zip file
        /// </summary>
        public Zip()
        {
            this.Archive = new ZipFile();
        }

        /// <summary>
        /// Inherits default constructor. Creates zip from path
        /// </summary>
        /// <param name="path">path to create zip from. The created zip will be in memory and contain entries from path</param>
        public Zip(string path) : this()
        {
            Archive.Name = path;
        }

        /// <summary>
        /// Inherits default constructor. Creates zip from path and uses password with zip
        /// </summary>
        /// <param name="path">path to create zip from. The created zip will be in memory and contain entries from path</param>
        /// <param name="password">password to use with zip file</param>
        public Zip(string path, string password) : this()
        {
            Archive.Password = password;
            Archive.Name = path;
        }

        #endregion

        #region Properties
        public ZipFile Archive { get; set; }

        #endregion

        

        /// <summary>
        /// Extracts file from the ZipFile "Archive" that this class creates by default
        /// </summary>
        /// /// <param name="destination">path to the extract destination</param>
        public void Extract(string destination)
        {

        }


        public void Compile()
        {

        }

        /// <summary>
        /// Reads the text from a file in the ZipFile "Archive" that the class creates by default. Use this if zip
        /// doesn't have a password
        /// </summary>
        /// <param name="filePathInZip">The path inside the zip to the file you want to read from</param>
        /// <returns></returns>
        public string ReadFileInZip(string filePathInZip) => ReadFileInZip(filePathInZip, null);

        /// <summary>
        /// Reads the text from a file in the ZipFile "Archive" that the class creates by default
        /// </summary>
        /// <param name="filePathInZip">The path inside the zip to the file you want to read from</param>
        /// <param name="password">The password to the zip</param>
        /// <returns></returns>
        public string ReadFileInZip(string filePathInZip, string password)
        {
            string returnText = "";

            if (this.Archive.ContainsEntry(filePathInZip))
            {
                foreach (var entry in this.Archive.Entries)
                {
                    if (entry.FileName.Replace("/", "\\").Contains(filePathInZip))
                    {
                        Stream s;
                        if (Guard.IsStringValid(password))
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
                System.Windows.MessageBox.Show("Not found\n\n" + filePathInZip + "\n\nCount " + this.Archive.Count());
                Log.Output("Unable to read " + filePathInZip.Replace(this.Archive.Name, "") + "  because it wasnt found");
            }
            return returnText;
        }

        /// <summary>
        /// Use ToString to get the name(path) of the zip object
        /// </summary>
        /// <returns>The name (path) of the current zip object</returns>
        public override string ToString()
        {
            return "Zip: " + Archive.Name;
        }
    }
}
