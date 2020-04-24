using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        /// Gets password list from raw github link. Uses methods from other classes
        /// </summary>
        /// <returns>A string list of passwords</returns>
        public async System.Threading.Tasks.Task<List<string>> GetPasswordsList()
        {
            string result = await game.GetPasswordsListAsync();
            List<string> passwords = game.CreatePasswordsList(result);

            this.Passwords = passwords;
            return passwords;
        }
    }
}
