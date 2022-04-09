using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
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

            Synonyms.CollectionChanged += (sender, args) =>
                NotifyPropertyChanged(nameof(SynonymsEmpty));
            Antonyms.CollectionChanged += (sender, args) =>
                NotifyPropertyChanged(nameof(AntonymsEmpty));
        }

        [JsonConstructor]
        public Word(ObservableCollection<VocabularyItem> synonyms, ObservableCollection<VocabularyItem> antonyms) : this()
        {
            this.Synonyms = synonyms;
            this.Antonyms = antonyms;
            
            foreach (var item in Synonyms)
                item.ContainerCollection = this.Synonyms;
            foreach (var item in Antonyms)
                item.ContainerCollection = this.Antonyms;
        }

        public ObservableCollection<VocabularyItem> Synonyms { get; } = new();
        public ObservableCollection<VocabularyItem> Antonyms { get; } = new();

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

        private bool SynonymsEmpty => Synonyms.Count <= 0;
        private bool AntonymsEmpty => Antonyms.Count <= 0;

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

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}