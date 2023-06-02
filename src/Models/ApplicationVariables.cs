using System;

namespace VocabularyTrainer.Models;

internal static class ApplicationVariables
{
    private static readonly string _documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    internal static string RecentDownloadLocation { get; set; } = _documentsPath;
    internal static string RecentUploadLocation { get; set; } = _documentsPath;
}