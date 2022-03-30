using System.Collections.Generic;

namespace Cache.Common
{
    public static class QueueUtils
    {
        /// <summary>
        /// Move an item somewhere in a queue back to the end of said queue.
        /// This function dequeues every item and checks whether it is the item to look for.
        /// If it is not, the item is enqueued again otherwise its reference is saved and it is enqueued as the last item. 
        /// </summary>
        /// <typeparam name="P">The type of the queue elements</typeparam>
        /// <param name="queue">The queue to search</param>
        /// <param name="element">The element to move to the back of the queue</param>
        public static void MoveItemToHead<P>(this Queue<P> queue, P element) where P : class
        {
            P itemToMove = null;

            var count = queue.Count;

            for (int i = 0; i < count; i++)
            {
                var item = queue.Dequeue();

                if (itemToMove == null && item.Equals(element))
                    itemToMove = item;
                else
                    queue.Enqueue(item);
            }

            if (itemToMove != null)
                queue.Enqueue(itemToMove);
        }
    }
}
