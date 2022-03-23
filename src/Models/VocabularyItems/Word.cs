using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Runtime.CompilerServices;
using ReactiveUI;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.Models.ItemStyleControls;

namespace VocabularyTrainer.Models
{
    public class Word : DualVocabularyItem, INotifyPropertyChanged
    {
        private int _index;
        
        public event PropertyChangedEventHandler? PropertyChanged;
        
        public Word()
        {
            ThesaurusTitleDefinitions = new[]
            {
                new Tuple<string, string, ItemStyleBase<Word>>("Synonyms:", "Add Synonym", new SynonymStyle(this)),
                new Tuple<string, string, ItemStyleBase<Word>>("Antonyms:", "Add Antonym", new AntonymStyle(this))
            };
            RemoveCommand = ReactiveCommand.Create<IVocabularyContainer<Word>>(Remove);
        }

        internal ObservableCollection<SingleVocabularyItem> Synonyms { get; } = new();
        internal ObservableCollection<SingleVocabularyItem> Antonyms { get; } = new();

        internal int Index
        {
            get => _index + 1;
            set
            {
                if (_index == value)
                    return;
                _index = value;
                NotifyPropertyChanged();
            }
        }

        private Tuple<string, string, ItemStyleBase<Word>>[] ThesaurusTitleDefinitions { get; }

        private ReactiveCommand<IVocabularyContainer<Word>, Unit> RemoveCommand { get; }

        private void Remove(IVocabularyContainer<Word> parent) 
            => parent.VocabularyItems.Remove(this);

        private void AddThesaurusItem(ItemStyleBase<Word> type)
        {
            switch (type)
            {
                case SynonymStyle:
                    Synonyms.Add(new SingleVocabularyItem(Synonyms));
                    break;
                case AntonymStyle:
                    Antonyms.Add(new SingleVocabularyItem(Antonyms));
                    break;
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}