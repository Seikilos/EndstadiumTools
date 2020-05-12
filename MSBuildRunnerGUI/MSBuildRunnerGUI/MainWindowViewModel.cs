using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MSBuildRunnerGUI.Annotations;
using MSBuildRunnerGUI.Contracts;
using MSBuildRunnerGUI.Data;
using MSBuildRunnerGUI.Logic;
using MSBuildRunnerGUI.Persistence;
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

        protected IFileIO _fileIO;
        private readonly IUserSettingsManager _userSettingsManager;


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
        private string _finalCommandLine;

        public ObservableCollection<DirectoryNode> RootNodes { get; protected set; }

        public string FinalCommandLine
        {
            get => _finalCommandLine;
            set
            {
                if (value == _finalCommandLine) return;
                _finalCommandLine = value;
                OnPropertyChanged();
            }
        }


        public DelegateCommand ToggleSettingsCommand { get; }

        public DelegateCommand LoadProjectsCommand { get; }

        public DelegateCommand OpenPersistedFileLocationCommand { get; }

        public DelegateCommand<DirectoryNode> RunBuildForDirectoryCommand { get; }
        public DelegateCommand<Project> RunBuildForProjectCommand { get; }


        public MainWindowViewModel(IFileIO fileIO, IUserSettingsManager userSettingsManager)
        {
            _fileIO = fileIO;
            _userSettingsManager = userSettingsManager;
            PropertyChanged += MainWindowViewModel_PropertyChanged;


            ToggleSettingsCommand = new DelegateCommand(() => SettingsActive = !SettingsActive);
            LoadProjectsCommand = new DelegateCommand(LoadProjects, CanLoadProjects);
            OpenPersistedFileLocationCommand = new DelegateCommand(OpenPersistedFileLocation);

            SettingsActive = true;
            Settings = new Settings();
            LoadSettings(); // done before INPC registration to avoid redundant saves
            Settings.PropertyChanged += SettingsOnPropertyChanged;
            
            RootNodes = new ObservableCollection<DirectoryNode>();

            RunBuildForDirectoryCommand = new DelegateCommand<DirectoryNode>(RunBuildForDirectory);
            RunBuildForProjectCommand = new DelegateCommand<Project>(RunBuildForProject);

           
        }

        private void OpenPersistedFileLocation()
        {
            Process.Start(Path.GetDirectoryName(_userSettingsManager.StorageFile));

        }

        private void LoadSettings()
        {
            var userSettings = _userSettingsManager.LoadFromLocation();

            Settings.MsBuildPath = userSettings.MsBuildExePath;
            Settings.MsBuildCommandLine = userSettings.MsBuildCommandLine;

            if (Settings.Tokens.Count != userSettings.TokenPositions.Count)
            {
                throw new InvalidOperationException($"Token count mismatch. Parsed {Settings.Tokens.Count} but token position count was {userSettings.TokenPositions.Count}");
            }

            if (Settings.Tokens.Count != userSettings.ActiveStates.Count)
            {
                throw new InvalidOperationException($"Active state count mismatch. Parsed {Settings.Tokens.Count} but active states count was {userSettings.ActiveStates.Count}");
            }

            for (var i = 0; i < Settings.Tokens.Count; ++i)
            {
                var token = Settings.Tokens[i];

                token.IsActive = userSettings.ActiveStates[i];

                var tokenPosition = userSettings.TokenPositions[i];
                if (tokenPosition < 0 || tokenPosition >= token.Values.Count)
                {
                    throw new InvalidOperationException($"Token position '{tokenPosition}' is invalid. Token '{token.TokenKey}' supports values: (0, {token.Values.Count-1}].");
                }

                token.SelectedElement = tokenPosition;
            }

            PathToDirectory = userSettings.LastSetDirectory;

        }

        private void SettingsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Settings.MsBuildCommandLine) || e.PropertyName == nameof(Settings.Tokens))
            {
                CreateFinalCommandLine();
            }

            // Persist settings on any Settings changes
            SaveSettings();

        }

        private void SaveSettings()
        {
            var us = new UserSettings();
            us.MsBuildCommandLine = Settings.MsBuildCommandLine;
            us.MsBuildExePath = Settings.MsBuildPath;
            us.LastSetDirectory = PathToDirectory;

            us.ActiveStates = Settings.Tokens.Select(t => t.IsActive).ToList();
            us.TokenPositions = Settings.Tokens.Select(t => t.SelectedElement).ToList();

            _userSettingsManager.SaveSettings(us);
        }

        private void CreateFinalCommandLine()
        {
            var sb = new StringBuilder();

            foreach (var token in Settings.Tokens)
            {
                if (token.IsActive == false)
                {
                    continue;
                }

                sb.Append(token.Values[token.SelectedElement]);
                sb.Append(" ");
            }

            FinalCommandLine = sb.ToString().TrimEnd();

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
            var runner = new MsBuildRunner(_fileIO, Settings.MsBuildPath, FinalCommandLine);

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
                parent.Projects.Add(new Project(file, _fileIO));
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
                    SaveSettings();
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
