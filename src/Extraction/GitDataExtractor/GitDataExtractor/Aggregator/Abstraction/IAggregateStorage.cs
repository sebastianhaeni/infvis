using System.Collections.Generic;
using GitDataExtractor.Aggregator;
using GitDataExtractor.Models.Aggregation;

namespace GitDataExtractor.Miner.Abstraction
{
    public interface IAggregateStorage
    {
        void Save(IEnumerable<HashTableIntervalGroup<File>> history);
    }
}
