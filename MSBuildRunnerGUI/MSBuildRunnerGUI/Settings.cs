using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MSBuildRunnerGUI.Annotations;

namespace MSBuildRunnerGUI
{
    public class Settings : INotifyPropertyChanged
    {

        private string _msBuildPath;
        private string _msBuildCommandLine;
        private bool _validMsBuildPath;
        private bool _validMsCommandLine;

        public string MsBuildPath
        {
            get => _msBuildPath;
            set
            {
                if (value == _msBuildPath) return;
                _msBuildPath = value;
                OnPropertyChanged();
            }
        }

        public string MsBuildCommandLine
        {
            get => _msBuildCommandLine;
            set
            {
                if (value == _msBuildCommandLine) return;
                _msBuildCommandLine = value;
                OnPropertyChanged();
            }
        }

        public bool ValidMsBuildPath
        {
            get => _validMsBuildPath;
            set
            {
                if (value == _validMsBuildPath) return;
                _validMsBuildPath = value;
                OnPropertyChanged();
            }
        }

        public bool ValidMsCommandLine
        {
            get => _validMsCommandLine;
            set
            {
                if (value == _validMsCommandLine) return;
                _validMsCommandLine = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public Settings()
        {
            PropertyChanged += Settings_PropertyChanged;

            MsBuildCommandLine = Properties.Settings.Default.MsBuidlCommandLine;
            MsBuildPath = Properties.Settings.Default.MsBuildPath;

       
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MsBuildPath))
            {
                Properties.Settings.Default.MsBuildPath = MsBuildPath;
                CheckBuildPath();
            }

            if (e.PropertyName == nameof(MsBuildCommandLine))
            {
                Properties.Settings.Default.MsBuidlCommandLine = MsBuildCommandLine;
                CheckCommandLine();

            }

            Properties.Settings.Default.Save();
        }

        private void CheckCommandLine()
        {

            ValidMsCommandLine = MsBuildCommandLine.Contains("%msbuild%");
        }

        private void CheckBuildPath()
        {
            ValidMsBuildPath = File.Exists(MsBuildPath);
        }
    }

}
