using System;

namespace Cache.Benchmarks.Model
{
    public class CacheStatistic
    {
        public int Hits { get; set; }

        public int Misses { get; set; }

        public double HitRatio => Hits / ((double)(Hits + Misses));
    }
}
