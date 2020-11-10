using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using AudioSteganographyProject.Interfaces;
using AudioSteganographyProject.Properties;
using AudioSteganographyProject.UI.Models;
using System;

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

        private async void onCreateButton_Click(object sender, RoutedEventArgs e)
        {
            string songPath = this.SongFile.FilePathString;
            string fileToHidePath = this.HiddenFile.FilePathString;
            string dirPath = this.NewFilePath.FilePathString;

            if (File.Exists(@songPath) && File.Exists(@fileToHidePath) && Directory.Exists(@dirPath))
            {
                try
                {
                    byte[] formatData = new byte[44];
                    //byte[] formatData = WAVReader.getFileFormatData(@songPath);
                    string newFilePath = Path.Combine(dirPath, Path.GetFileName(fileToHidePath));



                    using (FileStream destinationStream = File.Open(newFilePath, FileMode.Create))
                    {
                        using (FileStream originalSongStream = File.Open(songPath, FileMode.Open))
                        {
                            using (FileStream hiddenFileStream = File.Open(@fileToHidePath, FileMode.Open))
                            {
                                await HideFileDataAsync(formatData, originalSongStream, hiddenFileStream, destinationStream);
                            }
                        }
                    }
                    MessageBox.Show("The new .wav file has been created.", "Stegangraphy Project", MessageBoxButton.OK);
                    //Thread thread = new Thread(new ThreadStart(WAVReader.HideFile(songPath, fileToHidePath, dirPath)));
                }
                catch (IOException exception)
                {
                    Console.WriteLine(exception);
                }
            }
            else
            {
                MessageBox.Show("Not all paths exist. Make sure that all paths exist and your song is a .wav file.", "Stegangraphy Project", MessageBoxButton.OK);
            }
        }

        public async Task HideFileDataAsync(byte[] formatData, FileStream originalSong, FileStream fileDataToHide, FileStream destinationFile)
        {
            if (formatData != null)
            {
                //UInt32 sampleRate = BitConverter.ToUInt32(formatData, 24);
                //UInt16 sampleSize = BitConverter.ToUInt16(formatData, 32);



                // Read up to byte 44 and write it to the destination stream. Byte 45 is the start of sample data.
                byte[] bufferBeforeSamples = new byte[44];
                byte[] sampleDataBuffer = new byte[1000];
                byte[] fileDataBuffer = new byte[10];
                int numBytesRead_OriginalSong = 0;
                int readLength_SampleData = 0;
                int readLength_HiddenFile = 0;

                while (numBytesRead_OriginalSong < bufferBeforeSamples.Length)
                {
                    int readLength = await originalSong.ReadAsync(bufferBeforeSamples, 0, bufferBeforeSamples.Length - numBytesRead_OriginalSong);

                    if (readLength == 0)
                    {
                        break;
                    }

                    numBytesRead_OriginalSong += readLength;
                }

                await destinationFile.WriteAsync(bufferBeforeSamples, 0, bufferBeforeSamples.Length);

                /*
                 * Start reading sample data and data from the file to be hidden into buffers.
                 * When both have been read, splice the file data into the sample data, and 
                 * then write to the destination. Repeat until there is no more file data,
                 * at which point the remaining song data will be directly copied.
                 */

                while (readLength_SampleData != -1 && readLength_HiddenFile != -1)
                {

                    if (readLength_SampleData != -1)
                    {
                        readLength_SampleData = await originalSong.ReadAsync(sampleDataBuffer, 0, sampleDataBuffer.Length);
                    }

                    if (readLength_HiddenFile != -1)
                    {
                        readLength_HiddenFile = await fileDataToHide.ReadAsync(fileDataBuffer, 0, fileDataBuffer.Length);

                        for (int i = 0; i < readLength_HiddenFile; i++)
                        {
                            int j = i * sampleDataBuffer.Length / fileDataBuffer.Length;
                            sampleDataBuffer[j] = fileDataBuffer[i];
                        }
                    }

                    await destinationFile.WriteAsync(sampleDataBuffer, 0, sampleDataBuffer.Length);
                }
            }
        }
    }
}
