using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TD_Loader.UserControls
{
    /// <summary>
    /// Interaction logic for Game_UC.xaml
    /// </summary>
    public partial class Game_UC : INotifyPropertyChanged
    {

        public Game_UC()
        {
            DataContext = this;
            InitializeComponent();
            GamePicture = new BitmapImage(new Uri("../Resources/BTD6 loaded.png", UriKind.Relative));
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
    }
}
