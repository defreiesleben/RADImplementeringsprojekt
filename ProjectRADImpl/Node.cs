using System;
using System.Collections.Generic;

namespace RADImplementationProject
{
    public class Node<T>
    {
        public ulong Key;
        public IIncrementable<T> Data;
        public Node<T> Next;

        public Node(ulong key, IIncrementable<T> v)
        {
            Key = key;
            Data = v;
            Next = null;
        }
    }
}
