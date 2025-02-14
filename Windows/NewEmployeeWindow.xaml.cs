using DiplomVersion1.Model;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DiplomVersion1.Windows
{
    /// <summary>
    /// Логика взаимодействия для NewEmployeeWindow.xaml
    /// </summary>
    public partial class NewEmployeeWindow : Window
    {
        private int? SelectedDepartmentId { get; set; }
        private int? SelectedPostId { get; set; }
        public Employee employee { get; set; }
        public NewEmployeeWindow(Employee _employee)
        {
            InitializeComponent();
            LoadDepartmentsAndPosts();

            employee = _employee;
            DataContext = employee;
            if (employee != null) {
                label.Content = "РЕДАКТИРОВАНИЕ СОТРУДНИКА";
            }

            if (employee.IdDepartment != null)
            {
                SelectedDepartmentId = employee.IdDepartment.Value;
                cbDepartment.SelectedValue = employee.IdDepartment.Value;
            }

            if (employee.IdPost != null)
            {
                SelectedPostId = employee.IdPost.Value;
                cbPost.SelectedValue = employee.IdPost.Value;
            }
        }

        private void LoadDepartmentsAndPosts()
        {
            using (var context = new BochagovaDiplomContext())
            {
                var departments = context.Departments.ToList();
                cbDepartment.ItemsSource = departments;
                var posts = context.Posts.ToList();
                cbPost.ItemsSource = posts;
            }
        }

        private void CbDepartment_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedDepartment = cbDepartment.SelectedItem as Department;
            if (selectedDepartment != null)
            {
                SelectedDepartmentId = selectedDepartment.IdDepartment;
            }
        }

        private void CbPost_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedPost = cbPost.SelectedItem as Post;
            if (selectedPost != null)
            {
                SelectedPostId = selectedPost.IdPost;
            }
        }

        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            string lastName = tbLastName.Text;
            string firstName = tbFirstName.Text;
            string patronymic = tbPatronymic.Text;

            if (string.IsNullOrWhiteSpace(lastName))
            {
                MessageBox.Show("Введите фамилию сотрудника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(firstName))
            {
                MessageBox.Show("Введите имя сотрудника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            employee.FirstName = firstName;
            employee.LastName = lastName;
            employee.Patronymic = patronymic;
            employee.IdPost = SelectedPostId;
            employee.IdDepartment = SelectedDepartmentId;

            using (var context = new BochagovaDiplomContext())
            {
                if (employee.IdEmployee == 0)
                {
                    context.Employees.Add(employee);
                }
                else
                {
                    context.Employees.Update(employee);
                }
                context.SaveChanges();
            }

            MessageBox.Show("Сотрудник успешно сохранен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            Close();
        }


        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[а-яА-Я]$"))
            {
                e.Handled = true;
                MessageBox.Show("Вводите в поле только русские буквы.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
