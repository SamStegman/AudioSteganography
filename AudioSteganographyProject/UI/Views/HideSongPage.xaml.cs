using AudioSteganographyProject.Interfaces;
using AudioSteganographyProject.UI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AudioSteganographyProject.UI.Views
{
    /// <summary>
    /// Interaction logic for FileSelectionWindow.xaml
    /// </summary>
    public partial class HideSongPage : Page, SteganographyPageInterface
    {
        public HideSongPageModel model;
        private byte[] RIFF_BYTES = Encoding.ASCII.GetBytes("RIFF");
        private byte[] DATA_BYTES = Encoding.ASCII.GetBytes("data");
        private byte[] STOP_BYTES = Encoding.ASCII.GetBytes("ASP STOP");

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

            Console.WriteLine(songPath);

            if (File.Exists(@songPath) && File.Exists(@fileToHidePath) && Directory.Exists(@dirPath))
            {
                try
                {
                    //byte[] formatData = new byte[44];
                    //byte[] formatData = WAVReader.getFileFormatData(@songPath);
                    string newFilePath = Path.Combine(dirPath, Path.GetFileName(songPath));

                    //FileStream destinationStream = File.Open(newFilePath, FileMode.Create);
                    //FileStream originalSongStream = File.Open(@songPath, FileMode.Open, FileAccess.Read);
                    //FileStream hiddenFileStream = File.Open(@fileToHidePath, FileMode.Open, FileAccess.Read);

                    //using (BinaryWriter binWriter = new BinaryWriter(destinationStream))
                    using (FileStream destinationStream = File.Open(@newFilePath, FileMode.Create))
                    {
                        //using (BinaryReader binReader_Song = new BinaryReader(originalSongStream))
                        using (FileStream originalSongStream = File.Open(@songPath, FileMode.Open, FileAccess.Read))
                        {
                            //using (BinaryReader binReader_HideFile = new BinaryReader(hiddenFileStream))
                            using (FileStream hiddenFileStream = File.Open(@fileToHidePath, FileMode.Open, FileAccess.Read))                            
                            {
                                byte[] formatData = await GetFormatData(originalSongStream);
                                //byte[] formatData = getFormatData(binReader_Song);

                                if (formatData.Length >= 8)
                                {
                                    byte[] dataByteCheck = new byte[4];
                                    Array.Copy(formatData, formatData.Length - 8, dataByteCheck, 0, 4);
                                    if (Enumerable.SequenceEqual(DATA_BYTES, dataByteCheck))
                                    {
                                        uint dataSubchunkBits = BitConverter.ToUInt32(formatData, formatData.Length - 8);
                                        FileInfo fileInfo = new FileInfo(fileToHidePath);

                                        // fileInfo.Length will always be positive and dataSubchunkBits will simply be expanded so no precision will be lost here.
                                        // We divide by 16 because each LSB XOR will require 2 bytes. 
                                        // 8 bytes is added to the length of the file to hide so the length of the stop message can be written.
                                        if (Convert.ToUInt64(dataSubchunkBits) / 16 > Convert.ToUInt64(fileInfo.Length) + 8)
                                        {
                                            //await destinationStream.WriteAsync(formatData, 0, formatData.Length);
                                            await SimpleDataCopyTest(formatData, originalSongStream, destinationStream);
                                            //await HideFileDataLSB(formatData, originalSongStream, hiddenFileStream, destinationStream);
                                            //await HideFileDataAsync(formatData, originalSongStream, hiddenFileStream, destinationStream);
                                            for (int i = 0; i < formatData.Length; i++)
                                            {
                                                Console.WriteLine(i + " " + formatData[i] + " " + Encoding.ASCII.GetString(formatData, i, 1));
                                            }
                                        }
                                    }
                                }
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

        public async Task SimpleDataCopyTest(byte[] formatData, FileStream originalSong, FileStream destinationFile)
        {
            //binWriter.Write(formatData);
            await destinationFile.WriteAsync(formatData, 0, formatData.Length);
            int remainingReadLength = -1;
            while (remainingReadLength != 0)
            {
                byte[] remainingDataBuffer = new byte[256];
                remainingReadLength = await originalSong.ReadAsync(remainingDataBuffer, 0, remainingDataBuffer.Length);
                //sbyte[] signedByteArray = new sbyte[remainingDataBuffer.Length];
                //signedByteArray = Array.ConvertAll(remainingDataBuffer, b => (sbyte)b);
                await destinationFile.WriteAsync(remainingDataBuffer.SelectMany(s => BitConverter.GetBytes((short)s)).ToArray(), 0, remainingReadLength);
            }
        }

        public byte[] getFormatData(BinaryReader originalSong)
        {
            List<byte> byteList = new List<byte>();
            Queue<byte> readQueue = new Queue<byte>();
            byte[] firstRead = new byte[4];
            byte[] formatDataArray = null;

            
            firstRead = originalSong.ReadBytes(firstRead.Length);
            if (Enumerable.SequenceEqual(RIFF_BYTES, firstRead))
            {
                byte nextFormatByte = 0;
                int readLength = -1;

                for (int i = 0; i < firstRead.Length; i++)
                {
                    readQueue.Enqueue(firstRead[i]);
                }

                nextFormatByte = originalSong.ReadByte();
                while (originalSong.BaseStream.Position != originalSong.BaseStream.Length)
                {
                    byteList.Add(readQueue.Dequeue());
                    readQueue.Enqueue(nextFormatByte);
                    nextFormatByte = originalSong.ReadByte();
                }

                while (readQueue.Count > 0)
                {
                    byteList.Add(readQueue.Dequeue());
                }

                byte[] dataChunkSizeBytes = new byte[4];
                dataChunkSizeBytes = originalSong.ReadBytes(dataChunkSizeBytes.Length);
                for (int i = 0; i < dataChunkSizeBytes.Length; i++)
                {
                    byteList.Add(dataChunkSizeBytes[i]);
                }
                formatDataArray = byteList.ToArray();
            }
            return formatDataArray;
        }

        public async Task<byte[]> GetFormatData(FileStream originalSong)
        {
            List<byte> byteList = new List<byte>();
            Queue<byte> readQueue = new Queue<byte>();
            byte[] firstRead = new byte[4];
            byte[] formatDataArray = null;

            await originalSong.ReadAsync(firstRead, 0, firstRead.Length);
            if (Enumerable.SequenceEqual(RIFF_BYTES, firstRead))
            {
                byte[] nextFormatByte = new byte[1];
                int readLength = -1;

                for (int i = 0; i < firstRead.Length; i++)
                {
                    readQueue.Enqueue(firstRead[i]);
                }
                
                readLength = await originalSong.ReadAsync(nextFormatByte, 0, nextFormatByte.Length);
                while (readLength != 0 && !Enumerable.SequenceEqual(DATA_BYTES, readQueue.ToArray()))
                {
                    byteList.Add(readQueue.Dequeue());
                    readQueue.Enqueue(nextFormatByte[0]);
                    readLength = await originalSong.ReadAsync(nextFormatByte, 0, nextFormatByte.Length);
                }

                while (readQueue.Count > 0)
                {
                    byteList.Add(readQueue.Dequeue());
                }

                byte[] dataChunkSizeBytes = new byte[4];
                await originalSong.ReadAsync(dataChunkSizeBytes, 0, dataChunkSizeBytes.Length);
                for (int i = 0; i < dataChunkSizeBytes.Length; i++)
                {
                    byteList.Add(dataChunkSizeBytes[i]);
                }
                formatDataArray = byteList.ToArray();
            }
            return formatDataArray;
        }

        public async Task HideFileDataLSB(byte[] formatData, FileStream originalSong, FileStream fileDataToHide, FileStream destinationFile)
        {
            await destinationFile.WriteAsync(formatData, 0, formatData.Length);

            int readLength_SampleData = -1;
            int readLength_HiddenFile = -1;

            while (readLength_HiddenFile != 0 || readLength_SampleData != 0)
            {
                byte[] sampleDataBuffer = new byte[2];
                byte[] hiddenFileBuffer = new byte[1];
                bool hasWrittenStop = false;

                readLength_HiddenFile = await fileDataToHide.ReadAsync(hiddenFileBuffer, 0, hiddenFileBuffer.Length);
                if (readLength_HiddenFile != 0)
                {
                    BitArray hiddenFileBits = new BitArray(hiddenFileBuffer);
                    for (int i = 0; i < hiddenFileBits.Count; i++)
                    {
                        // The choice to read two bytes here and then divide the buffer into individual bytes was done to ensure that the destination file will end on an even sample boundry.
                        // Additionally, this approach means that the position of the LSBs will be static due to the behavior of BitArrays representing 1 byte.
                        
                        //sampleDataBuffer = originalSong.ReadBytes(sampleDataBuffer.Length));
                        readLength_SampleData = await originalSong.ReadAsync(sampleDataBuffer, 0, sampleDataBuffer.Length);
                        byte[] sampleByteArray1 = new byte[1];
                        byte[] sampleByteArray2 = new byte[1];
                        Array.Copy(sampleDataBuffer, 0, sampleByteArray1, 0, 1);
                        Array.Copy(sampleDataBuffer, 1, sampleByteArray2, 0, 1);

                        BitArray sampleByte1 = new BitArray(sampleByteArray1);
                        BitArray sampleByte2 = new BitArray(sampleByteArray2);
                        sampleByte1.Set(7, xOrBits(hiddenFileBits.Get(i), sampleByte2.Get(7)));

                        await destinationFile.WriteAsync(convertToByteArray(sampleByte1), 0, 1);
                        await destinationFile.WriteAsync(convertToByteArray(sampleByte2), 0, 1);
                    }
                }
                else
                {
                    if (!hasWrittenStop)
                    {
                        await destinationFile.WriteAsync(STOP_BYTES, 0, STOP_BYTES.Length);
                        hasWrittenStop = true;
                        await originalSong.ReadAsync(new byte[STOP_BYTES.Length], 0, STOP_BYTES.Length);
                    }
                    else
                    {
                        byte[] remainingDataBuffer = new byte[256];
                        int remainingReadLength = -1;
                        while (remainingReadLength != 0)
                        {
                            remainingReadLength = await originalSong.ReadAsync(remainingDataBuffer, 0, remainingDataBuffer.Length);
                            await destinationFile.WriteAsync(remainingDataBuffer, 0, remainingReadLength);
                        }
                    }
                }
            }
        }

        public bool xOrBits(bool bit1, bool bit2)
        {
            return (bit1 && !bit2) || (!bit1 && bit2);
        }


        public byte[] convertToByteArray(BitArray bits)
        {
            if (bits.Count != 8)
            {
                throw new ArgumentException("Not 8 bits in BitArray");
            }
            byte[] byteValue = new byte[1];
            BitArray temp = new BitArray(bits.Cast<bool>().Reverse().ToArray());
            temp.CopyTo(byteValue, 0);
            return byteValue;
        }

        public async Task HideFileDataAsync(byte[] formatData, FileStream originalSong, FileStream fileDataToHide, FileStream destinationFile)
        {
            if (formatData != null)
            {
                //UInt32 sampleRate = BitConverter.ToUInt32(formatData, 24);
                //UInt16 sampleSize = BitConverter.ToUInt16(formatData, 32);

                byte[] sampleDataBuffer = new byte[1000];
                byte[] fileDataBuffer = new byte[10];
                int readLength_SampleData = -1;
                int readLength_HiddenFile = -1;             

                /*
                 * Start reading sample data and data from the file to be hidden into buffers.
                 * When both have been read, splice the file data into the sample data, and 
                 * then write to the destination. Repeat until there is no more file data,
                 * at which point the remaining song data will be directly copied.
                 */

                while (readLength_SampleData != 0 || readLength_HiddenFile != 0)
                {

                    if (readLength_SampleData != 0)
                    {
                        readLength_SampleData = await originalSong.ReadAsync(sampleDataBuffer, 0, sampleDataBuffer.Length);
                    }

                    if (readLength_HiddenFile != 0)
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
