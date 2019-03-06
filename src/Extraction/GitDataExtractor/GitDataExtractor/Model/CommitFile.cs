namespace GitDataExtractor.Model
{
    public class CommitFile
    {
        public string FilePath { get; set; }

        public int LinesAdded { get; set; }

        public int LinesDeleted { get; set; }

        public string Status { get; set; }
    }
}
