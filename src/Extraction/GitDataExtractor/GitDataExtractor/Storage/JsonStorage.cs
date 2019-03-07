using System.Collections.Generic;
using System.IO;
using GitDataExtractor.Abstraction;
using GitDataExtractor.Model;
using Newtonsoft.Json;

namespace GitDataExtractor.Storage
{
    internal class JsonStorage : IDataStorage
    {
        public void Save(IEnumerable<Commit> history) => File.WriteAllText(Configuration.Instance.CommitHistoryFilePath, JsonConvert.SerializeObject(history, Formatting.None));

        public IList<Commit> Read() => JsonConvert.DeserializeObject<List<Commit>>(File.ReadAllText(Configuration.Instance.CommitHistoryFilePath));
    }
}
