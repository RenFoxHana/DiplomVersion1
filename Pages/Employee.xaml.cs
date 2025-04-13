using DiplomVersion1.Helper;
using DiplomVersion1.Model;
using DiplomVersion1.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace DiplomVersion1.Pages
{
    /// <summary>
    /// Логика взаимодействия для Employee.xaml
    /// </summary>
    public partial class Employee : Page
    {
        BochagovaDiplomContext db = new BochagovaDiplomContext();
        MainWindow MainWindow { get; set; }

        public Employee(MainWindow mainWindow)
        {
            InitializeComponent();
            LoadEmployees();
            MainWindow = mainWindow;
            UIHelper.ConfigureUIForWatchman(
                FindName("AddButton") as Button,
                FindName("EditButton") as Button
            );
        }

        private void LoadEmployees()
        {
            using (var context = new BochagovaDiplomContext())
            {
                var employees = context.Employees
                    .Include(d => d.IdPostNavigation)
                    .Include(d => d.IdDepartmentNavigation)
                    .ToList();

                listEmployees.ItemsSource = employees;
            }
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            NewEmployeeWindow EmployeeWindow = new NewEmployeeWindow(new Model.Employee());
            if (EmployeeWindow.ShowDialog() == true)
                LoadEmployees();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmployee = listEmployees.SelectedItem as Model.Employee;

            if (selectedEmployee == null)
            {
                MessageBox.Show("Выберите сотрудника для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NewEmployeeWindow EmployeeWindow = new NewEmployeeWindow(selectedEmployee);
            EmployeeWindow.ShowDialog();
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
