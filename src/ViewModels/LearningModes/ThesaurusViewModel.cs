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

    public ThesaurusViewModel(Lesson lesson) : base(lesson, false)
    {
        LearningModeOptions settings = CurrentLesson.LearningModeSettings;
        this.AskSynonyms = settings.AskSynonyms;
        this.AskAntonyms = settings.AskAntonyms;
        
        this.KnownWords = WordsList.Count(x => x.LearningStateInModes[LearningMode].CustomHasFlag(Utilities.KnownFlags) 
                                               || (x.VocabularyReferences != null 
                                               && x.VocabularyReferences.Any(y => y.LearningStateInModes[LearningMode].CustomHasFlag(Utilities.KnownFlags))));
        this.WrongWords = WordsList.Count(x => x.LearningStateInModes[LearningMode].CustomHasFlag(Utilities.WrongFlags) 
                                               || (x.VocabularyReferences != null 
                                                   && x.VocabularyReferences.Any(y => y.LearningStateInModes[LearningMode].CustomHasFlag(Utilities.WrongFlags))));
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

    protected override void PickWord(bool resetKnownWords = false, bool goForward = true, bool changeLearningState = true)
    {
        base.PickWord(resetKnownWords, goForward, changeLearningState);
        SetThesaurus();
    }
    
    protected override void PickWordProgressive()
    {
        base.PickWordProgressive();
        SetThesaurus();
    }

    protected override void ShuffleWords()
    {
        base.ShuffleWords();
        SetThesaurus();
    }

    protected override void Initialize(bool initializeWords)
    {
        LearningModeOptions settings = CurrentLesson.LearningModeSettings;
        if(settings.ProgressiveLearningInModes.ContainsKey(LearningMode))
            this.ProgressiveLearningEnabled = settings.ProgressiveLearningInModes[LearningMode];
        
        SetLearningMode(LearningModeType.Thesaurus, "Synonyms and Antonyms");
        ConstructThesaurusItems();
        InitCurrentWord();
        VerifyAndSetItem(SetThesaurus);
        base.Initialize(initializeWords);
    }

    protected override void ResetInitialWordsOrder()
        => ConstructThesaurusItems();

    private new void CountCorrect()
    {
        Utilities.ChangeLearningStateThesaurus(CurrentWord, this, true);
        NextWord();
    }

    /// <summary>
    /// Create a collection of all distinct vocabulary items (including words as well as synonyms and antonyms of those words)
    /// with their own synonyms and antonyms and overwrite <see cref="LearningModeViewModelBase.WordsList"/> with this new collection.
    /// </summary>
    private void ConstructThesaurusItems()
    {
        var wordComparer = new WordEqualityComparer();
        var vocabularyItemComparer = new VocabularyItemEqualityComparer();
        // Remove exact duplicates of words (independent from order of synonyms or the Term) and words without synonyms and antonyms
        var distinctWords = CurrentLesson.VocabularyItems.Where(x => x.HasSynonyms || x.HasAntonyms)
                                                                        .Distinct(wordComparer);

        IList<Word> thesaurusItems = new List<Word>();
        foreach (Word word in distinctWords)
        {
            EvaluateThesaurus(word, word.Synonyms, true);
            EvaluateThesaurus(word, word.Antonyms, false);
            
            Word? equalWord = thesaurusItems.SingleOrDefault(x => x.Definition.Equals(word.Definition));
            // There is already an item with this definition, add the synonyms and antonyms of the current word to this already existing item
            if (equalWord is not null)
            {
                foreach (VocabularyItem synonym in word.Synonyms.ToArray())
                {
                    if(!equalWord.Synonyms.Contains(synonym, vocabularyItemComparer) && !synonym.Definition.Equals(equalWord.Definition))
                        equalWord.Synonyms.Add(synonym);
                }

                foreach (VocabularyItem antonym in word.Antonyms.ToArray())
                {
                    if(!equalWord.Antonyms.Contains(antonym, vocabularyItemComparer) && !antonym.Definition.Equals(equalWord.Definition))
                        equalWord.Antonyms.Add(antonym);
                }
                equalWord.VocabularyReferences?.Add(word);
                equalWord = null;
                continue;
            }
            
            word.VocabularyReferences = new List<VocabularyItem>();
            thesaurusItems.Add(word);
        }
        WordsList = thesaurusItems.ToArray();

        void EvaluateThesaurus(Word word, IEnumerable<VocabularyItem> thesaurusList, bool isSynonym)
        {
            foreach (VocabularyItem item in thesaurusList.ToArray())
            {
                Word? equalWord = thesaurusItems.SingleOrDefault(x => x.Definition.Equals(item.Definition));
                IList<VocabularyItem> oppositeThesaurusList;
                // There is already an item with this definition, add the synonyms and antonyms of the current word to this already existing item
                if (equalWord is not null)
                {
                    IList<VocabularyItem> equalWordThesaurus = isSynonym ? equalWord.Synonyms : equalWord.Antonyms;
                    oppositeThesaurusList = isSynonym ? equalWord.Antonyms : equalWord.Synonyms;
                    if(!equalWordThesaurus.Any(x => x.Definition.Equals(word.Definition)))
                        equalWordThesaurus.Add(word);

                    foreach (VocabularyItem synonym in word.Synonyms.ToArray())
                    {
                        if (!equalWordThesaurus.Contains(synonym, vocabularyItemComparer) && !synonym.Definition.Equals(equalWord.Definition))
                            equalWordThesaurus.Add(synonym);
                    }

                    foreach (VocabularyItem antonym in word.Antonyms.ToArray())
                    {
                        if (!oppositeThesaurusList.Contains(antonym, vocabularyItemComparer) && !antonym.Definition.Equals(equalWord.Definition))
                            oppositeThesaurusList.Add(antonym);
                    }
                    equalWord.VocabularyReferences?.Add(item);
                    equalWord = null;
                    continue;
                }
                
                oppositeThesaurusList = isSynonym ? word.Antonyms : word.Synonyms;
                var synonyms = new ObservableCollection<VocabularyItem>(thesaurusList.Where(x => !x.Definition.Equals(item.Definition))
                                                                                     .Distinct(vocabularyItemComparer));
                var antonyms = new ObservableCollection<VocabularyItem>(oppositeThesaurusList.Where(x => !x.Definition.Equals(item.Definition))
                                                                                             .Distinct(vocabularyItemComparer));
                // Create a synonym/antonym out of the `Word` containing the current thesaurus item, which will now be turned into an own `Word`
                var mainDefinitionThesaurus = new VocabularyItem
                {
                    Definition = word.Definition,
                    VocabularyReferences = new List<VocabularyItem> { word }
                };

                Word newWord = new Word(synonyms, antonyms, LearningState.NotAsked, (int)PartOfSpeech.None)
                {
                    Definition = item.Definition,
                    VocabularyReferences = new List<VocabularyItem> { item }
                };
                newWord.LearningStateInModes[LearningModeType.Thesaurus] = item.LearningStateInModes[LearningModeType.Thesaurus];
                var thesaurusCollection = isSynonym ? newWord.Synonyms : newWord.Antonyms;
                if (!thesaurusCollection.Contains(mainDefinitionThesaurus, vocabularyItemComparer)
                    && !mainDefinitionThesaurus.Definition.Equals(newWord.Definition))
                {
                    thesaurusCollection.Add(mainDefinitionThesaurus);
                }
                thesaurusItems.Add(newWord);
            }
        }
    }

    private void SetThesaurus()
    {
        Collection<VocabularyItem> synonyms = CurrentWord.Synonyms;
        Collection<VocabularyItem> antonyms = CurrentWord.Antonyms;
        if (synonyms.Count <= 0 && antonyms.Count <= 0)
        {
            NextWord();
            return;
        }
        
        this.DisplayedTerm = CurrentWord.Definition;
        
        bool synonymChosen;
        bool antonymChosen;
        if (AskSynonyms == AskAntonyms)
        {
            if (synonyms.Count > 0 && antonyms.Count > 0)
            {
                var rnd = new Random();
                int thesaurusType = rnd.Next(0, 2);
                synonymChosen = thesaurusType == 0;
                antonymChosen = thesaurusType == 1;
            }
            else
            {
                synonymChosen = synonyms.Count > 0;
                antonymChosen = antonyms.Count > 0;
            }
        }
        else
        {
            synonymChosen = AskSynonyms;
            antonymChosen = AskAntonyms;
        }

        if (synonymChosen && synonyms.Count > 0)
        {
            PossibleDefinitions = synonyms.Select(x => x.Definition);
            this.ThesaurusType = SynonymType;
            this.IndefiniteArticle = IndefiniteWithoutVowel;
        }
        else if (antonymChosen && antonyms.Count > 0)
        {
            PossibleDefinitions = antonyms.Select(x => x.Definition);
            this.ThesaurusType = AntonymType;
            this.IndefiniteArticle = IndefiniteWithVowel;
        }
        else
        {
            NextWord(false);
        }
    }
}