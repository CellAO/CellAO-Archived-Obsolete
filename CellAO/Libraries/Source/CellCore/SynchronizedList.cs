using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

#pragma warning disable 1591

namespace Cell.Core
{
	/// <summary>
	/// Syncronized List{T} based on SynchronizedDictionary provided by Nomad
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SynchronizedList<T> : List<T>
	{
		private readonly ReaderWriterLockSlim m_rwLock = new ReaderWriterLockSlim();
		private volatile int m_userLock = 0;

		public SynchronizedList() : base() { }
		public SynchronizedList(int capacity) : base(capacity) { }
		public SynchronizedList(IEnumerable<T> collection) : base(collection) { }

		public new T this[int index]
		{
			get
			{
				if (index > Count)
					throw new ArgumentOutOfRangeException("index");

				if (m_userLock != 0 && m_userLock == Thread.CurrentThread.ManagedThreadId)
				{
					return base[index];
				}

				T result;
				m_rwLock.EnterReadLock();

				try
				{
					result = base[index];
				}
				finally
				{
					m_rwLock.ExitReadLock();
				}

				return result;
			}
			set
			{
				if (index > Count)
					throw new ArgumentOutOfRangeException("index");

				if (m_userLock != 0 && m_userLock == Thread.CurrentThread.ManagedThreadId)
				{
					base[index] = value;
					return;
				}

				m_rwLock.EnterWriteLock();

				try
				{
					base[index] = value;
				}
				finally
				{
					m_rwLock.ExitWriteLock();
				}
			}
		}

		public new void Add(T value)
		{
			if (m_userLock != 0 && m_userLock == Thread.CurrentThread.ManagedThreadId)
			{
				base.Add(value);
				return;
			}

			m_rwLock.EnterWriteLock();

			try
			{
				base.Add(value);
			}
			finally
			{
				m_rwLock.ExitWriteLock();
			}
		}

		public new bool Remove(T value)
		{
			if (m_userLock != 0 && m_userLock == Thread.CurrentThread.ManagedThreadId)
			{
				return base.Remove(value);
			}

			m_rwLock.EnterWriteLock();

			try
			{
				return base.Remove(value);
			}
			finally
			{
				m_rwLock.ExitWriteLock();
			}
		}

		public new void RemoveAt(int index)
		{
			if (index > Count)
				throw new ArgumentOutOfRangeException("index");

			if (m_userLock != 0 && m_userLock == Thread.CurrentThread.ManagedThreadId)
			{
				base.RemoveAt(index);
				return;
			}

			m_rwLock.EnterWriteLock();

			try
			{
				base.RemoveAt(index);
			}
			finally
			{
				m_rwLock.ExitWriteLock();
			}
		}

		public new void Clear()
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

		public new bool Contains(T item)
		{
			if (m_userLock != 0 && m_userLock == Thread.CurrentThread.ManagedThreadId)
			{
				return base.Contains(item);
			}

			m_rwLock.EnterReadLock();

			try
			{
				return base.Contains(item);
			}
			finally
			{
				m_rwLock.ExitReadLock();
			}
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
