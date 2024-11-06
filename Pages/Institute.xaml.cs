using DiplomVersion1.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace DiplomVersion1.Pages
{
    /// <summary>
    /// Логика взаимодействия для Institute.xaml
    /// </summary>
    public partial class Institute : Page
    {
        private InstituteVM vmInstitute;
        public Institute()
        {
            InitializeComponent();
            vmInstitute = new InstituteVM();
            DataContext = vmInstitute;
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        private void ButtonJou_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Journal());
        }

        private void ButtonPost_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Post());
        }
    }
}
