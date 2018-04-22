#region

using System;
using Autodesk.AutoCAD.DatabaseServices;

#endregion

namespace PGA.Autodesk.Settings
{
    public class AcadSettings
    {
        // Registry Path
        private string m_path = "";

        /// <summary>
        ///     Global Variables for the App.
        /// </summary>
        /// <remarks></remarks>

        // Autodesk Reg Node
        private string topRK = "";

        public string TopRK
        {
            get { return topRK = HostApplicationServices.Current.UserRegistryProductRootKey; }
        }

        public string Path
        {
            get { return m_path = TopRK + @"\Applications\PGA\PGA-PuttTinSurface2014.bundle"; }
        }

        // Folder and Registry Path's
        public static string RegistryPath
        {
            get
            {
                return System.IO.Path.Combine(HostApplicationServices.Current.UserRegistryProductRootKey,
                    @"\Applications\PGA\PGA-PuttTinSurface2014.bundle");
            }
        }

        public static string AppFolderPath
        {
            get
            {
                var Top = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Top + @"\Autodesk\ApplicationPlugins\PGA-PuttTinSurface2014.bundle";
            }
        }

        public static string AppFolderResourcePath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                       @"\Autodesk\ApplicationPlugins\PGA-PuttTinSurface2014.bundle\Contents\Resources";
            }
        }

        public static string AppFolderScriptPath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                       @"\Autodesk\ApplicationPlugins\PGA-PuttTinSurface2014.bundle\Contents\Scripts";
            }
        }
    }
}