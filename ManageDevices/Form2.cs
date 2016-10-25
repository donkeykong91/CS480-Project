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
    public partial class Form2 : Form
    {
        Main form1;
        public Form2(Main form_1)
        {
            InitializeComponent();
            form1 = form_1;
        }

        // For the 'No' button
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //For the 'Yes' button
        private void button1_Click(object sender, EventArgs e)
        {
            DirectoryInfo dir = (DirectoryInfo)form1.treeView1.SelectedNode.Tag;
            File.WriteAllText(Path.Combine(dir.FullName, "test1234.txt"), "Testing");
            this.Close();
        }
    }
}
