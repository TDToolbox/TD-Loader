using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TD_Loader.Classes;
using BTD_Backend;
using BTD_Backend.Game;
using System.Resources;
using System.Windows.Threading;
using System.Windows.Media;

namespace TD_Loader.UserControls
{
    /// <summary>
    /// Interaction logic for Mods_UserControl.xaml
    /// </summary>
    public partial class Mods_UserControl : UserControl
    {
        public static Mods_UserControl instance;
        public List<string> modPaths = new List<string>();
        public List<ModItem_UserControl> modItems = new List<ModItem_UserControl>();
        public Mods_UserControl()
        {
            InitializeComponent();
            GamesList.GameChanged += GamesList_GameChanged;
            instance = this;

            PopulateMods(SessionData.CurrentGame);
        }

        public void PopulateMods(GameType game)
        {
            string modsDir = TempSettings.Instance.GetModsDir(game);
            if (String.IsNullOrEmpty(modsDir) || !Directory.Exists(modsDir))
                return;

            Mods_ListBox.Items.Clear();
            SelectedMods_ListBox.Items.Clear();
            modPaths = new List<string>();
            modItems = new List<ModItem_UserControl>();

            var mods = new DirectoryInfo(modsDir).GetFiles("*.*");
            List<string> fileExtensions = new List<string>() { ".jet", ".zip", ".rar", ".7z", ".btd6mod" };

            foreach (var mod in mods)
            {
                bool goodExtension = false;
                foreach (var item in fileExtensions)
                {
                    if (!mod.Name.EndsWith(item))
                        continue;
                    
                    goodExtension = true;
                    break;
                }

                if (!goodExtension || Mods_ListBox.Items.Contains(mod))
                    continue;

                AddItemToModsList(mod);
            }

            List<string> TempList = new List<string>();
            foreach (var selected in TempSettings.Instance.LastUsedMods)
            {
                if (!File.Exists(selected) || String.IsNullOrEmpty(selected))
                {
                    Log.Output("Attempted to add a mod that doesnt exist to the Selected Mods list");
                    continue;
                }
                TempList.Add(selected);
                AddToSelectedModLB(selected);
            }

            if (TempList.Count != TempSettings.Instance.LastUsedMods.Count)
                TempSettings.Instance.LastUsedMods = TempList;

            SelectedMods_ListBox.SelectedIndex = 0;
        }

        private void AddToSelectedModLB(string modPath)
        {
            

            FileInfo f = new FileInfo(modPath);

            modPaths.Add(modPath);
            SelectedMods_ListBox.Items.Add(f.Name);

            foreach (var modItem in modItems)
            {
                if (modItem.ToString() == modPath)
                    modItem.Enable_CheckBox.IsChecked = true;
            }

            SelectedMods_ListBox.SelectedIndex = SelectedMods_ListBox.Items.Count - 1;
        }
        

        public void AddItemToModsList(string modPath) => AddItemToModsList(new FileInfo(modPath));
        public void AddItemToModsList(FileInfo modFile)
        {
            if (Mods_ListBox.ActualWidth <= 0)
                return;

            ModItem_UserControl item = new ModItem_UserControl();
            item.MinWidth = Mods_ListBox.ActualWidth - 31;
            item.ModName.Text = modFile.Name;
            item.modName = modFile.Name;
            item.modPath = modFile.FullName;

            Thickness margin = item.Margin;
            if (Mods_ListBox.Items.Count == 0)
            {
                margin.Top = 10;
                item.Margin = margin;
            }
            
            Mods_ListBox.Items.Add(item);
            modItems.Add(item);
        }


        public void HandlePriorityButtons()
        {
            List<Button> priorityButtons = new List<Button>() { HighestPriority, RaisePriority, LowerPriority, LowestPriority };
            foreach (var item in priorityButtons)
                item.IsEnabled = false;

            if (SelectedMods_ListBox.Items.Count <= 1)
                return;

            int index = SelectedMods_ListBox.SelectedIndex;
            if (index == 0)
            {
                priorityButtons[3].IsEnabled = true;
                priorityButtons[2].IsEnabled = true;
            }
            else if (index == SelectedMods_ListBox.Items.Count - 1)
            {
                priorityButtons[0].IsEnabled = true;
                priorityButtons[1].IsEnabled = true;
            }
            else
            {
                foreach (var item in priorityButtons)
                    item.IsEnabled = true;
            }
        }
        

        

        

        private void GamesList_GameChanged(object sender, GamesList.GameListEventArgs e)
        {
            Mods_ListBox.Items.Clear();
            SelectedMods_ListBox.Items.Clear();

            if (SessionData.CurrentGame == GameType.None)
            {
                MainWindow.instance.Mods_Tab.Visibility = Visibility.Collapsed;
                return;
            }

            if (MainWindow.instance.Mods_Tab.Visibility == Visibility.Collapsed)
            {
                MainWindow.instance.Mods_Tab.Visibility = Visibility.Visible;
                blinkTimer.Start();
            }

            PopulateMods(SessionData.CurrentGame);
            Mods_TextBlock.Text = SessionData.CurrentGame.ToString() + " Mods";
        }

        // The timer's Tick event.
        private bool BlinkOn = false;
        private int blinkCount = 0;
        public void FlashTab(object sender, EventArgs e)
        {
            if (blinkCount >= 8)
            {
                BlinkOn = false;
                blinkCount = 0;
                MainWindow.instance.Mods_Tab.Foreground = Brushes.White;
                blinkTimer.Stop();
                return;
            }

            if (BlinkOn)
                MainWindow.instance.Mods_Tab.Foreground = Brushes.Black;
            else
                MainWindow.instance.Mods_Tab.Foreground = Brushes.White;

            BlinkOn = !BlinkOn;
            blinkCount++;
        }


        #region UI Events

        bool controlInitialized = false;
        GameType lastGame = GameType.None;
        DispatcherTimer blinkTimer = new DispatcherTimer();
        private void ModsUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            blinkTimer.Tick += FlashTab;
            blinkTimer.Interval = new TimeSpan(0, 0, 0, 0, 75);

            if (controlInitialized)
            {
                if (SessionData.CurrentGame != GameType.None && SessionData.CurrentGame != lastGame)
                {
                    PopulateMods(SessionData.CurrentGame);
                    lastGame = SessionData.CurrentGame;
                }
            }

            controlInitialized = true;
        }

        private void ModsUserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (ModItem_UserControl item in Mods_ListBox.Items)
            {
                if ((Mods_ListBox.ActualWidth - 31) > 0)
                    item.MinWidth = Mods_ListBox.ActualWidth - 31;
            }
        }


        private void AddMods_Button_Click(object sender, RoutedEventArgs e)
        {
            if (TempGuard.IsDoingWork(MainWindow.workType)) return;

            if (SessionData.CurrentGame == GameType.None)
            {
                Log.Output("You need to choose a game before you can add mods!");
                return;
            }

            if (String.IsNullOrEmpty(TempSettings.Instance.GetModsDir(SessionData.CurrentGame)))
            {
                Game_UC.Instance.SetModsDir();
                if (String.IsNullOrEmpty(TempSettings.Instance.GetModsDir(SessionData.CurrentGame)))
                {
                    Log.Output("Can't add mods. You need to set a mods directory.");
                    return;
                }
            }

            string modFolder = TempSettings.Instance.GetModsDir(SessionData.CurrentGame);
            if (!Directory.Exists(modFolder))
                Directory.CreateDirectory(modFolder);

            MainWindow.doingWork = true;
            MainWindow.workType = "Adding mods";

            string allModTypes = "All Mod Types|*.jet;*.zip;*.rar;*.7z;*.btd6mod";
            List<string> mods = FileIO.BrowseForFiles("Browse for mods", "", allModTypes + "|Jet files (*.jet)|*.jet|Zip files (*.zip)|*.zip|Rar files (*.rar)|*.rar|7z files (*.7z)|*.7z|BTD6 Mods (*.btd6mod)|*.btd6mod", "");

            if (mods == null || mods.Count == 0)
            {
                MainWindow.doingWork = false;
                return;
            }

            foreach (string mod in mods)
            {
                FileInfo f = new FileInfo(mod);
                Log.Output("Added " + f.Name);

                string dest = BTD_Backend.IO.FileIO.IncrementFileName(modFolder + "\\" + f.Name);
                File.Copy(mod, dest);
                f = new FileInfo(dest);
                
                AddItemToModsList(f.FullName);
            }

            MainWindow.doingWork = false;
            MainWindow.workType = "";
        }

        private void SelectedMods_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => HandlePriorityButtons();

        private void RaisePriority_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.doingWork)
            {
                MessageBox.Show("Cant do that! Currently doing something else... Please wait");
                Log.Output("Cant do that! Currently doing something else... Please wait");
                return;
            }

            int index = SelectedMods_ListBox.SelectedIndex;
            string temp = SelectedMods_ListBox.Items.GetItemAt(index - 1).ToString();
            string tempPath = modPaths[index - 1];

            SelectedMods_ListBox.Items[index - 1] = SelectedMods_ListBox.SelectedItem;
            modPaths[index - 1] = modPaths[index];

            SelectedMods_ListBox.Items[index] = temp;
            modPaths[index] = tempPath;

            SelectedMods_ListBox.SelectedIndex = index - 1;
        }
        private void LowerPriority_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.doingWork)
            {
                MessageBox.Show("Cant do that! Currently doing something else... Please wait");
                Log.Output("Cant do that! Currently doing something else... Please wait");
                return;
            }

            int index = SelectedMods_ListBox.SelectedIndex;
            string temp = SelectedMods_ListBox.Items.GetItemAt(index + 1).ToString();
            string tempPath = modPaths[index + 1];

            SelectedMods_ListBox.Items[index + 1] = SelectedMods_ListBox.SelectedItem;
            modPaths[index + 1] = modPaths[index];

            SelectedMods_ListBox.Items[index] = temp;
            modPaths[index] = tempPath;

            SelectedMods_ListBox.SelectedIndex = index + 1;
        }

        private void HighestPriority_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.doingWork)
            {
                MessageBox.Show("Cant do that! Currently doing something else... Please wait");
                Log.Output("Cant do that! Currently doing something else... Please wait");
                return;
            }

            string temp = SelectedMods_ListBox.Items.GetItemAt(SelectedMods_ListBox.SelectedIndex).ToString();
            var newItems = new DataGrid().Items;
            string tempPath = modPaths[SelectedMods_ListBox.SelectedIndex];

            SelectedMods_ListBox.Items.RemoveAt(SelectedMods_ListBox.SelectedIndex);
            newItems.Add(temp);
            foreach (var item in SelectedMods_ListBox.Items)
                newItems.Add(item);

            SelectedMods_ListBox.Items.Clear();
            foreach (var i in newItems)
                SelectedMods_ListBox.Items.Add(i);

            modPaths.Remove(tempPath);
            modPaths.Insert(0, tempPath);

            SelectedMods_ListBox.SelectedIndex = 0;
        }

        private void LowestPriority_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.doingWork)
            {
                MessageBox.Show("Cant do that! Currently doing something else... Please wait");
                Log.Output("Cant do that! Currently doing something else... Please wait");
                return;
            }

            string temp = SelectedMods_ListBox.Items.GetItemAt(SelectedMods_ListBox.SelectedIndex).ToString();
            var newItems = new DataGrid().Items;
            string tempPath = modPaths[SelectedMods_ListBox.SelectedIndex];

            SelectedMods_ListBox.Items.RemoveAt(SelectedMods_ListBox.SelectedIndex);
            foreach (var item in SelectedMods_ListBox.Items)
                newItems.Add(item);
            newItems.Add(temp);

            SelectedMods_ListBox.Items.Clear();
            foreach (var i in newItems)
                SelectedMods_ListBox.Items.Add(i);


            modPaths.Remove(tempPath);
            modPaths.Insert(SelectedMods_ListBox.Items.Count - 1, tempPath);


            SelectedMods_ListBox.SelectedIndex = SelectedMods_ListBox.Items.Count - 1;
        }

        #endregion
    }
}
