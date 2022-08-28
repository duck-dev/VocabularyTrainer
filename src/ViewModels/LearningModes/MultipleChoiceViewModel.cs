using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicData;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Models;
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

    public MultipleChoiceViewModel(Lesson lesson) : base(lesson)
    {
        VerifyAndSetItem(SetWord);
    }

    private ObservableCollection<string> Choices { get; } = new();

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
        AddPossibleDefinitions();
    }

    protected override void ShuffleWords()
    {
        base.ShuffleWords();
        SetWord();
    }

    private void CheckAnswer(string answer)
    {
        // TODO: Start transition for button colors (green for correct one, red for wrong given answer if necessary)
        // TODO: Enable button for next word
        Utilities.ChangeLearningState(CurrentWord, this, answer.Equals(Definition), considerOverallState: true);
    }

    private void AddPossibleDefinitions()
    {
        PossibleDefinitions = new List<string> {Definition};
        var allWords = new List<Word>(WordsList);
        allWords.Remove(CurrentWord); // No duplicate definition in choices (prevent choosing seeked definition again)
        
        int similarWordProbability = Random.Shared.Next(0, _similarWordLimits[^1]);
        if (similarWordProbability < _similarWordLimits[0])
        {
            var similarWords = allWords.Where(x => Utilities.LevenshteinDistance(x.Definition, Definition) <= 2).ToList();
            if (similarWords.Count > 0)
            {
                int randomSimilarWord = Random.Shared.Next(0, similarWords.Count);
                Word similarWord = similarWords[randomSimilarWord];
                Choices.Add(similarWord.Definition);
                allWords.Remove(similarWord); // Prevent duplicate definition for further calculations
            }
        }

        if (Choices.Count == 4)
            return; // TODO: Do something with `list`

        PartOfSpeech partOfSpeech = CurrentWord.SelectedPartOfSpeech;
        IEnumerable<Word> possibleWords = allWords.Where(x => x.SelectedPartOfSpeech == partOfSpeech);
        List<string> possibleDefinitions = possibleWords.Select(x => x.Definition).ToList();
        int threshold = 4 - Choices.Count;
        if (possibleDefinitions.Count == threshold)
        {
            Choices.AddRange(possibleDefinitions);
        }
        else if (possibleDefinitions.Count > threshold)
        {
            if (CompleteMissingDefinitions(threshold, possibleDefinitions, Choices, allWords))
                return;
        }
        else
        {
            if (CompleteMissingDefinitions(threshold, possibleDefinitions, Choices, allWords))
                return;
            threshold = 4 - Choices.Count;
            var completedDefinitions = new List<string>(possibleDefinitions);
            int alternativeIndex = 0;
            while (completedDefinitions.Count < threshold)
            {
                if (_alternativePartOfSpeeches.ContainsKey(partOfSpeech) && 
                    alternativeIndex < _alternativePartOfSpeeches[partOfSpeech].Length)
                {
                    PartOfSpeech alternative = _alternativePartOfSpeeches[partOfSpeech][alternativeIndex];
                    possibleWords = allWords.Where(x => x.SelectedPartOfSpeech == alternative);
                    if (!possibleWords.Any())
                    {
                        alternativeIndex++;
                        continue;
                    }
                    possibleDefinitions = possibleWords.Select(x => x.Definition).ToList();
                    if (CompleteMissingDefinitions(threshold, possibleDefinitions, Choices, allWords))
                        return;

                    alternativeIndex++;
                    continue;
                }

                if (CompleteMissingDefinitions(threshold, allWords.Select(x => x.Definition).ToList(), Choices, allWords))
                    return;
                throw new Exception("Something went terribly wrong. The choices couldn't be filled. [MULTIPLE CHOICE]");
            }
        }
    }

    private static bool CompleteMissingDefinitions(int iterationCount, IList<string> collection, ICollection<string> targetList, IList allWords)
    {
        for (int i = 0; i < iterationCount; i++)
        {
            if (collection.Count <= 0)
                return false;
            int randomIndex = Random.Shared.Next(0, collection.Count);
            targetList.Add(collection[randomIndex]);
            collection.RemoveAt(randomIndex);
            allWords.RemoveAt(randomIndex);
        }

        return targetList.Count == 4;
    }
}