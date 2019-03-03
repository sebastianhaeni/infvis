namespace GitDataExtractor
{
    public static class Statistics
    {
        public static uint ProcessedCommits { get; set; }

        public static uint ProcessedFiles { get; set; }

        public static void Save()
        {
            Logger.Write(" ");
            Logger.Write(nameof(ProcessedCommits) + ": " + ProcessedCommits);
            Logger.Write(nameof(ProcessedFiles) + ": " + ProcessedFiles);
        }
    }
}
