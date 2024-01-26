using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ArtMapper.Models
{
    public class ArtMapModel
    {
        public int ArtMapId { get; set; }
        public string ArtName { get; set; }
        public string ArtPath { get; set; }
        public string ArtDimentions { get; set; }
        public DateTime ArtDateAdded { get; set; }
        public string ArtStatus { get; set; }
        public bool ArtExists { get; set; }
        public ICommand OpenLocationCommand { get; set; }
        public ICommand EditImageCommand { get; set; }
    }
}
