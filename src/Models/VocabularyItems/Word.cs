using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Text.Json.Serialization;
using Avalonia;
using Avalonia.Media;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.Models.ItemStyleControls;
using VocabularyTrainer.ResourcesNamespace;
using VocabularyTrainer.UtilityCollection;
using VocabularyTrainer.ViewModels;

#pragma warning disable CS0659

namespace VocabularyTrainer.Models;

public class Word : DualVocabularyItem, IIndexable, IContentVerification<Word>, IEquatable<Word>
{
    private ObservableCollection<VocabularyItem> _synonyms = new();
    private ObservableCollection<VocabularyItem> _antonyms = new();
        
    private int _index;
    private int _partOfSpeechIndex;
    private int _changedPartOfSpeechIndex;
    private LearningState _learningStatus;
    private readonly List<VocabularyItem> _changedSynonyms = new();
    private readonly List<VocabularyItem> _changedAntonyms = new();
    private readonly PartOfSpeech[] _partsOfSpeech = Enum.GetValues<PartOfSpeech>();

    private readonly Dictionary<LearningState, SolidColorBrush> _statusColors = new()
    {
        { LearningState.VeryHard, Resources.DarkRedBrush },
        { LearningState.WrongOnce, Resources.MainRedBrush },
        { LearningState.NotAsked, Resources.MainGreyBrush },
        { LearningState.KnownOnce, Resources.LightGreenBrush },
        { LearningState.KnownPerfectly, Resources.MainGreenBrush }
    };

    public Word()
    {
        ThesaurusTitleDefinitions = new[]
        {
            new Tuple<string, string, ItemStyleBase<Word>>("Synonyms:", "Add Synonym", new SynonymStyle(this)),
            new Tuple<string, string, ItemStyleBase<Word>>("Antonyms:", "Add Antonym", new AntonymStyle(this))
        };
        RemoveCommand = ReactiveCommand.Create<IVocabularyContainer<Word>>(Remove);
        SubscribeSynonyms();
        SubscribeAntonyms();

        LearningStatus = LearningState.NotAsked;
        foreach (LearningModeType value in Enum.GetValues(typeof(LearningModeType)))
        {
            if(value != LearningModeType.Thesaurus)
                LearningStateInModes.Add(value, LearningState.NotAsked);
        }
    }

    [JsonConstructor]
    public Word(ObservableCollection<VocabularyItem> synonyms, ObservableCollection<VocabularyItem> antonyms,
        LearningState learningStatus, int partOfSpeechIndex) : this()
    {
        this.Synonyms = synonyms;
        this.Antonyms = antonyms;
        this.LearningStatus = learningStatus;
        this.PartOfSpeechIndex = partOfSpeechIndex;

        foreach (var item in this.Synonyms)
            item.ContainerCollection = this.Synonyms;
        foreach (var item in this.Antonyms)
            item.ContainerCollection = this.Antonyms;
    }

    public ObservableCollection<VocabularyItem> Synonyms
    {
        get => _synonyms;
        private set
        {
            _synonyms = value;
            SubscribeSynonyms();
            NotifyPropertyChanged(nameof(Synonyms));
        }
    }

    public ObservableCollection<VocabularyItem> Antonyms
    {
        get => _antonyms;
        private set
        {
            _antonyms = value;
            SubscribeAntonyms();
            NotifyPropertyChanged(nameof(Antonyms));
        }
    }

    public LearningState LearningStatus
    {
        get => _learningStatus;
        set
        {
            _learningStatus = value;
            WordStatusColor = _statusColors[_learningStatus];
        }
    }

    public int PartOfSpeechIndex
    {
        get => _partOfSpeechIndex;
        set
        {
            _partOfSpeechIndex = ChangedPartOfSpeechIndex = value;
            NotifyPropertyChanged(nameof(SelectedPartOfSpeech));
        }
    }

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

    internal bool DataChanged => !ChangedTerm.Equals(Term)
                                 || !ChangedDefinition.Equals(Definition)
                                 || ChangedPartOfSpeechIndex != PartOfSpeechIndex
                                 || Synonyms.Any(x => !x.ChangedDefinition.Equals(x.Definition))
                                 || Antonyms.Any(x => !x.ChangedDefinition.Equals(x.Definition))
                                 || _changedSynonyms.Count > 0 || _changedAntonyms.Count > 0;

    internal bool IsFilled => (!string.IsNullOrEmpty(ChangedTerm) && !string.IsNullOrWhiteSpace(ChangedTerm))
                              || (!string.IsNullOrEmpty(ChangedDefinition) && !string.IsNullOrWhiteSpace(ChangedDefinition))
                              || (SelectedPartOfSpeech != PartOfSpeech.None)
                              || (Synonyms.Any(x => !string.IsNullOrEmpty(x.Definition) && !string.IsNullOrWhiteSpace(x.Definition)))
                              || (Antonyms.Any(x => !string.IsNullOrEmpty(x.Definition) && !string.IsNullOrWhiteSpace(x.Definition)));

    internal bool HasSynonyms => Synonyms.Count > 0;
    internal bool HasAntonyms => Antonyms.Count > 0;

    internal bool IsThesaurusEnabled => MainWindowViewModel.CurrentLesson != null
                                        && MainWindowViewModel.CurrentLesson.LearningModeSettings.ShowThesaurusInModes[LearningModeType.VocabularyList] == true
                                        && (HasSynonyms || HasAntonyms);
    
    internal PartOfSpeech SelectedPartOfSpeech => _partsOfSpeech[_partOfSpeechIndex];

    internal int ChangedPartOfSpeechIndex
    {
        get => _changedPartOfSpeechIndex;
        set
        {
            _changedPartOfSpeechIndex = value;
            InvokeNotifyChanged();
        }
    }

    internal PartOfSpeechContainer[] PartsOfSpeech { get; } =
    {
        new("None", Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "LightGrey", Resources.StyleIndex) 
                    ?? new SolidColorBrush(Color.Parse("#A8ADB5"))),
        new("Noun", Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "LightBrown", Resources.StyleIndex) 
                    ?? new SolidColorBrush(Color.Parse("#A8744F"))),
        new("Verb", Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "VeryLightBlue", Resources.StyleIndex) 
                    ?? new SolidColorBrush(Color.Parse("#ABBECE"))),
        new("Adjective", Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "MainYellow", Resources.StyleIndex) 
                    ?? new SolidColorBrush(Color.Parse("#FCC100"))),
        new("Adverb", Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "Orange", Resources.StyleIndex) 
                         ?? new SolidColorBrush(Color.Parse("#DD9A25"))),
        new("Pronoun", Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "SoftGreen", Resources.StyleIndex) 
                      ?? new SolidColorBrush(Color.Parse("#0CA079"))),
        new("Other", Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "LightGreen", Resources.StyleIndex) 
                       ?? new SolidColorBrush(Color.Parse("#91C669")))
    };

    internal SolidColorBrush WordStatusColor { get; private set; }

    private Tuple<string, string, ItemStyleBase<Word>>[] ThesaurusTitleDefinitions { get; }

    private ReactiveCommand<IVocabularyContainer<Word>, Unit> RemoveCommand { get; }

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
                                                       && x.ChangedPartOfSpeechIndex == this.ChangedPartOfSpeechIndex
                                                       && x.Synonyms.SequenceEqual(this.Synonyms)
                                                       && x.Antonyms.SequenceEqual(this.Antonyms));
        return identicalItem is not null && !ReferenceEquals(identicalItem, this);
    }

    public override void EqualizeChangedData()
    {
        base.EqualizeChangedData();

        this.ChangedPartOfSpeechIndex = PartOfSpeechIndex;
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
               && other.ChangedPartOfSpeechIndex == this.ChangedPartOfSpeechIndex
               && other.Synonyms.SequenceEqual(this.Synonyms) && other.Antonyms.SequenceEqual(this.Antonyms);
    }

    public override bool Equals(object? obj) => Equals(obj as Word);

    public override void SaveChanges()
    {
        base.SaveChanges();
        this.PartOfSpeechIndex = ChangedPartOfSpeechIndex;
        foreach (var item in Synonyms)
            item.SaveChanges();
        foreach (var item in Antonyms)
            item.SaveChanges();
        _changedSynonyms.Clear();
        _changedAntonyms.Clear();
    }
    
    public override object Clone() => new Word(Synonyms, Antonyms, LearningStatus, PartOfSpeechIndex)
    {
        Term = this.Term,
        Definition = this.Definition,
        IsDifficult = this.IsDifficult,
        LearningStateInModes = this.LearningStateInModes
    };

    protected internal override bool ContainsTerm(string search)
    {
        string modifiedSearch = Utilities.ModifyAnswer(search, ModificationSettings);
        if (base.ContainsTerm(modifiedSearch))
            return true;
        return Synonyms.Any(x => x.ContainsTerm(modifiedSearch)) || Antonyms.Any(x => x.ContainsTerm(modifiedSearch));
    }

    internal void ClearCollections()
    {
        this.Synonyms = new ObservableCollection<VocabularyItem>(Synonyms.Where(x => !_changedSynonyms.Contains(x)));
        this.Antonyms = new ObservableCollection<VocabularyItem>(Antonyms.Where(x => !_changedAntonyms.Contains(x)));

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var synonym in _changedSynonyms.ToArray())
        {
            if(synonym.ChangedAction == NotifyCollectionChangedAction.Remove)
                this.Synonyms.Add(synonym);
        }

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var antonym in _changedAntonyms.ToArray())
        {
            if(antonym.ChangedAction == NotifyCollectionChangedAction.Remove)
                this.Antonyms.Add(antonym);
        }
            
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

    private void SubscribeSynonyms()
    {
        Synonyms.CollectionChanged += (sender, args) =>
        {
            Utilities.AddChangedItems(_changedSynonyms, args);
            NotifyChanges(args);
        };
    }
        
    private void SubscribeAntonyms()
    {
        Antonyms.CollectionChanged += (sender, args) =>
        {
            Utilities.AddChangedItems(_changedAntonyms, args);
            NotifyChanges(args);
        };
    }

    private static void NotifyChanges(NotifyCollectionChangedEventArgs args)
    {
        if (args.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Remove)
            NotifyDataChanged();
        MainWindowViewModel.CurrentLesson?.InvokeNotifyChanges();
    }

    private static void NotifyDataChanged()
    {
        var lesson = MainWindowViewModel.CurrentLesson;
        lesson?.NotifyPropertyChanged(nameof(lesson.DataChanged));
    }
}