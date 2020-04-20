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
            Mods_ListBox.Items.Clear();
            SelectedMods_ListBox.Items.Clear();
            var mods = new DirectoryInfo(Settings.GetModsDir(game)).GetFiles("*.*");
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
                        Mods_ListBox.Items.Add(item);
                    }
                }
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
            if (!MainWindow.doingWork)
            {
                MainWindow.doingWork = true;
                if (Settings.settings.GameName != "" && Settings.settings.GameName != null)
                {
                    List<string> mods = Mods.AddMods();
                    string modD = Settings.GetModsDir(Settings.settings.GameName);
                    if (modD == "" || modD == null)
                    {
                        Log.Output("Your mods directory is invalid");
                        Game.SetModsDir(Settings.settings.GameName);
                    }

                    if (mods != null && (modD != "" && modD != null))
                    {
                        foreach (string mod in mods)
                        {
                            string[] split = mod.Split('\\');
                            string filename = split[split.Length - 1];
                            string copiedMod = Mods.CopyMod(mod, modD + "\\" + filename);

                            if (copiedMod != "")
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
            }
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
    }
}
