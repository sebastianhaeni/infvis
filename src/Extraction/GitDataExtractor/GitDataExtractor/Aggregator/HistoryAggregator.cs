using System;
using System.Collections.Generic;
using System.Linq;
using GitDataExtractor.Miner.Abstraction;
using GitDataExtractor.Models.Aggregation;
using GitDataExtractor.Models.Mining;

namespace GitDataExtractor.Aggregator
{
    public class HistoryAggregator
    {
        private static TimeSpan _interval = TimeSpan.FromDays(1);

        private readonly IHistoryStorage _historyStorage;

        private readonly IAggregateStorage _aggregateStorage;

        public HistoryAggregator(IHistoryStorage historyStorage, IAggregateStorage aggregateStorage)
        {
            _historyStorage = historyStorage;
            _aggregateStorage = aggregateStorage;
        }

        public void Execute()
        {
            // Original, the newest commit comes first but it should be reverse.
            IEnumerable<Commit> history = _historyStorage.Read().Reverse();

            // The last commit contains the history start date.
            DateTime startTime = history.First().Date;

            var commitsGroupedByInterval = new IntervalGroupedCommitCollection(startTime, _interval, history);

            LinkedList<HashTableIntervalGroup<File>> intervalGroups = GetFilesPerInterval(commitsGroupedByInterval);

            CalculateLinesOfCodePerFile(intervalGroups);

            _aggregateStorage.Save(intervalGroups);
        }

        private static void CalculateLinesOfCodePerFile(LinkedList<HashTableIntervalGroup<File>> intervalGroups)
        {
            IDictionary<string, int> fileLinesOfCode = new Dictionary<string, int>();

            HashTableIntervalGroup<File> lastIntervalGroup = intervalGroups.First();
            foreach (HashTableIntervalGroup<File> intervalGroup in intervalGroups.Skip(1))
            {
                foreach (File file in intervalGroup.Elements.Values)
                {
                    if (!fileLinesOfCode.ContainsKey(file.FilePath))
                    {
                        fileLinesOfCode.Add(file.FilePath, file.RelativeLinesDelta);
                    }
                    else
                    {
                        fileLinesOfCode[file.FilePath] = fileLinesOfCode[file.FilePath] + file.RelativeLinesDelta;
                    }

                    file.LinesOfCode = fileLinesOfCode[file.FilePath];
                }

                lastIntervalGroup = intervalGroup;
            }
        }

        private LinkedList<HashTableIntervalGroup<File>> GetFilesPerInterval(IntervalGroupedCommitCollection commitsGroupedByInterval)
        {
            var intervalGroups = new LinkedList<HashTableIntervalGroup<File>>();

            foreach (LinkedIntervalGroup<Commit> commitGroup in commitsGroupedByInterval)
            {
                var filesPerInterval = new HashTableIntervalGroup<File>();
                intervalGroups.AddLast(filesPerInterval);
                filesPerInterval.StartTime = commitGroup.StartTime;
                filesPerInterval.EndTime = commitGroup.EndTime;

                foreach (Commit commit in commitGroup.Elements)
                {
                    foreach (CommitFile file in commit.Files)
                    {
                        if (!IncludeFile(file.FilePath))
                        {
                            continue;
                        }

                        if (!filesPerInterval.Elements.ContainsKey(file.FilePath))
                        {
                            filesPerInterval.Elements.Add(file.FilePath, new File { FilePath = file.FilePath });
                        }

                        filesPerInterval.Elements[file.FilePath].LinesAdded += file.LinesAdded;
                        filesPerInterval.Elements[file.FilePath].LinesDeleted += file.LinesDeleted;
                    }
                }
            }

            return intervalGroups;
        }

        private bool IncludeFile(string fullName) => System.IO.Path.GetExtension(fullName) == ".cs";
    }
}
