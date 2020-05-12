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

        }


        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MsBuildPath))
            {
                CheckBuildPath();
            }

            if (e.PropertyName == nameof(MsBuildCommandLine))
            {
                CheckCommandLine();
            }
        }

        private void UpdateTokens()
        {
            var oldTokens = Tokens;
            oldTokens.ForEach(t => t.PropertyChanged -= TokenPropertyChanged);

            Tokens = TokenParser.Parse(MsBuildCommandLine);
            Tokens.ForEach(t => t.PropertyChanged += TokenPropertyChanged);

            // Apply old state
            foreach (var token in Tokens)
            {

                var firstOld = oldTokens.FirstOrDefault(t => t.TokenKey == token.TokenKey);
               
                if( firstOld == null)
                {
                    continue;
                }


                token.IsActive = firstOld.IsActive;

                transferSelectedElement(token, firstOld);

            }



        }

        private void TokenPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Tokens));
        }

        private void transferSelectedElement(Token newToken, Token oldToken)
        {
            var oldSelectedElement = oldToken.Values[oldToken.SelectedElement];

            for (var i = 0; i < newToken.Values.Count; ++i)
            {
                var foundToken = newToken.Values[i];
                if (oldSelectedElement == foundToken)
                {
                    newToken.SelectedElement = i;
                    return;
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
