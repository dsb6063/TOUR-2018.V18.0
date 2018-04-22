#region

using System;
using System.Windows;
using PGA.Database;

#endregion

namespace PGA.MessengerManager
{
    public static class MessengerManager
    {
        public static void AddLog(string message)
        {
            DatabaseLogs.FormatLogs(message);
        }

        public static void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public static void ShowMessageAndLog(string message)
        {
            MessageBox.Show(message);
            DatabaseLogs.FormatLogs(message);
        }

        public static void ShowMessageAlert(string message)
        {
            MessageBox.Show("Alert! " + message);
            DatabaseLogs.FormatLogs("Alert! " + message);
        }

        public static void ShowMessageAlert(Exception message)
        {
            var target = message.TargetSite;
            var source = message.Source;
            var stack = message.StackTrace;

            MessageBox.Show("Alert! " + message.Message);
            DatabaseLogs.FormatLogs(string.Format("Alert! {0}-{1}-{2}", target, source, stack));
        }

        public static void LogException(Exception message)
        {
            var target = message.TargetSite;
            var source = message.Source;
            var stack = message.StackTrace;

            DatabaseLogs.FormatLogs(string.Format("Alert! {0}-{1}-{2}", target, source, stack));
        }

        public static void LogException(string str, Exception message)
        {
            var target = message.TargetSite;
            var source = message.Source;
            var stack = message.StackTrace;

            DatabaseLogs.FormatLogs(string.Format("Alert! {0}-{1}-{2}", target, source, stack));
        }

        public static void AddLog(string str, Exception message)
        {
            var target = message.TargetSite;
            var source = message.Source;
            var stack = message.StackTrace;

            DatabaseLogs.FormatLogs(string.Format("Alert! {0}-{1}-{2}", target, source, stack));

        }
    }
}