using DiplomVersion1.Helper;
using DiplomVersion1.Model;
using DiplomVersion1.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
                //EditPost.CanExecute(true);
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

        //private RelayCommand editPost;
        //public RelayCommand EditPost
        //{
        //    get
        //    {
        //        return editPost ??
        //        (editPost = new RelayCommand(obj =>
        //        {
        //            NewInstituteWindow wnInstitute = new NewInstituteWindow
        //            {
        //                Title = "Редактирование института",
        //            };
        //            Post institute = selectedPost;
        //            Post tempinstitute = new Post();
        //            tempinstitute = institute.ShallowCopy();
        //            wnInstitute.DataContext = tempinstitute;
        //            if (wnInstitute.ShowDialog() == true)
        //            {
        //                institute.NameIns = tempinstitute.NameIns;
        //                db.SaveChanges();
        //                OnPropertyChanged("SelectedInstitute");
        //                ICollectionView view = CollectionViewSource.GetDefaultView(ListPost);
        //                view.Refresh();
        //            }
        //        }, (obj) => SelectedPost != null && ListPost.Count > 0));
        //    }
        //}

        //private RelayCommand deleteInstitute;
        //public RelayCommand DeleteInstitute
        //{
        //    get
        //    {
        //        return deleteInstitute ??
        //        (deleteInstitute = new RelayCommand(obj =>
        //        {
        //            Post institute = SelectedPost;
        //            MessageBoxResult result = MessageBox.Show("Удалить данные по институту: "
        //                + institute.NameIns, "Предупреждение", MessageBoxButton.OKCancel,
        //            MessageBoxImage.Warning);
        //            if (result == MessageBoxResult.OK)
        //            {
        //                ListPost.Remove(institute);
        //                db.Institutes.Remove(institute);
        //                db.SaveChanges();
        //            }
        //        }, (obj) => SelectedPost != null && ListPost.Count > 0));
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
