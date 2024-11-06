using DiplomVersion1.Pages;
using System.Windows;

namespace DiplomVersion1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = new Journal();
        }
    }
}