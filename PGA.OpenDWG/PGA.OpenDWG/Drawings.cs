using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Core = Autodesk.AutoCAD.ApplicationServices.Core;

namespace PGA.OpenDWG
{
    public interface IDrawingCommands
    {
        bool GetDrawingPath();
        bool OpenDWG(string path);
        bool OpenDWGWithTemplate(string dwg, string template);
        bool OpenDWGWithTemplate(string dwg, string template,string type);
    }

    public class DrawingCommands : IDrawingCommands
    {
        [CommandMethod("PTH")]
        public void DrawingPath()
        {

            Document doc = Core.Application.DocumentManager.MdiActiveDocument;

            HostApplicationServices hs =
                HostApplicationServices.Current;
            string path =
                hs.FindFile(
                    doc.Name,
                    doc.Database,
                    FindFileHint.Default
                    );
            doc.Editor.WriteMessage(
                "\nFile was found in: " + path
                );
        }

        public bool GetDrawingPath()
        {
            
            Document doc = Core.Application.DocumentManager.MdiActiveDocument;

            HostApplicationServices hs =
                HostApplicationServices.Current;
            string path =
                hs.FindFile(
                    doc.Name,
                    doc.Database,
                    FindFileHint.Default
                    );
            doc.Editor.WriteMessage(
                "\nFile was found in: " + path
                );

            return true;
        }

        public bool OpenDWG(string path)
        {
            return false;
        }

        public bool OpenDWGWithTemplate(string dwg, string template)
        {
            throw new System.NotImplementedException();
        }

        public bool OpenDWGWithTemplate(string dwg, string template, string type)
        {
            throw new System.NotImplementedException();
        }
    }
}


