using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
using Microsoft.Win32;

namespace CompareTwoFiles
{
    /// <summary>
    /// Interaction logic for HashFilePickerControl.xaml
    /// </summary>
    public partial class HashFilePickerControl : UserControl, INotifyPropertyChanged
    {
        private string _filePath;
        private string _hash;

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (value == _filePath) return;
                _filePath = value;
                OnPropertyChanged();

                if (File.Exists(FilePath))
                {
                    _createHash();
                }
                else
                {
                    Hash = null; // Invalidate hash
                }
            }
        }

        public string Hash
        {
            get { return _hash; }
            private set
            {
                if (value == _hash) return;
                _hash = value;
                OnPropertyChanged();
            }
        }

        private void _createHash()
        {
            // Force null access
            Hash = HashProviderGetHash(FilePath);

        }


        /// <summary>
        /// Default event invoked to obtain a hash from arbitrary hash providers
        /// </summary>
        public event Func<string, string> HashProviderGetHash;

        public HashFilePickerControl()
        {
            InitializeComponent();

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SelectFileClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();

            // Set filter options and filter index.
            fileDialog.Filter = "All Files (*.*)|*.*";
           

            // Call the ShowDialog method to show the dialog box.
            var userClickedOK = fileDialog.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {

                FilePath = fileDialog.FileName;
            }
        }

        private void TextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void TextBox_Drop(object sender, DragEventArgs e)
        {
            // Get data object
            var dataObject = e.Data as DataObject;
            System.Diagnostics.Debug.Assert(dataObject != null);

            // Check for file list
            if (dataObject.ContainsFileDropList() && dataObject.GetFileDropList().Count > 0)
            {
                FilePath = dataObject.GetFileDropList()[0];
            }
        }

        private void TextBox_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
    }
}
