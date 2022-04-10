using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.UtilityCollection;

namespace VocabularyTrainer.Models
{
    public class Lesson : IVocabularyContainer<Word>
    {
        private string? _name;
        private string? _description;
        private readonly List<Word> _changedWords = new(); 
        
        [JsonConstructor]
        public Lesson(string name, string description, ObservableCollection<Word> vocabularyItems)
        {
            this.Name = name;
            this.Description = description;
            this.VocabularyItems = new ObservableCollection<Word>(vocabularyItems);
            VocabularyItems.CollectionChanged += (sender, args) =>
                Utilities.AddChangedItems(_changedWords, args);
        }
        
        public string? Name
        {
            get => _name; 
            private set => this.ChangedName = _name = value;
        }
        public string? Description
        {
            get => _description; 
            private set => this.ChangedDescription = _description = value;
        }
        public ObservableCollection<Word> VocabularyItems { get; }
        
        internal string? ChangedName { get; private set; }
        internal string? ChangedDescription { get; private set; }

        internal void SaveChanges()
        {
            this.Name = ChangedName;
            this.Description = ChangedDescription;
            foreach (var word in VocabularyItems)
                word.SaveChanges();
            _changedWords.Clear();
        }
    }
}