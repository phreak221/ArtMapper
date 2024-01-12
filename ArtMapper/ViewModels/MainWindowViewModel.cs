using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ArtMapper.Models;

namespace ArtMapper.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<ViewModelBase> _workspaces;

        public ObservableCollection<ViewModelBase> Workspaces
        {
            get
            {
                if (_workspaces == null)
                {
                    _workspaces = new ObservableCollection<ViewModelBase>();
                    _workspaces.CollectionChanged += OnWorkspacesChanged;
                }
                return _workspaces;
            }
        }

        private void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }

        public ICommand MenuExit { get; set; }
        public ICommand MenuAddNew { get; set; }
        public ICommand MenuEdit { get; set; }
        public ICommand MenuScanDrive { get; set; }
        public ICommand MenuHome { get; set; }

        public MainWindowViewModel(string[] argfiles)
        {
            if (argfiles.Length > 0)
            {
                ShowAddNewArt(argfiles[0]);
            }
            else
            {
                ShowArtGrid();
            }

            MenuHome = new RelayCommand(ShowMainView);
            MenuAddNew = new RelayCommand(ShowAddNewArt);
            MenuEdit = new RelayCommand(EditArtMap);
            MenuScanDrive = new RelayCommand(ScanDriveForArt);
            MenuExit = new RelayCommand(ExitArtMapper);
        }

        private void ScanDriveForArt(object obj)
        {
            Workspaces.Clear();
            ScanDriveViewModel workspace = new ScanDriveViewModel(Workspaces);
            Workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        private void ShowMainView(object obj)
        {
            ShowArtGrid();
        }

        private void EditArtMap(object obj)
        {
            
        }

        private void ShowArtGrid()
        {
            Workspaces.Clear();
            ArtGridViewModel workspace = new ArtGridViewModel(Workspaces);
            Workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        private void ShowAddNewArt(string artfile)
        {
            ArtMapDb art = new ArtMapDb();
            art.ArtName = Path.GetFileNameWithoutExtension(artfile);
            art.ArtPath = artfile;
            BitmapImage img = new BitmapImage(new Uri(artfile));
            art.ArtDimentions = $"{img.Width} X {img.Height}";
            FileInfo fi = new FileInfo(artfile);
            art.ArtFileSize = $"{fi.Length}";
            art.ArtExists = true;
            art.ArtDateAdded = fi.CreationTime;

            Workspaces.Clear();
            AddArtViewModel workspace = new AddArtViewModel(Workspaces, art);
            Workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        private void ShowAddNewArt(object obj)
        {
            Workspaces.Clear();
            AddArtViewModel workspace = new AddArtViewModel(Workspaces);
            Workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }
        
        private void ExitArtMapper(object obj)
        {
            App.Current.MainWindow.Close();
        }

        public void SetActiveWorkspace(ViewModelBase workspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(Workspaces);
            collectionView?.MoveCurrentTo(workspace);
        }
    }
}
