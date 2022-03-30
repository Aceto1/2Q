using Cache.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cache.LRUK
{
    /// <summary>
    /// This implementation never evicts elements from the history control block since benchmarks in our case never last longer than ~30 seconds and the feature was therefore not needed.
    /// </summary>
    /// <typeparam name="P"></typeparam>
    public class LRUK<P> : ICache<P> where P : class
    {
        private readonly HashSet<P> buffer;

        private readonly Hashtable lastAccessTimes = new Hashtable();

        private readonly Hashtable accessTimeHistories = new Hashtable();

        private readonly int capacity;
        private readonly int k;
        private readonly double crp;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="k"></param>
        /// <param name="crp"></param>
        public LRUK(int capacity, int k, double crp)
        {
            buffer = new HashSet<P>(capacity);
            this.capacity = capacity;
            this.k = k;
            this.crp = crp * TimeSpan.TicksPerSecond;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public bool Access(P page)
        {
            var t = DateTime.UtcNow.Ticks;

            if(buffer.Contains(page)) {
                if(t - (long)lastAccessTimes[page] > crp)
                {
                    var accessHistory = (long[])accessTimeHistories[page];

                    var correlationPeriod = (long)lastAccessTimes[page] - accessHistory[0];

                    for (int i = 1; i < k; i++)
                    {
                        accessHistory[i] = accessHistory[i - 1] + correlationPeriod;
                    }

                    accessHistory[0] = t;
                    accessTimeHistories[page] = accessHistory;
                    lastAccessTimes[page] = t;
                }
                else
                {
                    lastAccessTimes[page] = t;
                }

                return true;
            }
            else
            {
                if(buffer.Count == capacity)
                {
                    var min = t;
                    var victim = buffer.First();

                    foreach (var bufferPage in buffer)
                    {       
                           // if eligible for replacement
                        if(t - (long)lastAccessTimes[bufferPage] > crp &&
                            // and max backwards K-Distance so far
                           ((long[])accessTimeHistories[bufferPage])[k - 1] < min)
                        {
                            victim = bufferPage;
                            min = ((long[])accessTimeHistories[bufferPage])[k - 1];
                        }
                    }

                    buffer.Remove(victim);
                }

                buffer.Add(page);

                if(accessTimeHistories.ContainsKey(page))
                {
                    var accessTimeHistory = (long[])accessTimeHistories[page];
                    for(int i = 1; i < k; i++)
                    {
                        accessTimeHistory[i] = accessTimeHistory[i - 1];
                    }
                }
                else
                {
                    accessTimeHistories.Add(page, new long[k]);
                }

                ((long[])accessTimeHistories[page])[0] = t;
                lastAccessTimes[page] = t;

                return false;
            }
        }
    }
}
