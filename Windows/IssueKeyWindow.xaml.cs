using DiplomVersion1.Model;
using System.Windows;
using System.Windows.Controls;

namespace DiplomVersion1.Windows
{
    /// <summary>
    /// Логика взаимодействия для IssueKeyWindow.xaml
    /// </summary>
    public partial class IssueKeyWindow : Window
    {
        public DateTime CurrentDateTime { get; set; }

        private List<Key> allAvailableKeys;
        private List<Employee> allEmployees;

        public IssueKeyWindow()
        {
            InitializeComponent();
            CurrentDateTime = DateTime.Now;
            LoadKeysAndEmployees();
            DataContext = this;
        }

        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                string searchText = comboBox.Text.ToLower();

                if (comboBox == cbKey)
                {
                    var filteredItems = allAvailableKeys
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

        private void LoadKeysAndEmployees()
        {
            using (var db = new BochagovaDiplomContext())
            {

                var latestLogEntries = db.LogOfIssuingKeys
                    .GroupBy(log => log.IdKey)
                    .Select(group => group.OrderByDescending(log => log.DateTimeOfIssue).First())
                    .ToList();

                //Ключи, которые уже сданы
                var returnedKeyIds = latestLogEntries
                    .Where(log => log.DateTimeOfDelivery != null)
                    .Select(log => log.IdKey)
                    .ToList();

                //Ключи, которые выданы
                var issuedKeyIds = latestLogEntries
                    .Where(log => log.DateTimeOfDelivery == null)
                    .Select(log => log.IdKey)
                    .ToList();

                //Ключи, которые никогда не выдавались
                var neverIssuedKeyIds = db.Keys
                    .Where(key => !db.LogOfIssuingKeys.Any(log => log.IdKey == key.IdKey))
                    .Select(key => key.IdKey)
                    .ToList();

                //Доступные ключи
                var availableKeyIds = returnedKeyIds.Concat(neverIssuedKeyIds).Distinct().ToList();

                allAvailableKeys = db.Keys
                    .Where(key => availableKeyIds.Contains(key.IdKey))
                    .ToList();

                allEmployees = db.Employees.ToList();

                cbKey.ItemsSource = allAvailableKeys;
                cbEmployee.ItemsSource = allEmployees;
            }
        }

        private void IssueButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedKey = cbKey.SelectedItem as Model.Key;
            var selectedEmployee = cbEmployee.SelectedItem as Employee;

            if (selectedKey == null || selectedEmployee == null)
            {
                MessageBox.Show("Выберите ключ и сотрудника.");
                return;
            }

            if (selectedKey.IdDepartment.HasValue)
            {
                if (!selectedEmployee.IdDepartment.HasValue || selectedKey.IdDepartment != selectedEmployee.IdDepartment)
                {
                    using (var db = new BochagovaDiplomContext())
                    {
                        var departmentName = db.Departments
                            .FirstOrDefault(dep => dep.IdDepartment == selectedKey.IdDepartment.Value)?.NameDep;

                        MessageBox.Show($"Данный ключ принадлежит подразделению: {departmentName}. " +
                                        $"Сотрудник не может получить ключ.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            using (var db = new BochagovaDiplomContext())
            {
                var logEntry = new LogOfIssuingKey
                {
                    IdKey = selectedKey.IdKey,
                    IdEmployee = selectedEmployee.IdEmployee,
                    IdWatchman = App.CurrentUserId,
                    DateTimeOfIssue = (DateTime)dpIssueDate.Value
                };

                db.LogOfIssuingKeys.Add(logEntry);
                db.SaveChanges();
            }

            MessageBox.Show("Ключ успешно выдан.");
            this.Close();
        }
    }
}