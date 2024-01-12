using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using sqx;


namespace csvrepair
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (File.Exists($"{Way}import.png"))
                Customisation.Icon(OpenFileButton, $"{Way}import.png");
            else
            {
                MessageBox.Show($"{Way}import.png IS MISSING");
                Environment.Exit(0);
            }
            // library +
            if (File.Exists($"{Way}import.png"))
                Customisation.Icon(SaveFileButton, $"{Way}export.png");
            else
            {
                MessageBox.Show($"{Way}export.png IS MISSING");
                Environment.Exit(0);
            }
            
        }
        private string Way = System.IO.Directory.GetCurrentDirectory() + "\\icons\\";
        private string CurrentFile;
        private void RibbonButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV UTF-8|*.csv";
            if (!ofd.ShowDialog() == true)
                return;
            TextRange doc = new TextRange(CsvBox.Document.ContentStart, CsvBox.Document.ContentEnd);
            using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
            {
                doc.Load(fs, DataFormats.Text);
            }
            CurrentFile = ofd.FileName;
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            TextRange tr = new TextRange(CsvBox.Document.ContentStart, CsvBox.Document.ContentEnd);
            FileStream fs = File.Create(CurrentFile);
            tr.Save(fs, DataFormats.Text);
        }

        private void SaveFileButton_RightClick(object sender, MouseButtonEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "t-SQL фацл запроса|*.sql";
            if (sfd.ShowDialog() == true)
            {
                TextRange doc = new TextRange(CsvBox.Document.ContentStart, CsvBox.Document.ContentEnd);
                using (FileStream fs = File.Create(sfd.FileName))
                {
                    doc.Save(fs, DataFormats.Text);
                }
            }
        }

        private void ChangeDel(char d)
        {
            try
            {
                TextRange doc = new TextRange(CsvBox.Document.ContentStart, CsvBox.Document.ContentEnd);
                // найти разделитель автоматически
                char[] det = doc.Text.ToCharArray();
                for (int i = 0; i < det.Length; i++)
                    if (det[i] == CsvDelimiterDetector.DetectSeparator(CurrentFile))
                        det[i] = d;
                CsvBox.Document.Blocks.Clear();
                CsvBox.AppendText(doc.Text);
                MessageBox.Show(CsvDelimiterDetector.DetectSeparator(CurrentFile).ToString());
                
            }
            catch (Exception ex) { MessageBox.Show($"{ex}"); }
        }
        private void SemiColonSelected(object sender, MouseButtonEventArgs e) => ChangeDel(';');
        private void CommaItemSelected(object sender, MouseButtonEventArgs e)=> ChangeDel(',');
        private void SpaceItemSelected(object sender, MouseButtonEventArgs e) => ChangeDel(' ');
        private void BackSlashItemSelected(object sender, MouseButtonEventArgs e) => ChangeDel('\\');
        private void SlashItemSelected(object sender, MouseButtonEventArgs e)=> ChangeDel('/');
    }


    static class CsvDelimiterDetector
    {
        private static readonly char[] SeparatorChars = { ';', '/', ' ', ',',  '\\'};

        public static char DetectSeparator(string csvFilePath)
        {
            string[] lines = File.ReadAllLines(csvFilePath);
            return DetectSeparator(lines);
        }

        public static char DetectSeparator(string[] lines)
        {
            var q = SeparatorChars.Select(sep => new
            { 
                Separator = sep, Found = lines.GroupBy(line => line.Count(ch => ch == sep)) 
            })
                .OrderByDescending(res => res.Found.Count(grp => grp.Key > 0))
                .ThenBy(res => res.Found.Count())
                .First();

            return q.Separator;
        }
    }
}
