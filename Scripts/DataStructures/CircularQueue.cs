using System.Collections.Generic;

namespace Tools.DataStructures
{ 
    public class CircularQueue<T>
    {
        private readonly Queue<T> queue;

        public int Count 
        {
            get
            {
                return queue.Count;
            }
        }

        public int Capacity { get; private set; }

        public CircularQueue(int capacity)
        {
            Capacity = capacity;
            queue = new Queue<T>(capacity);
        }

        public CircularQueue<T> Clone()
        {
            CircularQueue<T> clone = new CircularQueue<T>(Capacity);
            clone.AddRange(new Queue<T>(queue));
            return clone;
        }

        public void Add(T element)
        {
            if (Capacity > 0)
            { 
                if (IsFull())
                {
                    queue.Dequeue();
                }

                queue.Enqueue(element);
            }
        }

        public void AddRange(IEnumerable<T> values)
        {
            foreach (T element in values)
            {
                Add(element);
            }
        }

        public T[] ToArray()
        {
            return queue.ToArray();
        }

        public CircularQueue<T> Copy()
        {
            CircularQueue<T> copy = new CircularQueue<T>(Capacity);
            copy.AddRange(queue);

            return copy;
        }

        public bool IsFull()
        {
            return queue.Count == Capacity;
        }
    }
}
