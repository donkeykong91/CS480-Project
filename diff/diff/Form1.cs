using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace diff
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // This method accepts two strings the represent two files to 
        // compare. A return value of 0 indicates that the contents of the files
        // are the same. A return value of any other value indicates that the 
        // files are not the same.
        private bool FileCompare(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            using (fs1 = new FileStream(file1, FileMode.Open))
            {
                using (fs2 = new FileStream(file2, FileMode.Open))
                {

                    // Check the file sizes. If they are not the same, the files 
                    // are not the same.
                    if (fs1.Length != fs2.Length)
                    {
                        // Return false to indicate files are different
                        return false;
                    }

                    // Read and compare a byte from each file until either a
                    // non-matching set of bytes is found or until the end of
                    // file1 is reached.
                    do
                    {
                        // Read one byte from each file.
                        file1byte = fs1.ReadByte();
                        file2byte = fs2.ReadByte();
                    }
                    while ((file1byte == file2byte) && (file1byte != -1));

                    // Return the success of the comparison. "file1byte" is 
                    // equal to "file2byte" at this point only if the files are 
                    // the same.
                    return ((file1byte - file2byte) == 0);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Compare the two files that referenced in the textbox controls.
            if (FileCompare(this.textBox1.Text, this.textBox2.Text))
            {
                MessageBox.Show("Files are equal.");
            }
            else
            {
                MessageBox.Show("Files are not equal.");
            }
        }
    }
}
