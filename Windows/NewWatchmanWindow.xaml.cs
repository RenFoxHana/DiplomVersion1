using DiplomVersion1.Helper;
using DiplomVersion1.Model;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiplomVersion1.Windows
{
    /// <summary>
    /// Логика взаимодействия для NewWatchmanWindow.xaml
    /// </summary>
    public partial class NewWatchmanWindow : Window
    {
        public Watchman watchman {  get; set; }
        public bool IsEdit { get; set; }
        public NewWatchmanWindow(Watchman _watchman)
        {
            InitializeComponent();

            watchman = _watchman;
            DataContext = watchman;
            if (watchman.IdWatchman != 0)
            {
                label.Content = "РЕДАКТИРОВАНИЕ ВАХТЁРА";
                IsEdit = true;
                tbLogin.Visibility = Visibility.Collapsed;
                pbPassword.Visibility = Visibility.Collapsed;
                textBlockLogin.Visibility = Visibility.Collapsed;
                textBlockPassword.Visibility = Visibility.Collapsed;
                MainGrid.RowDefinitions.RemoveAt(5);
                MainGrid.RowDefinitions.RemoveAt(4);
                Grid.SetRow(SaveButton, 4);
                Grid.SetRow(CancelButton, 4);
                Height = 300;
            }
        }

        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            string lastName = tbLastName.Text;
            string firstName = tbFirstName.Text;
            string patronymic = tbPatronymic.Text;

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

            if (!IsEdit)
            {
                string login = tbLogin.Text;
                string password = pbPassword.Password;

                if (string.IsNullOrWhiteSpace(login))
                {
                    MessageBox.Show("Введите логин вахтёра.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Введите пароль вахтёра.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (password.Length < 5 || password.Length > 20)
                {
                    MessageBox.Show("Пароль должен содержать от 5 до 20 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var regex = new Regex(@"^(?=.*[0-9])(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?/~\\-]).{5,20}$");
                if (!regex.IsMatch(password))
                {
                    MessageBox.Show("Пароль должен включать хотя бы одну цифру и один специальный символ, например знаки препинания.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string hashedPassword = PasswordHasher.HashPassword(password);

                watchman.WmLogin = login;
                watchman.WmPassword = hashedPassword;
            }

            watchman.FirstNameWm = firstName;
            watchman.LastNameWm = lastName;
            watchman.PatronymicWm = patronymic;

            using (var context = new BochagovaDiplomContext())
            {
                if (watchman.IdWatchman == 0)
                {
                    context.Watchmen.Add(watchman);
                    MessageBox.Show("Вахтёр успешно сохранен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    context.Watchmen.Update(watchman);
                    MessageBox.Show("Вахтёр успешно обновлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                context.SaveChanges();
            }
            Close();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[а-яА-Я]$"))
            {
                e.Handled = true;
                MessageBox.Show("Вводите в поле только русские буквы.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
