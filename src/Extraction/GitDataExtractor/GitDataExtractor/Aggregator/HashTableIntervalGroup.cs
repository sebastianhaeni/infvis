using System;
using System.Collections.Generic;

namespace GitDataExtractor.Aggregator
{
    public class HashTableIntervalGroup<T>
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public Dictionary<string, T> Elements { get; set; } = new Dictionary<string, T>();
    }
}
