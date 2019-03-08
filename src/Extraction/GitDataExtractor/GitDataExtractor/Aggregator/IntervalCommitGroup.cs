using System;
using System.Collections.Generic;
using GitDataExtractor.Model;

namespace GitDataExtractor.Aggregator
{
    public class IntervalCommitGroup : LinkedList<Commit>
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
