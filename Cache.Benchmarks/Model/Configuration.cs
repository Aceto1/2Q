using System.Collections.Generic;

namespace Cache.Benchmarks.Model
{
    public class Configuration
    {
        public List<string> References { get; set; }

        public int PageSlots { get; set; }

        public int PageCount { get; set; }

        public int SampleSize { get; set; }

        public string Description { get; set; }
    }
}
