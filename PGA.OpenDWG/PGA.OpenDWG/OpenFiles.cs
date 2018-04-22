using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using PGA.Database;
using System;
using System.IO;
using ACADDB = Autodesk.AutoCAD.DatabaseServices;
using COMS = PGA.MessengerManager;
using Process = ProcessPolylines.ProcessPolylines;

[assembly: ExtensionApplication(typeof(PGA.OpenDWG.OpenDWG))]
[assembly: CommandClass(typeof(PGA.OpenDWG.OpenDWG))]
namespace PGA.OpenDWG
{

    public  class OpenDWG : IExtensionApplication
    {


        public void Initialize()
        {

        }

        public void Terminate()
        {

        }

        //[CommandMethod("DWG_Open")]
        public static void OpenDwgForReadWrite(string fileName)
        {

            Document doc =
                Application.DocumentManager.MdiActiveDocument;


            if (!System.IO.File.Exists(fileName))
                throw new FileNotFoundException(fileName);

            // Create a database and try to load the file
            ACADDB.Database db = new ACADDB.Database(false, true);
            using (db)
            {
                try
                {
                    db.ReadDwgFile(
                        fileName,
                        System.IO.FileShare.ReadWrite,
                        false,
                        ""
                        );

                    var wdb = ACADDB.HostApplicationServices.WorkingDatabase;
                    ACADDB.HostApplicationServices.WorkingDatabase = db;

                    // Purge unused DGN linestyles from the drawing
                    // (returns false if nothing is erased)
                    ACADDB.ObjectIdCollection collection = Process.GetIdsByTypeTypeValue(
                        "POLYLINE", "LWPOLYLINE", "POLYLINE2D");

                    Process.CopyPolylinesBetweenDatabases(db, collection);


                    // Still need to reset the working database

                    ACADDB.HostApplicationServices.WorkingDatabase = wdb;
 

                }
                catch (System.Exception)
                {
                    COMS.MessengerManager.AddLog(String.Format(
                        "Unable to read drawing file.",
                        db.FingerprintGuid.ToString())
                        );
                    return;
                }
            }

            Application.DocumentManager.MdiActiveDocument = doc;
        }

        //[CommandMethod("DWG_Open")]
            public static
            void OpenDwgFromDatabaseLink(string fileName)
        {


            Document doc =
                Application.DocumentManager.MdiActiveDocument;


            if (!System.IO.File.Exists(fileName))
                throw new FileNotFoundException(fileName);

            // Create a database and try to load the file
            ACADDB.Database db = new ACADDB.Database(false, true);
            using (db)
            {
                try
                {
                    db.ReadDwgFile(
                        fileName,
                        System.IO.FileShare.Read,
                        false,
                        ""
                        );
                }
                catch (System.Exception)
                {
                    COMS.MessengerManager.AddLog(String.Format(
                        "Unable to read drawing file.",
                        db.FingerprintGuid.ToString())
                        );
                    return;
                }

                try
                {
                    ACADDB.Transaction tr =
                        db.TransactionManager.StartTransaction();
                    using (tr)
                    {
                        // Open the blocktable, get the modelspace
                        ACADDB.BlockTable bt =
                            (ACADDB.BlockTable) tr.GetObject(
                                db.BlockTableId,
                                ACADDB.OpenMode.ForRead
                                );

                        ACADDB.BlockTableRecord btr =
                            (ACADDB.BlockTableRecord) tr.GetObject(
                                bt[ACADDB.BlockTableRecord.ModelSpace],
                                ACADDB.OpenMode.ForRead
                                );


                        // Iterate through it, dumping objects
                        foreach (ACADDB.ObjectId objId in btr)
                        {
                            ACADDB.Entity ent =
                                (ACADDB.Entity) tr.GetObject(
                                    objId,
                                    ACADDB.OpenMode.ForRead
                                    );

                            // Let's get rid of the standard namespace
                            const string prefix =
                                "Autodesk.AutoCAD.DatabaseServices.";
                            string typeString =
                                ent.GetType().ToString();
                            if (typeString.Contains(prefix))
                                typeString =
                                    typeString.Substring(prefix.Length);

                            //ed.WriteMessage(
                            //    "\nEntity " +
                            //    ent.ObjectId.ToString() +
                            //    " of type " +
                            //    typeString +
                            //    " found on layer " +
                            //    ent.Layer +
                            //    " with colour " +
                            //    ent.Color.ToString()
                            //    );
                        }
                    }
                }
                catch (System.Exception)
                {
                    COMS.MessengerManager.AddLog(String.Format(
                        "Unable to read drawing file.",
                        db.FingerprintGuid.ToString())
                        );
                    return;
                }


            }
        }
 

        public string CurrentDrawingToProcess()
        {
           DatabaseCommands dbCommands = new DatabaseCommands();
           return dbCommands.GetCurrentDwgToProcess();
               
        }
             
    }
}