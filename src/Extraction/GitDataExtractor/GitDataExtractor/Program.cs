using System;
using GitDataExtractor.Miner;
using GitDataExtractor.Storage;

namespace GitDataExtractor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var extractor = new GitHistoryExtractor(new JsonStorage());
            extractor.Run();

            Statistics.Save();

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }
    }
}
