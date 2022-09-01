using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Models;
using VocabularyTrainer.UtilityCollection;

namespace VocabularyTrainer.ViewModels.LearningModes;

public abstract class SingleWordViewModelBase : LearningModeViewModelBase
{
    private Word _currentWord = null!;
    private int _wordIndex;
    private string? _displayedTerm;
    private int _seenWords;

    private bool _askTerm;
    private bool _askDefinition;
    private bool _progressiveLearningEnabled;

    private bool _progressiveLearningOptionInitialized;

    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    protected SingleWordViewModelBase(Lesson lesson, bool initializeWords = true) : base(lesson)
    {
        Initialize(initializeWords);
        ShuffleButtonEnabled = ShufflingAllowed;
        IsSeenWordsEnabled = !ProgressiveLearningEnabled;
    }

    protected Word CurrentWord
    {
        get => _currentWord; 
        private set => this.RaiseAndSetIfChanged(ref _currentWord, value);
    }

    protected string? DisplayedTerm
    {
        get => _displayedTerm;
        set => this.RaiseAndSetIfChanged(ref _displayedTerm, value);
    }

    protected string Definition { get; set; } = string.Empty;

    protected int WordIndexCorrected => _wordIndex + 1;
    protected int MaximumItems => WordsList.Length;

    protected bool IsCurrentWordDifficult
    {
        get
        {
            if (LearningMode != LearningModeType.Thesaurus)
                return CurrentWord.IsDifficult;
            
            return CurrentWord.IsDifficult || (CurrentWord.VocabularyReferences is not null 
                                               && CurrentWord.VocabularyReferences.Any(x => x.IsDifficult));
        }
    }
        
    protected int SeenWords
    {
        get => _seenWords; 
        set => this.RaiseAndSetIfChanged(ref _seenWords, value);
    }

    protected bool AskTerm
    {
        get => _askTerm;
        set
        {
            this.RaiseAndSetIfChanged(ref _askTerm, value);
            CurrentLesson.LearningModeSettings.AskTermInModes[LearningMode] = value;
            DataManager.SaveData();
        }
    }

    protected bool AskDefinition
    {
        get => _askDefinition;
        set
        {
            this.RaiseAndSetIfChanged(ref _askDefinition, value);
            CurrentLesson.LearningModeSettings.AskDefinitionInModes[LearningMode] = value;
            DataManager.SaveData();
        }
    }
    
    protected bool ProgressiveLearningEnabled
    {
        get => _progressiveLearningEnabled;
        set
        {
            this.RaiseAndSetIfChanged(ref _progressiveLearningEnabled, value);
            ShuffleButtonEnabled = ShufflingAllowed;
            IsSeenWordsEnabled = !value;
            CurrentLesson.LearningModeSettings.ProgressiveLearningInModes[LearningMode] = value;
            DataManager.SaveData();

            if (!_progressiveLearningOptionInitialized)
            {
                _progressiveLearningOptionInitialized = true;
                return;
            }

            if (value == true)
                return;

            if (ShuffleWordsAutomatically)
                ShuffleWords();
            else
                ResetInitialWordsOrder();
            _wordIndex = 0;
            PickWord(true);
        }
    }
    
    protected bool IsTermChosen { get; private set; }

    protected override bool ShufflingAllowed => !ProgressiveLearningEnabled;

    protected void PreviousWord()
    {
        if(ProgressiveLearningEnabled)
        {
            PickWordProgressive();
            return;
        }

        _wordIndex--;
        WrapWords(WordsList.Length - 1, false);
    }

    protected virtual void NextWord(bool changeLearningState = true)
    {
        if(ProgressiveLearningEnabled)
        {
            PickWordProgressive();
            return;
        }

        _wordIndex++;
        WrapWords(0, true, changeLearningState);
    }

    protected virtual void PickWord(bool resetKnownWords = false, bool goForward = true, bool changeLearningState = true)
    {
        var word = WordsList[_wordIndex];
            
        CurrentWord = word;
        this.RaisePropertyChanged(nameof(IsCurrentWordDifficult));
        this.RaisePropertyChanged(nameof(WordIndexCorrected));

        // Create display when the words should be reset and only reset upon confirming
        // Remove parameter in PickWord(), move this code below to this specific confirmation method,
        // ... set NextWord() and PreviousWord() back to normal.
        if ((this.SeenWords == WordsList.Length && ((goForward && _wordIndex == 0) || (!goForward && _wordIndex == WordsList.Length - 1))) 
            || resetKnownWords) // Looking at micro-performance, checking `resetKnownWords` should come first, but it looks horrible
        {
            ResetKnownWords();
            return;
        }
            
        int factor = goForward ? -1 : 1;
        int oldIndex = _wordIndex + factor;
        if (oldIndex < 0)
            oldIndex = WordsList.Length - 1;
        else if (oldIndex >= WordsList.Length)
            oldIndex = 0;

        if (!changeLearningState)
            return;
        Word previousWord = WordsList[oldIndex];
        LearningState knownState = previousWord.LearningStateInModes[this.LearningMode];
        if(knownState.CustomHasFlag(LearningState.NotAsked))
            Utilities.RemoveLearningState(previousWord, this, LearningState.NotAsked, considerOverallState: false);
    }
    
    protected virtual void PickWordProgressive()
    {
        ICollection<Word> updatedWordsList;
        if (WordsList.Length > 1)
        {
            updatedWordsList = new List<Word>(WordsList);
            updatedWordsList.Remove(CurrentWord);
        }
        else
        {
            updatedWordsList = WordsList;
        }

        IReadOnlyList<Word> category = Utilities.PickCategory(updatedWordsList, LearningMode);
        var random = new Random();
        int index = random.Next(0, category.Count);
        
        CurrentWord = category[index];
        this.RaisePropertyChanged(nameof(IsCurrentWordDifficult));
    }
        
    protected override void ShuffleWords()
    {
        base.ShuffleWords();
        if(!ProgressiveLearningEnabled)
            PickWord(true);
    }

    protected bool VerifyAndSetItem(Action action) // action = either SetWord() or SetThesaurus()
    {
        if (!ProgressiveLearningEnabled)
            action.Invoke();
        return !ProgressiveLearningEnabled;
    }

    protected virtual void SetWord()
    {
        if (AskTerm == AskDefinition)
        {
            var rnd = new Random();
            int num = rnd.Next(0, 2);
            this.IsTermChosen = num == 0;
            this.DisplayedTerm = CurrentWord.GetAdjustedTerm(IsTermChosen);
            this.Definition = CurrentWord.GetAdjustedDefinition(IsTermChosen);
        }
        else
        {
            this.IsTermChosen = AskTerm;
            this.DisplayedTerm = CurrentWord.GetAdjustedTerm(AskTerm);
            this.Definition = CurrentWord.GetAdjustedDefinition(AskTerm);
        }
    }

    protected internal virtual void VisualizeLearningProgress(LearningState previousState, LearningState newState)
    {
        if(!newState.CustomHasFlag(LearningState.NotAsked) && previousState.CustomHasFlag(LearningState.NotAsked))
            this.SeenWords++;
        DataManager.SaveData();
    }

    protected void InitCurrentWord()
    {
        if (ProgressiveLearningEnabled)
        {
            PickWordProgressive();
            return;
        }

        Dictionary<LearningModeType, bool> shuffledDict = CurrentLesson.IsShuffledInModes;
        if (shuffledDict[this.LearningMode] == true
            || WordsList.All(x => !x.LearningStateInModes[this.LearningMode].CustomHasFlag(LearningState.NotAsked)))
        {
            shuffledDict[this.LearningMode] = false;
            _wordIndex = 0;
            PickWord(true);
            return;
        }
            
        for (int i = 0; i < WordsList.Length; i++)
        {
            if (!WordsList[i].LearningStateInModes[this.LearningMode].CustomHasFlag(LearningState.NotAsked)) 
                continue;
            _wordIndex = i;
            break;
        }
        this.SeenWords = WordsList.Count(x => !x.LearningStateInModes[this.LearningMode].CustomHasFlag(LearningState.NotAsked));

        this.RaisePropertyChanged(nameof(MaximumItems));
        PickWord(_wordIndex == 0);
    }

    protected virtual void Initialize(bool initializeWords)
    {
        LearningModeOptions settings = CurrentLesson.LearningModeSettings;
        if(settings.AskTermInModes.ContainsKey(LearningMode))
            this.AskTerm = settings.AskTermInModes[LearningMode];
        if(settings.AskDefinitionInModes.ContainsKey(LearningMode))
            this.AskDefinition = settings.AskDefinitionInModes[LearningMode];
        if(settings.ProgressiveLearningInModes.ContainsKey(LearningMode))
            this.ProgressiveLearningEnabled = settings.ProgressiveLearningInModes[LearningMode];
        InitializeSettings();

        if(initializeWords)
            InitCurrentWord();
    }

    internal void SetDifficultTerm(bool difficult, VocabularyItem? item = null)
    {
        VocabularyItem vocabularyItem = item ?? CurrentWord;
        vocabularyItem.IsDifficult = difficult;
        if (LearningMode is LearningModeType.Thesaurus && vocabularyItem.VocabularyReferences != null)
        {
            foreach (VocabularyItem itemRef in vocabularyItem.VocabularyReferences)
                itemRef.IsDifficult = difficult;
        }
        DataManager.SaveData();
    }

    private void WrapWords(int newIndex, bool goForward, bool changeLearningState = true)
    {
        bool wrapWords = _wordIndex >= WordsList.Length || _wordIndex < 0;
        if (wrapWords)
            _wordIndex = newIndex;

        bool resetWords = wrapWords && (this.SeenWords == WordsList.Length || this.SeenWords == WordsList.Length - 1);
        PickWord(resetWords, goForward, changeLearningState);
    }
    
    private void ResetKnownWords()
    {
        this.SeenWords = 0;
        foreach (var word in WordsList)
            Utilities.AddLearningState(word, this, LearningState.NotAsked, considerOverallState: false);
    }
}