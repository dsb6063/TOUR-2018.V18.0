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
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Text;
using System;

namespace MatrixEditor
{
  /// <summary>
  /// Interaction logic for MatrixEditorControl.xaml
  /// </summary>
  /// 
  public partial class MatrixEditorControl : UserControl
  {
    #region Member variables / constants

    Matrix3d _current; // The current matrix state
    ObjectId _entId;   // The selected entity
    bool _dirty;       // Whether the matrix has been edited

    // String to display in property combo

    const string entireEntity = "Entire entity";

    #endregion

    #region P/Invoke declarations

    // Win32 call to avoid double-click on entity selection

    [DllImport("user32.dll")]
    private static extern IntPtr SetFocus(IntPtr hWnd);

    #endregion

    #region Constructor

    public MatrixEditorControl()
    {
      InitializeComponent();
      SetIdentity();
      ClearPropertyCombo();
      _dirty = false;
    }

    #endregion

    #region Externally callable protocol

    // Called by our external command to set the
    // selected entity

    public void SetSelectedEntity(ObjectId id)
    {
      _entId = id;

      ClearPropertyCombo();
      PropertyCombo.SelectedIndex = 0;

      if (id == ObjectId.Null)
      {
        // Disable the UI, if the ID is null

        TransformButton.IsEnabled = false;
        TransformButton.Content = " ";
        PropertyCombo.IsEnabled = false;
      }
      else
      {
        // Enabled the UI

        TransformButton.IsEnabled = true;
        PropertyCombo.IsEnabled = true;
        TransformButton.Content = "Transform";

        Document doc =
          Autodesk.AutoCAD.ApplicationServices.Application.
          DocumentManager.MdiActiveDocument;
        Transaction tr = doc.TransactionManager.StartTransaction();
        using (tr)
        {
          // Open our object and query its properties and their
          // types

          Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;
          if (ent != null)
          {
            PropertyInfo[] props = ent.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
              // We currently only transform 3D points and matrices

              if ((prop.PropertyType == typeof(Matrix3d) ||
                   prop.PropertyType == typeof(Point3d))
                   && prop.CanWrite)
              {
                PropertyCombo.Items.Add(prop.Name);
              }
            }
          }
          tr.Commit();
        }
      }
    }

    #endregion

    #region Matrix display functions

    // Reset display to the identity matrix

    internal void SetIdentity()
    {
      _current = Matrix3d.Identity;
      LoadMatrix(_current);
      LoadMatrix(_current);
    }

    // Load a specified matrix in the UI

    internal void LoadMatrix(Matrix3d mat)
    {
      double[] data = mat.ToArray();

      SetMatrixEntry(a, data[0], true);
      SetMatrixEntry(b, data[1], true);
      SetMatrixEntry(c, data[2], true);
      SetMatrixEntry(d, data[3], true);
      SetMatrixEntry(e, data[4], true);
      SetMatrixEntry(f, data[5], true);
      SetMatrixEntry(g, data[6], true);
      SetMatrixEntry(h, data[7], true);
      SetMatrixEntry(i, data[8], true);
      SetMatrixEntry(j, data[9], true);
      SetMatrixEntry(k, data[10], true);
      SetMatrixEntry(l, data[11], true);
      SetMatrixEntry(m, data[12], true);
      SetMatrixEntry(n, data[13], true);
      SetMatrixEntry(o, data[14], true);
      SetMatrixEntry(p, data[15], true);
    }

    // Add (by multiplication) a matrix to the current one
    // and display it in the UI

    internal void AddMatrix(Matrix3d mat)
    {
      _current = _current.PreMultiplyBy(mat);
      LoadMatrix(_current);
    }

    // Update our current matrix if edited manually

    internal void UpdateIfDirty()
    {
      if (_dirty)
      {
        double[] data =
          new double[16]
          {
            Double.Parse(a.Text),
            Double.Parse(b.Text),
            Double.Parse(c.Text),
            Double.Parse(d.Text),
            Double.Parse(e.Text),
            Double.Parse(f.Text),
            Double.Parse(g.Text),
            Double.Parse(h.Text),
            Double.Parse(i.Text),
            Double.Parse(j.Text),
            Double.Parse(k.Text),
            Double.Parse(l.Text),
            Double.Parse(m.Text),
            Double.Parse(n.Text),
            Double.Parse(o.Text),
            Double.Parse(p.Text)
          };

        _current = new Matrix3d(data);
        LoadMatrix(_current);
        _dirty = false;
      }
    }

    // Truncate a matrix value for display

    private string TruncateForDisplay(double value)
    {
      int whole = (int)value;
      double partial = Math.Abs(value - whole);

      if (partial < Tolerance.Global.EqualPoint)
        return whole.ToString();
      else
        return value.ToString("F2", CultureInfo.InvariantCulture);
    }

    // Set the value of a matrix entry, changing the colour
    // to red, if it has changed

    private void SetMatrixEntry(
      TextBox tb, double value, bool truncate
    )
    {
      string content =
        (truncate ? TruncateForDisplay(value) : value.ToString());
      if (tb.Text != content)
      {
        tb.Text = content;
        tb.Foreground = Brushes.Red;
      }
      else
      {
        tb.Foreground = Brushes.Black;
      }
    }

    // Get the current matrix as a string,
    // to send as a command argument

    internal string GetMatrixString(Matrix3d mat)
    {
      double[] data = mat.ToArray();
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < data.Length; i++)
      {
        sb.Append(data[i]);
        if (i < data.Length - 1)
          sb.Append(',');
      }
      return sb.ToString();
    }

    #endregion

    #region Other UI functions

    // Clear the property selection combobox

    internal void ClearPropertyCombo()
    {
      PropertyCombo.Items.Clear();
      PropertyCombo.Items.Add(entireEntity);
    }

    #endregion

    #region Button-click events

    private void IdentityButton_Click(
      object sender, RoutedEventArgs e
    )
    {
      SetIdentity();
    }

    private void UCSButton_Click(
      object sender, RoutedEventArgs e
    )
    {
      _current =
        Autodesk.AutoCAD.ApplicationServices.Application.
        DocumentManager.MdiActiveDocument.Editor.
        CurrentUserCoordinateSystem;
      LoadMatrix(_current);
    }

    private void TransposeButton_Click(
      object sender, RoutedEventArgs e
    )
    {
      UpdateIfDirty();

      _current = _current.Transpose();
      LoadMatrix(_current);
    }

    private void DispButton_Click(
      object sender, RoutedEventArgs e
    )
    {
      UpdateIfDirty();

      AddMatrix(
        Matrix3d.Displacement(
          new Vector3d(
            Double.Parse(DispVectorX.Text),
            Double.Parse(DispVectorY.Text),
            Double.Parse(DispVectorZ.Text)
          )
        )
      );
    }

    private void ScaleButton_Click(
      object sender, RoutedEventArgs e
    )
    {
      UpdateIfDirty();

      AddMatrix(
        Matrix3d.Scaling(
          Double.Parse(ScaleFactor.Text),
          new Point3d(
            Double.Parse(ScaleOrigX.Text),
            Double.Parse(ScaleOrigY.Text),
            Double.Parse(ScaleOrigZ.Text)
          )
        )
      );
    }
    
    private void MirrButton_Click(
      object sender, RoutedEventArgs e
    )
    {
      UpdateIfDirty();

      AddMatrix(
        Matrix3d.Mirroring(
          new Line3d(
            new Point3d(
              Double.Parse(MirrStartX.Text),
              Double.Parse(MirrStartY.Text),
              Double.Parse(MirrStartZ.Text)
            ),
            new Point3d(
              Double.Parse(MirrEndX.Text),
              Double.Parse(MirrEndY.Text),
              Double.Parse(MirrEndZ.Text)
            )
          )
        )
      );
    }

    private void RotButton_Click(
      object sender, RoutedEventArgs e
    )
    {
      UpdateIfDirty();

      AddMatrix(
        Matrix3d.Rotation(
          Double.Parse(RotAngle.Text) * Math.PI / 180.0,
          new Vector3d(
            Double.Parse(RotAxisX.Text),
            Double.Parse(RotAxisY.Text),
            Double.Parse(RotAxisZ.Text)
          ),
          new Point3d(
            Double.Parse(RotOrigX.Text),
            Double.Parse(RotOrigY.Text),
            Double.Parse(RotOrigZ.Text)
          )
        )
      );
    }

    private void SelectButton_Click(
      object sender, RoutedEventArgs e
    )
    {
      SetFocus(
        Autodesk.AutoCAD.ApplicationServices.Application.
        MainWindow.Handle
      );

      Autodesk.AutoCAD.ApplicationServices.
      Application.DocumentManager.MdiActiveDocument.
      SendStringToExecute(
        "_SELMATENT ", false, false, false
      );
    }

    private void TransformButton_Click(
      object sender, RoutedEventArgs e
    )
    {
      Document doc =
        Autodesk.AutoCAD.ApplicationServices.
        Application.DocumentManager.MdiActiveDocument;
      Editor ed = doc.Editor;

      UpdateIfDirty();

      ed.SetImpliedSelection(new ObjectId[] { _entId });
      string cmd =
        "_TRANS " +
        (PropertyCombo.Text == entireEntity ?
          " " : PropertyCombo.Text + " ") +
        GetMatrixString(_current) + " ";
      doc.SendStringToExecute(
        cmd, false, false, true
      );
    }
    
    #endregion

    #region Other UI events

    // A matrix value has been edited manually

    private void cell_TextChanged(
      object sender, TextChangedEventArgs e
    )
    {
      // Change the text colour to red and set the dirty flag

      TextBox tb = (TextBox)sender;
      tb.Foreground = Brushes.Red;
      _dirty = true;
    }

    #endregion
  }
}
