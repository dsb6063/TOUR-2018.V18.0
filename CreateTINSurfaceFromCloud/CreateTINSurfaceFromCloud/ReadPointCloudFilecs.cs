// ***********************************************************************
// Assembly         : CreateTINSurfaceFromCloud
// Author           : Daryl Banks, PSM
// Created          : 01-04-2016
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 02-28-2017
// ***********************************************************************
// <copyright file="ReadPointCloudFilecs.cs" company="Banks & Banks Consulting">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Autodesk.AutoCAD.Geometry;
using COMS = PGA.MessengerManager.MessengerManager;


namespace CreateTINSurfaceFromCloud
{
    /// <summary>
    /// Class ReadPointCloudFile.
    /// </summary>
    public static class ReadPointCloudFile
    {
        /// <summary>
        /// Reads the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>Point3dCollection.</returns>
        public static Point3dCollection ReadFile(string path, string filename)
        {
            Point3dCollection points = new Point3dCollection();
            string line;
            using (StreamReader reader = new StreamReader(Path.Combine(path, filename)))
                while ((line = reader.ReadLine()) != null)
                {
                    points.Add(GetCoordinates(line));

                }
            return points;
        }


        /// <summary>
        /// Gets the coordinates.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>Point3d.</returns>
        /// <exception cref="FileFormatException">
        /// LiDAR file positions are malformed! (PN,Y,X,Z,DESC
        /// or
        /// LiDAR file contents are malformed! (PN,Y,X,Z,DESC
        /// </exception>
        public static Point3d GetCoordinates(string line)
        {
                var cell = line.Split(',');

            if (CheckLineLength(cell))
            {
                throw new FileFormatException("LiDAR file positions are malformed! (PN,Y,X,Z,DESC)");
            }
            if (MalformedCell(cell))
            {
                throw new FileFormatException(String.Format("LiDAR file contents are invalid! (PN,Y,X,Z,DESC) at line {0}",cell[0]));
            }
          
            double y = Double.Parse(cell[1]);
            double x = Double.Parse(cell[2]);
            double z = Double.Parse(cell[3]);

            return new Point3d(x, y, z);
           
        }

        /// <summary>
        /// Malformed the cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool MalformedCell(string[] cell)
        {
            try
            {
                Double.Parse(cell[1]);
                Double.Parse(cell[2]);
                Double.Parse(cell[3]);
            }
            catch (Exception )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks the length of the line.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CheckLineLength(string [] cell)
        {
            if (cell.Length != 5)
                return true;
            return false;
        }
        
    }

}

