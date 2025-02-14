using DiplomVersion1.Model;
using System.Windows;

namespace DiplomVersion1.Windows
{
    /// <summary>
    /// Логика взаимодействия для ReturnKeyWindow.xaml
    /// </summary>
    public partial class ReturnKeyWindow : Window
    {
        private LogOfIssuingKey _selectedLogEntry;
        public DateTime CurrentDateTime { get; set; }
        public ReturnKeyWindow(LogOfIssuingKey selectedLogEntry)
        {
            InitializeComponent();

            _selectedLogEntry = selectedLogEntry;

            tbKey.Text = _selectedLogEntry.IdKeyNavigation?.AudienceNumber;
            tbEmployee.Text = _selectedLogEntry.IdEmployeeNavigation.FullName;

            CurrentDateTime = DateTime.Now;
            DataContext = this;
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new BochagovaDiplomContext())
            {
                var logEntry = db.LogOfIssuingKeys.Find(_selectedLogEntry.IdEntry);
                if (logEntry != null)
                {
                    logEntry.DateTimeOfDelivery = dpReturnDate.Value;
                    db.LogOfIssuingKeys.Update(logEntry);
                    db.SaveChanges();
                }
            }

            MessageBox.Show("Ключ успешно сдан.");
            Close();
        }
    }
}
