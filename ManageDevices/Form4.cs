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

namespace ManageDevices
{
    public partial class Form4 : Form
    {
        Main f1;        
        
        public Form4(Main f1)
        {
            this.f1 = f1;
            InitializeComponent();
        }

        // For the 'No' button
        private void button2_Click(object sender, EventArgs e)
        {            
            this.Close();
        }

        // For the 'Yes' button
        private void button1_Click(object sender, EventArgs e)
        {           

            List<FileInfo> fl = (List<FileInfo>)f1.selected;
            // Deletes each file that was selected from Computer
            foreach (FileInfo fi in fl) {    
                fi.Delete();
            }

            // Removes the files selected from ListBox
            for (int x = f1.getList.SelectedIndices.Count - 1; x >= 0; x--)
            {
                int idx = f1.getList.SelectedIndices[x];      
                f1.getList.Items.RemoveAt(idx);
            }

            // Updates listbox
            f1.getList.Update();                                
            this.Close();

        }


        private void Form4_Load(object sender, EventArgs e)
        {

        }
    }
}
