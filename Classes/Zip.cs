using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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
            Archive = new ZipFile();
        }

        /// <summary>
        /// Inherits default constructor. Creates zip from path
        /// </summary>
        /// <param name="path">path to create zip from. The created zip will be in memory and contain entries from path</param>
        public Zip(string path) : this()
        {
            Archive = new ZipFile(path);
            Archive.Name = path;
        }

        /// <summary>
        /// Inherits default constructor. Creates zip from path and uses password with zip
        /// </summary>
        /// <param name="path">path to create zip from. The created zip will be in memory and contain entries from path</param>
        /// <param name="password">password to use with zip file</param>
        public Zip(string path, string password)
        {
            Archive = new ZipFile(path);
            Archive.Name = path;

            Archive.Password = password;
            CurrentPassword = password;
        }

        #endregion

        #region Properties
        public event EventHandler PasswordAquired;
        public ZipFile Archive { get; set; }
        public string CurrentPassword { get; set; }

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

        /*public static List<string> GetModdedFolders(Zip original, Zip modded)
        {
            List<string> moddedFolders = new List<string>();

            foreach(ZipEntry entry in modded.Archive.Entries)
            {
                if(entry.IsDirectory)
                {
                    string[] split = entry.FileName.Split('/');
                    if (split.Length == 4)
                    {
                        string foldername = entry.FileName;

                        var originalSize = original.GetEntry(foldername).UncompressedSize;
                        var moddedSize = modded.GetEntry(foldername).UncompressedSize;

                        MessageBox.Show("One " + entry.IsDirectory.ToString());
                        MessageBox.Show(moddedSize.ToString());

                        *//*JetReader reader = new JetReader();
                        bool result = reader.CompareFiles(original, modded, foldername);
                        if (result == true)
                            moddedFolders.Add(foldername);*//*
                    }
                }
                
            }
            MessageBox.Show(moddedFolders.Count.ToString());
            return moddedFolders;
        }
*/

        public void CopyFilesBetweenZips(ZipFile source, ZipFile dest, string filepath)
        {
            string zipPath = filepath.Replace("\\", "/");
            filepath = filepath.Replace("/", "\\");

            GetEntry(source, zipPath).Extract(Settings.settings.StagingDir, ExtractExistingFileAction.OverwriteSilently);
            
            string[] split = filepath.Split('\\');
            string filename = split[split.Length - 1].Replace("\\","");

            dest.UpdateFile(Settings.settings.StagingDir + "\\" + filepath, zipPath.Replace(filename,""));
            dest.Save(Archive.Name);

            if (Directory.Exists(Settings.settings.StagingDir + "\\Assets\\JSON"))
                Directory.Delete(Settings.settings.StagingDir + "\\Assets\\JSON", true);
        }

        /// <summary>
        /// Returns the zip entry at the specified path. Uses the ZipFile from the Zip object
        /// </summary>
        /// <param name="filePathInZip">path to the file in the zip</param>
        /// <returns>Zip entry</returns>
        public ZipEntry GetEntry(string filePathInZip) => Zip.GetEntry(this.Archive, filePathInZip);

        /// <summary>
        /// Returns the zip entry at the specified path. Gets the file from the ZipFile argument
        /// </summary>
        /// <param name="zip">The ZipFile you want to get the entry from</param>
        /// <param name="filePathInZip">path to the file in the zip</param>
        /// <returns>Zip entry</returns>
        public static ZipEntry GetEntry(ZipFile zip, string filePathInZip)
        {
            if(zip == null)
            {
                Log.Output("Failed to get entry. Zip was null");
                MessageBox.Show("Failed to get entry. Zip was null");
                return null;
            }

            if(!Guard.IsStringValid(filePathInZip))
            {
                Log.Output("Failed to get entry. filePathInZip was invalid");
                MessageBox.Show("Failed to get entry. filePathInZip was invalid");
                return null;
            }

            foreach (var entry in zip.Entries)
            {
                if (entry.FileName == filePathInZip)
                {
                    return entry;
                }
            }
            return null;
        }

        #region Read Files in Zip
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
                    if (entry.FileName.Replace("\\", "/").Contains(filePathInZip))
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
                Log.Output("Not found! Failed to read file in zip\n\nZip: " + this.ToString() + "\n\nFailed to read file at: " + filePathInZip + "\n\nArchive file count " + this.Archive.Count());
            }
            return returnText;
        }
        #endregion


        #region Password Stuff
        /// <summary>
        /// Combines Password-Getter methods to get the zip password on thread
        /// </summary>
        /// <returns>password</returns>
        public string GetPassword()
        {
            List<string> passList = new List<string>();

            string passwordsFile = Settings.settings.MainSettingsDir + "\\passwords.txt";
            if (File.Exists(passwordsFile) && Settings.settings.DidBtdbUpdate == false)
            {
                StringReader reader = new StringReader(File.ReadAllText(passwordsFile));
                string line = string.Empty;
                do
                {
                    line = reader.ReadLine();
                    if (line != null)
                        passList.Add(line);
                } while (line != null);
            }
            else
            {
                MessageBox.Show("Reaquiring password list..");
                string gitText = "";
                Thread thread = new Thread(delegate ()
                {
                    bool error = false;
                    for (int i = 0; i < 100; i++)
                    {
                        try { gitText = GetPasswordsList(); break; }
                        catch (System.Net.WebException)
                        {
                            if (!error)
                            {
                                MessageBox.Show("Failed to read passwords list... Will continue trying for the next 10 seconds...");
                                error = true;
                            }
                            Thread.Sleep(100);
                        }
                    }
                });

                thread.Start();
                thread.Join();

                if (!Guard.IsStringValid(gitText))
                {
                    Log.Output("Failed to get password...");
                    MessageBox.Show("Failed to get password... Please try again...");
                    return "";
                }

                passList = CreatePasswordsList(gitText);
            }
            
            string password = DiscoverZipPasswordThread(passList);
            
            //PasswordAquired.Invoke(this, EventArgs.Empty);

            return password;
        }

        /// <summary>
        /// Threaded version of DiscoverZipPassword. Checks each password from the password list on github to see if 
        /// any work, and returns the one that does
        /// </summary>
        /// <param name="zip">The zip file we need the password for</param>
        /// <param name="passwords">the list of passwords we got from github</param>
        /// <returns>the password for the zip</returns>
        private string DiscoverZipPasswordThread(List<string> passwords)
        {
            string password = "";
            Thread thread = new Thread(delegate () { password = DiscoverZipPassword(passwords); });
            thread.Start();

            thread.Join();
            if (!Guard.IsStringValid(password))
                MessageBox.Show("Failed to get password for " + this.ToString());
            return password;
        }

        /// <summary>
        /// Checks each password from the password list on github to see if any work, and returns the one that does
        /// </summary>
        /// <param name="zip">The zip file we need the password for</param>
        /// <param name="passwords">the list of passwords we got from github</param>
        /// <returns>the password for the zip</returns>
        private string DiscoverZipPassword(List<string> passwords)
        {
            if (this.Archive == null)
            {
                Log.Output("Failed to get zip password. Zip file is null");
                return null;
            }

            if (passwords == null || passwords.Count <= 0)
            {
                Log.Output("Failed to get zip password. The list of passwords was empty");
                return null;
            }

            Stream s;
            string password = "";
            foreach (var entry in this.Archive)
            {
                if (!entry.IsDirectory)
                {
                    foreach (var pass in passwords)
                    {
                        try
                        {
                            s = entry.OpenReader(pass);
                            password = pass;
                            break;
                        }
                        catch (Ionic.Zip.BadPasswordException) { }
                    }
                    break;
                }
            }
            return password;
        }

        /// <summary>
        /// Gets the list of passwords from Github
        /// </summary>
        /// <returns>raw text read from url</returns>
        public string GetPasswordsList()
        {
            string url = "https://raw.githubusercontent.com/TDToolbox/BTDToolbox-2019_LiveFIles/master/BTD%20Battles%20Passwords";
            WebClient web = new WebClient();
            var stringData = web.DownloadString(url);
            return stringData;
        }

        /// <summary>
        /// Takes raw text from github and turns it into a list of passwords
        /// </summary>
        /// <param name="rawTextFromGithub">The raw text from the github page, returned from Game.GetPasswordsListAsync</param>
        /// <returns></returns>
        public List<string> CreatePasswordsList(string rawTextFromGithub)
        {
            if (!Guard.IsStringValid(rawTextFromGithub))
            {
                Log.Output("Password list from github was invalid");
                return null;
            }

            List<string> passwords = new List<string>();

            StringReader reader = new StringReader(rawTextFromGithub);
            string line = string.Empty;
            do
            {
                line = reader.ReadLine();
                if (line != null)
                {
                    if (line.Contains("-"))
                    {
                        if (line.Contains("Password"))
                        {
                            passwords.Add("Q%_{6#Px]]");
                        }
                        else
                        {
                            string removeText = line.Remove(line.IndexOf('-') + 1, line.Length - (line.IndexOf('-') + 1));
                            string password = line.Replace(removeText + " ", "");

                            if (password.Length <= 16)
                            {
                                passwords.Add(password);
                            }
                        }
                    }
                }

            } while (line != null);

            //Reverse the order so newest comes first
            List<string> temp = new List<string>();
            for(int i = passwords.Count - 1; i > 0; i--)
                temp.Add(passwords[i]);

            passwords.Clear();
            passwords = temp;

            if (Settings.settings.GameName == "BTDB")
                CreatePasswordFile(passwords);

            return passwords;
        }

        /// <summary>
        /// Creates a list of the passwords and stores it on disk
        /// </summary>
        /// <param name="passwords">the list of passwords gotten from github</param>
        private void CreatePasswordFile(List<string> passwords)
        {
            string passwordsFile = Settings.settings.MainSettingsDir + "\\passwords.txt";
            if (File.Exists(passwordsFile))
                File.Delete(passwordsFile);

            using (StreamWriter writetext = new StreamWriter(passwordsFile))
            {
                foreach (string password in passwords)
                {
                    writetext.WriteLine(password);
                }
            }            
        }
        #endregion


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
