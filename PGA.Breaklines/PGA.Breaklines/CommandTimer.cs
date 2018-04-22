using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using global::Autodesk.AutoCAD.DatabaseServices;
using PGA.Database;

namespace PGA.Breaklines
{
    public partial class Commands
    {
        //private static int MaxDWGs  = 0;
        private static int TotalDWGs= 0;
        private static bool _bLinesComplete = false; 
        private static bool _flocked     = false;
        private static bool _Initialized = false;


        public Commands(DateTime value, string outdwg)
        {
            _time = value;
            _drawingName = outdwg;

            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    #region Initialize Fields

                    _ListofDrawings = new List<string>();
                    #endregion

                    #region Set Global Fields

                    _directory = Path.GetDirectoryName(_drawingName);

                    _ListofDrawings.Add(outdwg);

                    #endregion

                    #region Initialize Classes and Objects

                    breaklines = new Breaklines(_time, _ListofDrawings);
                    selectPolylines = new SelectPolylines();
                    usedFeatureLines = new ObjectIdCollection();
                    used3dCollection = new ObjectIdCollection();
                    usedDrawings = new List<string>();

                    #endregion

                }
            }
            catch (System.Exception)
            {
                PGA.MessengerManager.MessengerManager.AddLog
                    ("Initialize Open Drawing for Processing!");
            }
        }

        private void OnIdle(object sender, EventArgs e)
        {
            if (!IsInitialized())
                return;
            if (!IsLocked() && !IsBreaklineComplete())
                AddBreaklinesByCommandLineV2();
            if (IsBreaklineComplete())
                AddFeatureLineElevFromSurface();
        }


        public void EnableLock()
        {
            _flocked = true;
        }

        public void DisableLock()
        {
            _flocked = false;
        }

        public bool IsLocked()
        {
            if (_flocked)
                return true;
            return false;
        }

        public bool IsBreaklineComplete()
        {
            return _bLinesComplete;
        }

        public bool IsInitialized()
        {
            if (_Initialized)
                return true;
            return false;
        }

        public static void SetInitialized()
        {
            _Initialized = true;
        }
    }
}
