using System;

namespace C3DSurfacesDemo
{
    public class CreateSurfaceException : ApplicationException
    {
        public CreateSurfaceException(string message)
            : base(message)
        {
        }
    }
}