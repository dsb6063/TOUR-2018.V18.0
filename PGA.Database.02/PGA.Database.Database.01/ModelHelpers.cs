#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endregion

namespace PGA.Database
{
    public class ModelHelpers
    {
    }

    public class Holes
    {
        private readonly string[] holes;

        public Holes()
        {
            holes = new string[18];

            for (var i = 1; i < 19; i++)
            {
                if (i < 10)
                    holes[i - 1] = $"{0}{i}";
                else
                {
                    holes[i - 1] = string.Format("{0}", arg0: i);
                }
                Debug.WriteLine(holes[i - 1]);
            }
        }


        public object this[int isuffix]
        {
            get { return GetHole(isuffix); }
        }

        public string GetHole(int hole)
        {
            hole -= 1;
            if ((hole < 0) || (hole > 18))
                return null;

            return holes[hole];
        }
    }


    public class TaskWithProjectInfo : IEnumerable<TaskWithProjectInfo>
    {
        public bool? Cancelled;
        public DateTime Completed;
        public string Course;
        public string Created;
        public DateTime Date;

        private readonly List<TaskWithProjectInfo> mylist = new List<TaskWithProjectInfo>();
        public bool? Paused;
        public string ProjectCreator;
        public DateTime Started;

        public TaskWithProjectInfo this[int index]
        {
            get { return mylist[index]; }
            set { mylist.Insert(index, value); }
        }


        public IEnumerator<TaskWithProjectInfo> GetEnumerator()
        {
            return mylist.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IList<TaskWithProjectInfo> ToList()
        {
            return mylist.ToList();
        }
    }


    public class MyTaskCollection<TaskWithProjectInfo> : IList<TaskWithProjectInfo>
    {
        private readonly IList<TaskWithProjectInfo> _list = new List<TaskWithProjectInfo>();

        #region Implementation of IEnumerable

        public IEnumerator<TaskWithProjectInfo> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<T>

        public void Add(TaskWithProjectInfo item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(TaskWithProjectInfo item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(TaskWithProjectInfo[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(TaskWithProjectInfo item)
        {
            return _list.Remove(item);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return _list.IsReadOnly; }
        }

        #endregion

        #region Implementation of IList<T>

        public int IndexOf(TaskWithProjectInfo item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, TaskWithProjectInfo item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public TaskWithProjectInfo this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        #endregion

        #region Your Added Stuff

        // Add new features to your collection.

        #endregion
    }

    public static class GolfHoles
    {
        public static string GetHoles(int h)
        {
            var hole = new Holes();
            return hole.GetHole(h);
        }

        public static string GetHoles(string h)
        {
            var hole = new Holes();
            return hole.GetHole(Convert.ToInt32(h));
        }

        public static bool IsWithinLimits(string h)
        {
            var strconvert = Convert.ToInt32(h);
            if ((strconvert > 0) && (strconvert < 19))
                return true;
            return false;
        }

        public static bool IsWithinLimits(int h)
        {
            var strconvert = Convert.ToInt32(h);
            if ((strconvert > 0) && (strconvert < 19))
                return true;
            return false;
        }
    }
}