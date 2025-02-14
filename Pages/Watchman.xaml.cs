using DiplomVersion1.Helper;
using DiplomVersion1.Model;
using DiplomVersion1.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace DiplomVersion1.Pages
{
    /// <summary>
    /// Логика взаимодействия для Watchman.xaml
    /// </summary>
    public partial class Watchman : Page
    {
        MainWindow MainWindow { get; set; }
        BochagovaDiplomContext db = new BochagovaDiplomContext();
        public Watchman(MainWindow mainWindow)
        {
            InitializeComponent();
            db.Watchmen.Load();
            DataContext = db.Watchmen.Local.ToObservableCollection();
            MainWindow = mainWindow;
            UIHelper.ConfigureUIForWatchman(
                FindName("AddButton") as Button,
                FindName("EditButton") as Button
            );
        }        
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            NewWatchmanWindow WatchmanWindow = new NewWatchmanWindow(new Model.Watchman());
            WatchmanWindow.ShowDialog();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedWatchman = listWatchman.SelectedItem as Model.Watchman;

            if (selectedWatchman == null)
            {
                MessageBox.Show("Выберите вахтёра для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NewWatchmanWindow WatchmanWindow = new NewWatchmanWindow(selectedWatchman);
            WatchmanWindow.ShowDialog();
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
