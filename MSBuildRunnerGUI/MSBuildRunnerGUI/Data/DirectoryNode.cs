using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MSBuildRunnerGUI.Annotations;

namespace MSBuildRunnerGUI.Data
{
    public class DirectoryNode: INotifyPropertyChanged
    {

        public string Name { get; }


        public ObservableCollection<DirectoryNode> Children { get; set; }

        public ObservableCollection<Project> Projects { get; set; }

        public DirectoryNode(string name, params DirectoryNode[] children)
        {
            Name = name;
            Children = new ObservableCollection<DirectoryNode>(children);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
