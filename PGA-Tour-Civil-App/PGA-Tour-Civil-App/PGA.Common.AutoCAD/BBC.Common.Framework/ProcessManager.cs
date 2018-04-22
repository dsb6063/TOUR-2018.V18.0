using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Pge.Common.Framework
{
    public static class ProcessMananger
    {
        /// <summary>
        /// Determines if a process is running by looking at the filename of process.
        /// </summary>
        /// <param name="name">The filename of the name process</param>
        /// <returns>true if the process is found, false otherwise.</returns>
        public static bool FindProcessByName(string processName, string moduleName)
        {
            foreach (Process process in Process.GetProcessesByName(processName))
            {
                string fileName = process.MainModule.FileName;

                if (moduleName == fileName)
                {
                    return true;
                }
            }
           
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="killAll"></param>
        public static void KillProcesses(string name)
        {
            Process[] processes = Process.GetProcesses();

            foreach( Process p in processes)
            {
                if (p.MainModule.FileName.ToUpper().Contains(name.ToUpper()))
                {
                    p.Kill();
                }
            }
        }

        /// <summary>
        /// Launch a program
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="arguments"></param>
        /// <param name="workingDirectory"></param>
        /// <param name="showError"></param>
        /// <returns></returns>
        public static bool LaunchProcess(string fileName, string arguments, string workingDirectory, bool showError)
        {
            Process process = new Process();
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.ErrorDialog = showError;
            process.StartInfo.WorkingDirectory = workingDirectory;
            return process.Start();
        }
    }
}
