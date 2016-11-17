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
    public partial class Main : Form
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
        
        public Main()
        {
            InitializeComponent();
            /*
            try
            {
                
                DriveInfo[] drive = DriveInfo.GetDrives();
                TreeNode root;
                foreach (DriveInfo dr in drive)
                {
                    if(dr.Name == "users")
                    {
                        root = new TreeNode(dr.Name);
                        root.Tag = 
                        treeView1.Nodes.Add(root);
                    }
                    Desktop.Items.Add(dr.Name);
                    DriveInfo d = new DriveInfo(dr.Name);
                    DirectoryInfo
                    dirInfo = d.RootDirectory;
                    DirectoryInfo[] dir = dirInfo.GetDirectories(".");
                    foreach (DirectoryInfo di in dir)
                    {
                        Desktop.Items.Add(di.Name);
                    }

                }
          
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            */
        }

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs; 
           
            foreach (DirectoryInfo subDir in subDirs)
                {
                try
                {
                    aNode = new TreeNode(subDir.Name, 0, 0);
                    aNode.Tag = subDir;
                    subSubDirs = subDir.GetDirectories();
                    
                    if (subSubDirs.Length != 0)
                    {
                        GetDirectories(subSubDirs, aNode);
                    }
                    nodeToAddTo.Nodes.Add(aNode);
                }
                catch (Exception e) { }
            }
            
        }

        // Add button
        private void button2_Click(object sender, EventArgs e)
        {
            Form2 frm = new ManageDevices.Form2(this);
            frm.Show();
        }

        // Delete button
        private void button3_Click(object sender, EventArgs e)
        {
            Form4 del = new ManageDevices.Form4(this); // Added this to form4
            del.Show();
            listBox1.Update();
            

        }


        // Load all the important folders to the treeView list
        private void Form1_Load(object sender, EventArgs e)
        {   
            try
            {
                Directory.SetCurrentDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                addToTree(Directory.GetCurrentDirectory());
                Directory.SetCurrentDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                addToTree(Directory.GetCurrentDirectory());
                Directory.SetCurrentDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
                addToTree(Directory.GetCurrentDirectory());
                Directory.SetCurrentDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
                addToTree(Directory.GetCurrentDirectory());
                Directory.SetCurrentDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
                addToTree(Directory.GetCurrentDirectory());

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        private void addToTree(string s)
        {
            try
            {
                TreeNode root;
                DirectoryInfo dri = new DirectoryInfo(s);


                root = new TreeNode(dri.Name);
                root.Tag = dri;
                GetDirectories(dri.GetDirectories(), root);
                treeView1.Nodes.Add(root);
            }   catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        // To get the files from within a folder and display in the listbox
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            listBox1.Items.Clear();
            DirectoryInfo dir = (DirectoryInfo)treeView1.SelectedNode.Tag;
            FileInfo[] fInfo = dir.GetFiles();
            foreach(FileInfo file in fInfo)
            {
                  
                listBox1.Items.Add(file);
             }
           

        }

        //method to pass in files that are selected
        public object delSelected
        {
            get
            {
                List<FileInfo> list = new List<FileInfo>();
                list = listBox1.SelectedItems.Cast<FileInfo>().ToList();
                return list; 

            }
                   
        }

        public string selected
        {
            get { return listBox1.SelectedItem.ToString(); }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
