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
using Dolinay;

namespace ManageDevices
{
    public partial class Main : Form
    {
        /* 
           Pretty sure we can go ahead and remove this code,
           DriveDetector can handle this better.
        */

        //private const int WM_DEVICECHANGE = 0x219;
        //private const int DBT_DEVICEARRIVAL = 0x8000;
        //private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        //private const int DBT_DEVICETYP_VOLUME = 0x00000002;

        /*
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
        */

        /*
            DriveDetector is an open source class written to handle notifications
            about drive insertion/removal. It also provides infromation about the inserted
            drive. Good stuff!
            More info here: http://www.codeproject.com/Articles/18062/Detecting-USB-Drive-Removal-in-a-C-Program
        */
        private DriveDetector driveDetector = null;
        private String arrivalMessage = "";
        private String removalMessage = "";
        private String driveLetter = "";
        
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

            /* --THIS IS A TEST OF THE DriveDetector CLASS-- */
            driveDetector = new DriveDetector();
            driveDetector.DeviceArrived += new DriveDetectorEventHandler(
            OnDriveArrived);
            driveDetector.DeviceRemoved += new DriveDetectorEventHandler(
                OnDriveRemoved);
            driveDetector.QueryRemove += new DriveDetectorEventHandler(
                OnQueryRemove);
        }

        /* --THIS IS A TEST OF THE DriveDetector CLASS-- */

        // Called by DriveDetector when removable device in inserted
        private void OnDriveArrived(object sender, DriveDetectorEventArgs e)
        {
            // e.Drive is the drive letter, e.g. "E:\\"
            // If you want to be notified when drive is being removed (and be
            // able to cancel it),
            // set HookQueryRemove to true
            e.HookQueryRemove = false;

            arrivalMessage = "Device " + e.Drive + " Connected";
            driveLetter = e.Drive;
            //Clear the listbox of previous message
            if (listBox1.Items.Contains(removalMessage))
            {
                listBox1.Items.Remove(removalMessage);
            }
            listBox1.Items.Add(arrivalMessage);
            addToTree(driveLetter);
        }

        // Called by DriveDetector after removable device has been unplugged
        private void OnDriveRemoved(object sender, DriveDetectorEventArgs e)
        {
            // TODO: do clean up here, etc. Letter of the removed drive is in
            // e.Drive;
            RemoveFromTree(driveLetter);
            treeView1.Update();
            removalMessage = "Device " + e.Drive + " Removed";
            driveLetter = "";
            listBox1.Items.Clear();
            listBox1.Items.Add(removalMessage);                        
            //listBox1.Update();
        }

        // Called by DriveDetector when removable drive is about to be removed
        private void OnQueryRemove(object sender, DriveDetectorEventArgs e)
        {
            // Should we allow the drive to be unplugged?
            if (MessageBox.Show("Allow remove?", "Query remove",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                    DialogResult.Yes)
                e.Cancel = false;        // Allow removal
            else
                e.Cancel = true;         // Cancel the removal of the device
        }

        /* --THIS IS A TEST OF THE DriveDetector CLASS-- */

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
            //Form2 frm = new ManageDevices.Form2(this);
            //frm.ShowDialog();
            String filename = "files.txt";
            String caption, message;
            DialogResult result;
            // Checks if flash drive is in
            if (Directory.Exists(driveLetter))
            {
                caption = "Add File";
                message = "File(s) don't match. \nWould you like to create a new file(s)?";
                result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo);
            }
            // If no flash drive then error message is given so drive can be inserted first before adding
            else
            {
                caption = "Error";
                message = "Error! No Flash Drive found. \nFlash Drive required before trying to add files to it.";
                result = MessageBox.Show(message, caption, MessageBoxButtons.OK);
            }
            
            if (result == DialogResult.Yes)
            {
                foreach (FileInfo fi in (List<FileInfo>)delSelected)
                {
                    //Writes files to be added to a text file that can be read from later.
                    using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename), true))
                    {
                        sw.WriteLine(fi.ToString());
                        //Check if file is already present in flash drive, if not then adds it to flash drive
                        if (searchFlashDrive(fi) == false) 
                        {
                            File.Copy(Path.Combine(fi.Directory.FullName, fi.ToString()), Path.Combine(driveLetter, fi.ToString()), true);
                        }
                    }
                }
            }
            listBox1.Update();
        }
        
        // Search Flash Drive for a File
        private Boolean searchFlashDrive(FileInfo fi)
        {
            DirectoryInfo dri = new DirectoryInfo(driveLetter);
            foreach (FileInfo f in dri.GetFiles())
            {
                if (f == fi)
                {
                    return true;
                }
            }
            return false;
        }

        // Delete button
        private void button3_Click(object sender, EventArgs e)
        {
            //Form4 del = new ManageDevices.Form4(this); // Added this to form4
            //del.Show();
            String caption = "Delete File";
            String message = "Are you sure you want to delete this file(s)?";
            DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                foreach(FileInfo fi in (List<FileInfo>)delSelected)
                {
                    fi.Delete();
                }
                for (int x = listBox1.SelectedIndices.Count - 1; x >= 0; x--)
                {
                    int idx = listBox1.SelectedIndices[x];
                    listBox1.Items.RemoveAt(idx);
                }
                listBox1.Update();
            }                                    
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

        /// <summary>
        /// This method should remove a node 's' from the treeview.
        /// </summary>
        /// <param name="s"></param>
        private void RemoveFromTree(string s)
        {
            foreach(TreeNode t in treeView1.Nodes)
            {
                if (t.Text == s){
                    treeView1.Nodes.Remove(t);
                }
            }
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
