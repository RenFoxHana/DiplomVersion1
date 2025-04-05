using DiplomVersion1.Model;
using System.Windows;
using System.Windows.Controls;

namespace DiplomVersion1.Windows
{
    /// <summary>
    /// Логика взаимодействия для FiltersWindow.xaml
    /// </summary>
    public partial class FiltersWindow : Window
    {
        public DateTime CurrentDateTime { get; set; }
        private List<Employee> allEmployees;
        private List<Key> allKeys;

        public event Action<DateTime?, DateTime?, DateTime?, DateTime?, Employee, Key> FiltersApplied;

        public FiltersWindow()
        {
            InitializeComponent();
            CurrentDateTime = DateTime.Now;
            LoadKeysAndEmployees();
        }

        private void LoadKeysAndEmployees()
        {
            using (var db = new BochagovaDiplomContext())
            {
                allKeys = db.Keys.ToList();
                allEmployees = db.Employees.ToList();

                cbKey.ItemsSource = allKeys;
                cbEmployee.ItemsSource = allEmployees;
            }
        }

        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                string searchText = comboBox.Text.ToLower();

                if (comboBox == cbKey)
                {
                    var filteredItems = allKeys
                        .Where(item => item.AudienceNumber.ToLower().Contains(searchText))
                        .ToList();
                    comboBox.ItemsSource = filteredItems;
                }
                else if (comboBox == cbEmployee)
                {
                    var filteredItems = allEmployees
                        .Where(item => item.FullName.ToLower().Contains(searchText))
                        .ToList();
                    comboBox.ItemsSource = filteredItems;
                }
                comboBox.IsDropDownOpen = true;
            }
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            DateTime? startDateIssue = dpStartDateIssue.Value;
            DateTime? endDateIssue = dpEndDateIssue.Value;
            DateTime? startDateReturn = dpStartDateReturn.Value;
            DateTime? endDateReturn = dpEndDateReturn.Value;
            Employee? selectedEmployee = cbEmployee.SelectedItem as Employee;
            Key? selectedKey = cbKey.SelectedItem as Key;

            FiltersApplied?.Invoke(startDateIssue, endDateIssue, startDateReturn, endDateReturn, selectedEmployee, selectedKey);

            Close();
        }
    }
}