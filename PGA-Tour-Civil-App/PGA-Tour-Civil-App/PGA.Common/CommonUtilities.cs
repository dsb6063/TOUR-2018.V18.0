#region

using System;

#endregion

namespace PGA.Common
{
    public static class CommonUtilities
    {
        public static string GetCode(string code)
        {
            try
            {
                var res = code.Split(':')[0];
                return res ?? "";
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.AddLog(ex.Message);
                return "";
            }
        }

        public static string GetLayer(string code)
        {
            try
            {
                var res = code.Split(':')[1];
                return res ?? "";
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.AddLog(ex.Message);
                return "";
            }
        }
    }
}