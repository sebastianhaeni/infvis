using System;
using System.Collections.Generic;
using GitDataExtractor.Model;
using LibGit2Sharp;

namespace GitDataExtractor.Miner
{
    public class GitClient : IDisposable
    {
        private readonly Repository _repo = new Repository(Configuration.RepositoryDirectory);

        public void Dispose() => _repo.Dispose();

        public IEnumerable<CommitAggregate> Get()
        {
            ICommitLog commitLog = _repo.Commits.QueryBy(new CommitFilter());

            IEnumerator<LibGit2Sharp.Commit> commitEnumerator = commitLog.GetEnumerator();
            commitEnumerator.MoveNext();
            LibGit2Sharp.Commit previouseCommit = commitEnumerator.Current;

            while (commitEnumerator.MoveNext())
            {
                Tree commitTree = commitEnumerator.Current.Tree; // Main Tree
                Tree parentCommitTree = previouseCommit.Tree; // Secondary Tree
                Patch patch = _repo.Diff.Compare<Patch>(parentCommitTree, commitTree); // Difference
                previouseCommit = commitEnumerator.Current;

                yield return new CommitAggregate
                {
                    Commit = commitEnumerator.Current,
                    Changes = patch
                };
            }
        }
    }
}
