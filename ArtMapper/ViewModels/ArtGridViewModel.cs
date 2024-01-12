using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ArtMapper.Config;
using ArtMapper.Models;
using SQLite;

namespace ArtMapper.ViewModels
{
    public class ArtGridViewModel : ViewModelBase
    {
        private ObservableCollection<ViewModelBase> _workspaces;
        private ObservableCollection<ArtMapDb> _moviePosterList;
        
        public ObservableCollection<ArtMapDb> MoviePosterList
        {
            get => _moviePosterList;
            set
            {
                if (_moviePosterList == value) return;
                _moviePosterList = value;
                OnPropertyChanged("MoviePosterList");
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
            BuildMoviePosterList();
            _workspaces = workspaces;
        }
        
        private void BuildMoviePosterList()
        {
            SQLiteConnection conn = new SQLiteConnection(Settings.DbPath, SQLiteOpenFlags.ReadWrite, false);
            MoviePosterList = new ObservableCollection<ArtMapDb>(conn.Table<ArtMapDb>().ToList());
            conn.Close();
        }
    }
}
