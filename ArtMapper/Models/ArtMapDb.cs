using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SQLite;

namespace ArtMapper.Models
{
    [Table("ArtMapDb")]
    public class ArtMapDb
    {
        [Column("ArtMapID")]
        [PrimaryKey, AutoIncrement, Unique]
        public int ArtMapID { get; set; }

        [Column("ArtName")]
        public string ArtName { get; set; }

        [Column("ArtPath")]
        public string ArtPath { get; set; }

        [Column("ArtDimentions")]
        public string ArtDimentions { get; set; }

        [Column("ArtFileSize")]
        public string ArtFileSize { get; set; }

        [Column("ArtDateAdded")]
        public DateTime ArtDateAdded { get; set; }

        [Column("ArtDeleted")]
        public bool ArtDeleted { get; set; }

        [Column("ArtExists")]
        public bool ArtExists { get; set; }
    }
}
