using DiplomVersion1.Model;
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
            db.Posts.Load();
            DataContext = db.Posts.Local.ToObservableCollection();
            MainWindow = mainWindow;
            UIHelper.ConfigureUIForWatchman(
                FindName("AddButton") as Button,
                FindName("EditButton") as Button
            );
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            NewPostWindow PostWindow = new NewPostWindow(new Model.Post());
            if (PostWindow.ShowDialog() == true)
            {
                Model.Post post = PostWindow.post;
                db.Posts.Add(post);
                db.SaveChanges();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Model.Post? post = listPost.SelectedItem as Model.Post;

            if (post is null) return;

            NewPostWindow PostWindow = new NewPostWindow(new Model.Post
            {
                NamePost = post.NamePost,
            });

            if (PostWindow.ShowDialog() == true)
            {
                if (post != null)
                {
                    post.NamePost = PostWindow.post.NamePost;
                    db.SaveChanges();
                    listPost.Items.Refresh();
                }
            }
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }

        private void ButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuWindow menuWindow = new MenuWindow(MainWindow, MainWindow.MainFrame);
            menuWindow.Show();
        }
    }
}
