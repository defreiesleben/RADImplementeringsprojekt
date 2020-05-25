using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RADImplementationProject
{
    public class HashTableChaining<T>
    {
        private ulong count = 0;
        private Node<T>[] nodeList = null;
        private Func<ulong, ulong> h = null;

        /// <summary>
        /// Create a hashtable with the given hashfunction, 
        /// </summary>
        /// <param name="hashfunction">The hashfunction, see HashFunction namespace</param>
        /// <param name="size">The max value the hash function maps to</param>
        public HashTableChaining(Func<ulong, ulong> hashfunction, ulong size)
        {
            nodeList = new Node<T>[size];
            h = hashfunction;
        }

        // Find the node having the key value.
        public T get(ulong key)
        {
            ulong hash = h(key);
            Node<T> head = nodeList[hash];

            if (head == null)
                return default(T);

            while (head.Key != key)
            {
                if (head.Next != null)
                    head = head.Next;
                else
                    return default(T);
            }
            return head.Data.GetValue();
        }

        // Set a given node x to a valiue v
        public void set(ulong key, IIncrementable<T> v)
        {
            ulong hash = h(key);
            Node<T> head = nodeList[hash];

            if (head == null)
            {
                nodeList[hash] = new Node<T>(key, v);
                count++;
                return;
            }

            while (head.Key != key && head.Next != null)
                head = head.Next;

            if (head.Key == key)
                head.Data = v;
            else
            {
                head.Next = new Node<T>(key, v);
                count++;
            }
        }

        // Increment the value of x with the value d
        public void increment(ulong key, IIncrementable<T> v)
        {
            ulong hash = h(key);
            Node<T> head = nodeList[hash];

            if (head == null) {
                nodeList[hash] = new Node<T>(key, v);
                count++;
                return;
            }

            while (head.Key != key && head.Next != null)
                head = head.Next;

            if (head.Key == key)
                head.Data.Increment(v.GetValue());
            else
            {
                head.Next = new Node<T>(key, v);
                count++;
            }
        }

        public ulong Count => count;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for(long i = 0; i < nodeList.LongLength; i++)
            {
                if (nodeList[i] == null)
                    sb.Append("[").Append(i).AppendLine("] = {}");
                else {
                    sb.Append("[").Append(i).Append("] = {");
                    Node<T> current = nodeList[i];
                    while (current != null)
                    {
                        sb.Append(current.Key).Append(":").Append(current.Data.GetValue()).Append(", ");
                        current = current.Next;
                    }
                    sb.Remove(sb.Length - 2, 2).AppendLine("}");
                }
            }
            return sb.ToString();
        }

        public Node<T>[] NodeList { get { return nodeList; } }
    }
}
