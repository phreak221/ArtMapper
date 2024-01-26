using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            BuildMoviePosterList();
            _workspaces = workspaces;
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
                    ArtPosterList.Add(new ArtMapModel()
                    {
                        ArtName = art.ArtName,
                        ArtPath = art.ArtPath,
                        ArtDimentions = art.ArtDimentions,
                        ArtDateAdded = art.ArtDateAdded,
                        ArtExists = art.ArtExists,
                        OpenLocationCommand = BtnOpenLocation
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
    }
}
