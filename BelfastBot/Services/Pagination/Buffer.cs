using System;
using System.Collections.Generic;
using System.Text;

namespace BelfastBot.Services.Pagination
{
    class Buffer<T> where T : struct
    {
        public int Capacity { get; private set; }
        public uint UsedSpots => m_index;

        public bool IsFull => UsedSpots >= Capacity;

        private uint m_index;
        private readonly T[] m_arr;

        public void Insert(T item)
        {
#if DEBUG
            if (m_index + 1 > Capacity)
                throw new OutOfMemoryException("Buffer ran out of memory when insert, please free stuff");
#endif
            m_arr[m_index++] = item;
        }

        /// <summary>
        /// Tries looking for an element that satisfies parameter <paramref name="f"/> backwards. Do not call if empty
        /// </summary>
        /// <param name="f">condition for find</param>
        /// <returns>Returns an element that satisfies parameter <paramref name="f"/>. if none was found it returns null</returns>
        public T? TryFindBackwards(Func<T, bool> f)
        {
            for(uint i = m_index - 1; i >= 0; i--)
            {
                T element = m_arr[i];
                if (f(element))
                    return element;
            }
            return null;
        }

        public void Free()
        {
#if DEBUG
            if (m_index < 0)
                throw new IndexOutOfRangeException("Free when there are 0 elements in the buffer is not allowed");
#endif
            m_index--;
        }

        public Buffer(int capacity)
        {
            Capacity = capacity;
            m_arr = new T[capacity];
        }
    }
}
