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
                var journalEntries = db.LogOfIssuingKeys
                    .Include(log => log.IdKeyNavigation)                    
                    .Include(log => log.IdEmployeeNavigation)
                    .ThenInclude(emp => emp.IdDepartmentNavigation)
                    .Include(log => log.IdWatchmanNavigation)
                    .ToList();

                listJournal.ItemsSource = journalEntries;
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
    }
}
