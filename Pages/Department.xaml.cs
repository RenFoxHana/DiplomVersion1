using DiplomVersion1.Model;
using DiplomVersion1.Windows;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using DiplomVersion1.Helper;

namespace DiplomVersion1.Pages
{
    /// <summary>
    /// Логика взаимодействия для Department.xaml
    /// </summary>
    public partial class Department : Page
    {
        BochagovaDiplomContext db = new BochagovaDiplomContext();
        MainWindow MainWindow { get; set; }
        public Department(MainWindow mainWindow)
        {
            InitializeComponent();
            LoadDepartments();
            MainWindow = mainWindow;
            UIHelper.ConfigureUIForWatchman(
                FindName("AddButton") as Button,
                FindName("EditButton") as Button
            );
        }

        private void LoadDepartments()
        {
            using (var context = new BochagovaDiplomContext())
            {
                var departments = context.Departments
                    .Include(d => d.IdInstituteNavigation) 
                    .ToList();

                listDepartment.ItemsSource = departments;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            NewDepartmentWindow DepartmentWindow = new NewDepartmentWindow(new Model.Department());
            DepartmentWindow.ShowDialog();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedDepartment = listDepartment.SelectedItem as Model.Department;

            if (selectedDepartment == null)
            {
                MessageBox.Show("Выберите подразделение для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NewDepartmentWindow DepartmentWindow = new NewDepartmentWindow(selectedDepartment);
            DepartmentWindow.ShowDialog();
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
