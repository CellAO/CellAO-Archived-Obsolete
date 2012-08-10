using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

#pragma warning disable 109
#pragma warning disable 414
#pragma warning disable 1591

namespace Cell.Core
{
	public class SynchronizedQueue<T> : Queue<T>
	{
		private readonly object m_objLock = new object();
		private volatile int m_userLock = 0;

		public SynchronizedQueue() : base() { }
		public SynchronizedQueue(int capacity) : base(capacity) { }
		public SynchronizedQueue(IEnumerable<T> collection) : base(collection) { }

		public new int Count
		{
			get
			{
				lock (m_objLock)
				{
					return base.Count;
				}
			}
		}

		public new bool IsSynchronized
		{
			get
			{
				return true;
			}
		}

		public new object SyncRoot
		{
			get
			{
				return m_objLock;
			}
		}

		public new void Clear()
		{
			lock (m_objLock)
			{
				base.Clear();
			}
		}

		public new bool Contains(T obj)
		{
			lock (m_objLock)
			{
				return base.Contains(obj);
			}
		}

		public new void CopyTo(T[] array, int arrayIndex)
		{
			lock (m_objLock)
			{
				base.CopyTo(array, arrayIndex);
			}
		}

		public new T Dequeue()
		{
			lock (m_objLock)
			{
				return base.Dequeue();
			}
		}

		public new void Enqueue(T value)
		{
			lock (m_objLock)
			{
				base.Enqueue(value);
			}
		}

		public new IEnumerator<T> GetEnumerator()
		{
			lock (m_objLock)
			{
				return base.GetEnumerator();
			}
		}

		public new T Peek()
		{
			lock (m_objLock)
			{
				return base.Peek();
			}
		}

		public new T[] ToArray()
		{
			lock (m_objLock)
			{
				return base.ToArray();
			}
		}

		public new void TrimExcess()
		{
			lock (m_objLock)
			{
				base.TrimExcess();
			}
		}
	}
}
