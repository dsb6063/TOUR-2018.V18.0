using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace PGA.TransFormReports
{

   public class Program
    {
       [STAThread]
       public static void Main(string[] args)
       {
           try
           {
                #region Test Mode
                ServiceManager sm = new ServiceManager(new DateTime(636090130330970000));

                #endregion
                #region Live Mode

                //var dateStamp = new DateTime(Convert.ToInt64(args[0]));

                //PGA.MessengerManager.MessengerManager.AddLog("Searching for Tasks: " + dateStamp);

                //ServiceManager sm = new ServiceManager(dateStamp); 
                #endregion

                try
                {
                   sm.StopService();
               }
               catch
               {
               }
               try
               {
                   sm.StartService();
               }
               catch
               {
               }
           }
           catch
           {
                Console.WriteLine("Error: Exiting Now!");
           }
       }


    }
}
