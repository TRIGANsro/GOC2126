using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace SpanVsMemory
{
    public class MyMemoryManager : MemoryManager<byte>
    {
        private byte[] _array;

        public MyMemoryManager(int size)
        {
            _array = new byte[size];
        }

        protected override void Dispose(bool disposing)
        {
            
        }

        public override Span<byte> GetSpan() => _array;

        public override MemoryHandle Pin(int elementIndex = 0)
        {
            unsafe
            {
                return new MemoryHandle(
                    (void*)System.Runtime.InteropServices.MemoryMarshal.GetReference(_array.AsSpan(elementIndex)));
            }
        }

        public override void Unpin() { }

        public void Dispose() { }

        protected override bool TryGetArray(out ArraySegment<byte> segment)
        {
            segment = new ArraySegment<byte>(_array);
            return true;
        }
    }
}
