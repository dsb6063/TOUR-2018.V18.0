using System;

using global::Autodesk.AutoCAD.ApplicationServices;
using global::Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace C3DSurfacesDemo
{
    public class EditorUtils 
    {
        public static void Regen()
        {
            _editor.Regen();
        }
        public static void Write(string msg)
        {
            _editor.WriteMessage(msg);
        }

        public static string PromptForString(string prompt)
        {
            string result = String.Empty;
            PromptStringOptions options = new PromptStringOptions(prompt);
            options.AllowSpaces = true;
            PromptResult promptResult = _editor.GetString(options);
            if (promptResult.Status == PromptStatus.OK)
            {
                result = promptResult.StringResult;
            }

            return result;
        }

        private static Editor _editor 
        {
            get {
                return Application.DocumentManager.MdiActiveDocument.Editor;
            }
        }


    }
}