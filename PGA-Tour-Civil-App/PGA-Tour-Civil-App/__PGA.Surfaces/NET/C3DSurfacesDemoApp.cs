using System;

using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(C3DSurfacesDemo.C3DSurfacesDemoApp))]

namespace C3DSurfacesDemo
{
    public class C3DSurfacesDemoApp : IExtensionApplication
    {
        public void Initialize()
        {

        }

        public void Terminate()
        {

        }
    }
}

