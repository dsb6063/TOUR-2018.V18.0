using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Civil.ApplicationServices;
using Acad = Autodesk.AutoCAD.DatabaseServices;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
namespace PGA.WindingNumAlgorithm
{
    public static class Active
    {
        /// <summary>
        /// Returns the active Editor object.
        /// </summary>
        /// <value>The editor.</value>
        public static Editor Editor
        {
            get { return Document.Editor; }
        }

        /// <summary>
        /// Returns the active Document object.
        /// </summary>
        /// <value>The document.</value>
        public static Document Document
        {
            get { return Application.DocumentManager.MdiActiveDocument; }
        }

        /// <summary>
        /// Returns the active Database object.
        /// </summary>
        /// <value>The database.</value>
        public static Acad.Database Database
        {
            get { return Document.Database; }
        }

        /// <summary>
        /// Sends a string to the command line in the active Editor
        /// </summary>
        /// <param name="message">The message to send.</param>
        public static void WriteMessage(string message)
        {
            Editor.WriteMessage(message);
        }

        /// <summary>
        /// Sends a string to the command line in the active Editor using String.Format.
        /// </summary>
        /// <param name="message">The message containing format specifications.</param>
        /// <param name="parameter">The variables to substitute into the format string.</param>
        public static void WriteMessage(string message, params object[] parameter)
        {
            Editor.WriteMessage(message, parameter);
        }

        /// <summary>
        /// Gets the active civil document.
        /// </summary>
        /// <value>The active civil document.</value>
        public static CivilDocument ActiveCivilDocument
        {
            get { return CivilApplication.ActiveDocument; }
        }

        /// <summary>
        /// Gets the working database.
        /// </summary>
        /// <value>The working database.</value>
        public static Acad.Database WorkingDatabase
        {
            get { return HostApplicationServices.WorkingDatabase; }
        }

        public static Transaction StartTransaction()
        {
            return HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction();
        }

        public static Document MdiDocument
        {
            get { return Application.DocumentManager.MdiActiveDocument; }
        }
    }
}
