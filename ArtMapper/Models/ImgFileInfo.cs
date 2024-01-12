using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ArtMapper.Models
{
    public class ImgFileInfo
    {
        public string ImageLocation { get; set; }
        public string ImageSize { get; set; }
        public DateTime ImageCreate { get; set; }

        public ICommand ButtonAddImage { get; set; }
    }
}
