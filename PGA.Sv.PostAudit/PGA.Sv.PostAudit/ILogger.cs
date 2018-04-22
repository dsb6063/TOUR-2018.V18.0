using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGA.Sv.PostAudit
{
    public interface  ILogger
    {
        bool LogException(string message, Exception ex);
        bool LogException( Exception ex);
        bool AddLog(string message);
    }
}
