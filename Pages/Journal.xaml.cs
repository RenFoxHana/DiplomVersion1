using System.Windows;
using System.Windows.Controls;

namespace DiplomVersion1.Pages
{
    /// <summary>
    /// Логика взаимодействия для Journal.xaml
    /// </summary>
    public partial class Journal : Page
    {
        public Journal()
        {
            InitializeComponent();
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        private void ButtonIns_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Institute());
        }

        private void ButtonPost_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Post());
        }
    }
}
