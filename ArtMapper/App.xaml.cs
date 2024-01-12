using ArtMapper.Config;
using ArtMapper.Models;
using ArtMapper.ViewModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO; 
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Threading;
//using System.Windows.Forms;
using SHDocVw;
using Shell32;

namespace ArtMapper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private List<string> _result = new List<string>();

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        private static IntPtr lastHandle = IntPtr.Zero;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!CreateMutex("123AKs82kA,ylAo2kAlUS2kYkala!")) return;
            Thread staThread = new Thread(() =>
            {
                _result = GetExplorerSelectedFiles();
                //_result.Add(@"C:\Users\phreak\Desktop\20200525_130738.jpg");
            });
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();

            if (!File.Exists(Settings.DbPath))
                BuildDatabase();

            base.OnStartup(e);
            MainWindow window = new MainWindow();
            var viewModel = new MainWindowViewModel(_result.ToArray());
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

        private static Mutex _appMutex;

        private static bool CreateMutex(string name)
        {
            _appMutex = new Mutex(false, name, out bool createdNew);
            return createdNew;
        }

        private static void CloseMutex()
        {
            if (_appMutex != null)
            {
                _appMutex.Close();
                _appMutex = null;
            }
        }

        private IntPtr GetLastActive()
        {
            IntPtr curHandle = GetForegroundWindow();

            do
            {
                IntPtr retHandle = lastHandle;
                lastHandle = curHandle;
                if (retHandle != IntPtr.Zero)
                    return retHandle;
            } while (curHandle == lastHandle);
            return IntPtr.Zero;
        }

        private List<string> GetExplorerSelectedFiles()
        {
            var lastActive = GetLastActive();
            List<string> selectFiles = new List<string>();
            Shell shell = new Shell();
            foreach (ShellBrowserWindow win in shell.Windows())
            {
                if (win.HWND == (int)lastActive)
                {
                    if (win.Document != null)
                    {
                        foreach (FolderItem fi in win.Document.SelectedItems)
                        {
                            selectFiles.Add(fi.Path);
                        }
                    }
                }
            }
            return selectFiles;
        }
    }
}
