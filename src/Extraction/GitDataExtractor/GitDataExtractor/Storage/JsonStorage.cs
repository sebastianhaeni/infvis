using System.Collections.Generic;
using GitDataExtractor.Aggregator;
using GitDataExtractor.Miner.Abstraction;
using GitDataExtractor.Models.Mining;
using Newtonsoft.Json;

namespace GitDataExtractor.Storage
{
    internal class JsonStorage : IAggregateStorage, IHistoryStorage
    {
        public void Save(IEnumerable<Commit> history) => System.IO.File.WriteAllText(Configuration.Instance.CommitHistoryFilePath, JsonConvert.SerializeObject(history, Formatting.None));

        public IList<Commit> Read() => JsonConvert.DeserializeObject<List<Commit>>(System.IO.File.ReadAllText(Configuration.Instance.CommitHistoryFilePath));

        public void Save(IEnumerable<HashTableIntervalGroup<Models.Aggregation.File>> history) => System.IO.File.WriteAllText(Configuration.Instance.AggregateFilePath, JsonConvert.SerializeObject(history, Formatting.None));
    }
}
