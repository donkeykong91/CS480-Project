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
    public partial class Form1 : Form
    {
        private const int WM_DEVICECHANGE = 0x219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVICETYP_VOLUME = 0x00000002;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case WM_DEVICECHANGE:
                    switch ((int)m.WParam)
                    {
                        case DBT_DEVICEARRIVAL:
                            listBox1.Items.Add("Device Connected");
                            break;
                        case DBT_DEVICEREMOVECOMPLETE:
                            listBox1.Items.Add("Removed");
                            break;
                    }
                    break;
            }
        }

        public Form1()
        {
            InitializeComponent();
            try
            {
                
             /* //Commented the following out because it would pop up a device not ready message
                
                DriveInfo[] drive = DriveInfo.GetDrives();
                            
                foreach (DriveInfo dr in drive)
                {
                    Desktop.Items.Add(dr.Name);
                    DriveInfo d = new DriveInfo(dr.Name);
                    DirectoryInfo dirInfo = d.RootDirectory;
                    DirectoryInfo[] dir = dirInfo.GetDirectories(".");
                    foreach (DirectoryInfo di in dir) { Desktop.Items.Add(di.Name); }
                }
                */
                Desktop.Items.Add(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)));
                Desktop.Items.Add(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                Desktop.Items.Add(System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
                Desktop.Items.Add(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                Desktop.CheckOnClick = true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // Add button
        private void button2_Click(object sender, EventArgs e)
        {
            Form2 frm = new ManageDevices.Form2();
            frm.Show();
        }

        // Delete button
        private void button3_Click(object sender, EventArgs e)
        {
            Form4 del = new ManageDevices.Form4();
            del.Show();

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
