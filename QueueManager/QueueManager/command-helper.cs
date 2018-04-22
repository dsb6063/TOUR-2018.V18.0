using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

namespace QueueManager
{
  public class CmdCommands
  {
    // Mutex to stop unknown command handler re-entrancy

    private bool _launched = false;

    [CommandMethod("CMDS")]
    public void CommandTranslation()
    {
      var doc = Application.DocumentManager.MdiActiveDocument;
      if (doc == null)
        return;

      // Add our command prefixing event handler

      _launched = false;

      doc.UnknownCommand += OnUnknownCommand;
      doc.CommandWillStart += OnCommandSomething;
      doc.CommandEnded += OnCommandSomething;
      doc.CommandFailed += OnCommandSomething;
      doc.CommandCancelled += OnCommandSomething;

      // autoComplete and autocoRrect cause problems with
      // this, so let's turn them off (we may want to warn
      // the user or reset the values, afterwards)

      var nm = Application.GetSystemVariable("NOMUTT");
      Application.SetSystemVariable("NOMUTT", 1);
      var ce = Application.GetSystemVariable("CMDECHO");
      Application.SetSystemVariable("CMDECHO", 0);

      doc.Editor.Command(
        "_.-INPUTSEARCHOPTIONS",
        "_C", "_N", "_R", "_N", "_S", "_N", "_T", "_N", "_M", "_N",
        ""
      );

      Application.SetSystemVariable("NOMUTT", nm);
      Application.SetSystemVariable("CMDECHO", ce);

      doc.Editor.WriteMessage(
        "\n\n" +
        "Global command support enabled. Run CMDSX to turn it off."
      );
    }

    [CommandMethod("CMDSX")]
    public void StopCommandTranslation()
    {
      var doc = Application.DocumentManager.MdiActiveDocument;
      if (doc == null)
        return;

      // Remove our command prefixing event handler

      doc.UnknownCommand -= OnUnknownCommand;
      doc.CommandWillStart -= OnCommandSomething;
      doc.CommandEnded -= OnCommandSomething;
      doc.CommandFailed -= OnCommandSomething;
      doc.CommandCancelled -= OnCommandSomething;

      doc.Editor.WriteMessage(
        "\nGlobal command support disabled. Run CMDS to turn it on."
      );
    }

    private void OnUnknownCommand(
      object sender, UnknownCommandEventArgs e
    )
    {
      var doc = sender as Document;
      if (doc == null)
        return;

      // Check to make sure we're not re-entering the handler

      if (_launched)
      {
        _launched = false;
      }
      else
      {
        try
        {
          // Set the mutex flag and call our command

          _launched = true;
          doc.SendStringToExecute(
            "_" + e.GlobalCommandName + " ", true, false, false
          );
        }
        catch (System.Exception ex)
        {
          doc.Editor.WriteMessage("\nException: {0}\n", ex.Message);
        }
      }
    }

    private void OnCommandSomething(
      object sender, CommandEventArgs e
    )
    {
      _launched = false;
    }
  }
}
