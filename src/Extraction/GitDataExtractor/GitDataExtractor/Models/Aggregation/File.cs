namespace GitDataExtractor.Models.Aggregation
{
    public class File
    {
        public string FilePath { get; set; }

        public int AbsoluteLinesDelta => LinesAdded + LinesDeleted;

        public int RelativeLinesDelta => LinesAdded - LinesDeleted;

        public int LinesOfCode { get; set; }

        public int LinesAdded { get; set; }

        public int LinesDeleted { get; set; }
    }
}
