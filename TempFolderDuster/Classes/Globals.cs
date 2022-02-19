using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TempFolderDuster.Classes
{
    public class Globals
    {
        public static string tempFolder = Path.GetTempPath();
        public static string LogFile = (AppDomain.CurrentDomain.BaseDirectory + @"\Log\").Replace("\\\\", "\\");
        public static string lastError;
        public static string AppUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
    }
}
