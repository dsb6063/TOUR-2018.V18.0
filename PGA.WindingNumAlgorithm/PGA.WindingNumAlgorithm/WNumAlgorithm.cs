using Autodesk.AutoCAD.Geometry;

namespace PGA.WindingNumAlgorithm
{
    public static class WNumAlgorithm
    {
        // Copyright 2000 softSurfer, 2012 Dan Sunday
        // This code may be freely used and modified for any purpose
        // providing that this copyright notice is included with it.
        // SoftSurfer makes no warranty for this code, and cannot be held
        // liable for any real or imagined damage resulting from its use.
        // Users of this code must verify correctness for their application.


        // a Point is defined by its coordinates {int x, y;}
        //===================================================================


        // isLeft(): tests if a point is Left|On|Right of an infinite line.
        //    Input:  three points P0, P1, and P2
        //    Return: >0 for P2 left of the line through P0 and P1
        //            =0 for P2  on the line
        //            <0 for P2  right of the line
        //    See: Algorithm 1 "Area of Triangles and Polygons"
        public static double isLeft(Point2d P0, Point2d P1, Point2d P2)
        {
            return ((P1.X - P0.X) * (P2.Y - P0.Y)
                    - (P2.X - P0.X) * (P1.Y - P0.Y));
        }
        //===================================================================


        // cn_PnPoly(): crossing number test for a point in a polygon
        //      Input:   P = a point,
        //               V[] = vertex points of a polygon V[n+1] with V[n]=V[0]
        //      Return:  0 = outside, 1 = inside
        // This code is patterned after [Franklin, 2000]
        public static int  cn_PnPoly(Point2d P, Point2d[] V, int n)
        {
            int cn = 0;    // the  crossing number counter

            // loop through all edges of the polygon
            for (int i = 0; i < n; i++)
            {    // edge from V[i]  to V[i+1]
                if (((V[i].Y <= P.Y) && (V[i + 1].Y > P.Y))     // an upward crossing
                 || ((V[i].Y > P.Y) && (V[i + 1].Y <= P.Y)))
                { // a downward crossing
                  // compute  the actual edge-ray intersect x-coordinate
                    double vt = (double)(P.Y - V[i].Y) / (V[i + 1].Y - V[i].Y);
                    if (P.X < V[i].X + vt * (V[i + 1].X - V[i].X)) // P.X < intersect
                        ++cn;   // a valid crossing of y=P.Y right of P.X
                }
            }
            return (cn & 1);    // 0 if even (out), and 1 if  odd (in)

        }
        //===================================================================


        // wn_PnPoly(): winding number test for a point in a polygon
        //      Input:   P = a point,
        //               V[] = vertex points of a polygon V[n+1] with V[n]=V[0]
        //      Return:  wn = the winding number (=0 only when P is outside)
        public static int  wn_PnPoly(Point2d P, Point2d[] V, int n)
        {
            int wn = 0;    // the  winding number counter

            // loop through all edges of the polygon
            for (int i = 0; i < n; i++)
            {   // edge from V[i] to  V[i+1]

                if (i + 1 == n)
                    return wn;

                if (V[i].Y <= P.Y)
                {          // start y <= P.Y
                    if (V[i + 1].Y > P.Y)      // an upward crossing
                        if (isLeft(V[i], V[i + 1], P) > 0)  // P left of  edge
                            ++wn;            // have  a valid up intersect
                }
                else
                {                        // start y > P.Y (no test needed)
                    if (V[i + 1].Y <= P.Y)     // a downward crossing
                        if (isLeft(V[i], V[i + 1], P) < 0)  // P right of  edge
                            --wn;            // have  a valid down intersect
                }
            }
            return wn;
        }
        //===================================================================

    }
}
