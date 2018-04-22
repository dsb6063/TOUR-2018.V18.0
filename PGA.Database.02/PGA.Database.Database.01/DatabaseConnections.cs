#region

using System;
using System.Data.SqlServerCe;
using System.IO;
using System.Reflection;
using PGA.DataContext;

#endregion

namespace PGA.Database
{
    public class GetDataBasePath
    {
        public static string DbFileName { get; } = "PGA.sdf";

        public static string PathDb { get; } =
            @"Autodesk\ApplicationPlugins\PGA-CivilTinSurf2018.bundle\Database\";

        public static string TemplatePath { get; } =
            @"autodesk\ApplicationPlugins\PGA-CivilTinSurf2018.bundle\contents\template\pga tour.dwt";

        public static string LogPath { get; } =
            @"autodesk\ApplicationPlugins\PGA-CivilTinSurf2018.bundle\contents\Logs\logs.txt";

        public static string CreateIfNotExists(string fileName)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // Set the data directory to the users %AppData% folder               
            // So the database file will be placed in:  C:\\Users\\<Username>\\AppData\\Roaming\\                
            AppDomain.CurrentDomain.SetData("DataDirectory", path); // Enure that the database file is present    
            if (!File.Exists(Path.Combine(path, fileName)))
            {
                //Get path to our .exe, which also has a copy of the database file      
                var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var codebase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                var exePath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                //Copy the file from the .exe location to the %AppData% folder       
                //File.Copy(Path.Combine(exePath, fileName), Path.Combine(path, fileName));

                return Path.Combine(path, fileName);
            }
            throw new FileNotFoundException("PGA.SDF Not Found!");
        }

        public static void GetPluginPath()
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var codebase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

            //System.Windows.Forms.MessageBox.Show(string.Format("Location = {0}\nCodeBase = {1}", location, codebase));
        }

        public static string GetAppPath()
        {
            return GetAppPath(null);
        }

        public static string GetAppPath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = DbFileName;

            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = Path.Combine(path, PathDb);
            // Set the data directory to the users %AppData% folder               
            // So the database file will be placed in:  C:\\Users\\<Username>\\AppData\\Roaming\\                
            //AppDomain.CurrentDomain.SetData("DataDirectory", path); // Enure that the database file is present    
            if (File.Exists(Path.Combine(path, fileName)))
            {
                //Get path to our .exe, which also has a copy of the database file      
                //var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                //var codebase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                //var exePath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                //Copy the file from the .exe location to the %AppData% folder       
                //File.Copy(Path.Combine(exePath, fileName), Path.Combine(path, fileName));

                return Path.Combine(path, fileName);
            }
            throw new FileNotFoundException("PGA.SDF Not Found!");
        }

        public static string GetTemplatePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = Path.GetFileName(TemplatePath);

            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = Path.Combine(path, Path.GetDirectoryName(TemplatePath));

            if (File.Exists(Path.Combine(path, fileName)))
            {
                var target = Path.Combine(path, fileName);
                return target;
            }
            throw new FileNotFoundException("PGA.SDF Not Found!");
        }

        public static string GetLogPath()
        {
            var fileName = Path.GetFileName(LogPath);

            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = Path.Combine(path, Path.GetDirectoryName(LogPath));

            if (File.Exists(Path.Combine(path, fileName)))
            {
                return Path.Combine(path, fileName);
            }
            throw new FileNotFoundException("Log.txt Not Found!");
        }

        public static PGAContext GetSql4Connection()
        {
            var connectionString = string.Format(
                "DataSource=\"{0}\"; Password='{1}'", GetAppPath(), "PGA");
            var conn = new SqlCeConnection(connectionString);
            var context = new PGAContext(conn);
            return context;
        }

        public static string GetExecutionPath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = Path.GetFileName(TemplatePath);

            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = Path.Combine(path, Path.GetDirectoryName(TemplatePath));
            // Set the data directory to the users %AppData% folder               
            // So the database file will be placed in:  C:\\Users\\<Username>\\AppData\\Roaming\\                
            //AppDomain.CurrentDomain.SetData("DataDirectory", path); // Enure that the database file is present    
            if (File.Exists(Path.Combine(path, fileName)))
            {
                //Get path to our .exe, which also has a copy of the database file      
                var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var codebase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                var exePath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                //Copy the file from the .exe location to the %AppData% folder       
                File.Copy(Path.Combine(exePath, fileName), Path.Combine(path, fileName));

                return Path.Combine(path, fileName);
            }
            throw new FileNotFoundException("PGA.SDF Not Found!");
        }
    }
}