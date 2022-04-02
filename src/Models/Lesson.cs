using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using VocabularyTrainer.Interfaces;

namespace VocabularyTrainer.Models
{
    public class Lesson : IVocabularyContainer<Word>
    {
        [JsonConstructor]
        public Lesson(string name, string description, ObservableCollection<Word> vocabularyItems)
        {
            this.Name = name;
            this.Description = description;
            this.VocabularyItems = new ObservableCollection<Word>(vocabularyItems);
        }
        
        public string Name { get; private set; }
        public string Description { get; private set; }
        public ObservableCollection<Word> VocabularyItems { get; }
    }
}