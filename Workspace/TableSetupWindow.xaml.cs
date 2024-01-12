using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace sqx
{
    /// <summary>
    /// Логика взаимодействия для TableSetupWindow.xaml
    /// </summary>
    public partial class TableSetupWindow : Window
    {
        public TableSetupWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) // OK
        {
            if (TableNameBox.Text == string.Empty)
                DataModel.Name = "table_name";
            else
            {
                Regex.Replace(TableNameBox.Text, @"[^0-9a-zA-Z]+", "");
                DataModel.Name = TableNameBox.Text;
            }
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) // Cancel
        {
            TableNameBox.Text = "table_name";
        }

        private void IsEnumEnabled_Checked(object sender, RoutedEventArgs e)
        {
            if (IsEnumEnabled.IsChecked == true)
                DataModel.IsEnumEnabled = true;
            else
                DataModel.IsEnumEnabled = false;
        }

        private void IsCommentEnabled_Checked(object sender, RoutedEventArgs e)
        {
            if (IsCommentEnabled.IsChecked == true)
                DataModel.IsCommentEnabled = true;
            else
                DataModel.IsCommentEnabled = false;
        }
    }
}
