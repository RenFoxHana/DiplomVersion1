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
            db.Institutes.Load();
            DataContext = db.Institutes
                .OrderBy(d => d.NameIns)
                .ToList();
            MainWindow = mainWindow;
            UIHelper.ConfigureUIForWatchman(
                FindName("AddButton") as Button,
                FindName("EditButton") as Button
            );
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            NewInstituteWindow InstituteWindow = new NewInstituteWindow(new Model.Institute());
            if (InstituteWindow.ShowDialog() == true)
            {
                Model.Institute institute = InstituteWindow.institute;
                db.Institutes.Add(institute);
                db.SaveChanges();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Model.Institute? institute = listInstitute.SelectedItem as Model.Institute;

            if (institute is null) return;

            NewInstituteWindow InstituteWindow = new NewInstituteWindow(new Model.Institute
            {
                NameIns = institute.NameIns,
            });

            if (InstituteWindow.ShowDialog() == true)
            {
                if (institute != null)
                {
                    institute.NameIns = InstituteWindow.institute.NameIns;
                    db.SaveChanges();
                    listInstitute.Items.Refresh();
                }
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
