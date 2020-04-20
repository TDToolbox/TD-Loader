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

namespace TD_Loader
{
    /// <summary>
    /// Interaction logic for ModItem_UserControl.xaml
    /// </summary>
    public partial class ModItem_UserControl : UserControl
    {
        public string modName = "";
        public string modPath = "";
        public bool enabled = false;

        public ModItem_UserControl()
        {
            InitializeComponent();
            Mods_UserControl.instance.SelectedMods_ListBox.FontSize = 19;
        }

        private void Enable_Button_Click(object sender, RoutedEventArgs e)
        {
            enabled = !enabled;

            if (enabled)
            {
                Enable_Button.Background = new SolidColorBrush(Color.FromArgb(255, 104, 201, 58));
                EnableButton_Text.Text = "Enabled";

                if(!Mods_UserControl.instance.SelectedMods_ListBox.Items.Contains(modName))
                {
                    Mods_UserControl.instance.SelectedMods_ListBox.Items.Add(modName);
                    Mods_UserControl.instance.SelectedMods_ListBox.SelectedIndex = Mods_UserControl.instance.SelectedMods_ListBox.Items.Count - 1;
                }
            }
            else
            {
                Enable_Button.Background = new SolidColorBrush(Color.FromArgb(255, 222, 37, 37));
                EnableButton_Text.Text = "Disabled";

                if (Mods_UserControl.instance.SelectedMods_ListBox.Items.Contains(modName))
                {
                    int selected = Mods_UserControl.instance.SelectedMods_ListBox.SelectedIndex;
                    Mods_UserControl.instance.SelectedMods_ListBox.Items.Remove(modName);

                    if(selected == 0 && Mods_UserControl.instance.SelectedMods_ListBox.Items.Count >= 1)
                        Mods_UserControl.instance.SelectedMods_ListBox.SelectedIndex = selected;
                    else if (Mods_UserControl.instance.SelectedMods_ListBox.Items.Count > 1)
                        Mods_UserControl.instance.SelectedMods_ListBox.SelectedIndex = selected - 1;
                }
            }
            
            Mods_UserControl.instance.HandlePriorityButtons();
        }
    }
}
