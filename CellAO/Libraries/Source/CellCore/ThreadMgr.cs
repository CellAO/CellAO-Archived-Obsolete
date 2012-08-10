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
using System.Collections;
using System.Threading;
using NLog;
using System.Collections.Generic;

#pragma warning disable 419

namespace Cell.Core
{
    /// <summary>
    /// This class manages the application thread pool.
    /// <seealso cref="IExecObj"/>
    /// </summary>
    /// <remarks>
    /// This class manages the queueing and execution of execution objects. If <see cref="ThreadMgr"/> is running with single-threading disabled, execution objects
    /// will be executed asynchronously and should be thread-safe. If not single-threaded <see cref="ThreadMgr"/> uses a system managed thread pool that scales according
    /// to the number of CPU's and the amount of data to be processed. See <see cref="System.Threading.ThreadPool"/> for more information.
    /// </remarks>
    public static class ThreadMgr
    {
        /// <summary>
        /// A delegate for the function that will handle an object after it has been executed.
        /// </summary>
        /// <param name="obj">The object that has been executed.</param>
        public delegate void ObjectExecuted(object obj);

        /// <summary>
        /// The delegate that holds the function for handling objects that have been executed.
        /// </summary>
        private static ObjectExecuted m_objExec;

        /// <summary>
        /// Thread for single-threaded execution.
        /// </summary>
        private static Thread m_thread;

        /// <summary>
        /// True if the application is to run with only 1 thread.
        /// </summary>
        private static bool m_singleThread;

        /// <summary>
        /// True if the <see cref="ThreadMgr"/> has been started.
        /// <seealso cref="ThreadMgr.Start"/>
        /// </summary>
        private static bool m_running;

        /// <summary>
        /// Queue of execution objects to be executed in a single-threaded enviroment.
        /// <seealso cref="IExecObj"/>
        /// </summary>
        private static List<IExecObj> m_queue = new List<IExecObj>();

        /// <summary>
        /// Spin wait lock for execution object queue synchronization
        /// </summary>
        private static SpinWaitLock m_queueSpinLock = new SpinWaitLock();

        /// <summary>
        /// Sets the post object execution handler.
        /// </summary>
        /// <value>The function to be called after an execution object has been executed.</value>
        public static ObjectExecuted PostObjectExecutionHandler
        {
            set { m_objExec = value; }
        }

        /// <summary>
        /// Gets/Sets the threading state of the application.
        /// <seealso cref="ThreadMgr.SwitchThreadModel"/>
        /// </summary>
        public static bool SingleThreaded
        {
            get { return m_singleThread; }
            set
            {
                if (value != m_singleThread)
                {
                    SwitchThreadModel();
                }
            }
        }

        /// <summary>
        /// Gets the current state of the <see cref="ThreadMgr"/>.
        /// </summary>
        public static bool Running
        {
            get { return m_running; }
        }

        /// <summary>
        /// Switches the current application thread model.
        /// </summary>
        private static void SwitchThreadModel()
        {
            if (!m_running)
            {
                return;
            }

            if (m_singleThread)
            {
                m_thread.Abort();
                m_thread = null;
                m_singleThread = false;
            }
            else
            {
                m_thread = new Thread(ThreadMain);
                m_thread.Start();
                m_singleThread = true;
            }
        }

        /// <summary>
        /// Called when running in the single-threaded execution model.
        /// </summary>
        private static void ThreadMain()
        {
            try
            {
                while (m_running)
                {
                    try
                    {
                        PurgeQueue();
                    }
                    catch (Exception ex)
                    {
                        LogManager.GetLogger(CellDef.CORE_LOG_FNAME).ErrorException("", ex);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                PurgeQueue();
            }
            catch (Exception e)
            {
                LogManager.GetLogger(CellDef.CORE_LOG_FNAME).ErrorException("", e);
            }
        }

        /// <summary>
        /// Purges the internal queue of execution objects.
        /// <seealso cref="IExecObj"/>
        /// </summary>
        private static void PurgeQueue()
        {
            IExecObj[] execObjs;

            m_queueSpinLock.Enter();

            try
            {
                execObjs = m_queue.ToArray();
                m_queue.Clear();
            }
            finally
            {
                m_queueSpinLock.Exit();
            }

            foreach (IExecObj obj in execObjs)
            {
                try
                {
                    obj.Execute();

                    if (m_objExec != null)
                    {
                        m_objExec(obj);
                    }
                }
                catch (Exception e)
                {
                    LogManager.GetLogger(CellDef.CORE_LOG_FNAME).ErrorException("", e);
                }
            }
        }

        /// <summary>
        /// Starts thread execution and <see cref="IExecObj"/> queueing.
        /// </summary>
        public static void Start()
        {
            m_objExec = null;
            m_running = true;

            if (m_singleThread)
            {
                m_thread = new Thread(new ThreadStart(ThreadMain));
                m_thread.Start();
            }
        }

        /// <summary>
        /// Starts thread execution and <see cref="IExecObj"/> queueing.
        /// </summary>
        public static void Start(ObjectExecuted postObjExecFunc)
        {
            m_objExec = postObjExecFunc;
            m_running = true;

            if (m_singleThread)
            {
                m_thread = new Thread(new ThreadStart(ThreadMain));
                m_thread.Start();
            }
        }

        /// <summary>
        /// Stops thread execution and purges the internal queue (<see cref="ThreadMgr.PurgeQueue"/>).
        /// </summary>
        public static void Stop()
        {
            m_running = false;

            PurgeQueue();

            if (m_singleThread)
            {
                m_thread.Abort();
                m_thread = null;
            }
        }

        /// <summary>
        /// Queue's an <see cref="IExecObj"/> for execution.
        /// <seealso cref="IExecObj"/>
        /// </summary>
        /// <param name="obj">The object to be executed</param>
        /// <returns>True if the object was successfully queued.</returns>
        /// <remarks>
        /// If the <see cref="ThreadMgr"/> hasn't been started yet or has been stopped the return value will be false. Otherwise it's always true.
        /// </remarks>
        public static bool QueueExecObj(IExecObj obj)
        {
            if (m_running)
            {
                if (m_singleThread)
                {
                    m_queueSpinLock.Enter();

                    try
                    {
                        m_queue.Add(obj);
                    }
                    finally
                    {
                        m_queueSpinLock.Exit();
                    }
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(PoolMain), obj);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Asynchronously executes an execution object.
        /// <seealso cref="IExecObj"/>
        /// <seealso cref="ThreadMgr.QueueExecObj"/>
        /// </summary>
        /// <param name="state">The object to be executed.</param>
        private static void PoolMain(object state)
        {
            IExecObj obj = (IExecObj)state;

            try
            {
                obj.Execute();

                if (m_objExec != null)
                {
                    m_objExec(obj);
                }
            }
            catch (Exception e)
            {
                LogManager.GetLogger(CellDef.CORE_LOG_FNAME).ErrorException("", e);

                Console.WriteLine("Error: " + e.ToString());
            }
        }
    }
}
