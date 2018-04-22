using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGA.DatabaseManager.Helpers
{

    public interface ITimeSpan
    {
        string CalcTimeSpan(DateTime d1, DateTime d2);
    }

    public class CustomTimeSpan:ITimeSpan
    {
        public string  CalcTimeSpan(DateTime d1, DateTime d2)
        {
            System.TimeSpan ts = (d1 - d2);
            return string.Format("{0:%d} days, {0:%h}:{0:%m}:{0:%s}", ts);

            //string.Format("{0:%d} days, {0:%h} hours, {0:%m} minutes, {0:%s} seconds", ts);
        }
    }
}
