using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
//using Common.Logging;


namespace Pge.Common.Framework
{
    public static class FileUtilities
    {
        //private static readonly ILog //_logger = LogManager.GetLogger("FileUtilites");

        /// <summary>
        /// Cleans the folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        public static void CleanFolder(string folder)
        {
            try
            {
                PGA.MessengerManager.MessengerManager.AddLog("CleanFolder called...");

                DirectoryInfo info = new DirectoryInfo(folder);
                DirectoryInfo[] infos = info.GetDirectories();
                foreach (DirectoryInfo di in infos)
                {
                    di.Delete(true);
                }

                foreach (FileInfo fi in info.GetFiles())
                {
                    fi.Delete();
                }
                
                PGA.MessengerManager.MessengerManager.AddLog("CleanFolder finished");
            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error caught in CleanFolders", ex);
                throw;
            }
        }

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The path of the file</param>
        /// <returns>True if the file exists, false if not.</returns>
        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }
               
        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The path of the file</param>
        /// <returns>True if the file exists, false if not.</returns>
        public static bool DirectoryExist(string path)
        {
            return Directory.Exists(path);
        }        

        /// <summary>
        /// Deletes the specified file
        /// </summary>
        /// <param name="path">The path of the file to delete</param>
        /// <returns></returns>
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void DeleteDirectory(string path)
        {
            PGA.MessengerManager.MessengerManager.AddLog("DeleteDirectory called..");
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                PGA.MessengerManager.MessengerManager.AddLog("Deleted the directory: " + path);
            }
        }

        /// <summary>
        /// Copies all files in the the directory
        /// </summary>
        /// <param name="sourceFileName">The source file</param>
        /// <param name="destFileName">The destination file</param>
        /// <returns>true if the copy succeeded, false otherwise.</returns>
        public static bool CopyDirectory(string source, string destination, bool includeSubdirectories)
        {
            try
            {
                SearchOption searchOptions = SearchOption.AllDirectories;

                if (includeSubdirectories)
                {
                    searchOptions = SearchOption.TopDirectoryOnly;
                }

                string[] files = Directory.GetFiles(source, "*.*", searchOptions);
                foreach (string fileName in files)
                {
                    string destinationFileName = Path.Combine(destination, Path.GetFileName(fileName));

                    if (!Directory.Exists(destination))
                    {
                        Directory.CreateDirectory(destination);
                    }
                    File.Copy(fileName, destinationFileName);
                }
                return true;
            }
            catch (System.Exception ex)
            {
               PGA.MessengerManager.MessengerManager.AddLog("CopyDirectory Directory failed", ex);
                return false;
            }
        }

        /// <summary>
        /// Copies the sourceFileName to the destFileName, Note that the full path must exist
        /// </summary>
        /// <param name="sourceFileName">The source file</param>
        /// <param name="destFileName">The destination file</param>
        /// <returns>true if the copy succeeded, false otherwise.</returns>
        public static bool CopyFile(string sourceFileName, string destFileName)
        {
            try
            {
                
                PGA.MessengerManager.MessengerManager.AddLog("Copyfile called with arguements: " + sourceFileName + destFileName);                 
                File.Copy(sourceFileName, destFileName, true);
                return true;
            }
            catch (System.IO.DirectoryNotFoundException dnf)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Directory not found.", dnf);
                return false;
            }
            catch (UnauthorizedAccessException uae)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Unauthorized Access Exception.", uae);
                return false;
            }
            catch (NotSupportedException nse)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Not Supported Exception.", nse);
                return false;
            }
            catch (System.IO.PathTooLongException ptle)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Unauthorized Access Exception.", ptle);
                throw;
            }
            catch (ArgumentNullException ane)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Unauthorized Access Exception.", ane);
                throw;
            }
            catch (System.IO.FileNotFoundException fnfe)
            {
                PGA.MessengerManager.MessengerManager.AddLog("File Not FoundException.", fnfe);
                return false;
            }        
        }

        /// <summary>
        /// Serializes the object to the given filename
        /// </summary>
        /// <param name="o">The Object to serialize</param>
        /// <param name="fileName">The File to save to</param>
        public static void SerializeToFile(object o, string fileName)
        {
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Create);
                using (TextWriter textWriter = new StreamWriter(fs, new UTF8Encoding()))
                {
                    XmlSerializer serializer = new XmlSerializer(o.GetType());
                    serializer.Serialize(textWriter, o);
                    textWriter.Close();
                }
            }
            catch (System.UnauthorizedAccessException uaae)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error creating the file " + fileName, uaae);
            }
            catch (System.Security.SecurityException sse)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error creating the file " + fileName, sse);
            }
            catch (System.ArgumentException ae)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error creating the file " + fileName, ae);
            }

            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error creating the file " + fileName, ex);
            }
        }

        /// <summary>
        ///     This creates the object representation of the specified XML
        /// </summary>
        /// <param name="xml" type="string">
        ///     <para>
        ///         The XML
        ///     </para>
        /// </param>
        /// <param name="theType" type="System.Type">
        ///     <para>
        ///         The object that represents the XML
        ///     </para>
        /// </param>
        /// <returns>
        ///     An instance of theType parameter
        /// </returns>
        public static object DeSerialize(string xml, Type theType)
        {
            object ret = null;
            if (xml.Length > 0)
            {
                using (StringReader myStringReader = new StringReader(xml))
                {
                    XmlTextReader reader = new XmlTextReader(myStringReader);
                    XmlSerializer serializer = new XmlSerializer(theType);
                    ret = serializer.Deserialize(reader);
                    reader.Close();
                }
            }

            return ret;
        }

        /// <summary>
        /// Deletes the specified file
        /// </summary>
        /// <param name="path">The path of the file to delete</param>
        /// <returns></returns>
        public static int GetSubFolderCount(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            return info.GetDirectories().Length;
        }

        /// <summary>
        /// Loads the XML.
        /// </summary>
        /// <param name="xmlFilename">The XML filename.</param>
        /// <param name="classFullname">The class fullname.</param>
        /// <returns></returns>
        public static object LoadXML(string xmlFilename, object classObject)
        {   
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                Type thisType = classObject.GetType();
                xmlDoc.Load(xmlFilename);

                XmlSerializer serializer = new XmlSerializer(thisType);
                using (StringReader stringReader = new StringReader(xmlDoc.InnerXml))
                {
                    XmlTextReader xmlTextReader = null;
                    try
                    {
                        xmlTextReader = new XmlTextReader(stringReader);

                        // Deserialize the user object
                        classObject = serializer.Deserialize(xmlTextReader);

                        //Close the stream.
                        xmlTextReader.Close();
                    }
                    catch
                    {
                        xmlTextReader.Close();
                        return null;
                    }
                }
            }
            catch
            {
                //_logger.Info("Error loading Config file...");
                throw;
            }

            xmlDoc = null;
            return classObject;
        }

        /// <summary>
        /// Loads the XML.
        /// </summary>
        /// <param name="xmlFilename">The XML filename.</param>
        /// <param name="classObject">The class object.</param>
        /// <param name="supportingClassObjects">The supporting class objects.</param>
        /// <returns></returns>
        public static object LoadXML(string xmlFilename, object classObject, Type[] supportingClassObjects)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                Type thisType = classObject.GetType();
                xmlDoc.Load(xmlFilename);

                //XmlSerializer serializer = new XmlSerializer(thisType, new Type[] { typeof(RangeTableBlockMapping) });

                XmlSerializer serializer = new XmlSerializer(thisType, supportingClassObjects);
                using (StringReader stringReader = new StringReader(xmlDoc.InnerXml))
                {
                    XmlTextReader xmlTextReader = null;
                    try
                    {
                        xmlTextReader = new XmlTextReader(stringReader);

                        // Deserialize the user object
                        classObject = serializer.Deserialize(xmlTextReader);

                        //Close the stream.
                        xmlTextReader.Close();
                    }
                    catch
                    {
                        xmlTextReader.Close();
                        return null;
                    }
                }
            }
            catch
            {
                //_logger.Info("Error loading Config file...");
                throw;
            }

            xmlDoc = null;
            return classObject;
        }



        /// <summary>
        ///     This creates the object representation of the specified XML
        /// </summary>
        /// <param name="fileName" type="string">
        ///     <para>
        ///         The xml file
        ///     </para>
        /// </param>
        /// <param name="theType" type="System.Type">
        ///     <para>
        ///         The object that represents the XML
        ///     </para>
        /// </param>
        /// <returns>
        ///     An instance of theType parameter
        /// </returns>
        ///
        public static object DeSerializeFromFile(string fileName, Type theType)
        {
            object ret = null;
            try
            {
                if (File.Exists(fileName))
                {
                    using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        XmlSerializer serializer = new XmlSerializer(theType);
                        ret = serializer.Deserialize(stream);
                        stream.Close();
                    }
                }

                return ret;
            }
            catch (System.UnauthorizedAccessException uaaex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Unauthorized Access Exception.", uaaex);
                throw;
            }
        }
       
        /// <summary>
        /// Returns a list of files based on the provided search pattern
        /// </summary>
        /// <param name="path">The directory to search</param>
        /// <param name="searchPattern">The search string to match against the names of files in path.</param>
        /// <param name="searchSubFolders">Determines if the method will search sub directories</param>
        /// <returns>A String array containing the names of files in the specified directory that
        ///  match the specified search pattern. File names include the full path.
        ///</returns>
        public static string[] GetFileList(string path, string searchPattern, bool searchSubFolders)
        {
            try
            {
                string[] results;
                if (searchSubFolders)
                {
                    results = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    results = Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
                }

                return results;
            }
            catch { throw; }
        }

        /// <summary>
        /// Gets a file type based on extension
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileType(string path)
        {
            try
            {
                string fileType = "OTHER";
                string extension = Path.GetExtension(path);
                switch (extension.ToUpper())
                {
                    case ".DWG":
                        if ( path.Contains("WIP"))
                        {
                            fileType = "WIP";
                        }
                        else
                        {
                            fileType = "VECTOR";
                        }                        
                        break;
                    case ".TIF":
                        fileType = "RASTER";
                        break;
                    case ".TFW":
                        fileType = "TFW";
                        break;

                }
                return fileType;
            }
            catch { throw; }
        }

        /// <summary>
        /// Combines two path strings.
        /// </summary>
        /// <param name="path1">First path</param>
        /// <param name="path2">Second path</param>
        /// <returns></returns>
        public static string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2);            
        }
        
        /// <summary>
        /// Creates a directory
        /// </summary>
        /// <param name="path">The path of the directory to create.</param>
        public static DirectoryInfo CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                PGA.MessengerManager.MessengerManager.AddLog("Creating directory: " + path);
                return Directory.CreateDirectory(path);
            }
            else
            {
                return new DirectoryInfo(path);
            }
        }

        /// <summary>
        /// Reads the given file and returns the list of filename
        /// </summary>
        /// <param name="fileName">The file to read</param>
        /// <returns></returns>
        public static IList<string> ReadDrawingListFile(string fileName)
        {           
            IList<string> lines = new List<string>();

            try
            {
                PGA.MessengerManager.MessengerManager.AddLog("Reading DrawingList File: " + fileName);

                using (TextReader reader = new StreamReader(fileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains(".dwg"))
                        {
                            lines.Add(line.Replace(".dwg", String.Empty));
                        }
                    }
                    reader.Close();
                }
            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error reading the DrawingList file: " + fileName, ex);
            }

            return lines;
        }


        /// <summary>
        /// Returns the file name with or without the extension
        /// </summary>
        /// <param name="withExtension">True to return the filename with extension, false otherwise</param>
        public static string GetFileName(string path, bool withExtension)
        {
            if ( withExtension )
            {
                return Path.GetFileName(path);
            }
            else
            {
                return Path.GetFileNameWithoutExtension(path);
            }
        }

        /// <summary>
        /// Returns the directory information for the file path
        /// </summary>        
        public static string GetDirectoryPath(string path)
        {
            return Path.GetDirectoryName(path);
        }
    }
}
