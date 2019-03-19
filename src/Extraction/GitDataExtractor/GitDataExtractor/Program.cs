using System;
using System.IO;
using GitDataExtractor.Aggregator;
using GitDataExtractor.Miner;
using GitDataExtractor.Storage;
using Microsoft.Extensions.Configuration;

namespace GitDataExtractor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.override.json", true);

            IConfigurationRoot configurationRoot = builder.Build();

            configurationRoot.Bind(Configuration.Instance);

            var storage = new JsonStorage();

            int toExecute = GetValidExecutionOption();
            switch (toExecute)
            {
                case 1:
                    var extractor = new GitHistoryExtractor(storage);
                    extractor.Run();
                    break;

                case 2:
                    var aggregator = new HistoryAggregator(storage, storage);
                    aggregator.Execute();
                    break;
            }
        }

        private static int GetValidExecutionOption()
        {
            while (true)
            {
                Console.WriteLine("[1] Run Extractor...");
                Console.WriteLine("[2] Run Aggregator...");
                if (int.TryParse(Console.ReadLine(), out int result) && (result == 1 || result == 2))
                {
                    return result;
                }

                Console.Clear();
            }
        }
    }
}
