using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using PGA.Database;
using PGA.DataContext;

namespace PGA.TransFormReports
{
    public class ServiceManager : IServiceManager
    {
        private readonly DateTime _dateTime;
        private string _temp = "";
        private string _start = "";

        private readonly string name = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().ProcessName);

        private readonly string path =
            Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public ServiceManager()
        {
        }

        public ServiceManager(DateTime datetime)
        {
            _dateTime = datetime;
        }

        public void HeaderComments()
        {
            try
            {
                Console.WriteLine("**********************************************");
                Console.WriteLine("");

                Console.WriteLine("Starting Reports...");
                Console.WriteLine("Searching for Tasks = " + _dateTime);
                Console.WriteLine("Executable Path = " + System.IO.Path.Combine(path, name + ".exe"));

                Console.WriteLine("");

                Console.WriteLine("**********************************************");
            }
            catch (Exception exception)
            {
                MessengerManager.MessengerManager.LogException(exception);
            }
        }
        public void FooterComments()
        {
            Console.WriteLine("Ending Reports...");

        }

        public async Task<int> StartService()
        {
            try
            {
                SetWorkDirs();  
                CleanDirs();
                CreateWorkDirs();
                HeaderComments();
                DoWork();          //Dwg Errors
                DoWorkAllErrors(); //Gen Errors
                FooterComments();
                return 1;
            }
            catch (Exception exception)
            {
                MessengerManager.MessengerManager.LogException(exception);
            }
            return 0;
        }

        private  void SetWorkDirs()
        {
            try
            {
                var tempPath = Path.GetTempPath();
                var workingfolder = Path.Combine(tempPath, "PGA-Temp-Reports");

                _temp = workingfolder;
                _start = Path.Combine(_temp, "Start");
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        private  void CreateWorkDirs()
        {
            try
            {
                var tempPath = Path.GetTempPath();
                var workingfolder = Path.Combine(tempPath, "PGA-Temp-Reports");

                _temp = workingfolder;
                _start = Path.Combine(_temp, "Start");

                if (!Directory.Exists(workingfolder))
                    Directory.CreateDirectory(workingfolder);

                if (!Directory.Exists(_start))
                    Directory.CreateDirectory(_start);
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        private int DoWork()
        {
            try
            {
                var reports = Commands.GetReportsToWrite();
                var firstOrDefault = reports.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    foreach (var items in reports)
                    {
                        try
                        {
                            var filename = items.FirstOrDefault().Source;

                            if (filename == null)
                                throw new ArgumentNullException
                                    (nameof(filename));

                            using (var commands = new DatabaseCommands())
                            {
                                if (string.IsNullOrEmpty(filename))
                                    return 0;
                                try
                                {
                                    var dwgfiles = commands.GetPolylineDwgDate(_dateTime).FirstOrDefault().SourcePath;
                                    var pntfiles = commands.GetPointPathByName(_dateTime, filename);

                                    if (!File.Exists(Path.Combine(_start, filename)))
                                        File.Copy(Path.Combine(dwgfiles, filename),
                                            Path.Combine(_start, filename));

                                    if (!File.Exists(Path.Combine(_start, Path.GetFileName(pntfiles))))
                                        File.Copy(pntfiles, Path.Combine(_start, Path.GetFileName(pntfiles)));
                                }
                                catch
                                {
                                }                              
                            }
                        }
                        catch (Exception ex)
                        {
                            MessengerManager.MessengerManager.AddLog("Could Not Retrieve file information!" + ex);
                        }
                    }
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.AddLog("Could Not Retrieve Files!" + ex);
            }
            return 1;
        }

        private void CleanDirs()
        {
            try
            {
                var di = new DirectoryInfo(_temp);

                foreach (var file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (var dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch (DirectoryNotFoundException){}
            catch (Exception exception)
            {
                MessengerManager.MessengerManager.LogException(exception);
            }
        }

        private  int DoWorkAllErrors()
        {
            using (var commands = new DatabaseCommands())
            {

                var logs = commands.GetLastAllLogs();
                IEnumerable<Logs> filtered = logs.Where(p => p.Issue.Contains("Alert") ||
                                               p.Issue.Contains("Error") ||
                                               p.Issue.Contains("Acdb") ||
                                               p.Issue.Contains("Runtime") ||
                                               p.Issue.Contains("Exception")
                    );

                if (filtered.FirstOrDefault() == null)
                    return 0;

                using (var sw = new StreamWriter(Path.Combine(_start, "report.txt")))
                {
                    foreach (var l in filtered)
                    {
                        sw.WriteLine("{0}{1}{2}", l.DateStamp, l.Id, l.Issue);
                    }
                    sw.Close();
                }

                using (var sw = new StreamWriter(Path.Combine(_start, "log.txt")))
                {
                    foreach (var l in logs)
                    {
                        sw.WriteLine("{0}{1}{2}", l.DateStamp, l.Id, l.Issue);
                    }
                    sw.Close();
                }

                var remoteName = "Report-" + DateTime.Now.Ticks + ".zip";

                try
                {
                    ZipFile.CreateFromDirectory(_start, Path.Combine(_temp, remoteName));
                    BBC.CloudManager.Program.GetServiceAndUpload(Path.Combine(_temp, remoteName));
                }
                catch
                {
                    return 0;
                }
            }
            return 1;
        }

        public bool StopService()
        {
            try
            {
                var processes = Process.GetProcessesByName(name);
                var count = processes.Count();
                foreach (var p in processes)
                {
                    p.Kill();
                }
                return true;
            }
            catch (Exception exception)
            {
                MessengerManager.MessengerManager.LogException(exception);
            }
            return false;
        }

        public bool SendReport(object info)
        {
            throw new NotImplementedException();
        }

        private string GetDwgFiles(List<PolylineDWGS> dwgfiles, string filename)
        {
            var rec = dwgfiles.Where(p =>
                p.DrawingName.Contains
                    (filename.Trim()))
                .FirstOrDefault();

            var source = rec.SourcePath;
            var name = rec.DrawingName;

            return Path.Combine(source, name);
        }

        private string GetHole(string filename)
        {
            var namewoext = Path.GetFileNameWithoutExtension(filename);
            var hole = namewoext.Substring(namewoext.Length - 2, 2);
            return hole;
        }

        private void ConfigProperties(Process process)
        {
            try
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Verb = "runas";
                process.StartInfo.FileName = Path.Combine(path, name + ".exe");
                process.StartInfo.WorkingDirectory = path;
                process.Start();
            }
            catch (Exception exception)
            {
                MessengerManager.MessengerManager.LogException(exception);
            }
        }


        private void SetAdminPrivledges()
        {
            if (IsAdministrator() == false)
            {
                // Restart program and run as admin
                var ps = Process.GetCurrentProcess();

                var exeName = Process.GetCurrentProcess().MainModule.FileName;
                var startInfo = new ProcessStartInfo(exeName);
                startInfo.Verb = "runas";
                Process.Start(startInfo);
                ps.Kill();
            }
        }

        private static bool IsAdministrator()
        {
            var identity  = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
