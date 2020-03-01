using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MSBuildRunnerGUI.Annotations;
using MSBuildRunnerGUI.Data;
using Prism.Commands;

namespace MSBuildRunnerGUI
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public bool SettingsActive
        {
            get => _settingsActive;
            set
            {
                if (value == _settingsActive) return;
                _settingsActive = value;
                OnPropertyChanged();
            }
        }


        private bool _settingsActive;
        private string _pathToDirectory;


        public Settings Settings { get;  }


        public string PathToDirectory
        {
            get => _pathToDirectory;
            set
            {
                if (value == _pathToDirectory) return;
                _pathToDirectory = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<DirectoryNode> RootNodes { get; protected set; }


        public DelegateCommand ToggleSettingsCommand { get; }

        public DelegateCommand LoadProjectsCommand { get; }

        public MainWindowViewModel()
        {
            PropertyChanged += MainWindowViewModel_PropertyChanged;

            Settings = new Settings();
            ToggleSettingsCommand = new DelegateCommand(() => SettingsActive = !SettingsActive);
            LoadProjectsCommand = new DelegateCommand(LoadProjects, CanLoadProjects);
            PathToDirectory = Properties.Settings.Default.PathToDirectory;

        }

        private bool CanLoadProjects()
        {
            return Directory.Exists(PathToDirectory);
        }

        private void LoadProjects()
        {
            if (CanLoadProjects() == false)
            {
                return;
            }

            RootNodes = new ObservableCollection<DirectoryNode>();
            RootNodes.Add(new DirectoryNode("Foo"));

            
        }

        private void MainWindowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(PathToDirectory):
                    Properties.Settings.Default.PathToDirectory = PathToDirectory;
                    Properties.Settings.Default.Save();
                    LoadProjectsCommand.RaiseCanExecuteChanged();
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
