using System;

namespace C3DSurfacesDemo
{
    public class COMInstanceException : ApplicationException
    {
        public COMInstanceException(string message, string objectInterface)
            : base(message)
        {
            m_ObjectInterface = objectInterface;
        }

        public string Interface
        {
            get { return m_ObjectInterface; }
        }

        private string m_ObjectInterface;
    }
}