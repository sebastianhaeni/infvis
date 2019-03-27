using System;
using System.Collections.Generic;

namespace GitDataExtractor.Aggregator
{
    public class LinkedIntervalGroup<T>
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public LinkedList<T> Elements { get; set; } = new LinkedList<T>();
    }
}
