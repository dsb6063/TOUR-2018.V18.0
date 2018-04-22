#region

using System.Collections;
using System.Collections.Generic;
using System.IO;
using global::Autodesk.AutoCAD.DatabaseServices;
using global::Autodesk.AutoCAD.Runtime;
using Exception = System.Exception;

#endregion

//using Common.Logging;

namespace BBC.Common.AutoCAD
{
    public class RasterManager
    {
        //private static readonly ILog _logger = LogManager.GetLogger(typeof(RasterManager));

        const string _aecImageInsertWorldFileCorrelationSource = "World File";
        const string rasterStudioArxPath = @"C:\Program Files\AutoCAD Raster Design 2009";

        RasterManager()
        {
            LoadRasterStudioApi(rasterStudioArxPath);
        }

        /// <summary>
        ///     Loads the raster studio API.
        /// </summary>
        public static void LoadRasterStudioApi(string rasterStudioArxPath)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start LoadRasterStudioApi");

            try
            {
                IList<string> aeciArxFiles = new List<string>();
                aeciArxFiles.Add(Path.Combine(rasterStudioArxPath, "AeciUi51.arx"));
                aeciArxFiles.Add(Path.Combine(rasterStudioArxPath, "AeciIbApi51.arx"));

                var isLoaded = false;

                foreach (var arxFileNameFull in aeciArxFiles)
                {
                    var arxFileName = Path.GetFileName(arxFileNameFull);
                    var loadedModules = SystemObjects.DynamicLinker.GetLoadedModules();
                    foreach (var loadedFile in loadedModules)
                    {
                        if (arxFileName.ToUpper() == loadedFile.ToUpper())
                        {
                            isLoaded = true;
                            break;
                        }
                    }
                    if (isLoaded == false)
                    {
                        PGA.MessengerManager.MessengerManager.AddLog("Loading: " + arxFileNameFull);
                        SystemObjects.DynamicLinker.LoadModule(arxFileNameFull, true, true);
                    }
                }
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error in LoadRasterStudioApi", ex);
                throw;
            }
            PGA.MessengerManager.MessengerManager.AddLog("End LoadRasterStudioApi");
        }

        ///// <summary>
        ///// Inserts the raster image using TFW.
        ///// </summary>
        ///// <param name="db">The db.</param>
        ///// <param name="imageFileName">Name of the image file.</param>
        ///// <param name="layerName">Name of the layer.</param>
        ///// <param name="colorIndex">Index of the color.</param>
        ///// <returns></returns>
        //public static ObjectId InsertRasterImageUsingTfw(Database db, string imageFileName, string layerName, int colorIndex)
        //{
        //    // Note that this function references the Raster Studio API.
        //    // The API will not demand load. Raster Studio must be initialized in case it is not loaded at startup
        //    // Reference LoadRasterStudioApi

        //    // TODO: Expect that the trans should not be passed as the COM operation will commit the insert...

        //    PGA.MessengerManager.MessengerManager.AddLog("Start InsertRasterImageUsingTfw");
        //    ObjectId rasterId = new ObjectId();

        //    try
        //    {
        //        string imageFolder = System.IO.Path.GetDirectoryName(imageFileName);
        //        string worldFileName = String.Empty;
        //        worldFileName = System.IO.Path.GetFileNameWithoutExtension(imageFileName);
        //        worldFileName += ".tfw";
        //        worldFileName = System.IO.Path.Combine(imageFolder, worldFileName);

        //        // Verify image file exists
        //        if (!System.IO.File.Exists(imageFileName))
        //        {
        //            return rasterId;
        //        }

        //        // Verify world correlation file exists
        //        if (!System.IO.File.Exists(worldFileName))
        //        {
        //            return rasterId;
        //        }

        //        using (Transaction trans = db.TransactionManager.StartTransaction())
        //        {
        //            // Create new image object
        //            AecImageInsert imageInsert = new AecImageInsert();
        //            // Set image object to reference correlation file
        //            imageInsert.CorrelationSource = _aecImageInsertWorldFileCorrelationSource;
        //            // Insert a new image and get an Object Id back as integer (COM cannot return an ObjectId)
        //            IntPtr oidInteger = (IntPtr)imageInsert.Insert(imageFileName);
        //            // Convert integer oid to full oid
        //            rasterId = new ObjectId(oidInteger);
        //            // Get the new image object from the rasterId
        //            RasterImage rasterImage = trans.GetObject(rasterId, OpenMode.ForRead) as RasterImage;
        //            //RasterImage rasterImage = trans.GetObject(rasterId, OpenMode.ForWrite) as RasterImage;
        //            // Set layer and color
        //            rasterImage.UpgradeOpen();
        //            rasterImage.Layer = layerName;
        //            rasterImage.ColorIndex = colorIndex;
        //            trans.Commit();
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        PGA.MessengerManager.MessengerManager.AddLog("Error in InsertRasterImageUsingTfw", ex);
        //        throw;
        //    }
        //    PGA.MessengerManager.MessengerManager.AddLog("End InsertRasterImageUsingTfw");
        //    return rasterId;
        //}

        /// <summary>
        ///     Sets the raster image clip frame.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="rasterImageId">The raster image id.</param>
        /// <param name="clipPoints">The clip points.</param>
        /// <returns></returns>
        /// <summary>
        ///     Gets all raster image objects.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <returns></returns>
        /// <summary>
        ///     Deletes all raster image objects.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <returns></returns>
        /// <summary>
        ///     Deletes the unreferenced raster image definitions.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <returns></returns>
        public static bool DeleteUnreferencedRasterImageDefinitions(Database db, Transaction trans)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start DeleteUnreferencedRasterImageDefinitions");
            // Get the Image Dictionary
            var imageDictionaryId = RasterImageDef.GetImageDictionary(db);
            if (imageDictionaryId == ObjectId.Null)
            {
                // No images defined?
                PGA.MessengerManager.MessengerManager.AddLog("End DeleteUnreferencedRasterImageDefinitions");
                return true;
            }

            var imageDictionary = (DBDictionary) trans.GetObject(imageDictionaryId, OpenMode.ForRead);
            if (imageDictionary.Count.Equals(0))
            {
                // No images defined?
                PGA.MessengerManager.MessengerManager.AddLog("End DeleteUnreferencedRasterImageDefinitions");
                return true;
            }

            foreach (DictionaryEntry imageDictionaryEntry in imageDictionary)
            {
                var imageName = imageDictionaryEntry.Key as string;
                var rasterImageDefId = (ObjectId) imageDictionaryEntry.Value;
                var rasterImageDef = trans.GetObject(rasterImageDefId, OpenMode.ForRead) as RasterImageDef;
                bool LockedLayers;
                var rasterImageRefCount = rasterImageDef.GetEntityCount(out LockedLayers);
                if (rasterImageRefCount.Equals(0))
                {
                    rasterImageDef.Erase();
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End DeleteUnreferencedRasterImageDefinitions");
            return true;
        }

        /// <summary>
        ///     Strips the name of the path from source file.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="rasterImageName">Name of the raster image.</param>
        /// <returns></returns>
        public static bool StripPathFromSourceFileName(Database db, Transaction trans, string rasterImageName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start DeleteUnreferencedRasterImageDefinitions");
            var rasterImageDefId = GetRasterImageDef(db, trans, rasterImageName);
            if (rasterImageDefId != ObjectId.Null)
            {
                var rasterImageDef = trans.GetObject(rasterImageDefId, OpenMode.ForRead) as RasterImageDef;
                var rasterImageFileNameFull = rasterImageDef.SourceFileName;
                var rasterImageFileName = Path.GetFileNameWithoutExtension(rasterImageFileNameFull);
                if (rasterImageFileName != rasterImageFileNameFull)
                {
                    rasterImageDef.SourceFileName = rasterImageFileName;
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End DeleteUnreferencedRasterImageDefinitions");
            return true;
        }

        /// <summary>
        ///     Gets the raster image def.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="rasterImageName">Name of the raster image.</param>
        /// <returns></returns>
        public static ObjectId GetRasterImageDef(Database db, Transaction trans, string rasterImageName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetRasterImageDef");
            var rasterImageDefId = ObjectId.Null;

            // Get the Image Dictionary
            var imageDictionaryId = RasterImageDef.GetImageDictionary(db);
            if (imageDictionaryId == ObjectId.Null)
            {
                // No images defined?
                return rasterImageDefId;
            }

            var imageDictionary = (DBDictionary) trans.GetObject(imageDictionaryId, OpenMode.ForRead);
            if (imageDictionary.Count.Equals(0))
            {
                // No images defined?
                return rasterImageDefId;
            }

            foreach (DictionaryEntry imageDictionaryEntry in imageDictionary)
            {
                var imageName = imageDictionaryEntry.Key as string;
                if (rasterImageName.ToUpper() == imageName.ToUpper())
                {
                    rasterImageDefId = (ObjectId) imageDictionaryEntry.Value;
                    break;
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End GetRasterImageDef");
            return rasterImageDefId;
        }
    }
}