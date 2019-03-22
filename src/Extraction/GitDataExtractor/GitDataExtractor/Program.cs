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

            ExecutionLoop(storage);
        }

        private static void ExecutionLoop(JsonStorage storage)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler((sender, args) => Environment.Exit(0));
            while (true)
            {
                int toExecute = GetValidExecutionOption();
                switch (toExecute)
                {
                    case 0:
                        new GitHistoryExtractor(storage).Run();
                        new HistoryAggregator(storage, storage).Execute();
                        break;

                    case 1:
                        new GitHistoryExtractor(storage).Run();
                        break;

                    case 2:
                        new HistoryAggregator(storage, storage).Execute();
                        break;
                }
            }
        }

        private static int GetValidExecutionOption()
        {
            while (true)
            {
                Console.WriteLine("[0] Run both...");
                Console.WriteLine("[1] Run Extractor...");
                Console.WriteLine("[2] Run Aggregator...");
                if (int.TryParse(Console.ReadLine(), out int result) && (result == 0 || result == 1 || result == 2))
                {
                    return result;
                }

                Console.Clear();
            }
        }
    }
}
