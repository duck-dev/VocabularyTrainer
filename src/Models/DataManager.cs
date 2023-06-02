using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using VocabularyTrainer.UtilityCollection;

namespace VocabularyTrainer.Models;

public static class DataManager
{
    internal static string LessonsFilePath { get; } = Path.Combine(Utilities.FilesParentPath, "Lessons.json");
    internal static bool LessonsFileExists => File.Exists(LessonsFilePath);
    internal static List<Lesson> Lessons { get; private set; } = new();

    internal static void LoadData()
    {
        if (!File.Exists(LessonsFilePath))
            return;

        string content = File.ReadAllText(LessonsFilePath);
        var deserializedList = JsonSerializer.Deserialize<List<Lesson>>(content);
        if(deserializedList is not null)
            Lessons = deserializedList;
    }

    internal static void SaveData()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(Lessons, options);
        File.WriteAllText(LessonsFilePath, jsonString);
    }

    internal static void AddData(Lesson lesson)
    {
        Lessons.Add(lesson);
        SaveData();
    }
}