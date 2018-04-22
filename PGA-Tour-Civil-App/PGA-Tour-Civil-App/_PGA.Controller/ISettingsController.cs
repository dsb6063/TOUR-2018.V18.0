#region

using PGA.Model.BLL;

#endregion

namespace PGA.Controller
{
    public interface ISettingsController
    {
        void SetController();
        void SetController(SettingsController controller);
        //void SetController(SettingsController controller, PGALayerStates data);
        void LoadDataSet(LayerStatesBLL states);
        //string gtxtExportPath { get; set; }
        //LayerStatesBLL gStates { get; set; }
        //#region Properties
        //string gtxtPointCloudFile { get; set; }
        //string gtxtDrawingTemplate { get; set; }
        //string gtxtFairwayLayer { get; set; }
        //string gtxtFairwayCode { get; set; }
        //string gtxtDirtOutlineLayer { get; set; }
        //string gtxtDirtOutlineCode { get; set; }
        //string gtxtCollarLayer { get; set; }
        //string gtxtCollarCode { get; set; }
        //string gtxtCartPathLayer { get; set; }
        //string gtxtCartPathCode { get; set; }
        //string gtxtBushOutlineLayer { get; set; }
        //string gtxtBushOutlineCode { get; set; }
        //string gtxtBunkerLayer { get; set; }
        //string gtxtBunkerCode { get; set; }
        //string gtxtBuildingLayer { get; set; }
        //string gtxtBuildingCode { get; set; }
        //string gtxtBridgeLayer { get; set; }
        //string gtxtBridgeCode { get; set; }
        //string gtxtWaterDropLayer { get; set; }
        //string gtxtWaterDropCode { get; set; }
        //string gtxtWaterLayer { get; set; }
        //string gtxtWaterCode { get; set; }
        //string gtxtWallLayer { get; set; }
        //string gtxtWallCode { get; set; }
        //string gtxtWalkStripLayer { get; set; }
        //string gtxtWalkStripCode { get; set; }
        //string gtxtTreeOutlineLayer { get; set; }
        //string gtxtTreeOutlineCode { get; set; }
        //string gtxtTeeBoxLayer { get; set; }
        //string gtxtTeeBoxCode { get; set; }
        //string gtxtStepsLayer { get; set; }
        //string gtxtStepsCode { get; set; }
        //string gtxtRoughOutlineLayer { get; set; }
        //string gtxtRoughOutlineCode { get; set; }
        //string gtxtRockOutlineLayer { get; set; }
        //string gtxtRockOutlineCode { get; set; }
        //string gtxtPathLayer { get; set; }
        //string gtxtPathCode { get; set; }
        //string gtxtOtherLayer { get; set; }
        //string gtxtOtherCode { get; set; }
        //string gtxtNativeAreaLayer { get; set; }
        //string gtxtNativeAreaCode { get; set; }
        //string gtxtLandscapingLayer { get; set; }
        //string gtxtLandscapingCode { get; set; }
        //string gtxtIntMedRoughLayer { get; set; }
        //string gtxtIntMedRoughCode { get; set; }
        //string gtxtGreenSideBunkerLayer { get; set; }
        //string gtxtGreenSideBunkerCode { get; set; }
        //string gtxtGreenLayer { get; set; }
        //string gtxtGreenCode { get; set; }


        //#endregion
    }
}