#region

using System;
using System.Collections.Generic;
using global::Autodesk.AutoCAD.DatabaseServices;

#endregion

namespace BBC.Common.AutoCAD
{
    public class DimStyleManager
    {
        /// <summary>
        ///     Gets a list of dimension style names.
        /// </summary>
        /// <param name="db">The database to read.</param>
        /// <returns></returns>
        public static IList<string> GetDimStyleNames(Database db)
        {
            IList<string> dimStyleNames = new List<string>();
            using (var trans = db.TransactionManager.StartTransaction())
            {
                dimStyleNames = GetDimStyleNames(db, trans);
                trans.Commit();
            }
            return dimStyleNames;
        }

        /// <summary>
        ///     Gets a list of dimension style names.
        /// </summary>
        /// <param name="db">The database to read.</param>
        /// <param name="trans">The transaction to use.</param>
        /// <returns>A list of dimension style names</returns>
        public static IList<string> GetDimStyleNames(Database db, Transaction trans)
        {
            IList<string> dimStyleNames = new List<string>();
            using (var dimStyleTable = (DimStyleTable) trans.GetObject(db.DimStyleTableId, OpenMode.ForWrite, false))
            {
                foreach (var id in dimStyleTable)
                {
                    using (var record = trans.GetObject(id, OpenMode.ForRead) as DimStyleTableRecord)
                    {
                        dimStyleNames.Add(record.Name);
                    }
                }
            }
            return dimStyleNames;
        }

        /// <summary>
        ///     Gets a list of dimension style names.
        /// </summary>
        /// <param name="db">The database to read.</param>
        /// <param name="trans">The transaction to use.</param>
        /// <returns>A list of dimension style names</returns>
        public static IList<string> GetDimStyleNames(Database db, Transaction trans, string filter)
        {
            if (trans == null)
                throw new ArgumentNullException("trans", "The argument 'trans' was null.");

            IList<string> dimStyleNames = new List<string>();

            using (var dimStyleTable = (DimStyleTable) trans.GetObject(db.DimStyleTableId, OpenMode.ForWrite, false))
            {
                foreach (var id in dimStyleTable)
                {
                    using (var record = trans.GetObject(id, OpenMode.ForRead) as DimStyleTableRecord)
                    {
                        if (record.Name.ToUpper().StartsWith(filter.ToUpper().Replace("*", string.Empty)))
                        {
                            dimStyleNames.Add(record.Name);
                        }
                    }
                }
            }
            return dimStyleNames;
        }

        /// <summary>
        ///     Sets a dimension style Dimexe property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetDimexe(Database db, string dimStyleName, double value)
        {
            var retval = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                retval = SetDimexe(db, trans, dimStyleName, value);
                trans.Commit();
            }
            return retval;
        }

        /// <summary>
        ///     Sets a dimension style Dimexe property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetDimexe(Database db, Transaction trans, string dimStyleName, double value)
        {
            var retval = false;
            using (var dimStyleTable = (DimStyleTable) trans.GetObject(db.DimStyleTableId, OpenMode.ForWrite, false))
            {
                if (dimStyleTable.Has(dimStyleName))
                {
                    var id = dimStyleTable[dimStyleName];

                    using (var record = trans.GetObject(id, OpenMode.ForWrite) as DimStyleTableRecord)
                    {
                        record.Dimexe = value;
                        retval = true;
                    }
                }
            }
            return retval;
        }

        /// <summary>
        ///     Sets a dimension style Dimexo property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetDimexo(Database db, string dimStyleName, double value)
        {
            var retval = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                retval = SetDimexo(db, trans, dimStyleName, value);
                trans.Commit();
            }
            return retval;
        }

        /// <summary>
        ///     Sets a dimension style Dimexo property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetDimexo(Database db, Transaction trans, string dimStyleName, double value)
        {
            var retval = false;
            using (var dimStyleTable = (DimStyleTable) trans.GetObject(db.DimStyleTableId, OpenMode.ForWrite, false))
            {
                if (dimStyleTable.Has(dimStyleName))
                {
                    var id = dimStyleTable[dimStyleName];

                    using (var record = trans.GetObject(id, OpenMode.ForWrite) as DimStyleTableRecord)
                    {
                        record.Dimexo = value;
                        retval = true;
                    }
                }
            }
            return retval;
        }

        /// <summary>
        ///     Sets a dimension style Dimdli property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetDimdli(Database db, string dimStyleName, double value)
        {
            var retval = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                retval = SetDimdli(db, trans, dimStyleName, value);
                trans.Commit();
            }
            return retval;
        }

        /// <summary>
        ///     Sets a dimension style Dimdli property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetDimdli(Database db, Transaction trans, string dimStyleName, double value)
        {
            var retval = false;
            using (var dimStyleTable = (DimStyleTable) trans.GetObject(db.DimStyleTableId, OpenMode.ForWrite, false))
            {
                if (dimStyleTable.Has(dimStyleName))
                {
                    var id = dimStyleTable[dimStyleName];

                    using (var record = trans.GetObject(id, OpenMode.ForWrite) as DimStyleTableRecord)
                    {
                        record.Dimdli = value;
                        retval = true;
                    }
                }
            }
            return retval;
        }

        /// <summary>
        ///     Sets a dimension style Dimasz property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetDimasz(Database db, string dimStyleName, double value)
        {
            var retval = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                retval = SetDimasz(db, trans, dimStyleName, value);
                trans.Commit();
            }
            return retval;
        }

        /// <summary>
        ///     Sets a dimension style Dimasz property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetDimasz(Database db, Transaction trans, string dimStyleName, double value)
        {
            var retval = false;
            using (var dimStyleTable = (DimStyleTable) trans.GetObject(db.DimStyleTableId, OpenMode.ForWrite, false))
            {
                if (dimStyleTable.Has(dimStyleName))
                {
                    var id = dimStyleTable[dimStyleName];

                    using (var record = trans.GetObject(id, OpenMode.ForWrite) as DimStyleTableRecord)
                    {
                        record.Dimasz = value;
                        retval = true;
                    }
                }
            }
            return retval;
        }

        /// <summary>
        ///     Sets a dimension style Dimcen property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetDimcen(Database db, string dimStyleName, double value)
        {
            var retval = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                retval = SetDimcen(db, trans, dimStyleName, value);
                trans.Commit();
            }
            return retval;
        }

        /// <summary>
        ///     Sets a dimension style Dimcen property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetDimcen(Database db, Transaction trans, string dimStyleName, double value)
        {
            var retval = false;
            using (var dimStyleTable = (DimStyleTable) trans.GetObject(db.DimStyleTableId, OpenMode.ForWrite, false))
            {
                if (dimStyleTable.Has(dimStyleName))
                {
                    var id = dimStyleTable[dimStyleName];

                    using (var record = trans.GetObject(id, OpenMode.ForWrite) as DimStyleTableRecord)
                    {
                        record.Dimcen = value;
                        retval = true;
                    }
                }
            }
            return retval;
        }

        /// <summary>
        ///     Sets a dimension style Dimgap property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetDimgap(Database db, string dimStyleName, double value)
        {
            var retval = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                retval = SetDimgap(db, trans, dimStyleName, value);
                trans.Commit();
            }
            return retval;
        }

        /// <summary>
        ///     Sets a dimension style Dimgap property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetDimgap(Database db, Transaction trans, string dimStyleName, double value)
        {
            var retval = false;
            using (var dimStyleTable = (DimStyleTable) trans.GetObject(db.DimStyleTableId, OpenMode.ForWrite, false))
            {
                if (dimStyleTable.Has(dimStyleName))
                {
                    var id = dimStyleTable[dimStyleName];

                    using (var record = trans.GetObject(id, OpenMode.ForWrite) as DimStyleTableRecord)
                    {
                        record.Dimgap = value;
                        retval = true;
                    }
                }
            }
            return retval;
        }


        /// <summary>
        ///     Gets a dimension style Dimexe property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetDimexe(Database db, string dimStyleName)
        {
            double retval = 0;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                retval = GetDimexe(db, trans, dimStyleName);
                trans.Commit();
            }
            return retval;
        }

        /// <summary>
        ///     Gets a dimension style Dimexe property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetDimexe(Database db, Transaction trans, string dimStyleName)
        {
            double retval = 0;
            using (var dimStyleTable = (DimStyleTable) trans.GetObject(db.DimStyleTableId, OpenMode.ForWrite, false))
            {
                if (dimStyleTable.Has(dimStyleName))
                {
                    var id = dimStyleTable[dimStyleName];

                    using (var record = trans.GetObject(id, OpenMode.ForWrite) as DimStyleTableRecord)
                    {
                        retval = record.Dimexe;
                    }
                }
            }
            return retval;
        }

        /// <summary>
        ///     Gets a dimension style Dimexo property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetDimexo(Database db, string dimStyleName)
        {
            double retval = 0;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                retval = GetDimexo(db, trans, dimStyleName);
                trans.Commit();
            }
            return retval;
        }

        /// <summary>
        ///     Gets a dimension style Dimexo property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetDimexo(Database db, Transaction trans, string dimStyleName)
        {
            double retval = 0;
            using (var dimStyleTable = (DimStyleTable) trans.GetObject(db.DimStyleTableId, OpenMode.ForWrite, false))
            {
                if (dimStyleTable.Has(dimStyleName))
                {
                    var id = dimStyleTable[dimStyleName];

                    using (var record = trans.GetObject(id, OpenMode.ForWrite) as DimStyleTableRecord)
                    {
                        retval = record.Dimexo;
                    }
                }
            }
            return retval;
        }

        /// <summary>
        ///     Gets a dimension style Dimdli property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetDimdli(Database db, string dimStyleName)
        {
            double retval = 0;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                retval = GetDimdli(db, trans, dimStyleName);
                trans.Commit();
            }
            return retval;
        }

        /// <summary>
        ///     Gets a dimension style Dimdli property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetDimdli(Database db, Transaction trans, string dimStyleName)
        {
            double retval = 0;
            using (var dimStyleTable = (DimStyleTable) trans.GetObject(db.DimStyleTableId, OpenMode.ForWrite, false))
            {
                if (dimStyleTable.Has(dimStyleName))
                {
                    var id = dimStyleTable[dimStyleName];

                    using (var record = trans.GetObject(id, OpenMode.ForWrite) as DimStyleTableRecord)
                    {
                        retval = record.Dimdli;
                    }
                }
            }
            return retval;
        }

        /// <summary>
        ///     Gets a dimension style Dimasz property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetDimasz(Database db, string dimStyleName)
        {
            double retval = 0;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                retval = GetDimasz(db, trans, dimStyleName);
                trans.Commit();
            }
            return retval;
        }

        /// <summary>
        ///     Gets a dimension style Dimasz property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetDimasz(Database db, Transaction trans, string dimStyleName)
        {
            double retval = 0;
            using (var dimStyleTable = (DimStyleTable) trans.GetObject(db.DimStyleTableId, OpenMode.ForWrite, false))
            {
                if (dimStyleTable.Has(dimStyleName))
                {
                    var id = dimStyleTable[dimStyleName];

                    using (var record = trans.GetObject(id, OpenMode.ForWrite) as DimStyleTableRecord)
                    {
                        retval = record.Dimasz;
                    }
                }
            }
            return retval;
        }

        /// <summary>
        ///     Gets a dimension style Dimcen property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetDimcen(Database db, string dimStyleName)
        {
            double retval = 0;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                retval = GetDimcen(db, trans, dimStyleName);
                trans.Commit();
            }
            return retval;
        }

        /// <summary>
        ///     Gets a dimension style Dimcen property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetDimcen(Database db, Transaction trans, string dimStyleName)
        {
            double retval = 0;
            using (var dimStyleTable = (DimStyleTable) trans.GetObject(db.DimStyleTableId, OpenMode.ForWrite, false))
            {
                if (dimStyleTable.Has(dimStyleName))
                {
                    var id = dimStyleTable[dimStyleName];

                    using (var record = trans.GetObject(id, OpenMode.ForWrite) as DimStyleTableRecord)
                    {
                        retval = record.Dimcen;
                    }
                }
            }
            return retval;
        }

        /// <summary>
        ///     Gets a dimension style Dimgap property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetDimgap(Database db, string dimStyleName)
        {
            double retval = 0;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                retval = GetDimgap(db, trans, dimStyleName);
                trans.Commit();
            }
            return retval;
        }

        /// <summary>
        ///     Gets a dimension style Dimgap property
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="dimStyleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetDimgap(Database db, Transaction trans, string dimStyleName)
        {
            double retval = 0;
            using (var dimStyleTable = (DimStyleTable) trans.GetObject(db.DimStyleTableId, OpenMode.ForWrite, false))
            {
                if (dimStyleTable.Has(dimStyleName))
                {
                    var id = dimStyleTable[dimStyleName];

                    using (var record = trans.GetObject(id, OpenMode.ForWrite) as DimStyleTableRecord)
                    {
                        retval = record.Dimgap;
                    }
                }
            }
            return retval;
        }
    }
}