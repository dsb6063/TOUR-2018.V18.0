using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGA.Sv.PostAudit
{
    public interface IDatabase
    {
        object GetSettings(DateTime dt);
    }
}
