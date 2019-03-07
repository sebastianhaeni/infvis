using System;
using System.Collections.Generic;
using GitDataExtractor.Abstraction;
using GitDataExtractor.Model;

namespace GitDataExtractor.Miner
{
    public class GitHistoryExtractor
    {
        private int _counter;

        private readonly IDataStorage _storage;

        public GitHistoryExtractor(IDataStorage storage)
        {
            _storage = storage;
        }

        public void Run()
        {
            IEnumerable<CommitAggregate> commitAggregates = new GitClient().Get();

            IList<Commit> history = new List<Commit>();

            foreach (CommitAggregate commitAggregate in commitAggregates)
            {
                _counter++;
                Console.WriteLine(_counter);
                if (!SkipCommit(commitAggregate))
                {
                    Commit commit = ProcessCommit(commitAggregate);
                    history.Add(commit);
                }
            }

            _storage.Save(history);
        }

        private Commit ProcessCommit(CommitAggregate commitAggregate)
        {
            var commit = new Commit();
            commit.Message = commitAggregate.Commit.MessageShort;
            commit.LinesAdded = commitAggregate.Changes.LinesAdded;
            commit.LinesDeleted = commitAggregate.Changes.LinesDeleted;
            commit.Date = commitAggregate.Commit.Committer.When.UtcDateTime;

            foreach (LibGit2Sharp.PatchEntryChanges patchEntryChanges in commitAggregate.Changes)
            {
                commit.AddFile(patchEntryChanges.Path, patchEntryChanges.LinesAdded, patchEntryChanges.LinesDeleted, patchEntryChanges.Status.ToString("G"));
            }

            return commit;
        }

        private static bool SkipCommit(CommitAggregate commitAggregate)
        {
            // This commit is empty. E.g. a merge commit.
            return commitAggregate.Changes.LinesAdded == 0 && commitAggregate.Changes.LinesDeleted == 0;
        }
    }
}
