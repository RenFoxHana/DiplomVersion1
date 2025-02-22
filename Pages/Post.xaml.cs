﻿using DiplomVersion1.Model;
using DiplomVersion1.ViewModel;
using DiplomVersion1.Windows;
using System.Windows;
using System.Windows.Controls;

namespace DiplomVersion1.Pages
{
    /// <summary>
    /// Логика взаимодействия для Post.xaml
    /// </summary>
    public partial class Post : Page
    {
        private PostVM vmPost;
        MainWindow MainWindow { get; set; }
        public Post(MainWindow mainWindow)
        {
            InitializeComponent();
            vmPost = new PostVM();
            DataContext = vmPost;
            MainWindow = mainWindow;
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        private void ButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuWindow menuWindow = new MenuWindow(MainWindow, MainWindow.MainFrame);
            menuWindow.Show();
        }
    }
}
