using BTD_Backend;
using BTD_Backend.Game;
using Microsoft.WindowsAPICodePack.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using TD_Loader.Classes;

namespace TD_Loader.UserControls
{
    /// <summary>
    /// Interaction logic for Game_UC.xaml
    /// </summary>
    public partial class Game_UC : INotifyPropertyChanged
    {
        public static Game_UC Instance;

        public Game_UC()
        {
            DataContext = this;
            InitializeComponent();
            Instance = this;
            GamesList.GameChanged += GamesList_GameChanged;
            GamePicture = new BitmapImage(new Uri("../Resources/" + SessionData.CurrentGame.ToString() +" loaded.png", UriKind.Relative));

            string modsDir = TempSettings.Instance.GetModsDir(SessionData.CurrentGame);
            if (!String.IsNullOrEmpty(modsDir))
                Mods_Dir_TextBox.Text = modsDir;
        }

        private void GamesList_GameChanged(object sender, GamesList.GameListEventArgs e)
        {
            string modsDir = TempSettings.Instance.GetModsDir(SessionData.CurrentGame);
            if (String.IsNullOrEmpty(modsDir))
                Mods_Dir_TextBox.Text = "";
            else
                Mods_Dir_TextBox.Text = modsDir;

            Instance.SetGamePicture();
        }


        #region Properties
        private BitmapImage gamePicture;
        public BitmapImage GamePicture
        {
            get { return gamePicture; }
            set
            {
                if (gamePicture != value)
                {
                    gamePicture = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion


        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public void SetGamePicture()
        {
            GamePicture = new BitmapImage(new Uri("../Resources/" + SessionData.CurrentGame.ToString() + " loaded.png", UriKind.Relative));
            
            string modsDir = TempSettings.Instance.GetModsDir(SessionData.CurrentGame);
            if (!String.IsNullOrEmpty(modsDir))
                Mods_Dir_TextBox.Text = modsDir;
        }

        private void SetModsDir_Button_Click(object sender, RoutedEventArgs e) => SetModsDir();

        public void SetModsDir()
        {
            if (SessionData.CurrentGame == GameType.None)
                return;

            string path = FileIO.BrowseForDirectory("Choose a directory for your mods", Environment.CurrentDirectory);

            if (String.IsNullOrEmpty(path))
                return;

            var gameTypeList = new List<GameType>() { GameType.BTD6, GameType.BTD5, GameType.BTDB, GameType.BMC, GameType.BTDAT, GameType.NKArchive };
            foreach (var item in gameTypeList)
            {
                if (TempSettings.Instance.GetModsDir(item) == path && SessionData.CurrentGame != item)
                {
                    Log.Output("Error! Can't use this path. The location you chose is being used by " + item.ToString()
                        + ". Please use another path for your mods folder");
                    return;
                }
            }

            Mods_Dir_TextBox.Text = path;
            TempSettings.Instance.SetModsDir(SessionData.CurrentGame, path);
            Mods_UserControl.instance.PopulateMods(SessionData.CurrentGame);
        }
    }
}
