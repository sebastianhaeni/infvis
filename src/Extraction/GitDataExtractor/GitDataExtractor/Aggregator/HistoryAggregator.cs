using System;
using System.Collections.Generic;
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
            IList<Commit> history = _storage.Read();

            // The last commit contains the history start date.
            DateTime startTime = history[history.Count - 1].Date;
            DateTime takeUntil = startTime + interval;

            var commitQueue = new Queue<Commit>(history);

            while (commitQueue.Count > 0)
            {
                var commit = commitQueue.Dequeue();
                if(commit.Date >= takeUntil)
                {
                    // TODO: Create bin.
                    // Option 1: Files first and then timeseries in it.
                    // Option 2: Timeseries first and then files in it.
                    IDictionary<string, CommitFile> files = new Dictionary<string, CommitFile>();
                    foreach (var file in commit.Files)
                    {
                        if (!files.ContainsKey(file.FilePath))
                        {
                            files.Add(file.FilePath, new CommitFile { FilePath = file.FilePath });
                        }

                        files[file.FilePath].LinesAdded += file.LinesAdded;
                        files[file.FilePath].LinesDeleted += file.LinesDeleted;
                    }
                }
                else
                {
                    takeUntil += interval;
                }
            }
        }
    }
}
