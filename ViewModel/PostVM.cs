using DiplomVersion1.Helper;
using DiplomVersion1.Model;
using DiplomVersion1.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

namespace DiplomVersion1.ViewModel
{
    public class PostVM
    {

        BochagovaDiplomContext db = new BochagovaDiplomContext();

        private Post selectedPost;
        public Post SelectedPost
        {
            get { return this.selectedPost; }
            set
            {
                this.selectedPost = value;
                OnPropertyChanged("SelectedInstitute");
                EditPost.CanExecute(true);
            }
        }

        public ObservableCollection<Post> ListPost { get; set; } =
                new ObservableCollection<Post>();
        public PostVM()
        {
            ListPost = new ObservableCollection<Post>(db.Posts.ToList());
        }

        private RelayCommand addPost;
        public RelayCommand AddPost
        {
            get
            {
                return addPost ??
                 (addPost = new RelayCommand(obj =>
                 {
                     NewPostWindow wnPost = new NewPostWindow
                     {
                         Title = "Новая должность",
                     };
                     Post post = new Post();
                     wnPost.DataContext = post;
                     if (wnPost.ShowDialog() == true)
                     {
                         ListPost.Add(post);
                         db.Posts.Add(post);
                         db.SaveChanges();
                     }
                     SelectedPost = post;
                 }));
            }
        }

        private RelayCommand editPost;
        public RelayCommand EditPost
        {
            get
            {
                return editPost ??
                (editPost = new RelayCommand(obj =>
                {
                    NewPostWindow wnPost = new NewPostWindow
                    {
                        Title = "Редактирование должности",
                    };
                    Post post = selectedPost;
                    Post temppost = new Post();
                    temppost = post.ShallowCopy();
                    wnPost.DataContext = temppost;
                    if (wnPost.ShowDialog() == true)
                    {
                        post.NamePost = temppost.NamePost;
                        db.SaveChanges();
                        OnPropertyChanged("SelectedPost");
                        ICollectionView view = CollectionViewSource.GetDefaultView(ListPost);
                        view.Refresh();
                    }
                }, (obj) => SelectedPost != null && ListPost.Count > 0));
            }
        }

        private RelayCommand deletePost;
        public RelayCommand DeletePost
        {
            get
            {
                return deletePost ??
                (deletePost = new RelayCommand(obj =>
                {
                    Post post = SelectedPost;
                    MessageBoxResult result = MessageBox.Show("Удалить данные по должности: "
                        + post.NamePost, "Предупреждение", MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning);
                    if (result == MessageBoxResult.OK)
                    {
                        ListPost.Remove(post);
                        db.Posts.Remove(post);
                        db.SaveChanges();
                    }
                }, (obj) => SelectedPost != null && ListPost.Count > 0));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
