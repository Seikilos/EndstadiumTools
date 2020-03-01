using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuildRunnerGUI.Data
{
    public class Project
    {
        public string Name { get; set; }
        public int TotalDependencies { get; set; }
        public int DependenciesOnThisLevel { get; set; }

        public Project(string name, int totalDependencies, int dependenciesOnThisLevel)
        {
            Name = name;
            TotalDependencies = totalDependencies;
            DependenciesOnThisLevel = dependenciesOnThisLevel;
        }
    }
}
