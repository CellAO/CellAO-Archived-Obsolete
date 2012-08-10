/*************************************************************************
 *                             Configuration.cs
 *                            ------------------
 *   begin                : Jan 24, 2007
 *   copyright            : (C) The WCell Team
 *   email                : info@wcell.org
 *************************************************************************/

/*************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *************************************************************************/

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Cell.Core
{
    /// <summary>
    /// Efficient method for performing thread safety while staying in user-mode.
    /// </summary>
    /// <remarks>
    /// <para>This is a value type so it works very efficiently when used as a field in a class.</para>
    /// <para>Avoid boxing or you will lose thread safety.</para>
    /// <para>This structure is based on Jeffrey Richter's article "Concurrent Affairs" in the October 2005 issue of MSDN Magazine.</para>
    /// </remarks>
    public struct SpinWaitLock
    {
        private const int m_free = 0;
        private const int m_owned = 1;
        private int m_lockState;
        private static readonly bool IsSingleCpuMachine = (Environment.ProcessorCount == 1);

        /// <summary>
        /// Attempts to lock a resource.
        /// </summary>
        public void Enter()
        {
            Thread.BeginCriticalRegion();

            while (true)
            {
                // If resource available, set it to in-use and return
                if (Interlocked.Exchange(ref m_lockState, m_owned) == m_free)
                {
                    return;
                }

                // Efficiently spin, until the resource looks like it might 
                // be free. NOTE: Just reading here (as compared to repeatedly 
                // calling Exchange) improves performance because writing 
                // forces all CPUs to update this value
                while (Thread.VolatileRead(ref m_lockState) == m_owned)
                {
                    StallThread();
                }
            }
        }

        /// <summary>
        /// Releases a resource.
        /// </summary>
        public void Exit()
        {
            // Mark the resource as available
            Interlocked.Exchange(ref m_lockState, m_free);
            Thread.EndCriticalRegion();
        }

#if LINUX
        private static void StallThread()
        {
            //Linux doesn't support SwitchToThread()
            Thread.SpinWait(1);
        }
#else
        private static void StallThread()
        {
            // On a single-CPU system, spinning does no good
            if (IsSingleCpuMachine)
            {
                SwitchToThread();
            }

                // Multi-CPU system might be hyper-threaded, let other thread run
            else
            {
                Thread.SpinWait(1);
            }
        }

        [DllImport("kernel32", ExactSpelling = true)]
        private static extern void SwitchToThread();
#endif
    }
}