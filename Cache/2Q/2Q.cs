using Cache.Common;
using System.Collections.Generic;

namespace Cache._2Q
{
    /// <summary>
    /// This class implements the full 2Q caching algorithm using queues backed by a hashtable
    /// for constant time lookup.
    /// </summary>
    public class _2Q<P> : ICache<P> where P : class
    {
        private readonly int Kin;
        private readonly int Kout;
        private readonly int Km;

        private readonly HashSet<P> AmHashset;
        private readonly Queue<P> Am;

        private readonly Queue<P> A1in;
        private readonly HashSet<P> A1inHashset;

        private readonly Queue<P> A1out;
        private readonly HashSet<P> A1outHashset;

        /// <summary>
        /// Creates a new 2Q cache with the specified capacity limit.
        /// </summary>
        /// <param name="amSize">How many elements should be cached at most.</param>
        public _2Q(int amSize, int inSize, int outSize)
        {
            Kin = inSize;
            Km = amSize;
            Kout = outSize;

            AmHashset = new HashSet<P>(Km);
            Am = new Queue<P>(Km);

            A1in = new Queue<P>(Kin);
            A1inHashset = new HashSet<P>(Kin);

            A1out = new Queue<P>(Kout);
            A1outHashset = new HashSet<P>(Kout);
        }

        /// <summary>
        /// Access the page and returns whether it was in the cache
        /// </summary>
        /// <param name="page">The page that is being accessed</param>
        /// <returns>Whether or not the page was in the cache</returns>
        public bool Access(P page)
        {
            if (AmHashset.Contains(page))
            {
                // Cache hit, move item to head of Am queue

                Am.MoveItemToHead(page);
                return true;
            }
            else if (A1outHashset.Contains(page))
            {
                // Cache miss, item was in A1out, load item into Am since it is a valid hot page

                if (Am.Count >= Km)
                {
                    // Free a page slot in Am

                    var removedItem = Am.Dequeue();
                    AmHashset.Remove(removedItem);
                }

                AmHashset.Add(page);
                Am.Enqueue(page);

                return false;
            }
            else if (A1inHashset.Contains(page))
            {
                // Cache hit, Do nothing

                return true;
            }
            else
            {
                // Cache miss, item wasn't in any queue, load into A1

                if (A1in.Count >= Kin)
                {
                    // A1in is full, tail out the last element of A1in and add it to A1out

                    var movingItem = A1in.Dequeue();
                    A1inHashset.Remove(movingItem);

                    if (A1out.Count >= Kout)
                    {
                        // Free a page slot in A1out, discard the item since it was not accessed in a while

                        var removedItem = A1out.Dequeue();
                        A1outHashset.Remove(removedItem);
                    }

                    A1out.Enqueue(movingItem);
                    A1outHashset.Add(movingItem);
                }

                A1in.Enqueue(page);
                A1inHashset.Add(page);

                return false;
            }
        }
    }
}
