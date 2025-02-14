using DiplomVersion1.Model;
using System.Windows;

namespace DiplomVersion1.Windows
{
    /// <summary>
    /// Логика взаимодействия для IssueKeyWindow.xaml
    /// </summary>
    public partial class IssueKeyWindow : Window
    {
        public DateTime CurrentDateTime { get; set; }
        public IssueKeyWindow()
        {
            InitializeComponent();
            CurrentDateTime = DateTime.Now;
            LoadKeysAndEmployees();
            DataContext = this;
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

                var availableKeys = db.Keys
                    .Where(key => availableKeyIds.Contains(key.IdKey))
                    .ToList();

                cbKey.ItemsSource = availableKeys;

                cbEmployee.ItemsSource = db.Employees.ToList();
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
