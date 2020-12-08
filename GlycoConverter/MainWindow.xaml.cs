using Microsoft.Win32;
using PrecursorIonClassLibrary.Averagine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace GlycoConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> files = new List<string>();
        private string output = "";
        private AveragineType type = AveragineType.Peptide;
        private int progressCounter;
        private int readingCounter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MSMSFileNames_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileNamesDialog = new OpenFileDialog();
            fileNamesDialog.Filter = "Raw File|*.raw";
            fileNamesDialog.Title = "Open a MS2 File";
            fileNamesDialog.Multiselect = true;
            fileNamesDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (fileNamesDialog.ShowDialog() == true)
            {
                foreach (string filename in fileNamesDialog.FileNames)
                {
                    if (!files.Contains(filename))
                    {
                        lbFiles.Items.Add(filename);
                        files.Add(filename);
                    }
                }

            }
        }

        private void OutputDir_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

            // Show the FolderBrowserDialog.
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string folderName = folderBrowserDialog.SelectedPath;
                Binding fileNameBinding = new Binding();
                fileNameBinding.Path = new PropertyPath("SelectedPath");
                fileNameBinding.Source = folderBrowserDialog;
                fileNameBinding.Mode = BindingMode.OneWay;
                displayOutput.SetBinding(TextBox.TextProperty, fileNameBinding);
                output = folderName;
            }
        }

        private void DeselectFiles_Click(object sender, RoutedEventArgs e)
        {
            if (lbFiles.SelectedItem != null)
            {
                string filename = lbFiles.SelectedItem.ToString();
                lbFiles.Items.Remove(lbFiles.SelectedItem);
                if (files.Contains(filename))
                    files.Remove(filename);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (files.Count == 0)
            {
                MessageBox.Show("Please select Raw Files!");
                return;
            }
            else if (!Directory.Exists(output))
            {
                MessageBox.Show("The directory not exists!");
                return;
            }

            ButtonRun.IsEnabled = false;

            Counter counter = new Counter();
            ProgressingCounter readCounter = new ProgressingCounter();
            IConverter converter = null;
            if (MGF.IsChecked == true)
            {
                converter = new MGFConverter(counter, readCounter);
            }
            else if (MZML.IsChecked == true)
            {
                converter = new MZMLConverter(counter, readCounter);
            }

            counter.progressChange += SearchProgressChanged;
            readCounter.progressChange += ReadingProgressChanged;
            
            await Task.Run(() => 
            {
                readingCounter = 0;
                foreach (string path in files)
                    converter.ParallelRun(path, output, type);
            });
            ButtonRun.IsEnabled = true;
        }

        private void UpdateProgress()
        {
            Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new ThreadStart(() =>
                {
                    SearchingStatus.Value = progressCounter * 1.0 / files.Count * 1000.0;
                }));
        }

        private void SearchProgressChanged(object sender, EventArgs e)
        {
            Interlocked.Increment(ref progressCounter);
            UpdateProgress();
        }


        private void UpdateReadingProgress(int total)
        {
            Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new ThreadStart(() =>
                {
                    ProgressStatus.Value = readingCounter * 1.0 / total * 1000.0;
                }));
        }

        private void ReadingProgressChanged(object sender, ProgressingEventArgs e)
        {
            Interlocked.Increment(ref readingCounter);
            UpdateReadingProgress(e.Total);
        }

        private void SelectTypes(object sender, RoutedEventArgs e)
        {
            if (Peptides.IsChecked == true)
            {
                type = AveragineType.Peptide;
            }
            else if (Glycopeptides.IsChecked == true)
            {
                type = AveragineType.GlycoPeptide;
            }
            else if (Glycan.IsChecked == true)
            {
                type = AveragineType.Glycan;
            }
            else if (PermethylatedGlycan.IsChecked == true)
            {
                type = AveragineType.PermethylatedGlycan;
            }
        }
    }
}
