using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using DiplomVersion1.Model;
using AForge.Video.DirectShow;
using ZXing;
using ZXing.Common;
using System.Windows.Controls;
using ZXing.Windows.Compatibility;
using System.Windows.Input;

namespace DiplomVersion1.Windows
{
    public partial class IssueKeyWindow : Window
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private bool isScanning = false;

        public DateTime CurrentDateTime { get; set; }

        private List<Model.Key> allAvailableKeys;
        private List<Employee> allEmployees;

        public IssueKeyWindow()
        {
            InitializeComponent();
            CurrentDateTime = DateTime.Now;
            LoadKeysAndEmployees();
            DataContext = this;

            //Инициализация списка камер
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                MessageBox.Show("Камера не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
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
                    MessageBox.Show("Данный ключ недоступен для выдачи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private byte[] ConvertInkCanvasToByteArray(InkCanvas inkCanvas)
        {
            if (inkCanvas.Strokes.Count == 0)
            {
                return null;
            }

            var renderTargetBitmap = new RenderTargetBitmap(
                (int)inkCanvas.ActualWidth,
                (int)inkCanvas.ActualHeight,
                96, 96, System.Windows.Media.PixelFormats.Pbgra32);

            renderTargetBitmap.Render(inkCanvas);

            var bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            using (var memoryStream = new System.IO.MemoryStream())
            {
                bitmapEncoder.Save(memoryStream);
                return memoryStream.ToArray();
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

                // Ключи, которые уже сданы
                var returnedKeyIds = latestLogEntries
                    .Where(log => log.DateTimeOfDelivery != null)
                    .Select(log => log.IdKey)
                    .ToList();

                // Ключи, которые выданы
                var issuedKeyIds = latestLogEntries
                    .Where(log => log.DateTimeOfDelivery == null)
                    .Select(log => log.IdKey)
                    .ToList();

                // Ключи, которые никогда не выдавались
                var neverIssuedKeyIds = db.Keys
                    .Where(key => !db.LogOfIssuingKeys.Any(log => log.IdKey == key.IdKey))
                    .Select(key => key.IdKey)
                    .ToList();

                // Доступные ключи
                var availableKeyIds = returnedKeyIds.Concat(neverIssuedKeyIds).Distinct().ToList();

                allAvailableKeys = db.Keys
                    .Where(key => availableKeyIds.Contains(key.IdKey))
                    .ToList();

                allEmployees = db.Employees.ToList();

                cbKey.ItemsSource = allAvailableKeys;
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

        private void IssueButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedKey = cbKey.SelectedItem as Model.Key;
            var selectedEmployee = cbEmployee.SelectedItem as Employee;

            if (selectedKey == null || selectedEmployee == null)
            {
                MessageBox.Show("Выберите ключ и сотрудника.");
                return;
            }

            byte[] signatureBytes = ConvertInkCanvasToByteArray(signatureCanvas);
            if (signatureBytes == null)
            {
                MessageBox.Show("Подпись отсутствует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new BochagovaDiplomContext())
            {
                var logEntry = new LogOfIssuingKey
                {
                    IdKey = selectedKey.IdKey,
                    IdEmployee = selectedEmployee.IdEmployee,
                    IdWatchman = App.CurrentUserId,
                    DateTimeOfIssue = (DateTime)dpIssueDate.Value,
                    Signature = signatureBytes
                };

                db.LogOfIssuingKeys.Add(logEntry);
                db.SaveChanges();
            }

            MessageBox.Show("Ключ успешно выдан.");
            this.Close();
        }
    }
}