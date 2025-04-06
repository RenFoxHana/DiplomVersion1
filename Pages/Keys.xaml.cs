using DiplomVersion1.Helper;
using DiplomVersion1.Model;
using DiplomVersion1.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace DiplomVersion1.Pages
{
    /// <summary>
    /// Логика взаимодействия для Keys.xaml
    /// </summary>
    public partial class Keys : Page
    {
        BochagovaDiplomContext db = new BochagovaDiplomContext();
        MainWindow MainWindow { get; set; }

        public Keys(MainWindow mainWindow)
        {
            InitializeComponent();
            LoadKeys();
            MainWindow = mainWindow;
            UIHelper.ConfigureUIForWatchman(
                FindName("AddButton") as Button,
                FindName("EditButton") as Button
            );
        }
        private void LoadKeys()
        {
            using (var context = new BochagovaDiplomContext())
            {
                var keys = context.Keys
                    .Include(d => d.IdInstituteNavigation)
                    .Include(d => d.IdDepartmentNavigation)
                    .OrderBy(d => d.AudienceNumber)
                    .ToList();

                listKeys.ItemsSource = keys;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            NewKeyWindow KeyWindow = new NewKeyWindow(new Model.Key());
            KeyWindow.ShowDialog();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedKey = listKeys.SelectedItem as Model.Key;

            if (selectedKey == null)
            {
                MessageBox.Show("Выберите ключ для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NewKeyWindow KeyWindow = new NewKeyWindow(selectedKey);
            KeyWindow.ShowDialog();
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
    }
}
