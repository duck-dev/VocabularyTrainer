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

    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    protected SingleWordViewModelBase(Lesson lesson, bool initializeWords = true) : base(lesson)
    {
        Initialize(lesson, initializeWords);
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
            if (LearningMode == LearningModeType.Thesaurus)
                return CurrentWord.VocabularyReferences is not null && CurrentWord.VocabularyReferences.Any(x => x.IsDifficult);
            return CurrentWord.IsDifficult;
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

    protected void PreviousWord()
    {
        _wordIndex--;
        WrapWords(WordsList.Length - 1, false);
    }

    protected virtual void NextWord(bool changeLearningState = true)
    {
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
            Utilities.RemoveLearningState(previousWord, this, LearningState.NotAsked);
    }
        
    protected override void ShuffleWords()
    {
        base.ShuffleWords();
        PickWord(true);
    }

    protected void SetWord()
    {
        if (AskTerm == AskDefinition)
        {
            var rnd = new Random();
            int num = rnd.Next(0, 2);
            this.DisplayedTerm = num == 0 ? CurrentWord.Term : CurrentWord.Definition;
            this.Definition = num == 0 ? CurrentWord.Definition : CurrentWord.Term;
        }
        else
        {
            this.DisplayedTerm = AskTerm ? CurrentWord.Term : CurrentWord.Definition;
            this.Definition = AskTerm ? CurrentWord.Definition : CurrentWord.Term;
        }
    }

    protected internal virtual void VisualizeLearningProgress(LearningState previousState, LearningState newState)
    {
        if(!newState.CustomHasFlag(LearningState.NotAsked) && previousState.CustomHasFlag(LearningState.NotAsked))
            this.SeenWords++;
        DataManager.SaveData();
    }

    protected virtual void ResetKnownWords()
    {
        this.SeenWords = 0;
        foreach (var word in WordsList)
            Utilities.AddLearningState(word, this, LearningState.NotAsked);
    }

    protected virtual void InitCurrentWord()
    {
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

    protected virtual void Initialize(Lesson lesson, bool initializeWords)
    {
        if(initializeWords)
            InitCurrentWord();
        
        InitializeSettings();
        LearningModeOptions settings = CurrentLesson.LearningModeSettings;
        if(settings.AskTermInModes.ContainsKey(LearningMode))
            this.AskTerm = settings.AskTermInModes[LearningMode];
        if(settings.AskDefinitionInModes.ContainsKey(LearningMode))
            this.AskDefinition = settings.AskDefinitionInModes[LearningMode];
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
}