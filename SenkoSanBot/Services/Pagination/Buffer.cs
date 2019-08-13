using System;
using System.Collections.Generic;
using System.Text;

namespace SenkoSanBot.Services.Pagination
{
    class Buffer<T> : Queue<T>
    {
        public int Capacity { get; private set; } = 0;

        public Buffer(int capacity) : base(capacity)
        {
            Capacity = capacity;
        }
    }
}
