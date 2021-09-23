using System;
using System.Collections.Generic;

namespace Simulation
{
    internal sealed class SortedLinkedList<T>
    {
        private LinkedList<T> _list;
        private Func<T, T, bool> _comparer;

        internal int Count => _list.Count;
        internal LinkedListNode<T> First => _list.First;
        internal LinkedListNode<T> Last => _list.Last;

        internal LinkedListNode<T> this[int index]
        {
            get
            {
                if (index >= _list.Count || index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));

                var current = _list.First;

                for (var i = 0; i < index; i++)
                    current = current.Next;

                return current;
            }
        }

        internal SortedLinkedList(Func<T, T, bool> comparer)
        {
            _list = new LinkedList<T>();
            _comparer = comparer;
        }

        internal void Add(T item)
        {
            var current = _list.First;

            while (current != null)
            {
                var shouldAdd = _comparer(current.Value, item);

                if (shouldAdd)
                {
                    _list.AddBefore(current, item);

                    return;
                }

                current = current.Next;
            }

            _list.AddLast(item);
        }

        internal void Remove(int index)
        {
            var node = this[index];

            _list.Remove(node);
        }

        internal void Remove(LinkedListNode<T> node)
        {
            _list.Remove(node);
        }

        internal void Remove(T value)
        {
            _list.Remove(value);
        }

        internal void Clear()
        {
            _list.Clear();
        }

        internal bool Contains(T value)
        {
            return _list.Contains(value);
        }
    }
} 