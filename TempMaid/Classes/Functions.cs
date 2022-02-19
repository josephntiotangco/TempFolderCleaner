using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TempMaid.Classes
{
    public class Functions
    {

        public static bool LogToFile(string FilePathName, string Content, string LastOperation)
        {
            try
            {
                if (File.Exists(FilePathName) == false) { File.Create(FilePathName).Dispose(); }
                using (StreamWriter sw = new StreamWriter(FilePathName, true))
                {
                    string WriteText = String.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now.ToString()) + "\t" + Content;
                    if (LastOperation != "")
                    {
                        WriteText = WriteText + "Last Performed Operation: " + LastOperation;
                    }
                    sw.WriteLine(WriteText);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public static bool LogErrorOrSuccess(string error, string lastOperation = "")
        {
            // mode = 0:Error 1:Success
            string filePath;
            string content;

            try
            {
                content = " MESSAGE: " + error;
                if (!Directory.Exists(Globals.LogFile)) { Directory.CreateDirectory(Globals.LogFile); }

                filePath = Globals.LogFile + @"\Log_" + String.Format("{0:0000}", "TempDuster") + "_" + String.Format("{0:yyyyMMdd}", DateTime.Now) + ".log"; //change datetime.now 

                return LogToFile(filePath, content, lastOperation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

        }

    }
}
