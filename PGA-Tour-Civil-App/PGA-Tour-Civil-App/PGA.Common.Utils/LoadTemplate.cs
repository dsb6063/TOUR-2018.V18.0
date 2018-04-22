#region

using System;
using System.IO;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

#endregion

namespace PGA.Autodesk.Utils
{
    public static class LoadTemplate
    {
        public static void LoadPGATemplate(string path)
        {
        

            CombineBlocksIntoLibrary(path);
        }

        public static void LoadByCommandPGATemplate()
        {
            string cmd;
            try
            {
                var path = PGA.Database.GetDataBasePath.GetTemplatePath(String.Empty);

             

                cmd = @"_.INSERT " + path + '\r' + '\n';
                var doc = Application.DocumentManager.MdiActiveDocument;
                doc.SendStringToExecute(cmd, true, false, false);
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error", ex);
            }
        }

        private static void CombineBlocksIntoLibrary()

        {
            var doc =
                Application.DocumentManager.MdiActiveDocument;

            var ed = doc.Editor;

            var destDb = doc.Database;


            //Get name of folder from which to load and import blocks


            var pr =
                ed.GetString("\nEnter the folder of source drawings: ");


            if (pr.Status != PromptStatus.OK)

                return;

            var pathName = pr.StringResult;


            // Check the folder exists


            if (!Directory.Exists(pathName))

            {
                ed.WriteMessage(
                    "\nDirectory does not exist: {0}", pathName
                );

                return;
            }


            // Get the names of our DWG files in that folder


            var fileNames = Directory.GetFiles(pathName, "*.dwg");


            // A counter for the files we've imported


            int imported = 0, failed = 0;


            // For each file in our list


            foreach (var fileName in fileNames)

            {
                // Double-check we have a DWG file (probably unnecessary)


                if (fileName.EndsWith(
                        ".dwg",
                        StringComparison.InvariantCultureIgnoreCase
                    )
                )

                {
                    // Catch exceptions at the file level to allow skipping


                    try

                    {
                        // Suggestion from Thorsten Meinecke...


                        var destName =
                            SymbolUtilityServices.GetSymbolNameFromPathName(
                                fileName, "dwg"
                            );


                        // And from Dan Glassman...


                        destName =
                            SymbolUtilityServices.RepairSymbolName(
                                destName, false
                            );


                        // Create a source database to load the DWG into


                        using (var db = new global::Autodesk.AutoCAD.DatabaseServices.Database(false, true))

                        {
                            // Read the DWG into our side database


                            db.ReadDwgFile(fileName, FileShare.Read, true, "");

                            var isAnno = db.AnnotativeDwg;


                            // Insert it into the destination database as

                            // a named block definition


                            var btrId = destDb.Insert(
                                destName,
                                db,
                                false
                            );


                            if (isAnno)

                            {
                                // If an annotative block, open the resultant BTR

                                // and set its annotative definition status


                                var tr =
                                    destDb.TransactionManager.StartTransaction();

                                using (tr)

                                {
                                    var btr =
                                        (BlockTableRecord) tr.GetObject(
                                            btrId,
                                            OpenMode.ForWrite
                                        );

                                    btr.Annotative = AnnotativeStates.True;

                                    tr.Commit();
                                }
                            }


                            // Print message and increment imported block counter


                            ed.WriteMessage("\nImported from \"{0}\".", fileName);

                            imported++;
                        }
                    }

                    catch (Exception ex)

                    {
                        ed.WriteMessage(
                            "\nProblem importing \"{0}\": {1} - file skipped.",
                            fileName, ex.Message
                        );

                        failed++;
                    }
                }
            }


            ed.WriteMessage(
                "\nImported block definitions from {0} files{1} in " +
                "\"{2}\" into the current drawing.",
                imported,
                failed > 0 ? " (" + failed + " failed)" : "",
                pathName
            );
        }

        private static void CombineBlocksIntoLibrary(string filename)
        {
            var doc =
                Application.DocumentManager.MdiActiveDocument;

            var ed = doc.Editor;

            var destDb = doc.Database;


            var pathName = Path.GetDirectoryName(filename);


            // Check the folder exists


            if (!Directory.Exists(pathName))
            {
                ed.WriteMessage(
                    "\nDirectory does not exist: {0}", pathName
                );

                return;
            }


            // Get the names of our DWG files in that folder


            var fileNames = Directory.GetFiles(pathName, "*.dwg");


            // A counter for the files we've imported


            int imported = 0, failed = 0;


            // For each file in our list


            foreach (var fileName in fileNames)
            {
                // Double-check we have a DWG file (probably unnecessary)


                if (fileName.EndsWith(
                        ".dwg",
                        StringComparison.InvariantCultureIgnoreCase
                    )
                )
                {
                    // Catch exceptions at the file level to allow skipping


                    try
                    {
                        // Suggestion from Thorsten Meinecke...

                        using (var docloc = doc.LockDocument())
                        {
                            var destName =
                                SymbolUtilityServices.GetSymbolNameFromPathName(
                                    fileName, "dwg"
                                );


                            // And from Dan Glassman...


                            destName =
                                SymbolUtilityServices.RepairSymbolName(
                                    destName, false
                                );


                            // Create a source database to load the DWG into


                            using (var db = new global::Autodesk.AutoCAD.DatabaseServices.Database(false, true))
                            {
                                // Read the DWG into our side database


                                db.ReadDwgFile(fileName, FileShare.Read, true, "");

                                var isAnno = db.AnnotativeDwg;


                                // Insert it into the destination database as

                                // a named block definition

                                try
                                {
                                    var btrId = destDb.Insert(
                                        destName,
                                        db,
                                        false
                                    );


                                    if (isAnno)
                                    {
                                        // If an annotative block, open the resultant BTR

                                        // and set its annotative definition status


                                        var tr =
                                            destDb.TransactionManager.StartTransaction();

                                        using (tr)
                                        {
                                            var btr =
                                                (BlockTableRecord) tr.GetObject(
                                                    btrId,
                                                    OpenMode.ForWrite
                                                );

                                            btr.Annotative = AnnotativeStates.True;

                                            tr.Commit();
                                        }
                                    }


                                    // Print message and increment imported block counter


                                    ed.WriteMessage("\nImported from \"{0}\".", fileName);

                                    imported++;
                                }

                                catch (System.Exception ex)
                                {
                                    ed.WriteMessage(
                                        "\nProblem importing \"{0}\": {1} - file skipped.",
                                        fileName, ex.Message
                                    );
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ed.WriteMessage(
                            "\nProblem importing \"{0}\": {1} - file skipped.",
                            fileName, ex.Message
                        );

                        failed++;
                    }
                }
            }


            ed.WriteMessage(
                "\nImported block definitions from {0} files{1} in " +
                "\"{2}\" into the current drawing.",
                imported,
                failed > 0 ? " (" + failed + " failed)" : "",
                pathName
            );
        }
    }
}