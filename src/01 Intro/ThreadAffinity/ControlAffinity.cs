#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Permissions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

#endregion // Using

namespace ThreadAffinity
{
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlThread)]
    public class ControlAffinity
    {
        #region Win 32

        [DllImport("Kernel32.dll")]
        [SuppressUnmanagedCodeSecurity]
        public static extern int GetCurrentProcessorNumber();

        [DllImport("ntdll.dll", CharSet = CharSet.Auto)]
        public static extern uint NtGetCurrentProcessorNumber();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThread();

        [DllImport("kernel32.dll")]
        private static extern IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);

        #endregion // Win 32

        #region Run

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlThread)]
        public static void Run()
        {
            int initProcNum = GetCurrentProcessorNumber();
            bool hasContextSwitching = false;// Switching

            for (int i = 0; i < 3; i++)
            {
                var sw = Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds < 10)
                {
                    int procNum = GetCurrentProcessorNumber();
                    if (procNum != initProcNum)
                        hasContextSwitching = true;
                }
                sw.Stop();
                //Thread.Sleep(1);
            }
            Console.WriteLine("Context Switching = {0}, Init Core = {1}", hasContextSwitching, initProcNum);
        }

        #endregion // Run

        #region RunWithWin32AffinityMask

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlThread)]
        public static void RunWithWin32AffinityMask()
        {
            int initProcNum = GetCurrentProcessorNumber();
            IntPtr hCurr = GetCurrentThread();
            SetThreadAffinityMask(hCurr, new IntPtr(1 << initProcNum));

            Run(); // run under affinity scope
        }

        #endregion // RunWithWin32AffinityMask

        /*
        * Process Proc = Process.GetCurrentProcess();   
        * long AffinityMask = (long)Proc.ProcessorAffinity;   
        * AffinityMask &= 0x000F; // use only any of the first 4 available processors   
        * Proc.ProcessorAffinity = (IntPtr)AffinityMask;    
        * ProcessThread Thread = Proc.Threads[0];   
        * AffinityMask = 0x0002; // use only the second processor, despite availability   
        * Thread.ProcessorAffinity = (IntPtr)AffinityMask; 
        */
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlThread)]
        public static void SetProcAffinity(params int[] coreMask)
        {
            int coresCount = Environment.ProcessorCount;
            int AffinityMask = 0;
            foreach (var core in coreMask)
            {
                if (coresCount < core)
                    throw new ArgumentException(string.Format("Cannot set core [{0}] on {1} cores machine", core, coresCount));
                AffinityMask |= 1 << core;
            }
            Process Proc = Process.GetCurrentProcess();
            Proc.ProcessorAffinity = (IntPtr)AffinityMask;   
        }
    }
}
