using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Win32;

namespace Pge.Common.Framework
{
    /// <summary>
    ///     This class provides a library of functions read and update
    ///     the registry
    /// </summary>
    public static class RegistryUtilities
    {
        public const string GDT_PROFILE = "GDT";

        private const string AUTOCAD_KEY = "Software\\Autodesk\\AutoCAD";
        private const string CURVER = "CurVer";
        public const string MAJOR_DEFAULT = "R17.2";
        private const string MINOR_DEFAULT = "ACAD-7002:409"; // Map 3d
        private const string ACAD_LOCATION = "AcadLocation";
        private const string PROFILES = "Profiles";
        private const string DEFAULT_PROFILE = "";
        private const string UNNAMED_PROFILE = @"<<Unnamed Profile>>";

        private const string INSTALLED_APPS = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        /// <summary>
        ///     Get the AUD product key.
        /// </summary>
        public static string ProductKey
        {
            get
            {
                string audProductKey = string.Format("{0}\\{1}\\{2}", AUTOCAD_KEY, MAJOR_DEFAULT, MINOR_DEFAULT);
                return audProductKey;
            }
        }

        /// <summary>
        ///     Get the AutoCAD product key for the current user.
        /// </summary>
        public static string CurrentProductKey
        {
            get
            {
                RegistryKey acad = Registry.CurrentUser.OpenSubKey(AUTOCAD_KEY);
                if (null != acad)
                {
                    string majorVersion = acad.GetValue(CURVER, MAJOR_DEFAULT) as string;
                    if (null != majorVersion)
                    {
                        RegistryKey major = acad.OpenSubKey(majorVersion);
                        if (null != major)
                        {
                            string minorVersion = major.GetValue(CURVER, MINOR_DEFAULT) as string;
                            if (null != minorVersion)
                            {
                                RegistryKey minor = major.OpenSubKey(minorVersion);
                                if (null != minor)
                                {
                                    return string.Format("{0}\\{1}\\{2}", AUTOCAD_KEY, majorVersion, minorVersion);
                                }
                            }
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        ///     Get the location of the acad.exe for the installed product.
        /// </summary>
        public static string AcadLocation
        {
            get
            {
                string productKey = RegistryUtilities.ProductKey;
                if (null != productKey)
                {
                    RegistryKey acad = Registry.LocalMachine.OpenSubKey(productKey);
                    if (null != acad)
                    {
                        string acadLocation = acad.GetValue(ACAD_LOCATION) as string;
                        if (null != acadLocation)
                        {
                            return acadLocation;
                        }
                    }
                }

                return null;
            }
        }


        /// <summary>
        ///     Switch to the specified AutoCAD profile
        /// </summary>
        /// <param name="profileName">the profileName as string</param>
        public static void SwitchProfile(string profileName)
        {
            string productKey = RegistryUtilities.ProductKey;
            if (null != productKey)
            {
                RegistryKey acad = Registry.CurrentUser.OpenSubKey(productKey);
                if (null != acad)
                {
                    RegistryKey profiles = acad.OpenSubKey(PROFILES, true);
                    if (null != profiles)
                    {
                        profiles.SetValue(DEFAULT_PROFILE, profileName);
                    }
                }
            }

        }

        /// <summary>
        /// Returns true if the profile exists, false if not
        /// </summary>
        /// <param name="profileName">profileName as string</param>
        /// <returns>true if it exists, false if not</returns>
        public static bool ProfileExists(string profileName)
        {
            string productKey = RegistryUtilities.ProductKey;
            if (null != productKey)
            {
                RegistryKey acad = Registry.CurrentUser.OpenSubKey(productKey);
                if (null != acad)
                {
                    RegistryKey profiles = acad.OpenSubKey(PROFILES, true);
                    if (null != profiles)
                    {
                        RegistryKey profile = profiles.OpenSubKey(profileName, false);
                        if (null != profile)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;

        }

        /// <summary>
        /// Deletes the AutoCAD profile identified by profile name
        /// </summary>
        /// <param name="profileName">profileName as String</param>
        public static void DeleteProfile(string profileName)
        {
            string productKey = RegistryUtilities.ProductKey;
            if (null != productKey)
            {
                RegistryKey acad = Registry.CurrentUser.OpenSubKey(productKey);
                if (null != acad)
                {
                    RegistryKey profiles = acad.OpenSubKey(PROFILES, true);
                    if (null != profiles)
                    {
                        RegistryKey profile = profiles.OpenSubKey(profileName, false);
                        if (null != profile)
                        {
                            profiles.DeleteSubKeyTree(profileName);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Switch to the default AutoCAD profile
        /// </summary>
        public static void SwitchDefaultProfile()
        {
            RegistryUtilities.SwitchProfile(UNNAMED_PROFILE);
        }

        public static void SaveGemsPasswordToRegistry(string scrambledPassword)
        {
            RegistryKey rKey = null;
            rKey = Registry.CurrentUser.OpenSubKey("GEMSEnvironment");
            if (rKey == null)
            {
                rKey = Registry.CurrentUser.CreateSubKey("GEMSEnvironment");
                if (!String.IsNullOrEmpty(scrambledPassword))
                {
                    rKey.SetValue(String.Empty, scrambledPassword);
                }
            }
        }

        /// <summary>
        /// Gets the gems password.
        /// </summary>
        /// <returns></returns>
        public static string GetGemsPassword()
        {
            string password = String.Empty;
            RegistryKey rKey = null;
            rKey = Registry.CurrentUser.OpenSubKey("GEMSEnvironment");
            if (rKey != null)
            {
                rKey = Registry.CurrentUser.CreateSubKey("GEMSEnvironment");
                password = (string)rKey.GetValue("", password);
            }
            return password;
        }

        /// <summary>
        /// Deletes the GEMS password key.
        /// </summary>
        /// <returns></returns>
        public static void DeleteGemsPassword()
        {
            Registry.CurrentUser.DeleteSubKey("GEMSEnvironment", false);
        }
    }
}