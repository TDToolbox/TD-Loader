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
        bool finishedLoading = false;
        public static bool doingWork = false;
        public static MainWindow instance;
        public static Mods_UserControl mods_User;
        public MainWindow()
        {
            InitializeComponent();
            instance = this;

            Startup();
            BTD5_Image.IsMouseDirectlyOverChanged += BTD5_Image_IsMouseDirectlyOverChanged;
            BTDB_Image.IsMouseDirectlyOverChanged += BTDB_Image_IsMouseDirectlyOverChanged;
            BMC_Image.IsMouseDirectlyOverChanged += BMC_Image_IsMouseDirectlyOverChanged;

            Main.Closing += Main_Closing;
        }
        private void Startup()
        {
            Settings.LoadSettings();

            var tab = new TabItem();
            tab.Header = "     Mods     ";
            tab.Padding = new Thickness(5);
            tab.FontSize = 25;
            tab.Content = new Mods_UserControl();
            Main_TabController.Items[1] = tab;
            
            //Mods_Tab.Content = new Mods_UserControl();

        }
        private void FinishedLoading()
        {
            bool dirNotFound = false;
            switch (Settings.settings.GameName)
            {
                case "BTD5":
                    if (Settings.settings.BTD5Dir != "" && Settings.settings.BTD5Dir != null)
                        BTD5_Image.Source = new BitmapImage(new Uri("Resources/btd5.png", UriKind.Relative));
                    else
                        dirNotFound = true;
                    break;


                case "BTDB":
                    if (Settings.settings.BTDBDir != "" && Settings.settings.BTDBDir != null)
                        BTDB_Image.Source = new BitmapImage(new Uri("Resources/btdb 2.png", UriKind.Relative));
                    else
                        dirNotFound = true;
                    break;


                case "BMC":
                    if (Settings.settings.BMCDir != "" && Settings.settings.BMCDir != null)
                        BMC_Image.Source = new BitmapImage(new Uri("Resources/bmc.png", UriKind.Relative));
                    else
                        dirNotFound = true;
                    break;
                default:
                    dirNotFound = true;
                    break;
            }

            //if (Settings.settings.GameName != null && Settings.settings.GameName != "")
            if(!dirNotFound)
            {
                GameHandling();
                
            }
            else
            {
                Settings.settings.GameName = "";
                Settings.SaveSettings();
            }
        }
        private async void GameHandling()
        {
            doingWork = true;


            //
            //Check for Game Updated
            string version = Game.GetVersion(Settings.settings.GameName);
            if (version != Settings.GetGameVersion(Settings.settings.GameName))
            {
                MessageBox.Show("Game has been updated... Reaquiring files...");
                Log.Output("Game has been updated... Reaquiring files...");
                string backupdir = Settings.GetBackupDir(Settings.settings.GameName);
                if (backupdir == "" || backupdir == null)
                    Game.CreateBackupDir(Settings.settings.GameName);

                await Game.CreateBackupAsync(Settings.settings.GameName);
                Log.Output("Done making backup");

                Settings.SetGameVersion(Settings.settings.GameName, version);
            }


            //
            //Check game dir
            bool error = false;
            string gameD = Settings.GetGameDir(Settings.settings.GameName);
            if (gameD != "" && gameD != null)
            {
                if(Directory.Exists(gameD))
                    Log.Output("Game Directory Found!");
                else
                {
                    error = true;
                    Log.Output("The saved game directory couldnt be found!");
                }
            }
            else
                error = true;

            if (error)
            {
                string dir = Game.SetGameDir(Settings.settings.GameName);
                if (dir != "" && dir != null)
                    Settings.SetGameDir(Settings.settings.GameName, dir);
                else
                {
                    Log.Output("Something went wrong... Failed to aquire game directory...");
                    ResetGamePictures();
                    return;
                }
            }
            

            //
            //Check Mods Dir
            string modsDir = Settings.GetModsDir(Settings.settings.GameName);
            if((Settings.settings.GameName != "" && Settings.settings.GameName != null) && (modsDir == "" || modsDir == null))
                Game.SetModsDir(Settings.settings.GameName);


            //
            //Check Backup
            bool valid = Game.VerifyBackup(Settings.settings.GameName);
            if(!valid)
            {
                string backupdir = Settings.GetBackupDir(Settings.settings.GameName);
                if (backupdir == "" || backupdir == null)
                    Game.CreateBackupDir(Settings.settings.GameName);

                await Game.CreateBackupAsync(Settings.settings.GameName);
                Log.Output("Done making backup");
            }


            //
            //Clear mods list
            Mods_UserControl.instance.PopulateMods(Settings.settings.GameName);

            //
            //Done
            doingWork = false;
        }
        public static async Task Wait(int time)
        {
            await Task.Delay(time);
        }

        //
        //Main events
        //
        private void Main_Activated(object sender, EventArgs e)
        {
            if (finishedLoading == false)
            {
                finishedLoading = true;
                FinishedLoading();
            }
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
            if (!doingWork)
            {
                if (Settings.settings.GameName != "BTD5")
                {
                    Settings.settings.GameName = "BTD5";
                    Settings.SaveSettings();
                    ResetGamePictures();
                    BTD5_Image.Source = new BitmapImage(new Uri("Resources/btd5.png", UriKind.Relative));

                    GameHandling();
                }
            }
            else
                Log.Output("TD Loader is currently doing something else. Please wait...");
        }
        private void BTDB_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!doingWork)
            {
                if (Settings.settings.GameName != "BTDB")
                {
                    Settings.settings.GameName = "BTDB";
                    Settings.SaveSettings();
                    ResetGamePictures();
                    BTDB_Image.Source = new BitmapImage(new Uri("Resources/btdb 2.png", UriKind.Relative));

                    GameHandling();
                }
            }
            else
                Log.Output("TD Loader is currently doing something else. Please wait...");
            
        }
        private void BMC_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!doingWork)
            {
                if (Settings.settings.GameName != "BMC")
                {

                    Settings.settings.GameName = "BMC";
                    Settings.SaveSettings();
                    ResetGamePictures();
                    BMC_Image.Source = new BitmapImage(new Uri("Resources/bmc.png", UriKind.Relative));

                    GameHandling();
                }
            }
            else
                Log.Output("TD Loader is currently doing something else. Please wait...");
        }
        private void Mods_Tab_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            //Main_TabController.Items.Add()
        }
    }
}
