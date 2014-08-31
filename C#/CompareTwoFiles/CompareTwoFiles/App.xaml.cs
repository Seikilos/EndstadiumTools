using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CompareTwoFiles
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string FcivPath;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            FcivPath = Path.Combine(Directory.GetCurrentDirectory(), "fciv.exe");

            if (File.Exists(FcivPath) == false)
            {
                MessageBox.Show(string.Format("Could not locate fciv.exe at current working directory at '{0}', aborting.", FcivPath), "Fciv not found", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
            }
        }
    }
}
