using System;
using System.Collections.Generic;
using GitDataExtractor.Miner.Abstraction;

namespace GitDataExtractor.Model
{
    public class Commit
    {
        private readonly IList<CommitFile> commitFiles = new List<CommitFile>();

        public IEnumerable<CommitFile> Files => commitFiles;

        public string Message { get; set; }

        public int LinesAdded { get; set; }

        public int LinesDeleted { get; set; }

        public DateTime Date { get; set; }

        public void AddFile(string filePath, int linesAdded, int linesDeleted, string status)
        {
            commitFiles.Add(new CommitFile
            {
                FilePath = filePath,
                LinesAdded = linesAdded,
                LinesDeleted = linesDeleted,
                Status = status
            });
        }
    }
}
