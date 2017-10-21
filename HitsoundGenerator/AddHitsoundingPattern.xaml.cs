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
using System.Windows.Shapes;
using System.Globalization;

namespace HitsoundGenerator
{
    /// <summary>
    /// Логика взаимодействия для AddHitsoundingPattern.xaml
    /// </summary>
    public partial class AddHitsoundingPattern : Window
    {
        public ConfiguredHitsound hs;

        public AddHitsoundingPattern()
        {
            InitializeComponent();
        }

        private int parsePeriod(string fraction)
        {
            switch (fraction)
            {
                case "1/1": return 1;
                case "1/2": return 2;
                case "1/3": return 3;
                case "1/4": return 4;
                default: throw new FormatException("Unrecognized fraction");
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var meta = metaTextBox.Text;

                if (!meta.Any())
                {
                    MessageBox.Show("Error, no meta specified");
                    return;
                }

                var period = parsePeriod(divisorComboBox.Text);

                if (meta.Count() % period != 0)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("The meta length does not divide the period. Are you sure?", "Meta length Confirmation", System.Windows.MessageBoxButton.YesNo);
                    if (messageBoxResult != MessageBoxResult.Yes)
                    {
                        return;
                    }
                }

                var startOffset = int.Parse(startOffsetTextbox.Text);
                var endOffset = int.Parse(endOffsetTextbox.Text);

                if (startOffset >= endOffset)
                {
                    MessageBox.Show("Error, start offset should be less than end offset");
                    return;
                }

                hs = new ConfiguredHitsound(meta.ToUpper(), startOffset, endOffset, period);
                DialogResult = true;
                Close();
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private long parseTimingFromClipboard()
        {
            const string format = "mm:ss:fff";

            try
            {
                string text = Clipboard.GetText();

                text = text.Substring(0, text.IndexOf('(') - 1);

                CultureInfo provider = CultureInfo.InvariantCulture;

                DateTime a = DateTime.ParseExact(text, format, provider);
                return a.Millisecond + a.Second * 1000 + a.Minute * 1000 * 60;
            }
            catch (Exception e)
            {
                MessageBox.Show("Unrecognized timing format. Make sure you have copied an object from the osu editor. The format template is " + format);
                return 0;
            }
            
        }
        private void parseFromClipboardIntoStartOffset_Click(object sender, RoutedEventArgs e)
        {
            startOffsetTextbox.Text = parseTimingFromClipboard().ToString();
        }

        private void parseFromClipboardIntoEndOffset_Click(object sender, RoutedEventArgs e)
        {
            endOffsetTextbox.Text = parseTimingFromClipboard().ToString();
        }
    }
}
