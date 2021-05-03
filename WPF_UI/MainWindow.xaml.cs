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

        private void DecodeRadioChecked(object sender, RoutedEventArgs e)
        {
            var radio = sender as RadioButton;
            if (radio is null || !radio.IsChecked.HasValue || !radio.IsChecked.Value) return;
        }

        private void EncodeRadioChecked(object sender, RoutedEventArgs e)
        {
            var radio = sender as RadioButton;
            if (radio is null || !radio.IsChecked.HasValue || !radio.IsChecked.Value) return;
        }

        private void SelectBtnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                pathTB.Text = openFileDialog.FileName;
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
                    await _huffmanService.EncodeAsync(pathTB.Text);
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
    }
}
