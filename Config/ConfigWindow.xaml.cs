using System;
using System.Windows;

using System.Data.SqlClient;


namespace sqx
{
    /// <summary>
    /// Логика взаимодействия для ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();

            DbUserName.Text = Environment.UserName;
            DbServer.Text = Environment.MachineName;
        }
        private string _trustedConnection;
        private void ApplyConfig_Click(object sender, RoutedEventArgs e)
        {
            // Apply settings. add to Console.
            _trustedConnection = DbIntegrity.Text;

            if (DataModel.isSure == false)
            {
                if (MessageBox.Show("Вы уверены?", "Внимание.", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    QueryBuild();
            }
            else
                QueryBuild();

        }

        private void DbIntergatedSecurity_Checked(object sender, RoutedEventArgs e)
        {
            ConsoleWindow.Instance.StateBox.AppendText("Integrated Security включен \n");
        }

        private void QueryBuild()
        {
            string ConnectionString;
            
            if (!ConsoleWindow.Instance.IsInitialized)
                ConsoleWindow.Instance.Show();
            
            ConnectionString = $"SERVER={DbServer.Text};DATABASE={DbName.Text}";

            ConsoleWindow.Instance.StateBox.AppendText($"[{DateTime.Now}]{ConnectionString}\n");
            
            if (DbIntergatedSecurity.IsChecked == true)
            {
                if (_trustedConnection == string.Empty)
                    ConsoleWindow.Instance.StateBox.AppendText("*** Параметр (Проверка подлинности) не может быть пуст ***\n");
                else
                    ConnectionString += $";Integrated Security={_trustedConnection}";
            }
            else
                ConnectionString += $"UserName={DbUserName.Text};Password={DbPassword.Text}";

            ConsoleWindow.Instance.StateBox.AppendText(ConnectionString);

            QueryRun(ConnectionString);
        }

        /*
USE test
INSERT INTO users (ID, Title) VALUES
(1, 'aaa'),
(2, 'bbb')
         */
        private void QueryRun(string ConnectionString)
        {
            ConsoleWindow.Instance.StateBox.AppendText("*** Соединение ***\n");
            ConsoleWindow.Instance.StateBox.AppendText($"{ConnectionString}\n");
            
            SqlConnection connection = new SqlConnection(ConnectionString);
            
            try
            {
                connection.Open();
                SqlCommand command;

                if (DataModel.Query != string.Empty)
                {
                   command = new SqlCommand(DataModel.Query, connection);
                   command.ExecuteNonQuery();
                   ConsoleWindow.Instance.StateBox.AppendText($"[{DateTime.Now}]Выполнено\n");
                }
                else
                    ConsoleWindow.Instance.StateBox.AppendText("*** Параметр SqlCommand.CommandText не содержит команду ***\n");
            }
            catch (Exception ex)
            {
                ConsoleWindow.Instance.StateBox.AppendText('\n' + ex.Message);
            }
            finally 
            {
                connection.Close();
                ConsoleWindow.Instance.StateBox.AppendText("*** Соединение закрыто ***\n");
            }
            
        }
    }
}
