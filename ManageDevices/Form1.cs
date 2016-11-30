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
using System.Threading;

namespace ManageDevices
{
    public partial class Main : Form
    {

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
        private String filename = "files.txt";
        public Main()
        {
            InitializeComponent();
            button1.Hide();
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

        private string syncFile(string file1, string file2)
        {
            File.Copy(file1, file2, true);
            return "Finished sync.";
        }

        //searches for the fileName in each directory. If not found, returns empty string.
        private string searchFileName(string fileName)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "//" + fileName;
            string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//" + fileName;
            string myPicturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "//" + fileName;
            string myVideosPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "//" + fileName;
            string myMusicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "//" + fileName;

            if ((new FileInfo(desktopPath)).Exists)
            {
                return desktopPath;
            }
            else if ((new FileInfo(myDocumentsPath)).Exists)
            {
                return myDocumentsPath;
            }
            else if ((new FileInfo(myPicturesPath)).Exists)
            {
                return myPicturesPath;
            }
            else if ((new FileInfo(myVideosPath)).Exists)
            {
                return myVideosPath;
            }
            else if ((new FileInfo(myMusicPath)).Exists)
            {
                return myMusicPath;
            }

            return "";
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

        /**
         *  Method: GetDirectories
         *  Purpose: This method adds the subdirectories under their Parent Directory 
         *  in the tree Node and keeps calling on this method until there 
         *  are no more directories to add
         **/
        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;

            // Gets each folder in the parent folder (subDir)
            foreach (DirectoryInfo subDir in subDirs)
            {
                try
                {
                    // Sets up a node as subDir
                    aNode = new TreeNode(subDir.Name, 0, 0);
                    // Stores information about folder in the Tag
                    aNode.Tag = subDir;
                    // gets other folders within the subDir
                    subSubDirs = subDir.GetDirectories();
                    // If there are folders within subDir, then call on this method again
                    if (subSubDirs.Length != 0)
                    {
                        GetDirectories(subSubDirs, aNode);
                    }
                    //Adds node to the tree
                    nodeToAddTo.Nodes.Add(aNode);
                }
                catch (Exception e) { }
            }

        }

        private Boolean isFlashDriveInserted()
        {
            return Directory.Exists(driveLetter);
        }

        // Add button
        private void button2_Click(object sender, EventArgs e)
        {
            List<FileInfo> fl = (List<FileInfo>)selected;
            // If no flash drive then error message is given so drive can be inserted first before adding
            if (!isFlashDriveInserted())
            {
                String caption = "Error";
                String message = "Error! No Flash Drive found. \nFlash Drive required before trying to add files to it.";
                DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.OK);
            }
            // Else calls on form2 to add Files
            else
            {
                Form2 frm = new ManageDevices.Form2(this);
                frm.ShowDialog();
            }
        }

        // Search Flash Drive for a File
        public Boolean searchFlashDrive(FileInfo fi)
        {
            DirectoryInfo dri = new DirectoryInfo(driveLetter);
            foreach (FileInfo f in dri.GetFiles())
            {
                if (f.Name == fi.Name)
                {
                    return true;
                }
            }
            return false;
        }

        // Delete button
        private void button3_Click(object sender, EventArgs e)
        {
            List<FileInfo> fl = (List<FileInfo>)selected;
            // If not files were selected, then show user error
            if (!fl.Any())
            {
                String caption = "Error";
                String message = "No file was selected";
                DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.OK);
            }
            else
            {
                // passed 'this' to Form4 constructor to access Main
                Form4 del = new ManageDevices.Form4(this);
                // Calls on method to show the Delete File form    
                del.Show();
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
            foreach (TreeNode t in treeView1.Nodes)
            {
                if (t.Text == s)
                {
                    treeView1.Nodes.Remove(t);
                }
            }
        }

        // This method adds the main folders to the Tree as roots
        private void addToTree(string s)
        {
            try
            {
                TreeNode root;

                DirectoryInfo dri = new DirectoryInfo(s);

                // 'root' holds the name of the Directory
                root = new TreeNode(dri.Name);
                // the Tag holds the information about the folder
                root.Tag = dri;
                // Adds all of the root's subdirectories to the Tree 
                GetDirectories(dri.GetDirectories(), root);
                treeView1.Nodes.Add(root);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        // To get the files from within a folder and display in the listbox
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //Disable dangerous buttons when directory is selected.
            button3.Enabled = false;
            button2.Enabled = false;
            button4.Enabled = false;

            listBox1.Items.Clear();
            DirectoryInfo dir = (DirectoryInfo)treeView1.SelectedNode.Tag;
            FileInfo[] fInfo = dir.GetFiles();
            foreach (FileInfo file in fInfo)
            {
                listBox1.Items.Add(file);
            }


        }

        // returns ListBox
        public ListBox getList { get { return listBox1; } }

        // returns DriveLetter
        public String getDriveLetter { get { return driveLetter; } }

        //Returns the selected file in the listbox,
        //make sure to cast to a FileInfo
        public object selectedFile
        {
            get { return listBox1.SelectedItem; }
        }

        //method to pass files that are selected
        public object selected
        {
            get
            {
                List<FileInfo> list = new List<FileInfo>();
                list = listBox1.SelectedItems.Cast<FileInfo>().ToList();
                return list;
            }

        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Re-enable funcitonal buttons for file management.
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;

            //Clear selected node in the tree view 
            //(so _AfterSelect event can trigger again)
            treeView1.SelectedNode = null;
        }

        //Sync button
        private void button4_Click(object sender, EventArgs e)
        {
            if (getDriveLetter.Length == 0)
            {
                MessageBox.Show("No external drive detected.\n"
                                + "Please insert an external drive.");
                return;
            }
            //button4.Hide();
            // Compare the two files that referenced in the textbox controls.
            FileInfo baseFile = (FileInfo)selectedFile;
            //Make sure the user has selected a file.
            if (baseFile == null)
            {
                MessageBox.Show("Please select a file to sync.");
                return;
            }
            //Make sure the user hasn't selected files inside the external drive.
            //if(baseFile.Directory.Name == getDriveLetter)
            //{

            //}
            string fileName = baseFile.Name;
            string file1 = driveLetter + fileName;
            //string file2 = searchFileName(baseFile.Name);
            string file2 = baseFile.FullName;            

            if (!searchFlashDrive(baseFile))
            {
                MessageBox.Show(fileName + " does not exist.\n"
                                 + "Adding file to usb.");
                //If the file does not exist in the flashdrive
                //add it to the flashdrive.
                File.Copy(baseFile.FullName, driveLetter + baseFile.Name);
            }
            else
            {
                if (FileCompare(file1, file2))
                {
                    MessageBox.Show(fileName + " is synced.");
                }
                else
                {
                    MessageBox.Show(syncFile(file1, file2));
                }
            }
        }

        //Returns to see if the Auto-Update check mark is selected or not
        public bool OptionSelected
        {
            get { return checkBox1.Checked; }
        }

        //Once Auto-Update is checked yes or no it does one of the following
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //If selected to auto-update
            //It shows its balloon tip and turns the icon blue
            if (OptionSelected)
            {
                this.Hide();
                trayIcon.ShowBalloonTip(5);
                trayIcon.Icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "blue_icon.ico"));
                autoUpdate.RunWorkerAsync();
            }
            //If auto-update is deselected
            //Turns icon red and gives balloon tool tip
            if (!OptionSelected)
            {
                trayIcon.Icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "red_icon.ico"));
                trayIcon.ShowBalloonTip(5, "Auto-Update disabled", "Files will no longer be automatically updated", new ToolTipIcon());
            }
        }

        //Shows main menu once icon is double clicked
        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        //Does the auto-update work here
        //uses a while loop to see that while auto-update is selected it will
        //run this routine over until deselected with 30 second sleep intervals in between
        private void autoUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            while (OptionSelected)
            {
                //Reads in text file
                System.IO.StreamReader file = new System.IO.StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename));
                string line;
                string file1;
                string file2;
                //Bool used in case files aren't found or flash drive isn't in
                //if neither of those issues occur it'll give the balloon tool tip 
                //that lets the user know everything has been synced
                bool fileNotFound = false;
                while ((line = file.ReadLine()) != null)
                {
                    file1 = searchFileName(line);
                    file2 = getDriveLetter + line;
                    //If the file is found and the flash drive is inserted the files are synced
                    if (file1 != "" && isFlashDriveInserted())
                    {
                        syncFile(file1, file2);
                    }
                    //If no flash drive is found then gives error message and marks the boolean as true
                    else if (!isFlashDriveInserted())
                    {
                        fileNotFound = true;
                        trayIcon.ShowBalloonTip(5, "Auto-Update Error", "No flash drive inserted", new ToolTipIcon());
                    }
                    //final else case is incase the file isn't found in the computer it'll give the error message 
                    //letting the user know which file wasn't found
                    else
                    {
                        fileNotFound = true;
                        trayIcon.ShowBalloonTip(5, "Auto-Update Error", "File(s) " + line + " not found. ", new ToolTipIcon());
                    }
                }
                //if no errors then returns quick update letting user know files are synced
                if (!fileNotFound)
                {
                    trayIcon.ShowBalloonTip(5, "Auto-Update", "Files synced and up to date", new ToolTipIcon());
                }
                file.Close();
                Thread.Sleep(30000);
            }
        }
    }
}
