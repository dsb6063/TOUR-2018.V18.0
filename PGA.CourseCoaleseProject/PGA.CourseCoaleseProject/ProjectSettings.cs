using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using PGA.Database;
using PGA.DataContext;
using System.Windows.Forms;
using System.Windows.Controls;
using COMS = PGA.MessengerManager;
using System.Xml;
using System.Xaml;

namespace PGA.CourseCoaleseProject
{
    public class ProjectSettings
    {
        public  ProjectSettings(DateTime? date)
        {
            var set = InitializeSettings(date);
                      InitializeSettings(set);

        }

        private Settings InitializeSettings(DateTime? date)
        {
            using (DatabaseCommands commands  = new DatabaseCommands())
            {
                if (date != null) return commands.GetSettingsByDate(date.Value);
            }

            return null;
        }

        public void InitializeSettings(Settings set)
        {
            try
            {
                UseSimplify = GetValueFromCheckBox(set.SimplifyPlines);
                UseProbing = GetValueFromCheckBox(set.Probing);
                UseSportVision = GetValueFromCheckBox(set.SportVision);
                UseSmoothing = GetValueFromCheckBox(set.Smoothing);
                UsePDFReport = GetValueFromCheckBox(set.PDFReport);
                UseAddFLtoWaterSurf = GetValueFromCheckBox(set.AddFLWaterSurf);
                UseGenErrorReport = GetValueFromCheckBox(set.GenErrReport);
                UseOutput3DPolys = GetValueFromCheckBox(set.Out3DPolys);
                UseReports = GetValueFromCheckBox(set.GenReports);
                UseBreaklines = GetValueFromCheckBox(set.CreateBreaklines);
                UseRemoveDups = GetValueFromCheckBox(set.DuplicateSurfaces);
                UseSkipDXF = GetValueFromCheckBox(set.SkipSDxf);
                UseGen2013DXF = GetValueFromCheckBox(set.CreateDXF);
                SimplifyTolerance = (double) set.SimplifyValue.GetValueOrDefault();

            }
            catch (Exception ex)
            {
                DatabaseLogs.AddLogs(ex.Message,"InitializeSettings");
            }
        }
        public double SimplifyTolerance { get; set; }

        public bool UseGen2013DXF { get; set; }

        public bool UseSkipDXF { get; set; }

        public bool UseRemoveDups { get; set; }

        public bool UseBreaklines { get; set; }

        public bool UseReports { get; set; }

        public bool UseOutput3DPolys { get; set; }

        public bool UseGenErrorReport { get; set; }

        public bool UseAddFLtoWaterSurf { get; set; }

        public bool UsePDFReport { get; set; }

        public bool UseSmoothing { get; set; }

        public bool UseProbing { get; set; }

        public bool UseSimplify { get; set; }
        public bool UseSportVision { get;  private set; }
         
        public bool GetValueFromCheckBox(object sender)
        {
            try
            {
                if (sender == null)
                    return false;

                try
                {
                    var testchkBox = (bool)sender;
                }
                catch (System.InvalidCastException)
                {
                    return false;
                }

                var chkBox = (bool)sender;

                if (chkBox == false)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

            return false;
        }
    }
}
