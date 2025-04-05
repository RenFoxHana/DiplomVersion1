using DiplomVersion1.Helper;
using DiplomVersion1.Model;
using System.Windows;
using System.Windows.Controls;

namespace DiplomVersion1
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            using (var db = new BochagovaDiplomContext())
            {
                // Проверка для вахтёра
                var watchman = db.Watchmen.FirstOrDefault(w => w.WmLogin == login);
                if (watchman != null && PasswordHasher.VerifyPassword(password, watchman.WmPassword))
                {
                    App.CurrentUserId = watchman.IdWatchman;
                    App.UserRole = "Watchman";

                    OpenMainWindow();
                    return;
                }

                // Проверка для администратора
                var admin = db.Admins.FirstOrDefault(a => a.AdminLogin == login);
                if (admin != null && PasswordHasher.VerifyPassword(password, admin.AdminPassword))
                {
                    App.CurrentUserId = admin.IdAdmin;
                    App.UserRole = "Admin";

                    OpenMainWindow();
                    return;
                }
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void OpenMainWindow()
        {
            MainWindow mainWindow = new MainWindow();
            Close();
            mainWindow.ShowDialog();            
        }
    }
}
