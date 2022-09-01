using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using DynamicData;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Models;
using VocabularyTrainer.ResourcesNamespace;
using VocabularyTrainer.UtilityCollection;

namespace VocabularyTrainer.ViewModels.LearningModes;

public sealed class MultipleChoiceViewModel : AnswerViewModelBase
{
    private static readonly Dictionary<PartOfSpeech, PartOfSpeech[]> _alternativePartOfSpeeches = new()
    {
        { PartOfSpeech.Adjective, new [] { PartOfSpeech.Adverb } },
        { PartOfSpeech.Adverb, new [] { PartOfSpeech.Adjective } },
        { PartOfSpeech.Pronoun, new [] { PartOfSpeech.Other } },
        { PartOfSpeech.Other, new [] { PartOfSpeech.Pronoun } }
    };

    private static readonly int[] _similarWordLimits = { 1, 4 };

    private static readonly LinearGradientBrush _greyGradient = 
        ConstructMultipleChoiceGradient(new[] { Resources.VeryLightGrey, Resources.VeryLightGrey });
    private static readonly LinearGradientBrush _greenGradient = 
        ConstructMultipleChoiceGradient(new[] { Resources.SoftGreen, Resources.VeryLightGrey });
    private static readonly LinearGradientBrush _redGradient = 
        ConstructMultipleChoiceGradient(new[] { Resources.DarkerLightRedContextMenu, Resources.VeryLightGrey });
    
    private ObservableCollection<string> _choices = new();

    public MultipleChoiceViewModel(Lesson lesson) : base(lesson)
    {
        VerifyAndSetItem(SetWord);
    }

    private ObservableCollection<string> Choices
    {
        get => _choices; 
        set => this.RaiseAndSetIfChanged(ref _choices, value);
    }
    
    private ObservableCollection<LinearGradientBrush> ButtonGradients { get; } = new()
    {
        _greyGradient, _greyGradient, _greyGradient, _greyGradient
    };

    protected override void Initialize(bool initializeWords)
    {
        SetLearningMode(LearningModeType.MultipleChoice, "Multiple Choice");
        base.Initialize(initializeWords);
    }
    
    protected override void PickWord(bool resetKnownWords = false, bool goForward = true, bool changeLearningState = true)
    {
        base.PickWord(resetKnownWords, goForward, changeLearningState);
        SetWord();
    }
    
    protected override void PickWordProgressive()
    {
        base.PickWordProgressive();
        SetWord();
    }

    protected override void SetWord()
    {
        base.SetWord();
        var definitions = DeterminePossibleDefinitions();
        Choices = new ObservableCollection<string>(definitions);
        for (int i = 0; i < ButtonGradients.Count; i++)
            ButtonGradients[i] = _greyGradient;
    }

    protected override void ShuffleWords()
    {
        base.ShuffleWords();
        SetWord();
    }

    private void CheckAnswer(int index)
    {
        // Return if the solution is already shown, so that editing the answer afterwards and with that changing the learning state is impossible
        if (IsSolutionShown)
            return;
        
        if (index >= Choices.Count || index < 0)
        {
            NextWord(false);
            return;
        }
        
        this.IsSolutionShown = true;

        string answer = Choices[index];
        bool correct = answer.Equals(Definition);

        if (correct)
        {
            ButtonGradients[index] = _greenGradient;
        }
        else
        {
            ButtonGradients[index] = _redGradient;
            int correctChoiceIndex = Choices.IndexOf(Definition, StringComparer.Ordinal);
            ButtonGradients[correctChoiceIndex] = _greenGradient;
        }
        
        Utilities.ChangeLearningState(CurrentWord, this, correct, considerOverallState: true);
    }

    private IEnumerable<string> DeterminePossibleDefinitions()
    {
        Choices.Clear();
        Choices.Add(Definition);
        var allWords = new List<Word>(WordsList);
        allWords.Remove(CurrentWord); // No duplicate definition in choices (prevent choosing seeked definition again)
        
        int similarWordProbability = Random.Shared.Next(0, _similarWordLimits[^1]);
        if (similarWordProbability < _similarWordLimits[0])
        {
            List<Word> similarWords = allWords.Where(x => Utilities.LevenshteinDistance(x.GetAdjustedDefinition(IsTermChosen), Definition) <= 2).ToList();
            if (similarWords.Count > 0)
            {
                int randomSimilarWord = Random.Shared.Next(0, similarWords.Count);
                Word similarWord = similarWords[randomSimilarWord];
                Choices.Add(similarWord.GetAdjustedDefinition(IsTermChosen));
                allWords.Remove(similarWord); // Prevent duplicate definition for further calculations
            }
        }

        if (Choices.Count == 4)
            return Choices.Shuffle();

        PartOfSpeech partOfSpeech = CurrentWord.SelectedPartOfSpeech;
        int threshold = 4 - Choices.Count;
        if (partOfSpeech == PartOfSpeech.None && CompleteMissingDefinitions(threshold, allWords, Choices, allWords, IsTermChosen))
            return Choices.Shuffle(); 
        
        IList<Word> possibleWords = allWords.Where(x => x.SelectedPartOfSpeech == partOfSpeech).ToList();
        List<string> possibleDefinitions = possibleWords.Select(x => x.GetAdjustedDefinition(IsTermChosen)).ToList();
        if (possibleDefinitions.Count == threshold)
        {
            Choices.AddRange(possibleDefinitions);
            return Choices.Shuffle();
        }
        else if (possibleDefinitions.Count > threshold)
        {
            if (CompleteMissingDefinitions(threshold, possibleWords, Choices, allWords, IsTermChosen))
                return Choices.Shuffle();
        }
        else
        {
            if (CompleteMissingDefinitions(threshold, possibleWords, Choices, allWords, IsTermChosen))
                return Choices.Shuffle();

            threshold = 4 - Choices.Count;
            int alternativeIndex = 0;
            while (threshold > 0)
            {
                if (_alternativePartOfSpeeches.ContainsKey(partOfSpeech) && 
                    alternativeIndex < _alternativePartOfSpeeches[partOfSpeech].Length)
                {
                    PartOfSpeech alternative = _alternativePartOfSpeeches[partOfSpeech][alternativeIndex];
                    possibleWords = allWords.Where(x => x.SelectedPartOfSpeech == alternative).ToList();
                    possibleDefinitions = possibleWords.Select(x => x.GetAdjustedDefinition(IsTermChosen)).ToList();
                    if (!possibleDefinitions.Any())
                    {
                        alternativeIndex++;
                        continue;
                    }
                    if (CompleteMissingDefinitions(threshold, possibleWords, Choices, allWords, IsTermChosen))
                        return Choices.Shuffle();

                    threshold = 4 - Choices.Count;
                    alternativeIndex++;
                    continue;
                }
                
                if (CompleteMissingDefinitions(threshold, allWords, Choices, allWords, IsTermChosen))
                    return Choices.Shuffle();
                throw new Exception("Something went terribly wrong. The choices couldn't be filled. [MULTIPLE CHOICE]");
            }
        }
        
        throw new Exception("Something went terribly wrong. The choices couldn't be filled. [MULTIPLE CHOICE]");
    }

    private static bool CompleteMissingDefinitions(int iterationCount, IList<Word> wordCollection, ICollection<string> targetList, 
        ICollection<Word> allWords, bool isTermChosen)
    {
        IList<string> collection = wordCollection.Select(x => x.GetAdjustedDefinition(isTermChosen)).ToList();
        for (int i = 0; i < iterationCount; i++)
        {
            if (wordCollection.Count <= 0)
                return false;

            int randomIndex = Random.Shared.Next(0, wordCollection.Count);
            targetList.Add(collection[randomIndex]);
            if (targetList.Count == 4)
                return true;
            
            collection.RemoveAt(randomIndex);
            allWords.Remove(wordCollection[randomIndex]);
            if(!ReferenceEquals(allWords, wordCollection))
                wordCollection.RemoveAt(randomIndex); // Must be after the line above, so that the word doesn't disappear
                                                      // from this collection before being accessed to remove it from `allWords`
        }

        return targetList.Count == 4;
    }

    private static LinearGradientBrush ConstructMultipleChoiceGradient(Color[] colors)
    {
        return Utilities.CreateLinearGradientBrush(
            new RelativePoint(0, 1, RelativeUnit.Relative),
            new RelativePoint(1, 0, RelativeUnit.Relative),
            colors,
            new[] { 0.3, 1.0 });
    }
}