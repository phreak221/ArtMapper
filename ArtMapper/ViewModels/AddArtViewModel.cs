using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using ArtMapper.Config;
using ArtMapper.Models;
using SQLite;
using System.ComponentModel;
using System.Windows.Data;

namespace ArtMapper.ViewModels
{
    public class AddArtViewModel : ViewModelBase
    {
        private ObservableCollection<ViewModelBase> _workspaces;

        public ICommand BtnSaveArtMap { get; set; }
        public ICommand BtnCancelArtMap { get; set; }
        public ICommand BtnSearchForArt { get; set; }
        
        public AddArtViewModel(ObservableCollection<ViewModelBase> workspaces, ArtMapDb artData = null)
        {
            _workspaces = workspaces;

            BtnSaveArtMap = new RelayCommand(AddNewArtMap);
            BtnCancelArtMap = new RelayCommand(CancelArtMap);
            BtnSearchForArt = new RelayCommand(SearchForArt);
            ArtIsDeleted = false;
            ArtIsActive = true;
            ArtFound = false;
            ArtMissing = true;
            ArtDateAdded = $"{DateTime.Now}";

            if (artData != null)
            {
                ArtMapId = artData.ArtMapID;
                ArtName = artData.ArtName;
                ArtPath = artData.ArtPath;
                ArtDimentions = artData.ArtDimentions;
                ArtFileSize = artData.ArtFileSize;
                ArtDateAdded = $"{artData.ArtDateAdded}";
                ArtIsDeleted = artData.ArtDeleted;
                ArtIsActive = (!artData.ArtDeleted);
                ArtFound = artData.ArtExists;
                ArtMissing = !artData.ArtExists;
            }
        }

        private int _artMapId;
        public int ArtMapId
        {
            get => _artMapId;
            set
            {
                if (_artMapId == value) return;
                _artMapId = value;
                OnPropertyChanged("ArtMapId");
            }
        }

        private string _artName;
        public string ArtName
        {
            get => _artName;
            set
            {
                if (_artName == value) return;
                _artName = value;
                OnPropertyChanged("ArtName");
            }
        }

        private string _artPath;
        public string ArtPath
        {
            get => _artPath;
            set
            {
                if (_artPath == value) return;
                _artPath = value;
                OnPropertyChanged("ArtPath");
            }
        }

        private string _artDimentions;
        public string ArtDimentions
        {
            get => _artDimentions;
            set
            {
                if (_artDimentions == value) return;
                _artDimentions = value;
                OnPropertyChanged("ArtDimentions");
            }
        }

        private string _artFileSize;
        public string ArtFileSize
        {
            get => _artFileSize;
            set
            {
                if (_artFileSize == value) return;
                _artFileSize = value;
                OnPropertyChanged("ArtFileSize");
            }
        }

        private string _artDateAdded;
        public string ArtDateAdded
        {
            get => _artDateAdded;
            set
            {
                if (_artDateAdded == value) return;
                _artDateAdded = value;
                OnPropertyChanged("ArtDateAdded");
            }
        }

        private bool _artIsDeleted;
        public bool ArtIsDeleted
        {
            get => _artIsDeleted;
            set
            {
                if (_artIsDeleted == value) return;
                _artIsDeleted = value;
                OnPropertyChanged("ArtIsDeleted");
            }
        }

        private bool _artIsActive;
        public bool ArtIsActive
        {
            get => _artIsActive;
            set
            {
                if (_artIsActive == value) return;
                _artIsActive = value;
                OnPropertyChanged("ArtIsActive");
            }
        }

        private bool _artDeleted;
        public bool ArtDeleted
        {
            get => _artDeleted;
            set
            {
                if (_artDeleted == value) return;
                _artDeleted = value;
                OnPropertyChanged("ArtDeleted");
            }
        }

        private bool _artFound;
        public bool ArtFound
        {
            get => _artFound;
            set
            {
                if (_artFound == value) return;
                _artFound = value;
                OnPropertyChanged("ArtFound");
            }
        }

        private bool _artMissing;
        public bool ArtMissing
        {
            get => _artMissing;
            set
            {
                if (_artMissing == value) return;
                _artMissing = value;
                OnPropertyChanged("ArtMissing");
            }
        }

        private bool _artExists;
        public bool ArtExists
        {
            get => _artExists;
            set
            {
                if (_artExists == value) return;
                _artExists = value;
                OnPropertyChanged("ArtExists");
            }
        }

        private void AddNewArtMap(object obj)
        {
            SQLiteConnection conn = new SQLiteConnection(Settings.DbPath, SQLiteOpenFlags.ReadWrite, false);
            try
            {
                if (ArtMapId > 0)
                {
                    ArtMapDb mapArt = conn.Table<ArtMapDb>().FirstOrDefault(x => x.ArtMapID == ArtMapId);
                    if (mapArt != null)
                    {
                        mapArt.ArtName = ArtName;
                        mapArt.ArtPath = ArtPath;
                        mapArt.ArtDimentions = ArtDimentions;
                        mapArt.ArtFileSize = ArtFileSize;
                        mapArt.ArtDateAdded = Convert.ToDateTime(ArtDateAdded);
                        mapArt.ArtDeleted = ArtIsDeleted;
                        mapArt.ArtExists = File.Exists(_artPath);

                        conn.Update(mapArt);
                    }
                }
                else
                {
                    ArtMapDb art = new ArtMapDb();
                    art.ArtName = _artName;
                    art.ArtPath = _artPath;
                    art.ArtDimentions = _artDimentions;
                    art.ArtFileSize = _artFileSize;
                    art.ArtDateAdded = Convert.ToDateTime(_artDateAdded);
                    art.ArtDeleted = ArtIsDeleted;
                    art.ArtExists = File.Exists(_artPath);

                    conn.Insert(art);
                }

                _workspaces.Clear();
                ArtGridViewModel workspace = new ArtGridViewModel(_workspaces);
                _workspaces.Add(workspace);
                SetActiveWorkspace(workspace);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"AddNewArtMap {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        private void CancelArtMap(object obj)
        {
            _workspaces.Clear();
            ArtGridViewModel workspace = new ArtGridViewModel(_workspaces);
            _workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        private void SearchForArt(object obj)
        {
            try
            {
                OpenFileDialog artFileDialog = new OpenFileDialog();
                if (artFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ArtPath = artFileDialog.FileName;
                    FileInfo fi = new FileInfo(ArtPath);
                    ArtFileSize = $"{fi.Length}";
                    BitmapImage img = new BitmapImage(new Uri(ArtPath));
                    ArtDimentions = $"{img.Width} X {img.Height}";
                    ArtExists = true;
                    ArtFound = true;
                    ArtMissing = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"SearchForArt {ex.Message}");
            }
        }

        public void SetActiveWorkspace(ViewModelBase workspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(workspace);
            collectionView?.MoveCurrentTo(workspace);
        }
    }
}
