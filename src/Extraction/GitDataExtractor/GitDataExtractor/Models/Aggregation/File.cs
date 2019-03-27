using Newtonsoft.Json;

namespace GitDataExtractor.Models.Aggregation
{
    public class File
    {
        public string FilePath { get; set; }

        [JsonIgnore]
        public int AbsoluteLinesDelta => LinesAdded + LinesDeleted;

        public int RelativeLinesDelta => LinesAdded - LinesDeleted;

        public int LinesOfCode { get; set; }

        [JsonIgnore]
        public int LinesAdded { get; set; }

        [JsonIgnore]
        public int LinesDeleted { get; set; }
    }
}
