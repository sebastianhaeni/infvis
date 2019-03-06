using System;
using System.Collections.Generic;
using GitDataExtractor.Model;
using LibGit2Sharp;

namespace GitDataExtractor.Miner
{
    public class GitClient : IDisposable
    {
        private readonly Repository _repo = new Repository(Configuration.Instance.RepositoryDirectory);

        public void Dispose() => _repo.Dispose();

        public IEnumerable<CommitAggregate> Get()
        {
            ICommitLog commitLog = _repo.Commits.QueryBy(new CommitFilter());

            IEnumerator<LibGit2Sharp.Commit> commitEnumerator = commitLog.GetEnumerator();
            commitEnumerator.MoveNext();
            LibGit2Sharp.Commit previousCommit = commitEnumerator.Current;

            while (commitEnumerator.MoveNext())
            {
                // As the enumerator reads the history top down, the current enumerator value
                // is the older commit. Therefore, the previousCommit is the newer commit.
                Tree olderCommitTree = commitEnumerator.Current.Tree;
                Tree newerCommmitTree = previousCommit.Tree;
                Patch patch = _repo.Diff.Compare<Patch>(olderCommitTree, newerCommmitTree);
                previousCommit = commitEnumerator.Current;

                yield return new CommitAggregate
                {
                    Commit = commitEnumerator.Current,
                    Changes = patch
                };
            }
        }
    }
}
