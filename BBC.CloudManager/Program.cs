// ***********************************************************************
// Assembly         : BBC.CloudManager
// Author           : Daryl Banks, PSM
// Created          : 06-08-2016
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 06-29-2016
// ***********************************************************************
// <copyright file="Program.cs" company="Banks & Banks Consulting">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Drive.v2;
using PGA.Database;
using File = Google.Apis.Drive.v2.Data.File;
using COMS = PGA.MessengerManager;

namespace BBC.CloudManager
{
    /// <summary>
    /// Class Program.
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            # region OAuth
            // Connect with Oauth2 Ask user for permission
            //String CLIENT_ID = "40682712771-98klh9ien3jf82bk6jsh6o0tt0l0hufr.apps.googleusercontent.com";
            //String CLIENT_SECRET = "zeE-FwAoy_Ja6JrJmE7jDPQ6";      
            //DriveService service = Authentication.AuthenticateOauth(CLIENT_ID, CLIENT_SECRET, Environment.UserName);  
            # endregion

            string TOP_LEVEL_DIR = "DatabaseManager-*";

           // File CurrentVersion = null;
             // connect with a Service Account

            string ServiceAccountEmail = "bbc-gen-svc-p12@cloudupdate-1331.iam.gserviceaccount.com";
            string AppDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string serviceAccountkeyFile = System.IO.Path.Combine(AppDataDir, 
                @"Autodesk\ApplicationPlugins\PGA-CivilTinSurf2018.bundle\Cert\CloudUpdate-5a0a72353113.p12");
            
              DriveService service = Authentication.AuthenticateServiceAccount(ServiceAccountEmail, serviceAccountkeyFile);

            if (service == null)
            {
                Console.WriteLine("Authentication error");
                goto End;
            }

           
            try
            {
                Console.WriteLine("Starting Update Application!");

                // Listing files with search.  
                // This searches for a directory with the name DiamtoSample

                Console.WriteLine("Contacting Google Cloud!");

                //string Q = "title contains " + "'" + "*" + "'" + " and mimeType != 'application/vnd.google-apps.folder'";
                string Q = "title contains " + "'" + TOP_LEVEL_DIR + "'" + " and mimeType != 'application/vnd.google-apps.folder'";

                var _Files = GoogleDriveHelper.GetFiles(service, Q);

                Console.WriteLine("Receiving response From Google Cloud!");


                if (_Files == null || _Files.Count == 0)
                {
                    Console.WriteLine("Could not find any Updates!");
                    goto End;
                }

                IList<File> selected = AddSelectedFiles(_Files);
          

                if (selected.FirstOrDefault() == null)
                {
                    Console.WriteLine("Could not find any New Updates!");
                    goto End;
                }

                foreach (File item in selected)
                {
                    Console.WriteLine("Checking For Updates!");

                    Console.WriteLine(item.Title + " " + item.MimeType);

                    // Assign Permissions
                    var HasOwnership = item.OwnerNames.Where(p => p.Contains("Daryl")).FirstOrDefault();

                    if (String.IsNullOrEmpty(HasOwnership))
                    {
                        GoogleDriveHelper.InsertPermission(service, item.Id, "dsb6063@gmail.com", "user", "owner");
                    }

                    if (IsUpdateQualified(item.Title, item.Md5Checksum))
                    {
                        Console.WriteLine("Found Update!");
                        var dir = CreateTempInstallFolder(item.Md5Checksum);
                        Console.WriteLine("Creating Local Folder! " + dir);
                        SetStartParameters(dir, item.Title, item.Md5Checksum);
                        SetBuildVersion(item.Title);
                        Console.WriteLine("Starting Download!");
                        GoogleDriveHelper.downloadFile(service, item, Path.Combine(dir, item.Title));
                        Console.WriteLine("Download Complete!");
                    }
                    else
                        Console.WriteLine("No new Updates!");

                    break;
                }


                # region MyRegion
                //// If there isn't a directory with this name lets create one.
                //if (_Files.Count == 0)
                //{
                //    _Files.Add(GoogleDriveHelper.createDirectory(service, "Update_1.1", "Update_1.1", "root"));
                //    // Assign Permissions
                //    GoogleDriveHelper.InsertPermission(service, _Files[0].Id, "dsb6063@gmail.com", "user", "owner");
                //}

                // We should have a directory now because we either had it to begin with or we just created one.
                //if (_Files.Count != 0)
                //{

                //    // This is the ID of the directory 
                //   // string directoryId = _Files[0].Id;

                //    ////Upload a file
                //    //File newFile = GoogleDriveHelper.uploadFile(service, @"c:\GoogleDevelop\PGASurfLog.txt", directoryId);
                //    //// Assign Permissions
                //    GoogleDriveHelper.InsertPermission(service, CurrentVersion.Id, "dsb6063@gmail.com", "user", "owner");
                //    //// Update The file
                //    //File UpdatedFile = GoogleDriveHelper.updateFile(service, @"c:\GoogleDevelop\PGASurfLog.txt", directoryId, newFile.Id);
                //    // Download the file
                //    CurrentVersion.DownloadUrl = CurrentVersion.AlternateLink;
                //    GoogleDriveHelper.downloadFile(service, CurrentVersion, String.Format( @"c:\GoogleDevelop\{0}.zip",CurrentVersion.Title));
                //    // delete The file
                //    //FilesResource.DeleteRequest request = service.Files.Delete(newFile.Id);
                //    //request.Execute();
                //}  
# endregion

                // Getting a list of ALL a users Files (This could take a while.)
                //_Files = GoogleDriveHelper.GetFiles(service, null);
                if (selected.FirstOrDefault() != null)
                {
                    foreach (File item in selected)
                    {
                        Console.WriteLine(item.Title + " " + item.MimeType);
                    }
                }            
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

            //clean run, exit here
            Console.WriteLine("Exiting!");
            Thread.Sleep(3000);
            Environment.Exit(0);

            //if any problems, exit here

            End:
            Console.WriteLine("Exiting!");
            Thread.Sleep(3000);
            Environment.Exit(1);
        }

        private static IList<File> AddSelectedFiles(IList<File> files)
        {
            try
            {
                var selected = new List<File>();
                selected.Add(GetVersion(files));
                return selected;
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return null;
        }

        private static void SetCurrentMD5(string md5Checksum)
        {
            try
            {                
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    commands.SetUpdateCurrentVersionMD5(md5Checksum);
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }

        private static bool CompareVersion(string v1,string v2)
        {
            try
            {
                if (String.IsNullOrEmpty(v2)  || String.IsNullOrEmpty(v2))
                    throw new ArgumentException("Argument is null or empty", nameof(v2));

                using (DatabaseCommands commands = new DatabaseCommands())
                {
                
                    var v1BuildDay = commands.ConvertInt32(v1.Substring(0, 5));
                    var v1BuildMin = commands.ConvertInt32(v1.Substring(12, 3));
                    var v2BuildDay = commands.ConvertInt32(v2.Substring(0, 5));
                    var v2BuildMin = commands.ConvertInt32(v2.Substring(12, 3));

                    var latest = String.Format("{0}.{1}", v1BuildDay, v1BuildMin);
                    var older  = String.Format("{0}.{1}", v2BuildDay, v2BuildMin);

                    var d1 = Convert.ToDouble(latest);
                    var d2 = Convert.ToDouble(older);

                    if (d1 > d2)
                        return true;

                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return false;
        }

        /// <summary>
        /// Determines whether [is update qualified] [the specified title].
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="md5Checksum">The MD5 checksum.</param>
        /// <returns><c>true</c> if [true if not equal MD5] [true if null]; otherwise, <c>false</c>.</returns>
        private static bool IsUpdateQualified(string title, string md5Checksum)
        {
            try
            {
                var filename = title.Split(new char[] { '-' }, 2);
                var version = filename[1].Substring(0, 15);

                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var latest = version;
                    var older  = commands.GetCurrentBuild();

                    var test1 = false;
                    if (String.IsNullOrEmpty(older))
                        test1 = true;
                    else
                        test1 = CompareVersion(latest, older);

                    var current = commands.GetUpdateCurrentVersionMD5();

                    var test2 = false;
                    if (String.IsNullOrEmpty(current))
                        test2 = true;
                    else
                       test2 = current == md5Checksum ? false : true;

                    if (test1 && test2)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return false;
        }

        private static void SetBuildVersion(string title)
        {
            if (String.IsNullOrEmpty(title))
                throw new ArgumentException("Argument is null or empty", nameof(title));

            try
            {
                var filename = title.Split(new char[] {'-'}, 2);
                var version  = filename[1].Substring(0, 15);

                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    commands.SetBuildVersionInUpdate(version);
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }

        private static void SetStartParameters(string dir, string topLevelDir, string md5Checksum)
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    commands.SetStartParamsInUpdate(dir,topLevelDir,md5Checksum);
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }

        public static string CreateTempInstallFolder(string MD5)
        {
            try
            {
                if (String.IsNullOrEmpty(MD5))
                    MD5 = DateTime.Now.Ticks.ToString();

                var folder = String.Format("PGA-{0}", MD5);

                string result = Path.GetTempPath();
                string dir = Path.Combine(result, folder);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return dir;
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

            return "";
        }

        public static File GetFilesFromDir(string dir,DriveService service)
        {
            if (String.IsNullOrEmpty(dir))
                return null;
            string Q = "title contains " + "'" + dir + "'" + " and mimeType = 'application/vnd.google-apps.folder'";

            var result = GoogleDriveHelper.GetFiles(service, Q);

            File f = result.Where(p => p.Title == dir).FirstOrDefault();

            return f;
        }

        public static IList<File> DatabaseQuery(string val, string id, DriveService service)
        {
            string Q1 = "'" + id + "'" + " in parents";
            string Q  = "'" + id + "'" + " in parents " +  " and mimeType = 'application/vnd.google-apps.folder'";
            return GoogleDriveHelper.GetFiles(service, Q1);

        }

        public static File GetVersion(IList<File> Version)
        {
            try
            {
                var refined = Version.Where(p => p.Title.Length == 35 && p.Title.Contains("-"))
                                .Select(p => p);

                if (refined.FirstOrDefault() == null)
                    return null;

                var BuildVersion =
                        from path in refined
                        let split = path.Title.Split(new char[] { '-' }, 2)
                        orderby split[1]
                        select new
                        {
                            build = split[1].Substring(0, 15),
                            majbuild = split[1].Substring(0, 5),
                            year = split[1].Substring(0, 2),
                            day = split[1].Substring(2, 3),
                            dbver = split[1].Split(new char[] { '.' }, 5)[1],
                            civil = split[1].Split(new char[] { '.' }, 5)[2],
                            minbuild = split[1].Split(new char[] { '.' }, 5)[3]
                        };

                var ordered = BuildVersion.Where(p => p.civil == "18").OrderByDescending(p =>
                              String.Format("{0}.{1}", p.majbuild, p.minbuild)).FirstOrDefault();
                #region Advanced
                //	
                //	var build        =   b.Split(new char[] {'.'}, 5); 
                //	var majbuild     =   build[0].Dump("maj");
                //	var dbversion    =   build[1].Dump("db");
                //	var civilversion =   build[2].Dump("civilversion");
                //	var minbuild     =   build[3].Dump("minbuild");
                #endregion
                
                var selected =  String.Format("DatabaseManager-{0}.exe", ordered.build);

                var output = Version.Where(p => p.Title == selected).Select(p => p).FirstOrDefault();

                return output;
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return null;
        }


        public static DriveService GetService()
        {
            // connect with a Service Account

            string ServiceAccountEmail = "bbc-gen-svc-p12@cloudupdate-1331.iam.gserviceaccount.com";
            string AppDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string serviceAccountkeyFile = System.IO.Path.Combine(AppDataDir,
                @"Autodesk\ApplicationPlugins\PGA-CivilTinSurf2018.bundle\Cert\CloudUpdate-5a0a72353113.p12");

            DriveService service = Authentication.AuthenticateServiceAccount(ServiceAccountEmail, serviceAccountkeyFile);

            if (service == null)
            {
                Console.WriteLine("Authentication error");
            }

            return service;
        }

        public static void GetServiceAndUpload(string combine)
        {
            File file = null;
            using (DriveService service = GetService())
            {
                file = GoogleDriveHelper.uploadFile(service, combine, "root");
            }

            if (file !=null)
                GetServiceAndTakeOwnerShip(file);
        }

        public static void GetServiceAndTakeOwnerShip(File file)
        {
            using (DriveService service = GetService())
            {
                GoogleDriveHelper.InsertPermission(service, file.Id, "dsb6063@gmail.com", "user", "writer");
            }
        }
    }
}
