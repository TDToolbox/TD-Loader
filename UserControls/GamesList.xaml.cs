using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using BTD_Backend.Game;
using TD_Loader.Classes;

namespace TD_Loader.UserControls
{
    public partial class GamesList : INotifyPropertyChanged
    {
        public static GamesList Instance;
        public Dictionary<GameType, BitmapImage> GameImgBindings;
        public Dictionary<BitmapImage, Image> ImageBindings;


        #region Constructors
        public GamesList()
        {
            DataContext = this;
            Instance = this;
            InitializeComponent();
            Startup();
        }

        #endregion


        #region Properties
        private BitmapImage btd6ImgBinding;
        public BitmapImage Btd6ImgBinding 
        {
            get { return btd6ImgBinding; } 
            set
            {
                if (btd6ImgBinding != value)
                {
                    btd6ImgBinding = value;
                    OnPropertyChanged();
                }
            }
        }

        private BitmapImage btd5ImgBinding;
        public BitmapImage Btd5ImgBinding
        {
            get { return btd5ImgBinding; }
            set
            {
                if (btd5ImgBinding != value)
                {
                    btd5ImgBinding = value;
                    OnPropertyChanged();
                }
            }
        }

        private BitmapImage btdbImgBinding;
        public BitmapImage BtdbImgBinding
        {
            get { return btdbImgBinding; }
            set
            {
                if (btdbImgBinding != value)
                {
                    btdbImgBinding = value;
                    OnPropertyChanged();
                }
            }
        }

        private BitmapImage bmcImgBinding;
        public BitmapImage BmcImgBinding
        {
            get { return bmcImgBinding; }
            set
            {
                if (bmcImgBinding != value)
                {
                    bmcImgBinding = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion


       
        private void Startup()
        {
            UnloadAllGames();
            GameChanged += GamesList_GameChanged;
        }

        private void GamesList_GameChanged(object sender, GameListEventArgs e)
        {
            if (SessionData.CurrentGame != GameType.None)
                TempSettings.SaveSettings();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GameImgBindings = new Dictionary<GameType, BitmapImage>();
            ImageBindings = new Dictionary<BitmapImage, Image>();

            GameImgBindings.Add(GameType.BTD6, Btd6ImgBinding);
            GameImgBindings.Add(GameType.BTD5, Btd5ImgBinding);
            GameImgBindings.Add(GameType.BTDB, BtdbImgBinding);
            GameImgBindings.Add(GameType.BMC, BmcImgBinding);

            ImageBindings.Add(Btd6ImgBinding, BTD6_Image);
            ImageBindings.Add(Btd5ImgBinding, BTD5_Image);
            ImageBindings.Add(BtdbImgBinding, BTDB_Image);
            ImageBindings.Add(BmcImgBinding, BMC_Image);

            UnloadAllGames();
        }

        public void UnloadAllGames(bool keepLoadedGame = true)
        {
            Btd6ImgBinding = GetBitmapImg(GameType.BTD6, false);
            Btd5ImgBinding = GetBitmapImg(GameType.BTD5, false);
            BtdbImgBinding = GetBitmapImg(GameType.BTDB, false);
            BmcImgBinding = GetBitmapImg(GameType.BMC, false);

            if (SessionData.CurrentGame != GameType.None && keepLoadedGame)
            {
                try
                {
                    var gameImgBinding = GameImgBindings[SessionData.CurrentGame];
                    var image = ImageBindings[gameImgBinding];
                    SetImage(image, true);
                }
                catch 
                {
                    SessionData.CurrentGame = GameType.None;
                    TempSettings.SaveSettings();
                    Instance.OnGameChanged(new GameListEventArgs());
                }
            }
        }

        public void SetImage(Image img, bool isLoaded) => img.Source = GetBitmapImg(img, isLoaded);

        public BitmapImage GetBitmapImg(GameType game, bool isLoaded) => GetBitmapImg(game.ToString(), isLoaded);

        public BitmapImage GetBitmapImg(Image img, bool isLoaded) =>
            GetBitmapImg(img.Name.ToString().Replace("_Image", ""), isLoaded);

        public BitmapImage GetBitmapImg(string game, bool isLoaded)
        {
            if (isLoaded)
                game += " loaded";
            else
                game += " not loaded";

            return new BitmapImage(new Uri("../Resources/" + game + ".png", UriKind.Relative));
        }


        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static event EventHandler<GameListEventArgs> GameChanged;
        public class GameListEventArgs : EventArgs
        {

        }

        public void OnGameChanged(GameListEventArgs e)
        {
            EventHandler<GameListEventArgs> handler = GameChanged;
            if (handler != null)
                handler(this, e);
        }

        #endregion

        #region UI Events
        private void BTD6_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SessionData.CurrentGame == GameType.BTD6)
                return;

            SessionData.CurrentGame = GameType.BTD6;

            var args = new GameListEventArgs();
            OnGameChanged(args);
        }

        private void BTD5_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SessionData.CurrentGame == GameType.BTD5)
                return;

            SessionData.CurrentGame = GameType.BTD5;
            var args = new GameListEventArgs();
            OnGameChanged(args);
        }

        private void BTDB_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SessionData.CurrentGame == GameType.BTDB)
                return;

            SessionData.CurrentGame = GameType.BTDB;
            var args = new GameListEventArgs();
            OnGameChanged(args);
        }

        private void BMC_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SessionData.CurrentGame == GameType.BMC)
                return;

            SessionData.CurrentGame = GameType.BMC;
            var args = new GameListEventArgs();
            OnGameChanged(args);
        }

        private void BTD6_Image_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UnloadAllGames();

            if (BTD6_Image.IsMouseOver || SessionData.CurrentGame == GameType.BTD6)
                Btd6ImgBinding = GetBitmapImg(GameType.BTD6, true);
        }

        private void BTD5_Image_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UnloadAllGames();
            if (BTD5_Image.IsMouseOver || SessionData.CurrentGame == GameType.BTD5)
                Btd5ImgBinding = GetBitmapImg(GameType.BTD5, true);
        }

        private void BTDB_Image_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UnloadAllGames();
            if (BTDB_Image.IsMouseOver || SessionData.CurrentGame == GameType.BTDB)
                BtdbImgBinding = GetBitmapImg(GameType.BTDB, true);
        }

        private void BMC_Image_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UnloadAllGames();
            if (BMC_Image.IsMouseOver || SessionData.CurrentGame == GameType.BMC)
                BmcImgBinding = GetBitmapImg(GameType.BMC, true);
        }
        #endregion

    }
}
