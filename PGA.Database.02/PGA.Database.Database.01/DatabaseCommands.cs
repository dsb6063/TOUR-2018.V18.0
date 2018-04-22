#region

using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Win32.SafeHandles;
using PGA.DataContext;

#endregion

namespace PGA.Database
{
    /// <summary>
    ///     Class DatabaseCommands.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    [Obfuscation(Feature = "renaming", Exclude = true)]
    public class DatabaseCommands : IDisposable
    {
        // Flag: Has Dispose already been called?
        /// <summary>
        ///     The disposed
        /// </summary>
        private bool disposed;

        // Instantiate a SafeHandle instance.
        /// <summary>
        ///     The handle
        /// </summary>
        private readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        // Public implementation of Dispose pattern callable by consumers.
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Gets the name of the feature index by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public string GetFeatureIndexByName(string name)
        {
            try
            {
                if ((name == null) || (name.Length < 2))
                    return name;

                var sub = name.Substring(2);
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = (from p in context.FeatureIndex
                        where p.FeatureName == name.Substring(2)
                        select p.Code.Trim()).FirstOrDefault();

                    return items;
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
            return null;
        }


        //public IQueryable<PGA.DataContext.Logs> FilterLogsForErrors()
        //{
        //    try
        //    {
        //        IQueryable<PGA.DataContext.Logs> results = null;
        //        using (var context = GetDataBasePath.GetSql4Connection())
        //        {
        //            var logs = (from p in context.Logs
        //                        select p);

        //            if (logs.FirstOrDefault() != null)
        //            {
        //                results = logs.Where(p => p.Issue.Contains("Alert") ||
        //                        p.Issue.Contains("Exception") ||
        //                        p.Issue.Contains("Error") ||
        //                        p.Issue.Contains("Locked") ||
        //                        p.Issue.Contains("Runtime"));
        //                ;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        PGA.Database.DatabaseLogs
        //            .FormatLogs(ex.Message);
        //    }
        //    return null;
        //}

        public List<Logs> FilterLogsForErrors()
        {
            try
            {
                IList<Logs> results = new List<Logs>();
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    context.DeferredLoadingEnabled = true;

                    var logs = from p in context.Logs
                        select p;

                    if (logs.FirstOrDefault() != null)
                    {
                        var query = logs.Where(p => p.Issue.Contains("Alert") ||
                                                    p.Issue.Contains("Exception") ||
                                                    p.Issue.Contains("Error") ||
                                                    p.Issue.Contains("Locked") ||
                                                    p.Issue.Contains("Runtime"));

                        foreach (var item in query)
                        {
                            results.Add(item);
                        }
                    }
                    if (results.Count > 0)
                        return results.ToList();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
            return null;
        }

        /// <summary>
        ///     Gets the timer information.
        /// </summary>
        /// <returns>List&lt;PGA.DataContext.Timer&gt;.</returns>
        public List<Timer> GetTimerInfo()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = from p in context.Timer
                        select p;
                    if (items.FirstOrDefault() != null)
                        return items.ToList();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
            return null;
        }

        /// <summary>
        ///     Deletes the timer information.
        /// </summary>
        public void DeleteTimerInfo()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var time = from p in context.Timer
                        select p;

                    if (time.FirstOrDefault() == null)
                        return;

                    context.Timer.DeleteAllOnSubmit(time);
                    context.SubmitChanges();

                    ResetIdentityColumns("Timer");
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
        }


        /// <summary>
        ///     Updates the total DWGS timer information.
        /// </summary>
        /// <param name="values">The values.</param>
        public void UpdateTotalDWGSTimerInfo(int values)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var time = (from p in context.Timer
                        select p).FirstOrDefault();

                    time.TotalDwgs = values;

                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
        }

        public void UpdateCourseDetails(string oldname, CourseDetails course)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var c = (from p in context.CourseDetails
                        where p.Name == oldname
                        select p).FirstOrDefault();

                    c.City = course.City;
                    c.CourseNum = course.CourseNum;
                    c.Name = course.Name;
                    c.State = course.State;
                    c.TOURCode = course.TOURCode;

                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
        }

        /// <summary>
        ///     Updates the course details.
        /// </summary>
        /// <param name="course">The course.</param>
        public void UpdateCourseDetails(CourseDetails course)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var c = (from p in context.CourseDetails
                        where p.Id == course.Id
                        select p).FirstOrDefault();

                    c.City = course.City;
                    c.CourseNum = course.CourseNum;
                    c.Name = course.Name;
                    c.State = course.State;
                    c.TOURCode = course.TOURCode;

                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
        }

        /// <summary>
        ///     Updates all timer information.
        /// </summary>
        /// <param name="values">The values.</param>
        public void UpdateAllTimerInfo(Timer values)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var time = (from p in context.Timer
                        select p).FirstOrDefault();

                    if (time == null)
                        return;

                    if (!IsNUllorEmpty(values.ActualTime))
                        time.ActualTime = values.ActualTime;
                    if (!IsNUllorEmpty(values.Average))
                        time.Average = values.Average;
                    if (!IsNUllorEmpty(values.Counter))
                        time.Counter = values.Counter;
                    if (!IsNUllorEmpty(values.EstimatedTime))
                        time.EstimatedTime = values.EstimatedTime;
                    if (!IsNUllorEmpty(values.Mean))
                        time.Mean = values.Mean;
                    if (!IsNUllorEmpty(values.Percentage))
                        time.Percentage = values.Percentage;
                    if (!IsNUllorEmpty(values.S1Percentage))
                        time.S1Percentage = values.S1Percentage;
                    if (!IsNUllorEmpty(values.S2Percentage))
                        time.S2Percentage = values.S2Percentage;
                    if (!IsNUllorEmpty(values.S3Percentage))
                        time.S3Percentage = values.S3Percentage;
                    if (!IsNUllorEmpty(values.TotalDwgs))
                        time.TotalDwgs = values.TotalDwgs;

                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
        }

        /// <summary>
        ///     Checks for timer recs.
        /// </summary>
        public void CheckForTimerRecs()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var time = (from p in context.Timer
                        select p).FirstOrDefault();

                    if (time == null)
                    {
                        time = new Timer();
                        context.Timer.InsertOnSubmit(time);
                        context.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
        }

        /// <summary>
        ///     Clears the timer information.
        /// </summary>
        public void ClearTimerInfo()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var time = (from p in context.Timer
                        select p).FirstOrDefault();

                    if (time == null)
                        return;

                    time.ActualTime = null;
                    time.Average = null;
                    time.Counter = 0;
                    time.EstimatedTime = null;
                    time.Mean = null;
                    time.Percentage = 0;
                    time.S1Percentage = 0;
                    time.S2Percentage = 0;
                    time.S3Percentage = 0;
                    time.TotalDwgs = 0;

                    context.SubmitChanges();

                    ResetIdentityColumns("Timer");
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
        }

        /// <summary>
        ///     Determines whether [is n ullor empty] [the specified value].
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns><c>true</c> if [is n ullor empty] [the specified value]; otherwise, <c>false</c>.</returns>
        public bool IsNUllorEmpty(object val)
        {
            try
            {
                var str = val.ToString();

                if (!string.IsNullOrEmpty(str))
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
            }
            return true;
        }

        /// <summary>
        ///     Sets the s1 timer information.
        /// </summary>
        /// <param name="sg1value">The sg1value.</param>
        public void SetS1TimerInfo(int sg1value)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var time = (from p in context.Timer
                        select p).FirstOrDefault();

                    time.S1Percentage = sg1value;

                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
        }

        /// <summary>
        ///     Sets the s2 timer information.
        /// </summary>
        /// <param name="sg2value">The sg2value.</param>
        public void SetS2TimerInfo(int sg2value)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var time = (from p in context.Timer
                        select p).FirstOrDefault();

                    time.S2Percentage = sg2value;

                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
        }

        /// <summary>
        ///     Sets the s3 timer information.
        /// </summary>
        /// <param name="sg3value">The sg3value.</param>
        public void SetS3TimerInfo(int sg3value)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var time = (from p in context.Timer
                        select p).FirstOrDefault();

                    time.S3Percentage = sg3value;

                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
        }


        /// <summary>
        ///     Gets the feature index by code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>System.String.</returns>
        public string GetFeatureIndexByCode(string code)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = from p in context.FeatureIndex
                        where p.FeatureName == code
                        select p.FeatureName;

                    return items.ToString();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
            return null;
        }

        /// <summary>
        ///     Gets the object identifier.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <returns>Int64.</returns>
        public static long GetObjectId(string handle)
        {
            return long.Parse(handle, NumberStyles.AllowHexSpecifier);
        }

        /// <summary>
        ///     Gets the exc features object FRMT.
        /// </summary>
        /// <returns>IQueryable&lt;System.Int64&gt;.</returns>
        public IQueryable<long> GetExcFeaturesObjFrmt()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = from p in context.ExcludedFeatures
                        select GetObjectId(p.Handle);

                    return items;
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
            return null;
        }

        /// <summary>
        ///     Gets all excluded features_ v2.
        /// </summary>
        /// <returns>IList&lt;ExcludedFeatures&gt;.</returns>
        public IList<ExcludedFeatures> GetAllExcludedFeatures_V2()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = from p in context.ExcludedFeatures
                        select p;

                    return items.ToList();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
            return null;
        }

        /// <summary>
        ///     Gets all excluded features.
        /// </summary>
        /// <returns>IQueryable&lt;System.String&gt;.</returns>
        public IQueryable<string> GetAllExcludedFeatures()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = from p in context.ExcludedFeatures
                        select p.Handle.Trim();

                    return items;
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
            return null;
        }

        /// <summary>
        ///     Clears the excluded features.
        /// </summary>
        public void ClearExcludedFeatures()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var excluded = from p in context.ExcludedFeatures
                        select p;

                    context.ExcludedFeatures.DeleteAllOnSubmit(excluded);
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
        }

        /// <summary>
        ///     Inserts to excluded features.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void InsertToExcludedFeatures(string id)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var excluded = new ExcludedFeatures();
                    excluded.Handle = id;
                    context.ExcludedFeatures.InsertOnSubmit(excluded);
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
        }

        /// <summary>
        ///     Gets all features.
        /// </summary>
        /// <returns>IQueryable&lt;FeatureIndex&gt;.</returns>
        public IQueryable<FeatureIndex> GetAllFeatures()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var features = from p in context.FeatureIndex
                        select p;

                    return features;
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
            return null;
        }

        /// <summary>
        ///     Gets the feature rank by code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>System.Int32.</returns>
        public int GetFeatureRankByCode(string code)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var rank = (from p in context.FeatureIndex
                        where p.Code.Trim().Equals(code.Trim())
                        select p.Rank).FirstOrDefault();

                    return rank;
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
            return 0;
        }


        /// <summary>
        ///     Inserts the notifications.
        /// </summary>
        /// <param name="note">The note.</param>
        public void InsertNotifications(string note)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var notify = new Notifications();
                    notify.DateStamp = DateTime.Now;
                    notify.HasError = false;
                    notify.Command = note;
                    context.Notifications.InsertOnSubmit(notify);
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
        }

        /// <summary>
        ///     Inserts the course detail.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="city">The city.</param>
        /// <param name="state">The state.</param>
        /// <param name="coursenum">The coursenum.</param>
        /// <param name="tourcode">The tourcode.</param>
        public void InsertCourseDetail(string name, string city, string state, string coursenum, string tourcode)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var course = new CourseDetails();
                    course.Name = name;
                    course.City = city;
                    course.State = state;
                    course.CourseNum = coursenum;
                    course.TOURCode = tourcode;
                    context.CourseDetails.InsertOnSubmit(course);
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs
                    .FormatLogs(ex.Message);
            }
        }

        /// <summary>
        ///     Gets the notifications.
        /// </summary>
        /// <returns>Notifications.</returns>
        public Notifications GetNotifications()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var note = (from p in context.Notifications
                    where p.HasError == false
                    select p).FirstOrDefault();

                if (note != null)
                {
                    DeleteNotifications(note.Id);

                    return note;
                }
            }
            return null;
        }

        /// <summary>
        ///     Gets the notifications.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>Notifications.</returns>
        public Notifications GetNotifications(DateTime date)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = (from p in context.Notifications
                    where p.DateStamp.Equals(date)
                    select p).FirstOrDefault();
                return path;
            }
        }

        /// <summary>
        ///     Deletes the notifications.
        /// </summary>
        public void DeleteNotifications()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = from p in context.Notifications
                    where p.HasError
                    select p;
                context.Notifications.DeleteAllOnSubmit(path);
                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Deletes the notifications.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void DeleteNotifications(int id)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = from p in context.Notifications
                    where p.Id == id
                    select p;
                context.Notifications.DeleteAllOnSubmit(path);
                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Deletes the course by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void DeleteCourseById(int id)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = from p in context.CourseDetails
                    where p.Id == id
                    select p;
                context.CourseDetails.DeleteAllOnSubmit(path);
                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Deletes the name of the course by.
        /// </summary>
        /// <param name="name">The name.</param>
        public void DeleteCourseByName(string name)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = from p in context.CourseDetails
                    where p.Name == name
                    select p;
                context.CourseDetails.DeleteAllOnSubmit(path);
                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Gets the template path.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetTemplatePath()
        {
            return GetDataBasePath.GetTemplatePath("");
        }

        /// <summary>
        ///     Gets the drawing stack information by date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>DrawingStack.</returns>
        public DrawingStack GetDrawingStackInfoByDate(DateTime date)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = (from p in context.DrawingStack
                    where p.DateStamp.Equals(date)
                    select p).FirstOrDefault();
                return path;
            }
        }

        /// <summary>
        ///     Gets the drawing stack information by date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="hole">The hole.</param>
        /// <returns>DrawingStack.</returns>
        public DrawingStack GetDrawingStackInfoByDate(DateTime date, string hole)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = (from p in context.DrawingStack
                    where p.DateStamp.Equals(date) &&
                          p.Hole.Equals(Convert.ToInt64(hole))
                    select p).FirstOrDefault();
                return path;
            }
        }

        /// <summary>
        ///     Gets the drawing stack information by date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="name">The name.</param>
        /// <returns>DrawingStack.</returns>
        public DrawingStack GetDrawingStackInfoByName(DateTime date, string name)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = (from p in context.DrawingStack
                    where p.DateStamp.Equals(date) &&
                          p.PolylineDwgName.Equals(name)
                    select p).FirstOrDefault();
                return path;
            }
        }


        /// <summary>
        ///     Gets the point path by date. Version 1.0
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="hole">The hole.</param>
        /// <returns>System.String.</returns>
        public string GetPointPathByDate(DateTime date, string hole)
        {
            var command = new DatabaseCommands();

            var stack = command.GetDrawingStackInfoByDate(date, hole);

            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = (from p in context.Settings
                    where p.DateStamp.Equals(date)
                    select p).FirstOrDefault();

                var dir = path.GlobalPointFilePath;
                var name = stack.PointCloudDwgName;

                return Path.Combine(dir, name);
            }
        }

        /// <summary>
        ///     Gets the point path by date. Version 2.0
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>System.String.</returns>
        public string GetPointPathByName(DateTime date, string filename)
        {
            var command = new DatabaseCommands();

            var stack = command.GetDrawingStackInfoByName(date, filename);

            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var dwg = (from p in context.PointCloud
                    where p.DateStamp.Equals(date)
                    select p).FirstOrDefault();

                var dir = dwg.SourcePath;
                var name = stack.PointCloudDwgName;

                return Path.Combine(dir, name);
            }
        }

        /// <summary>
        ///     Sets the date started in task.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool SetDateStartedInTask(DateTime date)
        {
            var settings = GetProjectID(date);
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = (from p in context.Tasks
                    where p.ProjectID.Equals(Convert.ToInt32(settings.Id))
                    select p).FirstOrDefault();

                path.TimeStarted = DateTime.Now;

                context.SubmitChanges();

                return true;
            }
        }

        /// <summary>
        ///     Sets the date ended in task.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool SetDateEndedInTask(DateTime date)
        {
            var settings = GetProjectID(date);
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = (from p in context.Tasks
                    where p.ProjectID.Equals(Convert.ToInt32(settings.Id))
                    select p).FirstOrDefault();

                path.TimeEnded = DateTime.Now;

                context.SubmitChanges();

                return true;
            }
        }

        /// <summary>
        ///     Gets the project identifier.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>Settings.</returns>
        public Settings GetProjectID(DateTime date)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = (from p in context.Settings
                    where p.DateStamp.Equals(date)
                    select p).FirstOrDefault();
                return path;
            }
        }


        /// <summary>
        ///     Loads the drawing stack.
        /// </summary>
        /// <param name="dateSelected">The date selected.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">
        ///     Polylines are null!
        ///     or
        ///     Pointclouds are null!
        /// </exception>
        public bool LoadDrawingStack(DateTime dateSelected)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var polylines =
                        context.PolylineDWGS.Where(
                            p => p.DateStamp == Convert.ToDateTime(dateSelected, CultureInfo.InvariantCulture));

                    var pointclouds =
                        context.PointCloud.Where(
                            p => p.DateStamp == Convert.ToDateTime(dateSelected, CultureInfo.InvariantCulture));

                    if (polylines.FirstOrDefault() == null)
                        throw new ArgumentNullException("Polylines are null!");
                    if (pointclouds.FirstOrDefault() == null)
                        throw new ArgumentNullException("Pointclouds are null!");

                    var drawingstack = new DrawingStack();

                    foreach (var pl in polylines)
                    {
                        foreach (var pc in pointclouds)
                        {
                            if (!StringHelpers.FileNameValidation(pc.DrawingName))
                            {
                                if (CompareByFileNameXX(pl.DrawingName, pc.DrawingName))
                                {
                                    var stack = new DrawingStack();
                                    stack.DateStamp = pc.DateStamp;
                                    stack.PointCloudDwgName = pc.DrawingName;
                                    stack.PolylineDwgName = pl.DrawingName;
                                    stack.SourcePolylineDwgID = pl.Id;
                                    stack.Hole = lOrder(pc.DrawingName);
                                    context.DrawingStack.InsertOnSubmit(stack);
                                    context.SubmitChanges();

                                    DatabaseLogs.FormatLogs(string.Format
                                    ("Added {0} DWG to Stack!",
                                        pc.DrawingName), pc.Id.ToString());
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "");
            }
            return false;
        }

        /// <summary>
        ///     Saves the task by date.
        /// </summary>
        /// <param name="dateSelected">The date selected.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool SaveTaskByDate(DateTime dateSelected)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var t = new Tasks();

                var task = GetTaskByDate(dateSelected);
                task.TimeStarted = DateTime.Now; //DateConverts.GetDateTimeNow();
                task.IsCancelled = false;
                task.IsPaused = true;

                context.SubmitChanges();
            }
            return true;
        }

        /// <summary>
        ///     Compares the name of the by file.
        /// </summary>
        /// <param name="file1">The file1.</param>
        /// <param name="file2">The file2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool CompareByFileName(string file1, string file2)
        {
            try
            {
                if (file1.Equals(file2, StringComparison.InvariantCultureIgnoreCase))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "CompareByFileName: Failed to convert Hole! ");
            }
            return false;
        }

        /// <summary>
        ///     Compares the by file name xx.
        /// </summary>
        /// <param name="file1">The file1.</param>
        /// <param name="file2">The file2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool CompareByFileNameXX(string file1, string file2)
        {
            try
            {
                var r1 = file1.Substring(file1.Length - 6, 2);
                var r2 = file2.Substring(file2.Length - 6, 2);

                if (r1.Equals(r2, StringComparison.InvariantCultureIgnoreCase))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "CompareByFileNameXX: Failed to convert Hole! ");
            }
            return false;
        }

        /// <summary>
        ///     Gets the Hole.
        /// </summary>
        /// <param name="file">File Name</param>
        /// <returns>System.Nullable&lt;System.Int64&gt;.</returns>
        private long? lOrder(string file)
        {
            var rand = new Random();
            try
            {
                if (CompareRegx("all", file))
                {
                    DatabaseLogs.FormatLogs("Omitted File: ", file);
                    return 0;
                }

                return Convert.ToInt64(file.Substring(file.Length - 6, 2));
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "LOrder Failed to convert Hole! ");
            }
            return rand.Next(50, 99);
        }

        /// <summary>
        ///     Holes the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>System.String.</returns>
        private string Hole(string file)
        {
            if (CompareRegx("all", file))
            {
                DatabaseLogs.FormatLogs("Omitted File: ", file);
                return 0.ToString();
            }

            return file.Substring(file.Length - 6, 2);
        }

        /// <summary>
        ///     Compares the case.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="paragraph">The paragraph.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool CompareCase(string word, string paragraph)
        {
            var culture = new CultureInfo("en-US");
            return culture.CompareInfo.IndexOf(paragraph, word, CompareOptions.IgnoreCase) >= 0;
        }

        /// <summary>
        ///     Compares the regx.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="paragraph">The paragraph.</param>
        /// <returns><c>true</c> if true if match, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private bool CompareRegx(string word, string paragraph)
        {
            return Regex.IsMatch(paragraph.Substring(0, 3), word, RegexOptions.IgnoreCase);
        }

        /// <summary>
        ///     Gets the full DWG path.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="hole">The hole.</param>
        /// <returns>System.String.</returns>
        public string GetFullDWGPath(DateTime time, int hole)
        {
            var _hole = Convert.ToInt64(hole);
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = (from p in context.Settings
                    where p.DateStamp.Equals(time)
                    select p.GlobalPolylineFilePath).FirstOrDefault();

                var fullpath = (from p in context.DrawingStack
                    where p.DateStamp.Equals(time) && p.Hole.Equals(_hole)
                    select p.PolylineDwgName).FirstOrDefault();

                return Path.Combine(path, fullpath);
            }
        }

        /// <summary>
        ///     Gets the full DWG path.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="hole">The hole.</param>
        /// <returns>System.String.</returns>
        public string GetFullDWGPath(DateTime time, string hole)
        {
            var _hole = Convert.ToInt64(hole);
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = (from p in context.Settings
                    where p.DateStamp.Equals(time)
                    select p.GlobalPolylineFilePath).FirstOrDefault();

                var fullpath = (from p in context.DrawingStack
                    where p.DateStamp.Equals(time) && p.Hole.Equals(_hole)
                    select p.PolylineDwgName).FirstOrDefault();

                return Path.Combine(path, fullpath);
            }
        }

        /// <summary>
        ///     Gets the global DWG path.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetGlobalDWGPath()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = (from p in context.Settings
                    select p.GlobalPolylineFilePath).FirstOrDefault();

                return path;
            }
        }

        /// <summary>
        ///     Gets the global DWG path.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>System.String.</returns>
        public string GetGlobalDWGPath(DateTime date)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = (from p in context.Settings
                    where p.DateStamp == Convert.ToDateTime(date)
                    select p.GlobalPolylineFilePath).FirstOrDefault();

                return path;
            }
        }

        /// <summary>
        ///     Gets the global destination path.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>System.String.</returns>
        public string GetGlobalDestinationPath(DateTime date)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = (from p in context.Settings
                    where p.DateStamp == Convert.ToDateTime(date)
                    select p.DestinationFolder).FirstOrDefault();

                return path;
            }
        }

        /// <summary>
        ///     Gets the global point cloud path.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>System.String.</returns>
        public string GetGlobalPointCloudPath(DateTime date)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = (from p in context.Settings
                    where p.DateStamp == Convert.ToDateTime(date)
                    select p.GlobalPointFilePath).FirstOrDefault();

                return path;
            }
        }

        /// <summary>
        ///     Gets the settings custom.
        /// </summary>
        /// <returns>IList&lt;Settings&gt;.</returns>
        public IList<Settings> GetSettingsCustom()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var tasks = from p in context.Settings
                    orderby p.Id ascending
                    select p;

                if (tasks.FirstOrDefault() == null)
                    return null;

                return tasks.ToList();
            }
        }

        /// <summary>
        ///     Gets the settings and task information.
        /// </summary>
        /// <returns>IList&lt;Settings&gt;.</returns>
        public IList<Settings> GetSettingsAndTaskInfo()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var tasks = (from p in context.Tasks
                    orderby p.Id ascending
                    select p).FirstOrDefault();

                if (tasks == null)
                    return null;

                var settings = from p in context.Settings
                    orderby p.Id ascending
                    select p;

                if (settings.FirstOrDefault() == null)
                    return null;

                return settings.ToList();
            }
        }

        /// <summary>
        ///     Gets the settings and task with filter.
        /// </summary>
        /// <returns>IList&lt;Settings&gt;.</returns>
        public IList<Settings> GetSettingsAndTaskWithFilter()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var tasks = (from p in context.Tasks
                    orderby p.Id ascending
                    where p.IsCancelled.Value == false
                    select p).FirstOrDefault();

                if (tasks == null)
                    return null;

                var settings = from p in context.Settings
                    orderby p.Id ascending
                    select p;

                if (settings.FirstOrDefault() == null)
                    return null;

                return settings.ToList();
            }
        }

        /// <summary>
        ///     Gets the settings by date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>Settings.</returns>
        public Settings GetSettingsByDate(DateTime date)
        {
            using (var commands = new DatabaseCommands())
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var tasks = (from p in context.Settings
                        where p.DateStamp == date
                        select p).FirstOrDefault();
                    return tasks;
                }
            }
        }

        /// <summary>
        ///     Determines whether [is DXF version by date] [the specified date].
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns><c>true</c> if [is DXF version by date] [the specified date]; otherwise, <c>false</c>.</returns>
        public bool IsDXFVersionByDate(DateTime date)
        {
            using (var commands = new DatabaseCommands())
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var settings = (from p in context.Settings
                        where p.DateStamp == date
                        select p).FirstOrDefault();

                    if (settings?.CreateDXF != null)
                        if (settings.CreateDXF == true)
                            return true;
                    return false;
                }
            }
        }

        /// <summary>
        ///     Determines whether [is skip DXF by date] [the specified date].
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns><c>true</c> if [is skip DXF by date] [the specified date]; otherwise, <c>false</c>.</returns>
        public bool IsSkipDXFByDate(DateTime date)
        {
            using (var commands = new DatabaseCommands())
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var tasks = (from p in context.Settings
                        where p.DateStamp == date
                        select p).FirstOrDefault();

                    if (tasks?.SkipSDxf != null)
                        if (tasks.SkipSDxf == true)
                            return true;
                    return false;
                }
            }
        }

        /// <summary>
        ///     Gets the settings course number by date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>System.String.</returns>
        public string GetSettingsCourseNumByDate(DateTime date)
        {
            using (var commands = new DatabaseCommands())
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var tasks = (from p in context.Settings
                        where p.DateStamp.Equals(date)
                        select p).FirstOrDefault();
                    return tasks.CourseCode;
                }
            }
        }


        /// <summary>
        ///     Loadands the process polys.
        /// </summary>
        /// <returns>IList&lt;DrawingStack&gt;.</returns>
        public IList<DrawingStack> LoadandProcessPolys()
        {
            var dwglist = new List<DrawingStack>();
            try
            {
                using (var commands = new DatabaseCommands())
                {
                    #region Get Dwgs to Process

                    using (var context = GetDataBasePath.GetSql4Connection())
                    {
                        #region Get Drawings

                        var tasks = (from p in context.TaskManager
                            where (p.Paused == true) &&
                                  (p.Started == true) &&
                                  (p.Completed == false) &&
                                  (p.DateStarted != null) &&
                                  (p.DateCompleted == null) &&
                                   p.Cancelled == false
                            orderby p.Id ascending
                            select p).FirstOrDefault();

                        if (tasks != null)
                        {
                            var dwgs = from p in context.DrawingStack
                                where p.DateStamp.Equals(tasks.DateStamp)
                                      && (p.IsCompleted == null)
                                      && (p.IsInvoked == null)
                                      && (p.IsRunning == null)
                                select p;

                            dwglist = dwgs.ToList();
                        }
                        else
                        {
                            throw new ArgumentNullException("Tasks", "LoadandPocessPolys");
                        }

                        #endregion
                        if (dwglist.Count != 0)
                        #region Pause TaskManager Task

                        tasks.Paused = true;


                        #endregion
                        else
                        {
                            tasks.Cancelled = true;
                        }
                        context.SubmitChanges();

                        #region Single Task is Deprecated

                        //var tasks = (from p in context.TaskManager
                        //    where p.Started == true &&
                        //          p.Completed == false
                        //    select p);

                        //if (tasks != null)
                        //{
                        //    var dwgs = (from p in context.DrawingStack
                        //        where p.DateStamp.Equals(tasks.DateStamp)
                        //        select p);
                        //    if (dwgs != null)
                        //        return dwgs.ToList();
                        //}  

                        #endregion
                    }

                    #endregion
                }
                return dwglist;
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, null);
            }

            return null;
        }

        /// <summary>
        ///     Formats the commands.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="function">The function.</param>
        /// <param name="dwgid">The dwgid.</param>
        public void FormatCommands(string command, string function, string dwgid)
        {
            AddCommandToDatabase(command + " " + function, dwgid);
        }

        /// <summary>
        ///     Adds the command to database.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="dwgid">The dwgid.</param>
        public void AddCommandToDatabase(string log, string dwgid)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var logs = new DrawingStack
                {
                    PolylineDwgName = log,
                    SourcePolylineDwgID = Convert.ToInt16(dwgid)
                };


                context.DrawingStack.InsertOnSubmit(logs);
                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Adds the drawing stack to database.
        /// </summary>
        /// <param name="dwg">The DWG.</param>
        /// <param name="func">The function.</param>
        /// <param name="error">if set to <c>true</c> [error].</param>
        /// <param name="completed">if set to <c>true</c> [completed].</param>
        /// <param name="running">if set to <c>true</c> [running].</param>
        /// <param name="started">if set to <c>true</c> [started].</param>
        /// <param name="dwgid">The dwgid.</param>
        /// <param name="starttime">The starttime.</param>
        /// <param name="completedTime">The completed time.</param>
        public void AddDrawingStackToDatabase(string dwg, string func, bool error, bool completed, bool running,
            bool started, string dwgid, DateTime starttime, DateTime completedTime)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var logs = new DrawingStack
                {
                    PolylineDwgName = dwg,
                    Function = func,
                    HasError = error,
                    IsCompleted = completed,
                    IsInvoked = started,
                    IsRunning = running,
                    TimeStarted = starttime,
                    TimeCompleted = completedTime,
                    SourcePolylineDwgID = Convert.ToInt16(dwgid)
                };


                context.DrawingStack.InsertOnSubmit(logs);
                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Adds the drawing stack to database.
        /// </summary>
        /// <param name="datestamp">The datestamp.</param>
        /// <param name="dwg">The DWG.</param>
        /// <param name="func">The function.</param>
        /// <param name="error">if set to <c>true</c> [error].</param>
        /// <param name="completed">if set to <c>true</c> [completed].</param>
        /// <param name="running">if set to <c>true</c> [running].</param>
        /// <param name="started">if set to <c>true</c> [started].</param>
        /// <param name="dwgid">The dwgid.</param>
        /// <param name="starttime">The starttime.</param>
        /// <param name="completedTime">The completed time.</param>
        public void AddDrawingStackToDatabase(DateTime datestamp, string dwg, string func, bool error, bool completed,
            bool running,
            bool started, string dwgid, DateTime starttime, DateTime completedTime)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var logs = new DrawingStack
                {
                    PolylineDwgName = dwg,
                    DateStamp = datestamp,
                    Function = func,
                    HasError = error,
                    IsCompleted = completed,
                    IsInvoked = started,
                    IsRunning = running,
                    TimeStarted = starttime,
                    TimeCompleted = completedTime,
                    SourcePolylineDwgID = Convert.ToInt16(dwgid)
                };


                context.DrawingStack.InsertOnSubmit(logs);
                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Adds the drawing stack is started flag.
        /// </summary>
        /// <param name="datestamp">The datestamp.</param>
        /// <param name="name">The name.</param>
        public void AddDrawingStackIsStartedFlag(DateTime datestamp, string name)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var result = (from p in context.DrawingStack
                    where (p.DateStamp == datestamp) &&
                          (p.PointCloudDwgName == name)
                    select p).FirstOrDefault();

                result.DateStamp = datestamp;
                result.IsInvoked = true;
                result.IsRunning = true;
                result.TimeStarted = DateTime.Now;

                context.DrawingStack.InsertOnSubmit(result);
                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Adds the drawing stack is completed flag.
        /// </summary>
        /// <param name="datestamp">The datestamp.</param>
        /// <param name="name">The name.</param>
        public void AddDrawingStackIsCompletedFlag(DateTime datestamp, string name)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var result = (from p in context.DrawingStack
                    where (p.DateStamp == datestamp) &&
                          (p.PointCloudDwgName == name)
                    select p).FirstOrDefault();

                result.DateStamp = datestamp;
                result.IsInvoked = true;
                result.IsRunning = true;
                result.IsCompleted = true;
                result.TimeCompleted = DateTime.Now;

                context.DrawingStack.InsertOnSubmit(result);
                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Gets the drawing stack not running flag.
        /// </summary>
        /// <param name="datestamp">The datestamp.</param>
        /// <param name="name">The name.</param>
        /// <returns>IList&lt;DrawingStack&gt;.</returns>
        public IList<DrawingStack> GetDrawingStackNotRunningFlag(DateTime datestamp, string name)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var result = from p in context.DrawingStack
                    where (p.DateStamp == datestamp) &&
                          (p.PointCloudDwgName == name) &&
                          (p.IsRunning == false) &&
                          (p.IsCompleted == false)
                    select p;

                if (result.FirstOrDefault() != null)
                    return result.ToList();
                return null;
            }
        }

        public IList<CourseDetails> GetCourseDetails()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var result = from p in context.CourseDetails
                        select p;


                    if (result.FirstOrDefault() != null)
                        return result.ToList().OrderBy(p => p.Name).ToList();
                    return null;
                }
            }
            catch (Exception)
            {
                DatabaseLogs.FormatLogs("Failed to get Course Details!");
            }
            return null;
        }

        /// <summary>
        ///     Adds the DWG to drawing stack.
        /// </summary>
        /// <param name="dwgName">Name of the DWG.</param>
        /// <param name="dwgid">The dwgid.</param>
        public void AddDWGToDrawingStack(string dwgName, string dwgid)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var dwg = new DrawingStack
                {
                    PolylineDwgName = dwgName,
                    SourcePolylineDwgID = Convert.ToInt16(dwgid)
                };


                context.DrawingStack.InsertOnSubmit(dwg);
                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Adds the dw gs to drawing stack.
        /// </summary>
        /// <param name="dwgs">The DWGS.</param>
        public void AddDwGsToDrawingStack(List<PolylineDWGS> dwgs)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                foreach (var dwg in dwgs)
                {
                    var drawingStackdwg = new DrawingStack
                    {
                        PolylineDwgName = dwg.DrawingName,
                        SourcePolylineDwgID = Convert.ToInt16(dwg.Id)
                    };


                    context.DrawingStack.InsertOnSubmit(drawingStackdwg);
                }

                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Gets the polyline DWG by DWG identifier.
        /// </summary>
        /// <param name="dwgid">The dwgid.</param>
        /// <returns>PolylineDWGS.</returns>
        public PolylineDWGS GetPolylineDwgByDwgId(int dwgid)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var dwg = from p in context.PolylineDWGS
                    where p.Id == dwgid
                    select p;

                return dwg as PolylineDWGS;
            }
        }

        /// <summary>
        ///     Gets the polyline DWG by date stamp.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>List&lt;PolylineDWGS&gt;.</returns>
        public List<PolylineDWGS> GetPolylineDwgByDateStamp(DateTime date)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var dwg = from p in context.PolylineDWGS
                    where p.DateStamp == date
                    select p;

                return new List<PolylineDWGS> {dwg as PolylineDWGS};
            }
        }

        /// <summary>
        ///     Gets the polyline DWG date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>List&lt;PolylineDWGS&gt;.</returns>
        public List<PolylineDWGS> GetPolylineDwgDate(DateTime date)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var dwg = from p in context.PolylineDWGS
                    where p.DateStamp.Equals(date)
                    select p;

                return dwg.ToList();
            }
        }

        /// <summary>
        ///     Gets the drawing stack DWG by DWG identifier.
        /// </summary>
        /// <param name="dwgid">The dwgid.</param>
        /// <returns>DrawingStack.</returns>
        public DrawingStack GetDrawingStackDwgByDwgID(int dwgid)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var dwg = from p in context.DrawingStack
                    where p.Id == dwgid
                    select p;

                return dwg as DrawingStack;
            }
        }

        /// <summary>
        ///     Gets all drawing stack DWG.
        /// </summary>
        /// <returns>List&lt;DrawingStack&gt;.</returns>
        public List<DrawingStack> GetAllDrawingStackDwg()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var dwg = from p in context.DrawingStack
                    select p;

                return dwg.ToList();
            }
        }


        /// <summary>
        ///     Determines whether [is drawing stack started flag set] [the specified DWG stack].
        /// </summary>
        /// <param name="dwgStack">The DWG stack.</param>
        /// <returns><c>true</c> if [is drawing stack started flag set] [the specified DWG stack]; otherwise, <c>false</c>.</returns>
        public bool IsDrawingStackStartedFlagSet(DrawingStack dwgStack)
        {
            try
            {
                if (dwgStack.IsInvoked == true)
                {
                    DatabaseLogs.AddLogs("Failed Flag Test: IsDrawingStackStartedFlagSet",
                        dwgStack.PolylineDwgName);
                    return true;
                }
            }
            catch (Exception)
            {
                DatabaseLogs.AddLogs("Failed to test start flag!", dwgStack.PolylineDwgName);
            }
            return false;
        }

        /// <summary>
        ///     Sets the drawing stack started flag.
        /// </summary>
        /// <param name="dwgStack">The DWG stack.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool SetDrawingStackStartedFlag(DrawingStack dwgStack)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var dwg = (from p in context.DrawingStack
                        where p.Id == dwgStack.Id
                        select p).FirstOrDefault();

                    dwg.IsInvoked = true;
                    dwg.TimeStarted = DateTime.Now;
                    context.SubmitChanges();

                    return true;
                }
            }
            catch (Exception)
            {
                DatabaseLogs.AddLogs("Failed to set start flag!", dwgStack.PolylineDwgName);
            }
            return false;
        }

        /// <summary>
        ///     Sets the drawing stack running flag.
        /// </summary>
        /// <param name="dwgStack">The DWG stack.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool SetDrawingStackRunningFlag(DrawingStack dwgStack)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var dwg = (from p in context.DrawingStack
                        where p.Id == dwgStack.Id
                        select p).FirstOrDefault();

                    dwg.IsRunning = true;

                    context.SubmitChanges();

                    return true;
                }
            }
            catch (Exception)
            {
                DatabaseLogs.AddLogs("Failed to set DWG Running Flag!", dwgStack.PolylineDwgName);
            }
            return false;
        }

        /// <summary>
        ///     Sets the drawing stack completed flag.
        /// </summary>
        /// <param name="dwgStack">The DWG stack.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool SetDrawingStackCompletedFlag(DrawingStack dwgStack)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var dwg = (from p in context.DrawingStack
                        where p.Id == dwgStack.Id
                        select p).FirstOrDefault();

                    dwg.IsCompleted = true;
                    dwg.TimeCompleted = DateTime.Now;
                    context.SubmitChanges();

                    return true;
                }
            }
            catch (Exception)
            {
                DatabaseLogs.AddLogs("Failed to set DWG Completed Flag!", dwgStack.PolylineDwgName);
            }
            return false;
        }

        /// <summary>
        ///     Gets the date time from polyline drawing identifier.
        /// </summary>
        /// <param name="dwgid">The dwgid.</param>
        /// <returns>System.Nullable&lt;DateTime&gt;.</returns>
        public DateTime? GetDateTimeFromPolylineDrawingId(int dwgid)
        {
            //DrawingStack --> PolylinedWGS
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var datetime = (
                        from p in context.DrawingStack
                        where p.SourcePolylineDwgID == dwgid
                        select new
                        {
                            p.SourcePolylineDwgID,
                            DWGDateTime = (from y in context.PolylineDWGS
                                where p.SourcePolylineDwgID == y.Id
                                select y.DateStamp).FirstOrDefault()
                        }).Distinct()
                    .OrderByDescending(subq => new {ID = subq.DWGDateTime}
                    );

                return Convert.ToDateTime(datetime);
            }
        }

        /// <summary>
        ///     Arranges the polyline DWGS DWG stack by number.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>IList&lt;PolylineDWGS&gt;.</returns>
        public IList<PolylineDWGS> ArrangePolylineDWGSDwgStackByNumber(DateTime dateTime)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var dwgs = from p in context.PolylineDWGS
                    where p.DateStamp.Equals(dateTime)
                    orderby p descending
                    select p;

                return dwgs.ToList();
            }
        }

        /// <summary>
        ///     Arranges the polyline DWGS DWG stack by number.
        /// </summary>
        /// <param name="ticks">The ticks.</param>
        /// <returns>IList&lt;DrawingStack&gt;.</returns>
        public IList<DrawingStack> ArrangePolylineDWGSDwgStackByNumber(long ticks)
        {
            IList<DrawingStack> drawingStacks = new List<DrawingStack>();

            DateTime dateTime;
            dateTime = DateConverts.ConvertTicksToDateTime(ticks);

            using (var context = GetDataBasePath.GetSql4Connection())
            {
                DrawingStack stack;

                var suffix = string.Empty;
                var dwgs = from p in context.PolylineDWGS
                    where p.DateStamp.Equals(dateTime)
                    orderby p.Id ascending
                    select p;

                foreach (var dwg in dwgs)
                {
                    if (dwg.DrawingName.Length >= 6)
                    {
                        stack = new DrawingStack();
                        suffix = dwg.DrawingName.Substring(dwg.DrawingName.Length - 6, 2);
                        stack.SourcePolylineDwgID = dwg.Id;
                        stack.PolylineDwgName = dwg.DrawingName;
                        stack.Hole = Convert.ToInt32(suffix);
                        drawingStacks.Add(stack);
                    }
                }


                return drawingStacks.ToList();
            }
        }

        /// <summary>
        ///     Arranges the point cloud DWGS to DWG stack.
        /// </summary>
        /// <param name="ticks">The ticks.</param>
        /// <param name="inStacks">The in stacks.</param>
        /// <returns>IList&lt;DrawingStack&gt;.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public IList<DrawingStack> ArrangePointCloudDwgsToDwgStack(long ticks, IList<DrawingStack> inStacks)
        {
            if (inStacks == null) throw new ArgumentNullException(nameof(inStacks));
            IList<DrawingStack> drawingStacks = new List<DrawingStack>();

            DateTime dateTime;
            dateTime = DateConverts.ConvertTicksToDateTime(ticks);

            using (var context = GetDataBasePath.GetSql4Connection())
            {
                DrawingStack stack = null;

                var suffix = string.Empty;
                var dwgs = from p in context.PointCloud
                    where p.DateStamp.Equals(dateTime)
                    orderby p.Id ascending
                    select p;

                foreach (var pclouds in dwgs)
                {
                    if (pclouds.DrawingName.Length >= 6)
                    {
                        foreach (var item in inStacks)
                        {
                            stack = new DrawingStack();
                            suffix = pclouds.DrawingName.Substring(pclouds.DrawingName.Length - 6, 2);

                            if (NumberConverts.IsNumeric(suffix))
                            {
                                if (item.Hole == Convert.ToInt32(suffix))
                                {
                                    stack.Function = pclouds.DateStamp.ToString();
                                    stack.SourcePolylineDwgID = pclouds.Id;
                                    stack.PointCloudDwgName = pclouds.DrawingName;
                                    stack.Hole = Convert.ToInt32(suffix);
                                    stack.PolylineDwgName = item.PolylineDwgName;
                                    drawingStacks.Add(stack);
                                    context.DrawingStack.InsertOnSubmit(stack);
                                }
                            }
                        }
                    }
                }
                context.SubmitChanges();


                return drawingStacks.ToList();
            }
        }


        /// <summary>
        ///     Gets the current DWG to process.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetCurrentDwgToProcess()
        {
            var path = GetDataBasePath.GetAppPath();

            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var activeDrawing =
                    from p in context.PolylineDWGS
                    select p;

                if (activeDrawing.Count().Equals(0))
                    return string.Empty;

                var found = false;
                foreach (var dwg in activeDrawing)
                {
                    if (!dwg.DateLoaded.HasValue)
                        found = true;

                    if (found)
                    {
                        dwg.DateLoaded = DateTime.Now;
                        context.SubmitChanges();

                        return Path.Combine(dwg.SourcePath, dwg.DrawingName);
                    }
                }

                return string.Empty;
            }
        }


        /// <summary>
        ///     Checks the time stamp.
        /// </summary>
        /// <param name="txtDateCreated">The text date created.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public bool CheckTimeStamp(string txtDateCreated)
        {
            if (txtDateCreated == null) throw new ArgumentNullException(nameof(txtDateCreated));
            if (string.IsNullOrEmpty(txtDateCreated)) throw new ArgumentNullException(nameof(txtDateCreated));
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var settings =
                    from p in context.Settings
                    where p.DateStamp == Convert.ToDateTime(txtDateCreated)
                    select p;

                var firstOrDefault = settings.FirstOrDefault();
                if (firstOrDefault != null) return true; //has duplicate
            }
            return false;
        }


        /// <summary>
        ///     Inserts the settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool InsertSettings(Settings settings)
        {
            return true;
        }

        /// <summary>
        ///     Gets all tasks.
        /// </summary>
        /// <returns>IList&lt;Tasks&gt;.</returns>
        public IList<Tasks> GetAllTasks()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var tasks =
                    from p in context.Tasks
                    select p;


                return tasks.ToList();
            }
        }

        /// <summary>
        ///     Gets the task by date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>Tasks.</returns>
        public Tasks GetTaskByDate(DateTime date)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var tasks =
                    from p in context.Tasks
                    where p.DateStamp == date
                    select p;


                return tasks.FirstOrDefault();
            }
        }

        /// <summary>
        ///     Updates the task start date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>Tasks.</returns>
        /// <exception cref="System.ArgumentNullException">Tasks</exception>
        public Tasks UpdateTaskStartDate(DateTime date)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var tasks =
                    (from p in context.Tasks
                        where p.DateStamp == date
                        select p).FirstOrDefault();

                    if (tasks != null)
                    {
                        tasks.TimeStarted = DateTime.Now;
                        context.SubmitChanges();
                        return tasks;
                    }

                    throw new ArgumentNullException("Tasks");
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "UpdateTaskStartDate Failed");
            }
            return null;
        }

        /// <summary>
        ///     Clears the task start date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>Tasks.</returns>
        /// <exception cref="System.ArgumentNullException">Tasks</exception>
        public Tasks ClearTaskStartDate(DateTime date)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var tasks =
                    (from p in context.Tasks
                        where p.DateStamp == date
                        select p).FirstOrDefault();

                    if (tasks != null)
                    {
                        tasks.TimeStarted = null;
                        context.SubmitChanges();
                        return tasks;
                    }

                    throw new ArgumentNullException("Tasks");
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "UpdateTaskStartDate Failed");
            }
            return null;
        }

        /// <summary>
        ///     Updates the task complete date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>Tasks.</returns>
        public Tasks UpdateTaskCompleteDate(DateTime date)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var tasks =
                (from p in context.Tasks
                    where p.DateStamp == date
                    select p).FirstOrDefault();


                tasks.TimeEnded = DateTime.Now;
                context.SubmitChanges();
                return tasks;
            }
        }

        /// <summary>
        ///     Updates the task manager complete date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>TaskManager.</returns>
        public TaskManager UpdateTaskManagerCompleteDate(DateTime date)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var tasks =
                (from p in context.TaskManager
                    where p.DateStamp == date
                    select p).FirstOrDefault();


                tasks.DateCompleted = DateTime.Now;
                tasks.Completed = true;
                context.SubmitChanges();
                return tasks;
            }
        }

        /// <summary>
        ///     Gets the name of the tasks project.
        /// </summary>
        /// <returns>IList&lt;TaskManager&gt;.</returns>
        public IList<TaskManager> GetTasksProjectName()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var tasks =
                    from settings in context.Settings
                    from p in context.Tasks
                    where (settings.Id == p.ProjectID) &&
                          !p.TimeEnded.HasValue &&
                          p.TimeStarted.HasValue &&
                          (p.IsPaused != true)
                    select new
                    {
                        Date = settings.DateStamp,
                        Course = settings.CourseName,
                        Created = settings.ProjectCreator,
                        Started = p.TimeStarted,
                        Completed = p.TimeEnded,
                        Paused = p.IsPaused,
                        Cancelled = p.IsCancelled
                    };

                if (tasks.FirstOrDefault() == null)
                    return null;

                IList<TaskManager> myItems = new List<TaskManager>();

                foreach (var source in tasks.ToList())
                {
                    var taskWithProject = new TaskManager();
                    if (source.Cancelled.HasValue) taskWithProject.Cancelled = source.Cancelled.Value;
                    if (source.Paused.HasValue) taskWithProject.Paused = source.Paused.Value;
                    if (source.Completed.HasValue)
                        taskWithProject.DateCompleted = source.Completed.HasValue
                            ? source.Completed.Value
                            : (DateTime?) null;
                    if (source.Started.HasValue)
                        taskWithProject.DateStarted = source.Started.HasValue ? source.Started.Value : (DateTime?) null;
                    if (!string.IsNullOrEmpty(source.Course)) taskWithProject.CourseName = source.Course;
                    if (source.Cancelled.HasValue) taskWithProject.Cancelled = source.Cancelled.Value;
                    if (source.Cancelled.HasValue) taskWithProject.Cancelled = source.Cancelled.Value;
                    if (!string.IsNullOrEmpty(source.Created)) taskWithProject.CreatedBy = source.Created;

                    taskWithProject.DateStamp = (DateTime) (source.Date.HasValue ? source.Date.Value : (DateTime?) null);

                    myItems.Add(taskWithProject);
                }

                return myItems;
            }
        }

        /// <summary>
        ///     Loads the data.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="Pointfiles">The pointfiles.</param>
        /// <param name="PolyLinefiles">The poly linefiles.</param>
        /// <param name="destFolderPath">The dest folder path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public bool LoadData(DateTime dateTime, FileInfo[] Pointfiles, FileInfo[] PolyLinefiles, string destFolderPath)
        {
            if (Pointfiles == null) throw new ArgumentNullException(nameof(Pointfiles));
            var dateCreated = dateTime;

            using (var context = GetDataBasePath.GetSql4Connection())
            {
                //Todo Add Task this when we condense
                // var task = new Tasks();


                foreach (var file in PolyLinefiles)
                {
                    var lines = new PolylineDWGS();
                    lines.DateStamp = dateCreated;
                    lines.DateLoaded = dateCreated;
                    lines.DrawingName = file.Name;
                    lines.SourcePath = file.DirectoryName;
                    lines.DestinationPath = destFolderPath;
                    context.PolylineDWGS.InsertOnSubmit(lines);
                }
                context.SubmitChanges();

                foreach (var file in Pointfiles)
                {
                    var points = new PointCloud();
                    points.DateStamp = dateCreated;
                    points.DateLoaded = dateCreated;
                    points.DrawingName = file.Name;
                    points.SourcePath = file.DirectoryName;
                    points.DestinationPath = destFolderPath;
                    context.PointCloud.InsertOnSubmit(points);
                }

                //task.TimeStarted = dateCreated;
                //task.IsCancelled = false;
                //task.IsPaused = true;

                //context.Tasks.InsertOnSubmit(task);

                // submit changes
                context.SubmitChanges();
            }

            return true;
        }


        /// <summary>
        ///     News the task.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="mySettings">My settings.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool NewTask(DateTime dateTime, Settings mySettings)
        {
            if (CheckTimeStamp(dateTime.ToString(CultureInfo.InvariantCulture)))
                return false;

            using (var context = GetDataBasePath.GetSql4Connection())
            {
                if (mySettings != null)
                    context.Settings.InsertOnSubmit(mySettings);
                context.SubmitChanges();

                //get settings id
                var settings = from p in context.Settings
                    where p.DateStamp == Convert.ToDateTime(dateTime)
                    select p;

                var firstOrDefault = settings.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var settingId = firstOrDefault.Id;
                    int setId = Convert.ToInt16(settingId);

                    //create the task
                    var tasks = new Tasks();
                    tasks.ProjectID = setId;
                    tasks.DateStamp = dateTime;
                    context.Tasks.InsertOnSubmit(tasks);
                }
                context.SubmitChanges();
                return true;
            }
        }

        /// <summary>
        ///     Tasks the manager validate date.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TaskManagerValidateDate(DateTime dateTime)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                //get settings id
                var settings = (from p in context.TaskManager
                    where p.DateStamp == Convert.ToDateTime(dateTime)
                    select p).FirstOrDefault();

                var HasData = settings != null;
                return HasData;
            }
        }

        /// <summary>
        ///     News the task manager.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool NewTaskManager(DateTime dateTime)
        {
            if (TaskManagerValidateDate(dateTime))
                return false;

            using (var context = GetDataBasePath.GetSql4Connection())
            {
                //get settings id
                var settings = (from p in context.Settings
                    where p.DateStamp == Convert.ToDateTime(dateTime)
                    select p).FirstOrDefault();

                if (settings != null)
                {
                    //create the taskmanager
                    var taskManager = new TaskManager();
                    taskManager.DateStamp = dateTime;
                    taskManager.CourseName = settings.CourseName;
                    taskManager.CreatedBy = settings.ProjectCreator;
                    taskManager.DateCreated = DateTime.Now;
                    taskManager.DateStarted = DateTime.Now;
                    taskManager.Paused = true; //changed 5.15.16
                    taskManager.Cancelled = false;
                    taskManager.Started = true;
                    taskManager.Completed = false;
                    taskManager.ProcessPolylines = false;
                    taskManager.ChangeLayers = false;
                    taskManager.LoadTINSurfaces = false;
                    taskManager.GenerateSurfaces = false;

                    context.TaskManager.InsertOnSubmit(taskManager);
                }
                context.SubmitChanges();
                return true;
            }
        }

        // Protected implementation of Dispose pattern.
        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        /// <summary>
        ///     Databases the pga context connection.
        /// </summary>
        /// <returns>PGAContext.</returns>
        public PGAContext DbPgaContextConnection()
        {
            return GetDataBasePath.GetSql4Connection();
        }

        /// <summary>
        ///     Inserts the new polyline geometry.
        /// </summary>
        /// <param name="dwg">The DWG.</param>
        /// <param name="time">The time.</param>
        /// <param name="boundary">The boundary.</param>
        /// <param name="points">The points.</param>
        public void InsertNewPolylineGeometry(DrawingStack dwg, DateTime time, byte[] boundary, byte[] points)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var polyline = new Polyline();
                polyline.DateStamp = time;
                polyline.Geometry = boundary;
                polyline.LiDARPoints = points;
                polyline.DrawingName = dwg.PolylineDwgName;
                polyline.Hole = dwg.Hole.ToString();
                context.Polyline.InsertOnSubmit(polyline);
                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Clears the log file from database.
        /// </summary>
        public void ClearLogFileFromDB()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var logs = from log in context.Logs
                    orderby log.Id descending
                    select log;

                if (logs.FirstOrDefault() != null)
                {
                    context.Logs.DeleteAllOnSubmit(logs);
                    context.SubmitChanges();
                }
            }
        }

        /// <summary>
        ///     Gets the last x numberof logs.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>Logs[].</returns>
        public Logs[] GetLastXNumberofLogs(int? number)
        {
            var num = 0;

            if (number == null) num = 10;
            else if ((num = Convert.ToInt32(number)) == 0)
            {
                return null;
            }
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var logs = (from log in context.Logs
                    orderby log.Id descending
                    select log).Take(num);

                if (logs.FirstOrDefault() != null)
                {
                    return logs.ToArray();
                }
            }
            return new Logs[] {};
        }

        /// <summary>
        ///     Gets the last all logs.
        /// </summary>
        /// <returns>Logs[].</returns>
        public Logs[] GetLastAllLogs()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var logs = from log in context.Logs
                    orderby log.Id descending
                    select log;

                if (logs.FirstOrDefault() != null)
                {
                    return logs.ToArray();
                }
            }
            return new Logs[] {};
        }

        /// <summary>
        ///     Gets the last x numberof logs.
        /// </summary>
        /// <param name="lower">The lower.</param>
        /// <param name="upper">The upper.</param>
        /// <returns>Logs[].</returns>
        public Logs[] GetLastXNumberofLogs(int lower, int upper)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var logs = (from log in context.Logs
                    orderby log.Id descending
                    select log).ToArray();

                if (logs.FirstOrDefault() != null)
                {
                    upper = logs.Count() - 1;

                    var subset = (from log in context.Logs
                        orderby log.Id descending
                        select log).Take(10);

                    if (subset.FirstOrDefault() != null)
                    {
                        return subset.ToArray();
                    }
                }
            }
            return new Logs[] {};
        }

        /// <summary>
        ///     Adds the export to cad record.
        /// </summary>
        /// <param name="dwg">The DWG.</param>
        /// <param name="completepath">The completepath.</param>
        /// <param name="toInt64">To int64.</param>
        public void AddExportToCADRecord(DrawingStack dwg, string completepath, long toInt64)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var exportToCad = new ExportToCADStack();

                exportToCad.DateStamp = dwg.DateStamp;
                exportToCad.CompletePath = completepath;
                exportToCad.DwgName = dwg.PolylineDwgName;
                exportToCad.Hole = Convert.ToInt64(dwg.Hole);
                context.ExportToCADStack.
                    InsertOnSubmit(exportToCad);

                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Adds the export to cad record.
        /// </summary>
        /// <param name="completepath">The completepath.</param>
        /// <param name="date">The date.</param>
        public void AddExportToCADRecord(string completepath, DateTime date)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var hole = lOrder(completepath);
                    var filename = Path.GetFileName(completepath);

                    var exportToCad = new ExportToCADStack();

                    exportToCad.DateStamp = date;
                    exportToCad.CompletePath = completepath;
                    exportToCad.DwgName = filename;
                    exportToCad.Hole = Convert.ToInt64(hole);
                    context.ExportToCADStack.
                        InsertOnSubmit(exportToCad);

                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, completepath);
            }
        }

        /// <summary>
        ///     Determines whether [is started export to cad] [the specified identifier].
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if [is started export to cad] [the specified identifier]; otherwise, <c>false</c>.</returns>
        public bool IsStartedExportToCad(int id)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var records = (from p in context.ExportToCADStack
                    where p.Id == id
                    select p).FirstOrDefault();
                if (
                    (records.IsInvoked == true) &&
                    (records.IsRunning == true))
                    return true;
            }
            return false;
        }

        /// <summary>
        ///     Determines whether [is completed export to cad] [the specified identifier].
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if [is completed export to cad] [the specified identifier]; otherwise, <c>false</c>.</returns>
        public bool IsCompletedExportToCad(int id)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var records = (from p in context.ExportToCADStack
                    where p.Id == id
                    select p).FirstOrDefault();
                if (
                    (records.IsInvoked == true) &&
                    (records.IsRunning == true) &&
                    (records.IsCompleted == true))
                    return true;
            }
            return false;
        }

        /// <summary>
        ///     Determines whether [is all completed eport to cad].
        /// </summary>
        /// <returns><c>true</c> if [is all completed eport to cad]; otherwise, <c>false</c>.</returns>
        public bool IsAllCompletedEportToCad()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var TotalCount = (from p in context.ExportToCADStack
                    select p).Count();

                var records = from p in context.ExportToCADStack
                    where (p.IsInvoked == true) &&
                          (p.IsRunning == true) &&
                          (p.IsCompleted == true) &&
                          (p.Function == "DONE")
                    select p;

                if (records.Any() &&
                    (TotalCount == records.Count()))
                    return true;
                return false;
            }
        }

        /// <summary>
        ///     Determines whether [is just started export to cad].
        /// </summary>
        /// <returns><c>true</c> if [is just started export to cad]; otherwise, <c>false</c>.</returns>
        public bool IsJustStartedExportToCad()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var TotalCount = (from p in context.ExportToCADStack
                    select p).Count();

                var records = from p in context.ExportToCADStack
                    where (p.IsInvoked == false) &&
                          (p.IsRunning == false) &&
                          (p.IsCompleted == false) &&
                          (p.Function == "")
                    select p;

                if (records.Any() &&
                    (TotalCount == records.Count()))
                    return true;
                return false;
            }
        }

        /// <summary>
        ///     Determines whether [is all DXF completed eport to cad].
        /// </summary>
        /// <returns><c>true</c> if [is all DXF completed eport to cad]; otherwise, <c>false</c>.</returns>
        public bool IsAllDXFCompletedEportToCad()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var TotalCount = (from p in context.ExportToCADStack
                    select p).Count();

                var records = from p in context.ExportToCADStack
                    where (p.IsInvoked == true) &&
                          (p.IsRunning == true) &&
                          (p.IsCompleted == true) &&
                          (p.Function == "DXF")
                    select p;

                if (records.Any() &&
                    (TotalCount == records.Count()))
                    return true;
                return false;
            }
        }

        /// <summary>
        ///     Determines whether [is all DWG completed eport to cad].
        /// </summary>
        /// <returns><c>true</c> if [is all DWG completed eport to cad]; otherwise, <c>false</c>.</returns>
        public bool IsAllDWGCompletedEportToCad()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var TotalCount = (from p in context.ExportToCADStack
                    select p).Count();

                var records = from p in context.ExportToCADStack
                    where (p.IsInvoked == true) &&
                          (p.IsRunning == true) &&
                          (p.IsCompleted == false) &&
                          (p.Function == "DWG")
                    select p;

                if (records.Any() &&
                    (TotalCount == records.Count()))
                    return true;
                return false;
            }
        }

        /// <summary>
        ///     Determines whether [is DXF started eport to cad].
        /// </summary>
        /// <returns><c>true</c> if [is DXF started eport to cad]; otherwise, <c>false</c>.</returns>
        public bool IsDXFStartedEportToCad()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var records = from p in context.ExportToCADStack
                    where (p.IsInvoked == true) &&
                          (p.IsRunning == true) &&
                          (p.IsCompleted == false) &&
                          (p.Function == "DXF")
                    select p;

                if (records.Any())
                    return true;
                return false;
            }
        }

        /// <summary>
        ///     Determines whether [is DWG started eport to cad].
        /// </summary>
        /// <returns><c>true</c> if [is DWG started eport to cad]; otherwise, <c>false</c>.</returns>
        public bool IsDWGStartedEportToCad()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var records = from p in context.ExportToCADStack
                    where (p.IsInvoked == true) &&
                          (p.IsRunning == true) &&
                          (p.IsCompleted == false) &&
                          (p.Function == "DWG")
                    select p;

                if (records.Any())
                    return true;
                return false;
            }
        }

        /// <summary>
        ///     Determines whether [is DWG started eprt to cad] [the specified identifier].
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if [is DWG started eprt to cad] [the specified identifier]; otherwise, <c>false</c>.</returns>
        public bool IsDWGStartedEprtToCad(int id)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var records = (from p in context.ExportToCADStack
                    where p.Id == id
                    select p).FirstOrDefault();
                if (
                    (records.IsInvoked == true) &&
                    (records.IsRunning == true) &&
                    (records.IsCompleted == false) &&
                    (records.Function == "DWG"))
                    return true;
            }
            return false;
        }

        /// <summary>
        ///     Determines whether [is DXF started export to cad] [the specified identifier].
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if [is DXF started export to cad] [the specified identifier]; otherwise, <c>false</c>.</returns>
        public bool IsDXFStartedExportToCad(int id)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var records = (from p in context.ExportToCADStack
                    where p.Id == id
                    select p).FirstOrDefault();
                if (
                    (records.IsInvoked == true) &&
                    (records.IsRunning == true) &&
                    (records.IsCompleted == false) &&
                    (records.Function == "DXF"))
                    return true;
            }
            return false;
        }

        /// <summary>
        ///     Sets the start flag export to cad.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void SetStartFlagExportToCad(int id)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var records = (from p in context.ExportToCADStack
                    where p.Id == id
                    select p).FirstOrDefault();

                records.IsInvoked = true;
                records.IsRunning = true;
                records.IsCompleted = false;
                records.Function = "DWG";
                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Locks the file export to cad.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void LockFileExportToCad(int id)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var records = (from p in context.ExportToCADStack
                    where p.Id == id
                    select p).FirstOrDefault();

                records.HasError = true;

                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Uns the lock file export to cad.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void UnLockFileExportToCad(int id)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var records = (from p in context.ExportToCADStack
                    where p.Id == id
                    select p).FirstOrDefault();

                records.HasError = false;

                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Anies the files locked.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool AnyFilesLocked()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var records = from p in context.ExportToCADStack
                    where p.HasError == true
                    select p;
                if (records.Any())
                    return true;
                return false;
            }
        }

        /// <summary>
        ///     Gens the use any files locked.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool GenUseAnyFilesLocked()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var records = from p in context.GeneralCADStack
                    where p.HasError == true
                    select p;
                if (records.Any())
                    return true;
                return false;
            }
        }

        /// <summary>
        ///     Sets the DXF flag export to cad.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void SetDXFFlagExportToCad(int id)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var records = (from p in context.ExportToCADStack
                    where p.Id == id
                    select p).FirstOrDefault();

                records.IsInvoked = true;
                records.IsRunning = true;
                records.IsCompleted = false;
                records.Function = "DXF";
                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Sets the completed flag export to cad.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void SetCompletedFlagExportToCad(int id)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var records = (from p in context.ExportToCADStack
                    where p.Id == id
                    select p).FirstOrDefault();

                records.IsInvoked = true;
                records.IsRunning = true;
                records.IsCompleted = true;
                records.Function = "DONE";
                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Sets the completed flag DWG export to cad.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void SetCompletedFlagDWGExportToCad(int id)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var records = (from p in context.ExportToCADStack
                    where p.Id == id
                    select p).FirstOrDefault();

                records.IsInvoked = true;
                records.IsRunning = true;
                records.IsCompleted = false;
                records.Function = "DWG";
                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Alls the completed flags export to cad.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool AllCompletedFlagsExportToCad()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var allrec = from p in context.ExportToCADStack
                    select p;
                var matrec = from p in context.ExportToCADStack
                    where p.IsCompleted == true
                    select p;

                if (allrec.Count() == matrec.Count())
                    return true;
            }
            return false;
        }

        /// <summary>
        ///     Gus all completed flags.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool GUAllCompletedFlags()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var matrec = (from p in context.GeneralCADStack
                    where p.IsCompleted == false
                    select p).FirstOrDefault();

                if (matrec == null)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        ///     Gets all export to cad records.
        /// </summary>
        /// <returns>List&lt;ExportToCADStack&gt;.</returns>
        public List<ExportToCADStack> GetAllExportToCadRecords()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var records = from p in context.ExportToCADStack
                    select p;

                return new List<ExportToCADStack>(records);
            }
        }

        /// <summary>
        ///     Deletes all export to cad records.
        /// </summary>
        public void DeleteAllExportToCadRecords()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var records = from p in context.ExportToCADStack
                    select p;
                context.ExportToCADStack.DeleteAllOnSubmit(records);
                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Gets the log file path.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetLogFilePath()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                return GetDataBasePath.GetLogPath();
            }
        }

        /// <summary>
        ///     Gets the connection string.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetConnectionString()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                return context.Connection.ConnectionString;
            }
        }

        /// <summary>
        ///     Deletes the data from all tables.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool DeleteDataFromAllTables()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var logs = from log in context.Logs
                        orderby log.Id descending
                        select log;

                    if (logs.FirstOrDefault() != null)
                    {
                        context.Logs.DeleteAllOnSubmit(logs);
                        context.SubmitChanges();
                    }

                    var dwgStack = from log in context.DrawingStack
                        orderby log.Id descending
                        select log;

                    if (dwgStack.FirstOrDefault() != null)
                    {
                        context.DrawingStack.DeleteAllOnSubmit(dwgStack);
                        context.SubmitChanges();
                    }

                    var glStack = from log in context.GlobalSettings
                        orderby log.Id descending
                        select log;

                    if (glStack.FirstOrDefault() != null)
                    {
                        context.GlobalSettings.DeleteAllOnSubmit(glStack);
                        context.SubmitChanges();
                    }

                    var PointCloud = from log in context.PointCloud
                        orderby log.Id descending
                        select log;

                    if (PointCloud.FirstOrDefault() != null)
                    {
                        context.PointCloud.DeleteAllOnSubmit(PointCloud);
                        context.SubmitChanges();
                    }

                    var Poly = from log in context.Polyline
                        orderby log.Id descending
                        select log;

                    if (Poly.FirstOrDefault() != null)
                    {
                        context.Polyline.DeleteAllOnSubmit(Poly);
                        context.SubmitChanges();
                    }


                    var Polydwg = from log in context.PolylineDWGS
                        orderby log.Id descending
                        select log;

                    if (Polydwg.FirstOrDefault() != null)
                    {
                        context.PolylineDWGS.DeleteAllOnSubmit(Polydwg);
                        context.SubmitChanges();
                    }


                    var setting = from log in context.Settings
                        orderby log.Id descending
                        select log;

                    if (setting.FirstOrDefault() != null)
                    {
                        context.Settings.DeleteAllOnSubmit(setting);
                        context.SubmitChanges();
                    }

                    var sdwg = from log in context.SourceDrawingInfo
                        orderby log.Id descending
                        select log;

                    if (sdwg.FirstOrDefault() != null)
                    {
                        context.SourceDrawingInfo.DeleteAllOnSubmit(sdwg);
                        context.SubmitChanges();
                    }


                    var tm = from log in context.TaskManager
                        orderby log.Id descending
                        select log;

                    if (tm.FirstOrDefault() != null)
                    {
                        context.TaskManager.DeleteAllOnSubmit(tm);
                        context.SubmitChanges();
                    }

                    var task = from log in context.Tasks
                        orderby log.Id descending
                        select log;

                    if (task.FirstOrDefault() != null)
                    {
                        context.Tasks.DeleteAllOnSubmit(task);
                        context.SubmitChanges();
                    }

                    var exportstk = from log in context.ExportToCADStack
                        orderby log.Id descending
                        select log;

                    if (exportstk.FirstOrDefault() != null)
                    {
                        context.ExportToCADStack.DeleteAllOnSubmit(exportstk);
                        context.SubmitChanges();
                    }


                    var notes = from log in context.Notifications
                        orderby log.Id descending
                        select log;

                    if (notes.FirstOrDefault() != null)
                    {
                        context.Notifications.DeleteAllOnSubmit(notes);
                        context.SubmitChanges();
                    }

                    var excluded = from log in context.ExcludedFeatures
                        orderby log.Id descending
                        select log;

                    if (excluded.FirstOrDefault() != null)
                    {
                        context.ExcludedFeatures.DeleteAllOnSubmit(excluded);
                        context.SubmitChanges();
                    }

                    var update = from p in context.Updates
                        orderby p.Id descending
                        select p;

                    if (update.FirstOrDefault() != null)
                    {
                        context.Updates.DeleteAllOnSubmit(update);
                        context.SubmitChanges();
                    }

                    var guse = from p in context.GeneralCADStack
                        orderby p.Id descending
                        select p;

                    if (guse.FirstOrDefault() != null)
                    {
                        context.GeneralCADStack.DeleteAllOnSubmit(guse);
                        context.SubmitChanges();
                    }


                    var pb = from p in context.Timer
                        orderby p.Id descending
                        select p;

                    if (pb.FirstOrDefault() != null)
                    {
                        context.Timer.DeleteAllOnSubmit(pb);
                        context.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "Error Deleting Data!");
                return false;
            }

            try
            {
                ResetIdentityColumns("ExportToCADStack");
                ResetIdentityColumns("Tasks");
                ResetIdentityColumns("TaskManager");
                ResetIdentityColumns("SourceDrawingInfo");
                ResetIdentityColumns("Settings");
                ResetIdentityColumns("PolylineDWGS");
                ResetIdentityColumns("Polyline");
                ResetIdentityColumns("PointCloud");
                ResetIdentityColumns("GlobalSettings");
                ResetIdentityColumns("DrawingStack");
                ResetIdentityColumns("Logs");
                ResetIdentityColumns("Notifications");
                ResetIdentityColumns("ExcludedFeatures");
                ResetIdentityColumns("Updates");
                ResetIdentityColumns("Timer");
                ResetIdentityColumns("Smoothing");
                ResetIdentityColumns("GeneralCADStack");
                ResetIdentityColumns("CourseDetails");

            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "Error Resetting Data!");
                return false;
            }

            return true;
        }


        /// <summary>
        ///     Deletes all from gu stack.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool DeleteAllFromGUStack()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var guse = from p in context.GeneralCADStack
                        orderby p.Id descending
                        select p;

                    if (guse.FirstOrDefault() != null)
                    {
                        context.GeneralCADStack.DeleteAllOnSubmit(guse);
                        context.SubmitChanges();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "Error General Use Stack!");
            }
            return false;
        }

        /// <summary>
        ///     Deletes all from courses.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool DeleteAllFromCourses()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var guse = from p in context.CourseDetails
                        orderby p.Id descending
                        select p;

                    if (guse.FirstOrDefault() != null)
                    {
                        context.CourseDetails.DeleteAllOnSubmit(guse);
                        context.SubmitChanges();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "Error General Use Stack!");
            }
            return false;
        }

        /// <summary>
        ///     Resets the identity columns.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ResetIdentityColumns(string val)
        {
            try
            {
                var command = string.Format("ALTER TABLE {0} ALTER COLUMN Id IDENTITY (1,1)", val);

                using (
                    var lcommand = new SqlCeCommand(command,
                        GetDataBasePath.GetSql4Connection().Connection as SqlCeConnection))
                {
                    lcommand.Connection.Open();
                    lcommand.ExecuteNonQuery();
                    lcommand.Connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "Error: Reset Column Identity! " + val);
            }
            return false;
        }

        /// <summary>
        ///     Determines whether [has more dw gs export to cad].
        /// </summary>
        /// <returns><c>true</c> if [has more dw gs export to cad]; otherwise, <c>false</c>.</returns>
        public bool HasMoreDWGsExportToCAD()
        {
            //state-->"",DWG,DXF,DONE
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var TotalCount = (from p in context.ExportToCADStack
                    where p.Function == ""
                    select p).Count();

                if (TotalCount > 0)
                    return true;
                return false;
            }
        }

        /// <summary>
        ///     Determines whether [has more dx fs export to cad].
        /// </summary>
        /// <returns><c>true</c> if [has more dx fs export to cad]; otherwise, <c>false</c>.</returns>
        public bool HasMoreDXFsExportToCAD()
        {
            //state-->"",DWG,DXF,DONE
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var TotalCount = (from p in context.ExportToCADStack
                    where p.Function == "DWG"
                    select p).Count();

                if (TotalCount > 0)
                    return true;
                return false;
            }
        }

        /// <summary>
        ///     DWGs the count export to cad.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int DWGCountExportToCAD()
        {
            //state-->"",DWG,DXF,DONE
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var TotalCount = (from p in context.ExportToCADStack
                    select p).Count();

                return TotalCount;
            }
        }

        /// <summary>
        ///     DWGs the count gen cad STK.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int DWGCountGenCadStk()
        {
            //state-->"",DWG,DXF,DONE
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var TotalCount = (from p in context.GeneralCADStack
                    select p).Count();

                return TotalCount;
            }
        }

        /// <summary>
        ///     Uses the simplify.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool UseSimplify(DateTime date)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var usesimp = (from p in context.Settings
                    where p.DateStamp == date
                    select p.SimplifyPlines).FirstOrDefault();

                return (usesimp != null) && (bool) usesimp;
            }
        }

        /// <summary>
        ///     Sends the complete flag to settings.
        /// </summary>
        public void SendCompleteFlagToSettings()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var records = (from p in context.ExportToCADStack
                    select p).FirstOrDefault();

                if (records != null)
                {
                    var date = records.DateStamp;
                    var settings = (from p in context.Settings
                        where p.DateStamp == date
                        select p).FirstOrDefault();

                    var tasks = (from p in context.Tasks
                        where p.DateStamp == date
                        select p).FirstOrDefault();

                    //update setting start and finish time

                    settings.StartTime = tasks.TimeStarted;
                    settings.EndTime = DateTime.Now;
                    context.SubmitChanges();
                }
            }
        }

        /// <summary>
        ///     gs the uget next DWG.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GUgetNextDwg()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var allrecs = from p in context.GeneralCADStack
                    select p;
                var currec = (from p in context.GeneralCADStack
                    where p.IsCompleted == null
                    select p).OrderBy(p => p.Id).FirstOrDefault();

                if (currec == null)
                    return string.Empty;

                return currec.CompletePath;
            }
        }

        /// <summary>
        ///     Inserts the polyline information.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="color">The color.</param>
        /// <param name="layer">The layer.</param>
        /// <param name="area">The area.</param>
        /// <param name="length">The length.</param>
        /// <param name="s">The s.</param>
        /// <param name="nodes">The nodes.</param>
        /// <param name="oid">The oid.</param>
        /// <param name="dwgName">Name of the DWG.</param>
        public void InsertPolylineInformation(DateTime time, string color, string layer, double area, string length,
            string s, int nodes, string oid, string dwgName)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var TOLERANCE = 0.1;
                var records = (from p in context.Polyline
                    where p.DateStamp == time
                    select p).FirstOrDefault(p => Math.Abs
                                                      (Convert.ToInt32(p.Area) - area) < TOLERANCE);

                if (records == null)
                {
                    var poly = new Polyline();
                    poly.Area = area.ToString();
                    poly.DateStamp = time;
                    poly.DrawingName = dwgName;
                    poly.Handle = handle.ToString();
                    poly.Depends = oid;
                    poly.Hole = Hole(dwgName);
                    poly.Length = length;
                    poly.Nodes = nodes.ToString();

                    context.Polyline.InsertOnSubmit(poly);
                    context.SubmitChanges();
                }
            }
        }

        /// <summary>
        ///     Sets the task paused.
        /// </summary>
        /// <param name="dateSelected">The date selected.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool SetTaskPaused(DateTime dateSelected)
        {
            var settings = GetProjectID(dateSelected);
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = (from p in context.Tasks
                    where p.DateStamp == dateSelected
                    select p).FirstOrDefault();

                path.IsPaused = true;

                context.SubmitChanges();

                return true;
            }
        }

        public bool SetTaskCancelled(DateTime dateSelected)
        {
            var settings = GetProjectID(dateSelected);
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = (from p in context.Tasks
                    where p.DateStamp == dateSelected
                    select p).FirstOrDefault();

                path.IsCancelled = true;

                context.SubmitChanges();

                return true;
            }
        }

        public bool SetTaskManagerCancelled(DateTime dateSelected)
        {
            var settings = GetProjectID(dateSelected);
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = (from p in context.TaskManager
                    where p.DateStamp == dateSelected
                    select p).FirstOrDefault();

                path.Cancelled = true;

                context.SubmitChanges();

                return true;
            }
        }

        public bool SetSettingsCanceled(DateTime date)
        {
            using (var commands = new DatabaseCommands())
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var settings = (from p in context.Settings
                        where p.DateStamp.Equals(date)
                        select p).FirstOrDefault();

                    settings.IsCancelled = true;

                    context.SubmitChanges();

                    return true;
                }
            }
        }

        public bool SetSettingsPaused(DateTime date)
        {
            using (var commands = new DatabaseCommands())
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var settings = (from p in context.Settings
                        where p.DateStamp.Equals(date)
                        select p).FirstOrDefault();

                    settings.IsPaused = true;

                    context.SubmitChanges();

                    return true;
                }
            }
        }

        public bool SetSettingsNOTPaused(DateTime date)
        {
            using (var commands = new DatabaseCommands())
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var settings = (from p in context.Settings
                        where p.DateStamp.Equals(date)
                        select p).FirstOrDefault();

                    settings.IsPaused = false;

                    context.SubmitChanges();

                    return true;
                }
            }
        }

        public bool SetTasksNOTPaused(DateTime date)
        {
            using (var commands = new DatabaseCommands())
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var settings = (from p in context.Tasks
                        where p.DateStamp.Equals(date)
                        select p).FirstOrDefault();

                    settings.IsPaused = false;

                    context.SubmitChanges();

                    return true;
                }
            }
        }

        /// <summary>
        ///     Dumps the drawing stack.
        /// </summary>
        public void DumpDrawingStack()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var path = from p in context.DrawingStack
                    select p;

                if (path != null)

                    foreach (var p in path)
                    {
                        var log = new Logs();
                        log.DateStamp = DateTime.Now.ToString();
                        log.Issue = string.Format("{0},{1},{2},{3},{4},{5},{6}", p.DateStamp, p.PolylineDwgName, p.Hole,
                            p.TimeCompleted,
                            p.TimeStarted, p.HasError, p.PointCloudDwgName);

                        context.Logs.InsertOnSubmit(log);
                    }
                context.SubmitChanges();
            }
        }

        /// <summary>
        ///     Sets the start parameters in update.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="topLevelDir">The top level dir.</param>
        /// <param name="md5Checksum">The MD5 checksum.</param>
        public void SetStartParamsInUpdate(string dir, string topLevelDir, string md5Checksum)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = (from p in context.Updates
                        select p).FirstOrDefault();

                    if (items == null)
                    {
                        var update = new Updates();
                        update.CurrentVersion = md5Checksum;
                        update.PreviousVersion = md5Checksum;
                        update.DownloadPath = dir;
                        update.ExeName = topLevelDir;
                        update.IsDownloadComplete = true;
                        update.RequestDate = DateTime.Now;
                        update.DateStarted = DateTime.Now;
                        context.Updates.InsertOnSubmit(update);
                    }
                    else
                    {
                        items.PreviousVersion = items.CurrentVersion;
                        items.CurrentVersion = md5Checksum;
                        items.DownloadPath = dir;
                        items.ExeName = topLevelDir;
                        items.IsDownloadComplete = true;
                        items.DateStarted = DateTime.Now;
                        items.RequestDate = DateTime.Now;
                    }
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "SetStartParams");
            }
        }

        /// <summary>
        ///     Sets the invoked flag in update.
        /// </summary>
        /// <param name="md5Checksum">The MD5 checksum.</param>
        public void SetInvokedFlagInUpdate(string md5Checksum)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = (from p in context.Updates
                        where p.CurrentVersion == md5Checksum
                        select p).FirstOrDefault();

                    if (items == null)
                    {
                        var update = new Updates();
                        update.CurrentVersion = md5Checksum;
                        update.IsInvoked = true;
                        context.Updates.InsertOnSubmit(update);
                    }
                    else
                    {
                        items.IsInvoked = true;
                    }
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "SetInvokedFlagInUpdate");
            }
        }

        /// <summary>
        ///     Sets the date completed in updates.
        /// </summary>
        /// <param name="md5Checksum">The MD5 checksum.</param>
        public void SetDateCompletedInUpdates(string md5Checksum)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = (from p in context.Updates
                        select p).FirstOrDefault();

                    if (items == null)
                    {
                        var update = new Updates();
                        update.IsInstalled = true;
                        update.DateCompleted = DateTime.Now;
                        context.Updates.InsertOnSubmit(update);
                    }
                    else
                    {
                        items.IsInvoked = true;
                        items.DateCompleted = DateTime.Now;
                    }
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "SetDateCompleted");
            }
        }

        /// <summary>
        ///     Ses the started flag in update.
        /// </summary>
        /// <param name="md5Checksum">The MD5 checksum.</param>
        public void SeStartedFlagInUpdate(string md5Checksum)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = (from p in context.Updates
                        where p.CurrentVersion == md5Checksum
                        select p).FirstOrDefault();

                    if (items == null)
                    {
                        var update = new Updates();
                        update.CurrentVersion = md5Checksum;
                        update.IsStarted = true;
                        update.DateStarted = DateTime.Now;
                        context.Updates.InsertOnSubmit(update);
                    }
                    else
                    {
                        items.IsStarted = true;
                        items.DateStarted = DateTime.Now;
                    }
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, " SeStartedFlagInUpdate");
            }
        }

        /// <summary>
        ///     Ses the installed flag in update.
        /// </summary>
        /// <param name="md5Checksum">The MD5 checksum.</param>
        public void SeInstalledFlagInUpdate(string md5Checksum)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = (from p in context.Updates
                        where p.CurrentVersion == md5Checksum
                        select p).FirstOrDefault();

                    if (items == null)
                    {
                        var update = new Updates();
                        update.CurrentVersion = md5Checksum;
                        update.IsInstalled = true;
                        update.DateCompleted = DateTime.Now;
                        context.Updates.InsertOnSubmit(update);
                    }
                    else
                    {
                        items.IsInstalled = true;
                        items.DateCompleted = DateTime.Now;
                    }
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "SeInstalledFlagInUpdate");
            }
        }

        /// <summary>
        ///     Sets the has error flag in update.
        /// </summary>
        /// <param name="md5Checksum">The MD5 checksum.</param>
        public void SetHasErrorFlagInUpdate(string md5Checksum)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = (from p in context.Updates
                        where p.CurrentVersion == md5Checksum
                        select p).FirstOrDefault();

                    if (items == null)
                    {
                        var update = new Updates();
                        update.CurrentVersion = md5Checksum;
                        update.HasError = true;
                        context.Updates.InsertOnSubmit(update);
                    }
                    else
                    {
                        items.HasError = true;
                    }
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "SetHasErrorFlagInUpdate");
            }
        }

        /// <summary>
        ///     Sets the update current version m d5.
        /// </summary>
        /// <param name="md5Checksum">The MD5 checksum.</param>
        public void SetUpdateCurrentVersionMD5(string md5Checksum)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = (from p in context.Updates
                        where p.CurrentVersion == md5Checksum
                        select p).FirstOrDefault();

                    if (items == null)
                    {
                        var update = new Updates();
                        update.CurrentVersion = md5Checksum;
                        context.Updates.InsertOnSubmit(update);
                    }
                    else
                    {
                        items.CurrentVersion = md5Checksum;
                    }
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, " SetUpdateCurrentVersionMD5");
            }
        }

        /// <summary>
        ///     Gets the update current version m d5.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetUpdateCurrentVersionMD5()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = (from p in context.Updates
                        select p).FirstOrDefault();


                    if (items == null)
                        return null;

                    return items.CurrentVersion;
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "GetUpdateCurrentVersionMD5");
            }
            return null;
        }

        /// <summary>
        ///     Gets the update previous version m d5.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetUpdatePreviousVersionMD5()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = (from p in context.Updates
                        select p).FirstOrDefault();

                    if (items == null)
                        return null;

                    return items.PreviousVersion;
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "GetUpdatePreviousVersionMD5");
            }
            return null;
        }

        /// <summary>
        ///     Gets the update record.
        /// </summary>
        /// <returns>Updates.</returns>
        public Updates GetUpdateRecord()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = (from p in context.Updates
                        select p).FirstOrDefault();

                    if (items == null)
                        return null;

                    return items;
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "GetUpdateRecord");
            }
            return null;
        }

        /// <summary>
        ///     Gets the current build.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetCurrentBuild()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = (from p in context.Updates
                        select p).FirstOrDefault();

                    if (items == null)
                        return null;
                    if (items.BuildDay == null)
                        return null;

                    var result = string.Format("{0}.{1}.{2}.{3}",
                        items.BuildDay,
                        items.BuildDBVersion,
                        items.BuildCivilVersion,
                        items.BuildMinor??1);

                    return result;
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "GetCurrentBuild");
            }
            return null;
        }

        /// <summary>
        ///     Gets the current database build.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetCurrentInstalledBuild()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = (from p in context.Updates
                        select p).FirstOrDefault();

                    if (items == null)
                        return null;

                    var result = string.Format("{0}.{1}.{2}.{3}",
                        items.OBuildDay,
                        items.OBuildDBVersion,
                        items.OBuildCivilVersion,
                        items.OBuildMinor);

                    return result;
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "GetCurrentDBBuild");
            }
            return null;
        }

        /// <summary>
        ///     Gets the update download flag.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool GetUpdateDownloadFlag()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = (from p in context.Updates
                        select p).FirstOrDefault();

                    if ((items == null) || (items.IsDownloadComplete == null))
                        return false;

                    return (bool) items.IsDownloadComplete;
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "GetUpdateDownloadFlag");
            }
            return false;
        }

        /// <summary>
        ///     Gets the update error flag.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool GetUpdateErrorFlag()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = (from p in context.Updates
                        select p).FirstOrDefault();

                    if ((items == null) || (items.HasError == null))
                        return false;

                    return (bool) items.HasError;
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "GetCurrentDBBuild");
            }
            return false;
        }

        /// <summary>
        ///     Sets the clear flags in update.
        /// </summary>
        public void SetClearFlagsInUpdate()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = (from p in context.Updates
                        select p).FirstOrDefault();

                    if (items == null)
                    {
                        var update = new Updates();
                        update.DownloadPath = null;
                        update.ExeName = null;
                        update.HasError = false;
                        update.IsDownloadComplete = false;
                        update.DateCompleted = null;
                        update.DateStarted = null;

                        context.Updates.InsertOnSubmit(update);
                    }
                    else
                    {
                        items.DownloadPath = null;
                        items.ExeName = null;
                        items.HasError = false;
                        items.IsDownloadComplete = false;
                        items.DateCompleted = null;
                        items.DateStarted = null;
                    }
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "SetClearFlagsInUpdate");
            }
        }

        /// <summary>
        ///     Sets the build version in update.
        /// </summary>
        /// <param name="version">The version.</param>
        public void SetBuildVersionInUpdate(string version)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = (from p in context.Updates
                        select p).FirstOrDefault();

                    var len = version.Length;

                    if (items == null)
                    {
                        var update = new Updates();
                        update.BuildCivilVersion = ConvertInt32(version.Substring(9, 2));
                        update.BuildDBVersion = ConvertInt32(version.Substring(6, 2));
                        update.BuildYear = ConvertInt32(version.Substring(0, 2));
                        update.BuildDay = ConvertInt32(version.Substring(0, 5));
                        if (len == 15)
                            update.OBuildMinor = ConvertInt32(version.Substring(12, 3));
                        else
                            update.OBuildMinor = ConvertInt32(version.Substring(12, len - 12));

                        context.Updates.InsertOnSubmit(update);
                    }
                    else
                    {
                        items.BuildCivilVersion = ConvertInt32(version.Substring(9, 2));
                        items.BuildDBVersion = ConvertInt32(version.Substring(6, 2));
                        items.BuildYear = ConvertInt32(version.Substring(0, 2));
                        items.BuildDay = ConvertInt32(version.Substring(0, 5));

                        if (len == 15)
                            items.BuildMinor = ConvertInt32(version.Substring(12, 3).Trim());
                        else
                        {
                            items.BuildMinor = ConvertInt32(version.Substring(12, len - 12).Trim());
                        }
                    }
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "SetBuildVersionInUpdate");
            }
        }

        /// <summary>
        ///     Sets the build database version in update.
        /// </summary>
        /// <param name="version">The version.</param>
        public void SetBuildDBVersionInUpdate(string version)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = (from p in context.Updates
                        select p).FirstOrDefault();

                    var len = version.Length;

                    if (items == null)
                    {
                        var update = new Updates();
                        update.OBuildCivilVersion = ConvertInt32(version.Substring(9, 2));
                        update.OBuildDBVersion = ConvertInt32(version.Substring(6, 2));
                        update.OBuildYear = ConvertInt32(version.Substring(0, 2));
                        update.OBuildDay = ConvertInt32(version.Substring(0, 5));

                        if (len == 15)
                            update.OBuildMinor = ConvertInt32(version.Substring(12, 3));
                        else
                            update.OBuildMinor = ConvertInt32(version.Substring(12, len - 12));

                        context.Updates.InsertOnSubmit(update);
                    }
                    else
                    {
                        items.OBuildCivilVersion = ConvertInt32(version.Substring(9, 2));
                        items.OBuildDBVersion = ConvertInt32(version.Substring(6, 2));
                        items.OBuildYear = ConvertInt32(version.Substring(0, 2));
                        items.OBuildDay = ConvertInt32(version.Substring(0, 5));
                        if (len == 15)
                            items.OBuildMinor = ConvertInt32(version.Substring(12, 3).Trim());
                        else
                        {
                            items.OBuildMinor = ConvertInt32(version.Substring(12, len - 12).Trim());
                        }
                    }
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "SetBuildVersionInUpdate");
            }
        }


        /// <summary>
        ///     Converts the int32.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns>System.Int32.</returns>
        public int ConvertInt32(string val)
        {
            var result = 0;
            try
            {
                int.TryParse(val, out result);
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "TryCast");
            }
            return result;
        }

        /// <summary>
        ///     Converts the int64.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns>System.Int64.</returns>
        public long ConvertInt64(string val)
        {
            long result = 0;
            try
            {
                long.TryParse(val, out result);
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "TryCast");
            }
            return result;
        }


        /// <summary>
        ///     Inserts the polyline.
        /// </summary>
        /// <param name="p">The p.</param>
        public void InsertPolyline(Polyline p)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    context.Polyline.InsertOnSubmit(p);
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "SetBuildVersionInUpdate");
            }
        }

        /// <summary>
        ///     Gets the poly inners by handle.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>IList&lt;Polyline&gt;.</returns>
        public IList<Polyline> GetPolyInnersByHandle(string e)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = from p in context.Polyline
                        where p.Handle == e
                        select p;
                    return items != null ? items.ToList() : null;
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "GetPolyInnersByHandle");
            }
            return null;
        }

        /// <summary>
        ///     Gets the poly inners.
        /// </summary>
        /// <returns>IList&lt;Polyline&gt;.</returns>
        public IList<Polyline> GetPolyInners()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = from p in context.Polyline
                        select p;
                    return items.ToList();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "SetBuildVersionInUpdate");
            }
            return null;
        }

        /// <summary>
        ///     Clears the polyline inners.
        /// </summary>
        public void ClearPolylineInners()
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = from p in context.Polyline
                        select p;

                    context.Polyline.DeleteAllOnSubmit(items);
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "SetBuildVersionInUpdate");
            }
        }

        /// <summary>
        ///     Gets the poly inners by area and layer.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="l">The l.</param>
        /// <returns>IList&lt;Polyline&gt;.</returns>
        public IList<Polyline> GetPolyInnersByAreaAndLayer(string a, string l)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var items = from p in context.Polyline
                        where (p.Area == a) && (p.Layer == l)
                        select p;
                    return items.ToList();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "SetBuildVersionInUpdate");
            }
            return null;
        }

        /// <summary>
        ///     Inserts the into general use DWGS.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool InsertIntoGeneralUseDwgs(List<string> files)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var date = DateTime.Now;
                    var stack = new List<GeneralCADStack>();

                    foreach (var item in files)
                    {
                        var f = new GeneralCADStack();
                        f.CompletePath = item;
                        f.DwgName = Path.GetFileName(item);
                        f.DateStamp = date;
                        f.Function = "SportVision";

                        stack.Add(f);
                    }

                    context.GeneralCADStack.InsertAllOnSubmit(stack);
                    context.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "Insert Files");
            }
            return false;
        }

        /// <summary>
        ///     Gus the mark completed.
        /// </summary>
        /// <param name="name">The name.</param>
        public void GUMarkCompleted(string name)
        {
            try
            {
                using (var context = GetDataBasePath.GetSql4Connection())
                {
                    var date = DateTime.Now;
                    var stack = (from f in context.GeneralCADStack
                        where (f.CompletePath == name) &&
                              (f.IsCompleted == null)
                        select f).FirstOrDefault();

                    stack.IsCompleted = true;
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.AddExceptionLogs(ex, "Insert Files");
            }
        }
    }
}