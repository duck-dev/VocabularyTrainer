using System;
using System.IO;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.UtilityCollection;

public static partial class Utilities
{
    internal static void DeleteDirectory(string directory)
    {
        if(Directory.Exists(directory))
            Directory.Delete(directory, true);
    }

    internal static void SaveFile(FileInfo file, string location, bool pathContainsFile, bool setRecentDirectory = true)
    {
        string path;
        string directory;
        if (pathContainsFile)
        {
            path = location;
            directory = Path.GetDirectoryName(location) ?? throw new ArgumentException($"{nameof(location)} refers to a root directory.");
        }
        else
        {
            path = Path.Combine(location, file.Name);
            directory = location;
        }
        
        if (File.Exists(path))
            path = Path.Combine(directory, $"{Path.GetFileNameWithoutExtension(path)} - Copy{Path.GetExtension(path)}");
        
        file.CopyTo(path, false);
        if (setRecentDirectory)
            SetRecentDirectory(location, pathContainsFile);
    }

    private static void SetRecentDirectory(string location, bool pathContainsFile)
    {
        string directory = location;
        if(pathContainsFile)
            directory = Path.GetDirectoryName(location) ?? throw new DirectoryNotFoundException($"{nameof(location)} refers to a root directory.");
        ApplicationVariables.RecentDownloadLocation = directory;
    }
}