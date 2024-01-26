using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ArtMapper.Config;
using ArtMapper.Models;
using SQLite;

namespace ArtMapper.ViewModels
{
    public class ArtGridViewModel : ViewModelBase
    {
        private ObservableCollection<ViewModelBase> _workspaces;
        private ObservableCollection<ArtMapModel> _artPosterList;
        
        public ICommand BtnOpenLocation { get; set; }
        public ICommand BtnEditImage { get; set; }

        public ObservableCollection<ArtMapModel> ArtPosterList
        {
            get => _artPosterList;
            set
            {
                if (_artPosterList == value) return;
                _artPosterList = value;
                OnPropertyChanged("ArtPosterList");
            }
        }

        private ArtMapDb _selectedArtMap;

        public ArtMapDb SelectedArtMap
        {
            get => _selectedArtMap;
            set
            {
                if (_selectedArtMap == value) return;
                _selectedArtMap = value;
                OnPropertyChanged("SelectedArtMap");
            }
        }
        
        public ArtGridViewModel(ObservableCollection<ViewModelBase> workspaces)
        {
            //AddFile_Drop = new RelayCommand(AddArtToList);
            BtnOpenLocation = new RelayCommand(OpenImgLocation);
            BtnEditImage = new RelayCommand(EditImageInfo);
            BuildMoviePosterList();
            _workspaces = workspaces;
        }

        private void EditImageInfo(object obj)
        {
            int artIndex = (int)obj;
            SQLiteConnection conn = new SQLiteConnection(Settings.DbPath, SQLiteOpenFlags.ReadWrite, false);
            try
            {
                ArtMapDb art = conn.Table<ArtMapDb>().FirstOrDefault(x => x.ArtMapID == artIndex);
                _workspaces.Clear();
                AddArtViewModel workspace = new AddArtViewModel(_workspaces, art);
                _workspaces.Add(workspace);
                SetActiveWorkspace(workspace);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"EditImageInfo: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        private void OpenImgLocation(object obj)
        {
            string path = (string)obj;
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = path,
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
        }

        private void BuildMoviePosterList()
        {
            SQLiteConnection conn = new SQLiteConnection(Settings.DbPath, SQLiteOpenFlags.ReadWrite, false);
            try
            {
                ArtPosterList = new ObservableCollection<ArtMapModel>();
                var arts = conn.Table<ArtMapDb>().ToList();
                foreach (var art in arts)
                {
                    var doesExists = (File.Exists(art.ArtPath));
                    ArtPosterList.Add(new ArtMapModel()
                    {
                        ArtMapId = art.ArtMapID,
                        ArtName = art.ArtName,
                        ArtPath = (doesExists) ? art.ArtPath : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NoPoster.jpg"),
                        ArtDimentions = art.ArtDimentions,
                        ArtDateAdded = art.ArtDateAdded,
                        ArtExists = doesExists,
                        ArtStatus = art.ArtDeleted ? "Deleted" : "Active",
                        OpenLocationCommand = BtnOpenLocation,
                        EditImageCommand = BtnEditImage
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"BuildMoviePoserList {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        public void SetActiveWorkspace(ViewModelBase workspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(_workspaces);
            collectionView?.MoveCurrentTo(workspace);
        }
    }
}
