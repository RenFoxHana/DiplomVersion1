﻿using DiplomVersion1.Model;
using DiplomVersion1.Windows;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using DiplomVersion1.Helper;

namespace DiplomVersion1.Pages
{
    /// <summary>
    /// Логика взаимодействия для Post.xaml
    /// </summary>
    public partial class Post : Page
    {
        BochagovaDiplomContext db = new BochagovaDiplomContext();
        MainWindow MainWindow { get; set; }
        public Post(MainWindow mainWindow)
        {
            InitializeComponent();
            LoadPosts();
            MainWindow = mainWindow;
            UIHelper.ConfigureUIForWatchman(
                FindName("AddButton") as Button,
                FindName("EditButton") as Button
            );
        }

        private void LoadPosts()
        {
            using (var context = new BochagovaDiplomContext())
            {
                var posts = context.Posts
                    .OrderBy(d => d.NamePost)
                    .ToList();

                listPost.ItemsSource = posts;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            NewPostWindow PostWindow = new NewPostWindow(new Model.Post());
            if (PostWindow.ShowDialog() == true)
            {
                LoadPosts();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Model.Post? post = listPost.SelectedItem as Model.Post;

            if (post is null)
            {
                MessageBox.Show("Выберите должность для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NewPostWindow PostWindow = new NewPostWindow(post);

            if (PostWindow.ShowDialog() == true)
            {
                LoadPosts();
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
