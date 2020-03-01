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
    public class MainWindowViewModelDesignerOnly : MainWindowViewModel
    {
        public MainWindowViewModelDesignerOnly()
        {
            RootNodes = new ObservableCollection<DirectoryNode>();

            var pr1 = new Project("Some Project");
            var pr2 = new Project("Other");
            var pr3 = new Project("Non");
            var pr4 = new Project("Def");
            var pr5 = new Project("Go");


            RootNodes.Add(
                new DirectoryNode("Top Level", 
                    new DirectoryNode("Child one")
                        {Projects = new ObservableCollection<Project>{pr1}}, 
                    new DirectoryNode("Child 2"){ Projects = new ObservableCollection<Project>{pr2, pr3}}));

            RootNodes.Add(
                new DirectoryNode("Top Level Other Entry", 
                    new DirectoryNode("Also child",
                        new DirectoryNode("Deeper child") {Projects = new ObservableCollection<Project>{pr4, pr5}})));

        }
    }
}
