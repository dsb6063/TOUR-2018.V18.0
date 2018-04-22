using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using PGA.Database;
using ACAD = Autodesk.AutoCAD.ApplicationServices;
using ACADRT = Autodesk.AutoCAD.Runtime;

namespace PGA.DrawingManager
{
    public static class OpenandCloseDwgs
    {
        [ACADRT.CommandMethodAttribute("PGA-MANAGEDWGS", ACADRT.CommandFlags.Session)]
        public static void OpenDrawingManager()
        {
            try
            {
                DatabaseCommands commands = new DatabaseCommands();
                var acDocMgr = ACAD.Application.DocumentManager;
                Document acDoc = ACAD.Application.DocumentManager.MdiActiveDocument;
                Document acNewDoc = null;
                if (acDocMgr.Count > 1)
                {
                    using (acDocMgr.MdiActiveDocument.LockDocument())
                    {
                        foreach (Document docs in acDocMgr)
                        {
                            acDocMgr.MdiActiveDocument = docs;
                            docs.CloseAndDiscard();
                        }
                    }
                }

                if (acDocMgr.Count == 1)
                {

                    acNewDoc =
                        acDocMgr.Add(commands.GetTemplatePath());
                    using (acDocMgr.MdiActiveDocument.LockDocument())
                    {
                        acDocMgr.MdiActiveDocument = acDoc;
                        acDocMgr.CurrentDocument.CloseAndDiscard();
                        acDocMgr.MdiActiveDocument = acNewDoc;

                    }

                }
            }
            catch (System.Exception ex)
            {
                PGA.Database.DatabaseLogs.FormatLogs("OpenDrawingManager: " + ex.Message);
            }

        }

        [CommandMethod("PGA-CD", CommandFlags.Session)]
        public static void CloseDocuments()
        {

            try
            {

                DocumentCollection docs = ACAD.Application.DocumentManager;
                foreach (Document doc in docs)
                {
                    // First cancel any running command
                    if (doc.CommandInProgress != "" &&
                        doc.CommandInProgress != "CD")
                    {
                        Document oDoc = ACAD.Application.DocumentManager.MdiActiveDocument;

                        oDoc.SendStringToExecute("\x03\x03", false, false, false);
                    }

                    if (doc.IsReadOnly)
                    {
                        try
                        {
                            doc.CloseAndDiscard();

                        }
                        catch (Exception ex)
                        {
                            PGA.Database.DatabaseLogs.FormatLogs("OpenDrawingManager: " + ex.Message);
                        }
                    }
                    else
                    {
                        // Activate the document, so we can check DBMOD
                        if (docs.MdiActiveDocument != doc)
                        {
                            try
                            {
                                using (ACAD.Application.DocumentManager.MdiActiveDocument.LockDocument())
                                {
                                    docs.MdiActiveDocument = doc;
                                }
                            }
                            catch (System.Exception ex)
                            {
                                PGA.Database.DatabaseLogs.FormatLogs("OpenDrawingManager: " + ex.Message);
                            }
                        }

                        int isModified =
                            System.Convert.ToInt32(
                                ACAD.Application.GetSystemVariable("DBMOD")
                            );

                        // No need to save if not modified
                        if (isModified == 0)
                        {
                            doc.CloseAndDiscard();
                        }
                        else
                        {
                            // This may create documents in strange places
                            doc.CloseAndSave(doc.Name);
                        }
                    }
                }
            }


            catch (System.Exception ex)
            {
                PGA.Database.DatabaseLogs.FormatLogs("ForceCloseDrawings: " + ex.Message);
            }

        }
    }


    public static class Commands
    {
        // If you change the command name, be sure to change
        // the code that checks for it, below.
        //
        public static void CloseDocuments()
        {
            DocumentCollection docs = ACAD.Application.DocumentManager;
            foreach (Document doc in docs)
            {
                // First cancel any running command
                if (doc.CommandInProgress != "" &&
                    doc.CommandInProgress != "CD")
                {
                    Document oDoc = ACAD.Application.DocumentManager.MdiActiveDocument;

                    oDoc.SendStringToExecute("\x03\x03", false, false, false);
                }

                if (doc.IsReadOnly)
                {
                    doc.CloseAndDiscard();
                }
                else
                {
                    // Activate the document, so we can check DBMOD
                    if (docs.MdiActiveDocument != doc)
                    {
                        using (ACAD.Application.DocumentManager.MdiActiveDocument.LockDocument())
                        {
                            docs.MdiActiveDocument = doc;
                        }
                    }
                    int isModified =
                        System.Convert.ToInt32(
                            ACAD.Application.GetSystemVariable("DBMOD")
                            );

                    // No need to save if not modified
                    if (isModified == 0)
                    {
                        doc.CloseAndDiscard();
                    }
                    else
                    {
                        // This may create documents in strange places
                        doc.CloseAndSave(doc.Name);
                    }
                }
            }
        }

        public static void ForceCloseAllButActiveDocuments()
        {
            try
            {
                //Added 3.1.2018 eDocumentSwitch is disabled
                DocumentCollection docs = ACAD.Application.DocumentManager;
                foreach (Document doc in docs)
                {
                  
            
                    if (docs.Count == 1)
                    {
                        return;
                    }
                    // First cancel any running command
                    if (doc.CommandInProgress != "" &&
                        doc.CommandInProgress != "CD")
                    {
                        Document oDoc = ACAD.Application.DocumentManager.MdiActiveDocument;

                        oDoc.SendStringToExecute("\x03\x03", false, false, false);
                    }
                    else
                    {
                        //Activate the document, so we can check DBMOD
                        if (docs.MdiActiveDocument != doc)
                        {
                            using (ACAD.Application.DocumentManager.MdiActiveDocument.LockDocument())
                            {
                                docs.MdiActiveDocument = doc;

                            }
                        }

                        int isModified =
                          System.Convert.ToInt32(
                              ACAD.Application.GetSystemVariable("DBMOD")
                              );

                        if (doc.IsReadOnly)
                        {
                            doc.CloseAndDiscard();
                        }
                        else if (isModified == 0)
                        {
                            doc.CloseAndDiscard();
                        }
                        else
                        {
                            using (doc.LockDocument())
                                doc.CloseAndDiscard();
                        }
                    }

                }
            }
            catch (System.Exception ex)
            {
                PGA.Database.DatabaseLogs.FormatLogs("ForceCloseDrawings: " + ex.Message);
            }
        }


        public static void CloseAllButActiveDocuments()
        {
            DocumentCollection docs = ACAD.Application.DocumentManager;
            foreach (Document doc in docs)
            {   //if (docs.MdiActiveDocument != doc && docs.MdiActiveDocument.Name != doc.Name)

                // Activate the document, so we can check DBMOD
                try
                {
                    if (docs.MdiActiveDocument != doc)
                    {

                        int isModified =
                            System.Convert.ToInt32(
                                ACAD.Application.GetSystemVariable("DBMOD")
                                );

                        // No need to save if not modified
                        if (isModified == 0)
                        {
                            doc.CloseAndDiscard();
                        }
                        else
                        {
                            // This may create documents in strange places
                            doc.CloseAndSave(doc.Name);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    PGA.Database.DatabaseLogs.FormatLogs(ex.Message);
                }
            }
        }
    }

}


