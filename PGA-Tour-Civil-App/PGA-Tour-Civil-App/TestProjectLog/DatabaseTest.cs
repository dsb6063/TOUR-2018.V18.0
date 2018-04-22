#region

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq.Expressions;
using Autodesk.AutoCAD.DatabaseServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestProjectLog
{
    /// <summary>
    ///     This is a test class for DatabaseTest and is intended
    ///     to contain all DatabaseTest Unit Tests
    /// </summary>
    [TestClass]
    public class DatabaseTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        ///     A test for Database Constructor
        /// </summary>
        [TestMethod]
        public void DatabaseConstructorTest()
        {
            var target = new Database();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///     A test for Database Constructor
        /// </summary>
        [TestMethod]
        [DeploymentItem("acdbmgd.dll")]
        public void DatabaseConstructorTest1()
        {
            var unmanagedPointer = new IntPtr(); // TODO: Initialize to an appropriate value
            var autoDelete = false; // TODO: Initialize to an appropriate value
            var target = new Database_Accessor(unmanagedPointer, autoDelete);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///     A test for Database Constructor
        /// </summary>
        [TestMethod]
        public void DatabaseConstructorTest2()
        {
            var buildDefaultDrawing = false; // TODO: Initialize to an appropriate value
            var noDocument = false; // TODO: Initialize to an appropriate value
            var target = new Database(buildDefaultDrawing, noDocument);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///     A test for AbortDeepClone
        /// </summary>
        [TestMethod]
        public void AbortDeepCloneTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            IdMapping idMap = null; // TODO: Initialize to an appropriate value
            target.AbortDeepClone(idMap);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for AddDBObject
        /// </summary>
        [TestMethod]
        public void AddDBObjectTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            DBObject appendIt = null; // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.AddDBObject(appendIt);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for ApplyPartialOpenFilters
        /// </summary>
        [TestMethod]
        public void ApplyPartialOpenFiltersTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            SpatialFilter spatialFilter = null; // TODO: Initialize to an appropriate value
            LayerFilter layerFilter = null; // TODO: Initialize to an appropriate value
            target.ApplyPartialOpenFilters(spatialFilter, layerFilter);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for AttachXref
        /// </summary>
        [TestMethod]
        public void AttachXrefTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var fileName = string.Empty; // TODO: Initialize to an appropriate value
            var blockName = string.Empty; // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.AttachXref(fileName, blockName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Audit
        /// </summary>
        [TestMethod]
        public void AuditTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            AuditInfo info = null; // TODO: Initialize to an appropriate value
            target.Audit(info);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for AuditXData
        /// </summary>
        [TestMethod]
        public void AuditXDataTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            AuditInfo info = null; // TODO: Initialize to an appropriate value
            target.AuditXData(info);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for BindXrefs
        /// </summary>
        [TestMethod]
        public void BindXrefsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectIdCollection xrefIds = null; // TODO: Initialize to an appropriate value
            var insertBind = false; // TODO: Initialize to an appropriate value
            target.BindXrefs(xrefIds, insertBind);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for ClassDxfName
        /// </summary>
        [TestMethod]
        public void ClassDxfNameTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            RXClass getMyDxfName = null; // TODO: Initialize to an appropriate value
            var expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.ClassDxfName(getMyDxfName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for CloseInput
        /// </summary>
        [TestMethod]
        public void CloseInputTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var closeFile = false; // TODO: Initialize to an appropriate value
            target.CloseInput(closeFile);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for CountEmptyObjects
        /// </summary>
        [TestMethod]
        public void CountEmptyObjectsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var flags = 0; // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.CountEmptyObjects(flags);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for CountHardReferences
        /// </summary>
        [TestMethod]
        public void CountHardReferencesTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectIdCollection ids = null; // TODO: Initialize to an appropriate value
            int[] count = null; // TODO: Initialize to an appropriate value
            target.CountHardReferences(ids, count);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for Create
        /// </summary>
        [TestMethod]
        [DeploymentItem("acdbmgd.dll")]
        public void CreateTest()
        {
            var unmanagedPointer = new IntPtr(); // TODO: Initialize to an appropriate value
            var autoDelete = false; // TODO: Initialize to an appropriate value
            Database expected = null; // TODO: Initialize to an appropriate value
            Database actual;
            actual = Database_Accessor.Create(unmanagedPointer, autoDelete);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for DeepCloneObjects
        /// </summary>
        [TestMethod]
        public void DeepCloneObjectsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectIdCollection identifiers = null; // TODO: Initialize to an appropriate value
            var id = new ObjectId(); // TODO: Initialize to an appropriate value
            IdMapping mapping = null; // TODO: Initialize to an appropriate value
            var deferTranslation = false; // TODO: Initialize to an appropriate value
            target.DeepCloneObjects(identifiers, id, mapping, deferTranslation);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for DetachXref
        /// </summary>
        [TestMethod]
        public void DetachXrefTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var xrefId = new ObjectId(); // TODO: Initialize to an appropriate value
            target.DetachXref(xrefId);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for DisablePartialOpen
        /// </summary>
        [TestMethod]
        public void DisablePartialOpenTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            target.DisablePartialOpen();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for DisableUndoRecording
        /// </summary>
        [TestMethod]
        public void DisableUndoRecordingTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var disable = false; // TODO: Initialize to an appropriate value
            target.DisableUndoRecording(disable);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for DxfIn
        /// </summary>
        [TestMethod]
        public void DxfInTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var fileName = string.Empty; // TODO: Initialize to an appropriate value
            var logFilename = string.Empty; // TODO: Initialize to an appropriate value
            target.DxfIn(fileName, logFilename);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for DxfOut
        /// </summary>
        [TestMethod]
        public void DxfOutTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var fileName = string.Empty; // TODO: Initialize to an appropriate value
            var precision = 0; // TODO: Initialize to an appropriate value
            var saveThumbnailImage = false; // TODO: Initialize to an appropriate value
            target.DxfOut(fileName, precision, saveThumbnailImage);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for DxfOut
        /// </summary>
        [TestMethod]
        public void DxfOutTest1()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var fileName = string.Empty; // TODO: Initialize to an appropriate value
            var precision = 0; // TODO: Initialize to an appropriate value
            DwgVersion version = new DwgVersion(); // TODO: Initialize to an appropriate value
            target.DxfOut(fileName, precision, version);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for EraseEmptyObjects
        /// </summary>
        [TestMethod]
        public void EraseEmptyObjectsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var flags = 0; // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.EraseEmptyObjects(flags);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for EvaluateFields
        /// </summary>
        [TestMethod]
        public void EvaluateFieldsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            FieldEvaluationContext context = new FieldEvaluationContext(); // TODO: Initialize to an appropriate value
            FieldEvaluationResult expected = new FieldEvaluationResult(); // TODO: Initialize to an appropriate value
            FieldEvaluationResult actual;
            actual = target.EvaluateFields(context);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for EvaluateFields
        /// </summary>
        [TestMethod]
        public void EvaluateFieldsTest1()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            FieldEvaluationContext context = new FieldEvaluationContext(); // TODO: Initialize to an appropriate value
            var prop = string.Empty; // TODO: Initialize to an appropriate value
            FieldEvaluationResult expected = new FieldEvaluationResult(); // TODO: Initialize to an appropriate value
            FieldEvaluationResult actual;
            actual = target.EvaluateFields(context, prop);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for EvaluateFields
        /// </summary>
        [TestMethod]
        public void EvaluateFieldsTest2()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            FieldEvaluationResult expected = new FieldEvaluationResult(); // TODO: Initialize to an appropriate value
            FieldEvaluationResult actual;
            actual = target.EvaluateFields();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for ForceWblockDatabaseCopy
        /// </summary>
        [TestMethod]
        public void ForceWblockDatabaseCopyTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            target.ForceWblockDatabaseCopy();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for FromAcadDatabase
        /// </summary>
        [TestMethod]
        public void FromAcadDatabaseTest()
        {
            object acadDatabase = null; // TODO: Initialize to an appropriate value
            Database expected = null; // TODO: Initialize to an appropriate value
            Database actual;
            actual = Database.FromAcadDatabase(acadDatabase);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for GetAllDatabases
        /// </summary>
        [TestMethod]
        public void GetAllDatabasesTest()
        {
            List<Database> expected = null; // TODO: Initialize to an appropriate value
            List<Database> actual;
            actual = Database.GetAllDatabases();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for GetDimRecentStyleList
        /// </summary>
        [TestMethod]
        public void GetDimRecentStyleListTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectIdCollection expected = null; // TODO: Initialize to an appropriate value
            ObjectIdCollection actual;
            actual = target.GetDimRecentStyleList();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for GetDimensionStyleChildData
        /// </summary>
        [TestMethod]
        public void GetDimensionStyleChildDataTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            RXClass classDescriptor = null; // TODO: Initialize to an appropriate value
            DimStyleTableRecord expected = null; // TODO: Initialize to an appropriate value
            DimStyleTableRecord actual;
            actual = target.GetDimensionStyleChildData(classDescriptor);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for GetDimensionStyleChildId
        /// </summary>
        [TestMethod]
        public void GetDimensionStyleChildIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            RXClass classDescriptor = null; // TODO: Initialize to an appropriate value
            var parentStyle = new ObjectId(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.GetDimensionStyleChildId(classDescriptor, parentStyle);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for GetDimensionStyleParentId
        /// </summary>
        [TestMethod]
        public void GetDimensionStyleParentIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var childStyle = new ObjectId(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.GetDimensionStyleParentId(childStyle);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for GetDimstyleData
        /// </summary>
        [TestMethod]
        public void GetDimstyleDataTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            DimStyleTableRecord expected = null; // TODO: Initialize to an appropriate value
            DimStyleTableRecord actual;
            actual = target.GetDimstyleData();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for GetHostDwgXrefGraph
        /// </summary>
        [TestMethod]
        public void GetHostDwgXrefGraphTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var includeGhosts = false; // TODO: Initialize to an appropriate value
            XrefGraph expected = null; // TODO: Initialize to an appropriate value
            XrefGraph actual;
            actual = target.GetHostDwgXrefGraph(includeGhosts);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for GetMetaObject
        /// </summary>
        [TestMethod]
        public void GetMetaObjectTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Expression parameter = null; // TODO: Initialize to an appropriate value
            DynamicMetaObject expected = null; // TODO: Initialize to an appropriate value
            DynamicMetaObject actual;
            actual = target.GetMetaObject(parameter);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for GetNearestLineWeight
        /// </summary>
        [TestMethod]
        public void GetNearestLineWeightTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var weight = 0; // TODO: Initialize to an appropriate value
            LineWeight expected = new LineWeight(); // TODO: Initialize to an appropriate value
            LineWeight actual;
            actual = target.GetNearestLineWeight(weight);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for GetObjectId
        /// </summary>
        [TestMethod]
        public void GetObjectIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var createIfNotFound = false; // TODO: Initialize to an appropriate value
            Handle objHandle = new Handle(); // TODO: Initialize to an appropriate value
            var identifier = 0; // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.GetObjectId(createIfNotFound, objHandle, identifier);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for GetSupportedDxfOutVersions
        /// </summary>
        [TestMethod]
        public void GetSupportedDxfOutVersionsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            DwgVersion[] expected = null; // TODO: Initialize to an appropriate value
            DwgVersion[] actual;
            actual = target.GetSupportedDxfOutVersions();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for GetSupportedSaveVersions
        /// </summary>
        [TestMethod]
        public void GetSupportedSaveVersionsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            DwgVersion[] expected = null; // TODO: Initialize to an appropriate value
            DwgVersion[] actual;
            actual = target.GetSupportedSaveVersions();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for GetViewports
        /// </summary>
        [TestMethod]
        public void GetViewportsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var bGetPaperspaceVports = false; // TODO: Initialize to an appropriate value
            ObjectIdCollection expected = null; // TODO: Initialize to an appropriate value
            ObjectIdCollection actual;
            actual = target.GetViewports(bGetPaperspaceVports);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for GetVisualStyleList
        /// </summary>
        [TestMethod]
        public void GetVisualStyleListTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            StringCollection expected = null; // TODO: Initialize to an appropriate value
            StringCollection actual;
            actual = target.GetVisualStyleList();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Insert
        /// </summary>
        [TestMethod]
        public void InsertTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var blockName = string.Empty; // TODO: Initialize to an appropriate value
            Database dataBase = null; // TODO: Initialize to an appropriate value
            var preserveSourceDatabase = false; // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.Insert(blockName, dataBase, preserveSourceDatabase);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Insert
        /// </summary>
        [TestMethod]
        public void InsertTest1()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Matrix3d transform = new Matrix3d(); // TODO: Initialize to an appropriate value
            Database dataBase = null; // TODO: Initialize to an appropriate value
            var preserveSourceDatabase = false; // TODO: Initialize to an appropriate value
            target.Insert(transform, dataBase, preserveSourceDatabase);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for Insert
        /// </summary>
        [TestMethod]
        public void InsertTest2()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var sourceBlockName = string.Empty; // TODO: Initialize to an appropriate value
            var destinationBlockName = string.Empty; // TODO: Initialize to an appropriate value
            Database dataBase = null; // TODO: Initialize to an appropriate value
            var preserveSourceDatabase = false; // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.Insert(sourceBlockName, destinationBlockName, dataBase, preserveSourceDatabase);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for IsObjectNonPersistent
        /// </summary>
        [TestMethod]
        public void IsObjectNonPersistentTest()
        {
            var id = new ObjectId(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = Database.IsObjectNonPersistent(id);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for IsValidLineWeight
        /// </summary>
        [TestMethod]
        public void IsValidLineWeightTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var weight = 0; // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsValidLineWeight(weight);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LoadLineTypeFile
        /// </summary>
        [TestMethod]
        public void LoadLineTypeFileTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var lineTypeName = string.Empty; // TODO: Initialize to an appropriate value
            var filename = string.Empty; // TODO: Initialize to an appropriate value
            target.LoadLineTypeFile(lineTypeName, filename);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for LoadMlineStyleFile
        /// </summary>
        [TestMethod]
        public void LoadMlineStyleFileTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var mlineStyleName = string.Empty; // TODO: Initialize to an appropriate value
            var fileName = string.Empty; // TODO: Initialize to an appropriate value
            target.LoadMlineStyleFile(mlineStyleName, fileName);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for MarkObjectNonPersistent
        /// </summary>
        [TestMethod]
        public void MarkObjectNonPersistentTest()
        {
            var id = new ObjectId(); // TODO: Initialize to an appropriate value
            var value = false; // TODO: Initialize to an appropriate value
            Database.MarkObjectNonPersistent(id, value);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for OverlayXref
        /// </summary>
        [TestMethod]
        public void OverlayXrefTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var fileName = string.Empty; // TODO: Initialize to an appropriate value
            var blockName = string.Empty; // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.OverlayXref(fileName, blockName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Purge
        /// </summary>
        [TestMethod]
        public void PurgeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectIdGraph idGraph = null; // TODO: Initialize to an appropriate value
            target.Purge(idGraph);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for Purge
        /// </summary>
        [TestMethod]
        public void PurgeTest1()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectIdCollection ids = null; // TODO: Initialize to an appropriate value
            target.Purge(ids);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for ReadDwgFile
        /// </summary>
        [TestMethod]
        public void ReadDwgFileTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var fileName = string.Empty; // TODO: Initialize to an appropriate value
            var fileSharing = new FileShare(); // TODO: Initialize to an appropriate value
            var allowCPConversion = false; // TODO: Initialize to an appropriate value
            var password = string.Empty; // TODO: Initialize to an appropriate value
            target.ReadDwgFile(fileName, fileSharing, allowCPConversion, password);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for ReadDwgFile
        /// </summary>
        [TestMethod]
        public void ReadDwgFileTest1()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var fileName = string.Empty; // TODO: Initialize to an appropriate value
            FileOpenMode mode = new FileOpenMode(); // TODO: Initialize to an appropriate value
            var allowCPConversion = false; // TODO: Initialize to an appropriate value
            var password = string.Empty; // TODO: Initialize to an appropriate value
            target.ReadDwgFile(fileName, mode, allowCPConversion, password);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for ReadDwgFile
        /// </summary>
        [TestMethod]
        public void ReadDwgFileTest2()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var drawingFile = new IntPtr(); // TODO: Initialize to an appropriate value
            var allowCPConversion = false; // TODO: Initialize to an appropriate value
            var password = string.Empty; // TODO: Initialize to an appropriate value
            target.ReadDwgFile(drawingFile, allowCPConversion, password);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for ReclaimMemoryFromErasedObjects
        /// </summary>
        [TestMethod]
        public void ReclaimMemoryFromErasedObjectsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectIdCollection ids = null; // TODO: Initialize to an appropriate value
            target.ReclaimMemoryFromErasedObjects(ids);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for ReloadXrefs
        /// </summary>
        [TestMethod]
        public void ReloadXrefsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectIdCollection xrefIds = null; // TODO: Initialize to an appropriate value
            target.ReloadXrefs(xrefIds);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for ResetTimes
        /// </summary>
        [TestMethod]
        public void ResetTimesTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            target.ResetTimes();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for ResolveXrefs
        /// </summary>
        [TestMethod]
        public void ResolveXrefsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var useThreadEngine = false; // TODO: Initialize to an appropriate value
            var doNewOnly = false; // TODO: Initialize to an appropriate value
            target.ResolveXrefs(useThreadEngine, doNewOnly);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for RestoreForwardingXrefSymbols
        /// </summary>
        [TestMethod]
        public void RestoreForwardingXrefSymbolsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            target.RestoreForwardingXrefSymbols();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for RestoreOriginalXrefSymbols
        /// </summary>
        [TestMethod]
        public void RestoreOriginalXrefSymbolsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            target.RestoreOriginalXrefSymbols();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for Save
        /// </summary>
        [TestMethod]
        public void SaveTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            target.Save();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for SaveAs
        /// </summary>
        [TestMethod]
        public void SaveAsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var fileName = string.Empty; // TODO: Initialize to an appropriate value
            var bBakAndRename = false; // TODO: Initialize to an appropriate value
            DwgVersion version = new DwgVersion(); // TODO: Initialize to an appropriate value
            SecurityParameters security = null; // TODO: Initialize to an appropriate value
            target.SaveAs(fileName, bBakAndRename, version, security);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for SaveAs
        /// </summary>
        [TestMethod]
        public void SaveAsTest1()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var fileName = string.Empty; // TODO: Initialize to an appropriate value
            SecurityParameters security = null; // TODO: Initialize to an appropriate value
            target.SaveAs(fileName, security);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for SaveAs
        /// </summary>
        [TestMethod]
        public void SaveAsTest2()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var fileName = string.Empty; // TODO: Initialize to an appropriate value
            DwgVersion version = new DwgVersion(); // TODO: Initialize to an appropriate value
            target.SaveAs(fileName, version);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for SetDimstyleData
        /// </summary>
        [TestMethod]
        public void SetDimstyleDataTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            DimStyleTableRecord style = null; // TODO: Initialize to an appropriate value
            target.SetDimstyleData(style);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for SetTimeZoneAsUtcOffset
        /// </summary>
        [TestMethod]
        public void SetTimeZoneAsUtcOffsetTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double offset = 0F; // TODO: Initialize to an appropriate value
            var expected = new TimeZone(); // TODO: Initialize to an appropriate value
            TimeZone actual;
            actual = target.SetTimeZoneAsUtcOffset(offset);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for SetWorldPaperspaceUcsBaseOrigin
        /// </summary>
        [TestMethod]
        public void SetWorldPaperspaceUcsBaseOriginTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Point3d origin = new Point3d(); // TODO: Initialize to an appropriate value
            OrthographicView orthoView = new OrthographicView(); // TODO: Initialize to an appropriate value
            target.SetWorldPaperspaceUcsBaseOrigin(origin, orthoView);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for SetWorldUcsBaseOrigin
        /// </summary>
        [TestMethod]
        public void SetWorldUcsBaseOriginTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Point3d origin = new Point3d(); // TODO: Initialize to an appropriate value
            OrthographicView orthoView = new OrthographicView(); // TODO: Initialize to an appropriate value
            target.SetWorldUcsBaseOrigin(origin, orthoView);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for TimeZoneDescription
        /// </summary>
        [TestMethod]
        public void TimeZoneDescriptionTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var tz = new TimeZone(); // TODO: Initialize to an appropriate value
            var expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.TimeZoneDescription(tz);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for TimeZoneOffset
        /// </summary>
        [TestMethod]
        public void TimeZoneOffsetTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var tz = new TimeZone(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            actual = target.TimeZoneOffset(tz);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for TryGetObjectId
        /// </summary>
        [TestMethod]
        public void TryGetObjectIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Handle objHandle = new Handle(); // TODO: Initialize to an appropriate value
            var id = new ObjectId(); // TODO: Initialize to an appropriate value
            var idExpected = new ObjectId(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.TryGetObjectId(objHandle, out id);
            Assert.AreEqual(idExpected, id);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for UnloadXrefs
        /// </summary>
        [TestMethod]
        public void UnloadXrefsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectIdCollection xrefIds = null; // TODO: Initialize to an appropriate value
            target.UnloadXrefs(xrefIds);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for UpdateExt
        /// </summary>
        [TestMethod]
        public void UpdateExtTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var doBestFit = false; // TODO: Initialize to an appropriate value
            target.UpdateExt(doBestFit);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for Wblock
        /// </summary>
        [TestMethod]
        public void WblockTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Database outputDataBase = null; // TODO: Initialize to an appropriate value
            ObjectIdCollection outObjIds = null; // TODO: Initialize to an appropriate value
            Point3d basePoint = new Point3d(); // TODO: Initialize to an appropriate value
            DuplicateRecordCloning cloning = new DuplicateRecordCloning(); // TODO: Initialize to an appropriate value
            target.Wblock(outputDataBase, outObjIds, basePoint, cloning);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for Wblock
        /// </summary>
        [TestMethod]
        public void WblockTest1()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var blockId = new ObjectId(); // TODO: Initialize to an appropriate value
            Database expected = null; // TODO: Initialize to an appropriate value
            Database actual;
            actual = target.Wblock(blockId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Wblock
        /// </summary>
        [TestMethod]
        public void WblockTest2()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Database expected = null; // TODO: Initialize to an appropriate value
            Database actual;
            actual = target.Wblock();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Wblock
        /// </summary>
        [TestMethod]
        public void WblockTest3()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectIdCollection outObjIds = null; // TODO: Initialize to an appropriate value
            Point3d basePoint = new Point3d(); // TODO: Initialize to an appropriate value
            Database expected = null; // TODO: Initialize to an appropriate value
            Database actual;
            actual = target.Wblock(outObjIds, basePoint);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for WblockCloneObjects
        /// </summary>
        [TestMethod]
        public void WblockCloneObjectsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectIdCollection identifiers = null; // TODO: Initialize to an appropriate value
            var id = new ObjectId(); // TODO: Initialize to an appropriate value
            IdMapping mapping = null; // TODO: Initialize to an appropriate value
            DuplicateRecordCloning cloning = new DuplicateRecordCloning(); // TODO: Initialize to an appropriate value
            var deferTranslation = false; // TODO: Initialize to an appropriate value
            target.WblockCloneObjects(identifiers, id, mapping, cloning, deferTranslation);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for WorldPaperspaceUcsBaseOrigin
        /// </summary>
        [TestMethod]
        public void WorldPaperspaceUcsBaseOriginTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            OrthographicView orthoView = new OrthographicView(); // TODO: Initialize to an appropriate value
            Point3d expected = new Point3d(); // TODO: Initialize to an appropriate value
            Point3d actual;
            actual = target.WorldPaperspaceUcsBaseOrigin(orthoView);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for WorldUcsBaseOrigin
        /// </summary>
        [TestMethod]
        public void WorldUcsBaseOriginTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            OrthographicView orthoView = new OrthographicView(); // TODO: Initialize to an appropriate value
            Point3d expected = new Point3d(); // TODO: Initialize to an appropriate value
            Point3d actual;
            actual = target.WorldUcsBaseOrigin(orthoView);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for XBindXrefs
        /// </summary>
        [TestMethod]
        public void XBindXrefsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectIdCollection xrefSymbolIds = null; // TODO: Initialize to an appropriate value
            var insertBind = false; // TODO: Initialize to an appropriate value
            target.XBindXrefs(xrefSymbolIds, insertBind);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for AcadDatabase
        /// </summary>
        [TestMethod]
        public void AcadDatabaseTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            object actual;
            actual = target.AcadDatabase;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for AllowExtendedNames
        /// </summary>
        [TestMethod]
        public void AllowExtendedNamesTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.AllowExtendedNames = expected;
            actual = target.AllowExtendedNames;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Angbase
        /// </summary>
        [TestMethod]
        public void AngbaseTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Angbase = expected;
            actual = target.Angbase;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Angdir
        /// </summary>
        [TestMethod]
        public void AngdirTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Angdir = expected;
            actual = target.Angdir;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for AnnoAllVisible
        /// </summary>
        [TestMethod]
        public void AnnoAllVisibleTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.AnnoAllVisible = expected;
            actual = target.AnnoAllVisible;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for AnnotativeDwg
        /// </summary>
        [TestMethod]
        public void AnnotativeDwgTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.AnnotativeDwg = expected;
            actual = target.AnnotativeDwg;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for ApproxNumObjects
        /// </summary>
        [TestMethod]
        public void ApproxNumObjectsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.ApproxNumObjects;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Attmode
        /// </summary>
        [TestMethod]
        public void AttmodeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Attmode = expected;
            actual = target.Attmode;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Aunits
        /// </summary>
        [TestMethod]
        public void AunitsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Aunits = expected;
            actual = target.Aunits;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Auprec
        /// </summary>
        [TestMethod]
        public void AuprecTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Auprec = expected;
            actual = target.Auprec;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for BlockTableId
        /// </summary>
        [TestMethod]
        public void BlockTableIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.BlockTableId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for ByBlockLinetype
        /// </summary>
        [TestMethod]
        public void ByBlockLinetypeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.ByBlockLinetype;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for ByLayerLinetype
        /// </summary>
        [TestMethod]
        public void ByLayerLinetypeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.ByLayerLinetype;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for CameraDisplay
        /// </summary>
        [TestMethod]
        public void CameraDisplayTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.CameraDisplay = expected;
            actual = target.CameraDisplay;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for CameraHeight
        /// </summary>
        [TestMethod]
        public void CameraHeightTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.CameraHeight = expected;
            actual = target.CameraHeight;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Cannoscale
        /// </summary>
        [TestMethod]
        public void CannoscaleTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            AnnotationScale expected = null; // TODO: Initialize to an appropriate value
            AnnotationScale actual;
            target.Cannoscale = expected;
            actual = target.Cannoscale;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Cecolor
        /// </summary>
        [TestMethod]
        public void CecolorTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Color expected = null; // TODO: Initialize to an appropriate value
            Color actual;
            target.Cecolor = expected;
            actual = target.Cecolor;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Celtscale
        /// </summary>
        [TestMethod]
        public void CeltscaleTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Celtscale = expected;
            actual = target.Celtscale;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Celtype
        /// </summary>
        [TestMethod]
        public void CeltypeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.Celtype = expected;
            actual = target.Celtype;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Celweight
        /// </summary>
        [TestMethod]
        public void CelweightTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            LineWeight expected = new LineWeight(); // TODO: Initialize to an appropriate value
            LineWeight actual;
            target.Celweight = expected;
            actual = target.Celweight;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Cetransparency
        /// </summary>
        [TestMethod]
        public void CetransparencyTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Transparency expected = new Transparency(); // TODO: Initialize to an appropriate value
            Transparency actual;
            target.Cetransparency = expected;
            actual = target.Cetransparency;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Chamfera
        /// </summary>
        [TestMethod]
        public void ChamferaTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Chamfera = expected;
            actual = target.Chamfera;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Chamferb
        /// </summary>
        [TestMethod]
        public void ChamferbTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Chamferb = expected;
            actual = target.Chamferb;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Chamferc
        /// </summary>
        [TestMethod]
        public void ChamfercTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Chamferc = expected;
            actual = target.Chamferc;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Chamferd
        /// </summary>
        [TestMethod]
        public void ChamferdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Chamferd = expected;
            actual = target.Chamferd;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Clayer
        /// </summary>
        [TestMethod]
        public void ClayerTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.Clayer = expected;
            actual = target.Clayer;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Cmaterial
        /// </summary>
        [TestMethod]
        public void CmaterialTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.Cmaterial = expected;
            actual = target.Cmaterial;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Cmljust
        /// </summary>
        [TestMethod]
        public void CmljustTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Cmljust = expected;
            actual = target.Cmljust;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Cmlscale
        /// </summary>
        [TestMethod]
        public void CmlscaleTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Cmlscale = expected;
            actual = target.Cmlscale;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for CmlstyleID
        /// </summary>
        [TestMethod]
        public void CmlstyleIDTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.CmlstyleID = expected;
            actual = target.CmlstyleID;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for ColorDictionaryId
        /// </summary>
        [TestMethod]
        public void ColorDictionaryIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.ColorDictionaryId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for ContinuousLinetype
        /// </summary>
        [TestMethod]
        public void ContinuousLinetypeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.ContinuousLinetype;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Cshadow
        /// </summary>
        [TestMethod]
        public void CshadowTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Cshadow = expected;
            actual = target.Cshadow;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for CurrentSpaceId
        /// </summary>
        [TestMethod]
        public void CurrentSpaceIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.CurrentSpaceId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for CurrentViewportTableRecordId
        /// </summary>
        [TestMethod]
        public void CurrentViewportTableRecordIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.CurrentViewportTableRecordId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for DataLinkDictionaryId
        /// </summary>
        [TestMethod]
        public void DataLinkDictionaryIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.DataLinkDictionaryId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for DataLinkManager
        /// </summary>
        [TestMethod]
        public void DataLinkManagerTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            DataLinkManager actual;
            actual = target.DataLinkManager;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for DetailViewStyle
        /// </summary>
        [TestMethod]
        public void DetailViewStyleTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.DetailViewStyle = expected;
            actual = target.DetailViewStyle;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for DetailViewStyleDictionaryId
        /// </summary>
        [TestMethod]
        public void DetailViewStyleDictionaryIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.DetailViewStyleDictionaryId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for DgnFrame
        /// </summary>
        [TestMethod]
        public void DgnFrameTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.DgnFrame = expected;
            actual = target.DgnFrame;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for DimAssoc
        /// </summary>
        [TestMethod]
        public void DimAssocTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.DimAssoc = expected;
            actual = target.DimAssoc;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for DimStyleTableId
        /// </summary>
        [TestMethod]
        public void DimStyleTableIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.DimStyleTableId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimadec
        /// </summary>
        [TestMethod]
        public void DimadecTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimadec = expected;
            actual = target.Dimadec;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimalt
        /// </summary>
        [TestMethod]
        public void DimaltTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Dimalt = expected;
            actual = target.Dimalt;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimaltd
        /// </summary>
        [TestMethod]
        public void DimaltdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimaltd = expected;
            actual = target.Dimaltd;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimaltf
        /// </summary>
        [TestMethod]
        public void DimaltfTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimaltf = expected;
            actual = target.Dimaltf;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimaltrnd
        /// </summary>
        [TestMethod]
        public void DimaltrndTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimaltrnd = expected;
            actual = target.Dimaltrnd;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimalttd
        /// </summary>
        [TestMethod]
        public void DimalttdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimalttd = expected;
            actual = target.Dimalttd;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimalttz
        /// </summary>
        [TestMethod]
        public void DimalttzTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimalttz = expected;
            actual = target.Dimalttz;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimaltu
        /// </summary>
        [TestMethod]
        public void DimaltuTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimaltu = expected;
            actual = target.Dimaltu;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimaltz
        /// </summary>
        [TestMethod]
        public void DimaltzTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimaltz = expected;
            actual = target.Dimaltz;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimapost
        /// </summary>
        [TestMethod]
        public void DimapostTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.Dimapost = expected;
            actual = target.Dimapost;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimarcsym
        /// </summary>
        [TestMethod]
        public void DimarcsymTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimarcsym = expected;
            actual = target.Dimarcsym;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimaso
        /// </summary>
        [TestMethod]
        public void DimasoTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Dimaso = expected;
            actual = target.Dimaso;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimasz
        /// </summary>
        [TestMethod]
        public void DimaszTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimasz = expected;
            actual = target.Dimasz;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimatfit
        /// </summary>
        [TestMethod]
        public void DimatfitTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimatfit = expected;
            actual = target.Dimatfit;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimaunit
        /// </summary>
        [TestMethod]
        public void DimaunitTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimaunit = expected;
            actual = target.Dimaunit;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimazin
        /// </summary>
        [TestMethod]
        public void DimazinTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimazin = expected;
            actual = target.Dimazin;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimblk
        /// </summary>
        [TestMethod]
        public void DimblkTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.Dimblk = expected;
            actual = target.Dimblk;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimblk1
        /// </summary>
        [TestMethod]
        public void Dimblk1Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.Dimblk1 = expected;
            actual = target.Dimblk1;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimblk2
        /// </summary>
        [TestMethod]
        public void Dimblk2Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.Dimblk2 = expected;
            actual = target.Dimblk2;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimcen
        /// </summary>
        [TestMethod]
        public void DimcenTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimcen = expected;
            actual = target.Dimcen;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimclrd
        /// </summary>
        [TestMethod]
        public void DimclrdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Color expected = null; // TODO: Initialize to an appropriate value
            Color actual;
            target.Dimclrd = expected;
            actual = target.Dimclrd;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimclre
        /// </summary>
        [TestMethod]
        public void DimclreTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Color expected = null; // TODO: Initialize to an appropriate value
            Color actual;
            target.Dimclre = expected;
            actual = target.Dimclre;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimclrt
        /// </summary>
        [TestMethod]
        public void DimclrtTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Color expected = null; // TODO: Initialize to an appropriate value
            Color actual;
            target.Dimclrt = expected;
            actual = target.Dimclrt;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimdec
        /// </summary>
        [TestMethod]
        public void DimdecTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimdec = expected;
            actual = target.Dimdec;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimdle
        /// </summary>
        [TestMethod]
        public void DimdleTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimdle = expected;
            actual = target.Dimdle;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimdli
        /// </summary>
        [TestMethod]
        public void DimdliTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimdli = expected;
            actual = target.Dimdli;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimdsep
        /// </summary>
        [TestMethod]
        public void DimdsepTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = '\0'; // TODO: Initialize to an appropriate value
            char actual;
            target.Dimdsep = expected;
            actual = target.Dimdsep;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimexe
        /// </summary>
        [TestMethod]
        public void DimexeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimexe = expected;
            actual = target.Dimexe;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimexo
        /// </summary>
        [TestMethod]
        public void DimexoTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimexo = expected;
            actual = target.Dimexo;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimfrac
        /// </summary>
        [TestMethod]
        public void DimfracTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimfrac = expected;
            actual = target.Dimfrac;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimfxlen
        /// </summary>
        [TestMethod]
        public void DimfxlenTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimfxlen = expected;
            actual = target.Dimfxlen;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for DimfxlenOn
        /// </summary>
        [TestMethod]
        public void DimfxlenOnTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.DimfxlenOn = expected;
            actual = target.DimfxlenOn;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimgap
        /// </summary>
        [TestMethod]
        public void DimgapTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimgap = expected;
            actual = target.Dimgap;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimjogang
        /// </summary>
        [TestMethod]
        public void DimjogangTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimjogang = expected;
            actual = target.Dimjogang;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimjust
        /// </summary>
        [TestMethod]
        public void DimjustTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimjust = expected;
            actual = target.Dimjust;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimldrblk
        /// </summary>
        [TestMethod]
        public void DimldrblkTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.Dimldrblk = expected;
            actual = target.Dimldrblk;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimlfac
        /// </summary>
        [TestMethod]
        public void DimlfacTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimlfac = expected;
            actual = target.Dimlfac;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimlim
        /// </summary>
        [TestMethod]
        public void DimlimTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Dimlim = expected;
            actual = target.Dimlim;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimltex1
        /// </summary>
        [TestMethod]
        public void Dimltex1Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.Dimltex1 = expected;
            actual = target.Dimltex1;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimltex2
        /// </summary>
        [TestMethod]
        public void Dimltex2Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.Dimltex2 = expected;
            actual = target.Dimltex2;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimltype
        /// </summary>
        [TestMethod]
        public void DimltypeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.Dimltype = expected;
            actual = target.Dimltype;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimlunit
        /// </summary>
        [TestMethod]
        public void DimlunitTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimlunit = expected;
            actual = target.Dimlunit;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimlwd
        /// </summary>
        [TestMethod]
        public void DimlwdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            LineWeight expected = new LineWeight(); // TODO: Initialize to an appropriate value
            LineWeight actual;
            target.Dimlwd = expected;
            actual = target.Dimlwd;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimlwe
        /// </summary>
        [TestMethod]
        public void DimlweTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            LineWeight expected = new LineWeight(); // TODO: Initialize to an appropriate value
            LineWeight actual;
            target.Dimlwe = expected;
            actual = target.Dimlwe;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimpost
        /// </summary>
        [TestMethod]
        public void DimpostTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.Dimpost = expected;
            actual = target.Dimpost;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimrnd
        /// </summary>
        [TestMethod]
        public void DimrndTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimrnd = expected;
            actual = target.Dimrnd;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimsah
        /// </summary>
        [TestMethod]
        public void DimsahTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Dimsah = expected;
            actual = target.Dimsah;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimscale
        /// </summary>
        [TestMethod]
        public void DimscaleTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimscale = expected;
            actual = target.Dimscale;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimsd1
        /// </summary>
        [TestMethod]
        public void Dimsd1Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Dimsd1 = expected;
            actual = target.Dimsd1;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimsd2
        /// </summary>
        [TestMethod]
        public void Dimsd2Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Dimsd2 = expected;
            actual = target.Dimsd2;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimse1
        /// </summary>
        [TestMethod]
        public void Dimse1Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Dimse1 = expected;
            actual = target.Dimse1;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimse2
        /// </summary>
        [TestMethod]
        public void Dimse2Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Dimse2 = expected;
            actual = target.Dimse2;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimsho
        /// </summary>
        [TestMethod]
        public void DimshoTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Dimsho = expected;
            actual = target.Dimsho;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimsoxd
        /// </summary>
        [TestMethod]
        public void DimsoxdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Dimsoxd = expected;
            actual = target.Dimsoxd;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimstyle
        /// </summary>
        [TestMethod]
        public void DimstyleTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.Dimstyle = expected;
            actual = target.Dimstyle;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtad
        /// </summary>
        [TestMethod]
        public void DimtadTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimtad = expected;
            actual = target.Dimtad;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtdec
        /// </summary>
        [TestMethod]
        public void DimtdecTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimtdec = expected;
            actual = target.Dimtdec;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtfac
        /// </summary>
        [TestMethod]
        public void DimtfacTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimtfac = expected;
            actual = target.Dimtfac;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtfill
        /// </summary>
        [TestMethod]
        public void DimtfillTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimtfill = expected;
            actual = target.Dimtfill;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtfillclr
        /// </summary>
        [TestMethod]
        public void DimtfillclrTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Color expected = null; // TODO: Initialize to an appropriate value
            Color actual;
            target.Dimtfillclr = expected;
            actual = target.Dimtfillclr;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtih
        /// </summary>
        [TestMethod]
        public void DimtihTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Dimtih = expected;
            actual = target.Dimtih;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtix
        /// </summary>
        [TestMethod]
        public void DimtixTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Dimtix = expected;
            actual = target.Dimtix;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtm
        /// </summary>
        [TestMethod]
        public void DimtmTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimtm = expected;
            actual = target.Dimtm;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtmove
        /// </summary>
        [TestMethod]
        public void DimtmoveTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimtmove = expected;
            actual = target.Dimtmove;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtofl
        /// </summary>
        [TestMethod]
        public void DimtoflTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Dimtofl = expected;
            actual = target.Dimtofl;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtoh
        /// </summary>
        [TestMethod]
        public void DimtohTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Dimtoh = expected;
            actual = target.Dimtoh;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtol
        /// </summary>
        [TestMethod]
        public void DimtolTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Dimtol = expected;
            actual = target.Dimtol;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtolj
        /// </summary>
        [TestMethod]
        public void DimtoljTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimtolj = expected;
            actual = target.Dimtolj;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtp
        /// </summary>
        [TestMethod]
        public void DimtpTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimtp = expected;
            actual = target.Dimtp;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtsz
        /// </summary>
        [TestMethod]
        public void DimtszTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimtsz = expected;
            actual = target.Dimtsz;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtvp
        /// </summary>
        [TestMethod]
        public void DimtvpTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimtvp = expected;
            actual = target.Dimtvp;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtxsty
        /// </summary>
        [TestMethod]
        public void DimtxstyTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.Dimtxsty = expected;
            actual = target.Dimtxsty;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtxt
        /// </summary>
        [TestMethod]
        public void DimtxtTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Dimtxt = expected;
            actual = target.Dimtxt;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtxtdirection
        /// </summary>
        [TestMethod]
        public void DimtxtdirectionTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Dimtxtdirection = expected;
            actual = target.Dimtxtdirection;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimtzin
        /// </summary>
        [TestMethod]
        public void DimtzinTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimtzin = expected;
            actual = target.Dimtzin;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimupt
        /// </summary>
        [TestMethod]
        public void DimuptTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Dimupt = expected;
            actual = target.Dimupt;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Dimzin
        /// </summary>
        [TestMethod]
        public void DimzinTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Dimzin = expected;
            actual = target.Dimzin;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for DispSilh
        /// </summary>
        [TestMethod]
        public void DispSilhTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.DispSilh = expected;
            actual = target.DispSilh;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for DrawOrderCtl
        /// </summary>
        [TestMethod]
        public void DrawOrderCtlTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            byte expected = 0; // TODO: Initialize to an appropriate value
            byte actual;
            target.DrawOrderCtl = expected;
            actual = target.DrawOrderCtl;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for DwfFrame
        /// </summary>
        [TestMethod]
        public void DwfFrameTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.DwfFrame = expected;
            actual = target.DwfFrame;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for DwgFileWasSavedByAutodeskSoftware
        /// </summary>
        [TestMethod]
        public void DwgFileWasSavedByAutodeskSoftwareTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.DwgFileWasSavedByAutodeskSoftware;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for DxEval
        /// </summary>
        [TestMethod]
        public void DxEvalTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.DxEval = expected;
            actual = target.DxEval;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Elevation
        /// </summary>
        [TestMethod]
        public void ElevationTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Elevation = expected;
            actual = target.Elevation;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for EndCaps
        /// </summary>
        [TestMethod]
        public void EndCapsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            EndCap expected = new EndCap(); // TODO: Initialize to an appropriate value
            EndCap actual;
            target.EndCaps = expected;
            actual = target.EndCaps;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Extmax
        /// </summary>
        [TestMethod]
        public void ExtmaxTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Point3d expected = new Point3d(); // TODO: Initialize to an appropriate value
            Point3d actual;
            target.Extmax = expected;
            actual = target.Extmax;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Extmin
        /// </summary>
        [TestMethod]
        public void ExtminTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Point3d expected = new Point3d(); // TODO: Initialize to an appropriate value
            Point3d actual;
            target.Extmin = expected;
            actual = target.Extmin;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Facetres
        /// </summary>
        [TestMethod]
        public void FacetresTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Facetres = expected;
            actual = target.Facetres;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for FileDependencyManager
        /// </summary>
        [TestMethod]
        public void FileDependencyManagerTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            FileDependencyManager actual;
            actual = target.FileDependencyManager;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Filename
        /// </summary>
        [TestMethod]
        public void FilenameTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Filename;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Filletrad
        /// </summary>
        [TestMethod]
        public void FilletradTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Filletrad = expected;
            actual = target.Filletrad;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Fillmode
        /// </summary>
        [TestMethod]
        public void FillmodeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Fillmode = expected;
            actual = target.Fillmode;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for FingerprintGuid
        /// </summary>
        [TestMethod]
        public void FingerprintGuidTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.FingerprintGuid = expected;
            actual = target.FingerprintGuid;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for GeoDataObject
        /// </summary>
        [TestMethod]
        public void GeoDataObjectTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.GeoDataObject;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for GroupDictionaryId
        /// </summary>
        [TestMethod]
        public void GroupDictionaryIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.GroupDictionaryId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for HaloGap
        /// </summary>
        [TestMethod]
        public void HaloGapTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.HaloGap = expected;
            actual = target.HaloGap;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Handseed
        /// </summary>
        [TestMethod]
        public void HandseedTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Handle expected = new Handle(); // TODO: Initialize to an appropriate value
            Handle actual;
            target.Handseed = expected;
            actual = target.Handseed;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for HideText
        /// </summary>
        [TestMethod]
        public void HideTextTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.HideText = expected;
            actual = target.HideText;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for HomeView
        /// </summary>
        [TestMethod]
        public void HomeViewTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            DbHomeView expected = null; // TODO: Initialize to an appropriate value
            DbHomeView actual;
            target.HomeView = expected;
            actual = target.HomeView;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for HpInherit
        /// </summary>
        [TestMethod]
        public void HpInheritTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.HpInherit = expected;
            actual = target.HpInherit;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for HpOrigin
        /// </summary>
        [TestMethod]
        public void HpOriginTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Point2d expected = new Point2d(); // TODO: Initialize to an appropriate value
            Point2d actual;
            target.HpOrigin = expected;
            actual = target.HpOrigin;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for HyperlinkBase
        /// </summary>
        [TestMethod]
        public void HyperlinkBaseTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.HyperlinkBase = expected;
            actual = target.HyperlinkBase;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Indexctl
        /// </summary>
        [TestMethod]
        public void IndexctlTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Indexctl = expected;
            actual = target.Indexctl;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Insbase
        /// </summary>
        [TestMethod]
        public void InsbaseTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Point3d expected = new Point3d(); // TODO: Initialize to an appropriate value
            Point3d actual;
            target.Insbase = expected;
            actual = target.Insbase;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Insunits
        /// </summary>
        [TestMethod]
        public void InsunitsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            UnitsValue expected = new UnitsValue(); // TODO: Initialize to an appropriate value
            UnitsValue actual;
            target.Insunits = expected;
            actual = target.Insunits;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Interferecolor
        /// </summary>
        [TestMethod]
        public void InterferecolorTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Color expected = null; // TODO: Initialize to an appropriate value
            Color actual;
            target.Interferecolor = expected;
            actual = target.Interferecolor;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Interfereobjvs
        /// </summary>
        [TestMethod]
        public void InterfereobjvsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.Interfereobjvs = expected;
            actual = target.Interfereobjvs;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Interferevpvs
        /// </summary>
        [TestMethod]
        public void InterferevpvsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.Interferevpvs = expected;
            actual = target.Interferevpvs;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for IntersectColor
        /// </summary>
        [TestMethod]
        public void IntersectColorTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.IntersectColor = expected;
            actual = target.IntersectColor;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for IntersectDisplay
        /// </summary>
        [TestMethod]
        public void IntersectDisplayTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.IntersectDisplay = expected;
            actual = target.IntersectDisplay;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for IsEmr
        /// </summary>
        [TestMethod]
        public void IsEmrTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsEmr;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for IsPartiallyOpened
        /// </summary>
        [TestMethod]
        public void IsPartiallyOpenedTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsPartiallyOpened;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Isolines
        /// </summary>
        [TestMethod]
        public void IsolinesTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Isolines = expected;
            actual = target.Isolines;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for JoinStyle
        /// </summary>
        [TestMethod]
        public void JoinStyleTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            JoinStyle expected = new JoinStyle(); // TODO: Initialize to an appropriate value
            JoinStyle actual;
            target.JoinStyle = expected;
            actual = target.JoinStyle;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LastSavedAsMaintenanceVersion
        /// </summary>
        [TestMethod]
        public void LastSavedAsMaintenanceVersionTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            MaintenanceReleaseVersion actual;
            actual = target.LastSavedAsMaintenanceVersion;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LastSavedAsVersion
        /// </summary>
        [TestMethod]
        public void LastSavedAsVersionTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            DwgVersion actual;
            actual = target.LastSavedAsVersion;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Latitude
        /// </summary>
        [TestMethod]
        public void LatitudeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Latitude = expected;
            actual = target.Latitude;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LayerEval
        /// </summary>
        [TestMethod]
        public void LayerEvalTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.LayerEval = expected;
            actual = target.LayerEval;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LayerFilters
        /// </summary>
        [TestMethod]
        public void LayerFiltersTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            LayerFilterTree expected = new LayerFilterTree(); // TODO: Initialize to an appropriate value
            LayerFilterTree actual;
            target.LayerFilters = expected;
            actual = target.LayerFilters;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LayerNotify
        /// </summary>
        [TestMethod]
        public void LayerNotifyTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.LayerNotify = expected;
            actual = target.LayerNotify;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LayerStateManager
        /// </summary>
        [TestMethod]
        public void LayerStateManagerTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            LayerStateManager actual;
            actual = target.LayerStateManager;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LayerTableId
        /// </summary>
        [TestMethod]
        public void LayerTableIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.LayerTableId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LayerZero
        /// </summary>
        [TestMethod]
        public void LayerZeroTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.LayerZero;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LayoutDictionaryId
        /// </summary>
        [TestMethod]
        public void LayoutDictionaryIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.LayoutDictionaryId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LensLength
        /// </summary>
        [TestMethod]
        public void LensLengthTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.LensLength = expected;
            actual = target.LensLength;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LightGlyphDisplay
        /// </summary>
        [TestMethod]
        public void LightGlyphDisplayTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.LightGlyphDisplay = expected;
            actual = target.LightGlyphDisplay;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LightingUnits
        /// </summary>
        [TestMethod]
        public void LightingUnitsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            byte expected = 0; // TODO: Initialize to an appropriate value
            byte actual;
            target.LightingUnits = expected;
            actual = target.LightingUnits;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LightsInBlocks
        /// </summary>
        [TestMethod]
        public void LightsInBlocksTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.LightsInBlocks = expected;
            actual = target.LightsInBlocks;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Limcheck
        /// </summary>
        [TestMethod]
        public void LimcheckTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Limcheck = expected;
            actual = target.Limcheck;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Limmax
        /// </summary>
        [TestMethod]
        public void LimmaxTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Point2d expected = new Point2d(); // TODO: Initialize to an appropriate value
            Point2d actual;
            target.Limmax = expected;
            actual = target.Limmax;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Limmin
        /// </summary>
        [TestMethod]
        public void LimminTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Point2d expected = new Point2d(); // TODO: Initialize to an appropriate value
            Point2d actual;
            target.Limmin = expected;
            actual = target.Limmin;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LineWeightDisplay
        /// </summary>
        [TestMethod]
        public void LineWeightDisplayTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.LineWeightDisplay = expected;
            actual = target.LineWeightDisplay;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LinetypeTableId
        /// </summary>
        [TestMethod]
        public void LinetypeTableIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.LinetypeTableId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LoftAng1
        /// </summary>
        [TestMethod]
        public void LoftAng1Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.LoftAng1 = expected;
            actual = target.LoftAng1;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LoftAng2
        /// </summary>
        [TestMethod]
        public void LoftAng2Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.LoftAng2 = expected;
            actual = target.LoftAng2;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LoftMag1
        /// </summary>
        [TestMethod]
        public void LoftMag1Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.LoftMag1 = expected;
            actual = target.LoftMag1;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LoftMag2
        /// </summary>
        [TestMethod]
        public void LoftMag2Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.LoftMag2 = expected;
            actual = target.LoftMag2;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LoftNormals
        /// </summary>
        [TestMethod]
        public void LoftNormalsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.LoftNormals = expected;
            actual = target.LoftNormals;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for LoftParam
        /// </summary>
        [TestMethod]
        public void LoftParamTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.LoftParam = expected;
            actual = target.LoftParam;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Longitude
        /// </summary>
        [TestMethod]
        public void LongitudeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Longitude = expected;
            actual = target.Longitude;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Ltscale
        /// </summary>
        [TestMethod]
        public void LtscaleTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Ltscale = expected;
            actual = target.Ltscale;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Lunits
        /// </summary>
        [TestMethod]
        public void LunitsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Lunits = expected;
            actual = target.Lunits;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Luprec
        /// </summary>
        [TestMethod]
        public void LuprecTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Luprec = expected;
            actual = target.Luprec;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for MLStyleDictionaryId
        /// </summary>
        [TestMethod]
        public void MLStyleDictionaryIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.MLStyleDictionaryId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for MLeaderStyleDictionaryId
        /// </summary>
        [TestMethod]
        public void MLeaderStyleDictionaryIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.MLeaderStyleDictionaryId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for MLeaderstyle
        /// </summary>
        [TestMethod]
        public void MLeaderstyleTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.MLeaderstyle = expected;
            actual = target.MLeaderstyle;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for MaintenanceReleaseVersion
        /// </summary>
        [TestMethod]
        public void MaintenanceReleaseVersionTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.MaintenanceReleaseVersion;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for MaterialDictionaryId
        /// </summary>
        [TestMethod]
        public void MaterialDictionaryIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.MaterialDictionaryId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Maxactvp
        /// </summary>
        [TestMethod]
        public void MaxactvpTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Maxactvp = expected;
            actual = target.Maxactvp;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Measurement
        /// </summary>
        [TestMethod]
        public void MeasurementTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            MeasurementValue expected = new MeasurementValue(); // TODO: Initialize to an appropriate value
            MeasurementValue actual;
            target.Measurement = expected;
            actual = target.Measurement;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Menu
        /// </summary>
        [TestMethod]
        public void MenuTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Menu;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Mirrtext
        /// </summary>
        [TestMethod]
        public void MirrtextTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Mirrtext = expected;
            actual = target.Mirrtext;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for MsLtScale
        /// </summary>
        [TestMethod]
        public void MsLtScaleTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.MsLtScale = expected;
            actual = target.MsLtScale;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for MsOleScale
        /// </summary>
        [TestMethod]
        public void MsOleScaleTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.MsOleScale = expected;
            actual = target.MsOleScale;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for NamedObjectsDictionaryId
        /// </summary>
        [TestMethod]
        public void NamedObjectsDictionaryIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.NamedObjectsDictionaryId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for NorthDirection
        /// </summary>
        [TestMethod]
        public void NorthDirectionTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.NorthDirection = expected;
            actual = target.NorthDirection;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for NumberOfSaves
        /// </summary>
        [TestMethod]
        public void NumberOfSavesTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.NumberOfSaves;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for ObjectContextManager
        /// </summary>
        [TestMethod]
        public void ObjectContextManagerTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectContextManager actual;
            actual = target.ObjectContextManager;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for ObscuredColor
        /// </summary>
        [TestMethod]
        public void ObscuredColorTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.ObscuredColor = expected;
            actual = target.ObscuredColor;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for ObscuredLineType
        /// </summary>
        [TestMethod]
        public void ObscuredLineTypeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.ObscuredLineType = expected;
            actual = target.ObscuredLineType;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for OleStartUp
        /// </summary>
        [TestMethod]
        public void OleStartUpTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.OleStartUp = expected;
            actual = target.OleStartUp;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for OriginalFileMaintenanceVersion
        /// </summary>
        [TestMethod]
        public void OriginalFileMaintenanceVersionTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            MaintenanceReleaseVersion actual;
            actual = target.OriginalFileMaintenanceVersion;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for OriginalFileName
        /// </summary>
        [TestMethod]
        public void OriginalFileNameTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.OriginalFileName;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for OriginalFileSavedByMaintenanceVersion
        /// </summary>
        [TestMethod]
        public void OriginalFileSavedByMaintenanceVersionTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            MaintenanceReleaseVersion actual;
            actual = target.OriginalFileSavedByMaintenanceVersion;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for OriginalFileSavedByVersion
        /// </summary>
        [TestMethod]
        public void OriginalFileSavedByVersionTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            DwgVersion actual;
            actual = target.OriginalFileSavedByVersion;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for OriginalFileVersion
        /// </summary>
        [TestMethod]
        public void OriginalFileVersionTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            DwgVersion actual;
            actual = target.OriginalFileVersion;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Orthomode
        /// </summary>
        [TestMethod]
        public void OrthomodeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Orthomode = expected;
            actual = target.Orthomode;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for PaperSpaceVportId
        /// </summary>
        [TestMethod]
        public void PaperSpaceVportIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.PaperSpaceVportId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for PdfFrame
        /// </summary>
        [TestMethod]
        public void PdfFrameTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.PdfFrame = expected;
            actual = target.PdfFrame;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Pdmode
        /// </summary>
        [TestMethod]
        public void PdmodeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Pdmode = expected;
            actual = target.Pdmode;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Pdsize
        /// </summary>
        [TestMethod]
        public void PdsizeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Pdsize = expected;
            actual = target.Pdsize;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Pelevation
        /// </summary>
        [TestMethod]
        public void PelevationTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Pelevation = expected;
            actual = target.Pelevation;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Pextmax
        /// </summary>
        [TestMethod]
        public void PextmaxTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Point3d expected = new Point3d(); // TODO: Initialize to an appropriate value
            Point3d actual;
            target.Pextmax = expected;
            actual = target.Pextmax;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Pextmin
        /// </summary>
        [TestMethod]
        public void PextminTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Point3d expected = new Point3d(); // TODO: Initialize to an appropriate value
            Point3d actual;
            target.Pextmin = expected;
            actual = target.Pextmin;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Pinsbase
        /// </summary>
        [TestMethod]
        public void PinsbaseTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Point3d expected = new Point3d(); // TODO: Initialize to an appropriate value
            Point3d actual;
            target.Pinsbase = expected;
            actual = target.Pinsbase;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Plimcheck
        /// </summary>
        [TestMethod]
        public void PlimcheckTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Plimcheck = expected;
            actual = target.Plimcheck;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Plimmax
        /// </summary>
        [TestMethod]
        public void PlimmaxTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Point2d expected = new Point2d(); // TODO: Initialize to an appropriate value
            Point2d actual;
            target.Plimmax = expected;
            actual = target.Plimmax;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Plimmin
        /// </summary>
        [TestMethod]
        public void PlimminTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Point2d expected = new Point2d(); // TODO: Initialize to an appropriate value
            Point2d actual;
            target.Plimmin = expected;
            actual = target.Plimmin;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for PlineEllipse
        /// </summary>
        [TestMethod]
        public void PlineEllipseTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.PlineEllipse = expected;
            actual = target.PlineEllipse;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Plinegen
        /// </summary>
        [TestMethod]
        public void PlinegenTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Plinegen = expected;
            actual = target.Plinegen;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Plinewid
        /// </summary>
        [TestMethod]
        public void PlinewidTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Plinewid = expected;
            actual = target.Plinewid;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for PlotSettingsDictionaryId
        /// </summary>
        [TestMethod]
        public void PlotSettingsDictionaryIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.PlotSettingsDictionaryId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for PlotStyleMode
        /// </summary>
        [TestMethod]
        public void PlotStyleModeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.PlotStyleMode;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for PlotStyleNameDictionaryId
        /// </summary>
        [TestMethod]
        public void PlotStyleNameDictionaryIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.PlotStyleNameDictionaryId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for PlotStyleNameId
        /// </summary>
        [TestMethod]
        public void PlotStyleNameIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            PlotStyleDescriptor expected = new PlotStyleDescriptor(); // TODO: Initialize to an appropriate value
            PlotStyleDescriptor actual;
            target.PlotStyleNameId = expected;
            actual = target.PlotStyleNameId;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for ProjectName
        /// </summary>
        [TestMethod]
        public void ProjectNameTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.ProjectName = expected;
            actual = target.ProjectName;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Psltscale
        /// </summary>
        [TestMethod]
        public void PsltscaleTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Psltscale = expected;
            actual = target.Psltscale;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for PsolHeight
        /// </summary>
        [TestMethod]
        public void PsolHeightTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.PsolHeight = expected;
            actual = target.PsolHeight;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for PsolWidth
        /// </summary>
        [TestMethod]
        public void PsolWidthTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.PsolWidth = expected;
            actual = target.PsolWidth;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for PucsBase
        /// </summary>
        [TestMethod]
        public void PucsBaseTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.PucsBase = expected;
            actual = target.PucsBase;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for PucsOrthographic
        /// </summary>
        [TestMethod]
        public void PucsOrthographicTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            OrthographicView actual;
            actual = target.PucsOrthographic;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Pucsname
        /// </summary>
        [TestMethod]
        public void PucsnameTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.Pucsname;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Pucsorg
        /// </summary>
        [TestMethod]
        public void PucsorgTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Point3d actual;
            actual = target.Pucsorg;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Pucsxdir
        /// </summary>
        [TestMethod]
        public void PucsxdirTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Vector3d actual;
            actual = target.Pucsxdir;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Pucsydir
        /// </summary>
        [TestMethod]
        public void PucsydirTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Vector3d actual;
            actual = target.Pucsydir;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Qtextmode
        /// </summary>
        [TestMethod]
        public void QtextmodeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Qtextmode = expected;
            actual = target.Qtextmode;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for RegAppTableId
        /// </summary>
        [TestMethod]
        public void RegAppTableIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.RegAppTableId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Regenmode
        /// </summary>
        [TestMethod]
        public void RegenmodeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Regenmode = expected;
            actual = target.Regenmode;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for RetainOriginalThumbnailBitmap
        /// </summary>
        [TestMethod]
        public void RetainOriginalThumbnailBitmapTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.RetainOriginalThumbnailBitmap = expected;
            actual = target.RetainOriginalThumbnailBitmap;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Saveproxygraphics
        /// </summary>
        [TestMethod]
        public void SaveproxygraphicsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Saveproxygraphics = expected;
            actual = target.Saveproxygraphics;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for SectionManagerId
        /// </summary>
        [TestMethod]
        public void SectionManagerIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.SectionManagerId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for SectionViewStyle
        /// </summary>
        [TestMethod]
        public void SectionViewStyleTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.SectionViewStyle = expected;
            actual = target.SectionViewStyle;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for SectionViewStyleDictionaryId
        /// </summary>
        [TestMethod]
        public void SectionViewStyleDictionaryIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.SectionViewStyleDictionaryId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for SecurityParameters
        /// </summary>
        [TestMethod]
        public void SecurityParametersTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            SecurityParameters expected = null; // TODO: Initialize to an appropriate value
            SecurityParameters actual;
            target.SecurityParameters = expected;
            actual = target.SecurityParameters;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Shadedge
        /// </summary>
        [TestMethod]
        public void ShadedgeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Shadedge = expected;
            actual = target.Shadedge;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Shadedif
        /// </summary>
        [TestMethod]
        public void ShadedifTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Shadedif = expected;
            actual = target.Shadedif;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for ShadowPlaneLocation
        /// </summary>
        [TestMethod]
        public void ShadowPlaneLocationTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.ShadowPlaneLocation = expected;
            actual = target.ShadowPlaneLocation;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for ShowHist
        /// </summary>
        [TestMethod]
        public void ShowHistTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.ShowHist = expected;
            actual = target.ShowHist;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Sketchinc
        /// </summary>
        [TestMethod]
        public void SketchincTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Sketchinc = expected;
            actual = target.Sketchinc;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Skpoly
        /// </summary>
        [TestMethod]
        public void SkpolyTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Skpoly = expected;
            actual = target.Skpoly;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for SolidHist
        /// </summary>
        [TestMethod]
        public void SolidHistTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.SolidHist = expected;
            actual = target.SolidHist;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for SortEnts
        /// </summary>
        [TestMethod]
        public void SortEntsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.SortEnts = expected;
            actual = target.SortEnts;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Splframe
        /// </summary>
        [TestMethod]
        public void SplframeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Splframe = expected;
            actual = target.Splframe;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Splinesegs
        /// </summary>
        [TestMethod]
        public void SplinesegsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Splinesegs = expected;
            actual = target.Splinesegs;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Splinetype
        /// </summary>
        [TestMethod]
        public void SplinetypeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Splinetype = expected;
            actual = target.Splinetype;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for StepSize
        /// </summary>
        [TestMethod]
        public void StepSizeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.StepSize = expected;
            actual = target.StepSize;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for StepsPerSec
        /// </summary>
        [TestMethod]
        public void StepsPerSecTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.StepsPerSec = expected;
            actual = target.StepsPerSec;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for StyleSheet
        /// </summary>
        [TestMethod]
        public void StyleSheetTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.StyleSheet = expected;
            actual = target.StyleSheet;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for SummaryInfo
        /// </summary>
        [TestMethod]
        public void SummaryInfoTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            DatabaseSummaryInfo expected = new DatabaseSummaryInfo(); // TODO: Initialize to an appropriate value
            DatabaseSummaryInfo actual;
            target.SummaryInfo = expected;
            actual = target.SummaryInfo;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Surftab1
        /// </summary>
        [TestMethod]
        public void Surftab1Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Surftab1 = expected;
            actual = target.Surftab1;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Surftab2
        /// </summary>
        [TestMethod]
        public void Surftab2Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Surftab2 = expected;
            actual = target.Surftab2;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Surftype
        /// </summary>
        [TestMethod]
        public void SurftypeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Surftype = expected;
            actual = target.Surftype;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Surfu
        /// </summary>
        [TestMethod]
        public void SurfuTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Surfu = expected;
            actual = target.Surfu;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Surfv
        /// </summary>
        [TestMethod]
        public void SurfvTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Surfv = expected;
            actual = target.Surfv;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for TStackAlign
        /// </summary>
        [TestMethod]
        public void TStackAlignTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.TStackAlign = expected;
            actual = target.TStackAlign;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for TableStyleDictionaryId
        /// </summary>
        [TestMethod]
        public void TableStyleDictionaryIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.TableStyleDictionaryId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Tablestyle
        /// </summary>
        [TestMethod]
        public void TablestyleTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.Tablestyle = expected;
            actual = target.Tablestyle;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Tdcreate
        /// </summary>
        [TestMethod]
        public void TdcreateTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            DateTime actual;
            actual = target.Tdcreate;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Tdindwg
        /// </summary>
        [TestMethod]
        public void TdindwgTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            TimeSpan actual;
            actual = target.Tdindwg;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Tducreate
        /// </summary>
        [TestMethod]
        public void TducreateTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            DateTime actual;
            actual = target.Tducreate;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Tdupdate
        /// </summary>
        [TestMethod]
        public void TdupdateTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            DateTime actual;
            actual = target.Tdupdate;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Tdusrtimer
        /// </summary>
        [TestMethod]
        public void TdusrtimerTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            TimeSpan actual;
            actual = target.Tdusrtimer;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Tduupdate
        /// </summary>
        [TestMethod]
        public void TduupdateTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            DateTime actual;
            actual = target.Tduupdate;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for TextStyleTableId
        /// </summary>
        [TestMethod]
        public void TextStyleTableIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.TextStyleTableId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Textsize
        /// </summary>
        [TestMethod]
        public void TextsizeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Textsize = expected;
            actual = target.Textsize;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Textstyle
        /// </summary>
        [TestMethod]
        public void TextstyleTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.Textstyle = expected;
            actual = target.Textstyle;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Thickness
        /// </summary>
        [TestMethod]
        public void ThicknessTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Thickness = expected;
            actual = target.Thickness;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for ThumbnailBitmap
        /// </summary>
        [TestMethod]
        public void ThumbnailBitmapTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Bitmap expected = null; // TODO: Initialize to an appropriate value
            Bitmap actual;
            target.ThumbnailBitmap = expected;
            actual = target.ThumbnailBitmap;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for TileMode
        /// </summary>
        [TestMethod]
        public void TileModeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.TileMode = expected;
            actual = target.TileMode;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for TileModeLightSynch
        /// </summary>
        [TestMethod]
        public void TileModeLightSynchTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.TileModeLightSynch = expected;
            actual = target.TileModeLightSynch;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for TimeZone
        /// </summary>
        [TestMethod]
        public void TimeZoneTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new TimeZone(); // TODO: Initialize to an appropriate value
            TimeZone actual;
            target.TimeZone = expected;
            actual = target.TimeZone;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Tracewid
        /// </summary>
        [TestMethod]
        public void TracewidTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Tracewid = expected;
            actual = target.Tracewid;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for TransactionManager
        /// </summary>
        [TestMethod]
        public void TransactionManagerTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            TransactionManager actual;
            actual = target.TransactionManager;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Treedepth
        /// </summary>
        [TestMethod]
        public void TreedepthTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Treedepth = expected;
            actual = target.Treedepth;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for TstackSize
        /// </summary>
        [TestMethod]
        public void TstackSizeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.TstackSize = expected;
            actual = target.TstackSize;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for UcsBase
        /// </summary>
        [TestMethod]
        public void UcsBaseTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.UcsBase = expected;
            actual = target.UcsBase;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for UcsOrthographic
        /// </summary>
        [TestMethod]
        public void UcsOrthographicTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            OrthographicView actual;
            actual = target.UcsOrthographic;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for UcsTableId
        /// </summary>
        [TestMethod]
        public void UcsTableIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.UcsTableId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Ucsname
        /// </summary>
        [TestMethod]
        public void UcsnameTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.Ucsname;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Ucsorg
        /// </summary>
        [TestMethod]
        public void UcsorgTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Point3d actual;
            actual = target.Ucsorg;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Ucsxdir
        /// </summary>
        [TestMethod]
        public void UcsxdirTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Vector3d actual;
            actual = target.Ucsxdir;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Ucsydir
        /// </summary>
        [TestMethod]
        public void UcsydirTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            Vector3d actual;
            actual = target.Ucsydir;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for UndoRecording
        /// </summary>
        [TestMethod]
        public void UndoRecordingTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.UndoRecording;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Unitmode
        /// </summary>
        [TestMethod]
        public void UnitmodeTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Unitmode = expected;
            actual = target.Unitmode;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for UpdateThumbnail
        /// </summary>
        [TestMethod]
        public void UpdateThumbnailTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.UpdateThumbnail = expected;
            actual = target.UpdateThumbnail;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Useri1
        /// </summary>
        [TestMethod]
        public void Useri1Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Useri1 = expected;
            actual = target.Useri1;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Useri2
        /// </summary>
        [TestMethod]
        public void Useri2Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Useri2 = expected;
            actual = target.Useri2;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Useri3
        /// </summary>
        [TestMethod]
        public void Useri3Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Useri3 = expected;
            actual = target.Useri3;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Useri4
        /// </summary>
        [TestMethod]
        public void Useri4Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Useri4 = expected;
            actual = target.Useri4;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Useri5
        /// </summary>
        [TestMethod]
        public void Useri5Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Useri5 = expected;
            actual = target.Useri5;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Userr1
        /// </summary>
        [TestMethod]
        public void Userr1Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Userr1 = expected;
            actual = target.Userr1;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Userr2
        /// </summary>
        [TestMethod]
        public void Userr2Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Userr2 = expected;
            actual = target.Userr2;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Userr3
        /// </summary>
        [TestMethod]
        public void Userr3Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Userr3 = expected;
            actual = target.Userr3;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Userr4
        /// </summary>
        [TestMethod]
        public void Userr4Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Userr4 = expected;
            actual = target.Userr4;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Userr5
        /// </summary>
        [TestMethod]
        public void Userr5Test()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.Userr5 = expected;
            actual = target.Userr5;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Usrtimer
        /// </summary>
        [TestMethod]
        public void UsrtimerTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Usrtimer = expected;
            actual = target.Usrtimer;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for VersionGuid
        /// </summary>
        [TestMethod]
        public void VersionGuidTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.VersionGuid = expected;
            actual = target.VersionGuid;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for ViewTableId
        /// </summary>
        [TestMethod]
        public void ViewTableIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.ViewTableId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for ViewportScaleDefault
        /// </summary>
        [TestMethod]
        public void ViewportScaleDefaultTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            target.ViewportScaleDefault = expected;
            actual = target.ViewportScaleDefault;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for ViewportTableId
        /// </summary>
        [TestMethod]
        public void ViewportTableIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.ViewportTableId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Visretain
        /// </summary>
        [TestMethod]
        public void VisretainTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Visretain = expected;
            actual = target.Visretain;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for VisualStyleDictionaryId
        /// </summary>
        [TestMethod]
        public void VisualStyleDictionaryIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.VisualStyleDictionaryId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for Worldview
        /// </summary>
        [TestMethod]
        public void WorldviewTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Worldview = expected;
            actual = target.Worldview;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for XclipFrame
        /// </summary>
        [TestMethod]
        public void XclipFrameTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.XclipFrame = expected;
            actual = target.XclipFrame;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for XrefBlockId
        /// </summary>
        [TestMethod]
        public void XrefBlockIdTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            actual = target.XrefBlockId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for XrefEditEnabled
        /// </summary>
        [TestMethod]
        public void XrefEditEnabledTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.XrefEditEnabled = expected;
            actual = target.XrefEditEnabled;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///     A test for dragvs
        /// </summary>
        [TestMethod]
        public void dragvsTest()
        {
            var target = new Database(); // TODO: Initialize to an appropriate value
            var expected = new ObjectId(); // TODO: Initialize to an appropriate value
            ObjectId actual;
            target.dragvs = expected;
            actual = target.dragvs;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion
    }

    public class Vector3d
    {
    }

    public class DBObject
    {
    }

    public class IdMapping
    {
    }

    public class ObjectId
    {
    }

    public class Database
    {
        public bool XrefEditEnabled { get; set; }
        public ObjectId dragvs { get; set; }

        public int XclipFrame { get; set; }

        public ObjectId XrefBlockId { get; set; }

        public bool Worldview { get; set; }

        public ObjectId VisualStyleDictionaryId { get; set; }

        public bool Visretain { get; set; }

        public ObjectId ViewportTableId { get; set; }

        public double ViewportScaleDefault { get; set; }

        public ObjectId ViewTableId { get; set; }

        public string VersionGuid { get; set; }

        public bool Usrtimer { get; set; }

        public double Userr5 { get; set; }

        public double Userr4 { get; set; }

        public double Userr3 { get; set; }

        public double Userr2 { get; set; }

        public double Userr1 { get; set; }

        public int Useri5 { get; set; }

        public int Useri4 { get; set; }

        public int Useri3 { get; set; }

        public int Useri2 { get; set; }

        public int Useri1 { get; set; }

        public int UpdateThumbnail { get; set; }

        public int Unitmode { get; set; }

        public bool UndoRecording { get; set; }

        public Vector3d Ucsydir { get; set; }

        public void CloseInput(bool closeFile)
        {
            throw new NotImplementedException();
        }
    }
}