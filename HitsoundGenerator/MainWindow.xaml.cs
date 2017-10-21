using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;
using BeatmapLib;
using System.IO;

namespace HitsoundGenerator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string mapPath;
        private Beatmap baseBeatmap;
        public ObservableCollection<ConfiguredHitsound> Patterns { get; set; }
        public BeatmapGenerator generator;

        public MainWindow()
        {
            Patterns = new ObservableCollection<ConfiguredHitsound>();
            DataContext = this;
            InitializeComponent();
        }

        private bool mapSelected()
        {
            return baseBeatmap != null;
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void initializeBeatmap()
        {
            generator = new BeatmapGenerator(baseBeatmap);
        }

        private void osuSelectButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog osuFileDialog = new OpenFileDialog();
            osuFileDialog.Filter = "osu! beatmap file|*.osu";

            if (osuFileDialog.ShowDialog() ?? true)
            {
                mapPath = osuFileDialog.FileName;
                baseBeatmap = new Beatmap(mapPath);
                songArtist.Content = baseBeatmap.Artist;
                songTitle.Content = baseBeatmap.Title;
                difficultyNameTextbox.Text = baseBeatmap.Version + "+hitsounds";
                initializeBeatmap();
            }
        }

        private void generateBeatmapButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckMapForSavingWithAlert())
                return;

            try
            {
                SaveCurrentMap();
                MessageBox.Show("Map saved");
            }
            catch (Exception ex)
            {
                MessageBox.Show("My appologies. Something went wrong while constructing the map.\n" + ex.ToString());
            }


        }

        private void deletePatternButton_Click(object sender, RoutedEventArgs e)
        {
            var items = ConfiguredHitsounds.SelectedItems;
            if (items.Count > 1)
            {
                for (int i = items.Count - 1; i >= 0; i--)
                    Patterns.Remove((ConfiguredHitsound)items[i]);
                ConfiguredHitsounds.SelectedIndex = -1;
                return;
            }

            int index = ConfiguredHitsounds.SelectedIndex;
            if (index == -1)
                MessageBox.Show("Please select a pattern");
            else
            {
                Patterns.RemoveAt(index);

                if (Patterns.Count > 0)
                {
                    if (index < Patterns.Count)
                        ConfiguredHitsounds.SelectedIndex = index;
                    else
                        ConfiguredHitsounds.SelectedIndex = index - 1;
                    ListViewItem item = ConfiguredHitsounds.ItemContainerGenerator.ContainerFromIndex(ConfiguredHitsounds.SelectedIndex) as ListViewItem;
                    item.Focus();
                }

            }
        }

        private void loadConfigButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog selectConfigDialogue = new OpenFileDialog();
            selectConfigDialogue.Filter = "osu!oABC|*.xml";
            selectConfigDialogue.Multiselect = false;

            /*if (selectConfigDialogue.ShowDialog() ?? true)
            {
                var obj = ConfigStorage.readFromFile(selectConfigDialogue.FileName);

                SelectConfig selectConfig = new SelectConfig(obj);

                if (selectConfig.ShowDialog() ?? true)
                {
                    foreach (var p in selectConfig.selected.patterns)
                        Patterns.Add(p);
                    difficultyNameTextbox.Text = selectConfig.selected.name;
                    beatmapStats = selectConfig.selected.beatmapStats;
                }
            }*/
        }

        private void saveConfigButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialogue = new SaveFileDialog();
            saveFileDialogue.Filter = "osu!oABC config|*.xml";

            /*if (saveFileDialogue.ShowDialog() ?? true)
            {
                var storage = new ConfigStorage();
                storage.configs.Add(new PatternConfiguration(difficultyNameTextbox.Text, beatmapStats, Patterns.ToList()));
                ConfigStorage.saveToFile(saveFileDialogue.FileName,storage);
                MessageBox.Show("Configuration saved!");
            }*/
        }

        private void clearListButton_Click(object sender, RoutedEventArgs e)
        {
            Patterns.Clear();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                deletePatternButton_Click(sender, e);
                return;
            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string help = "Please refer to https://github.com/firedigger/TODO!";
            MessageBox.Show(help, "Help");
        }

        private void SaveCurrentMap(bool addHitsounds = true)
        {
            if (addHitsounds)
                generator.addHitsounds(Patterns);
            Beatmap generatedMap = generator.exportBeatmap();

            generatedMap.Version = difficultyNameTextbox.Text;
            generatedMap.regenerateFilename();

            if (File.Exists(generatedMap.Filename))
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("An .osu file with such name already exists. Keep in mind that it is unsafe to override your existing handcrafted beatmap with a generated one and could cause data loss.", "Override confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    generatedMap.Save(generatedMap.Filename);
                }
            }
            else
                generatedMap.Save(generatedMap.Filename);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (CheckMapForSavingWithAlert())
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("This will clear all the existing(including prior to using the program) hitounds from the map. Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    generator.clearHitsounds();
                    SaveCurrentMap(false);
                }
            }
        }

        private bool CheckMapForSavingWithAlert()
        {
            if (!mapSelected())
            {
                MessageBox.Show("You must select .osu file first!");
                return false;
            }

            if (difficultyNameTextbox.Text.Length == 0)
            {
                MessageBox.Show("Please enter the difficulty name!");
                return false;
            }
            return true;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void addHitsoundMeta_Click(object sender, RoutedEventArgs e)
        {
            AddHitsoundingPattern dialog = new AddHitsoundingPattern();
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                Patterns.Add(dialog.hs);
            }
        }
    }
}
