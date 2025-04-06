using DiplomVersion1.Model;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DiplomVersion1.Windows
{
    /// <summary>
    /// Логика взаимодействия для NewPostWindow.xaml
    /// </summary>
    public partial class NewPostWindow : Window
    {
        public Post post {  get; set; }
        public NewPostWindow(Post _post)
        {
            InitializeComponent();
            post = _post;
            DataContext = post;

            if (post.IdPost != 0)
            {
                label.Content = "РЕДАКТИРОВАНИЕ ДОЛЖНОСТИ";
            }
        }

        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Text))
            {
                MessageBox.Show("Введите название должности.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (!Regex.IsMatch(e.Text, @"^[а-яА-Я]$"))
            {
                e.Handled = true;
                MessageBox.Show("Вводите в поле названия должности только буквы.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
