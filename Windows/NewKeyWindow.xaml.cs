using DiplomVersion1.Model;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DiplomVersion1.Windows
{
    /// <summary>
    /// Логика взаимодействия для NewKeyWindow.xaml
    /// </summary>
    public partial class NewKeyWindow : Window
    {
        private int? SelectedDepartmentId { get; set; }
        private int? SelectedInstituteId { get; set; }
        public Model.Key key { get; set; }
        public NewKeyWindow(Model.Key _key)
        {
            InitializeComponent();
            LoadInstitutesDepartments();

            key = _key;
            DataContext = key;

            if (key != null)
            {
                label.Content = "РЕДАКТИРОВАНИЕ КЛЮЧА";
            }

            if (key.IdInstitute != null)
            {
                SelectedInstituteId = key.IdInstitute.Value;
                cbInstitute.SelectedValue = key.IdInstitute.Value;
            }

            if (key.IdDepartment != null)
            {
                SelectedDepartmentId = key.IdDepartment.Value;
                cbDepartment.SelectedValue = key.IdDepartment.Value;
            }
        }

        private void LoadInstitutesDepartments()
        {
            using (var context = new BochagovaDiplomContext())
            {
                var institutes = context.Institutes.ToList();
                cbInstitute.ItemsSource = institutes;

                // Загрузка подразделений, которые не принадлежат институтам
                var departmentsWithoutInstitute = context.Departments
                    .Where(d => d.IdInstitute == null)
                    .ToList();

                cbDepartment.ItemsSource = departmentsWithoutInstitute;
                cbDepartment.SelectedIndex = -1;
            }
        }

        private void CbDepartment_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedDepartment = cbDepartment.SelectedItem as Department;
            if (selectedDepartment != null)
            {
                SelectedDepartmentId = selectedDepartment.IdDepartment;
            }
            else
            {
                SelectedDepartmentId = null;
            }
        }

        private void CbInstitute_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            using (var context = new BochagovaDiplomContext())
            {
                var selectedInstitute = cbInstitute.SelectedItem as Institute;

                if (selectedInstitute != null)
                {                    
                    SelectedInstituteId = selectedInstitute.IdInstitute;

                    var departments = context.Departments
                        .Where(d => d.IdInstitute == selectedInstitute.IdInstitute)
                        .ToList();

                    cbDepartment.ItemsSource = departments;
                }
                else
                {                    
                    SelectedInstituteId = null;

                    var departmentsWithoutInstitute = context.Departments
                        .Where(d => d.IdInstitute == null)
                        .ToList();

                    cbDepartment.ItemsSource = departmentsWithoutInstitute;
                }

                cbDepartment.SelectedIndex = -1;
            }
        }

        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            string keyNumber = tbAudienceNumber.Text;

            if (string.IsNullOrWhiteSpace(keyNumber))
            {
                MessageBox.Show("Введите номер аудитории.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SelectedInstituteId.HasValue && !SelectedDepartmentId.HasValue)
            {
                MessageBox.Show("Выберите кафедру института.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            key.AudienceNumber = keyNumber;
            key.IdInstitute = SelectedInstituteId;
            key.IdDepartment = SelectedDepartmentId;

            using (var context = new BochagovaDiplomContext())
            {
                if (key.IdKey == 0)
                {
                    context.Keys.Add(key);
                }
                else
                {
                    context.Keys.Update(key);
                }
                context.SaveChanges();
            }

            MessageBox.Show("Ключ успешно сохранен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            Close();
        }
    }
}
