using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MSBuildRunnerGUI.Annotations;
using Prism.Commands;

namespace MSBuildRunnerGUI.Data
{
    public class Token: INotifyPropertyChanged
    {
        private List<string> _values;
        private string _tokenKey;
        private bool _isActive;
        private int _selectedElement;

        public List<string> Values
        {
            get => _values;
            set
            {
                if (Equals(value, _values)) return;
                _values = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasVariants));
            }
        }

        public bool HasVariants => Values.Count > 1;

        /// <summary>
        /// Defines the unique name of the token without delimiters 
        /// </summary>
        public string TokenKey
        {
            get => _tokenKey;
            set
            {
                if (value == _tokenKey) return;
                _tokenKey = value;
                OnPropertyChanged();
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (value == _isActive) return;
                _isActive = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand ToggleActiveState { get; }

        public int SelectedElement
        {
            get => _selectedElement;
            set
            {
                if (value == _selectedElement) return;
                if(value < 0 || value >= Values.Count) throw new ArgumentOutOfRangeException(nameof(SelectedElement));
                _selectedElement = value;
                OnPropertyChanged();
            }
        }

        public Token(bool isActive, params string[] values)
        {
            Values = new List<string>(values);
            if (Values.Any() == false)
            {
                throw new ArgumentNullException(nameof(values));
            }

            IsActive = isActive;

            ToggleActiveState = new DelegateCommand(() => IsActive = !IsActive);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
