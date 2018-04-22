using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace PGA.AttributeRefManager
{
    public static class AttributeManager
    {
        public static int UpdateAttributesInDatabase(

  Database db,

  string blockName,

  string attbName,

  string attbValue

)

        {

            // Get the IDs of the spaces we want to process

            // and simply call a function to process each

            ObjectId msId, psId;

            Transaction tr =

              db.TransactionManager.StartTransaction();

            using (tr)

            {

                BlockTable bt =

                  (BlockTable)tr.GetObject(

                    db.BlockTableId,

                    OpenMode.ForRead

                  );

                msId =

                  bt[BlockTableRecord.ModelSpace];

                psId =

                  bt[BlockTableRecord.PaperSpace];

                // Not needed, but quicker than aborting

                tr.Commit();

            }

            int msCount =

              UpdateAttributesInBlock(

                db,

                msId,

                blockName,

                attbName,

                attbValue

              );

            int psCount =

              UpdateAttributesInBlock(

                db,

                psId,

                blockName,

                attbName,

                attbValue

              );

            return msCount + psCount;

        }

        public static int UpdateAttributesInBlock(

          Database db,

          ObjectId btrId,

          string blockName,

          string attbName,

          string attbValue

        )

        {

            // Will return the number of attributes modified

            int changedCount = 0;

            Transaction tr =

              db.TransactionManager.StartTransaction();

            using (tr)

            {

                BlockTableRecord btr =

                  (BlockTableRecord)tr.GetObject(

                    btrId,

                    OpenMode.ForRead

                  );

                // Test each entity in the container...

                foreach (ObjectId entId in btr)

                {

                    Entity ent =

                      tr.GetObject(entId, OpenMode.ForRead)

                      as Entity;

                    if (ent != null)

                    {

                        BlockReference br = ent as BlockReference;

                        if (br != null)

                        {

                            BlockTableRecord bd =

                              (BlockTableRecord)tr.GetObject(

                                br.BlockTableRecord,

                                OpenMode.ForRead

                            );

                            // ... to see whether it's a block with

                            // the name we're after
                            Debug.WriteLine(bd.Name);
                            if (bd.Name.ToUpper() == blockName.ToUpper())

                            {

                                // Check each of the attributes...

                                foreach (

                                  ObjectId arId in br.AttributeCollection

                                )

                                {

                                    DBObject obj =

                                      tr.GetObject(

                                        arId,

                                        OpenMode.ForRead

                                      );

                                    AttributeReference ar =

                                      obj as AttributeReference;

                                    if (ar != null)

                                    {
                                        Debug.WriteLine(ar.TextString);
                                        // ... to see whether it has

                                        // the tag we're after

                                        if (ar.Tag.ToUpper() == attbName.ToUpper())

                                        {

                                            // If so, update the value

                                            // and increment the counter

                                            ar.UpgradeOpen();

                                            ar.TextString = attbValue;

                                            ar.DowngradeOpen();

                                            changedCount++;

                                        }

                                    }

                                }

                            }

                            // Recurse for nested blocks

                            changedCount +=

                              UpdateAttributesInBlock(

                                db,

                                br.BlockTableRecord,

                                blockName,

                                attbName,

                                attbValue

                              );

                        }

                    }

                }

                tr.Commit();

            }

            return changedCount;

        }

    }
}

