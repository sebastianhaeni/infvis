using System.Collections.Generic;
using GitDataExtractor.Model;

namespace GitDataExtractor.Miner.Abstraction
{
    public interface IDataStorage
    {
        void Save(IEnumerable<Commit> history);
    }
}
