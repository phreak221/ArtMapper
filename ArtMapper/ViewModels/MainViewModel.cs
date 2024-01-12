using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtMapper.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<ViewModelBase> _workspaces;

        public MainViewModel(ObservableCollection<ViewModelBase> workspaces)
        {
            _workspaces = workspaces;
        }
    }
}
