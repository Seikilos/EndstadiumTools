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
    public class MainWindowViewModelDesignerOnly : MainWindowViewModel
    {
        public MainWindowViewModelDesignerOnly() : base(new FileIO())
        {
            RootNodes = new ObservableCollection<DirectoryNode>();
          
            var pr1 = new Project("Some Project", _fileIO);
            var pr2 = new Project("Other", _fileIO);
            var pr3 = new Project("Non", _fileIO);
            var pr4 = new Project("Def", _fileIO);
            var pr5 = new Project("Go", _fileIO);


            RootNodes.Add(
                new DirectoryNode("Top Level", 
                    new DirectoryNode("Child one")
                        {Projects = new ObservableCollection<Project>{pr1}}, 
                    new DirectoryNode("Child 2"){ Projects = new ObservableCollection<Project>{pr2, pr3}}));

            RootNodes.Add(
                new DirectoryNode("Top Level Other Entry", 
                    new DirectoryNode("Also child",
                        new DirectoryNode("Deeper child") {Projects = new ObservableCollection<Project>{pr4, pr5}})));


            Settings.MsBuildPath = "Path to some exe";
            Settings.MsBuildCommandLine = "/p:Foo %file% -target:Clean /target:Build";
            Settings.Tokens[1].IsActive = false;

        }
    }
}
