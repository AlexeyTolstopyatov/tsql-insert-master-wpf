using System.Windows;

namespace sqx
{
    /// <summary>
    /// Логика взаимодействия для ConsoleWindow.xaml
    /// </summary>
    public partial class ConsoleWindow : Window
    {
        public static ConsoleWindow Instance { get; set; }
        
        public ConsoleWindow()
        {
            InitializeComponent();
        }
    }
}
