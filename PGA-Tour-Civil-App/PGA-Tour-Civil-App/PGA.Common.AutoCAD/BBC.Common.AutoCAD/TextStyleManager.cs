#region

using System;
using System.Collections.Generic;
using global::Autodesk.AutoCAD.DatabaseServices;

#endregion

//using Common.Logging;

namespace BBC.Common.AutoCAD
{
    public class TextStyleManager
    {
        /// <summary>
        ///     Gets a list of text style names.
        /// </summary>
        /// <param name="db">The database to read.</param>
        /// <returns></returns>
        public static IList<string> GetTextStyleNames(Database db)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetTextStyleNames");
            IList<string> textStyleNames = new List<string>();
            using (var trans = db.TransactionManager.StartTransaction())
            {
                textStyleNames = GetTextStyleNames(db, trans);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End GetTextStyleNames");
            return textStyleNames;
        }

        /// <summary>
        ///     Gets a list of text style names.
        /// </summary>
        /// <param name="db">The database to read.</param>
        /// <param name="trans">The transaction to use.</param>
        /// <returns>A list of text style names</returns>
        public static IList<string> GetTextStyleNames(Database db, Transaction trans)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetTextStyleNames");
            IList<string> textStyleNames = new List<string>();
            using (var textStyleTable = (TextStyleTable) trans.GetObject(db.TextStyleTableId, OpenMode.ForWrite, false))
            {
                foreach (var id in textStyleTable)
                {
                    using (var record = trans.GetObject(id, OpenMode.ForRead) as TextStyleTableRecord)
                    {
                        textStyleNames.Add(record.Name);
                    }
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End GetTextStyleNames");
            return textStyleNames;
        }

        /// <summary>
        ///     Gets a list of text style names.
        /// </summary>
        /// <param name="db">The database to read.</param>
        /// <param name="trans">The transaction to use.</param>
        /// <returns>A list of text style names</returns>
        public static IList<string> GetTextStyleNames(Database db, Transaction trans, string filter)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetTextStyleNames");
            if (trans == null)
                throw new ArgumentNullException("trans", "The argument 'trans' was null.");

            IList<string> textStyleNames = new List<string>();

            using (var textStyleTable = (TextStyleTable) trans.GetObject(db.TextStyleTableId, OpenMode.ForWrite, false))
            {
                foreach (var id in textStyleTable)
                {
                    using (var record = trans.GetObject(id, OpenMode.ForRead) as TextStyleTableRecord)
                    {
                        if (record.Name.ToUpper().StartsWith(filter.ToUpper().Replace("*", string.Empty)))
                        {
                            textStyleNames.Add(record.Name);
                        }
                    }
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End GetTextStyleNames");
            return textStyleNames;
        }

        /// <summary>
        ///     Sets a text style height
        /// </summary>
        /// <param name="db"></param>
        /// <param name="textStyleName"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static bool SetTextStyleHeight(Database db, string textStyleName, double height)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start SetTextStyleHeight");
            var retval = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                retval = SetTextStyleHeight(db, trans, textStyleName, height);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End SetTextStyleHeight");
            return retval;
        }

        /// <summary>
        ///     Sets a text style height
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="textStyleName"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static bool SetTextStyleHeight(Database db, Transaction trans, string textStyleName, double height)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start SetTextStyleHeight");
            var retval = false;
            using (var textStyleTable = (TextStyleTable) trans.GetObject(db.TextStyleTableId, OpenMode.ForWrite, false))
            {
                if (textStyleTable.Has(textStyleName))
                {
                    var id = textStyleTable[textStyleName];

                    using (var record = trans.GetObject(id, OpenMode.ForWrite) as TextStyleTableRecord)
                    {
                        record.TextSize = height;
                        retval = true;
                    }
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End SetTextStyleHeight");
            return retval;
        }

        #region Private Members

        //private static readonly ILog //_logger = LogManager.GetLogger(typeof(TextStyleManager));

        #endregion
    }
}