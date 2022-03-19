using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VocabularyTrainer.Models.ItemStyleControls;
using VocabularyTrainer.ViewModels;

namespace VocabularyTrainer.Models
{
    public class Word
    {
        public Word()
        {
            ThesaurusTitleDefinitions = new[]
            {
                new Tuple<string, string, ItemStyleBase<Word>>("Synonyms:", "Add Synonym", new SynonymStyle(this)),
                new Tuple<string, string, ItemStyleBase<Word>>("Antonyms:", "Add Antonym", new AntonymStyle(this))
            };
        }
        
        private Tuple<string, string, ItemStyleBase<Word>>[] ThesaurusTitleDefinitions { get; }
        
        internal string? Term { get; set; }
        internal string? Definition { get; set; }
        internal ObservableCollection<KeyValuePair<string,string>> Synonyms { get; } = new();
        internal ObservableCollection<KeyValuePair<string,string>> Antonyms { get; } = new();
        
        internal AddLessonViewModel? ParentLesson { get; init; }

        private void RemoveWord() => ParentLesson?.Words.Remove(this);

        public void AddThesaurusItem(ItemStyleBase<Word> type)
        {
            switch (type)
            {
                case SynonymStyle:
                    Synonyms.Add(new KeyValuePair<string, string>());
                    break;
                case AntonymStyle:
                    Antonyms.Add(new KeyValuePair<string, string>());
                    break;
            }
        }
    }
}