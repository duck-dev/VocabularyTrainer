using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.UtilityCollection;

namespace VocabularyTrainer.Models;

public class Lesson : IVocabularyContainer<Word>, INotifyPropertyChangedHelper
{
    private string _name;
    private string _description;
    private ObservableCollection<Word> _vocabularyItems;
    private LessonOptions _options = LessonOptions.BalancedTolerance;
        
    private string _changedName = string.Empty;
    private string _changedDescription = string.Empty;
    private LessonOptions _changedOptions = null!;
    private readonly List<Word> _changedWords = new();
        
    public event PropertyChangedEventHandler? PropertyChanged;

    internal delegate void NotifyChanged();
    internal event NotifyChanged? NotifyCollectionsChanged;
        
    [JsonConstructor]
    public Lesson(string name, string description, ObservableCollection<Word> vocabularyItems,
        Dictionary<LearningModeType, bool> isShuffledInModes, LessonOptions options, LearningModeOptions learningModeSettings)
        : this(name, description, vocabularyItems, isShuffledInModes, options, learningModeSettings, true) 
    { }

    public Lesson(string name, string description, IEnumerable<Word> vocabularyItems, 
        Dictionary<LearningModeType, bool> isShuffledInModes, LessonOptions options, LearningModeOptions learningModeSettings, bool fromJson)
    {
        this.Name = _name = name;
        this.Description = _description = description;
        _vocabularyItems = this.VocabularyItems = new ObservableCollection<Word>(vocabularyItems);
        this.Options = options;
        this.LearningModeSettings = learningModeSettings;
        this.IsShuffledInModes = isShuffledInModes;

        Utilities.NotifyItemAdded += SubscribeVocabularyChanges; 
        foreach (var word in VocabularyItems)
        {
            SubscribeVocabularyChanges(word);
            foreach(var synonym in word.Synonyms)
                SubscribeVocabularyChanges(synonym);
            foreach (var antonym in word.Antonyms)
                SubscribeVocabularyChanges(antonym);
                
            if(!fromJson)
                word.EqualizeChangedData();
        }
    }

    internal static bool CheckUnsavedEnabled { get; set; } = true;
        
    public string Name
    {
        get => _name; 
        private set => this.ChangedName = _name = value.Trim();
    }
    public string Description
    {
        get => _description; 
        private set => this.ChangedDescription = _description = value.Trim();
    }

    public ObservableCollection<Word> VocabularyItems
    {
        get => _vocabularyItems;
        private set
        {
            _vocabularyItems = value;
            _vocabularyItems.CollectionChanged += OnVocabularyItemsChanged;
            NotifyPropertyChanged(nameof(VocabularyItems));
        }
    }

    public Dictionary<LearningModeType, bool> IsShuffledInModes { get; } = new();

    public LessonOptions Options
    {
        get => _options;
        set => this.ChangedOptions = _options = value;
    }
    
    public LearningModeOptions LearningModeSettings { get; set; }

    internal LessonOptions ChangedOptions
    {
        get => _changedOptions;
        set
        {
            _changedOptions = value;
            NotifyPropertyChanged(nameof(DataChanged));
        }
    }

    private string ChangedName
    {
        get => _changedName;
        set
        {
            _changedName = value.Trim();
            NotifyPropertyChanged(nameof(DataChanged));
        }
    }

    private string ChangedDescription
    {
        get => _changedDescription;
        set
        {
            _changedDescription = value.Trim();
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
                   || _changedWords.Count > 0
                   || !this.ChangedOptions.Equals(this.Options);
        }
    }
        
    internal static Dictionary<LearningModeType, bool> InitShuffledDictionary()
    {
        var dict = new Dictionary<LearningModeType, bool>();
        foreach(LearningModeType value in Enum.GetValues(typeof(LearningModeType)))
            dict.Add(value, false);
            
        return dict;
    }
        
    public void NotifyPropertyChanged([CallerMemberName] string propertyName = "") 
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
    public void DiscardChanges()
    {
        this.ChangedName = this.Name;
        this.ChangedDescription = this.Description;
        this.ChangedOptions = this.Options;
        this.VocabularyItems = new ObservableCollection<Word>(this.VocabularyItems.Where(x => !_changedWords.Contains(x)));
        foreach (var word in _changedWords.Where(word => word.ChangedAction == NotifyCollectionChangedAction.Remove).ToArray())
            this.VocabularyItems.Add(word);

        _changedWords.Clear();
        foreach (var word in this.VocabularyItems)
        {
            word.ClearCollections();
            word.EqualizeChangedData();
        }
    }

    internal void SaveChanges()
    {
        this.Name = ChangedName;
        this.Description = ChangedDescription;
        this.Options = ChangedOptions;
        foreach (var word in VocabularyItems)
            word.SaveChanges();
        _changedWords.Clear();
            
        NotifyPropertyChanged(nameof(DataChanged));
    }

    internal void InvokeNotifyChanges() => NotifyCollectionsChanged?.Invoke();

    private void AddWord()
    {
        var word = new Word();
        VocabularyItems.Add(word);
    }

    private void SubscribeVocabularyChanges(VocabularyItem item)
        => item.NotifyChanged += () => NotifyPropertyChanged(nameof(DataChanged));

    private void OnVocabularyItemsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        Utilities.AddChangedItems(_changedWords, args);
        if (args.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Remove)
            NotifyPropertyChanged(nameof(DataChanged));
        InvokeNotifyChanges();
    } 
        
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