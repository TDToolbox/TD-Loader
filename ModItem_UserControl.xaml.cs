using System;
using System.Collections.Generic;
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
    /// Interaction logic for ModItem_UserControl.xaml
    /// </summary>
    public partial class ModItem_UserControl : UserControl
    {
        public string modName = "";
        public string modPath = "";

        public ModItem_UserControl()
        {
            InitializeComponent();
            Mods_UserControl.instance.SelectedMods_ListBox.FontSize = 19;
        }

        private void CheckBox_Clicked(object sender, RoutedEventArgs e)
        {
            if (Guard.IsDoingWork(MainWindow.workType))
                return;

            CheckBox cb = (CheckBox)(sender);

            if (cb.IsChecked == true)
            {
                // Is checked
                if (!Mods_UserControl.instance.SelectedMods_ListBox.Items.Contains(modName))
                {
                    Mods_UserControl.instance.SelectedMods_ListBox.Items.Add(modName);
                    Mods_UserControl.instance.SelectedMods_ListBox.SelectedIndex = Mods_UserControl.instance.SelectedMods_ListBox.Items.Count - 1;
                    Mods_UserControl.instance.modPaths.Add(modPath);
                }
                
            }
            else
            {
                // Is not checked
                if (Mods_UserControl.instance.SelectedMods_ListBox.Items.Contains(modName))
                {
                    int selected = Mods_UserControl.instance.SelectedMods_ListBox.SelectedIndex;
                    Mods_UserControl.instance.SelectedMods_ListBox.Items.Remove(modName);
                    Mods_UserControl.instance.modPaths.Remove(modPath);

                    if (selected == 0 && Mods_UserControl.instance.SelectedMods_ListBox.Items.Count >= 1)
                        Mods_UserControl.instance.SelectedMods_ListBox.SelectedIndex = selected;
                    else if (Mods_UserControl.instance.SelectedMods_ListBox.Items.Count > 1)
                        Mods_UserControl.instance.SelectedMods_ListBox.SelectedIndex = selected - 1;
                }
            }

            Mods_UserControl.instance.HandlePriorityButtons();
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
