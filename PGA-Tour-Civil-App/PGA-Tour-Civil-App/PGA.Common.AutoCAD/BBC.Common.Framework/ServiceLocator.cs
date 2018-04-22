using System;
using System.Collections.Generic;
using System.Text;

using Pge.Common.DataManager;

namespace Pge.Common.Framework
{
    public class ServiceLocator
    {
        public static IDataManager DataManager
        {
            get
            {
                return new DataManagerImpl();
            }
        }
    }
}
