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
        public Mods_UserControl()
        {
            InitializeComponent();
            instance = this;
        }

        public void PopulateMods(string game)
        {
            Mods_ListBox.Items.Clear();
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
    }
}
