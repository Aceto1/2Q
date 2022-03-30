using Cache.Common;
using System.Collections.Generic;

namespace Cache.LRU
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="P"></typeparam>
    public class LRU<P> : ICache<P> where P : class
    {
        private readonly Queue<P> queue;
        private readonly HashSet<P> hashset;
        private readonly int capacity;

        public LRU(int capacity)
        {
            queue = new Queue<P>(capacity);
            hashset = new HashSet<P>(capacity);
            this.capacity = capacity;
        }

        public bool Access(P page)
        {
            if (hashset.Contains(page))
            {
                queue.MoveItemToHead(page);
                return true;
            }
            else
            {
                if (queue.Count >= capacity)
                {
                    var removedItem = queue.Dequeue();
                    hashset.Remove(removedItem);
                }

                queue.Enqueue(page);
                hashset.Add(page);

                return false;
            }
        }
    }
}
