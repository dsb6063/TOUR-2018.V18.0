//
// (C) Copyright 1990-2011 by Autodesk, Inc.
//
//
//
// By using this code, you are agreeing to the terms
// and conditions of the License Agreement that appeared
// and was accepted upon download or installation
// (or in connection with the download or installation)
// of the Autodesk software in which this code is included.
// All permissions on use of this code are as set forth
// in such License Agreement provided that the above copyright
// notice appears in all authorized copies and that both that
// copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting 
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to 
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Colors;

namespace Autodesk.Consulting.AutoCAD.Utilities
{
    class TestingCommands
    {

        public Database Database
        {
            get
            {
                return Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            }
        }

        public Transaction Transaction
        {
            get
            {
                return this.Database.TransactionManager.StartTransaction();
            }
        }

        public void CommitAndDispose()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Commit();
                this.Transaction.Dispose();
            }
        }

        [CommandMethod("CreateLayerNoTrans")]
        public ObjectId CreateLayerNoTrans()
        {
            // create the layer with no transaction
            //
            ObjectId id = LayerManager.CreateLayer(this.Database, "NewLayerName");

            Debug.Assert(id.IsValid);

            return id;
        }

        [CommandMethod("CreateLayerTrans")]
        public ObjectId CreateLayerTrans()
        {
            // create the layer with a transaction
            ObjectId id = CreateLayerTrans(true);

            Debug.Assert(id.IsValid);

            return id;
        }

        
        public ObjectId CreateLayerTrans(bool commit)
        {
            // create the layer with a transaction
            ObjectId id = LayerManager.CreateLayer(this.Database, this.Transaction, "NewLayerName");
            if (commit)
            {
                CommitAndDispose();
            }
            Debug.Assert(id.IsValid);

            return id;
        }

        [CommandMethod("DeleteLayerNoTrans")]
        public void DeleteLayerNoTrans()
        {
            // create the layer with a transaction
            LayerManager.DeleteLayer(this.Database, "NewLayerName");
            CommitAndDispose();
        }

        [CommandMethod("DeleteLayerTrans")]
        public void DeleteLayerTrans()
        {
            CreateLayerTrans(false);

            // create the layer with a transaction
            LayerManager.DeleteLayer(this.Database, this.Transaction, "NewLayerName");
            
            try
            {
                if (LayerManager.GetLayerId(this.Database, "NewLayerName").IsValid)
                {
                    Debug.Assert(false);
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                if (ex.ErrorStatus != ErrorStatus.InvalidLayer)
                {
                    Debug.Assert(false);
                }
            }
        }

        public void LockLayer()
        {
            ObjectId id = CreateLayerNoTrans();
            LayerTableRecord layerTableRecord = this.Transaction.GetObject(id, OpenMode.ForWrite) as LayerTableRecord;
            layerTableRecord.IsLocked = true;

            Debug.Assert(LayerManager.IsLocked(this.Database, layerTableRecord.Name));

            Debug.Assert(layerTableRecord.IsLocked);
            layerTableRecord.Dispose();   
        }
    }
}
