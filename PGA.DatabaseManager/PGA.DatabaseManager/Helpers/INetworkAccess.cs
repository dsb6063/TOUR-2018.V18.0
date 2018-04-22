using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PGA.DatabaseManager.Helpers
{
    public interface INetworkAccess
    {
        bool TestAccess(string path);
    }

    public class NetworkAccess:INetworkAccess
    {
        public bool TestAccess(string path)
        {
            if (String.IsNullOrEmpty(path)) return false;

            ReadWriteTest.CurrentUserSecurity security = new ReadWriteTest.CurrentUserSecurity();

            DirectoryInfo directory = new DirectoryInfo(path);

            if (!security.HasAccess(directory, FileSystemRights.Read))
                MessageBox.Show("Directory does not have Read Access! " + path);
            else if (!security.HasAccess(directory, FileSystemRights.Write))
                MessageBox.Show("Directory does not have Write Access! " + path);
            else if (!security.HasAccess(directory, FileSystemRights.Delete))
                MessageBox.Show("Directory does not have Delete Permissions! " + path);
            else if (!security.HasAccess(directory, FileSystemRights.AppendData))
                MessageBox.Show("Directory does not have Append Permissions! " + path);
            else if (!security.HasAccess(directory, FileSystemRights.CreateDirectories))
                MessageBox.Show("Directory does not have Create Directory Permissions! " + path);
            else if (!security.HasAccess(directory, FileSystemRights.CreateFiles))
                MessageBox.Show("Directory does not have Create file Permissions! " + path);
            else
            {
                return true; //has access
            }

            return false; //failed
        }
    }
}
