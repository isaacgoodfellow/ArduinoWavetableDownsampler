using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Downsampler
{
    class Utility
    {

        /// <summary>
        /// Export for use as a wavetable array in c
        /// </summary>
        /// <param name="filename">The completely pathed filename of the file to read</param>
        public static String TableFromWav(string filename)
        {

            ArrayList samples = new ArrayList();

            String ret = "int [] table = {";
            String suff = "};";
            
            WAVFile file = new WAVFile();

            String retval = file.Open(filename, WAVFile.WAVFileMode.READ);
            if (retval != "")
                throw new WAVFileException(retval, "WAVFile.Convert_Copy()");

            double dur = (double)file.NumSamples / file.SampleRateHz;
            if (dur > 0.5)
            {
                MessageBox.Show("Wav File too long, please use .wavs less than 1 second long.", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "ERROR";
            }

            int lastZero = -1;
            bool hasNonZero = false;
            bool runOfZeros = false;

            while (file.NumSamplesRemaining > 0)
            {
                short sample = file.GetNextSampleAs8Bit();
                if (sample != 0)
                {
                    hasNonZero = true;
                    runOfZeros = false;
                }
                else if( sample == 0 && !runOfZeros )
                {
                    lastZero = ret.Length;
                    runOfZeros = true;
                }

                if (hasNonZero)
                {
                    ret += sample.ToString();
                    ret += ",";
                }
            }

            file.Close();
            
            //Remove trailing zeros
            ret = ret.Remove(lastZero, ret.Length - lastZero);
            //Remove trailing comma
            if (ret != null)
                ret = ret.TrimEnd(',');

            return ret += suff;

        }

        /// <summary>
        /// Export for use as a wavetable header in Mozzi
        /// </summary>
        /// <param name="filename">The completely pathed filename of the file to read</param>
        public static String SaveForMozzi(string filename)
        {
            return "Mozzi Export Waiting";
        }

    }
}
