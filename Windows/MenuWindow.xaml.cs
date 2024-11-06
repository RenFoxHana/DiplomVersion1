using DiplomVersion1.Pages;
using System.Windows;
using System.Windows.Controls;

namespace DiplomVersion1.Windows
{
    /// <summary>
    /// Логика взаимодействия для MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        MainWindow mainWindow;
        private Frame navigationFrame;

        public MenuWindow(MainWindow mainWin, Frame frame)
        {
            InitializeComponent();
            mainWindow = mainWin;
            navigationFrame = frame;
        }

        private void ButtonIns_Click(object sender, RoutedEventArgs e)
        {
            if (navigationFrame != null)
            {
                navigationFrame.Navigate(new Institute(mainWindow)); 
            }
        }

        private void ButtonPost_Click(object sender, RoutedEventArgs e)
        {
            if (navigationFrame != null)
            {
                navigationFrame.Navigate(new Post(mainWindow)); 
            }
        }

        private void ButtonDep_Click(object sender, RoutedEventArgs e)
        {
            if (navigationFrame != null)
            {
                navigationFrame.Navigate(new Department(mainWindow));
            }
        }
        private void ButtonEmp_Click(object sender, RoutedEventArgs e)
        {
            if (navigationFrame != null)
            {
                navigationFrame.Navigate(new Employee(mainWindow));
            }
        }
        private void ButtonKey_Click(object sender, RoutedEventArgs e)
        {
            if (navigationFrame != null)
            {
                navigationFrame.Navigate(new Keys(mainWindow));
            }
        }
        private void ButtonWM_Click(object sender, RoutedEventArgs e)
        {
            if (navigationFrame != null)
            {
                navigationFrame.Navigate(new Watchman(mainWindow));
            }
        }
        private void ButtonJournal_Click(object sender, RoutedEventArgs e)
        {
            if (navigationFrame != null)
            {
                navigationFrame.Navigate(new Journal(mainWindow));
            }
        }
    }
}
