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

        public void Add(T element)
        {
            if (queue.Count == Capacity)
            {
                queue.Dequeue();
            }

            queue.Enqueue(element);
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
    }
}
