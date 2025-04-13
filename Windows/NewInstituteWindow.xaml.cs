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

            if (institute.IdInstitute != 0)
            {
                label.Content = "РЕДАКТИРОВАНИЕ ИНСТИТУТА";
            }
        }

        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            string instituteName = tbNameInstitute.Text;

            string normalizedInstituteName = instituteName.Trim();
            if (IsDuplicateInstitute(normalizedInstituteName, institute.IdInstitute))
            {
                MessageBox.Show("Институт с таким названием уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            institute.NameIns = normalizedInstituteName;

            using (var context = new BochagovaDiplomContext())
            {
                if (institute.IdInstitute == 0)
                {
                    context.Institutes.Add(institute);
                }
                else
                {
                    context.Institutes.Update(institute);
                }
                context.SaveChanges();
            }

            MessageBox.Show("Институт успешно сохранен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
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

        private bool IsDuplicateInstitute(string instituteName, int? excludeId = null)
        {
            using (var context = new BochagovaDiplomContext())
            {
                string normalizedInstituteName = instituteName.Trim().ToLower();

                return context.Institutes
                    .Any(i => i.NameIns.Trim().ToLower() == normalizedInstituteName && i.IdInstitute != excludeId);
            }
        }
    }
}
