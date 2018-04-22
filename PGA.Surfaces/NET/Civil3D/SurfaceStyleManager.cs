


using System;
using Autodesk.AutoCAD.Colors;
using global::Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices.Styles;

namespace C3DSurfacesDemo
{
    public class SurfaceStyleManager 
    {
        public static bool Exists(string styleName)
        {
            return (GetStyleId(styleName) != ObjectId.Null);
            
        }

        public static ObjectId GetStyleId(string styleName)
        {
            using (Transaction tr = CivilApplicationManager.StartTransaction())
            {
                CivilDocument doc = CivilApplicationManager.ActiveCivilDocument;
                SurfaceStyleCollection styles = doc.Styles.SurfaceStyles;
                foreach (ObjectId styleId in styles)
                {
                    SurfaceStyle style = styleId.GetObject(OpenMode.ForRead) as SurfaceStyle;
                    if (styleName == style.Name)
                    {
                        tr.Commit();
                        return styleId;
                    }
                }
                tr.Abort();
            }

            return ObjectId.Null;
        }

        public static void CreateDefault(string styleName)
        {
            try
            {


                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {
                    CivilDocument doc = CivilApplication.ActiveDocument;
                    SurfaceStyleCollection styles = doc.Styles.SurfaceStyles;
                    ObjectId styleId = styles.Add(styleName==String.Empty?"Style-"+DateTime.Now.Millisecond:styleName);
                    SurfaceStyle style = styleId.GetObject(OpenMode.ForWrite) as SurfaceStyle;
                    DisplayStyle majorContours = style.GetDisplayStylePlan(SurfaceDisplayStyleType.MajorContour);
                    majorContours.Visible = true;
                    majorContours.Color = Color.FromRgb(255, 255, 0);
                    style.ContourStyle.BaseElevationInterval = 5.0;
                    style.ContourStyle.MajorContourInterval = 20.0;
                    DisplayStyle minorContours = style.GetDisplayStylePlan(SurfaceDisplayStyleType.MinorContour);
                    minorContours.Visible = true;
                    minorContours.Color = Color.FromRgb(0, 255, 0);
                    DisplayStyle triangles = style.GetDisplayStylePlan(SurfaceDisplayStyleType.Triangles);

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }
    }
}