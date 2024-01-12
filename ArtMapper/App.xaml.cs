using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ArtMapper.Config;
using ArtMapper.Models;
using ArtMapper.ViewModels;
using SQLite;

namespace ArtMapper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (!File.Exists(Settings.DbPath))
                BuildDatabase();

            base.OnStartup(e);
            MainWindow window = new MainWindow();
            var viewModel = new MainWindowViewModel();
            window.DataContext = viewModel;
            window.Show();
        }

        private void BuildDatabase()
        {
            if (!File.Exists(Settings.DbPath))
                File.Create(Settings.DbPath).Close();

            SQLiteConnection conn = new SQLiteConnection(Settings.DbPath, SQLiteOpenFlags.ReadWrite, false);
            conn.CreateTable<ArtMapDb>();
            conn.Close();
        }
    }
}
