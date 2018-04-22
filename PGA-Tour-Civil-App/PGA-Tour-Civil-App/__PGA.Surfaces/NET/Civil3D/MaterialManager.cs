using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGA.Model.BLL;
using System.Reflection;
using PGA.Civil.Logging;

namespace C3DSurfacesDemo
{
    public static class MaterialManager
    {

        /*Maps basic materials from Civil3D to Max libraries*/


        public static class LayerMaterialMappings
        {
            private static string _bridge = "S-BRIDGES:Birch - Solid Stained Dark No Gloss";
            public static string Bridge
            {
                get
                {
                    return _bridge;
                }
            }

            private static string _building = "S-BUILDING:Aluminum - Flat";
            public static string Building
            {
                get
                {
                    return _building;
                }
            }

            private static string _bunker = "S-BUNKER:Golden Sand";
            public static string Bunker
            {
                get
                {
                    return _bunker;
                }
            }

            private static string _bushOutline = "S-BUSH:Grass - Thick";
            public static string BushOutline
            {
                get
                {
                    return _bushOutline;
                }
            }

            private static string _cartPath = "S-CARTPATH:Rubble - River Rock";
            public static string CartPath
            {
                get
                {
                    return _cartPath;
                }
            }

            private static string _collar = "S-COLLAR:Grass - Thick";
            public static string Collar
            {
                get
                {
                    return _collar;
                }
            }

            private static string _dirtOutline = "S-DIRT-OUTLINE:Dirt - Dark Brown";
            public static string DirtOutline
            {
                get
                {
                    return _dirtOutline;
                }
            }

            private static string _fairway = "S-FAIRWAY:Grass - Bermuda";
            public static string Fairway
            {
                get
                {
                    return _fairway;
                }
            }

            private static string _green = "S-GREEN:Grass - Bermuda";
            public static string Green
            {
                get
                {
                    return _green;
                }
            }

            private static string _greenSideBunker = "S-GREENSIDE-BUNKER:Sand";
            public static string GreenSideBunker
            {
                get
                {
                    return _greenSideBunker;
                }
            }

            private static string _intMedRough = "S-INTERMEDIATE-ROUGH:Grass - Thick";
            public static string IntMedRough
            {
                get
                {
                    return _intMedRough;
                }
            }

            private static string _landScaping = "S-LANDSCAPING:Grass - Light Rye";
            public static string LandScaping
            {
                get
                {
                    return _landScaping;
                }
            }

            private static string _nativeArea = "S-NATIVE-AREA:Grass - Bluegrass";
            public static string NativeArea
            {
                get
                {
                    return _nativeArea;
                }
            }

            private static string _other = "S-OTHER:Gravel - Compact";
            public static string Other
            {
                get
                {
                    return _other;
                }
            }

            private static string _path = "S-PATH:Gravel - Crushed";
            public static string Path
            {
                get
                {
                    return _path;
                }
            }

            private static string _rockOutline = "S-ROCK-OUTLINE:Gravel - Compact";
            public static string RockOutline
            {
                get
                {
                    return _rockOutline;
                }
            }

            private static string _roughOutline = "S-ROUGH-OUTLINES:Grass - Thick";
            public static string RoughOutline
            {
                get
                {
                    return _roughOutline;
                }
            }

            private static string _steps = "S-STEPS:Gravel - Crushed";
            public static string Steps
            {
                get
                {
                    return _steps;
                }
            }

            private static string _teeBox = "S-TEE-BOX:Grass - St. Augustine";
            public static string TeeBox
            {
                get
                {
                    return _teeBox;
                }
            }

            private static string _treeOutline = "S-TREE-OUTLINE:Grass - Light Rye";
            public static string TreeOutline
            {
                get
                {
                    return _treeOutline;
                }
            }

            private  static string _walkStrip = "S-WALK-STRIP:Asphalt - Wet";
            public static string WalkStrip
            {
                get
                {
                    return _walkStrip;
                }
            }

            private static string _wall = "S-WALL:Jagged Rock Wall";
            public static string Wall
            {
                get
                {
                    return _wall;
                }
            }

            private static string _water = "S-WATER:Water";
            public static string Water
            {
                get
                {
                    return _water;
                }
            }

            private static string _waterDrop = "S-WATER-DROP:Swimming Pool";
            public static string WaterDrop
            {
                get
                {
                    return _waterDrop;
                }
            }

            private static uint _layerStateID;
            public static uint LayerStateID
            {
                get
                {
                    return _layerStateID;
                }
            }

        }

        public static string GetMaterialForSurfaceName(string surfaceName)
        {
            try
            {
                if (String.IsNullOrEmpty(surfaceName))
                    return null;

                string layerName = CleanSurfaceName(surfaceName);
                PropertyInfo[] materials  = typeof(LayerMaterialMappings).GetProperties();

                object val= new object();
                foreach (PropertyInfo material in materials)
                {
                    val = material.GetValue(val, null);
                    if (surfaceName == GetCode(val.ToString()))
                    {
                        return GetLayer(val.ToString());
                    }
                }
                    
            }
            catch (Exception ex)
            {
                PGA.Civil.Logging.ACADLogging.LogMyExceptions
                    ("GetMaterialForSurfaceName: " + surfaceName + " "  + ex.Message);
            }

            return null;
        }
        
        private static string CleanSurfaceName(string surfaceName)
        {
          return  surfaceName.Substring(2);
        }
        public static string GetCode(string code)
        {
            try
            {
                string res = code.Split(':')[0];
                return res ?? "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string GetLayer(string code)
        {
            try
            {
                string res = code.Split(':')[1];
                return res ?? "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
