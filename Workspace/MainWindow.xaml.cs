using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.IO.Csv;

using Microsoft.Win32;

using System.Diagnostics;

namespace sqx
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            if (Directory.Exists(Ico))
            {
                Customisation.Icon(OpenFile, $"{Ico}import.png");
                Customisation.Icon(SaveFile, $"{Ico}export.png");
                Customisation.Icon(CConfig, $"{Ico}sql.png");
                Customisation.Icon(FileSetup, $"{Ico}table.png");
            }
            else
            {
                MessageBox.Show($"{Ico} is missing");
                Environment.Exit(0);
            }
        }
        private string Ico = Directory.GetCurrentDirectory() + "\\icons\\";
        private void OpenData()
        {
            QueryBox.Document.Blocks.Clear();

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Неструктурированный файл UTF-8|*.csv";

            if (!openFile.ShowDialog() == true)
                return;

            DataModel.File = openFile.FileName;

            GetData();
        }

        // [C] номер ячейки
        // [D] данные ячейки
        // [i] счетчик ячеек

        
        
        private void ApplyTableName_Click(object sender, RoutedEventArgs e) => DataModel.Name = SetupTableBox.Text;

        private void GetData()
        {
            try
            {
                ConsoleWindow.Instance = new ConsoleWindow();
                ConsoleWindow.Instance.Show();
            }
            catch (Exception xc) 
            { 
                ConsoleWindow.Instance.StateBox.AppendText(xc.Message);
            }
            
            ConsoleWindow.Instance.StateBox.AppendText("*** Будьте уверены, что в CSV указан один тип разделителя *** \n");
            ConsoleWindow.Instance.StateBox.AppendText("При импорте перезаписанного файла с разными разделителями возможна потеря данных! \n");
            ConsoleWindow.Instance.StateBox.AppendText($"[{DateTime.Now}]Инициализация CsvReader для: {DataModel.File}\n");
            
            try
            {
                // load file
                using (CsvReader re = new CsvReader(DataModel.File)) 
                {
                    re.Delimiter = char.Parse(DataModel.Delimeter);

                    // logging
                    ConsoleWindow.Instance.StateBox.AppendText($"[{DateTime.Now}]Чтение данных из: {DataModel.File}\n");

                    // (floats.csv OutOfRangeException)
                    // (qd.csv OutOfRangeException)

                    // build table's model
                    DataModel.Table = re.ReadData().ToArray();

                    // done. -> set-up table's name
                    ConsoleWindow.Instance.StateBox.AppendText($"[{DateTime.Now}]Успешно.\n");
                    ConsoleWindow.Instance.StateBox.AppendText(@"*** Данные внесены *** \n");
                    ConsoleWindow.Instance.StateBox.AppendText(@"*** Укажите название таблицы перед сборкой запроса. ***\n");

                    // open TableSetup
                    {
                        TableSetupWindow ts = new TableSetupWindow();
                        ts.ShowDialog();
                    }

                    if (DataModel.IsEnumEnabled == true) // undef
                        Build();
                    else
                        BuildNoID();

                }
                // destroy CsvReader (idk why i need use Dispose)
                
            }
            catch (Exception xc)
            {
                ConsoleWindow.Instance.StateBox.AppendText($"*** Таблица повреждена ***\n");
                ConsoleWindow.Instance.StateBox.AppendText($"{xc.Message}\n");
            }
        }

        
        static int ei; // element
        static int ri; // row
        static int cells;

        private void BuildNoID() // сборка без ключа
        {
            // detect last file.tmp
            // 
            // if (DataModel.Table == null)
            //     ReadTemp();

            // delete table counters
            ri = 0;
            ei = 0;
            cells = 0;

            
            if (CommentCheck.IsChecked == true || DataModel.IsCommentEnabled == true)
                QueryBox.AppendText($"/*Scipt created {DateTime.Now}. Build with no ident*/");

            QueryBox.AppendText($"\nINSERT INTO [{DataModel.Name}] \n(");
            double pattern = 0;

            try
            {

                cells = DataModel.Table.First().Count();

                for (int i = 0; i < DataModel.Table.Count(); i++)
                {
                    if (i > 0) break;
                    for (int j = 0; j < DataModel.Table.First().Count(); j++)
                    {
                        string dt = DataModel.Table.ElementAt(i).ElementAt(j);
                        string[] adt = dt.Split(';');
                        foreach (string x in adt)
                        {
                            QueryBox.AppendText(x);
                            if (ei < adt.Length - 1)
                                QueryBox.AppendText(",");
                            ei++;
                        }
                        QueryBox.AppendText(")");
                    }
                }
                QueryBox.AppendText(" VALUES \n");
                for (int i = 1; i < DataModel.Table.Count(); i++)
                {
                    QueryBox.AppendText($"(");
                    for (int j = 0; j < cells; j++)
                    {
                        ei = 0;
                        string dt = (DataModel.Table.ElementAt(i).ElementAt(j));
                        string[] adt = dt.Split(';');
                        foreach (string x in adt)
                        {
                            if (double.TryParse(x, out pattern))
                                QueryBox.AppendText(x);
                            else
                                QueryBox.AppendText($"'{x}'");

                            if (ei < adt.Length - 1)
                                QueryBox.AppendText(", ");
                            ei++;

                        }
                        ri++;
                    }
                    QueryBox.AppendText(")");
                    if (ri < DataModel.Table.Count() - 1)
                        QueryBox.AppendText(", \n");
                }
                ConsoleWindow.Instance.StateBox.AppendText($"Записано успешно:\nЯчеек в строке {ei}.\nСтрок в таблице {ri}\n");

            }
            catch (Exception xc)
            {
                ConsoleWindow.Instance.StateBox.AppendText($"{xc.Message}\n");
                ri = 0;
                ei = 0;
                cells = 0;
            }

            // 1) Create file.tmp
            // 2) write data
            // 3) delete table

            // CreateTemp(file)
            // WriteAll(file)
            DataModel.Table = null;

        }
        private void Build()
        {
            ri = 0;
            ei = 0;
            cells = 0;
            if (CommentCheck.IsChecked == true || DataModel.IsCommentEnabled == true)
                QueryBox.AppendText($"/*Scipt created {DateTime.Now}. Build with no ident*/ \n");
            QueryBox.AppendText($"\nINSERT INTO [{DataModel.Name}] \n(ID, ");

            // TryParse(x, pattern)
            double pattern = 0;

            try
            {

                cells = DataModel.Table.First().Count();

                for (int i = 0; i < DataModel.Table.Count(); i++)
                {
                    if (i > 0) break;
                    for (int j = 0; j < DataModel.Table.First().Count(); j++)
                    {
                        string dt = DataModel.Table.ElementAt(i).ElementAt(j);
                        string[] adt = dt.Split(';');
                        foreach (string x in adt)
                        {
                            QueryBox.AppendText(x);
                            if (ei < adt.Length - 1)
                                QueryBox.AppendText(",");
                            ei++;
                        }
                        QueryBox.AppendText(")");
                    }
                }
                QueryBox.AppendText(" VALUES \n");
                for (int i = 1; i < DataModel.Table.Count(); i++)
                {
                    QueryBox.AppendText($"({ri+1}, "); // {i} doesnt work :D
                    for (int j = 0; j < cells; j++)
                    {
                        ei = 0;
                        string dt = (DataModel.Table.ElementAt(i).ElementAt(j));
                        string[] adt = dt.Split(';');
                        foreach (string x in adt)
                        {
                            if (double.TryParse(x, out pattern))
                                QueryBox.AppendText(x);
                            else
                                QueryBox.AppendText($"'{x}'");

                            if (ei < adt.Length - 1)
                                QueryBox.AppendText(", ");
                            ei++;

                        }
                        ri++;
                    }
                    QueryBox.AppendText(")");
                    if (ri < DataModel.Table.Count() - 1)
                        QueryBox.AppendText(", \n");
                }
                ConsoleWindow.Instance.StateBox.AppendText($"Записано успешно:\nЯчеек в строке {ei}.\nСтрок в таблице {ri}\n");

            }
            catch (Exception xc)
            {
                ConsoleWindow.Instance.StateBox.AppendText($"{xc}\n");
                ri = 0;
                ei = 0;
                cells = 0;
            }
            
            
            // CreateTemp(file);
            // WriteAll(file)
            DataModel.Table = null;
        }

        private void CConfig_Click(object sender, RoutedEventArgs e)
        {
            DataModel.isSure = false;
            TextRange doc = new TextRange(QueryBox.Document.ContentStart, QueryBox.Document.ContentEnd);
            DataModel.Query = doc.Text;
            ConfigWindow cf = new ConfigWindow();
            cf.ShowDialog();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenData();
        }

        private void QueryBuild_Click(object sender, RoutedEventArgs e)
        {
            ConsoleWindow.Instance.StateBox.AppendText($"[{DateTime.Now}]Сбор данных...\n");
            if (IdentCheck.IsChecked == true)
                Build();
            else
                BuildNoID();
        }

        private void FileSetup_Click(object sender, RoutedEventArgs e)
        {
            // way till csvrepair
            var csvrepair = $"{Directory.GetCurrentDirectory()}\\csvrepair.exe";
            if (File.Exists(csvrepair))
                Process.Start(csvrepair);
            else
                MessageBox.Show("Csv Repair не найден");
        }

        private void WhileClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void RightButtonClick(object sender, MouseButtonEventArgs e)
        {
            DataModel.isSure = true;
            TextRange doc = new TextRange(QueryBox.Document.ContentStart, QueryBox.Document.ContentEnd);
            DataModel.Query = doc.Text;
            ConfigWindow cf = new ConfigWindow();
            cf.ShowDialog();
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "t-SQL файл запроса|*.sql";

            if (sfd.ShowDialog() == true)
            {
                TextRange doc = new TextRange(QueryBox.Document.ContentStart, QueryBox.Document.ContentEnd);
                using (FileStream fs = File.Create(sfd.FileName))
                {
                    doc.Save(fs, DataFormats.Text);
                }
            }


            // if file.tmp exists
            //try
            //{
            //    File.Delete(_temp);
            //}
            //catch { /*message*/ }
        }

        private void About_Click(object sender , RoutedEventArgs e)
        {
            if (File.Exists("tablever.exe"))
            {
                Process.Start("tablever.exe");
            }
            else 
            {
                MessageBox.Show("не обнаружено");
            }
        }
    }
}
