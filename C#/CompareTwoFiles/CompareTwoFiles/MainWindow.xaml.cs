using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CompareTwoFiles.Annotations;

namespace CompareTwoFiles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
       
        private bool _equal;
        private string _leftHash;
        private string _rightHash;

        public bool Equal
        {
            get { return _equal; }
            set
            {
                if (value.Equals(_equal)) return;
                _equal = value;
                OnPropertyChanged();
            }
        }

        public string LeftHash
        {
            get { return _leftHash; }
            private set
            {
                if (value == _leftHash) return;
                _leftHash = value;
                OnPropertyChanged();
                _updateEqual();
              
            }
        }

       

        public string RightHash
        {
            get { return _rightHash; }
            private set
            {
                if (value == _rightHash) return;
                _rightHash = value;
                OnPropertyChanged();
                _updateEqual();
            }
        }

        private void _updateEqual()
        {
            Equal = _leftHash == _rightHash;
        }

        public MainWindow()
        {
            InitializeComponent();

            LeftControl.PropertyChanged += (s,e) => LeftHash = _getHash(s,e);
            RightControl.PropertyChanged += (s, e) => RightHash = _getHash(s, e);
        }



        private string _getHash(object sender, PropertyChangedEventArgs e)
        {
            if (sender is HashFilePickerControl && e.PropertyName == "Hash")
            {
                return  (sender as HashFilePickerControl).Hash;
            }
            return null;
        }
        

        private string HashFilePickerControl_OnHashProviderGetHash(string arg)
        {
            var pi = new ProcessStartInfo
                {
                    FileName =  App.FcivPath,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = string.Format("-add \"{0}\" -md5 ", arg)
                };
            var res = Process.Start(pi);

            res.WaitForExit();

            if (res.ExitCode != 0)
            {
                return null;
            }

            var str = res.StandardOutput.ReadToEnd();
            var lines = str.Trim().Split(new [] { Environment.NewLine }, StringSplitOptions.None);

            if (lines.Count() < 4) 
            {
                return null;
            }
            return lines[3].Substring(0, 32);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged<T>(Expression<Func<T>> selectorExpression)
        {
            if (selectorExpression == null)
                throw new ArgumentNullException("selectorExpression");
            var body = selectorExpression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("The body must be a member expression");
            OnPropertyChanged(body.Member.Name);
        }
    }
}
