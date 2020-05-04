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
            instance = this;

            HighestPriority.IsEnabled = false;
            RaisePriority.IsEnabled = false;
            LowestPriority.IsEnabled = false;
            LowerPriority.IsEnabled = false;
            SelectedMods_ListBox.SelectionChanged += SelectedMods_ListBox_SelectionChanged;
        }
        public void PopulateMods(string game)
        {
            modPaths = new List<string>();
            Mods_ListBox.Items.Clear();
            SelectedMods_ListBox.Items.Clear();
            modItems = new List<ModItem_UserControl>();

            var mods = new DirectoryInfo(Settings.game.ModsDir).GetFiles("*.*");
            foreach (var mod in mods)
            {
                if (mod.Name.EndsWith(".jet") || mod.Name.EndsWith(".zip") || mod.Name.EndsWith(".rar") || mod.Name.EndsWith(".7z"))
                {
                    if (!Mods_ListBox.Items.Contains(mod))
                    {
                        ModItem_UserControl item = new ModItem_UserControl();

                        if((Mods_ListBox.ActualWidth - 31) > 0)
                            item.MinWidth = Mods_ListBox.ActualWidth - 31;
                        item.ModName.Text = mod.Name;

                        Thickness margin = item.Margin;
                        if (Mods_ListBox.Items.Count == 0)
                        {
                            margin.Top = 10;
                            item.Margin = margin;
                        }
                        item.modName = mod.Name;
                        item.modPath = mod.FullName;
                        
                        modItems.Add(item);
                        Mods_ListBox.Items.Add(item);
                    }
                }
            }
            foreach(var selected in Settings.game.LoadedMods)
                AddToSelectedModLB(selected);

            Settings.game.LoadedMods.Clear();
            Settings.game.LoadedMods = modPaths;
            Settings.SaveGameFile();
            Settings.SaveSettings();
            
            SelectedMods_ListBox.SelectedIndex = 0;

        }
        private void AddToSelectedModLB(string modPath)
        {
            if(!File.Exists(modPath) || !Guard.IsStringValid(modPath))
            {
                Log.Output("Attempted to load a selected mod that doesnt exist!");
                if(modPaths.Contains(modPath))
                    modPaths.Remove(modPath);
                return;
            }

            string[] split = modPath.Split('\\');
            string modname = split[split.Length - 1];

            modPaths.Add(modPath);
            SelectedMods_ListBox.Items.Add(modname);

            foreach (var modItem in modItems)
            {
                if (modItem.ToString() == modPath)
                    modItem.Enable_CheckBox.IsChecked = true;
            }
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
            if (Guard.IsDoingWork(MainWindow.workType)) return;

            MainWindow.doingWork = true;
            MainWindow.workType = "Adding mods";
            if (Settings.settings.GameName != "" && Settings.settings.GameName != null)
            {
                List<string> mods = Mods.AddMods();
                string modD = Settings.game.ModsDir;
                if (modD == "" || modD == null)
                {
                    Log.Output("Your mods directory is invalid");
                    Game.SetModsDir(Settings.settings.GameName);
                }

                if (mods != null && (modD != "" && modD != null))
                {
                    MainWindow.workType = "Copying mods";
                    foreach (string mod in mods)
                    {
                        string[] split = mod.Split('\\');
                        string filename = split[split.Length - 1];
                        string copiedMod = Mods.CopyMod(mod, modD + "\\" + filename);

                        if (copiedMod != "" && copiedMod != " ")
                        {
                            FileInfo f = new FileInfo(copiedMod);

                            ModItem_UserControl item = new ModItem_UserControl();
                            item.MinWidth = Mods_ListBox.ActualWidth - 31;
                            item.ModName.Text = f.Name;

                            Thickness margin = item.Margin;
                            if (Mods_ListBox.Items.Count == 0)
                            {
                                margin.Top = 10;
                                item.Margin = margin;
                            }
                            item.modName = f.Name;
                            item.modPath = f.FullName;

                            Mods_ListBox.Items.Add(item);
                        }
                    }
                }
                else
                    Log.Output("Mods directory not found... Please try again");
            }
            else
                Log.ForceOutput("You need to choose a game before you can add mods!");
            
            MainWindow.doingWork = false;
            MainWindow.workType = "";
        }

        public void HandlePriorityButtons()
        {
            HighestPriority.IsEnabled = false;
            RaisePriority.IsEnabled = false;
            LowestPriority.IsEnabled = false;
            LowerPriority.IsEnabled = false;
            
            if(SelectedMods_ListBox.Items.Count == 1)
            {
                SelectedMods_ListBox.SelectedIndex = 0;
            }
            else if (SelectedMods_ListBox.SelectedIndex == SelectedMods_ListBox.Items.Count +1)
            {
                SelectedMods_ListBox.SelectedIndex = SelectedMods_ListBox.Items.Count - 1;
            }
            else if (SelectedMods_ListBox.Items.Count > 1)
            {
                if (SelectedMods_ListBox.SelectedIndex == 0)
                {
                    LowestPriority.IsEnabled = true;
                    LowerPriority.IsEnabled = true;
                }
                else if (SelectedMods_ListBox.SelectedIndex == SelectedMods_ListBox.Items.Count - 1)
                {
                    HighestPriority.IsEnabled = true;
                    RaisePriority.IsEnabled = true;
                }
                else
                {
                    HighestPriority.IsEnabled = true;
                    RaisePriority.IsEnabled = true;
                    LowestPriority.IsEnabled = true;
                    LowerPriority.IsEnabled = true;
                }
            }
        }
        private void SelectedMods_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HandlePriorityButtons();
        }

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
            modPaths.Insert(SelectedMods_ListBox.Items.Count-1, tempPath);


            SelectedMods_ListBox.SelectedIndex = SelectedMods_ListBox.Items.Count-1;
        }

        private void SelectedMods_ListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if(SelectedMods_ListBox.Items.Contains("") || SelectedMods_ListBox.Items.Contains(" "))
            {
                modPaths.Remove("");
                modPaths.Remove(" ");
                SelectedMods_ListBox.Items.Remove("");
                SelectedMods_ListBox.Items.Remove(" ");
            }
        }
    }
}
