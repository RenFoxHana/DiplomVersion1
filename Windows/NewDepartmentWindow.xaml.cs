using DiplomVersion1.Model;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DiplomVersion1.Windows
{
    /// <summary>
    /// Логика взаимодействия для NewDepartmentWindow.xaml
    /// </summary>
    public partial class NewDepartmentWindow : Window
    {
        private int? SelectedInstituteId { get; set; }
        public Department department { get; set; }
        public NewDepartmentWindow(Department _department)
        {
            InitializeComponent();
            LoadInstitutes();

            department = _department;
            DataContext = department;

            if (department.IdInstitute != null)
            {
                SelectedInstituteId = department.IdInstitute.Value;
                cbInstitute.SelectedValue = department.IdInstitute.Value;
            }
        }

        private void LoadInstitutes()
        {
            using (var context = new BochagovaDiplomContext())
            {
                var institutes = context.Institutes.ToList();
                cbInstitute.ItemsSource = institutes;
            }
        }

        private void CbInstitute_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedInstitute = cbInstitute.SelectedItem as Institute;
            if (selectedInstitute != null)
            {
                SelectedInstituteId = selectedInstitute.IdInstitute;
            }
        }

        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            string departmentName = tbNameDepartment.Text;            

            department.NameDep = departmentName;
            department.IdInstitute = SelectedInstituteId;

            using (var context = new BochagovaDiplomContext())
            {
                if (department.IdDepartment == 0)
                {
                    context.Departments.Add(department);
                }
                else
                {
                    context.Departments.Update(department);
                }
                context.SaveChanges();
            }

            MessageBox.Show("Подразделение успешно сохранено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            Close();
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Text))
            {
                MessageBox.Show("Введите название подразделения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (!Regex.IsMatch(e.Text, @"^[а-яА-Я]$"))
            {
                e.Handled = true;
                MessageBox.Show("Вводите в поле названия подразделения только буквы.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
