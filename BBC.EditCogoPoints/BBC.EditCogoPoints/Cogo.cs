using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.AutoCAD.Interop;
using Autodesk.AECC.Interop.Survey;
using Autodesk.AECC.Interop.UiSurvey;


namespace PGA.EditCogoPoints
{
    public static class Cogo
    {

       public static Editor ed = Active.Active.Editor;


        public static void GetCogoPoints()
        {
            // Select the location for COGO Point

            PromptPointOptions ppo = new PromptPointOptions("\nSelect the location to Create a COGO Point :");

            PromptPointResult ppr = ed.GetPoint(ppo);

            if (ppr.Status != PromptStatus.OK)
                return;

            Point3d location = ppr.Value;

            //start a transaction

            using (Transaction trans = Active.Active.StartTransaction())
            {

                // All points in a document are held in a CogoPointCollection object

                // We can access CogoPointCollection through the CivilDocument.CogoPoints property

                CogoPointCollection cogoPoints = CivilApplication.ActiveDocument.CogoPoints;

                // Adds a new CogoPoint at the given location with the specified description information

                ObjectId pointId    = cogoPoints.Add(location, "Survey Point",true);

                CogoPoint cogoPoint = pointId.GetObject(OpenMode.ForWrite) as CogoPoint;

                // Set Some Properties

                if (cogoPoint != null)
                {
                    cogoPoint.PointName = "Survey_Base_Point";

                    cogoPoint.RawDescription = "This is Survey Base Point";
                }

                trans.Commit();
            }
        }

        public static void EditCogoPoints()
        {
            Editor ed = Active.Active.Editor;

            // Select the location for COGO Point

            PromptPointOptions ppo = new PromptPointOptions("\nSelect the location to Create a COGO Point :");

            PromptPointResult ppr = ed.GetPoint(ppo);

            if (ppr.Status != PromptStatus.OK)
                return;

            Point3d location = ppr.Value;

            //start a transaction

            using (Transaction trans = Active.Active.StartTransaction())
            {

                // All points in a document are held in a CogoPointCollection object

                // We can access CogoPointCollection through the CivilDocument.CogoPoints property

                CogoPointCollection cogoPoints = CivilApplication.ActiveDocument.CogoPoints;

                // Adds a new CogoPoint at the given location with the specified description information

                ObjectId pointId = cogoPoints.Add(location, "Survey Point", true);

                CogoPoint cogoPoint = pointId.GetObject(OpenMode.ForWrite) as CogoPoint;

                // Set Some Properties

                if (cogoPoint != null)
                {
                    cogoPoint.PointName = "Survey_Base_Point";

                    cogoPoint.RawDescription = "This is Survey Base Point";
                }

                trans.Commit();
            }
        }

        public static bool IsSurveyPoint(ObjectId oid)
        {
            //open the COGO Point
            using (var trans = Active.Active.StartTransaction())
            {
                CogoPoint cogoPoint = trans.GetObject(oid, OpenMode.ForRead) as CogoPoint;

                // Access COGO Point Properties

                // CogoPoint.IsSurveyPoint property indicates whether this Cogo Point is a Survey Point
                if (cogoPoint == null)
                    return false;

                if (cogoPoint.IsSurveyPoint)

                {
                    // DO your stuff
                    ed.WriteMessage("\nSelected COGO Point is a Survey Point");
                    return true;

                }

                //  CogoPoint.IsProjectPoint property indicates whether the CogoPoint is a project point

                //  i.e. if the COGO point is from Vault Project Points

                else if (cogoPoint.IsProjectPoint)

                {
                    // DO your stuff

                    ed.WriteMessage("\nSelected COGO Point is a Project Point");
                    return true;

                }

                // normal COGO point object created in the Civil 3D drawing file

                else

                {
                    // DO your stuff

                    ed.WriteMessage("\nSelected COGO Point is neither Survey nor Project Point");
                    return false;

                }
                trans.Commit();
            }
        }

        //public static void AccessSurveyDB()
        //{
        //    // Get the AutoCAD Editor


        //    // Get the Survey Application and AeccSurveyDatabase

        //    AeccSurveyApplication aeccSurveyApp = new AeccSurveyApplication();

        //    aeccSurveyApp.Init((AcadApplication)AcadApp.AcadApplication);

        //    AeccSurveyDatabase aeccSurveydb = (AeccSurveyDatabase)aeccSurveyApp.ActiveDocument.Database;

        //    AeccSurveyProjects surveyProjs = aeccSurveydb.Projects;



        //    // Accessing a Particular Survey Project

        //    // IAeccSurveyProjects:: FindItem

        //    // Gets the Survey Project with the given name.



        //    AeccSurveyProject surveyProj = surveyProjs.FindItem("Test_Survey_DB");

        //    if (surveyProj != null)

        //    {

        //        // now we can access the various Props of AeccSurveyProject object           

        //        ed.WriteMessage("\nSurvey Project Name :  " + surveyProj.Name.ToString());

        //        ed.WriteMessage("\nSurvey Project Path :  " + surveyProj.Path.ToString());

        //        ed.WriteMessage("\nNext available point number   : " + surveyProj.GetNextWritablePointNumber().ToString());

        //    }
        //}
    }


}
    
