using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AudioSteganographyProject.UI.Controls
{
    /// <summary>
    /// Interaction logic for FileSelectionControl.xaml
    /// </summary>
    public partial class FileSelectionControl : System.Windows.Controls.UserControl
    {
        public FileSelectionControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string Title 
        { 
            get => descriptionTextBlock.Text;
            set => descriptionTextBlock.Text = value;
        }

        public string FilePathString
        {
            get => filePathInput.Text;
            set => filePathInput.Text = value;
        }

        private void browseFilesButton_Click(object sender, RoutedEventArgs e)
        {
            var fileExplorerDialog = new System.Windows.Forms.OpenFileDialog();
            var selection = fileExplorerDialog.ShowDialog();
            switch (selection)
            {
                case System.Windows.Forms.DialogResult.OK:
                    var fileName = fileExplorerDialog.FileName;
                    FilePathString = fileName;
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    break;
            }
        }
    }
}

