using System.Collections.Generic;
using GitDataExtractor.Models.Mining;

namespace GitDataExtractor.Miner.Abstraction
{
    public interface IHistoryStorage
    {
        void Save(IEnumerable<Commit> history);

        IList<Commit> Read();
    }
}
