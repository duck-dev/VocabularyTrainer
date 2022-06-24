using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using VocabularyTrainer.UtilityCollection;

namespace VocabularyTrainer.Models;

public static class DataManager
{
    private static readonly string _filePath = Path.Combine(Utilities.FilesParentPath, "Lessons.json");
        
    internal static List<Lesson> Lessons { get; private set; } = new();

    internal static void LoadData()
    {
        if (!File.Exists(_filePath))
            return;

        string content = File.ReadAllText(_filePath);
        var deserializedList = JsonSerializer.Deserialize<List<Lesson>>(content);
        if(deserializedList is not null)
            Lessons = deserializedList;
    }

    internal static void SaveData()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(Lessons, options);
        File.WriteAllText(_filePath, jsonString);
    }

    internal static void AddData(Lesson lesson)
    {
        Lessons.Add(lesson);
        SaveData();
    }
}