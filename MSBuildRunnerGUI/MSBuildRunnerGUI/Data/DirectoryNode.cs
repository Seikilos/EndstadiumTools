using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using MSBuildRunnerGUI.Annotations;

namespace MSBuildRunnerGUI.Data
{
    public class DirectoryNode: INotifyPropertyChanged
    {

        public string Name { get; }


        public ObservableCollection<DirectoryNode> Children { get; set; }

        public ObservableCollection<Project> Projects { get; set; }

        public bool HasProjectsRecursively => _getProjectsRecursively(this);

        private bool _getProjectsRecursively(DirectoryNode node)
        {

            if (node.Projects.Any())
            {
                return true;
            }

            foreach (var child in Children)
            {
                if (child.HasProjectsRecursively)
                {
                    return true;
                }
            }

            return false;
        }

        public DirectoryNode(string name, params DirectoryNode[] children)
        {
            Name = name;
            Children = new ObservableCollection<DirectoryNode>(children);
            Projects = new ObservableCollection<Project>();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
