using DiplomVersion1.Helper;
using DiplomVersion1.Model;
using DiplomVersion1.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace DiplomVersion1.Pages
{
    /// <summary>
    /// Логика взаимодействия для Journal.xaml
    /// </summary>
    public partial class Journal : Page
    {
        MainWindow MainWindow { get; set; }

        // Хранение всех данных журнала
        private List<LogOfIssuingKey> allLogs;

        public Journal(MainWindow mainWindow)
        {
            InitializeComponent();
            LoadJournalData();
            MainWindow = mainWindow;
            UIHelper.ConfigureUIForAdmin(
                FindName("IssueButton") as Button,
                FindName("ReturnButton") as Button
            );
        }

        private void LoadJournalData()
        {
            using (var db = new BochagovaDiplomContext())
            {
                allLogs = db.LogOfIssuingKeys
                    .Include(log => log.IdKeyNavigation)
                    .Include(log => log.IdEmployeeNavigation)
                    .ThenInclude(emp => emp.IdDepartmentNavigation)
                    .Include(log => log.IdWatchmanNavigation)
                    .ToList();

                listJournal.ItemsSource = allLogs;
            }
        }

        private void IssueKey_Click(object sender, RoutedEventArgs e)
        {
            var issueWindow = new IssueKeyWindow();
            issueWindow.ShowDialog();
            LoadJournalData();
        }

        private void ReturnKey_Click(object sender, RoutedEventArgs e)
        {
            var selectedLogEntry = listJournal.SelectedItem as LogOfIssuingKey;

            if (selectedLogEntry == null)
            {
                MessageBox.Show("Выберите запись для сдачи ключа.");
                return;
            }

            if (selectedLogEntry.DateTimeOfDelivery.HasValue)
            {
                MessageBox.Show("Этот ключ уже сдан.");
                return;
            }

            var returnWindow = new ReturnKeyWindow(selectedLogEntry);
            returnWindow.ShowDialog();
            LoadJournalData();
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }

        private void ButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuWindow menuWindow = new MenuWindow(MainWindow, MainWindow.MainFrame);
            menuWindow.ShowDialog();
        }

        private void Filters_Click(object sender, RoutedEventArgs e)
        {
            var filtersWindow = new FiltersWindow();

            filtersWindow.FiltersApplied += ApplyFilters;

            filtersWindow.ShowDialog();
        }

        private void ApplyFilters(DateTime? startDateIssue, DateTime? endDateIssue, DateTime? startDateReturn, DateTime? endDateReturn, Model.Employee employee, Key key)
        {
            var filteredLogs = allLogs.AsQueryable();

            if (startDateIssue.HasValue && endDateIssue.HasValue)
            {
                filteredLogs = filteredLogs.Where(log =>
                    log.DateTimeOfIssue >= startDateIssue.Value &&
                    log.DateTimeOfIssue <= endDateIssue.Value);
            }
            else if (startDateIssue.HasValue)
            {
                filteredLogs = filteredLogs.Where(log => log.DateTimeOfIssue >= startDateIssue.Value);
            }
            else if (endDateIssue.HasValue)
            {
                filteredLogs = filteredLogs.Where(log => log.DateTimeOfIssue <= endDateIssue.Value);
            }

            if (startDateReturn.HasValue && endDateReturn.HasValue)
            {
                filteredLogs = filteredLogs.Where(log =>
                    log.DateTimeOfDelivery.HasValue &&
                    log.DateTimeOfDelivery.Value >= startDateReturn.Value &&
                    log.DateTimeOfDelivery.Value <= endDateReturn.Value);
            }
            else if (startDateReturn.HasValue)
            {
                filteredLogs = filteredLogs.Where(log =>
                    log.DateTimeOfDelivery.HasValue &&
                    log.DateTimeOfDelivery.Value >= startDateReturn.Value);
            }
            else if (endDateReturn.HasValue)
            {
                filteredLogs = filteredLogs.Where(log =>
                    log.DateTimeOfDelivery.HasValue &&
                    log.DateTimeOfDelivery.Value <= endDateReturn.Value);
            }

            if (employee != null)
            {
                filteredLogs = filteredLogs.Where(log => log.IdEmployee == employee.IdEmployee);
            }

            if (key != null)
            {
                filteredLogs = filteredLogs.Where(log => log.IdKey == key.IdKey);
            }

            listJournal.ItemsSource = filteredLogs.ToList();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            listJournal.ItemsSource = allLogs;
        }
    }
}