using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FileCutter
{
    class clsApplicationContext : ApplicationContext
    {
        private clsApplicationContext()
        {
            byte[] arrFileContents;
            OpenFileDialog objDialog = new OpenFileDialog();
            string strFileName = "";
            objDialog.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            objDialog.DefaultExt = ".*";
            objDialog.Filter = "All files|*.*"; 
            DialogResult result = objDialog.ShowDialog();
            int intAllocatedLength;

            if (result == DialogResult.OK)
            {
                strFileName = objDialog.FileName;
                // read the file into the byte array
                FileStream objFileStream = new FileStream(strFileName, FileMode.Open, FileAccess.Read);
                try
                {
                    int intLength = (int)objFileStream.Length;
                    int intStartPosition = Convert.ToInt32 (InputBox.Show("Enter starting position. File Size " + Convert.ToString(intLength) + " bytes", "Start position", "0").Text);
                    int intEndPosition = Convert.ToInt32(InputBox.Show("Enter end position. File Size " + Convert.ToString(intLength) + " bytes", "End position", Convert.ToString(intLength - 1)).Text);
                    if (intStartPosition < 0)
                        intStartPosition = 0;
                    if (intEndPosition > intLength - 1)
                        intEndPosition = intLength - 1;
                    if (intStartPosition > intEndPosition)
                        intEndPosition = intStartPosition;
                    arrFileContents = new byte[0];
                    intAllocatedLength = AllocateMemory(ref arrFileContents, intEndPosition - intStartPosition + 1);
                    if (intAllocatedLength != intEndPosition - intStartPosition + 1)
                        MessageBox.Show("Cannot create a chunk this big because of memory restriction \nOnly " + Convert.ToString(intAllocatedLength) + " of " + Convert.ToString(intEndPosition - intStartPosition + 1) + " will be created");
                    int intCount;
                    int intOffset = 0;
                    objFileStream.Seek(intStartPosition,SeekOrigin.Begin);
                    while ((intCount = objFileStream.Read(arrFileContents, intOffset, intAllocatedLength - intOffset)) > 0)
                        intOffset += intCount;
                }
                finally
                {
                    objFileStream.Close();
                }
                strFileName = Path.GetDirectoryName(strFileName) + "\\FilePart.txt";
                objFileStream = new FileStream(strFileName, FileMode.Create, FileAccess.Write);
                objFileStream.Write(arrFileContents, 0, intAllocatedLength);
                objFileStream.Flush();
                objFileStream.Close();
            }
            Environment.Exit(0);
        }

        private int AllocateMemory(ref byte[] arrFileContents, int intLength)
        {
            bool blnDone = false;
            int intAllocatedLength;
            int intIterations = 0;

            intAllocatedLength = intLength;

            while (!blnDone && intIterations < 100)
            {
                try
                {
                    arrFileContents = new byte[intAllocatedLength];
                    blnDone = true;
                }
                catch
                {
                    intIterations++;
                    intAllocatedLength = intAllocatedLength / 2;
                }
            }

            return intAllocatedLength;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new clsApplicationContext());
        }

    }
}
