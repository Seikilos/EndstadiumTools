using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using MSBuildRunnerGUI.Annotations;
using MSBuildRunnerGUI.Contracts;

namespace MSBuildRunnerGUI.Data
{
    public class Project : INotifyPropertyChanged
    {
        public string Name => Path.GetFileName(FullPath);
       
        public int TotalDependencies { get; private set; }
        public int DependenciesOnThisLevel { get; private set; }

        public bool ScanCompleted => _scanTask.IsCompleted;

        // ReSharper disable once NotAccessedField.Local
        private readonly Task _scanTask;
        private BuildResultEnum _buildResult;

        public enum BuildResultEnum
        {
            Unknown,
            Failed,
            Successful
        }

        public BuildResultEnum BuildResult
        {
            get => _buildResult;
            set
            {
                if (value == _buildResult) return;
                _buildResult = value;
                OnPropertyChanged();
            }
        }

        public string FullPath { get; }

        private IFileIO _fileIO;

        public Project(string fullPath, IFileIO fileIO)
        {
            FullPath = fullPath;
            _fileIO = fileIO;
            BuildResult = BuildResultEnum.Unknown;

            _scanTask = Task.Run(ScanProjectFile);
        }

        private void ScanProjectFile()
        {
            if (_fileIO.Exists(FullPath) == false)
            {
                TotalDependencies = -1;
                return;
            }

            try
            {
                var doc = XDocument.Parse(_fileIO.ReadFile(FullPath));

                // Ignore namespaces to be independent from old and new project types
                var projectReferences = doc.XPathSelectElements("//*[contains(local-name(),'ProjectReference')]");

                foreach (var projectReference in projectReferences)
                {
                    var includePath = projectReference.Attribute("Include");

                    if (includePath == null)
                    {
                        continue;
                    }

                    // If more than one ".." exit, it is an external dependency
                    var c = Regex.Matches(includePath.Value, "\\.\\.").Count;

                    if (c < 2)
                    {
                        ++DependenciesOnThisLevel;
                    }
                    ++TotalDependencies;
                }

            }
            catch (Exception e)
            {
                TotalDependencies = -2;
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
