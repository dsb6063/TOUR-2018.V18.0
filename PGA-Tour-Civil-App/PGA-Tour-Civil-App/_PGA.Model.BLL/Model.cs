#region

using System;
using PGA.Autodesk.Settings;
using PGA.Common.Registry;

#endregion

namespace PGA.Model.BLL

{
    public class Model : IModel
    {
        public void WriteDataToRegistry(string Key, string Val)
        {
            if (Key == null) throw new ArgumentNullException("Key");
            try
            {
                RegistryFunctions.GlobalWriteToRegistry(Key, Val,
                    AcadSettings.RegistryPath);
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.AddLog(ex.Message);
            }
        }

        public LayerStatesBLL GetDataFromRegistry()
        {
            try
            {
                var states = new LayerStatesBLL();

                CleanDataFromRegistry("Bridge", AcadSettings.RegistryPath, states.Bridge);
                CleanDataFromRegistry("Building", AcadSettings.RegistryPath, states.Building);
                CleanDataFromRegistry("Bunker", AcadSettings.RegistryPath, states.Bunker);
                CleanDataFromRegistry("BushOutline", AcadSettings.RegistryPath, states.BushOutline);
                CleanDataFromRegistry("CartPath", AcadSettings.RegistryPath, states.CartPath);
                CleanDataFromRegistry("Collar", AcadSettings.RegistryPath, states.Collar);
                CleanDataFromRegistry("DirtOutline", AcadSettings.RegistryPath, states.DirtOutline);
                CleanDataFromRegistry("Fairway", AcadSettings.RegistryPath, states.Fairway);
                CleanDataFromRegistry("Green", AcadSettings.RegistryPath, states.Green);
                CleanDataFromRegistry("GreenSideBunker", AcadSettings.RegistryPath, states.GreenSideBunker);
                CleanDataFromRegistry("IntMedRough", AcadSettings.RegistryPath, states.IntMedRough);
                CleanDataFromRegistry("LandScaping", AcadSettings.RegistryPath, states.LandScaping);
                CleanDataFromRegistry("NativeArea", AcadSettings.RegistryPath, states.NativeArea);
                CleanDataFromRegistry("Other", AcadSettings.RegistryPath, states.Other);
                CleanDataFromRegistry("Path", AcadSettings.RegistryPath, states.Path);
                CleanDataFromRegistry("RockOutline", AcadSettings.RegistryPath, states.RockOutline);
                CleanDataFromRegistry("RoughOutline", AcadSettings.RegistryPath, states.RoughOutline);
                CleanDataFromRegistry("Steps", AcadSettings.RegistryPath, states.Steps);
                CleanDataFromRegistry("TeeBox", AcadSettings.RegistryPath, states.TeeBox);
                CleanDataFromRegistry("TreeOutline", AcadSettings.RegistryPath, states.TreeOutline);
                CleanDataFromRegistry("WalkStrip", AcadSettings.RegistryPath, states.WalkStrip);
                CleanDataFromRegistry("Wall", AcadSettings.RegistryPath, states.Wall);
                CleanDataFromRegistry("Water", AcadSettings.RegistryPath, states.Water);
                CleanDataFromRegistry("WaterDrop", AcadSettings.RegistryPath, states.WaterDrop);

                //string value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("Bridge",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if(!String.IsNullOrEmpty(value)) {states.Bridge = value;}

                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("Building",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.Building = value; }

                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("Bunker",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.Bunker = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("BushOutline",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.BushOutline = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("CartPath",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.CartPath = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("Collar",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.Collar = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("DirtOutline",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.DirtOutline = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("Fairway",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.Fairway = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("Green",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.Green = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("GreenSideBunker",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.GreenSideBunker = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("IntMedRough",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.IntMedRough = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("LandScaping",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.LandScaping = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("NativeArea",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.NativeArea = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("Other",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.Other = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("Path",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.Path = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("RockOutline",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.RockOutline = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("RoughOutline",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.RoughOutline = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("Steps",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.Steps = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("TeeBox",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.TeeBox = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("TreeOutline",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.TreeOutline = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("WalkStrip",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.WalkStrip = value; }
                //value = "";WaterDrop
                //value = RegistryFunctions.GlobalReadFromRegistry("Wall",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.Wall = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("Water",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.Water = value; }
                //value = "";
                //value = RegistryFunctions.GlobalReadFromRegistry("",
                //    PGA.Autodesk.Settings.AcadSettings.RegistryPath);
                //if (!String.IsNullOrEmpty(value)) { states.WaterDrop = value; }


                return states; //only loop once
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.AddLog(ex.Message);
            }
            return null;
        }

        public void GetDataFromRegistry(string Key)
        {
            if (Key == null) throw new ArgumentNullException("Key");
            try
            {
                RegistryFunctions.GlobalReadFromRegistry(Key,
                    AcadSettings.RegistryPath);
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.AddLog(ex.Message);
            }
        }

        public LayerStatesBLL GetPathDataFromRegistry()
        {
            // TODO: Implement this method
            //throw new NotImplementedException();
            return null;
        }

        public string CleanDataFromRegistry(string name, string Path, string states)
        {
            var value = "";
            value = RegistryFunctions.GlobalReadFromRegistry(name, Path);
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }
            return states;
        }

        public void AssignStates()
        {
        }
    }
}