using LibGit2Sharp;

namespace GitDataExtractor.Models.Mining
{
    public class CommitAggregate
    {
        public LibGit2Sharp.Commit Commit { get; set; }

        public Patch Changes { get; set; }
    }
}
