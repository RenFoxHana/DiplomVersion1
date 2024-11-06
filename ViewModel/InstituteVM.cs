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
    public class InstituteVM
    {
        BochagovaDiplomContext db = new BochagovaDiplomContext();

        private Institute selectedInstitute;
        public Institute SelectedInstitute
        {
            get { return this.selectedInstitute; }
            set
            {
                this.selectedInstitute = value;
                OnPropertyChanged("SelectedInstitute");
                EditInstitute.CanExecute(true);
            }
        }

        public ObservableCollection<Institute> ListInstitute { get; set; } =
                new ObservableCollection<Institute>();
        public InstituteVM()
        {
            ListInstitute = new ObservableCollection<Institute>(db.Institutes.ToList());
        }

        private RelayCommand addInstitute;
        public RelayCommand AddInstitute
        {
            get
            {
                return addInstitute ??
                 (addInstitute = new RelayCommand(obj =>
                 {
                     NewInstituteWindow wnInstitute = new NewInstituteWindow
                     {
                         Title = "Новый институт",
                     };
                     Institute institute = new Institute();
                     wnInstitute.DataContext = institute;
                     if (wnInstitute.ShowDialog() == true)
                     {
                         ListInstitute.Add(institute);
                         db.Institutes.Add(institute);
                         db.SaveChanges();
                     }
                     SelectedInstitute = institute;
                 }));
            }
        }

        private RelayCommand editInstitute;
        public RelayCommand EditInstitute
        {
            get
            {
                return editInstitute ??
                (editInstitute = new RelayCommand(obj =>
                {
                    NewInstituteWindow wnInstitute = new NewInstituteWindow
                    {
                        Title = "Редактирование института",
                    };
                    Institute institute = selectedInstitute;
                    Institute tempinstitute = new Institute();
                    tempinstitute = institute.ShallowCopy();
                    wnInstitute.DataContext = tempinstitute;
                    if (wnInstitute.ShowDialog() == true)
                    {
                        institute.NameIns = tempinstitute.NameIns;
                        db.SaveChanges();
                        OnPropertyChanged("SelectedInstitute");
                        ICollectionView view = CollectionViewSource.GetDefaultView(ListInstitute);
                        view.Refresh();
                    }
                }, (obj) => SelectedInstitute != null && ListInstitute.Count > 0));
            }
        }

        private RelayCommand deleteInstitute;
        public RelayCommand DeleteInstitute
        {
            get
            {
                return deleteInstitute ??
                (deleteInstitute = new RelayCommand(obj =>
                {
                    Institute institute = SelectedInstitute;
                    MessageBoxResult result = MessageBox.Show("Удалить данные по институту: "
                        + institute.NameIns, "Предупреждение", MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning);
                    if (result == MessageBoxResult.OK)
                    {
                        ListInstitute.Remove(institute);
                        db.Institutes.Remove(institute);
                        db.SaveChanges();
                    }
                }, (obj) => SelectedInstitute != null && ListInstitute.Count > 0));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
