using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;


using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;



namespace Pge.Common.AutoCAD
{
    public class QueryManager : IDisposable
    {
        #region Private Members

        //private static readonly ILog //_logger = LogManager.GetLogger(typeof(QueryManager));
        private MapApplication _mapApp = Autodesk.Gis.Map.HostMapApplicationServices.Application;
        private ProjectModel _projectModel = null;
        private IList<String> _attachedDwgs;

        private Point3dCollection _points;
        private LocationType _locationType = LocationType.LocationCrossing;
        private bool _useBoundry = false;
        private bool _autoDetach = true;
        private QueryType _queryType = QueryType.QueryDraw;
        private IList<string> _layerFilters = new List<string>();
        private IList<string> _objectFilters = new List<string>();
        private IList<string> _blockNameFilters = new List<string>();
        private string _colorPropertyAlteration;
        private string _layerPropertyAlteration;
        private string _lineTypePropertyAlteration;
        private double _blockScalePropertyAlteration;
        private string _blockRangeTableName;
        private string _blockDefinitionFolder;
        private IList<RangeTableBlockMapping> _blockPropertyAlterations = new List<RangeTableBlockMapping>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryManager"/> class.
        /// </summary>
        public QueryManager()
        {
            _mapApp = Autodesk.Gis.Map.HostMapApplicationServices.Application;
            _projectModel = _mapApp.ActiveProject;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryManager"/> class.
        /// </summary>
        /// <param name="db">The db.</param>
        public QueryManager(Database db)
        {
            _mapApp = Autodesk.Gis.Map.HostMapApplicationServices.Application;
            _projectModel = _mapApp.GetProjectForDB(db);            
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [auto detach].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [auto detach]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoDetach
        {
            get
            {
                return _autoDetach;
            }
            set
            {
                _autoDetach = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use boundry].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use boundry]; otherwise, <c>false</c>.
        /// </value>
        public bool UseBoundry
        {
            get
            {
                return _useBoundry;
            }
            set
            {
                _useBoundry = value;
            }
        }

        /// <summary>
        /// Gets or sets the layer filter.
        /// </summary>
        /// <value>
        /// The layer filter.
        /// </value>
        public IList<string> LayerFilters
        {
            get
            {
                return _layerFilters;
            }
            set
            {
                _layerFilters = value;
            }
        }

        /// <summary>
        /// Gets or sets the property filters.
        /// </summary>
        /// <value>
        /// The property filters.
        /// </value>
        public IList<string> ObjectFilters
        {
            get
            {
                return _objectFilters;
            }
            set
            {
                _objectFilters = value;
            }
        }

        /// <summary>
        /// Gets or sets the block name filters.
        /// </summary>
        /// <value>
        /// The block name filters.
        /// </value>
        public IList<string> BlockNameFilters
        {
            get
            {
                return _blockNameFilters;
            }
            set
            {
                _blockNameFilters = value;
            }
        }

        /// <summary>
        /// Gets or sets the block scale property alteration.
        /// </summary>
        /// <value>
        /// The block scale property alteration.
        /// </value>
        public double BlockScalePropertyAlteration
        {
            get
            {
                return _blockScalePropertyAlteration;
            }
            set
            {
                _blockScalePropertyAlteration = value;
            }
        }

        /// <summary>
        /// Gets or sets the color property alteration.
        /// </summary>
        /// <value>
        /// The color property alteration.
        /// </value>
        public string ColorPropertyAlteration
        {
            get
            {
                return _colorPropertyAlteration;
            }
            set
            {
                _colorPropertyAlteration = value;
            }
        }

        /// <summary>
        /// Gets or sets the layer property alteration.
        /// </summary>
        /// <value>
        /// The layer property alteration.
        /// </value>
        public string LayerPropertyAlteration
        {
            get
            {
                return _layerPropertyAlteration;
            }
            set
            {
                _layerPropertyAlteration = value;
            }
        }

        /// <summary>
        /// Gets or sets the linetype property alteration.
        /// </summary>
        /// <value>
        /// The linetype property alteration.
        /// </value>
        public string LineTypePropertyAlteration
        {
            get
            {
                return _lineTypePropertyAlteration;
            }
            set
            {
                _lineTypePropertyAlteration = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the block range table.
        /// </summary>
        /// <value>
        /// The name of the block range table.
        /// </value>
        public string BlockRangeTableName
        {
            get
            {
                return _blockRangeTableName;
            }
            set
            {
                _blockRangeTableName = value;
            }
        }

        /// <summary>
        /// Gets or sets the block definition folder.
        /// </summary>
        /// <value>
        /// The block definition folder.
        /// </value>
        public string BlockDefinitionFolder
        {
            get
            {
                return _blockDefinitionFolder;
            }
            set
            {
                _blockDefinitionFolder = value;
            }
        }

        /// <summary>
        /// Gets or sets the block property alterations.
        /// </summary>
        /// <value>
        /// The block property alterations.
        /// </value>
        public IList<RangeTableBlockMapping> BlockPropertyAlterations
        {
            get
            {
                return _blockPropertyAlterations;
            }
            set
            {
                _blockPropertyAlterations = value;
            }
        }

        /// <summary>
        /// Gets or sets the Project Model.
        /// </summary>
        /// <value>
        /// ProjectModel object.
        /// </value>
        public ProjectModel ProjectModel
        {
            get
            {
                if (_projectModel == null)
                {
                    _projectModel = _mapApp.ActiveProject;
                }
                return _projectModel;
            }
        }

        /// <summary>
        /// Sets the selection points.
        /// </summary>
        /// <value>
        /// The selection points.
        /// </value>
        public Point3dCollection SelectionPoints
        {
            get
            {
                return _points;
            }
            set
            {
                _points = value;
            }
        }

        /// <summary>
        /// Sets the type of the location.
        /// </summary>
        /// <value>
        /// The type of the location.
        /// </value>
        public LocationType LocationType
        {
            get
            {
                return _locationType;
            }
            set
            {
                _locationType = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the query.
        /// </summary>
        /// <value>
        /// The type of the query.
        /// </value>
        public QueryType QueryType
        {
            get
            {
                return _queryType;
            }
            set
            {
                _queryType = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [Create Selection Set From Queried Objects].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [Create Selection Set From Queried Objects]; otherwise, <c>false</c>.
        /// </value>
        public bool CreateSelectionSetFromQueriedObjects
        {
            get
            {
                return this.ProjectModel.Options.MkSelSetWithQryObj;
            }
            set
            {
                this.ProjectModel.Options.MkSelSetWithQryObj = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [set dont add objects to save set].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [set dont add objects to save set]; otherwise, <c>false</c>.
        /// </value>
        public bool DontAddObjectsToSaveSet
        {
            get
            {
                return this.ProjectModel.Options.DontAddObjectsToSaveSet;
            }
            set
            {
                this.ProjectModel.Options.DontAddObjectsToSaveSet = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [set dont add objects to save set].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [set dont add objects to save set]; otherwise, <c>false</c>.
        /// </value>
        public bool EraseSavedBackObjects
        {
            get
            {
                return this.ProjectModel.Options.EraseSavedBackObjects;
            }
            set
            {
                this.ProjectModel.Options.EraseSavedBackObjects = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [set mark objects for editing without prompting].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [set mark objects for editing without prompting]; otherwise, <c>false</c>.
        /// </value>
        public bool MarkObjectsForEditingWithoutPrompting
        {
            get
            {
                return this.ProjectModel.Options.MarkObjectsForEditingWithoutPrompting;
            }
            set
            {
                this.ProjectModel.Options.MarkObjectsForEditingWithoutPrompting = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [set show preview image as boundary only].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [set show preview image as boundary only]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowPreviewImageAsBoundaryOnly
        {
            get
            {
                return this.ProjectModel.Options.ShowPreviewImageAsBoundaryOnly;
            }
            set
            {
                this.ProjectModel.Options.ShowPreviewImageAsBoundaryOnly = value;
            }
        }

        #endregion

        #region Public Methods

        #region MapOptions

        /// <summary>
        /// Sets the dont add objects to save set.
        /// </summary>
        /// <param name="valueToSet">if set to <c>true</c> [value to set].</param>
        public void SetDontAddObjectsToSaveSet(bool valueToSet)
        {
            this.ProjectModel.Options.DontAddObjectsToSaveSet = valueToSet;
        }

        /// <summary>
        /// Sets the mark objects for editing without prompting.
        /// </summary>
        /// <param name="valueToSet">if set to <c>true</c> [value to set].</param>
        public void SetMarkObjectsForEditingWithoutPrompting(bool valueToSet)
        {
            this.ProjectModel.Options.MarkObjectsForEditingWithoutPrompting = valueToSet;
        }

        /// <summary>
        /// Sets the show preview image as boundary only.
        /// </summary>
        /// <param name="valueToSet">if set to <c>true</c> [value to set].</param>
        public void SetShowPreviewImageAsBoundaryOnly(bool valueToSet)
        {
            this.ProjectModel.Options.ShowPreviewImageAsBoundaryOnly = valueToSet;
        }

        #endregion

        /// <summary>
        /// Defines the query.
        /// </summary>
        /// <param name="queryModel">The query model.</param>
        /// <returns></returns>
        private bool DefineQuery(QueryModel queryModel)
        {
            //_logger.Info("Defining AutoCAD Map Query...");

            bool retVal = false;

            //_logger.Debug("Start DefineQuery");
            try
            {
                // create query branch
                //
                QueryBranch queryBranch = QueryBranch.Create();

                // create query conditions
                //
                IList<QueryCondition> queryConditions = new List<QueryCondition>();

                // location filter
                //
                if (this.UseBoundry)
                {
                    //_logger.Info("Defining AutoCAD Map Location Query...");
                    // define the window based on the users selection points
                    //
                    LocationCondition qryCondition = new LocationCondition();
                    qryCondition.JoinOperator = JoinOperator.OperatorAnd;
                    qryCondition.LocationType = this.LocationType;
                    LocationBoundary boundary = new PolygonBoundary(this.SelectionPoints);
                    qryCondition.Boundary = boundary;
                    queryBranch.AppendOperand(qryCondition);
                    queryConditions.Add(qryCondition);
                }

                // object filter
                //
                if (this.ObjectFilters.Count == 0)
                {
                    //_logger.Info("Defining AutoCAD Map Property * Query...");
                    PropertyCondition propCondition = new PropertyCondition();
                    propCondition.JoinOperator = JoinOperator.OperatorAnd;
                    propCondition.PropertyType = PropertyType.EntityType;
                    propCondition.Value = "*";
                    queryBranch.AppendOperand(propCondition);
                    queryConditions.Add(propCondition);
                }
                else
                {
                    bool isFirstObjectType = true;
                    int firstIndex = 0;
                    int lastIndex = 0;

                    firstIndex = queryBranch.OperandCount;
                    //_logger.Info("Defining AutoCAD Map Property Query...");
                    foreach (string objectFilter in this.ObjectFilters)
                    {
                        PropertyCondition propCondition = new PropertyCondition();
                        if (isFirstObjectType == true)
                        {
                            propCondition.JoinOperator = JoinOperator.OperatorAnd;
                            isFirstObjectType = false;
                        }
                        else
                        {
                            propCondition.JoinOperator = JoinOperator.OperatorOr;
                        }
                        propCondition.PropertyType = PropertyType.EntityType;
                        propCondition.Value = objectFilter;
                        queryBranch.AppendOperand(propCondition);
                        queryConditions.Add(propCondition);
                    }

                    //TODO: Why does queryBranch.Group not work?
                    if (this.ObjectFilters.Count > 1)
                    {
                        lastIndex = queryBranch.OperandCount - firstIndex;
                        lastIndex -= 1;
                        queryBranch.Group(firstIndex, lastIndex);
                    }
                }

                // block name filter
                //
                if (this.BlockNameFilters.Count > 0)
                {
                    bool isFirstBlockName = true;
                    int firstIndex = 0;
                    int lastIndex = 0;

                    firstIndex = queryBranch.OperandCount;
                    //_logger.Info("Defining AutoCAD Map Block Name Query...");
                    foreach (string blockNameFilter in this.BlockNameFilters)
                    {
                        PropertyCondition propCondition = new PropertyCondition();
                        if (isFirstBlockName == true)
                        {
                            propCondition.JoinOperator = JoinOperator.OperatorAnd;
                            isFirstBlockName = false;
                        }
                        else
                        {
                            propCondition.JoinOperator = JoinOperator.OperatorOr;
                        }
                        propCondition.PropertyType = PropertyType.BlockName;
                        propCondition.Value = blockNameFilter;
                        queryBranch.AppendOperand(propCondition);
                        queryConditions.Add(propCondition);
                    }

                    //TODO: Why does queryBranch.Group not work?
                    if (this.BlockNameFilters.Count > 1)
                    {
                        lastIndex = queryBranch.OperandCount - firstIndex;
                        lastIndex -= 1;
                        queryBranch.Group(firstIndex, lastIndex);
                    }
                }

                // layer filter
                //
                foreach (string layerFilter in this.LayerFilters)
                {
                    PropertyCondition layerPropertyCondition = new PropertyCondition();
                    layerPropertyCondition.JoinOperator = JoinOperator.OperatorAnd;
                    layerPropertyCondition.PropertyType = PropertyType.Layer;
                    layerPropertyCondition.Value = layerFilter;
                    queryBranch.AppendOperand(layerPropertyCondition);
                    queryConditions.Add(layerPropertyCondition);
                }

                // Define query
                //
                queryModel.Define(queryBranch);

                // clean up
                foreach (QueryCondition pc in queryConditions)
                {
                    pc.Dispose();
                }
                queryBranch.Dispose();

                retVal = true;
            }
            catch (System.Exception ex)
            {
                //_logger.Error("Error defining query", ex);
                throw;
                //AcadUtilities.WriteMessage("\nError defining query");
            }
            //_logger.Debug("End DefineQuery");
            return retVal;
        }

        /// <summary>
        /// Defines the property alterations.
        /// </summary>
        /// <param name="queryModel">The query model.</param>
        private bool DefinePropertyAlterations(QueryModel queryModel)
        {
            //_logger.Info("Defining AutoCAD Map Property Alterations...");

            bool retVal = false;

            //_logger.Debug("Start DefinePropertyAlterations");
            try
            {
                // Clear any defined property alterations
                queryModel.PropertyAlteration.Clear();
                queryModel.EnablePropertyAlteration(false);

                // Define Block Scale Property Alterations
                if (this.BlockScalePropertyAlteration > 0)
                {
                    PropertyAlteration blockScaleAlteration = queryModel.PropertyAlteration.AddAlteration(AlterationType.AlterationScale);
                    blockScaleAlteration.Expression = this.BlockScalePropertyAlteration.ToString();
                    queryModel.EnablePropertyAlteration(true);
                }

                // Define Color Property Alterations
                if (!String.IsNullOrEmpty(this.ColorPropertyAlteration))
                {
                    PropertyAlteration colorAlteration = queryModel.PropertyAlteration.AddAlteration(AlterationType.AlterationColor);
                    colorAlteration.Expression = this.ColorPropertyAlteration;
                    queryModel.EnablePropertyAlteration(true);
                }

                // Define Layer Property Alterations
                if (!String.IsNullOrEmpty(this.LayerPropertyAlteration))
                {
                    PropertyAlteration layerAlteration = queryModel.PropertyAlteration.AddAlteration(AlterationType.AlterationLayer);
                    layerAlteration.Expression = this.LayerPropertyAlteration;
                    queryModel.EnablePropertyAlteration(true);
                }

                // Define LineType Property Alterations
                if (!String.IsNullOrEmpty(this.LineTypePropertyAlteration))
                {
                    PropertyAlteration lineTypeAlteration = queryModel.PropertyAlteration.AddAlteration(AlterationType.AlterationLineType);
                    lineTypeAlteration.Expression = this.LineTypePropertyAlteration;
                    queryModel.EnablePropertyAlteration(true);
                }

                // TextStyle Property Alteration?
                // TextHeight Property Alteration?

                if (this.BlockPropertyAlterations.Count > 0)
                {
                    // Be sure all target blocks are defined in current drawing
                    DefineAllTargetBlocks();

                    // Define BlockName Property Alterations using Range Table
                    string rangeTableNameBase = this.BlockRangeTableName;

                    // add block name alterations
                    int rangeTableIndex = 1;
                    foreach (RangeTableBlockMapping blockMapping in this.BlockPropertyAlterations)
                    {
                        string rangeTableName = rangeTableNameBase + "_" + rangeTableIndex.ToString();
                        rangeTableIndex++;

                        // Delete existing range table if defined
                        for (int i = 0; i < this.ProjectModel.RangeTables.RangeTableCount; i++)
                        {
                            RangeTable rt = this.ProjectModel.RangeTables[i];
                            if (rt.Name.CompareTo(rangeTableName) == 0)
                            {
                                this.ProjectModel.RangeTables.RemoveRangeTable(i);
                                break;
                            }
                        }

                        // create a new range table
                        RangeTable rangeTable = this.ProjectModel.RangeTables.AddRangeTable(rangeTableName, rangeTableName);
                        foreach (string sourceBlockName in blockMapping.SourceBlockNames)
                        {
                            rangeTable.AddRangeLine(RangeOperator.RangeEqual, sourceBlockName, blockMapping.TargetBlockName);
                        }

                        // if any ranges were added, add a property alteration to support them
                        if (rangeTable.RangeLinesCount > 0)
                        {
                            PropertyAlteration rangeTableBlockAlteration = queryModel.PropertyAlteration.AddAlteration(AlterationType.AlterationBlockName);
                            string rangeTableExpression = "(Range .BLOCKNAME " + rangeTableName + ")";
                            rangeTableBlockAlteration.Expression = rangeTableExpression;
                            queryModel.EnablePropertyAlteration(true);
                        }
                        else
                        {
                            queryModel.Project.RangeTables.RemoveRangeTable(rangeTableName);
                        }
                    }
                }
                retVal = true;
            }
            catch (System.Exception ex)
            {
                //_logger.Error("Error defining property alterations", ex);
                throw;
                //AcadUtilities.WriteMessage("\nError defining property alterations");
            }
            //_logger.Debug("End DefinePropertyAlterations");
            return retVal;
        }

        /// <summary>
        /// Defines all target blocks.
        /// </summary>
        /// <returns></returns>
        private bool DefineAllTargetBlocks()
        {
            bool retVal = true;

            //_logger.Debug("Start DefineAllTargetBlocks");
            Database db = _projectModel.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                foreach (RangeTableBlockMapping blockMapping in this.BlockPropertyAlterations)
                {
                    if (!BlockManager.IsBlockDefined(db, trans, blockMapping.TargetBlockName))
                    {
                        if (!BlockManager.DefineBlockFromFileOnDisk(db, trans, blockMapping.TargetBlockName, this.BlockDefinitionFolder))
                        {
                            retVal = false;
                        }
                    }
                }
                trans.Commit();
            }
            retVal = true;

            //_logger.Debug("End DefineAllTargetBlocks");
            return retVal;
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        public ObjectIdCollection ExecuteQuery()
        {
            //_logger.Info("Executing AutoCAD Map Query...");

            //_logger.Debug("Start ExecuteQuery");
            ObjectIdCollection oids = new ObjectIdCollection();

            // create a new query
            //
            QueryModel queryModel = this.ProjectModel.CreateQuery(true);

            // Define the query
            if (!DefineQuery(queryModel))
            {
                return oids;
            }

            // Define the property alterations
            if (!DefinePropertyAlterations(queryModel))
            {
                return oids;
            }

            // set type of query: draw or preview
            queryModel.Mode = this.QueryType;

            // Execute query to return an ObjectIdCollection
            oids = queryModel.Execute(this.ProjectModel.DrawingSet);

            // clean up
            queryModel.Clear();
            queryModel.Dispose();

            //_logger.Debug("End ExecuteQuery");
            return oids;
        }

        /// <summary>
        /// Runs the query. No object id collection generated or returned
        /// </summary>
        public void RunQuery()
        {
            //_logger.Info("Running AutoCAD Map Query...");

            //_logger.Debug("Start RunQuery");
            // create a new query
            //
            QueryModel queryModel = this.ProjectModel.CreateQuery(true);

            // Define the query
            if (!DefineQuery(queryModel))
            {
                return;
            }

            // set type of query: draw or preview
            queryModel.Mode = this.QueryType;

            // Execute query
            queryModel.Run();

            // clean up
            queryModel.Clear();
            queryModel.Dispose();
            //_logger.Debug("End RunQuery");
        }

        #region DrawingSet

        /// <summary>
        /// Actvates the drawings.
        /// </summary>
        /// <param name="drawingNames">The drawing names.</param>
        public void ActivateDrawings(IList<string> drawingNames)
        {
            //_logger.Info("Actvating drawings...");

            //_logger.Debug("Start ActivateDrawings");
            // loop through all the drawing in the list
            // if the drawing is attached and inactive, activate it
            //
            foreach (string dwg in drawingNames)
            {
                try
                {
                    AttachedDrawing oneAttachedDrawing = this.ProjectModel.DrawingSet.DirectAttachedDrawings[dwg];
                    if (oneAttachedDrawing.ActiveStatus == AdeDwgStatus.DwgInactive)
                    {
                        oneAttachedDrawing.Activate();
                    }
                }
                catch (System.Exception ex)
                {
                    //_logger.Error("Error activating drawing: " + dwg, ex);
                    throw;
                    // AcadUtilities.WriteMessage("\nError activating drawing: " + dwg);
                }
            }
            //_logger.Debug("End ActivateDrawings");
        }

        /// <summary>
        /// Deactivates the drawings.
        /// </summary>
        /// <param name="drawingNames">The drawing names.</param>
        public void DeActivateDrawings(IList<string> drawingNames)
        {
            //_logger.Info("DeActvating drawings...");

            //_logger.Debug("Start DeActivateDrawings");
            // loop through all the drawing in the list
            // if the drawing is attached and active, DeActivate it
            //
            foreach (string dwg in drawingNames)
            {
                try
                {
                    AttachedDrawing oneAttachedDrawing = this.ProjectModel.DrawingSet.DirectAttachedDrawings[dwg];
                    if (oneAttachedDrawing.ActiveStatus == AdeDwgStatus.DwgActive)
                    {
                        oneAttachedDrawing.Deactivate();
                    }
                }
                catch (System.Exception ex)
                {
                    //_logger.Error("Error DeActivating drawing: " + dwg, ex);
                    throw;
                    //AcadUtilities.WriteMessage("\nError deactivating drawing: " + dwg);
                }
            }
            //_logger.Debug("End DeActivateDrawings");
        }

        /// <summary>
        /// Attaches the drawings.
        /// </summary>
        public void AttachDrawings(IList<string> drawingNames)
        {
            //_logger.Info("Attaching drawings...");

            //_logger.Debug("Start AttachDrawings");
            _attachedDwgs = new List<string>();

            // loop through all the drawing in the config file
            // if the file exists, attach it
            //
            foreach (string dwg in drawingNames)
            {
                try
                {
                    if (File.Exists(dwg))
                    {
                        this.ProjectModel.DrawingSet.AttachDrawing(dwg);
                        _attachedDwgs.Add(dwg);
                    }
                    else
                    {
                        //_logger.Debug("Could not find the drawing to attach: " + dwg);
                        AcadUtilities.WriteMessage("\nCould not find the drawing to attach: " + dwg);
                    }
                }
                catch (System.Exception ex)
                {
                    //_logger.Error("Error attaching drawing: " + dwg, ex);
                    throw;
                    //AcadUtilities.WriteMessage("\nError attaching drawing: " + dwg);
                }
            }
            //_logger.Debug("End AttachDrawings");
        }

        /// <summary>
        /// Detaches the drawings.
        /// </summary>
        public void DetachDrawings(IList<string> drawingNames)
        {
            //_logger.Info("Detaching drawings...");

            //_logger.Debug("Start DetachDrawings");
            // turn off the logging
            //
            //int logFileActiveValue = _mapApp.GetIntOptionValue("LogFileActive");
            //int logMessageLevelValue = _mapApp.GetIntOptionValue("LogMessageLevel");
            //_mapApp.SetIntOptionValue("LogFileActive", 1);
            //_mapApp.SetIntOptionValue("LogMessageLevel", 0);

            foreach (string dwg in drawingNames)
            {
                try
                {
                    this.ProjectModel.DrawingSet.DetachDrawing(dwg);
                }
                catch (System.Exception ex)
                {
                    //_logger.Error("Error detaching drawing: " + dwg, ex);
                    throw;
                    //AcadUtilities.WriteMessage("Error detaching drawing: " + dwg);
                }
            }

            // reset the logging
            //
            //_mapApp.SetIntOptionValue("LogMessageLevel", logMessageLevelValue);
            //_mapApp.SetIntOptionValue("LogFileActive", logFileActiveValue);
            //_logger.Debug("End DetachDrawings");

        }

        /// <summary>
        /// Gets the attached drawing by the drawing name.
        /// </summary>
        /// <param name="drawingName">Name of the drawing.</param>
        /// <returns></returns>
        public AttachedDrawing GetAttachedDrawingByName(string drawingName)
        {
            //_logger.Debug("Start GetAttachedDrawingByName");
            AttachedDrawing foundAttachedDrawing = null;
            AttachedDrawing attachedDrawing = this.ProjectModel.DrawingSet.DirectAttachedDrawings[drawingName];
            if (attachedDrawing != null)
            {
                foundAttachedDrawing = attachedDrawing;
            }
            //_logger.Debug("End GetAttachedDrawingByName");
            return foundAttachedDrawing;
        }

        #endregion

        #region SaveSet

        /// <summary>
        /// Adds the objects to save set.
        /// </summary>
        /// <param name="oids">The oids.</param>
        public void AddObjectsToSaveSet(ObjectIdCollection oids)
        {
            //_logger.Debug("Start AddObjectsToSaveSet");
            this.ProjectModel.SaveSet.AddObjects(oids);
            //_logger.Debug("End AddObjectsToSaveSet");
        }

        /// <summary>
        /// Removes all objects from save set.
        /// </summary>
        public void RemoveAllObjectsFromSaveSet()
        {
            //_logger.Debug("Start RemoveAllObjectsFromSaveSet");
            ObjectIdCollection oids = GetAllObjectsInSaveSet();
            if (oids.Count > 0)
            {
                this.ProjectModel.SaveSet.RemoveObjects(oids);
            }
            //_logger.Debug("End RemoveAllObjectsFromSaveSet");
        }

        /// <summary>
        /// Removes the objects from save set.
        /// </summary>
        /// <param name="oids">The oids.</param>
        public void RemoveObjectsFromSaveSet(ObjectIdCollection oids)
        {
            //_logger.Debug("Start RemoveObjectsFromSaveSet");
            this.ProjectModel.SaveSet.RemoveObjects(oids);
            //_logger.Debug("End RemoveObjectsFromSaveSet");
        }

        /// <summary>
        /// Gets all objects in save set.
        /// </summary>
        /// <returns></returns>
        public ObjectIdCollection GetAllObjectsInSaveSet()
        {
            //_logger.Debug("Start GetAllObjectsInSaveSet");
            ObjectIdCollection oids = new ObjectIdCollection();
            if (this.ProjectModel.SaveSet.GetObjectsCount(SaveSetObjectTypes.NewlyCreated) > 0)
            {
                ObjectIdCollection oidsNew = this.ProjectModel.SaveSet.GetObjects(SaveSetObjectTypes.NewlyCreated);
                foreach (ObjectId oid in oidsNew)
                {
                    oids.Add(oid);
                }
            }
            if (this.ProjectModel.SaveSet.GetObjectsCount(SaveSetObjectTypes.QueriedErased) > 0)
            {
                ObjectIdCollection oidsErased = this.ProjectModel.SaveSet.GetObjects(SaveSetObjectTypes.QueriedErased);
                foreach (ObjectId oid in oidsErased)
                {
                    oids.Add(oid);
                }
            }
            if (this.ProjectModel.SaveSet.GetObjectsCount(SaveSetObjectTypes.QueriedExisted) > 0)
            {
                ObjectIdCollection oidsQueried = this.ProjectModel.SaveSet.GetObjects(SaveSetObjectTypes.QueriedExisted);
                foreach (ObjectId oid in oidsQueried)
                {
                    oids.Add(oid);
                }
            }
            //_logger.Debug("End GetAllObjectsInSaveSet");
            return oids;
        }

        #endregion

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //_logger.Debug("Start Dispose");
            if (_mapApp != null)
            {
                _mapApp.Dispose();
            }
            if (_projectModel != null)
            {
                _projectModel.Dispose();
            }
            //_logger.Debug("End Dispose");
        }

        #endregion
    }
}

