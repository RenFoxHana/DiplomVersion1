using DiplomVersion1.Helper;
using DiplomVersion1.Model;
using DiplomVersion1.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace DiplomVersion1.Pages
{
    /// <summary>
    /// Логика взаимодействия для Institute.xaml
    /// </summary>
    public partial class Institute : Page
    {
        BochagovaDiplomContext db = new BochagovaDiplomContext();
        MainWindow MainWindow { get; set; }
        public Institute(MainWindow mainWindow)
        {
            InitializeComponent();
            LoadInstitutes();
            MainWindow = mainWindow;
            UIHelper.ConfigureUIForWatchman(
                FindName("AddButton") as Button,
                FindName("EditButton") as Button
            );
        }

        private void LoadInstitutes()
        {
            using (var context = new BochagovaDiplomContext())
            {
                var institutes = context.Institutes
                    .Include(i => i.Departments)
                    .OrderBy(d => d.NameIns)
                    .ToList();

                listInstitute.ItemsSource = institutes;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            NewInstituteWindow InstituteWindow = new NewInstituteWindow(new Model.Institute());
            if (InstituteWindow.ShowDialog() == true)
            {
                LoadInstitutes();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Model.Institute? institute = listInstitute.SelectedItem as Model.Institute;

            if (institute is null)
            {
                MessageBox.Show("Выберите институт для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NewInstituteWindow InstituteWindow = new NewInstituteWindow(institute);
            if (InstituteWindow.ShowDialog() == true)
            {
                LoadInstitutes();
            }
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
