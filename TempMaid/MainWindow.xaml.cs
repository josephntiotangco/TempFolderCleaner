using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Security.Permissions;
using System.Security.AccessControl;
using System.Security.Principal;
using TempMaid.Classes;

namespace TempMaid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string tempPath;
        int deletedFiles;
        int notDeletedFiles;
        
        public MainWindow()
        {
            InitializeComponent();
            tempPath = Path.GetTempPath();
            tbTempPath.Text = tempPath.ToUpper();
            InitializeDetails();
            DeleteLogFiles();
        }
        private void InitializeDetails()
        {
            tbTempFilesCount.Text = GetTempFilesCount(tempPath).ToString();
            tbTempFilesSize.Text = SummarizeSize(GetTempFilesSize(tempPath));
            tbStatus.Text = "DELETED : " + (deletedFiles != 0 ? deletedFiles.ToString() : "0") + " NOT DELETED : " + (notDeletedFiles != 0 ? notDeletedFiles.ToString() : "0"); 
        }
        private void DeleteLogFiles()
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
        private int GetTempFilesCount(string path)
        {
            int count = 0;
            DirectoryInfo di = new DirectoryInfo(path);

            foreach(FileInfo file in di.EnumerateFiles())
            {
                if (file.Exists)
                {
                    count++;
                }
            }

            return count;
        }
        private long GetTempFilesSize(string path)
        {
            long size = 0;
            DirectoryInfo di = new DirectoryInfo(path);

            foreach(FileInfo file in di.EnumerateFiles())
            {
                if (file.Exists)
                {
                    size += file.Length;
                }
            }

            return size;
        }
        private void DeleteAllFiles(string path)
        {
            deletedFiles = 0;
            notDeletedFiles = 0;
            int i = 0;
            DirectoryInfo di = new DirectoryInfo(path);

            if(di != null)
            {
                DirectorySecurity dSecurity = di.GetAccessControl();
                dSecurity.AddAccessRule(new FileSystemAccessRule(Globals.AppUser, FileSystemRights.FullControl,AccessControlType.Allow));
                di.SetAccessControl(dSecurity);

                foreach (FileInfo file in di.EnumerateFiles())
                {
                    i++;
                    try
                    {
                        file.IsReadOnly = false;
                        file.Delete();
                        deletedFiles++;
                    }
                    catch (Exception e)
                    {
                        Globals.lastError = e.Message;
                        Functions.LogErrorOrSuccess(Globals.lastError, "Delete of File #:" + i + "-" + file.Name + " Size :" + file.Length );
                        notDeletedFiles++;
                        continue;
                    }

                }
             
            }
        }
        private string SummarizeSize(long size)
        {
            string sizeString = "";

            if(size > 100)
            {
                if (size > 1000)
                {
                    if (size > 1000000)
                    {
                        if (size > 100000000)
                        {
                            if(size > 1000000000000)
                            {
                                sizeString = "" + (size / 1000000000000) + " TB";
                            }
                            else
                            {
                                sizeString = "" + (size / 100000000) + " GB";
                            }
                        }
                        else
                        {
                            sizeString = "" + (size / 1000000) + " MB";
                        }
                    }
                    else
                    {
                        sizeString = "" + (size / 1000000) + " MB";
                    }
                }
                else
                {
                    sizeString = "" + (size / 100) + " KB";
                }
            }

            return sizeString;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            InitializeDetails();
        }

        private void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            DeleteAllFiles(tempPath);
            MessageBox.Show("Done Deleted:" + deletedFiles.ToString());
            InitializeDetails();
        }
    }
}
