﻿using System;
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
            List<FileInfo> fl = (List<FileInfo>)f1.delSelected;

            foreach (FileInfo fi in fl) { 
           
                fi.Delete();
            }

            this.Close();
             
        }
    }
}
