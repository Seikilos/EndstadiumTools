using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MSBuildRunnerGUI.Annotations;
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


        public Settings Settings { get;  }


        public DelegateCommand ToggleSettingsCommand { get; }

        public MainWindowViewModel()
        {
            Settings = new Settings();
            ToggleSettingsCommand = new DelegateCommand(() => SettingsActive = !SettingsActive);

        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
