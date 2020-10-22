using System.IO;
using System.Windows;
using System.Windows.Controls;
using AudioSteganographyProject.Interfaces;
using AudioSteganographyProject.Properties;
using AudioSteganographyProject.UI.Models;

namespace AudioSteganographyProject.UI.Views
{
    /// <summary>
    /// Interaction logic for FileSelectionWindow.xaml
    /// </summary>
    public partial class HideSongPage : Page, SteganographyPageInterface
    {
        public HideSongPageModel model;

        public HideSongPage()
        {
            InitializeComponent();

            this.model = new HideSongPageModel();
        }

        public HideSongPage(HideSongPageModel existingModel)
        {
            InitializeComponent();

            this.model = existingModel;
        }

        public PageModel getModel()
        {
            return this.model;
        }

        public void updateModel()
        {
            model.updateModel(this);
        }

        public void onCreateButton_Click(object sender, RoutedEventArgs e)
        {
            string songPath = this.SongFile.FilePathString;
            string fileToHidePath = this.HiddenFile.FilePathString;
            string dirPath = this.NewFilePath.FilePathString;

            if (File.Exists(@songPath) && File.Exists(@fileToHidePath) && Directory.Exists(@dirPath))
            {
                WAVReader.HideFile(songPath, fileToHidePath, dirPath);
                //Thread thread = new Thread(new ThreadStart(WAVReader.HideFile(songPath, fileToHidePath, dirPath)));
            }
            else
            {
                MessageBox.Show("Not all paths exist. Make sure that all paths exist and your song is a .wav file.", "Stegangraphy Project", MessageBoxButton.OK);
            }
        }
    }
}
