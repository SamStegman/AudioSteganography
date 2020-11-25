using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace AudioSteganographyProject.UI.Views
{
    /// <summary>
    /// Interaction logic for ReadHiddenInfoPage.xaml
    /// </summary>
    public partial class ReadHiddenInfoPage : Page
    {
        public ReadHiddenInfoPage()
        {
            InitializeComponent();
        }

        private async void onCreateButton_Click(object sender, RoutedEventArgs e)
        {
            string songPath = this.SongFile.FilePathString;
            string dirPath = this.HiddenFile.FilePathString;

            Console.WriteLine(songPath);

            if (File.Exists(@songPath) && File.Exists(@dirPath))
            {
                try
                {

                    //FileStream destinationStream = File.Open(newFilePath, FileMode.Create);
                    //FileStream originalSongStream = File.Open(@songPath, FileMode.Open, FileAccess.Read);
                    //FileStream hiddenFileStream = File.Open(@fileToHidePath, FileMode.Open, FileAccess.Read);


                    //using (BinaryReader binReader_Song = new BinaryReader(originalSongStream))
                    using (FileStream originalSongStream = File.Open(@songPath, FileMode.Open, FileAccess.Read))
                    {
                        //using (BinaryReader binReader_HideFile = new BinaryReader(hiddenFileStream))
                        using (FileStream hiddenFileStream = File.Open(@dirPath, FileMode.Open, FileAccess.Read))
                        {
                            Dictionary<String, byte[]> formatData = new Dictionary<string, byte[]>();
                            Dictionary<String, int> info = GetFormatData(originalSongStream, out formatData);
                            //byte[] formatData = getFormatData(binReader_Song);

                            if (formatData["head"].Length >= 8)
                            {
                                byte[] dataByteCheck = new byte[4];
                                Array.Copy(formatData["head"], formatData["head"].Length - 8, dataByteCheck, 0, 4);
                                uint dataSubchunkBits = BitConverter.ToUInt32(formatData["head"], formatData["head"].Length - 8);
                                FileInfo fileInfo = new FileInfo(dirPath);

                                // fileInfo.Length will always be positive and dataSubchunkBits will simply be expanded so no precision will be lost here.
                                // We divide by 16 because each LSB XOR will require 2 bytes. 
                                // 8 bytes is added to the length of the file to hide so the length of the stop message can be written.
                                if (Convert.ToUInt64(dataSubchunkBits) / 16 > Convert.ToUInt64(fileInfo.Length) + 8)
                                {
                                    //await destinationStream.WriteAsync(formatData, 0, formatData.Length);
                                    BitArray sav = getData(formatData, hiddenFileStream, info);
                                    byte[] bytes = new byte[sav.Length / 8 + (sav.Length % 8 == 0 ? 0 : 1)];                                    sav.CopyTo(bytes, 0);
                                    hiddenFileStream.Close();
                                    Console.WriteLine(bytes);
                                    bytes = ReverseThemBitties(bytes);
                                    File.WriteAllBytes(dirPath, bytes);
                                    //await HideFileDataLSB(formatData, originalSongStream, hiddenFileStream, destinationStream);
                                    //await HideFileDataAsync(formatData, originalSongStream, hiddenFileStream, destinationStream);
                                    for (int i = 0; i < formatData["head"].Length; i++)
                                    {
                                        Console.WriteLine(i + " " + formatData["head"][i] + " " + Encoding.ASCII.GetString(formatData["head"], i, 1));
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

        public static byte ReverseBitsWithLoop(byte v)
        {
            byte r = v; // r will be reversed bits of v; first get LSB of v
            int s = 7; // extra shift needed at end
            for (v >>= 1; v != 0; v >>= 1)
            {
                r <<= 1;
                r |= (byte)(v & 1);
                s--;
            }
            r <<= s; // shift when v's highest bits are zero
            return r;
        }

        public static byte[] ReverseThemBitties(byte[] v)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = ReverseBitsWithLoop(v[i]);
            }
            return v;
        }

            private  BitArray getData(Dictionary<string, byte[]> formatData, FileStream hiddenFileStream, Dictionary<String, int> info)
        {
            int bytestoskip = info["bitDepth"] / 8;
            byte[] data = formatData["data"];
            BitArray ans = new BitArray(data.Length/bytestoskip);
            int bitnum = 0;
            for(int bytenum = 0; bytenum < data.Length; bytenum += bytestoskip)
            {
                byte theByte = data[bytenum];
                BitArray bits = new BitArray(new byte[] { theByte });
                if (theByte == 0)
                {
                    ans[bitnum] = false;
                }
                else
                {
                    ans[bitnum] = bits[0];
                }
                bitnum++;
            }
            return ans;
        }

        public int readbytes(List<byte> b, int num, BinaryReader r)
        {
            byte[] hold = new byte[num];
            for (int i = 0; i < num; i++)
            {
                byte temp = r.ReadByte();
                b.Add(temp);
                hold[i] = temp;
            }
            if (num == 4)
            {
                return BitConverter.ToInt32(hold, 0);
            }
            else
            {
                return BitConverter.ToInt16(hold, 0);
            }

        }

        public Dictionary<String, int> GetFormatData(FileStream originalSong, out Dictionary<string, byte[]> raw)
        {
            raw = new Dictionary<string, byte[]>();
            BinaryReader reader = new BinaryReader(originalSong);
            Dictionary<String, int> ans = new Dictionary<String, int>();
            List<byte> head = new List<byte>();
            // chunk 
            int chunkID = readbytes(head, 4, reader);
            int fileSize = readbytes(head, 4, reader);
            int riffType = readbytes(head, 4, reader);


            // chunk 1
            int fmtID = readbytes(head, 4, reader);
            int fmtSize = readbytes(head, 4, reader); // bytes for this chunk (expect 16 or 18)

            // 16 bytes coming...
            int fmtCode = readbytes(head, 2, reader);
            int channels = readbytes(head, 2, reader);
            int sampleRate = readbytes(head, 4, reader);
            int byteRate = readbytes(head, 4, reader);
            int fmtBlockAlign = readbytes(head, 2, reader);
            int bitDepth = readbytes(head, 2, reader);
            ans.Add("bitDepth", bitDepth);

            if (fmtSize == 18)
            {
                // Read any extra values
                int fmtExtraSize = readbytes(head, 2, reader);
                reader.ReadBytes(fmtExtraSize);
            }

            // chunk 2
            int dataID = readbytes(head, 4, reader);
            int bytes = readbytes(head, 4, reader);

            // DATA!
            byte[] data = reader.ReadBytes(bytes);


            raw.Add("head", head.ToArray());
            raw.Add("data", data);

            return ans;

        }

    }
}
