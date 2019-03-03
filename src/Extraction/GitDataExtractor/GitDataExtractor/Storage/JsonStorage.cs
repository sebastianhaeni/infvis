using System.Collections.Generic;
using System.IO;
using GitDataExtractor.Miner.Abstraction;
using GitDataExtractor.Model;
using Newtonsoft.Json;

namespace GitDataExtractor.Storage
{
    internal class JsonStorage : IDataStorage
    {
        public void Save(IEnumerable<Commit> history) => File.WriteAllText(Configuration.CommitHistoryFilePath, JsonConvert.SerializeObject(history, Formatting.None));
    }
}
