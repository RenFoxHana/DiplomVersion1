﻿using DiplomVersion1.Helper;
using DiplomVersion1.Model;
using DiplomVersion1.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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
                    .OrderByDescending(log => log.DateTimeOfIssue)
                    .ToList();

                listJournal.ItemsSource = allLogs;
            }
        }

        private void ListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedEntry = listJournal.SelectedItem as LogOfIssuingKey;
            if (selectedEntry == null || selectedEntry.Signature == null)
            {
                MessageBox.Show("Подпись отсутствует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            BitmapImage signatureImage = ConvertByteArrayToBitmapImage(selectedEntry.Signature);
            if (signatureImage != null)
            {
                SignatureImage.Source = signatureImage;
                SignaturePopup.IsOpen = true;
            }
        }

        private BitmapImage ConvertByteArrayToBitmapImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                return null;
            }

            var bitmapImage = new BitmapImage();
            using (var memoryStream = new System.IO.MemoryStream(byteArray))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }

        private void Grid_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SignaturePopup.IsOpen)
            {
                SignaturePopup.IsOpen = false;
                e.Handled = true;
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
            var returnWindow = new ReturnKeyWindow();
            returnWindow.ShowDialog();
            LoadJournalData();
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            Window.GetWindow(this)?.Close();
            loginWindow.ShowDialog();            
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