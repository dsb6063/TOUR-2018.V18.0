using System.Windows.Forms;
using PGA.Model.BLL;

namespace PGA.Views
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdOpenExportPath = new System.Windows.Forms.Button();
            this.cmdOpenPointFile = new System.Windows.Forms.Button();
            this.cmdOpenDwgTemplate = new System.Windows.Forms.Button();
            this.txtExportPath = new System.Windows.Forms.TextBox();
            this.txtPointCloudFile = new System.Windows.Forms.TextBox();
            this.txtDrawingTemplate = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtWaterDropLayer = new System.Windows.Forms.TextBox();
            this.txtWaterDropCode = new System.Windows.Forms.TextBox();
            this.txtWaterLayer = new System.Windows.Forms.TextBox();
            this.txtWaterCode = new System.Windows.Forms.TextBox();
            this.txtWallLayer = new System.Windows.Forms.TextBox();
            this.txtWallCode = new System.Windows.Forms.TextBox();
            this.txtWalkStripLayer = new System.Windows.Forms.TextBox();
            this.txtWalkStripCode = new System.Windows.Forms.TextBox();
            this.txtTreeOutlineLayer = new System.Windows.Forms.TextBox();
            this.txtTreeOutlineCode = new System.Windows.Forms.TextBox();
            this.txtTeeBoxLayer = new System.Windows.Forms.TextBox();
            this.txtTeeBoxCode = new System.Windows.Forms.TextBox();
            this.txtStepsLayer = new System.Windows.Forms.TextBox();
            this.txtStepsCode = new System.Windows.Forms.TextBox();
            this.txtRoughOutlineLayer = new System.Windows.Forms.TextBox();
            this.txtRoughOutlineCode = new System.Windows.Forms.TextBox();
            this.txtRockOutlineLayer = new System.Windows.Forms.TextBox();
            this.txtRockOutlineCode = new System.Windows.Forms.TextBox();
            this.txtPathLayer = new System.Windows.Forms.TextBox();
            this.txtPathCode = new System.Windows.Forms.TextBox();
            this.txtOtherLayer = new System.Windows.Forms.TextBox();
            this.txtOtherCode = new System.Windows.Forms.TextBox();
            this.txtNativeAreaLayer = new System.Windows.Forms.TextBox();
            this.txtNativeAreaCode = new System.Windows.Forms.TextBox();
            this.txtLandscapingLayer = new System.Windows.Forms.TextBox();
            this.txtLandscapingCode = new System.Windows.Forms.TextBox();
            this.txtIntMedRoughLayer = new System.Windows.Forms.TextBox();
            this.txtIntMedRoughCode = new System.Windows.Forms.TextBox();
            this.txtGreenSideBunkerLayer = new System.Windows.Forms.TextBox();
            this.txtGreenSideBunkerCode = new System.Windows.Forms.TextBox();
            this.txtGreenLayer = new System.Windows.Forms.TextBox();
            this.txtGreenCode = new System.Windows.Forms.TextBox();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdSave = new System.Windows.Forms.Button();
            this.txtFairwayLayer = new System.Windows.Forms.TextBox();
            this.txtFairwayCode = new System.Windows.Forms.TextBox();
            this.txtDirtOutlineLayer = new System.Windows.Forms.TextBox();
            this.txtDirtOutlineCode = new System.Windows.Forms.TextBox();
            this.txtCollarLayer = new System.Windows.Forms.TextBox();
            this.txtCollarCode = new System.Windows.Forms.TextBox();
            this.txtCartPathLayer = new System.Windows.Forms.TextBox();
            this.txtCartPathCode = new System.Windows.Forms.TextBox();
            this.txtBushOutlineLayer = new System.Windows.Forms.TextBox();
            this.txtBushOutlineCode = new System.Windows.Forms.TextBox();
            this.txtBunkerLayer = new System.Windows.Forms.TextBox();
            this.txtBunkerCode = new System.Windows.Forms.TextBox();
            this.txtBuildingLayer = new System.Windows.Forms.TextBox();
            this.txtBuildingCode = new System.Windows.Forms.TextBox();
            this.txtBridgeLayer = new System.Windows.Forms.TextBox();
            this.txtBridgeCode = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmdOpenExportPath);
            this.groupBox1.Controls.Add(this.cmdOpenPointFile);
            this.groupBox1.Controls.Add(this.cmdOpenDwgTemplate);
            this.groupBox1.Controls.Add(this.txtExportPath);
            this.groupBox1.Controls.Add(this.txtPointCloudFile);
            this.groupBox1.Controls.Add(this.txtDrawingTemplate);
            this.groupBox1.Location = new System.Drawing.Point(71, 36);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(814, 169);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File Settings";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "Export Path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Point Cloud";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Template";
            // 
            // cmdOpenExportPath
            // 
            this.cmdOpenExportPath.Location = new System.Drawing.Point(669, 113);
            this.cmdOpenExportPath.Name = "cmdOpenExportPath";
            this.cmdOpenExportPath.Size = new System.Drawing.Size(40, 23);
            this.cmdOpenExportPath.TabIndex = 5;
            this.cmdOpenExportPath.Text = "...";
            this.cmdOpenExportPath.UseVisualStyleBackColor = true;
            this.cmdOpenExportPath.Click += new System.EventHandler(this.cmdOpenExportPath_Click);
            // 
            // cmdOpenPointFile
            // 
            this.cmdOpenPointFile.Location = new System.Drawing.Point(669, 73);
            this.cmdOpenPointFile.Name = "cmdOpenPointFile";
            this.cmdOpenPointFile.Size = new System.Drawing.Size(40, 23);
            this.cmdOpenPointFile.TabIndex = 4;
            this.cmdOpenPointFile.Text = "...";
            this.cmdOpenPointFile.UseVisualStyleBackColor = true;
            this.cmdOpenPointFile.Click += new System.EventHandler(this.cmdOpenPointFile_Click);
            // 
            // cmdOpenDwgTemplate
            // 
            this.cmdOpenDwgTemplate.Location = new System.Drawing.Point(669, 32);
            this.cmdOpenDwgTemplate.Name = "cmdOpenDwgTemplate";
            this.cmdOpenDwgTemplate.Size = new System.Drawing.Size(40, 23);
            this.cmdOpenDwgTemplate.TabIndex = 3;
            this.cmdOpenDwgTemplate.Text = "...";
            this.cmdOpenDwgTemplate.UseVisualStyleBackColor = true;
            this.cmdOpenDwgTemplate.Click += new System.EventHandler(this.cmdOpenDwgTemplate_Click);
            // 
            // txtExportPath
            // 
            this.txtExportPath.Location = new System.Drawing.Point(117, 113);
            this.txtExportPath.Name = "txtExportPath";
            this.txtExportPath.Size = new System.Drawing.Size(536, 22);
            this.txtExportPath.TabIndex = 2;
            // 
            // txtPointCloudFile
            // 
            this.txtPointCloudFile.Location = new System.Drawing.Point(117, 73);
            this.txtPointCloudFile.Name = "txtPointCloudFile";
            this.txtPointCloudFile.Size = new System.Drawing.Size(536, 22);
            this.txtPointCloudFile.TabIndex = 1;
            // 
            // txtDrawingTemplate
            // 
            this.txtDrawingTemplate.Location = new System.Drawing.Point(117, 33);
            this.txtDrawingTemplate.Name = "txtDrawingTemplate";
            this.txtDrawingTemplate.Size = new System.Drawing.Size(536, 22);
            this.txtDrawingTemplate.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtWaterDropLayer);
            this.groupBox2.Controls.Add(this.txtWaterDropCode);
            this.groupBox2.Controls.Add(this.txtWaterLayer);
            this.groupBox2.Controls.Add(this.txtWaterCode);
            this.groupBox2.Controls.Add(this.txtWallLayer);
            this.groupBox2.Controls.Add(this.txtWallCode);
            this.groupBox2.Controls.Add(this.txtWalkStripLayer);
            this.groupBox2.Controls.Add(this.txtWalkStripCode);
            this.groupBox2.Controls.Add(this.txtTreeOutlineLayer);
            this.groupBox2.Controls.Add(this.txtTreeOutlineCode);
            this.groupBox2.Controls.Add(this.txtTeeBoxLayer);
            this.groupBox2.Controls.Add(this.txtTeeBoxCode);
            this.groupBox2.Controls.Add(this.txtStepsLayer);
            this.groupBox2.Controls.Add(this.txtStepsCode);
            this.groupBox2.Controls.Add(this.txtRoughOutlineLayer);
            this.groupBox2.Controls.Add(this.txtRoughOutlineCode);
            this.groupBox2.Controls.Add(this.txtRockOutlineLayer);
            this.groupBox2.Controls.Add(this.txtRockOutlineCode);
            this.groupBox2.Controls.Add(this.txtPathLayer);
            this.groupBox2.Controls.Add(this.txtPathCode);
            this.groupBox2.Controls.Add(this.txtOtherLayer);
            this.groupBox2.Controls.Add(this.txtOtherCode);
            this.groupBox2.Controls.Add(this.txtNativeAreaLayer);
            this.groupBox2.Controls.Add(this.txtNativeAreaCode);
            this.groupBox2.Controls.Add(this.txtLandscapingLayer);
            this.groupBox2.Controls.Add(this.txtLandscapingCode);
            this.groupBox2.Controls.Add(this.txtIntMedRoughLayer);
            this.groupBox2.Controls.Add(this.txtIntMedRoughCode);
            this.groupBox2.Controls.Add(this.txtGreenSideBunkerLayer);
            this.groupBox2.Controls.Add(this.txtGreenSideBunkerCode);
            this.groupBox2.Controls.Add(this.txtGreenLayer);
            this.groupBox2.Controls.Add(this.txtGreenCode);
            this.groupBox2.Controls.Add(this.txtFairwayLayer);
            this.groupBox2.Controls.Add(this.txtFairwayCode);
            this.groupBox2.Controls.Add(this.txtDirtOutlineLayer);
            this.groupBox2.Controls.Add(this.txtDirtOutlineCode);
            this.groupBox2.Controls.Add(this.txtCollarLayer);
            this.groupBox2.Controls.Add(this.txtCollarCode);
            this.groupBox2.Controls.Add(this.txtCartPathLayer);
            this.groupBox2.Controls.Add(this.txtCartPathCode);
            this.groupBox2.Controls.Add(this.txtBushOutlineLayer);
            this.groupBox2.Controls.Add(this.txtBushOutlineCode);
            this.groupBox2.Controls.Add(this.txtBunkerLayer);
            this.groupBox2.Controls.Add(this.txtBunkerCode);
            this.groupBox2.Controls.Add(this.txtBuildingLayer);
            this.groupBox2.Controls.Add(this.txtBuildingCode);
            this.groupBox2.Controls.Add(this.txtBridgeLayer);
            this.groupBox2.Controls.Add(this.txtBridgeCode);
            this.groupBox2.Location = new System.Drawing.Point(35, 223);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(887, 273);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Layer Settings";
            // 
            // txtWaterDropLayer
            // 
            this.txtWaterDropLayer.Location = new System.Drawing.Point(704, 232);
            this.txtWaterDropLayer.Name = "txtWaterDropLayer";
            this.txtWaterDropLayer.Size = new System.Drawing.Size(100, 22);
            this.txtWaterDropLayer.TabIndex = 89;
            // 
            // txtWaterDropCode
            // 
            this.txtWaterDropCode.Location = new System.Drawing.Point(638, 232);
            this.txtWaterDropCode.Name = "txtWaterDropCode";
            this.txtWaterDropCode.Size = new System.Drawing.Size(49, 22);
            this.txtWaterDropCode.TabIndex = 88;
            // 
            // txtWaterLayer
            // 
            this.txtWaterLayer.Location = new System.Drawing.Point(704, 204);
            this.txtWaterLayer.Name = "txtWaterLayer";
            this.txtWaterLayer.Size = new System.Drawing.Size(100, 22);
            this.txtWaterLayer.TabIndex = 87;
            // 
            // txtWaterCode
            // 
            this.txtWaterCode.Location = new System.Drawing.Point(638, 204);
            this.txtWaterCode.Name = "txtWaterCode";
            this.txtWaterCode.Size = new System.Drawing.Size(49, 22);
            this.txtWaterCode.TabIndex = 86;
            // 
            // txtWallLayer
            // 
            this.txtWallLayer.Location = new System.Drawing.Point(704, 176);
            this.txtWallLayer.Name = "txtWallLayer";
            this.txtWallLayer.Size = new System.Drawing.Size(100, 22);
            this.txtWallLayer.TabIndex = 85;
            // 
            // txtWallCode
            // 
            this.txtWallCode.Location = new System.Drawing.Point(638, 176);
            this.txtWallCode.Name = "txtWallCode";
            this.txtWallCode.Size = new System.Drawing.Size(49, 22);
            this.txtWallCode.TabIndex = 84;
            // 
            // txtWalkStripLayer
            // 
            this.txtWalkStripLayer.Location = new System.Drawing.Point(704, 148);
            this.txtWalkStripLayer.Name = "txtWalkStripLayer";
            this.txtWalkStripLayer.Size = new System.Drawing.Size(100, 22);
            this.txtWalkStripLayer.TabIndex = 83;
            // 
            // txtWalkStripCode
            // 
            this.txtWalkStripCode.Location = new System.Drawing.Point(638, 148);
            this.txtWalkStripCode.Name = "txtWalkStripCode";
            this.txtWalkStripCode.Size = new System.Drawing.Size(49, 22);
            this.txtWalkStripCode.TabIndex = 82;
            // 
            // txtTreeOutlineLayer
            // 
            this.txtTreeOutlineLayer.Location = new System.Drawing.Point(704, 120);
            this.txtTreeOutlineLayer.Name = "txtTreeOutlineLayer";
            this.txtTreeOutlineLayer.Size = new System.Drawing.Size(100, 22);
            this.txtTreeOutlineLayer.TabIndex = 81;
            // 
            // txtTreeOutlineCode
            // 
            this.txtTreeOutlineCode.Location = new System.Drawing.Point(638, 120);
            this.txtTreeOutlineCode.Name = "txtTreeOutlineCode";
            this.txtTreeOutlineCode.Size = new System.Drawing.Size(49, 22);
            this.txtTreeOutlineCode.TabIndex = 80;
            // 
            // txtTeeBoxLayer
            // 
            this.txtTeeBoxLayer.Location = new System.Drawing.Point(704, 92);
            this.txtTeeBoxLayer.Name = "txtTeeBoxLayer";
            this.txtTeeBoxLayer.Size = new System.Drawing.Size(100, 22);
            this.txtTeeBoxLayer.TabIndex = 79;
            // 
            // txtTeeBoxCode
            // 
            this.txtTeeBoxCode.Location = new System.Drawing.Point(638, 92);
            this.txtTeeBoxCode.Name = "txtTeeBoxCode";
            this.txtTeeBoxCode.Size = new System.Drawing.Size(49, 22);
            this.txtTeeBoxCode.TabIndex = 78;
            // 
            // txtStepsLayer
            // 
            this.txtStepsLayer.Location = new System.Drawing.Point(704, 64);
            this.txtStepsLayer.Name = "txtStepsLayer";
            this.txtStepsLayer.Size = new System.Drawing.Size(100, 22);
            this.txtStepsLayer.TabIndex = 77;
            // 
            // txtStepsCode
            // 
            this.txtStepsCode.Location = new System.Drawing.Point(638, 64);
            this.txtStepsCode.Name = "txtStepsCode";
            this.txtStepsCode.Size = new System.Drawing.Size(49, 22);
            this.txtStepsCode.TabIndex = 76;
            // 
            // txtRoughOutlineLayer
            // 
            this.txtRoughOutlineLayer.Location = new System.Drawing.Point(704, 36);
            this.txtRoughOutlineLayer.Name = "txtRoughOutlineLayer";
            this.txtRoughOutlineLayer.Size = new System.Drawing.Size(100, 22);
            this.txtRoughOutlineLayer.TabIndex = 75;
            // 
            // txtRoughOutlineCode
            // 
            this.txtRoughOutlineCode.Location = new System.Drawing.Point(638, 36);
            this.txtRoughOutlineCode.Name = "txtRoughOutlineCode";
            this.txtRoughOutlineCode.Size = new System.Drawing.Size(49, 22);
            this.txtRoughOutlineCode.TabIndex = 74;
            // 
            // txtRockOutlineLayer
            // 
            this.txtRockOutlineLayer.Location = new System.Drawing.Point(430, 232);
            this.txtRockOutlineLayer.Name = "txtRockOutlineLayer";
            this.txtRockOutlineLayer.Size = new System.Drawing.Size(100, 22);
            this.txtRockOutlineLayer.TabIndex = 73;
            // 
            // txtRockOutlineCode
            // 
            this.txtRockOutlineCode.Location = new System.Drawing.Point(364, 232);
            this.txtRockOutlineCode.Name = "txtRockOutlineCode";
            this.txtRockOutlineCode.Size = new System.Drawing.Size(49, 22);
            this.txtRockOutlineCode.TabIndex = 72;
            // 
            // txtPathLayer
            // 
            this.txtPathLayer.Location = new System.Drawing.Point(430, 204);
            this.txtPathLayer.Name = "txtPathLayer";
            this.txtPathLayer.Size = new System.Drawing.Size(100, 22);
            this.txtPathLayer.TabIndex = 71;
            // 
            // txtPathCode
            // 
            this.txtPathCode.Location = new System.Drawing.Point(364, 204);
            this.txtPathCode.Name = "txtPathCode";
            this.txtPathCode.Size = new System.Drawing.Size(49, 22);
            this.txtPathCode.TabIndex = 70;
            // 
            // txtOtherLayer
            // 
            this.txtOtherLayer.Location = new System.Drawing.Point(430, 176);
            this.txtOtherLayer.Name = "txtOtherLayer";
            this.txtOtherLayer.Size = new System.Drawing.Size(100, 22);
            this.txtOtherLayer.TabIndex = 69;
            // 
            // txtOtherCode
            // 
            this.txtOtherCode.Location = new System.Drawing.Point(364, 176);
            this.txtOtherCode.Name = "txtOtherCode";
            this.txtOtherCode.Size = new System.Drawing.Size(49, 22);
            this.txtOtherCode.TabIndex = 68;
            // 
            // txtNativeAreaLayer
            // 
            this.txtNativeAreaLayer.Location = new System.Drawing.Point(430, 148);
            this.txtNativeAreaLayer.Name = "txtNativeAreaLayer";
            this.txtNativeAreaLayer.Size = new System.Drawing.Size(100, 22);
            this.txtNativeAreaLayer.TabIndex = 67;
            // 
            // txtNativeAreaCode
            // 
            this.txtNativeAreaCode.Location = new System.Drawing.Point(364, 148);
            this.txtNativeAreaCode.Name = "txtNativeAreaCode";
            this.txtNativeAreaCode.Size = new System.Drawing.Size(49, 22);
            this.txtNativeAreaCode.TabIndex = 66;
            // 
            // txtLandscapingLayer
            // 
            this.txtLandscapingLayer.Location = new System.Drawing.Point(430, 120);
            this.txtLandscapingLayer.Name = "txtLandscapingLayer";
            this.txtLandscapingLayer.Size = new System.Drawing.Size(100, 22);
            this.txtLandscapingLayer.TabIndex = 65;
            // 
            // txtLandscapingCode
            // 
            this.txtLandscapingCode.Location = new System.Drawing.Point(364, 120);
            this.txtLandscapingCode.Name = "txtLandscapingCode";
            this.txtLandscapingCode.Size = new System.Drawing.Size(49, 22);
            this.txtLandscapingCode.TabIndex = 64;
            // 
            // txtIntMedRoughLayer
            // 
            this.txtIntMedRoughLayer.Location = new System.Drawing.Point(430, 92);
            this.txtIntMedRoughLayer.Name = "txtIntMedRoughLayer";
            this.txtIntMedRoughLayer.Size = new System.Drawing.Size(100, 22);
            this.txtIntMedRoughLayer.TabIndex = 63;
            // 
            // txtIntMedRoughCode
            // 
            this.txtIntMedRoughCode.Location = new System.Drawing.Point(364, 92);
            this.txtIntMedRoughCode.Name = "txtIntMedRoughCode";
            this.txtIntMedRoughCode.Size = new System.Drawing.Size(49, 22);
            this.txtIntMedRoughCode.TabIndex = 62;
            // 
            // txtGreenSideBunkerLayer
            // 
            this.txtGreenSideBunkerLayer.Location = new System.Drawing.Point(430, 64);
            this.txtGreenSideBunkerLayer.Name = "txtGreenSideBunkerLayer";
            this.txtGreenSideBunkerLayer.Size = new System.Drawing.Size(100, 22);
            this.txtGreenSideBunkerLayer.TabIndex = 61;
            // 
            // txtGreenSideBunkerCode
            // 
            this.txtGreenSideBunkerCode.Location = new System.Drawing.Point(364, 64);
            this.txtGreenSideBunkerCode.Name = "txtGreenSideBunkerCode";
            this.txtGreenSideBunkerCode.Size = new System.Drawing.Size(49, 22);
            this.txtGreenSideBunkerCode.TabIndex = 60;
            // 
            // txtGreenLayer
            // 
            this.txtGreenLayer.Location = new System.Drawing.Point(430, 36);
            this.txtGreenLayer.Name = "txtGreenLayer";
            this.txtGreenLayer.Size = new System.Drawing.Size(100, 22);
            this.txtGreenLayer.TabIndex = 59;
            // 
            // txtGreenCode
            // 
            this.txtGreenCode.Location = new System.Drawing.Point(364, 36);
            this.txtGreenCode.Name = "txtGreenCode";
            this.txtGreenCode.Size = new System.Drawing.Size(49, 22);
            this.txtGreenCode.TabIndex = 58;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(487, 510);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 57;
            this.cmdCancel.Text = "Close";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdSave
            // 
            this.cmdSave.Location = new System.Drawing.Point(394, 510);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(75, 23);
            this.cmdSave.TabIndex = 56;
            this.cmdSave.Text = "Save";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // txtFairwayLayer
            // 
            this.txtFairwayLayer.Location = new System.Drawing.Point(148, 232);
            this.txtFairwayLayer.Name = "txtFairwayLayer";
            this.txtFairwayLayer.Size = new System.Drawing.Size(100, 22);
            this.txtFairwayLayer.TabIndex = 23;
            // 
            // txtFairwayCode
            // 
            this.txtFairwayCode.Location = new System.Drawing.Point(82, 232);
            this.txtFairwayCode.Name = "txtFairwayCode";
            this.txtFairwayCode.Size = new System.Drawing.Size(49, 22);
            this.txtFairwayCode.TabIndex = 22;
            // 
            // txtDirtOutlineLayer
            // 
            this.txtDirtOutlineLayer.Location = new System.Drawing.Point(148, 204);
            this.txtDirtOutlineLayer.Name = "txtDirtOutlineLayer";
            this.txtDirtOutlineLayer.Size = new System.Drawing.Size(100, 22);
            this.txtDirtOutlineLayer.TabIndex = 21;
            // 
            // txtDirtOutlineCode
            // 
            this.txtDirtOutlineCode.Location = new System.Drawing.Point(82, 204);
            this.txtDirtOutlineCode.Name = "txtDirtOutlineCode";
            this.txtDirtOutlineCode.Size = new System.Drawing.Size(49, 22);
            this.txtDirtOutlineCode.TabIndex = 20;
            // 
            // txtCollarLayer
            // 
            this.txtCollarLayer.Location = new System.Drawing.Point(148, 176);
            this.txtCollarLayer.Name = "txtCollarLayer";
            this.txtCollarLayer.Size = new System.Drawing.Size(100, 22);
            this.txtCollarLayer.TabIndex = 19;
            // 
            // txtCollarCode
            // 
            this.txtCollarCode.Location = new System.Drawing.Point(82, 176);
            this.txtCollarCode.Name = "txtCollarCode";
            this.txtCollarCode.Size = new System.Drawing.Size(49, 22);
            this.txtCollarCode.TabIndex = 18;
            // 
            // txtCartPathLayer
            // 
            this.txtCartPathLayer.Location = new System.Drawing.Point(148, 148);
            this.txtCartPathLayer.Name = "txtCartPathLayer";
            this.txtCartPathLayer.Size = new System.Drawing.Size(100, 22);
            this.txtCartPathLayer.TabIndex = 17;
            // 
            // txtCartPathCode
            // 
            this.txtCartPathCode.Location = new System.Drawing.Point(82, 148);
            this.txtCartPathCode.Name = "txtCartPathCode";
            this.txtCartPathCode.Size = new System.Drawing.Size(49, 22);
            this.txtCartPathCode.TabIndex = 16;
            // 
            // txtBushOutlineLayer
            // 
            this.txtBushOutlineLayer.Location = new System.Drawing.Point(148, 120);
            this.txtBushOutlineLayer.Name = "txtBushOutlineLayer";
            this.txtBushOutlineLayer.Size = new System.Drawing.Size(100, 22);
            this.txtBushOutlineLayer.TabIndex = 7;
            // 
            // txtBushOutlineCode
            // 
            this.txtBushOutlineCode.Location = new System.Drawing.Point(82, 120);
            this.txtBushOutlineCode.Name = "txtBushOutlineCode";
            this.txtBushOutlineCode.Size = new System.Drawing.Size(49, 22);
            this.txtBushOutlineCode.TabIndex = 6;
            // 
            // txtBunkerLayer
            // 
            this.txtBunkerLayer.Location = new System.Drawing.Point(148, 92);
            this.txtBunkerLayer.Name = "txtBunkerLayer";
            this.txtBunkerLayer.Size = new System.Drawing.Size(100, 22);
            this.txtBunkerLayer.TabIndex = 5;
            // 
            // txtBunkerCode
            // 
            this.txtBunkerCode.Location = new System.Drawing.Point(82, 92);
            this.txtBunkerCode.Name = "txtBunkerCode";
            this.txtBunkerCode.Size = new System.Drawing.Size(49, 22);
            this.txtBunkerCode.TabIndex = 4;
            // 
            // txtBuildingLayer
            // 
            this.txtBuildingLayer.Location = new System.Drawing.Point(148, 64);
            this.txtBuildingLayer.Name = "txtBuildingLayer";
            this.txtBuildingLayer.Size = new System.Drawing.Size(100, 22);
            this.txtBuildingLayer.TabIndex = 3;
            // 
            // txtBuildingCode
            // 
            this.txtBuildingCode.Location = new System.Drawing.Point(82, 64);
            this.txtBuildingCode.Name = "txtBuildingCode";
            this.txtBuildingCode.Size = new System.Drawing.Size(49, 22);
            this.txtBuildingCode.TabIndex = 2;
            // 
            // txtBridgeLayer
            // 
            this.txtBridgeLayer.Location = new System.Drawing.Point(148, 36);
            this.txtBridgeLayer.Name = "txtBridgeLayer";
            this.txtBridgeLayer.Size = new System.Drawing.Size(100, 22);
            this.txtBridgeLayer.TabIndex = 1;
            // 
            // txtBridgeCode
            // 
            this.txtBridgeCode.Location = new System.Drawing.Point(82, 36);
            this.txtBridgeCode.Name = "txtBridgeCode";
            this.txtBridgeCode.Size = new System.Drawing.Size(49, 22);
            this.txtBridgeCode.TabIndex = 0;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(956, 545);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdSave);
            this.Name = "Settings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cmdOpenExportPath;
        private System.Windows.Forms.Button cmdOpenPointFile;
        private System.Windows.Forms.Button cmdOpenDwgTemplate;
        private System.Windows.Forms.TextBox txtExportPath;
        private System.Windows.Forms.TextBox txtPointCloudFile;
        private System.Windows.Forms.TextBox txtDrawingTemplate;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtFairwayLayer;
        private System.Windows.Forms.TextBox txtFairwayCode;
        private System.Windows.Forms.TextBox txtDirtOutlineLayer;
        private System.Windows.Forms.TextBox txtDirtOutlineCode;
        private System.Windows.Forms.TextBox txtCollarLayer;
        private System.Windows.Forms.TextBox txtCollarCode;
        private System.Windows.Forms.TextBox txtCartPathLayer;
        private System.Windows.Forms.TextBox txtCartPathCode;
        private System.Windows.Forms.TextBox txtBushOutlineLayer;
        private System.Windows.Forms.TextBox txtBushOutlineCode;
        private System.Windows.Forms.TextBox txtBunkerLayer;
        private System.Windows.Forms.TextBox txtBunkerCode;
        private System.Windows.Forms.TextBox txtBuildingLayer;
        private System.Windows.Forms.TextBox txtBuildingCode;
        private System.Windows.Forms.TextBox txtBridgeLayer;
        private System.Windows.Forms.TextBox txtBridgeCode;
        private System.Windows.Forms.TextBox txtWaterDropLayer;
        private System.Windows.Forms.TextBox txtWaterDropCode;
        private System.Windows.Forms.TextBox txtWaterLayer;
        private System.Windows.Forms.TextBox txtWaterCode;
        private System.Windows.Forms.TextBox txtWallLayer;
        private System.Windows.Forms.TextBox txtWallCode;
        private System.Windows.Forms.TextBox txtWalkStripLayer;
        private System.Windows.Forms.TextBox txtWalkStripCode;
        private System.Windows.Forms.TextBox txtTreeOutlineLayer;
        private System.Windows.Forms.TextBox txtTreeOutlineCode;
        private System.Windows.Forms.TextBox txtTeeBoxLayer;
        private System.Windows.Forms.TextBox txtTeeBoxCode;
        private System.Windows.Forms.TextBox txtStepsLayer;
        private System.Windows.Forms.TextBox txtStepsCode;
        private System.Windows.Forms.TextBox txtRoughOutlineLayer;
        private System.Windows.Forms.TextBox txtRoughOutlineCode;
        private System.Windows.Forms.TextBox txtRockOutlineLayer;
        private System.Windows.Forms.TextBox txtRockOutlineCode;
        private System.Windows.Forms.TextBox txtPathLayer;
        private System.Windows.Forms.TextBox txtPathCode;
        private System.Windows.Forms.TextBox txtOtherLayer;
        private System.Windows.Forms.TextBox txtOtherCode;
        private System.Windows.Forms.TextBox txtNativeAreaLayer;
        private System.Windows.Forms.TextBox txtNativeAreaCode;
        private System.Windows.Forms.TextBox txtLandscapingLayer;
        private System.Windows.Forms.TextBox txtLandscapingCode;
        private System.Windows.Forms.TextBox txtIntMedRoughLayer;
        private System.Windows.Forms.TextBox txtIntMedRoughCode;
        private System.Windows.Forms.TextBox txtGreenSideBunkerLayer;
        private System.Windows.Forms.TextBox txtGreenSideBunkerCode;
        private System.Windows.Forms.TextBox txtGreenLayer;
        private System.Windows.Forms.TextBox txtGreenCode;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
   
    }
}