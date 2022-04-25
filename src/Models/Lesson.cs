using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.UtilityCollection;

namespace VocabularyTrainer.Models
{
    public class Lesson : IVocabularyContainer<Word>, INotifyPropertyChangedHelper
    {
        private string _name;
        private string _description;
        private string _changedName = string.Empty;
        private string _changedDescription = string.Empty;
        private readonly List<Word> _changedWords = new(); 
        
        public event PropertyChangedEventHandler? PropertyChanged;
        
        [JsonConstructor]
        public Lesson(string name, string description, ObservableCollection<Word> vocabularyItems)
        {
            this.Name = _name = name;
            this.Description = _description = description;
            this.VocabularyItems = new ObservableCollection<Word>(vocabularyItems);
            VocabularyItems.CollectionChanged += (sender, args) =>
            {
                Utilities.AddChangedItems(_changedWords, args);
                if (args.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Remove)
                    NotifyPropertyChanged(nameof(DataChanged));
            };

            Utilities.NotifyItemAdded += SubscribeVocabularyChanges; 
            foreach (var word in VocabularyItems)
            {
                SubscribeVocabularyChanges(word);
                foreach(var synonym in word.Synonyms)
                    SubscribeVocabularyChanges(synonym);
                foreach (var antonym in word.Antonyms)
                    SubscribeVocabularyChanges(antonym);
            }
        }

        public static bool CheckUnsavedEnabled { get; set; } = true;
        
        public string Name
        {
            get => _name; 
            private set => this.ChangedName = _name = value;
        }
        public string Description
        {
            get => _description; 
            private set => this.ChangedDescription = _description = value;
        }
        public ObservableCollection<Word> VocabularyItems { get; }

        private string ChangedName
        {
            get => _changedName;
            set
            {
                _changedName = value;
                NotifyPropertyChanged(nameof(DataChanged));
            }
        }

        private string ChangedDescription
        {
            get => _changedDescription;
            set
            {
                _changedDescription = value;
                NotifyPropertyChanged(nameof(DataChanged));
            }
        }

        internal bool DataChanged
        {
            get
            {
                // ReSharper disable once InvertIf
                if (CheckUnsavedEnabled)
                {
                    foreach (var item in VocabularyItems)
                    {
                        Utilities.CheckUnsavedContent(item, _changedWords);
                        item.CheckUnsavedContent();
                    }
                }

                return !ChangedName.Equals(Name) 
                       || !ChangedDescription.Equals(Description)
                       || VocabularyItems.Any(x => x.DataChanged) 
                       || _changedWords.Count > 0;
            }
        }
        
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "") 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        internal void SaveChanges()
        {
            this.Name = ChangedName;
            this.Description = ChangedDescription;
            foreach (var word in VocabularyItems)
                word.SaveChanges();
            _changedWords.Clear();
            
            NotifyPropertyChanged(nameof(DataChanged));
        }

        internal void DiscardChanges()
        {
            this.ChangedName = this.Name;
            this.ChangedDescription = this.Description;
            foreach(var word in VocabularyItems)
                word.EqualizeChangedData();
        }

        private void AddWord()
        {
            var word = new Word();
            VocabularyItems.Add(word);
        }

        private void SubscribeVocabularyChanges(VocabularyItem item)
            => item.NotifyChanged += () => NotifyPropertyChanged(nameof(DataChanged));
        
        // internal void DebugUnsavedChanges()
        // {
        //     Utilities.Log("Lesson:\n-------");
        //     foreach(var x in _changedWords)
        //         Utilities.Log($"      • {x.ChangedTerm} ({x.Term}) ---- {x.ChangedDefinition} ({x.Definition})");
        //     Utilities.Log(" ");
        //     foreach (var y in VocabularyItems)
        //         y.DebugUnsavedChanges();
        // }
    }
}