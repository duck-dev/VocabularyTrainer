using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Models;
using VocabularyTrainer.UtilityCollection;

namespace VocabularyTrainer.ViewModels.LearningModes;

public sealed class ThesaurusViewModel : AnswerViewModelBase
{
    private const string SynonymType = "SYNONYM";
    private const string AntonymType = "ANTONYM";
    private const string IndefiniteWithoutVowel = "a";
    private const string IndefiniteWithVowel = "an";
    
    private bool _askSynonyms;
    private bool _askAntonyms;
    private string _thesaurusType = SynonymType;
    private string _indefiniteArticle = IndefiniteWithoutVowel;

    private VocabularyItem? _currentThesaurusItem;
    private IEnumerable<VocabularyItem> _currentCollection = Array.Empty<VocabularyItem>();
    private IEnumerable<string> _possibleDefinitions = Array.Empty<string>();

    public ThesaurusViewModel(Lesson lesson) : base(lesson)
    {
        SetThesaurus();
        
        IEnumerable<LearningState> wordStates = lesson.VocabularyItems.Select(x => x.LearningStateInModes[LearningMode]).ToArray();
        IEnumerable<LearningState> synonymStates = Array.Empty<LearningState>();
        IEnumerable<LearningState> antonymStates = Array.Empty<LearningState>();
        foreach (Word word in lesson.VocabularyItems)
        {
            synonymStates = word.Synonyms.Select(x => x.LearningStateInModes[LearningMode]).ToArray();
            antonymStates = word.Antonyms.Select(x => x.LearningStateInModes[LearningMode]).ToArray();
        }

        KnownWords = wordStates.Count(x => x.CustomHasFlag(KnownFlags)) + 
                     synonymStates.Count(x => x.CustomHasFlag(KnownFlags)) + 
                     antonymStates.Count(x => x.CustomHasFlag(KnownFlags));
        WrongWords = wordStates.Count(x => x.CustomHasFlag(WrongFlags)) + 
                     synonymStates.Count(x => x.CustomHasFlag(WrongFlags)) + 
                     antonymStates.Count(x => x.CustomHasFlag(WrongFlags));
    }

    private bool AskSynonyms
    {
        get => _askSynonyms;
        set
        {
            this.RaiseAndSetIfChanged(ref _askSynonyms, value);
            LearningModeOptions settings = CurrentLesson.LearningModeSettings;
            settings.AskSynonyms = value;
            CurrentLesson.LearningModeSettings = settings;
            DataManager.SaveData();
        }
    }
    
    private bool AskAntonyms
    {
        get => _askAntonyms;
        set
        {
            this.RaiseAndSetIfChanged(ref _askAntonyms, value);
            LearningModeOptions settings = CurrentLesson.LearningModeSettings;
            settings.AskAntonyms = value;
            CurrentLesson.LearningModeSettings = settings;
            DataManager.SaveData();
        }
    }

    private string ThesaurusType
    {
        get => _thesaurusType;
        set => this.RaiseAndSetIfChanged(ref _thesaurusType, value);
    }

    private string IndefiniteArticle
    {
        get => _indefiniteArticle;
        set => this.RaiseAndSetIfChanged(ref _indefiniteArticle, value);
    }

    protected override void PickWord(bool resetKnownWords = false, bool goForward = true)
    {
        base.PickWord(resetKnownWords, goForward);
        SetThesaurus();
    }

    protected override void ShuffleWords()
    {
        base.ShuffleWords();
        SetThesaurus();
    }

    protected override void InitCurrentWord()
    {
        SetLearningMode(LearningModeType.Thesaurus);
        base.InitCurrentWord();
    }
    
    protected override void ResetKnownWords()
    {
        SeenWords = 0;
        foreach (Word word in WordsList)
        {
            foreach(VocabularyItem synonym in word.Synonyms)
                Utilities.AddLearningState(synonym, this, LearningState.NotAsked);
            foreach(VocabularyItem antonym in word.Antonyms)
                Utilities.AddLearningState(antonym, this, LearningState.NotAsked);
        }
    }

    private new void CountCorrect()
    {
        if(_currentThesaurusItem is not null)
            Utilities.ChangeLearningState(_currentThesaurusItem, this, true);
        NextWord();
    }
    
    private new void ShowSolution()
    {
        OpenSolutionPanel(this.DisplayedTerm, string.Join(", ", _possibleDefinitions), false);
        if(_currentThesaurusItem is not null)
            Utilities.ChangeLearningState(_currentThesaurusItem, this, false);
    }
    
    private new void CheckAnswer()
    {
        string? modifiedAnswer = Utilities.ModifyAnswer(Answer, CurrentLesson);
        int mistakeTolerance = CurrentLesson.Options.CorrectionSteps;
        bool tolerateTransposition = CurrentLesson.Options.TolerateSwappedLetters;

        bool correct = false;
        string finalDefinition = string.Join(", ", _possibleDefinitions);
        foreach (string definition in _currentCollection.Where(x => x != _currentThesaurusItem).Select(x => x.Definition))
        {
            string? modifiedDefinition = Utilities.ModifyAnswer(definition, CurrentLesson);
            correct = modifiedAnswer is not null && modifiedDefinition is not null && (modifiedDefinition.Equals(modifiedAnswer) 
                || Utilities.LevenshteinDistance(modifiedDefinition, modifiedAnswer, tolerateTransposition) <= mistakeTolerance);
            if (!correct) 
                continue;
            finalDefinition = definition;
            break;
        }
        OpenSolutionPanel(this.DisplayedTerm, finalDefinition, correct);
        if(_currentThesaurusItem is not null)
            Utilities.ChangeLearningState(_currentThesaurusItem, this, correct);
    }

    private void SetThesaurus()
    {
        if (CurrentWord.Synonyms.Count <= 0 && CurrentWord.Antonyms.Count <= 0)
        {
            NextWord();
            return;
        }
        
        var rnd = new Random();
        List<VocabularyItem> collection;
        bool synonymChosen;
        bool antonymChosen;
        if (AskSynonyms == AskAntonyms)
        {
            int thesaurusType = rnd.Next(0, 2);
            synonymChosen = thesaurusType == 0;
            antonymChosen = thesaurusType == 1;
        }
        else
        {
            synonymChosen = AskSynonyms;
            antonymChosen = AskAntonyms;
        }

        if (synonymChosen && CurrentWord.Synonyms.Count > 0)
        {
            collection = new List<VocabularyItem>(CurrentWord.Synonyms.Clone());
            this.ThesaurusType = SynonymType;
            this.IndefiniteArticle = IndefiniteWithoutVowel;
        }
        else if (antonymChosen && CurrentWord.Antonyms.Count > 0)
        {
            collection = new List<VocabularyItem>(CurrentWord.Antonyms.Clone());
            collection.AddRange(CurrentWord.Synonyms);
            this.ThesaurusType = AntonymType;
            this.IndefiniteArticle = IndefiniteWithVowel;
        }
        else
        {
            NextWord();
            return;
        }
        
        collection.Add(CurrentWord);
        
        int index = rnd.Next(0, collection.Count);
        this.DisplayedTerm = collection[index].Definition;
        _currentThesaurusItem = collection[index];
        _currentCollection = collection;
        _possibleDefinitions = collection.Where(x => x != _currentThesaurusItem).Select(x => x.Definition);
    }
}