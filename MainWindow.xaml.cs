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
        bool doingWork = false;
        public static MainWindow instance;

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

            
            string modsDir = Settings.GetModsDir(Settings.settings.GameName);

            if((Settings.settings.GameName != "" && Settings.settings.GameName != null) && (modsDir == "" || modsDir == null))
                Game.SetModsDir(Settings.settings.GameName);

            bool valid = Game.VerifyBackup(Settings.settings.GameName);
            if(!valid)
            {
                string backupdir = Settings.GetBackupDir(Settings.settings.GameName);
                if (backupdir == "" || backupdir == null)
                    Game.CreateBackupDir(Settings.settings.GameName);

                await Game.CreateBackupAsync(Settings.settings.GameName);
                Log.Output("Done making backup");
            }

            Mods_ListBox.Items.Clear();
            PopulateMods(Settings.settings.GameName);
            doingWork = false;
        }
        private void PopulateMods(string game)
        {
            var mods = new DirectoryInfo(Settings.GetModsDir(game)).GetFiles("*.*");
            foreach (var mod in mods)
            {
                if(mod.Name.EndsWith(".jet") || mod.Name.EndsWith(".zip") || mod.Name.EndsWith(".rar") || mod.Name.EndsWith(".7z"))
                {
                    if (!Mods_ListBox.Items.Contains(mod))
                    {
                        CheckBox a = new CheckBox();
                        a.Content = mod.Name;
                        a.Foreground = Brushes.White;

                        if (Settings.GetLoadedMods(game).Contains(mod.Name))
                            a.IsChecked = true;
                        Mods_ListBox.Items.Add(a);
                    }
                }
            }
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
        private void AddMods_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!doingWork)
            {
                doingWork = true;
                if (Settings.settings.GameName != "" && Settings.settings.GameName != null)
                {
                    List<string> mods = Mods.AddMods();
                    string modD = Settings.GetModsDir(Settings.settings.GameName);
                    if (modD == "" || modD == null)
                    {
                        Log.Output("Your mods directory is invalid");
                        Game.SetModsDir(Settings.settings.GameName);
                    }

                    if (modD != "" && modD != null)
                    {
                        foreach (string mod in mods)
                        {
                            string[] split = mod.Split('\\');
                            string filename = split[split.Length - 1];
                            string copiedMod = Mods.CopyMod(mod, modD + "\\" + filename);

                            if (copiedMod != "")
                            {
                                FileInfo f = new FileInfo(copiedMod);

                                CheckBox a = new CheckBox();
                                a.Content = f.Name;
                                a.Foreground = Brushes.White;
                                Mods_ListBox.Items.Add(a);
                            }
                        }
                    }
                    else
                        Log.Output("Mods directory not found... Please try again");
                }
                else
                    Log.ForceOutput("You need to choose a game before you can add mods!");

                doingWork = false;
            }
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

                    if (Settings.settings.BTD5Dir == "" || Settings.settings.BTD5Dir == null)
                    {
                        string path = Game.SetGameDir(Settings.settings.GameName);

                        if (path == "" || path == null)
                            Log.Output("Something went wrong... Failed to aquire game directory...");
                        else
                        {
                            Log.Output("You selected " + path + " for your game directory");
                            Settings.settings.BTD5Dir = path;
                            Settings.SaveSettings();
                        }
                    }

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

                    if (Settings.settings.BTDBDir == "" || Settings.settings.BTDBDir == null)
                    {
                        string path = Game.SetGameDir(Settings.settings.GameName);

                        if (path == "" || path == null)
                            Log.Output("Something went wrong... Failed to aquire game directory...");
                        else
                        {
                            Log.Output("You selected " + path + " for your game directory");
                            Settings.settings.BTDBDir = path;
                            Settings.SaveSettings();
                        }
                    }

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

                    if (Settings.settings.BMCDir == "" || Settings.settings.BMCDir == null)
                    {
                        string path = Game.SetGameDir(Settings.settings.GameName);

                        if (path == "" || path == null)
                            Log.Output("Something went wrong... Failed to aquire game directory...");
                        else
                        {
                            Log.Output("You selected " + path + " for your game directory");
                            Settings.settings.BMCDir = path;
                            Settings.SaveSettings();
                        }
                    }

                    GameHandling();
                }
            }
            else
                Log.Output("TD Loader is currently doing something else. Please wait...");
        }

        
    }
}
