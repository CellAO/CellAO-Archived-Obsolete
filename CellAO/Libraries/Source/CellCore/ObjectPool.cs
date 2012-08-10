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
using System.Diagnostics.CodeAnalysis;

#pragma warning disable 1573

namespace Cell.Core
{
    /// <summary>
    /// A structure that contains information about an object pool.
    /// </summary>
    public struct ObjectPoolInfo
    {
        /// <summary>
        /// The number of hard references contained in the pool.
        /// </summary>
        public int m_numHardRef;

        /// <summary>
        /// The number of weak references contained in the pool.
        /// </summary>
        public int m_numWeakRef;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="weak">The number of weak references in the pool.</param>
        /// <param name="hard">The number of hard references in the pool.</param>
        public ObjectPoolInfo(int weak, int hard)
        {
            m_numHardRef = hard;
            m_numWeakRef = weak;
        }
    } ;

    /// <summary>
    /// This class represents a pool of objects.
    /// </summary>
    public class ObjectPool<T> : IObjectPool
    {
        /// <summary>
        /// Delegate holds pointers to functions for allocating new pool objects.
        /// </summary>
        public delegate T ObjCreateFunc();

        /// <summary>
        /// Delegate holds pointers to functions for cleaning up reclaimed objects.
        /// </summary>
        public delegate void ObjCleanFunc(T obj);

        /// <summary>
        /// A queue of objects in the pool.
        /// </summary>
		private SynchronizedQueue<object> m_queue = new SynchronizedQueue<object>();

        /// <summary>
        /// The minimum # of hard references that must be in the pool.
        /// </summary>
        private int m_minSize = 25;

        /// <summary>
        /// The number of hard references in the queue.
        /// </summary>
        private int m_hardRef = 0;

        /// <summary>
        /// Function pointer to the allocation function.
        /// </summary>
        private ObjCreateFunc m_createObj;

        /// <summary>
        /// Function pointer to the allocation function.
        /// </summary>
        private ObjCleanFunc m_cleanObj;

        /// <summary>
        /// Gets the number of hard references that are currently in the pool.
        /// </summary>
        public int NumInPool
        {
            get { return m_hardRef; }
        }

        /// <summary>
        /// Gets the minimum size of the pool.
        /// </summary>
        public int MinimumSize
        {
            get { return m_minSize; }
            set { m_minSize = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="func">Function pointer to the allocation function.</param>
        public ObjectPool(ObjCreateFunc func)
        {
            m_createObj = func;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="func">Function pointer to the allocation function.</param>
        public ObjectPool(ObjCreateFunc func, ObjCleanFunc cleanFunc)
        {
            m_createObj = func;
            m_cleanObj = cleanFunc;
        }

        /// <summary>
        /// Adds an object to the queue.
        /// </summary>
        /// <param name="obj">The object to be added.</param>
        /// <remarks>If there are at least <see cref="ObjectPool&lt;T&gt;.MinimumSize"/> hard references in the pool then the object is added as a WeakReference.
        /// A WeakReference allows an object to be collected by the GC if there are no other hard references to it.</remarks>
        public void Enqueue(T obj) 
        {
            if (m_hardRef >= m_minSize)
            {
				if (m_cleanObj != null)
				{
					m_cleanObj(obj);
				}

                m_queue.Enqueue(new WeakReference(obj));
            }
            else
            {
                if (m_cleanObj != null)
                {
                    m_cleanObj(obj);
                }

                m_queue.Enqueue(obj);
                ++m_hardRef;
            }
        }

        /// <summary>
        /// Adds an object to the queue.
        /// </summary>
        /// <param name="obj">The object to be added.</param>
        /// <remarks>If there are at least <see cref="ObjectPool&lt;T&gt;.MinimumSize"/> hard references in the pool then the object is added as a WeakReference.
        /// A WeakReference allows an object to be collected by the GC if there are no other hard references to it.</remarks>
        public void Enqueue(object obj)
        {
            if (obj is T)
            {
                if (m_hardRef >= m_minSize)
                {
                    m_queue.Enqueue(new WeakReference(obj));
                }
                else
                {
                    m_queue.Enqueue(obj);
                    ++m_hardRef;
                }
            }
        }

#pragma warning disable 0693
        /// <summary>
        /// Removes an object from the queue.
        /// </summary>
        /// <returns>An object from the queue or a new object if none were in the queue.</returns>
        
        public T Dequeue<T>() where T : class
        {
            DequeueObj:
            {
                object obj;
                obj = m_queue.Dequeue();

                if (obj is WeakReference)
                {
                    WeakReference robj = (WeakReference) obj;
                    if (robj.IsAlive)
                    {
                        return robj.Target as T;
                    }
                    else
                    {
                        goto DequeueObj;
                    }
                }
                else
                {
                    if (obj == null)
                    {
                        return m_createObj() as T;
                    }
                    else
                    {
                        --m_hardRef;
                        return obj as T;
                    }
                }
            }
        }
#pragma warning restore 0693

        /// <summary>
        /// Removes an object from the queue.
        /// </summary>
        /// <returns>An object from the queue or a new object if none were in the queue.</returns>
        public object DequeueObj()
        {
            object obj;

            try
            {
                DequeueObj:
                {
                    obj = m_queue.Dequeue();

                    WeakReference robj = obj as WeakReference;
                    if (robj != null)
                    {
                        if (robj.IsAlive)
                        {
                            return robj.Target;
                        }
                        else
                        {
                            goto DequeueObj;
                        }
                    }
                    else
                    {
                        --m_hardRef;
                    }
                }
            }
            catch (Exception)
            {
                obj = m_createObj();
            }

            return obj;
        }

        /// <summary>
        /// Gets information about the object pool.
        /// </summary>
        /// <value>A new <see cref="ObjectPoolInfo"/> object that contains information about the pool.</value>
        public ObjectPoolInfo Info
        {
            get
            {
                ObjectPoolInfo info;
                info.m_numHardRef = m_hardRef;
                info.m_numWeakRef = m_queue.Count - m_hardRef;
                return info;
            }
        }
    }
}