using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtMapper.Config
{
    public static class Settings
    {
        public static string DbPath = $"{AppDomain.CurrentDomain.BaseDirectory}artmapper.sqlite";
    }
}
