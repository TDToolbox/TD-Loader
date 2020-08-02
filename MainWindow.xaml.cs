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
using System.Windows.Threading;
using TD_Loader.Classes;
using BTD_Backend;
using BTD_Backend.Web;
using BTD_Backend.NKHook5;
using BTD_Backend.Persistence;
using TD_Loader.UserControls;

namespace TD_Loader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public event ProcFinished_BoolEventHandler FinishedGameHandling;

        bool checkedNKH = false;
        bool finishedLoading = false;
        public static bool doingWork = false;
        public static bool exit = false;
        public static bool resetGameFiles = false;
        public static string workType = "";
        public static MainWindow instance;
        public static Mods_UserControl mods_User;
        public static Plugins_UserControl plugin_User;
        public MainWindow()
        {
            InitializeComponent();
            instance = this;
            
            Startup();
            
            
            
            

            Main.Closing += Main_Closing;
            JetReader.FinishedStagingMods += JetReader_FinishedStagingMods;
            Log.MessageLogged += Log_MessageLogged;
        }

        private void Log_MessageLogged(object sender, Log.LogEvents e)
        {
            if (e.UseMsgBox)
                MessageBox.Show(e.Message);
            else
            {
                OutputLog.Dispatcher.BeginInvoke((Action)(() =>
                {
                    OutputLog.AppendText(e.Message);
                    OutputLog.ScrollToEnd();
                }));
            }
        }

        private void Startup()
        {
            Settings.LoadSettings();
            if (Settings.game == null)
                Settings.SetGameFile(Settings.settings.GameName);

            Log.Output("Program initializing...");

            //removed for cleanup
            //
            //ResetGamePictures();
            //CreateModsTab();
            //ShowHidePlugins();


            /*UpdateHandler update = new UpdateHandler()
            {
                GitApiReleasesURL = "https://api.github.com/repos/TDToolbox/TD-Loader/releases",
                ProjectExePath = Environment.CurrentDirectory + "\\TD Loader.exe",
                InstallDirectory = Environment.CurrentDirectory,
                ProjectName = "TD Loader",
                UpdatedZipName = "TD Loader.zip"
            };
            BgThread.AddToQueue(() => update.HandleUpdates(false));*/

            FinishedGameHandling += MainWindow_FinishedGameHandling;
        }
        private void FinishedLoading()
        {
            string tdloaderDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TD Loader";
            UserData.MainProgramExePath = Environment.CurrentDirectory + "\\TD Loader.exe";
            UserData.MainProgramName = "TD Loader";
            UserData.MainSettingsDir = tdloaderDir;
            UserData.UserDataFilePath = tdloaderDir + "\\userdata.json";

            BgThread.AddToQueue(() =>
            {
                UserData.LoadUserData();
                UserData.SaveUserData();
            });
            


            if (!Guard.IsStringValid(Settings.game.GameDir) || !Guard.IsStringValid(Settings.game.GameName))
            {
                Settings.settings.GameName = "";
                Settings.SaveSettings();
                Settings.game = null;
                return;
            }

            //removed for cleanup
            //
            /*switch (Settings.game.GameName)
            {
                case "BTD6":
                    BTD6_Image.Source = new BitmapImage(new Uri("Resources/btd6.png", UriKind.Relative));
                    break;
                case "BTD5":
                        BTD5_Image.Source = new BitmapImage(new Uri("Resources/btd5.png", UriKind.Relative));
                    break;
                case "BTDB":
                        BTDB_Image.Source = new BitmapImage(new Uri("Resources/btdb 2.png", UriKind.Relative));
                    break;
                case "BMC":
                        BMC_Image.Source = new BitmapImage(new Uri("Resources/bmc.png", UriKind.Relative));
                    break;
            }*/
            GameHandling();
        }
        private void GameHandling()
        {
            if (Settings.game == null)
                return;

            Settings.SaveGameFile();
            doingWork = true;

            Log.Output("Game: " + Settings.game.GameName);
            workType = "Initializing mod loader for " + Settings.game.GameName;
            ShowHidePlugins();
            new Thread(() =>
            {
                Game.CloseGameIfOpen(Settings.game.GameName);
                Game.ValidateGameDir();
                Game.WasGameUpdated();

                if (!Guard.IsStringValid(Settings.game.ModsDir))
                    Game.SetModsDir(Settings.game.GameName);

                Game.ValidateBackup();

                NKHook5Manager.HandleUpdates();

                doingWork = false;
                workType = "";

                if (FinishedGameHandling != null)
                    FinishedGameHandling(this, new ProcFinished_BoolEventArgs(true));
                
            }).Start();
        }
        private void MainWindow_FinishedGameHandling(object source, ProcFinished_BoolEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                ShowHidePlugins();
                Mods_UserControl.instance.PopulateMods(Settings.game.GameName);
                Mods_UserControl.instance.Mods_TextBlock.Text = Settings.game.GameName + " Mods";

                
                CreatePluginsTab();
            }));
            
        }

        //
        //Main events
        //
        public static async Task Wait(int time)
        {
            await Task.Delay(time);
        }
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
            Process.GetCurrentProcess().Kill();
        }
        private void Launch_Button_Clicked(object sender, RoutedEventArgs e)
        {
            if (TempGuard.IsDoingWork(workType))
                return;

            Game.DoLaunchWithMods();
        }
        private void JetReader_FinishedStagingMods(object sender, EventArgs e)
        {
            Game.LaunchGame();
        }



        //
        //UI events
        //
        private void CreateModsTab()
        {
            mods_User = new Mods_UserControl();
            var tab = new TabItem();
            tab.Header = "     Mods     ";
            tab.Padding = new Thickness(5);
            tab.FontSize = 25;
            tab.Content = mods_User;
            Main_TabController.Items[1] = tab;
        }
        private void CreatePluginsTab()
        {
            plugin_User = new Plugins_UserControl()
            {
                isPlugins = true
            };
            Plugins_Tab.Content = plugin_User;
        }
        private void ShowHidePlugins()
        {
            /*if (Settings.game.GameName == "BTD5" && Settings.game != null)
            {
                Plugins_Tab.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Plugins_Tab.Visibility = Visibility.Visible; }));
                LaunchGrid.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { LaunchGrid.MinHeight = 195; }));
            }
            else
            {
                Plugins_Tab.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { Plugins_Tab.Visibility = Visibility.Collapsed; }));
                LaunchGrid.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { LaunchGrid.MinHeight = 165; }));
            }*/
        }
        private void ResetGameFiles_CB_Click(object sender, RoutedEventArgs e)
        {
            resetGameFiles = !resetGameFiles;
        }

        private void ConsoleColapsed(object sender, RoutedEventArgs e)
        {
            if (OutputLog.Visibility == Visibility.Collapsed)
            {
                OutputLog.Visibility = Visibility.Visible;
                CollapseConsole_Button.Content = "Hide Console";
            }
            else
            {
                OutputLog.Visibility = Visibility.Collapsed;
                CollapseConsole_Button.Content = "Show Console";
            }
        }

        //removed for cleanup
        //
        /*private void BTD6_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TempGuard.IsDoingWork(workType))
                return;

            if (Settings.game == null || Settings.game.GameName != "BTD6")
            {
                ResetGamePictures();
                Settings.SaveGameFile();
                Settings.SetGameFile("BTD6");
                BTD6_Image.Source = new BitmapImage(new Uri("Resources/btd6.png", UriKind.Relative));

                GameHandling();
            }
        }
        private void BTD5_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TempGuard.IsDoingWork(workType))
                return;

            if (Settings.game == null || Settings.game.GameName != "BTD5")
            {
                ResetGamePictures();
                Settings.SaveGameFile();
                Settings.SetGameFile("BTD5");
                BTD5_Image.Source = new BitmapImage(new Uri("Resources/btd5.png", UriKind.Relative));

                GameHandling();
            }
        }
        private void BTDB_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TempGuard.IsDoingWork(workType))
                return;

            if (Settings.game == null || Settings.game.GameName != "BTDB")
            {
                ResetGamePictures();
                Settings.SaveGameFile();
                Settings.SetGameFile("BTDB");
                BTDB_Image.Source = new BitmapImage(new Uri("Resources/btdb 2.png", UriKind.Relative));

                GameHandling();
            }

        }
        private void BMC_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TempGuard.IsDoingWork(workType))
                return;

            if (Settings.game == null || Settings.game.GameName != "BMC")
            {
                ResetGamePictures();
                Settings.SaveGameFile();
                Settings.SetGameFile("BMC");
                BMC_Image.Source = new BitmapImage(new Uri("Resources/bmc.png", UriKind.Relative));

                GameHandling();
            }
        }
        private void BMC_Image_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Settings.game == null || Settings.game.GameName != "BMC")
            {
                if (!BMC_Image.IsMouseOver)
                    BMC_Image.Source = new BitmapImage(new Uri("Resources/bmc_not loaded.png", UriKind.Relative));
                else
                    BMC_Image.Source = new BitmapImage(new Uri("Resources/bmc.png", UriKind.Relative));
            }
        }
        private void BTDB_Image_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Settings.game == null || Settings.game.GameName != "BTDB")
            {
                if (!BTDB_Image.IsMouseOver)
                    BTDB_Image.Source = new BitmapImage(new Uri("Resources/btdb 2_not loaded.png", UriKind.Relative));
                else
                    BTDB_Image.Source = new BitmapImage(new Uri("Resources/btdb 2.png", UriKind.Relative));
            }                
        }
        private void BTD5_Image_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Settings.game == null || Settings.game.GameName != "BTD5")
            {
                if (!BTD5_Image.IsMouseOver)
                    BTD5_Image.Source = new BitmapImage(new Uri("Resources/btd5_not loaded.png", UriKind.Relative));
                else
                    BTD5_Image.Source = new BitmapImage(new Uri("Resources/btd5.png", UriKind.Relative));
            }
        }
        private void BTD6_Image_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Settings.game == null || Settings.game.GameName != "BTD6")
            {
                if (!BTD6_Image.IsMouseOver)
                    BTD6_Image.Source = new BitmapImage(new Uri("Resources/btd6_not loaded.png", UriKind.Relative));
                else
                    BTD6_Image.Source = new BitmapImage(new Uri("Resources/btd6.png", UriKind.Relative));
            }
        }*/
    }
}
