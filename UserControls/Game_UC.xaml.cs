using Microsoft.WindowsAPICodePack.Shell.Interop;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
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

            GamePicture = new BitmapImage(new Uri("../Resources/" + SessionData.CurrentGame.ToString() +" loaded.png", UriKind.Relative));
            //GamePicture = new BitmapImage(new Uri("../Resources/BTD6 loaded.png", UriKind.Relative));

            //SessionData.CurrentGame.ToString()
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
        }
    }
}
