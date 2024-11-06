using DiplomVersion1.Windows;
using System.Windows;
using System.Windows.Controls;

namespace DiplomVersion1.Pages
{
    /// <summary>
    /// Логика взаимодействия для Watchman.xaml
    /// </summary>
    public partial class Watchman : Page
    {
        MainWindow MainWindow { get; set; }

        public Watchman(MainWindow mainWindow)
        {
            InitializeComponent();
            MainWindow = mainWindow;
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        private void ButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuWindow menuWindow = new MenuWindow(MainWindow, MainWindow.MainFrame);
            menuWindow.Show();
        }
    }
}
