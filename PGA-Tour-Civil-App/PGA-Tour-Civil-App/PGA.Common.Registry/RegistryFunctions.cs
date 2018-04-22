#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.AccessControl;
using Microsoft.Win32;
using Reg = Microsoft.Win32.Registry;
using settings = PGA.Autodesk.Settings;

#endregion

namespace PGA.Common.Registry
{
    /// <summary>
    ///     Initialize values from Registry
    /// </summary>
    public class RegistryFunctions
    {
        public static settings.AcadSettings settings = new settings.AcadSettings();

        /// <summary>
        ///     Globals the write to registry                                                             .
        /// </summary>
        /// <param name="key">The key                                     .</param>
        /// <param name="value">The value                                 .</param>
        /// <param name="Path">The path                                   .</param>
        /// <remarks></remarks>
        public static void GlobalWriteToRegistry(string key, string value, string Path)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(Path))
            {
                Debug.WriteLine("Registry Keys are empty!");
                throw new NoNullAllowedException(string.Format("Registry Keys are empty! Key{0},Path{1}", key, Path));
                //return;
            }

            //RegistryKey regKeyAppRoot = Registry.LocalMachine.CreateSubKey(settings.Path);
            var rs = new RegistrySecurity();


            var user = System.IO.Path.Combine(Environment.UserDomainName, Environment.UserName);


            // Allow the current user to read and delete the key                                          .
            //
            rs.AddAccessRule(new RegistryAccessRule(user,
                RegistryRights.ReadKey | RegistryRights.Delete |
                RegistryRights.WriteKey,
                InheritanceFlags.None,
                PropagationFlags.None,
                AccessControlType.Allow));

            var rk = Reg.CurrentUser;
            try
            {
                var rkTop = rk.OpenSubKey(settings.Path, true);
                using (rkTop)
                {
                    if (rkTop == null)
                    {
                        // Top key does not exist, create it                                                          .

                        var rkTopCreated = Reg.CurrentUser.CreateSubKey(settings.Path,
                            RegistryKeyPermissionCheck.Default,
                            rs);

                        // set the default value
                        if (rkTopCreated != null) rkTopCreated.SetValue(key, value);
                    }
                    else
                    {
                        // key exists
                        try
                        {
                            rkTop.SetValue(key, value);
                        }
                        catch (Exception)
                        {
                            rkTop.SetValue(key, value);
                        }
                    }
                }

                rk.Close();
            }

            catch (NullReferenceException ex)
            {
                MessengerManager.MessengerManager.AddLog(string.Format("Registry: {0}", ex.Message));
            }
        }

        /// <summary>
        ///     Globals the read from registry                                                            .
        /// </summary>
        /// <param name="key">The key                                     .</param>
        /// <param name="Path">The path                                   .</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GlobalReadFromRegistry(string key, string Path)
        {
            var strPassCode = "";
            try
            {
                var rs = new RegistrySecurity();


                var user = System.IO.Path.Combine(Environment.UserDomainName, Environment.UserName);


                // Allow the current user to read and delete the key                                          .
                //
                rs.AddAccessRule(new RegistryAccessRule(user,
                    RegistryRights.ReadKey | RegistryRights.Delete |
                    RegistryRights.WriteKey,
                    InheritanceFlags.None,
                    PropagationFlags.None,
                    AccessControlType.Allow));

                var rk = Reg.CurrentUser;

                // Valid input
                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(settings.Path))
                {
                    return "";
                }

                // Combine Path and Key
                var subKey = System.IO.Path.Combine(settings.Path, key);

                //Verify the top level node exists

                using (var rkTop = rk.OpenSubKey(settings.Path, true))
                {
                    if (rkTop == null)
                    {
                        // Top key does not exist, create it                                                          .

                        var rkTopCreated = Reg.CurrentUser.CreateSubKey(settings.Path,
                            RegistryKeyPermissionCheck.Default,
                            rs);

                        // set the default value
                        if (rkTopCreated != null) rkTopCreated.SetValue(key, "");
                    }
                    else
                    {
                        // key exists
                        try
                        {
                            strPassCode = rkTop.GetValue(key).ToString();
                        }
                        catch (Exception)
                        {
                            rkTop.SetValue(key, "");
                            strPassCode = rkTop.GetValue(key).ToString();
                        }
                    }
                }

                rk.Close();
            }

            catch (ArgumentNullException ex)
            {
                MessengerManager.MessengerManager.AddLog(string.Format("MSG103003: Security Permissions." +
                                                                       " Caught ArgumentNullException: {0}", ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                MessengerManager.MessengerManager.AddLog(
                    string.Format("MSG103004: Unable to change permissions for the key." +
                                  " Caught UnauthorizedAccessException: {0}", ex.Message));
            }

            catch (Exception ex)
            {
                MessengerManager.MessengerManager.AddLog("MSG103005: RegistryFunctions" + ex.Message);
            }

            return strPassCode;
        }

        /// <summary>
        ///     Globals the registry is top level exist                                                   .
        /// </summary>
        /// <param name="Path">The path                                   .</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool GlobalRegistryIsTopLevelExist(string Path)
        {
            if (string.IsNullOrEmpty(settings.Path))
            {
                return false;
            }

            //RegistryKey regKeyAppRoot            = Registry                                             .LocalMachine.CreateSubKey(settings.Path);
            var rs = new RegistrySecurity();


            var user = Environment.UserDomainName + "\\" + Environment.UserName;


            // Allow the current user to read and delete the key                                          .
            //
            rs.AddAccessRule(new RegistryAccessRule(user,
                RegistryRights.ReadKey | RegistryRights.Delete |
                RegistryRights.WriteKey,
                InheritanceFlags.None,
                PropagationFlags.None,
                AccessControlType.Allow));

            var rk = Reg.CurrentUser;
            try
            {
                using (var rkTop = rk.OpenSubKey(settings.Path, true))
                {
                    if (rkTop == null)
                    {
                        // Top key does not exist, create it                                                          .

                        var rkTopCreated = Reg.CurrentUser.CreateSubKey(settings.Path,
                            RegistryKeyPermissionCheck.Default,
                            rs);
                    }
                }

                rk.Close();
            }

            catch (NullReferenceException ex)
            {
                MessengerManager.MessengerManager.AddLog(string.Format("MSG103006: {0}", ex.Message));
            }

            return true;
        }

        /// <summary>
        ///     Writes to registry                                                                        .
        /// </summary>
        /// <param name="txt">The TXT                                     .</param>
        /// <remarks></remarks>
        //public void WriteToRegistry(TextBox txt)
        //{
        //    var settings                         = new Settings();

        //    try
        //    {
        //        GlobalWriteToRegistry("Key", Encryption                                             .ComputerSHA1Hash(txt.Text), settings.Path);
        //        GlobalWriteToRegistry("PassCode", Encryption                                        .ComputerMD5Hash(txt.Text), settings.Path);
        //    }
        //    catch (NullReferenceException ex)
        //    {
        //        Helper                                                                              .InfoMessageBox((String.Format("MSG103007: {0}", ex.Message)));
        //    }
        //}
        public IList<string> RegistryValues()
        {
            IList<string> myRegVals = new List<string>();

            myRegVals.Add(TurboBoost);
            myRegVals.Add(AutoCADVersion);
            myRegVals.Add(Address);
            myRegVals.Add(AppendDate);
            myRegVals.Add(BuisnessInfo);
            myRegVals.Add(Certificate_CN);
            myRegVals.Add(Certificate_Method);
            myRegVals.Add(Certificate_Path);
            myRegVals.Add(Company_Name);
            myRegVals.Add(DateFormat);
            myRegVals.Add(EncryptionMethod);
            myRegVals.Add(IndividualName);
            myRegVals.Add(InitialDirectory);
            myRegVals.Add(Key);
            myRegVals.Add(License);
            myRegVals.Add(Output_Format);
            myRegVals.Add(PassCode);
            myRegVals.Add(Profession);
            myRegVals.Add(Replace);
            myRegVals.Add(Search);
            myRegVals.Add(StampAffix);
            myRegVals.Add(StampDateNotCode);
            myRegVals.Add(StampSearchText);

            myRegVals.Add(Title);
            myRegVals.Add(Author);
            myRegVals.Add(Subject);
            myRegVals.Add(Keywords);
            myRegVals.Add(Comments);
            myRegVals.Add(EnableComments);
            myRegVals.Add(CusLicense);
            myRegVals.Add(CusProfession);
            myRegVals.Add(CusMACID);
            myRegVals.Add(CusIndividName);
            myRegVals.Add(CusElectronicSig);
            myRegVals.Add(CusComputerID);
            myRegVals.Add(CusCompanyName);
            myRegVals.Add(CusBusInfo);
            myRegVals.Add(CusAddress);
            myRegVals.Add(CusTitle);
            myRegVals.Add(BlkLPadding);
            myRegVals.Add(PasswordLevel);
            myRegVals.Add(PasswordUseAlpha);
            myRegVals.Add(IsNoBlock);
            myRegVals.Add(SvTolerance);

            return myRegVals;
        }

        /// <summary>
        ///     Initializes the reg vals                                                                  .
        /// </summary>
        public void InitializeRegVals()
        {
            //TODO foreach loop to read registry values
            TurboBoost = GlobalReadFromRegistry("TurboBoost", settings.Path);
            AutoCADVersion = GlobalReadFromRegistry("ACAD_VERSION", settings.Path);
            Address = GlobalReadFromRegistry("Address", settings.Path);
            AppendDate = GlobalReadFromRegistry("Append_Date", settings.Path);
            BuisnessInfo = GlobalReadFromRegistry("BusinessInfo", settings.Path);
            Certificate_CN = GlobalReadFromRegistry("Certificate_CN", settings.Path);
            Certificate_Method = GlobalReadFromRegistry("Certificate_Method", settings.Path);
            Certificate_Path = GlobalReadFromRegistry("Certificate_Path", settings.Path);
            Company_Name = GlobalReadFromRegistry("CompanyName", settings.Path);
            DateFormat = GlobalReadFromRegistry("Date_Format", settings.Path);
            EncryptionMethod = GlobalReadFromRegistry("Encryption_Method", settings.Path);
            IndividualName = GlobalReadFromRegistry("IndividualName", settings.Path);
            InitialDirectory = GlobalReadFromRegistry("InitialDir", settings.Path);
            Key = GlobalReadFromRegistry("Key", settings.Path);
            License = GlobalReadFromRegistry("License", settings.Path);
            Output_Format = GlobalReadFromRegistry("Output_Format", settings.Path);
            PassCode = GlobalReadFromRegistry("PassCode", settings.Path);
            Profession = GlobalReadFromRegistry("Profession", settings.Path);
            Replace = GlobalReadFromRegistry("Replace", settings.Path);
            Search = GlobalReadFromRegistry("Search", settings.Path);
            StampAffix = GlobalReadFromRegistry("StampAffix", settings.Path);
            StampDateNotCode = GlobalReadFromRegistry("StampDateNotCode", settings.Path);
            StampSearchText = GlobalReadFromRegistry("StampSearchText", settings.Path);

            Title = GlobalReadFromRegistry("Title", settings.Path);
            Author = GlobalReadFromRegistry("Author", settings.Path);
            Subject = GlobalReadFromRegistry("Subject", settings.Path);
            Keywords = GlobalReadFromRegistry("Keywords", settings.Path);
            Comments = GlobalReadFromRegistry("Comments", settings.Path);
            EnableComments = GlobalReadFromRegistry("EnableComments", settings.Path);
            CusTitle = GlobalReadFromRegistry("CusTitle", settings.Path);
            CusProfession = GlobalReadFromRegistry("CusProfession", settings.Path);
            CusMACID = GlobalReadFromRegistry("CusMACID", settings.Path);
            CusLicense = GlobalReadFromRegistry("CusLicense", settings.Path);
            CusIndividName = GlobalReadFromRegistry("CusIndividName", settings.Path);
            CusElectronicSig = GlobalReadFromRegistry("CusElectronicSig", settings.Path);
            CusComputerID = GlobalReadFromRegistry("CusComputerID", settings.Path);
            CusCompanyName = GlobalReadFromRegistry("CusCompanyName", settings.Path);
            CusBusInfo = GlobalReadFromRegistry("CusBusInfo", settings.Path);
            CusAddress = GlobalReadFromRegistry("CusAddress", settings.Path);
            BlkLPadding = GlobalReadFromRegistry("BlkLPadding", settings.Path);
            PasswordLevel = GlobalReadFromRegistry("PasswordLevel", settings.Path);
            PasswordUseAlpha = GlobalReadFromRegistry("PasswordUseAlpha", settings.Path);
            IsNoBlock = GlobalReadFromRegistry("IsNoBlock", settings.Path);
            SvTolerance = GlobalReadFromRegistry("SvTolerance", settings.Path);
        }

        #region  Public Properties

        //Public Properties
        private static string m_TurboBoost = "";

        public string TurboBoost
        {
            get { return m_TurboBoost; }
            set
            {
                if (m_TurboBoost == value)
                    return;
                m_TurboBoost = value;
            }
        }

        private static string m_AutoCADVersion = "";

        public string AutoCADVersion
        {
            get { return m_AutoCADVersion; }
            set
            {
                if (m_AutoCADVersion == value)
                    return;
                m_AutoCADVersion = value;
            }
        }

        private static string m_Address = "";

        public string Address
        {
            get { return m_Address; }
            set
            {
                if (m_Address == value)
                    return;
                m_Address = value;
            }
        }

        private static string m_AppendDate = "";

        public string AppendDate
        {
            get { return m_AppendDate; }
            set
            {
                if (m_AppendDate == value)
                    return;
                m_AppendDate = value;
            }
        }

        private static string m_BuisnessInfo = "";

        public string BuisnessInfo
        {
            get { return m_BuisnessInfo; }
            set
            {
                if (m_BuisnessInfo == value)
                    return;
                m_BuisnessInfo = value;
            }
        }

        private static string m_Certificate_CN = "";

        public string Certificate_CN
        {
            get { return m_Certificate_CN; }
            set
            {
                if (m_Certificate_CN == value)
                    return;
                m_Certificate_CN = value;
            }
        }

        private static string m_Certificate_Method = "";

        public string Certificate_Method
        {
            get { return m_Certificate_Method; }
            set
            {
                if (m_Certificate_Method == value)
                    return;
                m_Certificate_Method = value;
            }
        }

        private static string m_Certificate_Path = "";

        public string Certificate_Path
        {
            get { return m_Certificate_Path; }
            set
            {
                if (m_Certificate_Path == value)
                    return;
                m_Certificate_Path = value;
            }
        }

        private static string m_Company_Name = "";

        public string Company_Name
        {
            get { return m_Company_Name; }
            set
            {
                if (m_Company_Name == value)
                    return;
                m_Company_Name = value;
            }
        }

        private static string m_DateFormat = "";

        public string DateFormat
        {
            get { return m_DateFormat; }
            set
            {
                if (m_DateFormat == value)
                    return;
                m_DateFormat = value;
            }
        }

        private static string m_EncryptionMethod = "";

        public string EncryptionMethod
        {
            get { return m_EncryptionMethod; }
            set
            {
                if (m_EncryptionMethod == value)
                    return;
                m_EncryptionMethod = value;
            }
        }

        private static string m_IndividualName = "";

        public string IndividualName
        {
            get { return m_IndividualName; }
            set
            {
                if (m_IndividualName == value)
                    return;
                m_IndividualName = value;
            }
        }

        private static string m_InitialDirectory = "";

        public string InitialDirectory
        {
            get { return m_InitialDirectory; }
            set
            {
                if (m_InitialDirectory == value)
                    return;
                m_InitialDirectory = value;
            }
        }

        private static string m_Key = "";

        public string Key
        {
            get { return m_Key; }
            set
            {
                if (m_Key == value)
                    return;
                m_Key = value;
            }
        }

        private static string m_License = "";

        public string License
        {
            get { return m_License; }
            set
            {
                if (m_License == value)
                    return;
                m_License = value;
            }
        }

        private static string m_Output_Format = "";

        public string Output_Format
        {
            get { return m_Output_Format; }
            set
            {
                if (m_Output_Format == value)
                    return;
                m_Output_Format = value;
            }
        }

        private static string m_PassCode = "";

        public string PassCode
        {
            get { return m_PassCode; }
            set
            {
                if (m_PassCode == value)
                    return;
                m_PassCode = value;
            }
        }

        private static string m_Profession = "";

        public string Profession
        {
            get { return m_Profession; }
            set
            {
                if (m_Profession == value)
                    return;
                m_Profession = value;
            }
        }

        private static string m_Replace = "";

        public string Replace
        {
            get { return m_Replace; }
            set
            {
                if (m_Replace == value)
                    return;
                m_Replace = value;
            }
        }

        private static string m_Search = "";

        public string Search
        {
            get { return m_Search; }
            set
            {
                if (m_Search == value)
                    return;
                m_Search = value;
            }
        }

        private static string m_StampAffix = "";

        public string StampAffix
        {
            get { return m_StampAffix; }
            set
            {
                if (m_StampAffix == value)
                    return;
                m_StampAffix = value;
            }
        }

        private static string m_StampDateNotCode = "";

        public string StampDateNotCode
        {
            get { return m_StampDateNotCode; }
            set
            {
                if (m_StampDateNotCode == value)
                    return;
                m_StampDateNotCode = value;
            }
        }

        private static string m_StampSearchText = "";

        public string StampSearchText
        {
            get { return m_StampSearchText; }
            set
            {
                if (m_StampSearchText == value)
                    return;
                m_StampSearchText = value;
            }
        }

        public string mTitle = "";

        public string Title
        {
            get { return mTitle; }
            set
            {
                if (mTitle == value)
                    return;
                mTitle = value;
            }
        }

        public string mAuthor = "";

        public string Author
        {
            get { return mAuthor; }
            set
            {
                if (mAuthor == value)
                    return;
                mAuthor = value;
            }
        }

        public string mSubject = "";

        public string Subject
        {
            get { return mSubject; }
            set
            {
                if (mSubject == value)
                    return;
                mSubject = value;
            }
        }

        public string mKeywords = "";

        public string Keywords
        {
            get { return mKeywords; }
            set
            {
                if (mKeywords == value)
                    return;
                mKeywords = value;
            }
        }

        public string mComments = "";

        public string Comments
        {
            get { return mComments; }
            set
            {
                if (mComments == value)
                    return;
                mComments = value;
            }
        }

        public string mEnableComments = "";

        public string EnableComments
        {
            get { return mEnableComments; }
            set
            {
                if (mEnableComments == value)
                    return;
                mEnableComments = value;
            }
        }

        public string mCusLicense = "";

        public string CusLicense
        {
            get { return mCusLicense; }
            set
            {
                if (mCusLicense == value)
                    return;
                mCusLicense = value;
            }
        }

        public string mCusTitle = "";

        public string CusTitle
        {
            get { return mCusTitle; }
            set
            {
                if (mCusTitle == value)
                    return;
                mCusTitle = value;
            }
        }

        public string mCusComputerID = "";

        public string CusComputerID
        {
            get { return mCusComputerID; }
            set
            {
                if (mCusComputerID == value)
                    return;
                mCusComputerID = value;
            }
        }

        public string mCusMACID = "";

        public string CusMACID
        {
            get { return mCusMACID; }
            set
            {
                if (mCusMACID == value)
                    return;
                mCusMACID = value;
            }
        }

        public string mCusElectronicSig = "";

        public string CusElectronicSig
        {
            get { return mCusElectronicSig; }
            set
            {
                if (mCusElectronicSig == value)
                    return;
                mCusElectronicSig = value;
            }
        }

        public string mCusBusInfo = "";

        public string CusBusInfo
        {
            get { return mCusBusInfo; }
            set
            {
                if (mCusBusInfo == value)
                    return;
                mCusBusInfo = value;
            }
        }

        public string mCusCompanyName = "";

        public string CusCompanyName
        {
            get { return mCusCompanyName; }
            set
            {
                if (mCusCompanyName == value)
                    return;
                mCusCompanyName = value;
            }
        }

        public string mCusAddress = "";

        public string CusAddress
        {
            get { return mCusAddress; }
            set
            {
                if (mCusAddress == value)
                    return;
                mCusAddress = value;
            }
        }

        public string mCusIndividName = "";

        public string CusIndividName
        {
            get { return mCusIndividName; }
            set
            {
                if (mCusIndividName == value)
                    return;
                mCusIndividName = value;
            }
        }

        public string mCusProfession = "";

        public string CusProfession
        {
            get { return mCusProfession; }
            set
            {
                if (mCusProfession == value)
                    return;
                mCusProfession = value;
            }
        }

        public string mBlkLPadding = "";

        public string BlkLPadding
        {
            get { return mBlkLPadding; }
            set { mBlkLPadding = value; }
        }

        public string mPasswordUseAlpha = "";

        public string PasswordUseAlpha
        {
            get { return mPasswordUseAlpha; }
            set { mPasswordUseAlpha = value; }
        }

        public string mPasswordLevel = "";

        public string PasswordLevel
        {
            get { return mPasswordLevel; }
            set { mPasswordLevel = value; }
        }

        public string mIsNoBlock = "";

        public string IsNoBlock
        {
            get { return mIsNoBlock; }
            set { mIsNoBlock = value; }
        }

        public string mSvTolerance = "";

        public string SvTolerance
        {
            get { return mSvTolerance; }
            set { mSvTolerance = value; }
        }

        #endregion
    }
}