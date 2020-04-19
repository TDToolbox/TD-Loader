using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TD_Loader.Classes;

namespace TD_Loader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Startup();
            BTD5_Image.IsMouseDirectlyOverChanged += BTD5_Image_IsMouseDirectlyOverChanged;
            BTDB_Image.IsMouseDirectlyOverChanged += BTDB_Image_IsMouseDirectlyOverChanged;
            BMC_Image.IsMouseDirectlyOverChanged += BMC_Image_IsMouseDirectlyOverChanged;

            Main.Closing += Main_Closing;
        }
        private void Startup()
        {
            Settings.LoadSettings();            
        }
        private void FinishedLoading()
        {
            switch (Settings.settings.GameName)
            {
                case "BTD5":
                    if (Settings.settings.BTD5Dir != "")
                    {
                        BTD5_Image.Source = new BitmapImage(new Uri("Resources/btd5.png", UriKind.Relative));
                        GameHandling();
                    }
                    break;
                case "BTDB":
                    if (Settings.settings.BTDBDir != "")
                    {
                        BTDB_Image.Source = new BitmapImage(new Uri("Resources/btdb 2.png", UriKind.Relative));
                        GameHandling();
                    }
                    break;
                case "BMC":
                    if (Settings.settings.BMCDir != "")
                    {
                        BMC_Image.Source = new BitmapImage(new Uri("Resources/bmc.png", UriKind.Relative));
                        GameHandling();
                    }
                    break;
            }
        }

        private void GameHandling()
        {
            string modsDir = "";
            switch (Settings.settings.GameName)
            {
                case "BTD5":
                    {
                        modsDir = Settings.settings.BTD5ModsDir;
                    }
                    break;
                case "BTDB":
                    {
                        modsDir = Settings.settings.BTDBModsDir;
                    }
                    break;
                case "BMC":
                    {
                        modsDir = Settings.settings.BMCModsDir;
                    }
                    break;
            }

            if(modsDir == "" || modsDir == null)
            {
                Game.SetModsDir(Settings.settings.GameName);
            }
        }


        //
        //Main events
        //
        private void Main_Activated(object sender, EventArgs e)
        {
            FinishedLoading();
        }
        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.SaveSettings();
        }


        //
        //UI events
        //
        private void ResetGamePictures()
        {
                BTD5_Image.Source = new BitmapImage(new Uri("Resources/btd5_not loaded.png", UriKind.Relative));
                BTDB_Image.Source = new BitmapImage(new Uri("Resources/btdb 2_not loaded.png", UriKind.Relative));
                BMC_Image.Source = new BitmapImage(new Uri("Resources/bmc_not loaded.png", UriKind.Relative));
        }
        private void BMC_Image_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(Settings.settings.GameName != "BMC")
            {
                if (!BMC_Image.IsMouseOver)
                    BMC_Image.Source = new BitmapImage(new Uri("Resources/bmc_not loaded.png", UriKind.Relative));
                else
                    BMC_Image.Source = new BitmapImage(new Uri("Resources/bmc.png", UriKind.Relative));
            }
        }
        private void BTDB_Image_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Settings.settings.GameName != "BTDB")
            {
                if (!BTDB_Image.IsMouseOver)
                    BTDB_Image.Source = new BitmapImage(new Uri("Resources/btdb 2_not loaded.png", UriKind.Relative));
                else
                    BTDB_Image.Source = new BitmapImage(new Uri("Resources/btdb 2.png", UriKind.Relative));
            }                
        }
        private void BTD5_Image_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Settings.settings.GameName != "BTD5")
            {
                if (!BTD5_Image.IsMouseOver)
                    BTD5_Image.Source = new BitmapImage(new Uri("Resources/btd5_not loaded.png", UriKind.Relative));
                else
                    BTD5_Image.Source = new BitmapImage(new Uri("Resources/btd5.png", UriKind.Relative));
            }
        }
        private void BTD5_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Settings.settings.GameName != "BTD5")
            {
                Settings.settings.GameName = "BTD5";
                Settings.SaveSettings();
                ResetGamePictures();
                BTD5_Image.Source = new BitmapImage(new Uri("Resources/btd5.png", UriKind.Relative));
                GameHandling();

                if (Settings.settings.BTD5Dir == "" || Settings.settings.BTD5Dir == null)
                {
                    string dir = Steam.GetGameDir(Steam.BTD5AppID);
                    if(dir != "" && dir != null)
                    {
                        Settings.settings.BTD5Dir = dir;
                        Settings.SaveSettings();
                    }
                    else
                    {
                        string result = FileIO.BrowseForGame();
                        if (result != "" && result != null)
                        {
                            Log.Output("You selected " + result);
                            Settings.settings.BTD5Dir = result;
                            Settings.SaveSettings();
                        }
                        else
                        {
                            Log.Output("Something went wrong... Failed to aquire game directory...");
                        }
                    }
                }
            }
        }
        private void BTDB_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Settings.settings.GameName != "BTDB")
            {
                Settings.settings.GameName = "BTDB";
                Settings.SaveSettings();
                ResetGamePictures();
                BTDB_Image.Source = new BitmapImage(new Uri("Resources/btdb 2.png", UriKind.Relative));
                GameHandling();

                if (Settings.settings.BTDBDir == "" || Settings.settings.BTDBDir == null)
                {
                    string dir = Steam.GetGameDir(Steam.BTDBAppID);
                    if (dir != "" && dir != null)
                    {
                        Settings.settings.BTDBDir = dir;
                        Settings.SaveSettings();
                    }
                    else
                    {
                        string result = FileIO.BrowseForGame();
                        if (result != "" && result != null)
                        {
                            Log.Output("You selected " + result);
                            Settings.settings.BTDBDir = result;
                            Settings.SaveSettings();
                        }
                        else
                        {
                            Log.Output("Something went wrong... Failed to aquire game directory...");
                        }
                    }
                }
            }
        }
        private void BMC_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(Settings.settings.GameName != "BMC")
            {
                Settings.settings.GameName = "BMC";
                Settings.SaveSettings();
                ResetGamePictures();
                BMC_Image.Source = new BitmapImage(new Uri("Resources/bmc.png", UriKind.Relative));
                GameHandling();

                if (Settings.settings.BMCDir == "" || Settings.settings.BMCDir == null)
                {
                    string dir = Steam.GetGameDir(Steam.BMCAppID);
                    if (dir != "" && dir != null)
                    {
                        Settings.settings.BMCDir = dir;
                        Settings.SaveSettings();
                    }
                    else
                    {
                        string result = FileIO.BrowseForGame();
                        if (result != "" && result != null)
                        {
                            Log.Output("You selected " + result);
                            Settings.settings.BMCDir = result;
                            Settings.SaveSettings();
                        }
                        else
                        {
                            Log.Output("Something went wrong... Failed to aquire game directory...");
                        }
                    }
                }
            }
        }
    }
}
