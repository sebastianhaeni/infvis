using System;
using System.Collections.Generic;
using GitDataExtractor.Model;

namespace GitDataExtractor.Aggregator
{
    internal class IntervalGroupedCommitCollection : LinkedList<IntervalCommitGroup>
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
            AddLast(new IntervalCommitGroup());

            DateTime takeUntil = _startTime + _interval;

            var commitQueue = new Queue<Commit>(_history);
            while (commitQueue.TryDequeue(out Commit commit))
            {
                // The current commit isn't in the current time range anymore.
                // As the commits are ordered, a new bag must be started.
                if (commit.Date > takeUntil)
                {
                    AddLast(new IntervalCommitGroup());
                    Last.Value.StartTime = takeUntil;
                    takeUntil += _interval;
                    Last.Value.EndTime = takeUntil;
                }

                Last.Value.AddLast(commit);
            }
        }
    }
}
