using System;
using System.Collections.Generic;
using System.Linq;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Models;
using VocabularyTrainer.ViewModels.LearningModes;

namespace VocabularyTrainer.UtilityCollection;

public static partial class Utilities
{
    /// <summary>
    /// Add a specific <see cref="LearningState"/> to a <see cref="Word"/> in a specific learning mode.
    /// </summary>
    /// <param name="word"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool)"/></param>
    /// <param name="singleWordViewModel"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool)"/></param>
    /// <param name="state">The state to be added.</param>
    public static void AddLearningState(VocabularyItem word, SingleWordViewModelBase singleWordViewModel, LearningState state)
    {
        LearningModeType learningMode = singleWordViewModel.LearningMode;
        LearningState result = word.LearningStateInModes[learningMode];
        result |= state;
            
        ChangeLearningState(word, singleWordViewModel, result);
    }

    /// <summary>
    /// Remove a specific <see cref="LearningState"/> from a <see cref="Word"/> in a specific learning mode.
    /// </summary>
    /// <param name="word"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool)"/></param>
    /// <param name="singleWordViewModel"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool)"/></param>
    /// <param name="state">The state to be removed.</param>
    public static void RemoveLearningState(VocabularyItem word, SingleWordViewModelBase singleWordViewModel, LearningState state)
    {
        LearningModeType learningMode = singleWordViewModel.LearningMode;
        LearningState result = word.LearningStateInModes[learningMode];
        result &= ~state;
            
        ChangeLearningState(word, singleWordViewModel, result);
    }

    /// <summary>
    /// Automatically change the <see cref="LearningState"/> of a <see cref="Word"/> in a specific learning mode by one step.
    /// </summary>
    /// <param name="word">The <see cref="Word"/> whose learning state should be changed.</param>
    /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
    /// <param name="known">Did the user answer correctly or not?</param>
    public static void ChangeLearningState(VocabularyItem word, SingleWordViewModelBase singleWordViewModel, bool known)
    {
        LearningModeType learningMode = singleWordViewModel.LearningMode;
        LearningState originalState = LearningState.NotAsked;
        LearningState newState = LearningState.NotAsked;
        foreach (VocabularyItem item in word.VocabularyReferences)
        {
            originalState = item.LearningStateInModes[learningMode];
            newState = originalState;

            if (originalState == LearningState.NotAsked)
            {
                ChangeLearningState(item, singleWordViewModel, known ? LearningState.KnownOnce : LearningState.WrongOnce);
                continue;
            }

            newState &= ~LearningState.NotAsked;
            newState = known ? newState.Next(false, LearningState.NotAsked) 
                : newState.Previous(false, LearningState.NotAsked);

            item.LearningStateInModes[learningMode] = newState;
        }
        singleWordViewModel.VisualizeLearningProgress(originalState, newState);
    }
        
    /// <summary>
    /// Change the <see cref="LearningState"/> of a <see cref="Word"/> in a specific learning mode to a specified value.
    /// </summary>
    /// <param name="word"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool)"/></param>
    /// <param name="singleWordViewModel"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool)"/></param>
    /// <param name="result">The value (<see cref="LearningState"/>) to be set.</param>
    public static void ChangeLearningState(VocabularyItem word, SingleWordViewModelBase singleWordViewModel, LearningState result)
    {
        LearningModeType learningMode = singleWordViewModel.LearningMode;
        LearningState previousState = LearningState.NotAsked;
        bool foundMatchingRef = false;
        foreach (VocabularyItem item in word.VocabularyReferences)
        {
            if (!item.LearningStateInModes.ContainsKey(learningMode))
                return;
            foundMatchingRef = true;
            previousState = item.LearningStateInModes[learningMode];
            item.LearningStateInModes[learningMode] = result;
        }
        if(foundMatchingRef)
            singleWordViewModel.VisualizeLearningProgress(previousState, result);
    }

    /// <summary>
    /// An implementation of the Damerau-Levenshtein-Distance to calculate the needed number of edits to make the two given
    /// strings exactly equal,
    /// considering edits as either insertions, deletions, substitutions or transpositions of two adjacent characters (Damerau).
    /// </summary>
    /// <param name="a">First string to compare.</param>
    /// <param name="b">Second string to compare.</param>
    /// <param name="ignoreTransposition">Ignore the transposition of adjacent characters (swapped letters) => no mistake.</param>
    /// <returns>The number of needed edits to make the two given strings equal.</returns>
    public static int LevenshteinDistance(string a, string b, bool ignoreTransposition = false)
    {
        int n = a.Length;
        int m = b.Length;
            
        if (n == 0)
            return m;
        if (m == 0)
            return n;
            
        int[,] matrix = new int[n+1, m+1];

        for (int i = 0; i <= n; matrix[i,0] = i++) { }
        for (int j = 0; j <= m; matrix[0,j] = j++) { }

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = a[i-1] == b[j-1] ? 0 : 1;
                IEnumerable<int> values = new[]
                {
                    matrix[i-1, j] + 1, // Deletion
                    matrix[i, j-1] + 1, // Insertion
                    matrix[i-1, j-1] + cost // Substitution
                };
                int distance = matrix[i, j] = values.Min();
                if (i > 1 && j > 1 && a[i-1] == b[j-2] && a[i-2] == b[j-1])
                {
                    if (ignoreTransposition)
                        cost = 0;
                    distance = Math.Min(distance, matrix[i-2, j-2] + cost); // Transposition
                }

                matrix[i, j] = distance;
            }
        }

        return matrix[n, m];
    }

    /// <summary>
    /// Modify a specific string to comply with the lesson options.
    /// </summary>
    /// <param name="originalString">The original unmodified string.</param>
    /// <param name="lesson">The current lesson that contains the relevant settings.</param>
    /// <returns>A modified string, according to the selected lesson options.</returns>
    public static string ModifyAnswer(string originalString, Lesson lesson)
    {
        string result = originalString;
            
        var settings = lesson.Options;
        if (settings.IgnoreAccentMarks)
            result = result.RemoveDiacritics();
        if (settings.IgnoreHyphens)
            result = result.RemoveHyphens();
        if (settings.IgnorePunctuation)
            result = result.RemovePunctuation();
        if (settings.IgnoreCapitalization)
            result = result.ToLowerInvariant();

        result = result.Trim().TrimUnnecessarySpaces();
        return result;
    }
}