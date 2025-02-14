using DiplomVersion1.Helper;
using DiplomVersion1.Model;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DiplomVersion1.Windows
{
    /// <summary>
    /// Логика взаимодействия для NewAdmin.xaml
    /// </summary>
    public partial class NewAdmin : Window
    {

        public Admin admin { get; set; }
        public NewAdmin(Admin _admin)
        {
            InitializeComponent();

            admin = _admin;
            DataContext = admin;
        }

        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            string lastName = tbLastName.Text;
            string firstName = tbFirstName.Text;
            string patronymic = tbPatronymic.Text;
            string login = tbLogin.Text;
            string password = pbPassword.Password;

            if (string.IsNullOrWhiteSpace(lastName))
            {
                MessageBox.Show("Введите фамилию вахтёра.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(firstName))
            {
                MessageBox.Show("Введите имя вахтёра.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(login))
            {
                MessageBox.Show("Введите логин вахтёра.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин вахтёра.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string hashedPassword = PasswordHasher.HashPassword(password);

            admin.FirstNameAdmin = firstName;
            admin.LastNameAdmin = lastName;
            admin.PatronymicAdmin = patronymic;
            admin.AdminLogin = login;
            admin.AdminPassword = hashedPassword;

            using (var context = new BochagovaDiplomContext())
            {
                if (admin.IdAdmin == 0)
                {
                    context.Admins.Add(admin);
                }
                else
                {
                    context.Admins.Update(admin);
                }
                context.SaveChanges();
            }

            MessageBox.Show("Админ успешно сохранен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            Close();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[а-яА-Я]$"))
            {
                e.Handled = true;
                MessageBox.Show("Вводите в поле 'Фамилия сотрудника' только буквы.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
