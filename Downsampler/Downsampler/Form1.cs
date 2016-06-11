using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Downsampler
{
    public partial class Form1 : Form
    {

        #region Member Variables
        /// <summary>
        /// Stores the paths to the audio files (without trailing backslash).
        /// </summary>
        private ArrayList mSoundFiles;
        private bool mIsCopying = false;
        #endregion

        /// <summary>
        /// Add a filename to our list of files for processing
        /// </summary>
        /// <param name="filename">The completely pathed filename of the file to read</param>
        private void AddAudioFilename(String filename)
        {

            WAVFormat format = WAVFile.GetAudioFormat(filename);

            if (format.NumChannels > 1)
            {
                //TODO: Pop up something here
                return;
            }

            if (format.BitsPerSample != 8 && format.BitsPerSample != 16)
            {
                Console.Out.WriteLine(format.BitsPerSample);
                return;
            }

            if (!WAVFile.IsWaveFile(filename))
            {
                //TODO: Pop up a window to alert the user that an invalid file was specified
                return;
            }

            Downsampler.WavetableRef wavref = new WavetableRef();
            wavref.filename = Path.GetFileName(filename);
            wavref.filedir = Path.GetDirectoryName(filename);
            wavref.filepath = filename;
            mSoundFiles.Add(wavref);

            fileGridView.Rows.Add(1);

            int rowNum = fileGridView.Rows.Count - 1;
            fileGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            fileGridView.Rows[rowNum].Cells[0].Value = wavref.filename;
            fileGridView.Rows[rowNum].Cells[1].Value = "Copy (As Array)";
            fileGridView.Rows[rowNum].Cells[2].Value = "Remove";

        }

        public Form1()
        {
            InitializeComponent();
            mSoundFiles = new ArrayList();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            statusText.Text = "Ready";
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void addFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "WAV audio files|*.wav";
            op.Multiselect = true;

            if (op.ShowDialog(this) == DialogResult.OK)
            {
                foreach (string fn in op.FileNames)
                {
                    AddAudioFilename(fn);
                }
            }

        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileGridView.Rows.Clear();
            if (mSoundFiles != null)
            {
                mSoundFiles.Clear();
            }
        }

        /// <summary>
        /// Returns the fully-pathed filename for one of the audio files from a given index.  If the
        /// index is not valid, this will return a blank string.
        /// </summary>
        /// <param name="pIndex">The index of the filename to return</param>
        /// <returns>The fully-pathed audio filename, or a blank string if the given index is not valid.</returns>
        private String GetFullyPathedAudioFilename(int pIndex)
        {
            String filename = "";

            if (mSoundFiles.Count > pIndex && mSoundFiles[pIndex] != null)
            {
                filename = ((Downsampler.WavetableRef)mSoundFiles[pIndex]).filepath;
            }

            return filename;
        }

        /// <summary>
        /// This is an event function that is called when a user clicks on one of the "Remove" buttons
        /// in the filename grid.  This will remove the filename from the GUI and mSoundFilePaths.
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Contains information for the data grid click event</param>
        private void fileGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == 1)
            {
                string path = GetFullyPathedAudioFilename(e.RowIndex);
                if (mIsCopying != true)
                {


                        mIsCopying = true;
                        statusText.Text = "Copying to clipboard";
                        string table = Utility.TableFromWav(GetFullyPathedAudioFilename(e.RowIndex));
                        System.Windows.Forms.Clipboard.SetText(table);
                        mIsCopying = false;
                        statusText.Text = "Ready";

                }
            }

            // If the user clicked on the button (column 1), then remove the audio file from the list.
            if (e.ColumnIndex == 2)
            {
                try
                {
                    fileGridView.Rows.RemoveAt(e.RowIndex);
                    mSoundFiles.RemoveAt(e.RowIndex);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(this, exc.Message, "Error removing audio file", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Avant Bard 2016. Makes 8 bit wavetables for use with arduino / what have you.");
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog(this) == DialogResult.OK)
            {
                string dir = folder.SelectedPath;
                foreach (WavetableRef wref in mSoundFiles)
                {
                    string filename = Path.GetFileNameWithoutExtension(wref.filename);
                    System.IO.File.WriteAllText(dir + "\\" + filename.ToUpper()+".h", Utility.TableFromWav(wref.filepath));
                }
                Console.Out.WriteLine(dir);
            }
        }
    }
}
