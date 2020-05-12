using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MSBuildRunnerGUI.Logic;
using MSBuildRunnerGUI.Persistence;

namespace MSBuildRunnerGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Hinting to english, but has no effect on msbuild unless OS language changes
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
            Environment.SetEnvironmentVariable("VSLANG","1033");


            var settingsLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MSBuildRunnerGUI", "user.settings");

            var io = new FileIO();
            var userSettingsManager = new UserSettingsManager(fileIO: io, fileLocation: settingsLocation);
            var vm = new MainWindowViewModel(io, userSettingsManager);

            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = vm;
           
            mainWindow.Show();
        }
    }
}
