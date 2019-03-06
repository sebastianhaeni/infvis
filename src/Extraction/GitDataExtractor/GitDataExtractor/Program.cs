using System;
using System.IO;
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

            var extractor = new GitHistoryExtractor(new JsonStorage());
            extractor.Run();
        }
    }
}
