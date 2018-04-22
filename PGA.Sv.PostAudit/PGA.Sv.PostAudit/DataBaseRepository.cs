using System;

namespace PGA.Sv.PostAudit
{
    internal class DataBaseRepository : IDatabase
    {

        ILogger _logger = new LoggerRepository();

        public DataBaseRepository()
        {
            //Initialize Here

        }


        public object GetSettings(DateTime dt)
        {
            throw new NotImplementedException();
        }
    }
}