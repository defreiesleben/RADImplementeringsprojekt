using System;
using System.Collections.Generic;

namespace RADImplementationProject
{
    public class HashTableChaining
    {
        public HashTableChaining(int _l)
        {
            ulong l = 1UL << _l;

            // node

            Node[] nodeList = new Node[l];
        }

        // Function for inserting datat in the front of the singly-linked list
        // The first node, head, will be null when the linked list is instantiated.
        // When we want to add any node at the front, we want the head to point to it.
        // We will create a new node. The next of the new node will point to the head of the Linked list.
        // The previous Head node is now the second node of Linked List because the new node is added at the front.
        // We will assign head to the new node.
        public void InsertFront(SingleLinkedList sList, int new_data)
        {
            Node new_node = new Node(new_data);
            new_node.next = sList.head;
            sList.head = new_node;
        }


        // Function for inserting datat in the end of the singly-linked list
        // If the Linked List is empty, then we simply add the new node as the Head of the Linked List.
        // If the Linked List is not empty, then we find the last node and make next of the last node to the new node.
        // new node is the last node now.

        public Node GetLastNode(SingleLinkedList sList)
        {
            Node temp = sList.head;

            while (temp.next != null)
            {
                temp = temp.next;
            }

            return temp;
        }

        public void InsertLast(SingleLinkedList sList, int new_data)
        {
            Node new_node = new Node(new_data);
            if (sList.head == null)
            {
                sList.head = new_node;
                return;
            }
            Node lastNode = GetLastNode(sList);
            lastNode.next = new_node;
        }


        // TROR VI SKAL HAVE ET FOREACH LOOP, NÅR VI GÅR IGENNEM NODELIST[L]
        // Find the node having the key value.
        public void get(SingleLinkedList sList, int key)
        {
            Node temp = sList.head;

            // er nedenstående redundant?
            if (temp == null)
            {
                return;
            }

            while (temp != null && temp.data != key)
            {
                temp = temp.next;
            }

            if (temp != null && temp.data == key)
            { 
                return;
            }

        }

        // Set a given node x to a valiue v
        // Skal vi køre hele listen igennem for at vide, x ikke er der?
        public void set(SingleLinkedList sList, int key, int v)
        {
            Node temp = sList.head;

            // Hvis x ikke er i vores liste, skal vi tilføje x med værdien v
            if (temp == null)
            {
                return;
            }

            // Iterate over items in the Node
            while (temp != null && temp.data != key)
            {
                temp = temp.next;
            }

            if (temp != null && temp.data == key)
            {
                // Here the key x is set to the value v
                temp.data = v;
            }
        }

        // Increment the value of x with the value d
        public void increment(SingleLinkedList sList, int key, int d)
        {
            Node temp = sList.head;

            // Hvis x ikke er i vores liste, skal vi tilføje x med værdien d
            if (temp == null)
            {
                return;
            }

            // Iterate over items in the Node
            while (temp != null && temp.data != key)
            {
                temp = temp.next;
            }

            if (temp != null && temp.data == key)
            {
                // Here the key x is incremented with the value d
                temp.data += d;
            }
        }
    }
}
