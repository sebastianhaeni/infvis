using System;
using System.IO;

namespace GitDataExtractor
{
    public static class Logger
    {
        public static void Write(string message)
        {
            Console.WriteLine(message);
            File.AppendAllText(Configuration.LogFilePath, Environment.NewLine + message);
        }
    }
}
