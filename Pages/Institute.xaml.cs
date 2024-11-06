using DiplomVersion1.ViewModel;
using DiplomVersion1.Windows;
using System.Windows;
using System.Windows.Controls;

namespace DiplomVersion1.Pages
{
    /// <summary>
    /// Логика взаимодействия для Institute.xaml
    /// </summary>
    public partial class Institute : Page
    {
        MainWindow MainWindow { get; set; }
        private InstituteVM vmInstitute;
        public Institute(MainWindow mainWindow)
        {
            InitializeComponent();
            vmInstitute = new InstituteVM();
            DataContext = vmInstitute;
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
