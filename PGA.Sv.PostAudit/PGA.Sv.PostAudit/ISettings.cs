using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PGA.Sv.PostAudit
{
    public interface ISettings
    {
        DataContext.Settings GetSettings();
        DataContext.Settings GetSettings(DateTime dt);
    }
}
