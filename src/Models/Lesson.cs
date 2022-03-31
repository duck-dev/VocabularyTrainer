using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace VocabularyTrainer.Models
{
    public class Lesson
    {
        [JsonConstructor]
        public Lesson(string name, string description, ObservableCollection<Word>? words)
        {
            this.Name = name;
            this.Description = description;
            
            if (words is null)
                return;
            this.Words = new ObservableCollection<Word>(words);
        }
        
        public string Name { get; private set; }
        public string Description { get; private set; }
        public ObservableCollection<Word>? Words { get; } = new();
    }
}