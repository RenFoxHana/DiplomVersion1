using System.Windows;
using System.Windows.Input;

namespace DiplomVersion1.Windows
{
    /// <summary>
    /// Логика взаимодействия для NewInstituteWindow.xaml
    /// </summary>
    public partial class NewInstituteWindow : Window
    {
        public NewInstituteWindow()
        {
            InitializeComponent();
        }

        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (char.IsDigit(e.Text, 0) && e.Text != ".")
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }
    }
}
