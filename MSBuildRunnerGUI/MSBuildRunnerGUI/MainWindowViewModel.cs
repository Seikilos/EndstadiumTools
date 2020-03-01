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
using MSBuildRunnerGUI.Logic;
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

        private readonly HashSet<string> directoryBlackList = new HashSet<string>(new []{".git", ".vs"});

        public ObservableCollection<DirectoryNode> RootNodes { get; protected set; }


        public DelegateCommand ToggleSettingsCommand { get; }

        public DelegateCommand LoadProjectsCommand { get; }

        public DelegateCommand<DirectoryNode> RunBuildForDirectoryCommand { get; }
        public DelegateCommand<Project> RunBuildForProjectCommand { get; }

        public MainWindowViewModel()
        {
            PropertyChanged += MainWindowViewModel_PropertyChanged;

            Settings = new Settings();
            ToggleSettingsCommand = new DelegateCommand(() => SettingsActive = !SettingsActive);
            LoadProjectsCommand = new DelegateCommand(LoadProjects, CanLoadProjects);
            PathToDirectory = Properties.Settings.Default.PathToDirectory;
            RootNodes = new ObservableCollection<DirectoryNode>();

            RunBuildForDirectoryCommand = new DelegateCommand<DirectoryNode>(RunBuildForDirectory);
            RunBuildForProjectCommand = new DelegateCommand<Project>(RunBuildForProject);

        }

        private void RunBuildForProject(Project project)
        {

            _runBuildForProject(project, true);

        }

        private void RunBuildForDirectory(DirectoryNode node)
        {
            var listOfProjects = _gatherProjectsRecursively(node);

            foreach (var project in listOfProjects)
            {
                _runBuildForProject(project, false);
            }

        }

        private void _runBuildForProject(Project project, bool waitForWindow)
        {
            var runner = new MsBuildRunner(Settings.MsBuildPath, Settings.MsBuildCommandLine);

            var exitCode = runner.RunMsBuild(project.FullPath, waitForWindow);

            if (exitCode == 0)
            {
                project.BuildResult = Project.BuildResultEnum.Successful;
            }
            else
            {
                project.BuildResult = Project.BuildResultEnum.Failed;
            }
        }

        private List<Project> _gatherProjectsRecursively(DirectoryNode node)
        {
            var projects = new List<Project>(node.Projects);

            foreach (var directoryNode in node.Children)
            {
                projects.AddRange(_gatherProjectsRecursively(directoryNode));
            }

            return projects;
        }

        private bool CanLoadProjects()
        {
            return Directory.Exists(PathToDirectory);
        }

        private async void LoadProjects()
        {
            try
            {
                if (CanLoadProjects() == false)
                {
                    return;
                }

                RootNodes.Clear();

                var dRoot = new DirectoryNode(_getNameForDirectory(PathToDirectory));
                RootNodes.Add(dRoot);
                await _addDirectoriesRecursivelyAsync(dRoot, PathToDirectory);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            
        }

        private async Task _addDirectoriesRecursivelyAsync(DirectoryNode parent, string pathToDirectory)
        {

            foreach (var file in Directory.GetFiles(pathToDirectory, "*.*proj"))
            {
                parent.Projects.Add(new Project(file));
            }


            foreach (var childDirectory in Directory.GetDirectories(pathToDirectory))
            {
                if (SkipThisDirectory(childDirectory))
                {
                    continue;
                }

                var node = new DirectoryNode(_getNameForDirectory(childDirectory));

                await _addDirectoriesRecursivelyAsync(node, childDirectory);

                if (node.HasProjectsRecursively)
                {
                    parent.Children.Add(node);
                }
            }
        }

        private bool SkipThisDirectory(string pathToDirectory)
        {
            if (directoryBlackList.Contains(new DirectoryInfo(pathToDirectory).Name))
            {
                return true;
            }

            return false;
        }

        private string _getNameForDirectory(string directory)
        {
            return new DirectoryInfo(directory).Name;
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
