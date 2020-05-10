using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using MSBuildRunnerGUI.Annotations;
using MSBuildRunnerGUI.Logic;

namespace MSBuildRunnerGUI.Data
{
    public class Settings : INotifyPropertyChanged
    {

        private string _msBuildPath;
        private string _msBuildCommandLine;
        private bool _validMsBuildPath;
        private bool _validMsCommandLine;
        private List<Token> _tokens;

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
                UpdateTokens();
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

        public List<Token> Tokens
        {
            get => _tokens;
            set
            {
                if (Equals(value, _tokens)) return;
                _tokens = value;
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
            Tokens = new List<Token>();
           
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

        private void UpdateTokens()
        {
            var oldTokens = Tokens;
            Tokens = TokenParser.Parse(MsBuildCommandLine);

            // Apply old state
            foreach (var token in Tokens)
            {
                var firstOld = oldTokens.FirstOrDefault(t => t.TokenKey == token.TokenKey);
                if (firstOld != null)
                {
                    token.IsActive = firstOld.IsActive;
                }
            }

        }
        private void CheckCommandLine()
        {

            ValidMsCommandLine = MsBuildCommandLine.Contains("%file%");
        }

        private void CheckBuildPath()
        {
            ValidMsBuildPath = File.Exists(MsBuildPath);
        }
    }

}
