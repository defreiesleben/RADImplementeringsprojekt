using System;
namespace RADImplementationProject
{
    public class Node
    {
        internal int data;
        internal Node next;
        public Node(int d)
        {
            data = d;
            next = null;
        }
    }

    // Create a linked list class
    // The SingleLinkedList will contain nodes of type Node class
    // When a new Linked List is instantiated, it just has the head, which is Null
    public class SingleLinkedList
    {
        internal Node head;
    }
}
