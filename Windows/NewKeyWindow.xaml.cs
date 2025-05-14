using DiplomVersion1.Helper;
using DiplomVersion1.Model;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

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

        private List<Department> allDepartmentsWithoutInstitutes;
        private List<Institute> allInstitutes;

        // Списки для хранения всех данных
        private List<Department> allDepartments;
        private List<Institute> allInstitutesCopy;

        public NewKeyWindow(Model.Key _key)
        {
            InitializeComponent();
            LoadInstitutesDepartments();

            key = _key;
            DataContext = key;

            if (key.IdKey != 0)
            {
                label.Content = "РЕДАКТИРОВАНИЕ КЛЮЧА";
                if (!string.IsNullOrWhiteSpace(key.QrCodeBase64))
                {
                    var imageSource = BitmapToImageSource(Convert.FromBase64String(key.QrCodeBase64));
                    qrCodeImage.Source = imageSource;
                }
            }
            else
            {
                GenerateQrCodeForNewKey();
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

        private void GenerateQrCodeForNewKey()
        {
            if (key.IdKey == 0)
            {
                using (var context = new BochagovaDiplomContext())
                {
                    int nextKeyId = context.Keys.Any() ? context.Keys.Max(k => k.IdKey) + 1 : 1;

                    string qrCodeContent = nextKeyId.ToString();
                    string qrCodeBase64 = QrCodeGenerator.GenerateQrCode(qrCodeContent);

                    key.QrCodeBase64 = qrCodeBase64;

                    //Отображение QR-кода
                    if (qrCodeImage != null)
                    {
                        var imageSource = BitmapToImageSource(Convert.FromBase64String(qrCodeBase64));
                        qrCodeImage.Source = imageSource;
                    }
                }
            }
            else
            {
                string qrCodeContent = key.IdKey.ToString();
                string qrCodeBase64 = QrCodeGenerator.GenerateQrCode(qrCodeContent);

                key.QrCodeBase64 = qrCodeBase64;

                //Отображение QR-кода
                if (qrCodeImage != null)
                {
                    var imageSource = BitmapToImageSource(Convert.FromBase64String(qrCodeBase64));
                    qrCodeImage.Source = imageSource;
                }
            }
        }

        private void PrintQrCode_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(key.QrCodeBase64))
            {
                MessageBox.Show("QR-код не сгенерирован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string qrCodeContent;

            if (key.IdKey == 0)
            {
                using (var context = new BochagovaDiplomContext())
                {
                    int nextKeyId = context.Keys.Any() ? context.Keys.Max(k => k.IdKey) + 1 : 1;
                    qrCodeContent = nextKeyId.ToString();
                }
            }
            else
            {
                qrCodeContent = key.IdKey.ToString();
            }
            var qrCodeBitmap = QrCodeGenerator.GenerateQrCodeBitmap(qrCodeContent, 236);

            //Печать QR-кода
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                var visual = new DrawingVisual();
                using (var context = visual.RenderOpen())
                {

                    var imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        qrCodeBitmap.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                    //Физический размер 1×1 см
                    double dpi = 300;
                    double widthInInches = 1 / 2.54;
                    double heightInInches = 1 / 2.54;

                    double widthInPixels = widthInInches * dpi;
                    double heightInPixels = heightInInches * dpi;

                    context.DrawImage(imageSource, new Rect(0, 0, widthInPixels, heightInPixels));
                }

                //Печать
                printDialog.PrintVisual(visual, "QR Code");
            }
        }

        private ImageSource BitmapToImageSource(byte[] imageData)
        {
            using var stream = new MemoryStream(imageData);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }


        private void RegenerateQrCode_Click(object sender, RoutedEventArgs e)
        {
            GenerateQrCodeForNewKey();
            MessageBox.Show("QR-код успешно создан.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadInstitutesDepartments()
        {
            using (var context = new BochagovaDiplomContext())
            {
                allInstitutes = context.Institutes.ToList();
                allInstitutesCopy = allInstitutes.ToList();
                cbInstitute.ItemsSource = allInstitutes;

                allDepartmentsWithoutInstitutes = context.Departments
                    .Where(d => d.IdInstitute == null)
                    .ToList();

                allDepartments = allDepartmentsWithoutInstitutes.ToList();
                cbDepartment.ItemsSource = allDepartmentsWithoutInstitutes;
                cbDepartment.SelectedIndex = -1;
            }
        }

        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                string searchText = comboBox.Text.ToLower();

                if (comboBox == cbInstitute)
                {
                    var filteredInstitutes = allInstitutesCopy
                        .Where(institute => institute.NameIns.ToLower().Contains(searchText))
                        .ToList();
                    comboBox.ItemsSource = filteredInstitutes;
                }
                else if (comboBox == cbDepartment)
                {
                    var filteredDepartments = allDepartments
                        .Where(department => department.NameDep.ToLower().Contains(searchText))
                        .ToList();
                    comboBox.ItemsSource = filteredDepartments;
                }

                comboBox.IsDropDownOpen = true;
            }
        }

        private void CbDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void CbInstitute_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

                    allDepartments = departments.ToList();
                    cbDepartment.ItemsSource = departments;
                }
                else
                {
                    SelectedInstituteId = null;

                    var departmentsWithoutInstitute = context.Departments
                        .Where(d => d.IdInstitute == null)
                        .ToList();

                    allDepartments = departmentsWithoutInstitute.ToList();
                    cbDepartment.ItemsSource = departmentsWithoutInstitute;
                }

                cbDepartment.SelectedIndex = -1;
            }
        }

        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbAudienceNumber.Text))
            {
                MessageBox.Show("Введите номер аудитории.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SelectedInstituteId.HasValue && !SelectedDepartmentId.HasValue)
            {
                MessageBox.Show("Выберите кафедру института.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(key.QrCodeBase64))
            {
                MessageBox.Show("QR-код не сгенерирован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            key.AudienceNumber = tbAudienceNumber.Text;
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