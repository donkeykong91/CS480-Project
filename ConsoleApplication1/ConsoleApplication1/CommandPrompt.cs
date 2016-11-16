using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHandlerTest
{
    class CommandPrompt
    {
        private Boolean flagSet;    //To control program loop.    
        private String savePathDirectory;        

        public CommandPrompt()
        {
            flagSet = true;
            savePathDirectory = @"C:\Users\Public\TestFolder\pathList.txt";
        }

        public void Run()
        {            
            Console.WriteLine("--File Handler Test 1.0--");
            Console.WriteLine("-------------------------");
            while (flagSet)
            {
                Console.Write("$ ");
                string input = Console.ReadLine();
                if (!ParseInput(input))
                {
                    Console.WriteLine("Invalid command.");
                }
            }
        }        

        /// <summary>
        /// This method will print out a list of directories.
        /// </summary>
        private void DisplayDirs()
        {
            System.IO.DriveInfo defaultDrive = new System.IO.DriveInfo(@"C:\");
            System.IO.DirectoryInfo dirInfo = defaultDrive.RootDirectory;
            System.IO.DirectoryInfo[] dirInfos = dirInfo.GetDirectories("*.*");

            foreach (System.IO.DirectoryInfo di in dirInfos)
            {
                Console.WriteLine("\\" + di.Name);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        private void DisplayDirs(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                DirectoryInfo[] dirs = dir.GetDirectories("*.*");
                foreach(DirectoryInfo di in dirs)
                {
                    Console.WriteLine("\\" + di.Name);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Prints a list of drives.
        /// </summary>
        private void ListDrives()
        {
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo di in drives)
            {
                Console.WriteLine(di.Name);
            }
        }

        /// <summary>
        /// Search uses the recursive Descend(string, string, List<>) method
        /// to recursvely search a given path for a given file.
        /// </summary>
        /// <param name="path"></param> Name of path to search on.
        /// <param name="file"></param> Name of file to search for.
        /// <returns>
        /// Returns a FileInfo object representing the file searched for.
        /// If file can't be found, returns null.
        /// </returns>
        private FileInfo Search(string path, string file)
        {
            List<FileInfo> list = new List<FileInfo>();
            Descend(path, file, list);
            if (list.Count > 0)
                return list[0];
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="file"></param>
        /// <param name="list"></param>
        private void Descend(string dir, String file, List<FileInfo> list)
        {
            try
            {

                //First search files of current directory.
                foreach (string f in Directory.GetFiles(dir, file))
                {
                    list.Add(new FileInfo(f));
                    if (list.Count > 0)
                        return;
                }

                //Then search subdirectories.
                foreach (string d in Directory.GetDirectories(dir))
                {                    
                    Descend(d, file, list);
                }
                
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);                                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private Boolean ParseInput(string input)
        {
            //Tokenizes input command.
            String[] tokens = input.Split(' ');

            //Determines whether input is a valid command.
            Boolean value = true;
            switch (tokens[0].ToLower())
            {
                case "":
                    break;
                case "dir":
                    if(tokens.Length == 1)
                    {
                        DisplayDirs();
                    }
                    if(tokens.Length == 2)
                    {
                        DisplayDirs(tokens[1]);
                    }
                    else
                    {
                        value = false;
                    }                                       
                    break;
                case "quit":
                    if(tokens.Length == 1)
                    {
                        flagSet = false;
                    }
                    else
                    {
                        value = false;
                    }                  
                    break;
                case "cls":
                    if(tokens.Length == 1)
                    {
                        Console.Clear();
                    }
                    else
                    {
                        value = false;
                    }                  
                    break;
                case "lsdrives":
                    if(tokens.Length == 1)
                    {
                        ListDrives();
                    }
                    else
                    {
                        value = false;
                    }
                    break;
                case "getpaths":
                    FileInfo[] paths = ImportPathList();
                    if(paths != null && paths.Length > 0)
                    {
                        foreach (FileInfo d in paths)
                        {
                            Console.WriteLine(d);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No paths listed");
                    }                    
                    break;
                case "search":
                    if(tokens.Length == 3)
                    {                        
                        try
                        {      
                            //Get the filename from second argument.                      
                            String fileName = tokens[2];

                            //Search for the file in the path given by first argument.
                            FileInfo targetFile = Search(tokens[1], fileName);
                            if(targetFile != null)
                            {
                                Console.WriteLine("Found " + 
                                                    targetFile.Name + " at " + 
                                                    targetFile.DirectoryName);
                            }
                            else
                            {
                                Console.WriteLine("File does not exist at " + tokens[1]);
                            }
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.StackTrace);
                            Console.WriteLine(e.Message);
                        }
                    }
                    else
                    {
                        value = false;
                    }
                    break;
                default:
                    value = false;
                    break;
            }

            return value;   
        }
        

        /// <summary>
        /// This method will export a list of selected paths
        /// (to be passed as a array of Strings) to a file
        /// called pathList.txt. To be used by the program to keep track 
        /// of what paths the user wants to sync.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public String ExportPathList(String[] paths)
        {
            try
            {
                using(StreamWriter writer = File.AppendText(savePathDirectory))
                {
                    foreach (String s in paths)
                    {
                        writer.WriteLine(s);
                    }
                }                                       

                return "List saved.";
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// This method will import the list of paths to keep 
        /// updated from the default saved file containing the paths
        /// the user wants to keep synced. Will return Null if list is empty.
        /// </summary>
        /// <returns></returns>
        public FileInfo[] ImportPathList()
        {
            List<FileInfo> pathsToSync = new List<FileInfo>();
            StreamReader sr = null;
            try
            {  
                //Check that the default list of paths exists.                              
                if(File.Exists(savePathDirectory) == true)
                {
                    sr = new StreamReader(savePathDirectory);
                    while (sr.Peek() != -1)
                    {
                        try
                        {
                            FileInfo fi = new FileInfo(sr.ReadLine().Trim());
                            if(fi.Exists == true)
                            {
                                pathsToSync.Add(fi);
                            }
                            else
                            {
                                Console.WriteLine("Not a file.");
                            }                            
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Not a file.");
                        }
                    }

                    //Get back list of files as a FileInfo[]
                    return pathsToSync.ToArray();
                }
                else
                {
                    //Create the default file, if it does not exist.
                    File.Create(savePathDirectory);                    
                    return null;
                }                                
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }            
        }
    }    
}
