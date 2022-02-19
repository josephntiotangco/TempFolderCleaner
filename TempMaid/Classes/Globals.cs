using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempMaid.Classes
{
    public class Globals
    {
        public static string LogFile = (AppDomain.CurrentDomain.BaseDirectory + @"\Log\").Replace("\\\\", "\\");
        public static string lastError;
        public static string AppUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
    }
}
