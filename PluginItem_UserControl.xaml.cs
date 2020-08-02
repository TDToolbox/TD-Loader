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
using BTD_Backend;
using BTD_Backend.NKHook5;

namespace TD_Loader
{
    /// <summary>
    /// Interaction logic for ModItem_UserControl.xaml
    /// </summary>
    public partial class PluginItem_UserControl : UserControl
    {
        public string modName = "";
        public string modPath = "";

        public PluginItem_UserControl()
        {
            InitializeComponent();
            Plugins_UserControl.instance.SelectedPlugins_ListBox.FontSize = 19;
        }

        private void CheckBox_Clicked(object sender, RoutedEventArgs e)
        {
            if (TempGuard.IsDoingWork(MainWindow.workType))
                return;

            CheckBox cb = (CheckBox)(sender);

            if (cb.IsChecked == true)
            {
                // Is checked
                if (!Plugins_UserControl.instance.SelectedPlugins_ListBox.Items.Contains(modName))
                {
                    if (!File.Exists(NKHook5Manager.nkhDir + "\\Plugins\\" + modName))
                    {
                        if (File.Exists(NKHook5Manager.nkhDir + "\\UnloadedPlugins\\" + modName))
                        {
                            string dest = Mods.IncrementName(NKHook5Manager.nkhDir + "\\Plugins\\" + modName);
                            File.Move(NKHook5Manager.nkhDir + "\\UnloadedPlugins\\" + modName, dest);
                        }
                    }
                    
                    Plugins_UserControl.instance.SelectedPlugins_ListBox.Items.Add(modName);
                    Plugins_UserControl.instance.SelectedPlugins_ListBox.SelectedIndex = Plugins_UserControl.instance.SelectedPlugins_ListBox.Items.Count - 1;
                }
            }
            else
            {
                // Is not checked
                if (Plugins_UserControl.instance.SelectedPlugins_ListBox.Items.Contains(modName))
                {
                    if (File.Exists(NKHook5Manager.nkhDir + "\\Plugins\\" + modName))
                    {
                        if (File.Exists(NKHook5Manager.nkhDir + "\\UnloadedPlugins\\" + modName))
                            File.Delete(NKHook5Manager.nkhDir + "\\UnloadedPlugins\\" + modName);

                        string dest = Mods.IncrementName(NKHook5Manager.nkhDir + "\\UnloadedPlugins\\" + modName);
                        File.Move(NKHook5Manager.nkhDir + "\\Plugins\\" + modName, dest);
                    }

                    int selected = Plugins_UserControl.instance.SelectedPlugins_ListBox.SelectedIndex;
                    Plugins_UserControl.instance.SelectedPlugins_ListBox.Items.Remove(modName);

                    if (selected == 0 && Plugins_UserControl.instance.SelectedPlugins_ListBox.Items.Count >= 1)
                        Plugins_UserControl.instance.SelectedPlugins_ListBox.SelectedIndex = selected;
                    else if (Plugins_UserControl.instance.SelectedPlugins_ListBox.Items.Count > 1)
                        Plugins_UserControl.instance.SelectedPlugins_ListBox.SelectedIndex = selected - 1;
                }
            }
        }

        public override string ToString()
        {
            return modPath;
        }

        private void SplitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This button is currently disabled. Check back on the next release");
        }

        private void ButtonChrome_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("This button is currently disabled. Check back on the next release");
        }
    }
}
