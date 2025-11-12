using System;

namespace MunicipalServicesApp.DataStructures
{
    
    /// Represents a single node in a custom linked list.
    /// dotnet-bot (2025). LinkedList Class (System.Collections.Generic). [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.linkedlist-1?view=net-9.0.‌
    
    public class LinkedListNode<T>
    {
        public T Data { get; set; }
        public LinkedListNode<T> Next { get; set; }

        public LinkedListNode(T data)
        {
            Data = data;
            Next = null;
        }
    }
}