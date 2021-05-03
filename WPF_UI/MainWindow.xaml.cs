using BL;
using Microsoft.Win32;
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

namespace WPF_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IHuffmanService _huffmanService;

        public MainWindow()
        {
            InitializeComponent();
            this._huffmanService = new HuffmanService();
        }

        private void ChangeBlockSizeEnabled(bool isEnabled)
        {
            blockSizeTB.IsEnabled = blockSizeSlider.IsEnabled = isEnabled;
        }

        private void ChangeBlockSizeValues(long min, long max)
        {
            max = max > int.MaxValue ? int.MaxValue : max;
            blockSizeSlider.Minimum = min;
            blockSizeSlider.Maximum = max;
            blockSizeSlider.Value = min;
            blockSizeTB.Text = min.ToString();
            minLabel.Content = $"Min: {min}";
            maxLabel.Content = $"Max: {max}";
        }

        private void DecodeRadioChecked(object sender, RoutedEventArgs e)
        {
            var radio = sender as RadioButton;
            if (radio is null || !radio.IsChecked.HasValue || !radio.IsChecked.Value) return;

            if (this.IsLoaded) ChangeBlockSizeEnabled(false);
        }

        private void EncodeRadioChecked(object sender, RoutedEventArgs e)
        {
            var radio = sender as RadioButton;
            if (radio is null || !radio.IsChecked.HasValue || !radio.IsChecked.Value) return;

            if (this.IsLoaded) ChangeBlockSizeEnabled(true);
        }

        private void SelectBtnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                pathTB.Text = openFileDialog.FileName;
                ChangeBlockSizeValues(2, new FileInfo(openFileDialog.FileName).Length);
            }
        }

        private bool CheckFileExist(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("The file does not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private async void RunBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                statusLabel.Content = "Status: ";
                runBtn.IsEnabled = selectBtn.IsEnabled = pathTB.IsEnabled = false;

                if (!CheckFileExist(pathTB.Text)) throw new Exception();

                if (encodeRadio.IsChecked.HasValue && encodeRadio.IsChecked.Value)
                    await _huffmanService.EncodeAsync(pathTB.Text, (int)Math.Floor(blockSizeSlider.Value));
                else if (decodeRadio.IsChecked.HasValue && decodeRadio.IsChecked.Value)
                    await _huffmanService.DecodeAsync(pathTB.Text);

                statusLabel.Content = "Status: Ok";
            }
            catch(Exception ex)
            {
                statusLabel.Content = "Status: Error";
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                runBtn.IsEnabled = selectBtn.IsEnabled = pathTB.IsEnabled = true;
            }
        }

        private void PathTBKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                if (CheckFileExist(pathTB.Text))
                    ChangeBlockSizeValues(2, new FileInfo(pathTB.Text).Length);
        }

        private void BlockSizeSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            if (slider is null) return;

            long sliderValue = (long)Math.Floor(slider.Value);
            if(blockSizeTB != null)
                blockSizeTB.Text = sliderValue.ToString();
        }

        private void blockSizeTBKeyDown(object sender, KeyEventArgs e)
        {
            var whiteList = new List<Key> { 
                Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4,
                Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9,
                Key.D0, Key.D1, Key.D2, Key.D3, Key.D4,
                Key.D5, Key.D6, Key.D7, Key.D8, Key.D9,
                Key.Enter, Key.Back
            };

            if (!whiteList.Contains(e.Key)) e.Handled = true;
            else if(e.Key == Key.Enter && long.TryParse(blockSizeTB.Text, out long value))
            {
                if(value >= blockSizeSlider.Minimum && value <= blockSizeSlider.Maximum)
                    blockSizeSlider.Value = value;
                else
                    MessageBox.Show("Incorrect block size", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
