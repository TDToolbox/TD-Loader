using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace TD_Loader.Classes
{
    class JetReader
    {
        /// <summary>
        /// This class combines operations to read jet file and return strings or lists of modded files, etc
        /// </summary>

        #region Constructor
        Game game;
        public JetReader()
        {
            var game = new Game();
        }

        #endregion

        #region Properties

        public List<string> Passwords { get; set; }
        public string GameName { get; set; } = Settings.settings.GameName;
        public string BTD5Password { get; } = "Q%_{6#Px]]";

        #endregion


        /// <summary>
        /// Reads the text from the file in both zip's and compares if they are the same or different. Different means its modded
        /// </summary>
        /// <param name="original">An object of the Zip class that is made from the original backup jet file</param>
        /// <param name="modded">An object of the Zip class that is made from the modded jet file</param>
        /// <param name="filepathInZip">the filepath to the file you want to compare. It will be the same for both jets</param>
        /// <returns></returns>
        public bool CompareFiles(Zip original, Zip modded, string filepathInZip)
        {
            if(original == null || modded == null)
            {
                Log.Output("One of the zip files you are trying to compare is invalid");
                return false;
            }

            string originalText = Regex.Replace(original.ReadFileInZip(filepathInZip, original.CurrentPassword), @"^\s*$(\n|\r|\r\n)", "", RegexOptions.Multiline).ToLower().Trim().Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\r\n", "");
            string modText = Regex.Replace(modded.ReadFileInZip(filepathInZip, original.CurrentPassword), @"^\s*$(\n|\r|\r\n)", "", RegexOptions.Multiline).ToLower().Trim().Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\r\n", "");

            bool isModded = false;
            if (modText != originalText)
                isModded = true;
            
            return isModded;
        }


        public List<string> GetAllModdedFiles(Zip original, Zip modded)
        {
            List<string> moddedFiles = new List<string>();
            
            foreach(var file in modded.Archive.Entries)
            {
                if (CompareFiles(original, modded, file.FileName))
                    moddedFiles.Add(file.FileName);
            }           

            return moddedFiles;
        }

        /// <summary>
        /// Gets password list from raw github link. Uses methods from other classes
        /// </summary>
        /// <returns>A string list of passwords</returns>
        public async System.Threading.Tasks.Task<List<string>> GetPasswordsList()
        {
            if (game == null)
                game = new Game();
            string result = await game.GetPasswordsListAsync();
            List<string> passwords = game.CreatePasswordsList(result);

            this.Passwords = passwords;
            return passwords;
        }
    }
}
