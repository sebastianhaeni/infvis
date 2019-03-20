using System;
using System.Collections.Generic;
using GitDataExtractor.Models.Mining;

namespace GitDataExtractor.Aggregator
{
    internal class IntervalGroupedCommitCollection : LinkedList<LinkedIntervalGroup<Commit>>
    {
        private readonly DateTime _startTime;
        private readonly TimeSpan _interval;
        private readonly IEnumerable<Commit> _history;

        public IntervalGroupedCommitCollection(DateTime startTime, TimeSpan interval, IEnumerable<Commit> history)
        {
            _startTime = startTime;
            _interval = interval;
            _history = history;

            Initialize();
        }

        private void Initialize()
        {
            DateTime takeUntil = _startTime + _interval;
            AddLast(new LinkedIntervalGroup<Commit>());
            Last.Value.StartTime = _startTime;
            Last.Value.EndTime = takeUntil;

            var commitQueue = new Queue<Commit>(_history);
            while (commitQueue.TryDequeue(out Commit commit))
            {
                // The current commit isn't in the current time range anymore.
                // As the commits are ordered, a new bag must be started.
                if (commit.Date > takeUntil)
                {
                    AddLast(new LinkedIntervalGroup<Commit>());
                    Last.Value.StartTime = takeUntil;
                    takeUntil += _interval;
                    Last.Value.EndTime = takeUntil;
                }

                Last.Value.Elements.AddLast(commit);
            }
        }
    }
}
