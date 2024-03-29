using System;
using System.Collections.Generic;
using System.Linq;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.UtilityCollection;

public static partial class Utilities
{
    /// <summary>
    /// An instance of the <see cref="Random"/> class used to pick a random value in a specific range.
    /// </summary>
    private static readonly Random _random = new();
    
    /// <summary>
    /// An array of factors with which proportions are created for the choice of a random category with weightings.
    /// </summary>
    private static readonly float[] _categoryProportions = { 1, 2, 2, 1.25f, 1.4f }; // 1:2:4:5:7 (categories grouped together: 3:4:12)

    /// <summary>
    /// Categorize words based on their learning state.
    /// </summary>
    /// <param name="wordsList">A collection of <see cref="Word"/> instances that will be categorized.</param>
    /// <param name="learningMode">A <see cref="LearningModeType"/> value, representing the learning mode to be used to retrieve the <see cref="LearningState"/>.</param>
    /// <param name="categoryLimits">The values represent a range in which a category is chosen
    /// if the randomly generated value is within this range.</param>
    /// <returns>An <see cref="IReadOnlyList{T}"/> collection of categories, containing <see cref="Word"/> instances.</returns>
    public static IReadOnlyList<IReadOnlyList<Word>> CategorizeWords(IEnumerable<Word> wordsList, LearningModeType learningMode, out List<int> categoryLimits)
    {
        var categories = new List<IReadOnlyList<Word>>();
        categoryLimits = new List<int> { 0 };
        LearningState[] learningStates = Enum.GetValues<LearningState>().Reverse().ToArray();
        int cost = 0;
        bool modeNotThesaurus = learningMode != LearningModeType.Thesaurus;
        for(int i = 0; i < learningStates.Length; i++)
        {
            LearningState state = learningStates[i];
            Func<Word, bool> predicate = x => (modeNotThesaurus ? x.LearningStatus : x.LearningStateInModes[learningMode]).CustomHasFlag(state) 
                                              || (x.VocabularyReferences != null 
                                                  && x.VocabularyReferences.Any(y => y.LearningStateInModes[learningMode].CustomHasFlag(state)));
            
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (state == LearningState.NotAsked)
            {
                predicate = x => (modeNotThesaurus ? x.LearningStatus : x.LearningStateInModes[learningMode]) == state
                                     || (x.VocabularyReferences != null
                                         && x.VocabularyReferences.Any(y => y.LearningStateInModes[learningMode] == state));
            }
            else if(state == LearningState.VeryHard)
            {
                var predicateCopy = predicate;
                predicate = x => x.IsDifficult || predicateCopy(x);
            }
    
            List<Word> category = wordsList.Where(predicate).ToList();
            if (category.Count <= 0)
            {
                if (cost != 0)
                    cost = CalculateLimit(i, cost);
                continue;
            }
            
            int limit = CalculateLimit(i, categoryLimits.Count <= 1 ? 0 : cost) - cost + categoryLimits[^1];
            cost = limit;

            categoryLimits.Add(limit);
            categories.Add(category);
        }
        
        return categories;

        // Local function that calculates the next value in `categoryLimits`
        int CalculateLimit(int i, int oldValue) // Last calculated value + (proportions[0] * proportions[1] * ... * proportions[i])
            => (int)Math.Round(oldValue + _categoryProportions.Take(i + 1).Aggregate(1f, (a, b) => a * b));
    }

    /// <summary>
    /// Picks a random category with specified probabilities to be chosen for each category.
    /// </summary>
    /// <param name="wordsList"><inheritdoc cref="CategorizeWords" path="/param[@name='wordsList']"/></param>
    /// <param name="learningMode">A <see cref="LearningModeType"/> value, representing the learning mode to be used to retrieve the <see cref="LearningState"/>.</param>
    /// <returns> A list of <see cref="Word"/> instances, each one of them having the same <see cref="LearningState"/>.</returns>
    public static IReadOnlyList<Word> PickCategory(IEnumerable<Word> wordsList, LearningModeType learningMode)
    {
        var categories = CategorizeWords(wordsList, learningMode, out List<int> categoryLimits);
        if (categories.Count <= 0 || categoryLimits.Count <= 0)
            throw new Exception("There are no categories to choose from.");
        
        int value = _random.Next(0, categoryLimits[^1] + 1);
        for(int i = 1; i < categoryLimits.Count; i++)
        {
            if(value >= categoryLimits[i-1] && value < categoryLimits[i])
                return categories[i-1];
        }
        
        return categories[^1]; // Access last category (VeryHard and star) in case of a failure; equivalent to `categories.Last()`
    }
}