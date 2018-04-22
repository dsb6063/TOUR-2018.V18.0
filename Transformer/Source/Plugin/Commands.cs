// (C) Copyright 2010 by Autodesk, Inc. 
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted, 
// provided that the above copyright notice appears in all copies and 
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting 
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to 
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System;
using MatrixEditor;

namespace Transformer
{
  public class App : IExtensionApplication
  {
    public void Initialize()
    {
      try
      {
        DemandLoading.RegistryUpdate.RegisterForDemandLoading();
      }
      catch
      { }
    }

    public void Terminate()
    {
    }
  }

  public class Commands
  {
    [CommandMethod("SELMATENT")]
    static public void SelectMatrixEntity()
    {
      Document doc =
        Autodesk.AutoCAD.ApplicationServices.
        Application.DocumentManager.MdiActiveDocument;
      Database db = doc.Database;
      Editor ed = doc.Editor;

      PromptEntityResult per = ed.GetEntity("\nSelect entity: ");
      if (per.Status != PromptStatus.OK)
        return;

      _mec.SetSelectedEntity(per.ObjectId);
    }

    [CommandMethod("TRANS", CommandFlags.UsePickSet)]
    static public void TransformEntity()
    {
      Document doc =
        Autodesk.AutoCAD.ApplicationServices.
        Application.DocumentManager.MdiActiveDocument;
      var db = doc.Database;
      Editor ed = doc.Editor;

      // Our selected entity (only one supported, for now)

      ObjectId id;

      // First query the pickfirst selection set

      PromptSelectionResult psr = ed.SelectImplied();
      if (psr.Status != PromptStatus.OK || psr.Value == null)
      {
        // If nothing selected, ask the user

        PromptEntityOptions peo =
          new PromptEntityOptions(
            "\nSelect entity to transform: "
          );
        PromptEntityResult per = ed.GetEntity(peo);
        if (per.Status != PromptStatus.OK)
          return;
        id = per.ObjectId;
      }
      else
      {
        // If the pickfirst set has one entry, take it

        SelectionSet ss = psr.Value;
        if (ss.Count != 1)
        {
          ed.WriteMessage(
            "\nThis command works on a single entity."
          );
          return;
        }
        ObjectId[] ids = ss.GetObjectIds();
        id = ids[0];
      }

      PromptResult pr = ed.GetString("\nEnter property name: ");
      if (pr.Status != PromptStatus.OK)
        return;

      string prop = pr.StringResult;

      // Now let's ask for the matrix string

      pr = ed.GetString("\nEnter matrix values: ");
      if (pr.Status != PromptStatus.OK)
        return;

      // Split the string into its individual cells

      string[] cells = pr.StringResult.Split(new char[] { ',' });
      if (cells.Length != 16)
      {
        ed.WriteMessage("\nMust contain 16 entries.");
        return;
      }

      try
      {
        // Convert the array of strings into one of doubles

        double[] data = new double[cells.Length];
        for (int i = 0; i < cells.Length; i++)
        {
          data[i] = double.Parse(cells[i]);
        }

        // Create a 3D matrix from our cell data

        Matrix3d mat = new Matrix3d(data);

        // Now we can transform the selected entity

        Transaction tr =
          doc.TransactionManager.StartTransaction();
        using (tr)
        {
          Entity ent =
            tr.GetObject(id, OpenMode.ForWrite)
            as Entity;
          if (ent != null)
          {
            bool transformed = false;

            // If the user specified a property to modify

            if (!string.IsNullOrEmpty(prop))
            {
              // Query the property's value

              object val =
                ent.GetType().InvokeMember(
                  prop, BindingFlags.GetProperty, null, ent, null
                );
              
              // We only know how to transform points and vectors

              if (val is Point3d)
              {
                // Cast and transform the point result

                Point3d pt = (Point3d)val,
                        res = pt.TransformBy(mat);

                // Set it back on the selected object

                ent.GetType().InvokeMember(
                  prop, BindingFlags.SetProperty, null,
                  ent, new object[] { res }
                );
                transformed = true;
              }
              else if (val is Vector3d)
              {
                // Cast and transform the vector result

                Vector3d vec = (Vector3d)val,
                         res = vec.TransformBy(mat);

                // Set it back on the selected object

                ent.GetType().InvokeMember(
                  prop, BindingFlags.SetProperty, null,
                  ent, new object[] { res }
                );
                transformed = true;
              }
            }

            // If we didn't transform a property,
            // do the whole object
            
            if (!transformed)
              ent.TransformBy(mat);
          }
          tr.Commit();
        }
      }
      catch (Autodesk.AutoCAD.Runtime.Exception ex)
      {
        ed.WriteMessage(
          "\nCould not transform entity: {0}", ex.Message
        );
      }
    }

    static PaletteSet _ps = null;
    static MatrixEditorControl _mec = null;
 
    [CommandMethod("MATRIX")]
    public void MatrixEditor()
    {
      if (_ps == null)
      {
        // Create the palette set
 
        _ps =
          new PaletteSet(
            "Matrix",
            new Guid("{69F6F495-3918-4229-AAFF-048ED94C231F}")
          );
        _ps.Size = new Size(720, 630);
        _ps.DockEnabled =
          (DockSides)((int)DockSides.Left + (int)DockSides.Right);
 
        // Create our first user control instance and
        // host it on a palette using AddVisual()

        _mec = new MatrixEditorControl();
        _ps.AddVisual("MatrixEditor", _mec);

        // Resize the control when the palette resizes

        _ps.SizeChanged +=
          delegate(object sender, PaletteSetSizeEventArgs e)
          {
            _mec.Width = e.Width;
            _mec.Height = e.Height;
          };
      }
 
      // Display our palette set
 
      _ps.KeepFocus = true;
      _ps.Visible = true;
    }
  }
}
