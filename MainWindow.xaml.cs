using DiplomVersion1.Model;
using DiplomVersion1.Pages;
using System.Windows;

namespace DiplomVersion1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BochagovaDiplomContext db = new BochagovaDiplomContext();
        public MainWindow()
        {
            db.Database.EnsureCreated();
            InitializeComponent();
            MainFrame.Content = new Journal(this);
        }
    }
}