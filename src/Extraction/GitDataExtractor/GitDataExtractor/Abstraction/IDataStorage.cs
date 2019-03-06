using System.Collections.Generic;
using GitDataExtractor.Model;

namespace GitDataExtractor.Abstraction
{
    public interface IDataStorage
    {
        void Save(IEnumerable<Commit> history);

        IList<Commit> Read();
    }
}
