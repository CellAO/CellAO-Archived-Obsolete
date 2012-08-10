/* Copyright (C) 2004-2007, Alexei Garbuzenko. All rights reserved.
 *
 * File:    SynchronizedDictionary.cs
 * Desc:    Common Utilities
 * Version: 0.223
 * Author:  Alexei Garbuzenko (Nomad)
 *
 * This file is part of the RunServer Game Server Platform
 *
 * Your use and or redistribution of this software in source and / or
 * binary form, with or without modification, is subject to your
 * ongoing acceptance of and compliance with the terms and conditions of
 * agreement with RunServer Platform owner and development team
 */

using System;
using System.Collections.Generic;
using System.Threading;

#pragma warning disable 1591

namespace Cell.Core
{
    public class SynchronizedDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        private readonly ReaderWriterLockSlim m_rwLock = new ReaderWriterLockSlim();
        private volatile int m_userLock = 0;

		public SynchronizedDictionary() { }
		public SynchronizedDictionary(int capacity) : base(capacity) { }
		public SynchronizedDictionary(IDictionary<TKey, TValue> dictionary)  : base(dictionary) { }

        public new TValue this[TKey key]
        {
            get
            {
				if (m_userLock != 0 && m_userLock == Thread.CurrentThread.ManagedThreadId)
				{
					return base[key];
				}

                m_rwLock.EnterReadLock();

				try
				{
					if (!base.ContainsKey(key))
					{
						throw new KeyNotFoundException();
					}

					return base[key];
				}
				finally
				{
					m_rwLock.ExitReadLock();
				}
            }
            set
            {
                if (m_userLock != 0 && m_userLock == Thread.CurrentThread.ManagedThreadId)
                {
                    base[key] = value;
                    return;
                }

				bool upgraded = true;
                m_rwLock.EnterUpgradeableReadLock();

				try
				{
					if (base.ContainsKey(key))
					{
						m_rwLock.EnterReadLock();
						m_rwLock.ExitUpgradeableReadLock();
						upgraded = false;

						try
						{
							base[key] = value;
						}
						finally
						{
							m_rwLock.ExitReadLock();
						}
					}
					else
					{
						m_rwLock.EnterWriteLock();

						try
						{
							base.Add(key, value);
						}
						finally
						{
							m_rwLock.ExitWriteLock();
						}
					}
				}
				finally
				{
					if (upgraded)
					{
						m_rwLock.ExitUpgradeableReadLock();
					}
				}
            }
        }

        public virtual new void Add(TKey key, TValue value)
        {
            if (m_userLock != 0 && m_userLock == Thread.CurrentThread.ManagedThreadId)
            {
                base.Add(key, value);
                return;
            }

            m_rwLock.EnterWriteLock();

			try
			{
				base.Add(key, value);
			}
			finally
			{
				m_rwLock.ExitWriteLock();
			}
        }

		public virtual new void Clear()
        {
            if (m_userLock != 0 && m_userLock == Thread.CurrentThread.ManagedThreadId)
            {
                base.Clear();
                return;
            }

            m_rwLock.EnterWriteLock();

			try
			{
				base.Clear();
			}
			finally
			{
				m_rwLock.ExitWriteLock();
			}
        }

        public new bool ContainsKey(TKey key)
        {
			if (m_userLock != 0 && m_userLock == Thread.CurrentThread.ManagedThreadId)
			{
				return base.ContainsKey(key);
			}

            m_rwLock.EnterReadLock();

			try
			{
				return base.ContainsKey(key);
			}
			finally
			{
				m_rwLock.ExitReadLock();
			}
        }

		public virtual new bool Remove(TKey key)
        {
			if (m_userLock != 0 && m_userLock == Thread.CurrentThread.ManagedThreadId)
			{
				return base.Remove(key);
			}

			m_rwLock.EnterUpgradeableReadLock();

			try
			{
				if (!base.ContainsKey(key)) {
					return false;
				}
				m_rwLock.EnterWriteLock();

				try {
					base.Remove(key);
				}
				finally {
					m_rwLock.ExitWriteLock();
				}
			}
			finally
			{
				m_rwLock.ExitUpgradeableReadLock();
			}
			return true;
        }

        public void EnterWriteLock()
        {
            m_rwLock.EnterWriteLock();
            m_userLock = Thread.CurrentThread.ManagedThreadId;
        }

        public void ExitWriteLock()
        {
            m_userLock = 0;
            m_rwLock.ExitWriteLock();
        }

        public void EnterReadLock()
        {
            m_rwLock.EnterReadLock();
            m_userLock = Thread.CurrentThread.ManagedThreadId;
        }

        public void ExitReadLock()
        {
            m_userLock = 0;
            m_rwLock.ExitReadLock();
        }
    }
}