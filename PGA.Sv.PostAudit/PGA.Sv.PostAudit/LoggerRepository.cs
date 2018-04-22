using System;
using PGA.DataContext;
using PGA.Database;
using COMS = PGA.MessengerManager.MessengerManager;

namespace PGA.Sv.PostAudit
{
    internal class LoggerRepository:ILogger
    {
        public LoggerRepository()
        {
        }

        public bool AddLog(string message)
        {
            try
            {
                COMS.AddLog(message);
            }
            catch (Exception )
            {
                throw;
            }
            return true;
        }

        public bool LogException(Exception ex)
        {
            try
            {
                COMS.LogException(ex);
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        public bool LogException(string message, Exception ex)
        {
            try
            {
                COMS.LogException(ex);
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }
    }
}