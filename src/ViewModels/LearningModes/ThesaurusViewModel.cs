using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Models;
using VocabularyTrainer.Models.EqualityComparers;
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

    private VocabularyItem _currentThesaurusItem = null!;
    private IEnumerable<VocabularyItem> _currentCollection = Array.Empty<VocabularyItem>();
    private IEnumerable<string> _possibleDefinitions = Array.Empty<string>();

    public ThesaurusViewModel(Lesson lesson) : base(lesson)
    {
        ConstructThesaurusItems();
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

    protected override int MaximumItems => CurrentLesson.VocabularyItems.Count(x => x.Synonyms.Count > 0 || x.Antonyms.Count > 0);

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
        Utilities.ChangeLearningState(_currentThesaurusItem, this, true);
        NextWord();
    }
    
    private new void ShowSolution()
    {
        OpenSolutionPanel(this.DisplayedTerm, string.Join(", ", _possibleDefinitions), false);
        Utilities.ChangeLearningState(_currentThesaurusItem, this, false);
    }
    
    private new void CheckAnswer()
    {
        string modifiedAnswer = Utilities.ModifyAnswer(Answer, CurrentLesson);
        int mistakeTolerance = CurrentLesson.Options.CorrectionSteps;
        bool tolerateTransposition = CurrentLesson.Options.TolerateSwappedLetters;

        bool correct = false;
        string finalDefinition = string.Join(", ", _possibleDefinitions);
        IEnumerable<string> newCollection = _currentCollection.Where(x => !_currentThesaurusItem.Definition.Equals(x.Definition))
                                                              .Select(x => x.Definition);
        foreach (string definition in newCollection)
        {
            string modifiedDefinition = Utilities.ModifyAnswer(definition, CurrentLesson);
            correct = modifiedDefinition.Equals(modifiedAnswer) 
                      || Utilities.LevenshteinDistance(modifiedDefinition, modifiedAnswer, tolerateTransposition) <= mistakeTolerance;
            if (!correct) 
                continue;
            finalDefinition = definition;
            break;
        }
        
        OpenSolutionPanel(this.DisplayedTerm, finalDefinition, correct);
        Utilities.ChangeLearningState(_currentThesaurusItem, this, correct);
    }

    private void ConstructThesaurusItems()
    {
        // Remove exact duplicates of Words (independent from order of synonyms or the Term)
        var distinctWords = CurrentLesson.VocabularyItems.Distinct(new WordEqualityComparer());

        IList<Word> thesaurusItems = new List<Word>();
        foreach (Word word in distinctWords)
        {
            EvaluateThesaurus(word, word.Synonyms, true);
            EvaluateThesaurus(word, word.Antonyms, false);
            
            Word? equalWord = WordsList.SingleOrDefault(x => x.Definition == word.Definition);
            if (equalWord is not null)
            {
                foreach(VocabularyItem synonym in word.Synonyms)
                    equalWord.Synonyms.Add(synonym);
                foreach(VocabularyItem antonym in word.Antonyms)
                    equalWord.Antonyms.Add(antonym);
                continue;
            }
            thesaurusItems.Add(word);
        }

        WordsList = thesaurusItems.ToArray();

        void EvaluateThesaurus(Word word, IList<VocabularyItem> thesaurusList, bool isSynonym)
        {
            foreach (VocabularyItem item in thesaurusList)
            {
                Word? equalWord = thesaurusItems.SingleOrDefault(x => x.Definition.Equals(item.Definition));
                if (equalWord is not null)
                {
                    IList<VocabularyItem> equalWordThesaurus = isSynonym ? equalWord.Synonyms : equalWord.Antonyms;
                    IList<VocabularyItem> oppositeThesaurusList = isSynonym ? equalWord.Antonyms : equalWord.Synonyms;
                    if(!equalWordThesaurus.Any(x => x.Definition.Equals(word.Definition)))
                        equalWordThesaurus.Add(word);

                    foreach (VocabularyItem synonym in word.Synonyms)
                    {
                        if (!equalWordThesaurus.Any(x => x.Definition.Equals(synonym.Definition)))
                            equalWordThesaurus.Add(synonym);
                    }

                    foreach (VocabularyItem antonym in word.Antonyms)
                    {
                        if (!oppositeThesaurusList.Any(x => x.Definition.Equals(antonym.Definition)))
                            oppositeThesaurusList.Add(antonym);
                    }
                    
                    continue;
                }
                
                var synonyms = new ObservableCollection<VocabularyItem>(word.Synonyms.Where(x => !x.Equals(item)));
                var antonyms = new ObservableCollection<VocabularyItem>(word.Antonyms.Where(x => !x.Equals(item)));
                var mainDefinitionThesaurus = new VocabularyItem(addSelfReference: false)
                {
                    Definition = word.Definition
                };
                mainDefinitionThesaurus.VocabularyReferences.Add(word);
                
                Word newWord = new Word(synonyms, antonyms, false)
                {
                    Definition = item.Definition
                };
                newWord.VocabularyReferences.Add(item);
                (isSynonym ? newWord.Synonyms : newWord.Antonyms).Add(mainDefinitionThesaurus);
            }
        }
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
        _possibleDefinitions = collection.Where(x => !_currentThesaurusItem.Definition.Equals(x.Definition))
                                         .Select(x => x.Definition);
    }
}