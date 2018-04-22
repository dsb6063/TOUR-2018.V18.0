#region

using System;
using System.Windows.Forms;
using PGA.Controller;
using PGA.Model.BLL;

#endregion

namespace PGA.Views
{
    public partial class Settings : Form, ISettingsController
    {
        private Settings _settings;
        private SettingsController _settingsController;
        private LayerStatesBLL _states;

        public void LoadDataSet(LayerStatesBLL states)
        {
            TxtBridgeLayer = GetLayer(states.Bridge);
            TxtBridgeCode = GetCode(states.Bridge);
            TxtBunkerCode = GetCode(states.Bunker);
            TxtBunkerLayer = GetLayer(states.Bunker);
            TxtBuildingCode = GetCode(states.Building);
            TxtBuildingLayer = GetLayer(states.Building);
            TxtBushOutlineCode = GetCode(states.BushOutline);
            TxtBushOutlineLayer = GetLayer(states.BushOutline);
            TxtCartPathCode = GetCode(states.CartPath);
            TxtCartPathLayer = GetLayer(states.CartPath);
            TxtCollarCode = GetCode(states.Collar);
            TxtCollarLayer = GetLayer(states.Collar);
            TxtDirtOutlineCode = GetCode(states.DirtOutline);
            TxtDirtOutlineLayer = GetLayer(states.DirtOutline);
            TxtFairwayCode = GetCode(states.Fairway);
            TxtFairwayLayer = GetLayer(states.Fairway);
            TxtRockOutlineLayer = GetLayer(states.RockOutline);
            TxtRockOutlineCode = GetCode(states.RockOutline);
            TxtRoughOutlineCode = GetCode(states.RoughOutline);
            TxtRoughOutlineLayer = GetLayer(states.RoughOutline);
            TxtStepsCode = GetCode(states.Steps);
            TxtStepsLayer = GetLayer(states.Steps);
            TxtTeeBoxCode = GetCode(states.TeeBox);
            TxtTeeBoxLayer = GetLayer(states.TeeBox);
            TxtTreeOutlineCode = GetCode(states.TreeOutline);
            TxtTreeOutlineLayer = GetLayer(states.TreeOutline);
            TxtWalkStripCode = GetCode(states.WalkStrip);
            TxtWalkStripLayer = GetLayer(states.WalkStrip);
            TxtWallCode = GetCode(states.Wall);
            TxtWallLayer = GetLayer(states.Wall);
            TxtWaterCode = GetCode(states.Water);
            TxtWaterLayer = GetLayer(states.Water);
            TxtWaterDropCode = GetCode(states.WaterDrop);
            TxtWaterDropLayer = GetLayer(states.WaterDrop);
            TxtPathLayer = GetLayer(states.Path);
            TxtPathCode = GetCode(states.Path);
            TxtGreenLayer = GetLayer(states.Green);
            TxtGreenCode = GetCode(states.Green);
            TxtGreenSideBunkerCode = GetCode(states.GreenSideBunker);
            TxtGreenSideBunkerLayer = GetLayer(states.GreenSideBunker);
            TxtOtherCode = GetCode(states.Other);
            TxtOtherLayer = GetLayer(states.Other);
            TxtNativeAreaCode = GetCode(states.NativeArea);
            TxtNativeAreaLayer = GetLayer(states.NativeArea);
            TxtLandscapingCode = GetCode(states.LandScaping);
            TxtLandscapingLayer = GetLayer(states.LandScaping);
            TxtIntMedRoughCode = GetCode(states.IntMedRough);
            TxtIntMedRoughLayer = GetCode(states.IntMedRough);

            _states = states;
        }

        public void Settings_Load(object sender, EventArgs e)
        {
            _settings = this;
        }

        public LayerStatesBLL GetFormData()
        {
            _states.Bridge = TxtBridgeCode + ":" + TxtBridgeLayer;
            _states.Bunker = TxtBunkerCode + ":" + TxtBunkerLayer;
            _states.Building = TxtBuildingCode + ":" + TxtBuildingLayer;
            _states.BushOutline = TxtBushOutlineCode + ":" + TxtBuildingLayer;
            _states.CartPath = TxtCartPathCode + ":" + TxtCartPathLayer;
            _states.Collar = TxtCollarCode + ":" + TxtCollarLayer;
            _states.DirtOutline = TxtDirtOutlineCode + ":" + TxtDirtOutlineLayer;
            _states.Fairway = TxtFairwayCode + ":" + TxtFairwayLayer;
            _states.RockOutline = TxtRockOutlineCode + ":" + TxtRockOutlineLayer;
            _states.RoughOutline = TxtRoughOutlineCode + ":" + TxtRoughOutlineLayer;
            _states.Steps = TxtStepsCode + ":" + TxtStepsLayer;
            _states.TeeBox = TxtTeeBoxCode + ":" + TxtTeeBoxLayer;
            _states.TreeOutline = TxtTreeOutlineCode + ":" + TxtTreeOutlineLayer;
            _states.WalkStrip = TxtWalkStripCode + ":" + TxtWalkStripLayer;
            _states.Wall = TxtWallCode + ":" + TxtWallLayer;
            _states.Water = TxtWaterCode + ":" + TxtWaterLayer;
            _states.WaterDrop = TxtWaterDropCode + ":" + TxtPathLayer;
            _states.Path = TxtPathCode + ":" + TxtPathLayer;
            _states.Green = TxtGreenCode + ":" + TxtGreenLayer;
            _states.WaterDrop = TxtWaterDropCode + ":" + TxtWaterDropLayer;
            _states.WaterDrop = TxtWaterDropCode + ":" + TxtWaterDropLayer;
            _states.GreenSideBunker = TxtGreenSideBunkerCode + ":" + TxtGreenSideBunkerLayer;
            _states.Other = TxtOtherCode + ":" + TxtOtherLayer;
            _states.NativeArea = TxtNativeAreaCode + ":" + TxtNativeAreaLayer;
            _states.LandScaping = TxtLandscapingCode + ":" + TxtLandscapingLayer;
            _states.IntMedRough = TxtIntMedRoughCode + ":" + TxtIntMedRoughLayer;


            return _states;
        }

        public string GetCode(string code)
        {
            try
            {
                var res = code.Split(':')[0];
                return res ?? "";
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.AddLog(ex.Message);
                return "";
            }
        }

        public string GetLayer(string code)
        {
            try
            {
                var res = code.Split(':')[1];
                return res ?? "";
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.AddLog(ex.Message);
                return "";
            }
        }

        #region Properties

        public string TxtPointCloudFile
        {
            get { return txtPointCloudFile.Text; }
            set { txtPointCloudFile.Text = value; }
        }

        public string TxtExportPath
        {
            get { return txtExportPath.Text; }
            set { txtExportPath.Text = value; }
        }

        public string TxtDrawingTemplate
        {
            get { return txtDrawingTemplate.Text; }
            set { txtDrawingTemplate.Text = value; }
        }

        public string TxtFairwayLayer
        {
            get { return txtFairwayLayer.Text; }
            set { txtFairwayLayer.Text = value; }
        }

        public string TxtFairwayCode
        {
            get { return txtFairwayCode.Text; }
            set { txtFairwayCode.Text = value; }
        }

        public string TxtDirtOutlineLayer
        {
            get { return txtDirtOutlineLayer.Text; }
            set { txtDirtOutlineLayer.Text = value; }
        }

        public string TxtDirtOutlineCode
        {
            get { return txtDirtOutlineCode.Text; }
            set { txtDirtOutlineCode.Text = value; }
        }

        public string TxtCollarLayer
        {
            get { return txtCollarLayer.Text; }
            set { txtCollarLayer.Text = value; }
        }

        public string TxtCollarCode
        {
            get { return txtCollarCode.Text; }
            set { txtCollarCode.Text = value; }
        }

        public string TxtCartPathLayer
        {
            get { return txtCartPathLayer.Text; }
            set { txtCartPathLayer.Text = value; }
        }

        public string TxtCartPathCode
        {
            get { return txtCartPathCode.Text; }
            set { txtCartPathCode.Text = value; }
        }

        public string TxtBushOutlineLayer
        {
            get { return txtBushOutlineLayer.Text; }
            set { txtBushOutlineLayer.Text = value; }
        }

        public string TxtBushOutlineCode
        {
            get { return txtBushOutlineCode.Text; }
            set { txtBushOutlineCode.Text = value; }
        }

        public string TxtBunkerLayer
        {
            get { return txtBunkerLayer.Text; }
            set { txtBunkerLayer.Text = value; }
        }

        public string TxtBunkerCode
        {
            get { return txtBunkerCode.Text; }
            set { txtBunkerCode.Text = value; }
        }

        public string TxtBuildingLayer
        {
            get { return txtBuildingLayer.Text; }
            set { txtBuildingLayer.Text = value; }
        }

        public string TxtBuildingCode
        {
            get { return txtBuildingCode.Text; }
            set { txtBuildingCode.Text = value; }
        }

        public string TxtBridgeLayer
        {
            get { return txtBridgeLayer.Text; }
            set { txtBridgeLayer.Text = value; }
        }

        public string TxtBridgeCode
        {
            get { return txtBridgeCode.Text; }
            set { txtBridgeCode.Text = value; }
        }

        public string TxtWaterDropLayer
        {
            get { return txtWaterDropLayer.Text; }
            set { txtWaterDropLayer.Text = value; }
        }

        public string TxtWaterDropCode
        {
            get { return txtWaterDropCode.Text; }
            set { txtWaterDropCode.Text = value; }
        }

        public string TxtWaterLayer
        {
            get { return txtWaterLayer.Text; }
            set { txtWaterLayer.Text = value; }
        }

        public string TxtWaterCode
        {
            get { return txtWaterCode.Text; }
            set { txtWaterCode.Text = value; }
        }

        public string TxtWallLayer
        {
            get { return txtWallLayer.Text; }
            set { txtWallLayer.Text = value; }
        }

        public string TxtWallCode
        {
            get { return txtWallCode.Text; }
            set { txtWallCode.Text = value; }
        }

        public string TxtWalkStripLayer
        {
            get { return txtWalkStripLayer.Text; }
            set { txtWalkStripLayer.Text = value; }
        }

        public string TxtWalkStripCode
        {
            get { return txtWalkStripCode.Text; }
            set { txtWalkStripCode.Text = value; }
        }

        public string TxtTreeOutlineLayer
        {
            get { return txtTreeOutlineLayer.Text; }
            set { txtTreeOutlineLayer.Text = value; }
        }

        public string TxtTreeOutlineCode
        {
            get { return txtTreeOutlineCode.Text; }
            set { txtTreeOutlineCode.Text = value; }
        }

        public string TxtTeeBoxLayer
        {
            get { return txtTeeBoxLayer.Text; }
            set { txtTeeBoxLayer.Text = value; }
        }

        public string TxtTeeBoxCode
        {
            get { return txtTeeBoxCode.Text; }
            set { txtTeeBoxCode.Text = value; }
        }

        public string TxtStepsLayer
        {
            get { return txtStepsLayer.Text; }
            set { txtStepsLayer.Text = value; }
        }

        public string TxtStepsCode
        {
            get { return txtStepsCode.Text; }
            set { txtStepsCode.Text = value; }
        }

        public string TxtRoughOutlineLayer
        {
            get { return txtRoughOutlineLayer.Text; }
            set { txtRoughOutlineLayer.Text = value; }
        }

        public string TxtRoughOutlineCode
        {
            get { return txtRoughOutlineCode.Text; }
            set { txtRoughOutlineCode.Text = value; }
        }

        public string TxtRockOutlineLayer
        {
            get { return txtRockOutlineLayer.Text; }
            set { txtRockOutlineLayer.Text = value; }
        }

        public string TxtRockOutlineCode
        {
            get { return txtRockOutlineCode.Text; }
            set { txtRockOutlineCode.Text = value; }
        }

        public string TxtPathCode
        {
            get { return txtPathCode.Text; }
            set { txtPathCode.Text = value; }
        }

        public string TxtPathLayer
        {
            get { return txtPathLayer.Text; }
            set { txtPathLayer.Text = value; }
        }

        public string TxtOtherLayer
        {
            get { return txtOtherLayer.Text; }
            set { txtOtherLayer.Text = value; }
        }

        public string TxtOtherCode
        {
            get { return txtOtherCode.Text; }
            set { txtOtherCode.Text = value; }
        }

        public string TxtNativeAreaLayer
        {
            get { return txtNativeAreaLayer.Text; }
            set { txtNativeAreaLayer.Text = value; }
        }

        public string TxtNativeAreaCode
        {
            get { return txtNativeAreaCode.Text; }
            set { txtNativeAreaCode.Text = value; }
        }

        public string TxtLandscapingLayer
        {
            get { return txtLandscapingLayer.Text; }
            set { txtLandscapingLayer.Text = value; }
        }

        public string TxtLandscapingCode
        {
            get { return txtLandscapingCode.Text; }
            set { txtLandscapingCode.Text = value; }
        }

        public string TxtIntMedRoughLayer
        {
            get { return txtIntMedRoughLayer.Text; }
            set { txtIntMedRoughLayer.Text = value; }
        }

        public string TxtIntMedRoughCode
        {
            get { return txtIntMedRoughCode.Text; }
            set { txtIntMedRoughCode.Text = value; }
        }

        public string TxtGreenSideBunkerLayer
        {
            get { return txtGreenSideBunkerLayer.Text; }
            set { txtGreenSideBunkerLayer.Text = value; }
        }

        public string TxtGreenSideBunkerCode
        {
            get { return txtGreenSideBunkerCode.Text; }
            set { txtGreenSideBunkerCode.Text = value; }
        }

        public string TxtGreenLayer
        {
            get { return txtGreenLayer.Text; }
            set { txtGreenLayer.Text = value; }
        }

        public string TxtGreenCode
        {
            get { return txtGreenCode.Text; }
            set { txtGreenCode.Text = value; }
        }

        // public string 

        #endregion

        #region SetController

        public void SetController(Settings controller)
        {
            throw new NotImplementedException();
        }

        //public void SetController(SettingsController controller, PGALayerStates data)
        //{
        //    throw new NotImplementedException();
        //}

        public void SetController()
        {
            throw new NotImplementedException();
        }

        public void SetController(SettingsController controller)
        {
            _settingsController = controller;
            //_settingsController.LoadData();
        }

        #endregion

        #region Events raised back to controller

        private void cmdSave_Click(object sender, EventArgs e)
        {
            _settingsController.Save(GetFormData());
            // _settingsController.Save(GetPathData());
        }

        private LayerStatesBLL GetPathData()
        {
            throw new NotImplementedException();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cmdOpenDwgTemplate_Click(object sender, EventArgs e)
        {
            TxtDrawingTemplate = _settingsController.OpenDwgTemplate();
        }

        private void cmdOpenPointFile_Click(object sender, EventArgs e)
        {
            TxtPointCloudFile = _settingsController.OpenPointFile();
        }

        private void cmdOpenExportPath_Click(object sender, EventArgs e)
        {
            TxtExportPath = _settingsController.OpenExportPath();
        }

        #endregion
    }
}