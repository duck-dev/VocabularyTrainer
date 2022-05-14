using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.Models.ItemStyleControls;
using VocabularyTrainer.UtilityCollection;
#pragma warning disable CS0659

namespace VocabularyTrainer.Models
{
    public class Word : DualVocabularyItem, INotifyPropertyChangedHelper, IIndexable, IContentVerification<Word>, IEquatable<Word>
    {
        private int _index;
        private readonly List<VocabularyItem> _changedSynonyms = new();
        private readonly List<VocabularyItem> _changedAntonyms = new();

        public event PropertyChangedEventHandler? PropertyChanged;
        
        public Word()
        {
            ThesaurusTitleDefinitions = new[]
            {
                new Tuple<string, string, ItemStyleBase<Word>>("Synonyms:", "Add Synonym", new SynonymStyle(this)),
                new Tuple<string, string, ItemStyleBase<Word>>("Antonyms:", "Add Antonym", new AntonymStyle(this))
            };
            RemoveCommand = ReactiveCommand.Create<IVocabularyContainer<Word>>(Remove);
            SubscribeCollectionsChanged();
            
            foreach(LearningModeType value in Enum.GetValues(typeof(LearningModeType)))
                KnownInModes.Add(value, LearningState.NotAsked);
        }

        [JsonConstructor]
        public Word(ObservableCollection<VocabularyItem> synonyms, ObservableCollection<VocabularyItem> antonyms, 
            Dictionary<LearningModeType, LearningState> knownInModes) : this()
        {
            this.Synonyms = synonyms;
            this.Antonyms = antonyms;
            this.KnownInModes = knownInModes;
            
            foreach (var item in Synonyms)
                item.ContainerCollection = this.Synonyms;
            foreach (var item in Antonyms)
                item.ContainerCollection = this.Antonyms;
            
            SubscribeCollectionsChanged(); // Must be called after assigning properties: Synonyms/Antonyms
        }

        public ObservableCollection<VocabularyItem> Synonyms { get; } = new();
        public ObservableCollection<VocabularyItem> Antonyms { get; } = new();
        public Dictionary<LearningModeType, LearningState> KnownInModes { get; } = new();

        [JsonIgnore]
        public int Index
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

        internal bool DataChanged => !ChangedTerm.Equals(Term) || !ChangedDefinition.Equals(Definition) 
                                     || Synonyms.Any(x => !x.ChangedDefinition.Equals(x.Definition)) 
                                     || Antonyms.Any(x => !x.ChangedDefinition.Equals(x.Definition))
                                     || _changedSynonyms.Count > 0 || _changedAntonyms.Count > 0;

        // private bool SynonymsEmpty => Synonyms.Count <= 0;
        // private bool AntonymsEmpty => Antonyms.Count <= 0;

        private Tuple<string, string, ItemStyleBase<Word>>[] ThesaurusTitleDefinitions { get; }

        private ReactiveCommand<IVocabularyContainer<Word>, Unit> RemoveCommand { get; }
        
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "") 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void CheckUnsavedContent()
        {
            foreach(var synonym in this.Synonyms)
                Utilities.CheckUnsavedContent(synonym, _changedSynonyms);
            foreach(var antonym in this.Antonyms)
                Utilities.CheckUnsavedContent(antonym, _changedAntonyms);
        }

        public bool MatchesUnsavedContent(IEnumerable<Word> collection, out Word? identicalItem)
        {
            identicalItem = collection.FirstOrDefault(x => x.ChangedAction == NotifyCollectionChangedAction.Remove
                                                           && x.ChangedDefinition.Equals(this.ChangedDefinition)
                                                           && x.ChangedTerm.Equals(this.ChangedTerm)
                                                           && x.Synonyms.SequenceEqual(this.Synonyms)
                                                           && x.Antonyms.SequenceEqual(this.Antonyms));
            return identicalItem is not null && !ReferenceEquals(identicalItem, this);
        }

        public override void EqualizeChangedData()
        {
            base.EqualizeChangedData();
            
            _changedSynonyms.Clear();
            _changedAntonyms.Clear();
            foreach (var synonym in this.Synonyms)
                synonym.EqualizeChangedData();
            foreach (var antonym in this.Antonyms)
                antonym.EqualizeChangedData();
        }

        public bool Equals(Word? other)
        {
            return other is not null && other.ChangedTerm.Equals(this.ChangedTerm) && other.Term.Equals(this.Term) 
                   && other.ChangedDefinition.Equals(this.ChangedDefinition) && other.Definition.Equals(this.Definition) 
                   && other.Synonyms.SequenceEqual(this.Synonyms) && other.Antonyms.SequenceEqual(this.Antonyms);
        }

        public override bool Equals(object? obj) => Equals(obj as Word);

        protected internal override void SaveChanges()
        {
            base.SaveChanges();
            foreach (var item in Synonyms)
                item.SaveChanges();
            foreach (var item in Antonyms)
                item.SaveChanges();
            _changedSynonyms.Clear();
            _changedAntonyms.Clear();
        }

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

        private void SubscribeCollectionsChanged()
        {
            Synonyms.CollectionChanged += (sender, args) =>
            {
                // NotifyPropertyChanged(nameof(SynonymsEmpty));
                Utilities.AddChangedItems(_changedSynonyms, args);
                if (args.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Remove)
                    NotifyDataChanged();
            };
            Antonyms.CollectionChanged += (sender, args) =>
            {
                // NotifyPropertyChanged(nameof(AntonymsEmpty));
                Utilities.AddChangedItems(_changedAntonyms, args);
                if (args.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Remove)
                    NotifyDataChanged();
            };
        }

        private void NotifyDataChanged()
        {
            var lesson = DataManager.Lessons.FirstOrDefault(x => x.VocabularyItems.Contains(this));
            lesson?.NotifyPropertyChanged(nameof(lesson.DataChanged));
        }
        
        // internal void DebugUnsavedChanges()
        // {
        //     Utilities.Log($"• Word ({this.ChangedTerm} - {this.ChangedDefinition}):\n----------------------------");
        //     foreach (var x in this.Synonyms)
        //         Utilities.Log($"      -S: {x.ChangedDefinition} ({x.Definition})");
        //     foreach(var y in this.Antonyms)
        //         Utilities.Log($"      -A: {y.ChangedDefinition} ({y.Definition})");
        // }
    }
}