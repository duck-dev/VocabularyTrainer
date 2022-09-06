using System;
using System.Collections.Generic;
using System.Linq;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.UtilityCollection;

public static partial class Utilities
{
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
    /// <param name="settings">The relevant settings.</param>
    /// <returns>A modified string, according to the selected lesson options.</returns>
    public static string ModifyAnswer(string originalString, LessonOptions settings)
    {
        string result = originalString;
        
        if (settings.IgnoreAccentMarks)
            result = result.RemoveDiacritics();
        if (settings.IgnoreHyphens)
            result = result.RemoveHyphens();
        if (settings.IgnorePunctuation)
            result = result.RemovePunctuation();
        if (settings.IgnoreCapitalization)
            result = result.ToUpperInvariant();

        result = result.Trim().TrimUnnecessarySpaces();
        return result;
    }
}