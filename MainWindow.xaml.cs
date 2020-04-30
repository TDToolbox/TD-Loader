using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
            JetReader.FinishedStagingMods += JetReader_FinishedStagingMods;
        }

        private void Startup()
        {
            Settings.LoadSettings();

            mods_User = new Mods_UserControl();
            var tab = new TabItem();
            tab.Header = "     Mods     ";
            tab.Padding = new Thickness(5);
            tab.FontSize = 25;
            tab.Content = mods_User;//new Mods_UserControl();
            Main_TabController.Items[1] = tab;

        }
        private void FinishedLoading()
        {
            bool dirNotFound = false;
            if (!Guard.IsStringValid(Settings.game.GameDir))
            {
                dirNotFound = true;
            }

            switch (Settings.game.GameName)
            {
                case "BTD5":
                        BTD5_Image.Source = new BitmapImage(new Uri("Resources/btd5.png", UriKind.Relative));
                    break;
                case "BTDB":
                        BTDB_Image.Source = new BitmapImage(new Uri("Resources/btdb 2.png", UriKind.Relative));
                    break;
                case "BMC":
                        BMC_Image.Source = new BitmapImage(new Uri("Resources/bmc.png", UriKind.Relative));
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
                Settings.game = null;
                Settings.SaveSettings();
            }
        }
        private async void GameHandling()
        {
            if(Settings.game == null)
            {
                return;
            }

            doingWork = true;
            Settings.SetGameFile();
            Settings.SaveSettings();         


            //
            //Check game dir
            bool error = false;
            string gameD = Settings.game.GameDir;
            if (Guard.IsStringValid(gameD))
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
                MessageBox.Show("Some setup is required before you can use mods with this game. Please be patient and read the following messages to " +
                    "make sure it sets up properly. This will take up to 2 minutes");
                string dir = Game.SetGameDir(Settings.settings.GameName);
                if (dir != "" && dir != null)
                {
                    Settings.game.GameDir = dir;
                    Settings.SaveGameFile();
                }
                else
                {
                    Log.Output("Something went wrong... Failed to aquire game directory...");
                    ResetGamePictures();
                    return;
                }
            }



            //
            //Check for Game Updated
            //Get Game Version if it wasnt 
            if (!Guard.IsStringValid(Settings.game.GameVersion))
            {
                Settings.game.GameVersion = Game.GetVersion(Settings.settings.GameName);
                Settings.SaveGameFile();
                Settings.SaveSettings();
            }
            else
            {
                string version = Game.GetVersion(Settings.settings.GameName);
                if (version != Settings.game.GameVersion)
                {
                    MessageBox.Show("Game has been updated... Reaquiring files...");
                    Log.Output("Game has been updated... Reaquiring files...");
                    string backupdir = Settings.game.GameBackupDir;
                    if (backupdir == "" || backupdir == null)
                        Game.CreateBackupDir(Settings.settings.GameName);

                    await Game.CreateBackupAsync(Settings.settings.GameName);
                    Log.Output("Done making backup");

                    Settings.game.GameVersion = version;
                    Settings.SaveGameFile();

                    if (Settings.settings.GameName == "BTDB")
                    {
                        Settings.settings.DidBtdbUpdate = true;
                        Settings.SaveSettings();

                        Zip original = new Zip(Settings.settings.BTDBBackupDir + "\\Assets\\data.jet");
                        Thread thread = new Thread(delegate () { original.GetPassword(); });
                        thread.Start();
                    }
                }
            }



            //
            //Check Mods Dir
            string modsDir = Settings.game.ModsDir;
            if((Settings.settings.GameName != "" && Settings.settings.GameName != null) && (modsDir == "" || modsDir == null))
                Game.SetModsDir(Settings.settings.GameName);


            //
            //Check Backup
            bool valid = Game.VerifyBackup(Settings.settings.GameName);
            if(!valid)
            {
                string backupdir = Settings.game.GameBackupDir;
                if (backupdir == "" || backupdir == null)
                    Game.CreateBackupDir(Settings.settings.GameName);

                await Game.CreateBackupAsync(Settings.settings.GameName);
                Log.Output("Done making backup");
            }

            //
            //Clear mods list
            Mods_UserControl.instance.PopulateMods(Settings.settings.GameName);
            Mods_UserControl.instance.Mods_TextBlock.Text = Settings.settings.GameName + " Mods";
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
            Environment.Exit(1);
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
            if (Settings.game == null)
                Settings.SetGameFile();

            if (Settings.game.GameName != "BMC")
            {
                if (!BMC_Image.IsMouseOver)
                    BMC_Image.Source = new BitmapImage(new Uri("Resources/bmc_not loaded.png", UriKind.Relative));
                else
                    BMC_Image.Source = new BitmapImage(new Uri("Resources/bmc.png", UriKind.Relative));
            }
        }
        private void BTDB_Image_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Settings.game == null)
                Settings.SetGameFile();

            if (Settings.game.GameName != "BTDB")
            {
                if (!BTDB_Image.IsMouseOver)
                    BTDB_Image.Source = new BitmapImage(new Uri("Resources/btdb 2_not loaded.png", UriKind.Relative));
                else
                    BTDB_Image.Source = new BitmapImage(new Uri("Resources/btdb 2.png", UriKind.Relative));
            }                
        }
        private void BTD5_Image_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Settings.game == null)
                Settings.SetGameFile();

            if (Settings.game.GameName != "BTD5")
            {
                if (!BTD5_Image.IsMouseOver)
                    BTD5_Image.Source = new BitmapImage(new Uri("Resources/btd5_not loaded.png", UriKind.Relative));
                else
                    BTD5_Image.Source = new BitmapImage(new Uri("Resources/btd5.png", UriKind.Relative));
            }
        }
        private void BTD5_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (doingWork)
            {
                MessageBox.Show("Currently doing something else. Please wait...");
                Log.Output("TD Loader is currently doing something else. Please wait...");
                return;
            }

            if (Settings.game.GameName != "BTD5")
            {
                ResetGamePictures();
                Settings.settings.GameName = "BTD5";
                BTD5_Image.Source = new BitmapImage(new Uri("Resources/btd5.png", UriKind.Relative));

                GameHandling();
            }
        }
        private void BTDB_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(doingWork)
            {
                MessageBox.Show("Currently doing something else. Please wait...");
                Log.Output("TD Loader is currently doing something else. Please wait...");
                return;
            }

            if (Settings.settings.GameName != "BTDB")
            {
                ResetGamePictures();
                Settings.settings.GameName = "BTDB";
                BTDB_Image.Source = new BitmapImage(new Uri("Resources/btdb 2.png", UriKind.Relative));

                GameHandling();
            }

        }
        private void BMC_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (doingWork)
            {
                MessageBox.Show("Currently doing something else. Please wait...");
                Log.Output("TD Loader is currently doing something else. Please wait...");
                return;
            }

            if (Settings.settings.GameName != "BMC")
            {
                ResetGamePictures();
                Settings.settings.GameName = "BMC";
                BMC_Image.Source = new BitmapImage(new Uri("Resources/bmc.png", UriKind.Relative));

                GameHandling();
            }
        }
        private void Mods_Tab_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            //Main_TabController.Items.Add()
        }


        private void Launch_Button_Clicked(object sender, RoutedEventArgs e)
        {
            if (doingWork == true)
            {
                MessageBox.Show("Cant do that! Currently doing something else");
                return;
            }

            if(mods_User.SelectedMods_ListBox.Items.Count > 0)
            {
                MessageBox.Show("Beginning to merge mods. Please wait, this will take up to 5 seconds per mod. The program is not frozen...");
                doingWork = true;

                Settings.game.LoadedMods = mods_User.modPaths;
                Settings.SaveGameFile();
                Settings.SaveSettings();

                JetReader jet = new JetReader();
                Thread thread = new Thread(delegate () { jet.DoWork(); });
                thread.Start();
            }
            else
            {
                Log.Output("You chose to play with no mods... Launching game");
                LaunchGame();
            }
        }
        private void JetReader_FinishedStagingMods(object sender, EventArgs e)
        {
            LaunchGame();
        }
        private void LaunchGame()
        {
            ulong apiId = 0;
            if (!Guard.IsStringValid(Settings.game.GameName))
            {
                MessageBox.Show("Failed to get game name for game. Unable to launch");
                return;
            }

            switch (Settings.game.GameName)
            {
                
                case "BTD5":
                    apiId = Steam.BTD5AppID;
                    break;
                case "BTDB":
                    apiId = Steam.BTDBAppID;
                    break;
                case "BMC":
                    apiId = Steam.BMCAppID;
                    break;
            }

            if(!Guard.IsStringValid(apiId.ToString()))
            {
                MessageBox.Show("Failed to get API ID for game. Unable to launch");
                return;
            }

            Process.Start("steam://Launch/" + apiId);
        }
    }
}
