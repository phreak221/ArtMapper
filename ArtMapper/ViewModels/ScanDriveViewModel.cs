using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ArtMapper.Models;

namespace ArtMapper.ViewModels
{
    public class ScanDriveViewModel : ViewModelBase
    {
        public ICommand BtnSearchPath { get; set; }
        public ICommand BtnAddImage { get; set; }        
        private ObservableCollection<ViewModelBase> _workspaces;
        private ObservableCollection<ImgFileInfo> _imgFiles;

        public ObservableCollection<ImgFileInfo> ImgFiles
        {
            get => _imgFiles;
            set
            {
                if (_imgFiles == value) return;
                _imgFiles = value;
                OnPropertyChanged("ImgFiles");
            }
        }

        private string _textSearchPath;

        public string TextSearchPath
        {
            get => _textSearchPath;
            set
            {
                if (_textSearchPath == value) return;
                _textSearchPath = value;
                OnPropertyChanged("TextSearchPath");
            }
        }
        
        public ScanDriveViewModel(ObservableCollection<ViewModelBase> workspaces)
        {
            _workspaces = workspaces;
            BtnSearchPath = new RelayCommand(SearchInImgPath);
            BtnAddImage = new RelayCommand(AddImageToLibrary);
        }

        private void AddImageToLibrary(object obj)
        {
            string imgObj = (string)obj;

            ArtMapDb art = new ArtMapDb();
            art.ArtName = $"New Image {DateTime.Now}";
            art.ArtPath = imgObj;
            BitmapImage img = new BitmapImage(new Uri(imgObj));
            art.ArtDimentions = $"{img.Width} X {img.Height}";
            FileInfo fi = new FileInfo(imgObj);
            art.ArtFileSize = $"{fi.Length}";
            art.ArtExists = true;
            art.ArtDateAdded = fi.CreationTime;

            _workspaces.Clear();
            AddArtViewModel workspace = new AddArtViewModel(_workspaces, art);
            _workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        private void SearchInImgPath(object obj)
        {
            List<ImgFileInfo> imgSearchResults = new List<ImgFileInfo>();
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                var artSearchList = Directory.EnumerateFiles(folderDialog.SelectedPath, "*.*", SearchOption.AllDirectories).Where(x => x.Contains(".jpg") || x.Contains(".png"));
                foreach (string artSearch in artSearchList)
                {
                    FileInfo fi = new FileInfo(artSearch);
                    ImgFileInfo imgInfo = new ImgFileInfo()
                    {
                        ImageLocation = artSearch,
                        ImageSize = fi.Length.ToString(),
                        ImageCreate = fi.CreationTime,
                        ButtonAddImage = BtnAddImage
                    };
                    imgSearchResults.Add(imgInfo);
                }
                TextSearchPath = folderDialog.SelectedPath;
            }

            ImgFiles = new ObservableCollection<ImgFileInfo>(imgSearchResults);
            CollectionViewSource.GetDefaultView(ImgFiles).Refresh();
        }

        public void SetActiveWorkspace(ViewModelBase workspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(workspace);
            collectionView?.MoveCurrentTo(workspace);
        }
    }
}
