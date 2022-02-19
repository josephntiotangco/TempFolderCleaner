using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;
using TempFolderDuster.Classes;
using System.Threading;


namespace TempFolderDuster
{
    class Program
    {
        public static int deletedFiles;
        public static int notDeletedFiles;
        public static int leftCursor;
        public static int topCursor;
        static void Main(string[] args)
        {
            Console.WriteLine("Program Starting...");
            Console.WriteLine("Deleting Previous Logs...");
            leftCursor = Console.CursorLeft;
            topCursor = Console.CursorTop;
        //InitializeTransaction();
        //ListTempFiles(Globals.tempFolder);
        //Console.ReadLine();
        Again:

            Sequence();

            string loading = @"-\-";

            for (int i = 20; i >= 0; i--)
            {
                string spacing = i > 10 ? "  " : "   ";
                Console.SetCursorPosition(leftCursor, topCursor + 1);
                switch (loading)
                {
                    case @"-\-":
                        loading = "-|-";
                        break;
                    case "-|-":
                        loading = "-/-";
                        break;
                    case "-/-":
                        loading = "---";
                        break;
                    case "---":
                        loading = @"-\-";
                        break;
                }

                Console.Write("Press Enter to Exit, else process will continue in... {0}{1}{2}", i, spacing, loading);
                while (!(Console.KeyAvailable && Console.ReadKey(true).Key != ConsoleKey.Enter)) { goto MoveNext; }
            MoveNext:
                System.Threading.Thread.Sleep(1000);
                if (i == 0)
                {
                    goto Again;
                }
            }


            Environment.Exit(0);


        }

        private static void InitializeTransaction()
        {
            Console.WriteLine("Getting Temp Folder...");
            Console.WriteLine("Temp Folder Path = " + Globals.tempFolder);
            Console.WriteLine("Temp Folder Files Count = " + GetTempFilesCount(Globals.tempFolder).ToString());
            Console.WriteLine("Temp Folder Size = " + SummarizeSize(GetTempFilesSize(Globals.tempFolder)));
        }
        private static void Sequence()
        {
            Console.SetCursorPosition(leftCursor + 1, topCursor + 1);
            InitializeTransaction();
            DeleteAllFiles(Globals.tempFolder);
            leftCursor = Console.CursorLeft;
            topCursor = Console.CursorTop;
        }
        private static void DeleteLogFiles()
        {
            DirectoryInfo myDi = new DirectoryInfo(Globals.LogFile);
            if (Directory.Exists(Globals.LogFile))
            {
                DirectorySecurity dSecurity = myDi.GetAccessControl();
                dSecurity.AddAccessRule(new FileSystemAccessRule(Globals.AppUser, FileSystemRights.FullControl, AccessControlType.Allow));
                myDi.SetAccessControl(dSecurity);

                foreach (FileInfo file in myDi.EnumerateFiles())
                {
                    try
                    {
                        file.IsReadOnly = false;
                        file.Delete();
                    }
                    catch (Exception e)
                    {
                        Globals.lastError = e.Message;
                        Functions.LogErrorOrSuccess(Globals.lastError, "Delete of File Name" + file.Name + " Size :" + file.Length);
                        continue;
                    }

                }
            }
        }
        private static void ListTempFiles(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            foreach(FileInfo file in di.EnumerateFiles())
            {
                Console.WriteLine(file.Name);
            }
            foreach(DirectoryInfo folder in di.EnumerateDirectories())
            {
                Console.WriteLine(folder.Name);
            }
        }
        private static int GetTempFilesCount(string path)
        {
            int count = 0;
            DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.EnumerateFiles())
            {
                if (file.Exists)
                {
                    count++;
                }
            }
            foreach (DirectoryInfo folder in di.EnumerateDirectories())
            {
                if (folder.Exists)
                {
                    count++;
                }
            }

            return count;
        }
        private static long GetTempFilesSize(string path)
        {
            long size = 0;
            DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.EnumerateFiles())
            {
                if (file.Exists)
                {
                    size += file.Length;
                }
            }
            foreach (DirectoryInfo folder in di.EnumerateDirectories())
            {
                if (folder.Exists)
                {
                    foreach(FileInfo subfile in folder.EnumerateFiles())
                    {
                        if (subfile.Exists)
                        {
                            size += subfile.Length;
                        }
                    }
                    foreach (DirectoryInfo subfolder in di.EnumerateDirectories())
                    {
                        if (subfolder.Exists)
                        {
                            foreach (FileInfo subfile2 in folder.EnumerateFiles())
                            {
                                if (subfile2.Exists)
                                {
                                    size += subfile2.Length;
                                }
                            }
                        }
                    }
                }
            }
            return size;
        }
        private static void DeleteAllFiles(string path)
        {
            deletedFiles = 0;
            notDeletedFiles = 0;
            int i = 0;
            int count = GetTempFilesCount(path);
            DirectoryInfo di = new DirectoryInfo(path);

            if (di != null)
            {
                DirectorySecurity dSecurity = di.GetAccessControl();
                dSecurity.AddAccessRule(new FileSystemAccessRule(Globals.AppUser, FileSystemRights.FullControl, AccessControlType.Allow));
                di.SetAccessControl(dSecurity);
                
                foreach (FileInfo file in di.EnumerateFiles())
                {
                    i++;
                    try
                    {
                        file.IsReadOnly = false;
                        file.Delete();
                        deletedFiles++;
                        Console.WriteLine(" Deleted : " + i + " / " + count);
                    }
                    catch (Exception e)
                    {
                        Globals.lastError = e.Message;
                        Functions.LogErrorOrSuccess(Globals.lastError, "Delete of File #:" + i + "-" + file.Name + " Size :" + SummarizeSize(file.Length));
                        notDeletedFiles++;

                        Console.WriteLine(" Skipped : " + i + " / " + count + " | " + Globals.lastError + ": Delete of File #:" + i + "-" + file.Name + " Size :" + SummarizeSize(file.Length));
                        continue;
                    }
                }

                foreach (DirectoryInfo folder in di.EnumerateDirectories())
                {
                    if (folder.Exists)
                    {
                        DirectorySecurity subfolderSec1 = folder.GetAccessControl();
                        subfolderSec1.AddAccessRule(new FileSystemAccessRule(Globals.AppUser, FileSystemRights.FullControl, AccessControlType.Allow));

                        long folderSize = 0;
                        i++;

                        foreach (FileInfo subfile1 in folder.EnumerateFiles())
                        {
                            if (subfile1.Exists)
                            {
                                folderSize += subfile1.Length;
                            }
                        }

                        foreach(DirectoryInfo subfolder in folder.EnumerateDirectories())
                        {
                            if (subfolder.Exists)
                            {
                                DirectorySecurity subfolderSec2 = folder.GetAccessControl();
                                subfolderSec2.AddAccessRule(new FileSystemAccessRule(Globals.AppUser, FileSystemRights.FullControl, AccessControlType.Allow));
                                try
                                {
                                    subfolder.SetAccessControl(subfolderSec2);
                                    foreach (FileInfo subfile1 in folder.EnumerateFiles())
                                    {
                                        if (subfile1.Exists)
                                        {
                                            folderSize += subfile1.Length;
                                        }
                                    }
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }

                        try
                        {
                            folder.SetAccessControl(subfolderSec1);
                            folder.Delete(true);
                            deletedFiles++;
                            Console.WriteLine(" Deleted : " + i + " / " + count);
                        }
                        catch (Exception e)
                        {
                            Globals.lastError = e.Message;
                            Functions.LogErrorOrSuccess(Globals.lastError, "Delete of Folder #:" + i + "-" + folder.Name + " Size :" + SummarizeSize(folderSize));
                            notDeletedFiles++;

                            Console.WriteLine(" Skipped : " + i + " / " + count +" | " + Globals.lastError + ": Delete of Folder #:" + i + "-" + folder.Name + " Size :" + SummarizeSize(folderSize));
                            continue;
                        }

                    }

                }
            }
        }
        private static string SummarizeSize(long size)
        {
            string sizeString = "";

            if (size > 100)
            {
                if (size > 1000)
                {
                    if (size > 1000000)
                    {
                        if (size > 100000000)
                        {
                            if (size > 1000000000000)
                            {
                                sizeString = "" + (size / 1000000000000) + " GB";
                            }
                            else
                            {
                                sizeString = "" + (size / 1000000) + " MB";
                            }
                        }
                        else
                        {
                            sizeString = "" + (size / 100000000) + " MB";
                        }
                    }
                    else
                    {
                        sizeString = "" + (size / 1000000) + " MB";
                    }
                }
                else
                {
                    sizeString = "" + (size / 1000) + " MB";
                }
            }
            else
            {
                sizeString = "" + (size / 100) + " KB";
            }

            return sizeString;
        }
    }
}
