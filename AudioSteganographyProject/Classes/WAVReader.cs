using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioSteganographyProject.Properties
{
    class WAVReader
    {
        public WAVReader()
        {
            // Constructor is left empty incase the class ever needs to be instanciated for method calls.
        }

        public static bool HideFile(string songPath, string fileToHidePath, string dirPath)
        {
            byte[] formatData = getFileFormatData(songPath);
            bool didWriteFile = true;

            if (formatData != null)
            {
                // Byte 24 is the first of the four sample rate bytes
                UInt32 sampleRate = BitConverter.ToUInt32(formatData, 24);
                // Byte 32 is the first fo the two bytes for sample size
                UInt16 sampleSize = BitConverter.ToUInt16(formatData, 32);

                string hiddenExtension = Path.GetExtension(fileToHidePath);
                byte[] hiddenExtensionBytes = Encoding.ASCII.GetBytes(hiddenExtension);
                string newFilePath = Path.Combine(dirPath, Path.GetFileName(fileToHidePath));

                try
                {
                    using (FileStream outputFile = new FileStream(newFilePath, FileMode.Create, FileAccess.Write))
                    {
                        using (FileStream songData = new FileStream(songPath, FileMode.Open, FileAccess.Read))
                        {
                            using (FileStream hiddenFile = new FileStream(fileToHidePath, FileMode.Open, FileAccess.Read))
                            {
                                int numBytesRead = 0;
                                byte[] originalData = new byte[formatData.Length];

                                while (numBytesRead < formatData.Length)
                                {
                                    int readLength = songData.Read(originalData, numBytesRead, formatData.Length);

                                    if (readLength == 0)
                                    {
                                        break;
                                    }

                                    numBytesRead += readLength;
                                }


                                int i = 0;
                                int iterationsToWait = 220; // 220 iterations is used here because it means that 5% of song data will be changed, as modern sample rates are 88200.
                                while (songData.CanRead)
                                {
                                    originalData = new byte[sampleSize * 20];
                                    int readLength = songData.Read(originalData, 0, originalData.Length);
                                    i++;
                                    i %= iterationsToWait;
                                    if (i == 0 && hiddenFile.CanRead)
                                    {
                                        byte[] newData = new byte[sampleSize];
                                        for (int j = 0; j < newData.Length; j++)
                                        {
                                            originalData[j] = newData[j];
                                        }
                                        outputFile.Write(originalData, 0, originalData.Length);
                                    }
                                    else
                                    {
                                        outputFile.Write(originalData, 0, originalData.Length);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine(e);
                    didWriteFile = false;
                }

            }
            else
            {
                didWriteFile = false;
            }

            return didWriteFile;
        }

        public static byte[] getFileFormatData(string filePath)
        {
            /*
             * We want to read the first 33 bytes of the file to get the format of the song data.
             * The data we will want is the sample rate and the block alignment. The sample rate
             * will allow us to determine how often we can replace the sample data and the block
             * alignment will tell us how many bytes each sample is. 
             * 
             * Sample Rate is bytes 24 to 27
             * Block Alignment is bytes 32 and 33.
             * 
             * http://soundfile.sapp.org/doc/WaveFormat/ for picture.
             */

            byte[] fileFormatData = null;
            try
            {
                using (FileStream wavSource = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    int formatDataLength = 35;
                    int numBytesRead = 0;
                    fileFormatData = new byte[formatDataLength];

                    while (numBytesRead < formatDataLength)
                    {
                        int readLength = wavSource.Read(fileFormatData, numBytesRead, formatDataLength);

                        if (readLength == 0)
                        {
                            break;
                        }

                        numBytesRead += readLength;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return fileFormatData;
        }
    }
}
