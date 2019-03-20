using System.Collections.Generic;

public class Configuration
{
    private static readonly Configuration _instance = new Configuration();

    private Configuration()
    { }

    public static Configuration Instance => _instance;

    public string RepositoryDirectory { get; set; }

    public string CommitHistoryFilePath { get; set; }

    public string AggregateFilePath { get; set; }

    public List<string> FileExtensionsToInclude { get; set; }
}
