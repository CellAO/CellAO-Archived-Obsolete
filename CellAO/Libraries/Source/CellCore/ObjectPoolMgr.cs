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
using System.Collections.Generic;
using System.Threading;

#pragma warning disable 1573

namespace Cell.Core
{
    /// <summary>
    /// This class manages objects in a pool to maximize memory (de)allocation efficiency.
    /// </summary>
    public static class ObjectPoolMgr
    {
        /// <summary>
        /// A list of types of objects the pool contains.
        /// </summary>
        private static Dictionary<string, IObjectPool> m_types = new Dictionary<string, IObjectPool>();

        /// <summary>
        /// Event for synchronizing object access.
        /// </summary>
        private static ManualResetEvent m_event = new ManualResetEvent(true);

        /// <summary>
        /// Returns true if the specified type is registered.
        /// </summary>
        /// <typeparam name="T">The type to check registration with.</typeparam>
        /// <returns>True if the specified type is registered.</returns>
        public static bool ContainsType<T>()
        {
            if (m_event.WaitOne(3000, false))
            {
                return m_types.ContainsKey(typeof (T).ToString());
            }

            return false;
        }

        /// <summary>
        /// Returns true if the specified type is registered.
        /// </summary>
        /// <param name="t">The type to check registration with.</param>
        /// <returns>True if the specified type is registered.</returns>
        public static bool ContainsType(Type t)
        {
            if (m_event.WaitOne(3000, false))
            {
                return m_types.ContainsKey(t.ToString());
            }

            return false;
        }

        /// <summary>
        /// Registers an object pool with the specified type.
        /// </summary>
        /// <param name="func">A pointer to a function that creates new objects.</param>
        /// <returns>True if the type already exists or was registered successfully. False if locking the internal pool list timed out.</returns>
        /// <remarks>The function waits 3000 milliseconds to aquire the lock of the internal pool list.</remarks>
        public static bool RegisterType<T>(ObjectPool<T>.ObjCreateFunc func)
        {
            bool res = false;
            if (m_event.WaitOne(3000, false))
            {
                if (!m_types.ContainsKey(typeof (T).ToString()))
                {
                    m_event.Reset();
                    m_types.Add(typeof (T).ToString(), new ObjectPool<T>(func));
                    m_event.Set();
                    res = true;
                }
                else
                {
                    res = true;
                }
            }

            return res;
        }

        /// <summary>
        /// Registers an object pool with the specified type.
        /// </summary>
        /// <param name="func">A pointer to a function that creates new objects.</param>
        /// <returns>True if the type already exists or was registered successfully. False if locking the internal pool list timed out.</returns>
        /// <remarks>The function waits 3000 milliseconds to aquire the lock of the internal pool list.</remarks>
        public static bool RegisterType<T>(ObjectPool<T>.ObjCreateFunc func, ObjectPool<T>.ObjCleanFunc cleanFunc)
        {
            bool res = false;
            if (m_event.WaitOne(3000, false))
            {
                if (!m_types.ContainsKey(typeof (T).ToString()))
                {
                    m_event.Reset();
                    m_types.Add(typeof (T).ToString(), new ObjectPool<T>(func, cleanFunc));
                    m_event.Set();
                    res = true;
                }
                else
                {
                    res = true;
                }
            }

            return res;
        }

        /// <summary>
        /// Sets the minimum number of hard references to be contained in the specified object pool.
        /// </summary>
        /// <param name="minSize">The minimum number of hard references to be contained in the specified object pool.</param>
        public static void SetMinimumSize<T>(int minSize)
        {
            if (m_event.WaitOne(3000, false))
            {
                if (m_types.ContainsKey(typeof (T).ToString()))
                {
                    ObjectPool<T> pool = (ObjectPool<T>) m_types[typeof (T).ToString()];
                    pool.MinimumSize = minSize;
                }
            }
        }

        /// <summary>
        /// Releases an object back into the object pool.
        /// </summary>
        /// <param name="obj">The object to be released.</param>
        public static void ReleaseObject<T>(T obj)
        {
            if (m_event.WaitOne(3000, false))
            {
                if (m_types.ContainsKey(obj.GetType().ToString()))
                {
                    ObjectPool<T> pool = m_types[typeof (T).ToString()] as ObjectPool<T>;

                    if (pool != null)
                    {
                        pool.Enqueue(obj);
                    }
                }
            }
        }

        /// <summary>
        /// Releases an object back into the object pool.
        /// </summary>
        /// <param name="obj">The object to be released.</param>
        public static void ReleaseObject(object obj)
        {
            if (m_event.WaitOne(3000, false))
            {
                if (m_types.ContainsKey(obj.GetType().ToString()))
                {
                    IObjectPool pool = m_types[obj.GetType().ToString()];

                    if (pool != null)
                    {
                        pool.Enqueue(obj);
                    }
                }
            }
        }

        /// <summary>
        /// Obtains an object from the specified object pool.
        /// </summary>
        /// <returns>If a lock could not be aquired on the object pool null is returned. Otherwise a hard reference to the object requested is returned.</returns>
        public static T ObtainObject<T>() where T : class
        {
            if (m_event.WaitOne(3000, false))
            {
                if (m_types.ContainsKey(typeof (T).ToString()))
                {
                    ObjectPool<T> pool = (ObjectPool<T>) m_types[typeof (T).ToString()];

                    return pool.Dequeue<T>();
                }
            }

            return default(T);
        }

        /// <summary>
        /// Gets information about the specified object pool.
        /// </summary>
        /// <returns>An object of type <see cref="ObjectPoolInfo"/> if the function succeeded, otherwise an object with all values equal to 0 is returned.</returns>
        public static ObjectPoolInfo GetPoolInfo<T>()
        {
            if (m_event.WaitOne(3000, false))
            {
                if (m_types.ContainsKey(typeof (T).ToString()))
                {
                    ObjectPool<T> pool = (ObjectPool<T>) m_types[typeof (T).ToString()];
                    return pool.Info;
                }
            }

            ObjectPoolInfo info;
            info.m_numWeakRef = 0;
            info.m_numHardRef = 0;

            return info;
        }
    }
}