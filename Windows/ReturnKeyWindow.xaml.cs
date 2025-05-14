using AForge.Video.DirectShow;
using DiplomVersion1.Model;
using System.Windows;
using ZXing.Common;
using ZXing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Controls;
using ZXing.Windows.Compatibility;
using Microsoft.EntityFrameworkCore;

namespace DiplomVersion1.Windows
{
    public partial class ReturnKeyWindow : Window
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private bool isScanning = false;

        private List<Key> allAvailableKeys;

        public DateTime CurrentDateTime { get; set; }

        public ReturnKeyWindow()
        {
            InitializeComponent();

            CurrentDateTime = DateTime.Now;
            DataContext = this;

            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                MessageBox.Show("Камера не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoadKeysAndEmployees();
        }

        private void StartScanning_Click(object sender, RoutedEventArgs e)
        {
            if (isScanning) return;

            isScanning = true;

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += VideoSource_NewFrame;
            videoSource.Start();
        }

        private void StopScanning_Click(object sender, RoutedEventArgs e)
        {
            if (!isScanning) return;

            isScanning = false;

            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.NewFrame -= VideoSource_NewFrame;
            }
        }

        private void VideoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            try
            {
                // Преобразуем кадр в изображение
                var frame = (Bitmap)eventArgs.Frame.Clone();

                if (frame == null)
                {
                    Console.WriteLine("Frame is null.");
                    return;
                }

                var bitmapSource = ConvertBitmapToBitmapSource(frame);

                Dispatcher.Invoke(() => cameraPreview.Source = bitmapSource);

                var barcodeReader = new BarcodeReader
                {
                    Options = new DecodingOptions
                    {
                        PossibleFormats = new[] { BarcodeFormat.QR_CODE },
                        TryHarder = true
                    },
                    AutoRotate = true
                };

                var result = barcodeReader.Decode(frame);
                if (result == null)
                {
                    Console.WriteLine("QR-код не распознан.");
                    return;
                }

                Dispatcher.Invoke(() =>
                {
                    HandleQrCode(result.Text);
                    StopScanning_Click(null, null);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при распознавании QR-кода: {ex.Message}");
            }
        }

        private BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Bmp);
                memoryStream.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        private void HandleQrCode(string qrCodeContent)
        {
            if (string.IsNullOrWhiteSpace(qrCodeContent))
            {
                MessageBox.Show("QR-код не распознан.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Ключ по ID из QR-кода
            using (var db = new BochagovaDiplomContext())
            {
                if (!int.TryParse(qrCodeContent, out int keyId))
                {
                    MessageBox.Show("Неверный формат QR-кода.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var selectedKey = db.Keys.FirstOrDefault(k => k.IdKey == keyId);
                if (selectedKey == null)
                {
                    MessageBox.Show("Ключ с указанным ID не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                cbKey.SelectedItem = allAvailableKeys.FirstOrDefault(k => k.IdKey == keyId);

                if (cbKey.SelectedItem == null)
                {
                    MessageBox.Show("Данный ключ недоступен для сдачи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //Последняя запись о выдаче ключа
                var latestLogEntry = db.LogOfIssuingKeys
                    .Where(log => log.IdKey == keyId && log.DateTimeOfDelivery == null)
                    .Include(log => log.IdEmployeeNavigation)
                    .OrderByDescending(log => log.DateTimeOfIssue)
                    .FirstOrDefault();

                if (latestLogEntry == null || latestLogEntry.IdEmployeeNavigation == null && cbKey.SelectedItem != null)
                {
                    MessageBox.Show("Информация о сотруднике не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                tbEmployee.Text = latestLogEntry.IdEmployeeNavigation.FullName;                

                dpReturnDate.Value = DateTime.Now;
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

                //Ключи, которые выданы
                var issuedKeyIds = latestLogEntries
                    .Where(log => log.DateTimeOfDelivery == null)
                    .Select(log => log.IdKey)
                    .ToList();

                //Доступные ключи
                var availableKeyIds = issuedKeyIds.Distinct().ToList();

                allAvailableKeys = db.Keys
                    .Where(key => availableKeyIds.Contains(key.IdKey))
                    .ToList();

                cbKey.ItemsSource = allAvailableKeys;
            }
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

                    //Если выбран ключ, заполняем текстовое поле сотрудника
                    var selectedKey = comboBox.SelectedItem as Key;
                    if (selectedKey != null)
                    {
                        using (var db = new BochagovaDiplomContext())
                        {
                            var latestLogEntry = db.LogOfIssuingKeys
                                .Where(log => log.IdKey == selectedKey.IdKey && log.DateTimeOfDelivery == null)
                                .OrderByDescending(log => log.DateTimeOfIssue)
                                .FirstOrDefault();

                            if (latestLogEntry != null && latestLogEntry.IdEmployeeNavigation != null)
                            {
                                tbEmployee.Text = latestLogEntry.IdEmployeeNavigation.FullName;
                            }
                        }
                    }
                }

                comboBox.IsDropDownOpen = true;
            }
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new BochagovaDiplomContext())
            {
                //Выбранный ключ
                var selectedKey = cbKey.SelectedItem as Key;
                if (selectedKey == null)
                {
                    MessageBox.Show("Выберите ключ для сдачи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //Последняя запись о выдаче ключа
                var latestLogEntry = db.LogOfIssuingKeys
                    .Where(log => log.IdKey == selectedKey.IdKey && log.DateTimeOfDelivery == null)
                    .OrderByDescending(log => log.DateTimeOfIssue)
                    .FirstOrDefault();

                if (latestLogEntry == null)
                {
                    MessageBox.Show("Запись о выдаче ключа не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Обновляем запись о сдаче ключа
                latestLogEntry.DateTimeOfDelivery = (DateTime)dpReturnDate.Value;
                db.LogOfIssuingKeys.Update(latestLogEntry);
                db.SaveChanges();
            }

            MessageBox.Show("Ключ успешно сдан.");
            Close();
        }
    }
}