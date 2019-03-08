using System;
using System.Collections.Generic;
using System.Linq;
using GitDataExtractor.Abstraction;
using GitDataExtractor.Model;

namespace GitDataExtractor.Aggregator
{
    public class HistoryAggregator
    {
        private static TimeSpan interval = TimeSpan.FromDays(30);

        private readonly IDataStorage _storage;

        public HistoryAggregator(IDataStorage storage) => _storage = storage;

        public void Execute()
        {
            // Original, the newest commit comes first but it should be reverse.
            IEnumerable<Commit> history = _storage.Read().Reverse();

            // The last commit contains the history start date.
            DateTime startTime = history.First().Date;

            var commitsGroupedByInterval = new IntervalGroupedCommitCollection(startTime, interval, history);
        }
    }
}
