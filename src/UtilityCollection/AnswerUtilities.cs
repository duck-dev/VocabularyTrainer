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
    /// <param name="word"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool,bool)"/></param>
    /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
    /// <param name="state">The state to be added.</param>
    public static void AddLearningState(VocabularyItem word, SingleWordViewModelBase singleWordViewModel, LearningState state)
    {
        LearningModeType learningMode = singleWordViewModel.LearningMode;
        LearningState result = word.LearningStateInModes[learningMode];
        result |= state;
        InvokeChangingState(word, singleWordViewModel, result);
    }

    /// <summary>
    /// Remove a specific <see cref="LearningState"/> from a <see cref="Word"/> in a specific learning mode.
    /// </summary>
    /// <param name="word"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool,bool)"/></param>
    /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
    /// <param name="state">The state to be removed.</param>
    public static void RemoveLearningState(VocabularyItem word, SingleWordViewModelBase singleWordViewModel, LearningState state)
    {
        LearningModeType learningMode = singleWordViewModel.LearningMode;
        LearningState result = word.LearningStateInModes[learningMode];
        result &= ~state;
        InvokeChangingState(word, singleWordViewModel, result);
    }

    /// <summary>
    /// Automatically change the <see cref="LearningState"/> of a <see cref="Word"/> in a specific learning mode by one step.
    /// </summary>
    /// <param name="word">The <see cref="Word"/> whose learning state should be changed.</param>
    /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
    /// <param name="known">Did the user answer correctly or not?</param>
    /// <param name="visualize">Whether the progress should be visualized in the progress bars. This is always desired, 
    /// unless this method is called from <see cref="ChangeLearningStateThesaurus"/>.</param>
    public static void ChangeLearningState(VocabularyItem word, SingleWordViewModelBase singleWordViewModel, bool known, bool visualize = true)
    {
        LearningModeType learningMode = singleWordViewModel.LearningMode;
        LearningState originalState = word.LearningStateInModes[learningMode];
        LearningState newState = originalState;
        
        if (originalState == LearningState.NotAsked)
        {
            ChangeLearningState(word, singleWordViewModel, known ? LearningState.KnownOnce : LearningState.WrongOnce, visualize);
            return;
        }

        newState &= ~LearningState.NotAsked;
        newState = known ? newState.Next(false, LearningState.NotAsked) 
            : newState.Previous(false, LearningState.NotAsked);
        
        word.LearningStateInModes[learningMode] = newState;
        if(visualize)
            singleWordViewModel.VisualizeLearningProgress(originalState, newState);
    }

    /// <summary>
    /// Change the <see cref="LearningState"/> of a <see cref="Word"/> in a specific learning mode to a specified value.
    /// </summary>
    /// <param name="word"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool,bool)"/></param>
    /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
    /// <param name="result">The value (<see cref="LearningState"/>) to be set.</param>
    /// <param name="visualize">Whether the progress should be visualized in the progress bars. This is always desired, 
    /// unless this method is called from <see cref="ChangeLearningStateThesaurus"/>.</param>
    public static void ChangeLearningState(VocabularyItem word, SingleWordViewModelBase singleWordViewModel, LearningState result, bool visualize = true)
    {
        LearningModeType learningMode = singleWordViewModel.LearningMode;
        LearningState previousState = word.LearningStateInModes[learningMode];
        word.LearningStateInModes[learningMode] = result;
        if(visualize)
            singleWordViewModel.VisualizeLearningProgress(previousState, result);
    }

    public static void ChangeLearningStateThesaurus(VocabularyItem word, SingleWordViewModelBase singleWordViewModel, bool known)
    {
        if (word.VocabularyReferences is null)
            return;

        // LearningState originalState = word.LearningStateInModes[LearningModeType.Thesaurus]; // Remove
        // Log("\n\n\nChangeLearningStateThesaurus:\n------------------------"); // Remove
        // Log($"    {originalState}... detailed inspection following...\n---------------"); // Remove
        foreach (VocabularyItem item in word.VocabularyReferences)
            ChangeLearningState(item, singleWordViewModel, known, false);
        ChangeLearningState(word, singleWordViewModel, known);
        // LearningState newState = word.LearningStateInModes[LearningModeType.Thesaurus]; // Remove
        // Log($"    \n{newState}... done!\n------------------------------------------------------------------------\n"); // Remove
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
    
    /// <summary>
    /// This method is used by <see cref="AddLearningState"/> and <see cref="RemoveLearningState"/> to call
    /// <see cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,LearningState,bool)"/>
    /// for the passed <paramref name="word"/> and for all items in it's <see cref="VocabularyItem.VocabularyReferences"/> collection./>
    /// </summary>
    /// <param name="word"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,LearningState,bool)"/></param>
    /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
    /// <param name="result">The value (<see cref="LearningState"/>) to be set.</param>
    private static void InvokeChangingState(VocabularyItem word, SingleWordViewModelBase singleWordViewModel, LearningState result)
    {
        if (word.VocabularyReferences is {Count: > 0})
        {
            foreach(VocabularyItem item in word.VocabularyReferences)
                ChangeLearningState(item, singleWordViewModel, result, false);
        }
        ChangeLearningState(word, singleWordViewModel, result);
    }
}