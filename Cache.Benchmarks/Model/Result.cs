using Cache.Benchmarks.Enum;
using System.Collections.Generic;

namespace Cache.Benchmarks.Model
{
    public class Result
    {
        public int PageSlots { get; set; }

        public int PageCount { get; set; }

        public double PageSlotPercentage => PageSlots / ((double)PageCount);

        public Dictionary<Algorithm, CacheStatistic> Stats { get; set; }
    }
}
