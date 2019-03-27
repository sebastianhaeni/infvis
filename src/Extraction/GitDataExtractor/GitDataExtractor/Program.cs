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
        // If you look at this code to assess my coding skill:
        // This should go to config! Even more, the switch case blow which uses the _byInterval boolean
        // is just ugly. It results in code duplication (same goes for the two Execute-Methods in the HistoryAggregator).
        // In reality I would solve this either pragmatic by extracting functions, by using polymorphism or functional programming.
        private static TimeSpan _interval = TimeSpan.FromDays(30);
        private const int _slices = 10;
        private const bool _byInterval = true;

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
                        if (_byInterval)
                        {
                            new HistoryAggregator(storage, storage).Execute(_interval);
                        }
                        else
                        {
                            new HistoryAggregator(storage, storage).Execute(_slices);
                        }
                        break;

                    case 1:
                        new GitHistoryExtractor(storage).Run();
                        break;

                    case 2:
                        if (_byInterval)
                        {
                            new HistoryAggregator(storage, storage).Execute(_interval);
                        }
                        else
                        {
                            new HistoryAggregator(storage, storage).Execute(_slices);
                        }
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
