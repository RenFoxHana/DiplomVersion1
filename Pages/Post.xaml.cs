using System.Windows;
using System.Windows.Controls;

namespace DiplomVersion1.Pages
{
    /// <summary>
    /// Логика взаимодействия для Post.xaml
    /// </summary>
    public partial class Post : Page
    {
        public Post()
        {
            InitializeComponent();
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        private void ButtonJou_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Journal());
        }

        private void ButtonIns_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Institute());
        }
    }
}
