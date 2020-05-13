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
    public partial class Plugins_UserControl : UserControl
    {
        public bool isPlugins { get; set; }
        public static Plugins_UserControl instance;
        public List<string> modPaths = new List<string>();
        public List<PluginItem_UserControl> modItems = new List<PluginItem_UserControl>();
        public Plugins_UserControl()
        {
            InitializeComponent();
            instance = this;

            HighestPriority.IsEnabled = false;
            RaisePriority.IsEnabled = false;
            LowestPriority.IsEnabled = false;
            LowerPriority.IsEnabled = false;
            SelectedPlugins_ListBox.SelectionChanged += SelectedPlugins_ListBox_SelectionChanged;
            PopulateMods();
        }
        public void PopulateMods()
        {
            modPaths = new List<string>();
            Plugins_ListBox.Items.Clear();
            SelectedPlugins_ListBox.Items.Clear();
            modItems = new List<PluginItem_UserControl>();


            var mods = new DirectoryInfo(NKHook.nkhDir).GetFiles("*.*", SearchOption.AllDirectories);
            var loadedPlugins = new DirectoryInfo(NKHook.nkhDir + "\\Plugins").GetFiles("*.*");
            foreach (var mod in mods)
            {
                if (!mod.Name.EndsWith(".dll") || mod.Name == "NKHook5.dll" || mod.Name == "NKHook5-CLR.dll")
                    continue;

                if (Plugins_ListBox.Items.Contains(mod))
                    continue;

                Log.Output(mod.Name);

                PluginItem_UserControl item = new PluginItem_UserControl();
                if ((Plugins_ListBox.ActualWidth - 31) > 0)
                    item.MinWidth = Plugins_ListBox.ActualWidth - 31;
                item.ModName.Text = mod.Name;

                Thickness margin = item.Margin;
                if (Plugins_ListBox.Items.Count == 0)
                {
                    margin.Top = 10;
                    item.Margin = margin;
                }
                item.modName = mod.Name;
                item.modPath = mod.FullName;

                modItems.Add(item);
                Plugins_ListBox.Items.Add(item);
            }

            foreach (var item in loadedPlugins)
                AddToSelectedModLB(item.FullName);


            SelectedPlugins_ListBox.SelectedIndex = 0;
        }
        private void AddToSelectedModLB(string modPath)
        {
            FileInfo f = new FileInfo(modPath);

            modPaths.Add(modPath);
            SelectedPlugins_ListBox.Items.Add(f.Name);

            foreach (var modItem in modItems)
            {
                if (modItem.ToString() == modPath)
                    modItem.Enable_CheckBox.IsChecked = true;
            }
        }


        private void ModsUserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (PluginItem_UserControl item in Plugins_ListBox.Items)
            {
                if ((Plugins_ListBox.ActualWidth - 31) > 0)
                    item.MinWidth = Plugins_ListBox.ActualWidth - 31;
            }
        }

        private void AddMods_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Guard.IsDoingWork(MainWindow.workType)) return;

            MainWindow.doingWork = true;
            MainWindow.workType = "Adding Plugins";
            if (!Guard.IsStringValid(Settings.settings.GameName))
            {
                Log.Output("You need to choose a game before you can add mods!");
                MainWindow.doingWork = false;
                return;
            }

            List<string> mods = FileIO.BrowseForFiles("Browse for mods", "", "dll files (*.dll)|*.dll", "");
            if (!Directory.Exists(NKHook.nkhDir + "\\UnloadedPlugins"))
                Directory.CreateDirectory(NKHook.nkhDir + "\\UnloadedPlugins");

            MainWindow.workType = "Copying Plugins";

            if (mods == null || mods.Count <= 0)
            {
                MainWindow.doingWork = false;
                return;
            }

            foreach (string mod in mods)
            {
                FileInfo file = new FileInfo(mod);
                if (!file.Name.EndsWith(".dll"))
                {
                    Log.Output("You selected an invalid plugin. All plugins must end in .dll");
                    continue;
                }
                string dest = Mods.IncrementName(NKHook.nkhDir + "\\UnloadedPlugins\\" + file.Name);
                File.Copy(mod, dest);

                string copiedMod = dest;
                if (!Guard.IsStringValid(copiedMod))
                {
                    Log.Output("Invalid plugin selected");
                    continue;
                }

                FileInfo f = new FileInfo(copiedMod);

                PluginItem_UserControl item = new PluginItem_UserControl();
                item.MinWidth = Plugins_ListBox.ActualWidth - 31;
                item.ModName.Text = f.Name;

                Thickness margin = item.Margin;
                if (Plugins_ListBox.Items.Count == 0)
                {
                    margin.Top = 10;
                    item.Margin = margin;
                }
                item.modName = f.Name;
                item.modPath = f.FullName;

                Plugins_ListBox.Items.Add(item);
                Log.Output("Added " + item);
            }

            MainWindow.doingWork = false;
            MainWindow.workType = "";
        }

        public void HandlePriorityButtons()
        {
            HighestPriority.IsEnabled = false;
            RaisePriority.IsEnabled = false;
            LowestPriority.IsEnabled = false;
            LowerPriority.IsEnabled = false;
            
            if(SelectedPlugins_ListBox.Items.Count == 1)
            {
                SelectedPlugins_ListBox.SelectedIndex = 0;
            }
            else if (SelectedPlugins_ListBox.SelectedIndex == SelectedPlugins_ListBox.Items.Count +1)
            {
                SelectedPlugins_ListBox.SelectedIndex = SelectedPlugins_ListBox.Items.Count - 1;
            }
            else if (SelectedPlugins_ListBox.Items.Count > 1)
            {
                if (SelectedPlugins_ListBox.SelectedIndex == 0)
                {
                    LowestPriority.IsEnabled = true;
                    LowerPriority.IsEnabled = true;
                }
                else if (SelectedPlugins_ListBox.SelectedIndex == SelectedPlugins_ListBox.Items.Count - 1)
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
        private void SelectedPlugins_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

            int index = SelectedPlugins_ListBox.SelectedIndex;
            string temp = SelectedPlugins_ListBox.Items.GetItemAt(index - 1).ToString();
            string tempPath = modPaths[index - 1];

            SelectedPlugins_ListBox.Items[index - 1] = SelectedPlugins_ListBox.SelectedItem;
            modPaths[index - 1] = modPaths[index];

            SelectedPlugins_ListBox.Items[index] = temp;
            modPaths[index] = tempPath;

            SelectedPlugins_ListBox.SelectedIndex = index - 1;
        }
        private void LowerPriority_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.doingWork)
            {
                MessageBox.Show("Cant do that! Currently doing something else... Please wait");
                Log.Output("Cant do that! Currently doing something else... Please wait");
                return;
            }

            int index = SelectedPlugins_ListBox.SelectedIndex;
            string temp = SelectedPlugins_ListBox.Items.GetItemAt(index + 1).ToString();
            string tempPath = modPaths[index + 1];

            SelectedPlugins_ListBox.Items[index + 1] = SelectedPlugins_ListBox.SelectedItem;
            modPaths[index + 1] = modPaths[index];

            SelectedPlugins_ListBox.Items[index] = temp;
            modPaths[index] = tempPath;

            SelectedPlugins_ListBox.SelectedIndex = index + 1;
        }

        private void HighestPriority_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.doingWork)
            {
                MessageBox.Show("Cant do that! Currently doing something else... Please wait");
                Log.Output("Cant do that! Currently doing something else... Please wait");
                return;
            }

            string temp = SelectedPlugins_ListBox.Items.GetItemAt(SelectedPlugins_ListBox.SelectedIndex).ToString();
            var newItems = new DataGrid().Items;
            string tempPath = modPaths[SelectedPlugins_ListBox.SelectedIndex];

            SelectedPlugins_ListBox.Items.RemoveAt(SelectedPlugins_ListBox.SelectedIndex);
            newItems.Add(temp);
            foreach (var item in SelectedPlugins_ListBox.Items)
                newItems.Add(item);

            SelectedPlugins_ListBox.Items.Clear();
            foreach (var i in newItems)
                SelectedPlugins_ListBox.Items.Add(i);

            modPaths.Remove(tempPath);
            modPaths.Insert(0, tempPath);

            SelectedPlugins_ListBox.SelectedIndex = 0;
        }

        private void LowestPriority_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.doingWork)
            {
                MessageBox.Show("Cant do that! Currently doing something else... Please wait");
                Log.Output("Cant do that! Currently doing something else... Please wait");
                return;
            }

            string temp = SelectedPlugins_ListBox.Items.GetItemAt(SelectedPlugins_ListBox.SelectedIndex).ToString();
            var newItems = new DataGrid().Items;
            string tempPath = modPaths[SelectedPlugins_ListBox.SelectedIndex];

            SelectedPlugins_ListBox.Items.RemoveAt(SelectedPlugins_ListBox.SelectedIndex);
            foreach (var item in SelectedPlugins_ListBox.Items)
                newItems.Add(item);
            newItems.Add(temp);

            SelectedPlugins_ListBox.Items.Clear();
            foreach (var i in newItems)
                SelectedPlugins_ListBox.Items.Add(i);


            modPaths.Remove(tempPath);
            modPaths.Insert(SelectedPlugins_ListBox.Items.Count-1, tempPath);


            SelectedPlugins_ListBox.SelectedIndex = SelectedPlugins_ListBox.Items.Count-1;
        }

        private void SelectedPlugins_ListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if(SelectedPlugins_ListBox.Items.Contains("") || SelectedPlugins_ListBox.Items.Contains(" "))
            {
                modPaths.Remove("");
                modPaths.Remove(" ");
                SelectedPlugins_ListBox.Items.Remove("");
                SelectedPlugins_ListBox.Items.Remove(" ");
            }
        }

        private void ModsUserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
