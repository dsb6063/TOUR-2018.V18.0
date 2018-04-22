using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGA.OffsetPolylines
{
    public class Algorithm
    {

        public double GetDistanceTolerance(int numOfPolylines, double step)
        {
            //T Total Tolerance
            //n Num of Polylines
            //s Step increment
            //F Final tolerance

            // (T - F)/s = Math.Ceiling(n);
            // T = (F + s*n)
            var n = numOfPolylines;

            var s = step;
            var F = step;


            return (F + s*n);
        }

        public double GetNumberOfPolylines(double estimate, double step)
        {
            //T Total Tolerance
            //n Num of Polylines
            //s Step increment
            //F Final tolerance

            // (T - F)/s = Math.Ceiling(n);
            // T = (F + s*n)

            var T = estimate;
            var s = step;
            var F = step;

            var n = (T - F)/s;

            return (Math.Ceiling(n));
        }




    }
}
