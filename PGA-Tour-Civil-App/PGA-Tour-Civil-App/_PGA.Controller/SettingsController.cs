#region

using System;
using System.Windows.Forms;
using PGA.Model.BLL;

#endregion

namespace PGA.Controller
{
    public class SettingsController
    {
        private ISettingsController _view;

        public SettingsController(ISettingsController view, object o)
        {
            _view = view;
            view.SetController(controller: this);
        }

        public SettingsController(ISettingsController view)
        {
            _view = view;
            view.SetController(this);
        }

        //public void SetController(SettingsController controller)
        //{
        //    throw new NotImplementedException();
        //}

        //public void SetController(SettingsController controller, object data)
        //{
        //    throw new NotImplementedException();
        //}

        public void Save(LayerStatesBLL changed_states)
        {
            try
            {
                IModel model = new Model.BLL.Model();
                model.WriteDataToRegistry("Bridge", changed_states.Bridge);
                model.WriteDataToRegistry("Building", changed_states.Building);
                model.WriteDataToRegistry("Bunker", changed_states.Bunker);
                model.WriteDataToRegistry("BushOutline", changed_states.BushOutline);
                model.WriteDataToRegistry("CartPath", changed_states.CartPath);
                model.WriteDataToRegistry("Collar", changed_states.Collar);
                model.WriteDataToRegistry("DirtOutline", changed_states.DirtOutline);
                model.WriteDataToRegistry("Fairway", changed_states.Fairway);
                model.WriteDataToRegistry("Green", changed_states.Green);
                model.WriteDataToRegistry("GreenSideBunker", changed_states.GreenSideBunker);
                model.WriteDataToRegistry("IntMedRough", changed_states.IntMedRough);
                model.WriteDataToRegistry("LandScaping", changed_states.LandScaping);
                model.WriteDataToRegistry("NativeArea", changed_states.NativeArea);
                model.WriteDataToRegistry("Other", changed_states.Other);
                model.WriteDataToRegistry("Path", changed_states.Path);
                model.WriteDataToRegistry("RockOutline", changed_states.RockOutline);
                model.WriteDataToRegistry("RoughOutline", changed_states.RoughOutline);
                model.WriteDataToRegistry("Steps", changed_states.Steps);
                model.WriteDataToRegistry("TeeBox", changed_states.TeeBox);
                model.WriteDataToRegistry("TreeOutline", changed_states.TreeOutline);
                model.WriteDataToRegistry("WalkStrip", changed_states.WalkStrip);
                model.WriteDataToRegistry("Wall", changed_states.Wall);
                model.WriteDataToRegistry("Water", changed_states.Water);
                model.WriteDataToRegistry("WaterDrop", changed_states.WaterDrop);
            }


            catch (Exception ex)
            {
                MessengerManager.MessengerManager.AddLog(ex.Message);
            }
        }

        public void Cancel()
        {
        }

        public string OpenDwgTemplate()
        {
            try
            {
                IModel model = new Model.BLL.Model();
                var openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory =
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var result = openFileDialog.ShowDialog(); // Show the dialog.
                if (result == DialogResult.OK)
                {
                    model.WriteDataToRegistry("Template", openFileDialog.FileName);
                }

                return openFileDialog.FileName;
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.AddLog(ex.Message);
            }
            return null;
        }

        public string OpenPointFile()
        {
            try
            {
                IModel model = new Model.BLL.Model();
                var openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var result = openFileDialog.ShowDialog(); // Show the dialog.
                if (result == DialogResult.OK)
                {
                    model.WriteDataToRegistry("PointFilePath", openFileDialog.FileName);
                }
                return openFileDialog.FileName;
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.AddLog(ex.Message);
            }
            return null;
        }

        public string OpenExportPath()
        {
            try
            {
                IModel model = new Model.BLL.Model();
                var openFileDialog = new FolderBrowserDialog();
                openFileDialog.ShowNewFolderButton = true;
                openFileDialog.RootFolder = Environment.SpecialFolder.MyDocuments;
                var result = openFileDialog.ShowDialog(); // Show the dialog.
                if (result == DialogResult.OK)
                {
                    model.WriteDataToRegistry("ExportPath", openFileDialog.SelectedPath);
                }
                return openFileDialog.SelectedPath;
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.AddLog(ex.Message);
            }
            return null;
        }

        //}
        //    return null;
        //    //this._view.LoadDataSet(model.GetPathDataFromRegistry());
        //    _view.LoadDataSet(model.GetDataFromRegistry());
        //    IModel model = new Model.BLL.Model();
        //{

        //public Model.DAL.PGALayerStates LoadData()
    }
}