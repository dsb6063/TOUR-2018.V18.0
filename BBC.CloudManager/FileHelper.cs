// ***********************************************************************
// Assembly         : BBC.CloudManager
// Author           : Daryl Banks, PSM
// Created          : 06-10-2016
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 02-11-2018
// ***********************************************************************
// <copyright file="FileHelper.cs" company="Banks & Banks Consulting">
//     Copyright ©  2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Drive.v2.Data;

namespace BBC.CloudManager
{
    /// <summary>
    /// Class FileHelper.
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Gets the latest version.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns>System.String.</returns>
        public static string GetLatestVersion(IList<File> files)
        {
            try
            {
                //Q-Build-16148.9.16.317 (2016)
                //Q-Build-18148.9.18.317 (2018)


                return GetBuild("18", GetFiles(files));

            }
            catch (Exception)
            {

            }
            return null;
        }
        /// <summary>
        /// Gets the latest version full text.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns>System.String.</returns>
        public static string GetLatestVersionFullText(IList<File> files)
        {
            try
            {
                //Q-Build-16148.9.16.317

                return GetBuildFullText("18", GetFiles(files));

            }
            catch (Exception)
            {

            }
            return null;
        }

        /// <summary>
        /// Gets the build full text.
        /// </summary>
        /// <param name="civilyear">The civilyear.</param>
        /// <param name="files">The files.</param>
        /// <returns>System.String.</returns>
        private static string GetBuildFullText(string civilyear, List<string> files)
        {
            # region commented

            //var t = new List<string>();
            //t.Add("Q-Build-16149.9.16.319");
            //t.Add("Q-Build-16149.9.16.317");
            //t.Add("Q-Build-16149.9.16.318");

            //var previousbuild = "16148.9.16.000";  

            # endregion

            var BuildVersion =
                from path in files
                let split = path.Split(new char[] {'-'}, 3)
                orderby split[2]
                select new
                {
                    name  = path,
                    build = split[2],
                    majbuild = split[2].Substring(0, 5),
                    year = split[2].Substring(0, 2),
                    day = split[2].Substring(2, 3),
                    dbver = split[2].Split(new char[] {'.'}, 4)[1],
                    civil = split[2].Split(new char[] {'.'}, 4)[2],
                    minbuild = split[2].Split(new char[] {'.'}, 4)[3]
                };

            #region commented

            //		 BuildVersion.Dump();
            //		 BuildVersion.OrderByDescending(p=>p.civil).Dump();
            //		 BuildVersion.OrderByDescending(p=>p.year).Dump();
            //		 BuildVersion.OrderByDescending(p=>p.day).Dump();
            //		 BuildVersion.OrderByDescending(p=>p.majbuild).Dump();  

            # endregion

            // largest build with provided civil year 
            var result = BuildVersion.Where(p => p.civil == civilyear).OrderByDescending
                (p => p.majbuild + p.minbuild).FirstOrDefault();

            return (result == null) ? "" : result.name;

        }

        /// <summary>
        /// Gets the latest version identifier.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns>System.String.</returns>
        public static string GetLatestVersionId(IList<File> files)
        {
            try
            {
                //Q-Build-16148.9.16.317

                return GetBuild("18", GetFilesId(files));

            }
            catch (Exception)
            {

            }
            return null;
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="myfiles">The myfiles.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public static List<string> GetFiles(IList<File> myfiles)
        {
            return myfiles.Select(p => p.Title).ToList();
        }

        /// <summary>
        /// Gets the files identifier.
        /// </summary>
        /// <param name="myfiles">The myfiles.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public static List<string> GetFilesId(IList<File> myfiles)
        {
            return myfiles.Select(p => p.Id).ToList();
        }
        /// <summary>
        /// Gets the build.
        /// </summary>
        /// <param name="civilyear">The civilyear.</param>
        /// <param name="files">The files.</param>
        /// <returns>System.String.</returns>
        public static string GetBuild(string civilyear, List<string> files )
        {
            # region commented
            //var t = new List<string>();
            //t.Add("Q-Build-16149.9.16.319");
            //t.Add("Q-Build-16149.9.16.317");
            //t.Add("Q-Build-16149.9.16.318");

            //var previousbuild = "16148.9.16.000";  
            # endregion

            var BuildVersion =
                    from path in files
                    let split = path.Split(new char[] { '-' }, 3)
                    orderby split[2]
                    select new
                    {
                        build    = split[2],
                        majbuild = split[2].Substring(0, 5),
                        year = split[2].Substring(0, 2),
                        day  = split[2].Substring(2, 3),
                        dbver = split[2].Split(new char[] { '.' }, 4)[1],
                        civil = split[2].Split(new char[] { '.' }, 4)[2],
                        minbuild = split[2].Split(new char[] { '.' }, 4)[3]
                    };

            #region commented
            //		 BuildVersion.Dump();
            //		 BuildVersion.OrderByDescending(p=>p.civil).Dump();
            //		 BuildVersion.OrderByDescending(p=>p.year).Dump();
            //		 BuildVersion.OrderByDescending(p=>p.day).Dump();
            //		 BuildVersion.OrderByDescending(p=>p.majbuild).Dump();  
            # endregion

            // largest build with provided civil year 
            var result = BuildVersion.Where(p => p.civil == civilyear).OrderByDescending
                         (p => p.majbuild + p.minbuild).FirstOrDefault();

            return (result == null) ? "" : result.build;
        
        }
        #region Methods for parsing format Q-Build-16149.9.16.318 
        /// <summary>
        /// Gets the civil year.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>System.String.</returns>
        public static string GetCivilYear(string file)
        {
            var split = file.Split(new char[] { '-' }, 3);
            var BuildVersion = split[2];
            return split[2].Split(new char[] { '.' }, 4)[2];
        }
        /// <summary>
        /// Gets the year.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>System.String.</returns>
        public static string GetYear(string file)
        {
            var split = file.Split(new char[] { '-' }, 3);
            var BuildVersion = split[2];
            return split[2].Substring(0, 2);
        }
        /// <summary>
        /// Gets the major build.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>System.String.</returns>
        public static string GetMajorBuild(string file)
        {
            var split = file.Split(new char[] { '-' }, 3);
            var BuildVersion = split[2];
            return split[2].Substring(0, 5);
        }

        /// <summary>
        /// Gets the minor build.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>System.String.</returns>
        public static string GetMinorBuild(string file)
        {
            var split = file.Split(new char[] { '-' }, 3);
            var BuildVersion = split[2];
            return split[2].Split(new char[] { '.' }, 4)[3];
        }

        /// <summary>
        /// Gets the build from cloud version.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>System.String.</returns>
        public static string GetBuildFromCloudVersion(string file)
        {
            var split = file.Split(new char[] { '-' }, 3);
            return split[2];
        }
        #endregion

        /// <summary>
        /// Gets the latest version by file.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns>File.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static File GetLatestVersionByFile(IList<File> files)
        {
            throw new NotImplementedException();
        }
    }

}
