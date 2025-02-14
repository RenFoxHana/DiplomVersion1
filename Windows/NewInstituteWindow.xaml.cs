using DiplomVersion1.Model;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DiplomVersion1.Windows
{
    /// <summary>
    /// Логика взаимодействия для NewInstituteWindow.xaml
    /// </summary>
    public partial class NewInstituteWindow : Window
    {
        public Institute institute { get; set; }
        public NewInstituteWindow(Institute _institute)
        {
            InitializeComponent();
            institute = _institute;
            DataContext = institute;

            if (institute != null)
            {
                label.Content = "РЕДАКТИРОВАНИЕ ИНСТИТУТА";
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
                MessageBox.Show("Введите название института.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (!Regex.IsMatch(e.Text, @"^[а-яА-Я]$"))
            {
                e.Handled = true;
                MessageBox.Show("Вводите в поле названия института только буквы.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
