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

namespace MSBuildRunnerGUI.Data
{
    public class Project : INotifyPropertyChanged
    {
        private readonly string _fullPath;
        public string Name => Path.GetFileName(_fullPath);
       
        public int TotalDependencies { get; private set; }
        public int DependenciesOnThisLevel { get; private set; }

        private Task _scanTask;

        public Project(string fullPath)
        {
            _fullPath = fullPath;

            _scanTask = Task.Run(scanProjectFile);
        }

        private void scanProjectFile()
        {
            if (File.Exists(_fullPath) == false)
            {
                TotalDependencies = -1;
                return;
            }

            try
            {
                var doc = XDocument.Load(_fullPath);

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
