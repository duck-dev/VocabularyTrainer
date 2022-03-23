using System;
using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.Models.ItemStyleControls;

namespace VocabularyTrainer.Models
{
    public class Word : VocabularyItem
    {
        public Word()
        {
            ThesaurusTitleDefinitions = new[]
            {
                new Tuple<string, string, ItemStyleBase<Word>>("Synonyms:", "Add Synonym", new SynonymStyle(this)),
                new Tuple<string, string, ItemStyleBase<Word>>("Antonyms:", "Add Antonym", new AntonymStyle(this))
            };
            RemoveCommand = ReactiveCommand.Create<IVocabularyContainer<Word>>(Remove);
        }
        
        internal ObservableCollection<VocabularyItem> Synonyms { get; } = new();
        internal ObservableCollection<VocabularyItem> Antonyms { get; } = new();

        private Tuple<string, string, ItemStyleBase<Word>>[] ThesaurusTitleDefinitions { get; }

        private ReactiveCommand<IVocabularyContainer<Word>, Unit> RemoveCommand { get; }

        private void Remove(IVocabularyContainer<Word> parent) 
            => parent.VocabularyItems.Remove(this);

        private void AddThesaurusItem(ItemStyleBase<Word> type)
        {
            switch (type)
            {
                case SynonymStyle:
                    Synonyms.Add(new VocabularyItem(Synonyms));
                    break;
                case AntonymStyle:
                    Antonyms.Add(new VocabularyItem(Antonyms));
                    break;
            }
        }
    }
}