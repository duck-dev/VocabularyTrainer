using System;
using System.Globalization;
using System.Text;

namespace VocabularyTrainer.Extensions;

public static partial class Extensions
{
    private static readonly char[] _punctuationCharacters =
    {
        '.', ',', '?', '!', ':', ';'
    };

    /// <summary>
    /// Replace characters with diacritics (accent marks) with their equivalent letter (e.g. "é" -> "e").
    /// This solution works for all very common letters with accent marks and most less common letters too.
    /// </summary>
    /// <param name="originalString">The original string.</param>
    /// <returns>A string with all letters with diacritics (accent marks) being replaced by their equivalent letter.</returns>
    public static string RemoveDiacritics(this string originalString)
    {
        string normalizedString = originalString.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(capacity: normalizedString.Length);
        
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (Rune c in normalizedString.EnumerateRunes())
        {
            var unicodeCategory = Rune.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                stringBuilder.Append(c);
        }
        
        string cleanString = stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        
        // The following 3 lines would cover more characters, but any non-latin characters will get distorted (e.g. Cyrillic, Chinese, Japanese etc.)
        // Apart from that many characters that aren't covered drastically change the meaning, so ignoring the accent mark might not be the best idea.
        // This part will only work if the NuGet package "System.Text.Encoding.CodePages" is installed...
        // ... and `Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)` is called (in the constructor of `MainWindowViewModel`)
        
        // byte[] tempBytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(cleanString);
        // string asciiStr = Encoding.UTF8.GetString(tempBytes);
        // return asciiStr;
        return cleanString;
    }

    /// <summary>
    /// Remove all punctuation marks from a string. The characters are defined in the <see cref="_punctuationCharacters"/> array.
    /// </summary>
    /// <param name="s">The original string.</param>
    /// <returns>A string without punctuation marks.</returns>
    public static string RemovePunctuation(this string s)
    {
        foreach (char c in _punctuationCharacters)
            s = s.Replace(c.ToString(), string.Empty);
        return s;
    }

    /// <summary>
    /// Remove all kinds of hyphens, dashes and minuses from a string.
    /// </summary>
    /// <param name="s">The original string.</param>
    /// <returns>A string without hyphens, dashes and/or minuses.</returns>
    public static string RemoveHyphens(this string s)
    {
        return s.Replace("\u2014", " ").Replace("\u2013", " ")
            .Replace("\u2012", " ").Replace("\u2011", " ").Replace("\u2010", " ")
            .Replace("\u2212", " ").Replace("\u002D", " ").Replace("\u29FF", " ")
            .Replace("\uFE63", " ").Replace("\uFF0D", " ");
    }

    /// <summary>
    /// Trim unnecessary spaces from a string (several spaces next to each other => only one space), as well as leading and trailing spaces.
    /// </summary>
    /// <param name="s">The original string.</param>
    /// <returns>A string without unnecessary spaces.</returns>
    public static string TrimUnnecessarySpaces(this string s)
    {
        var stringBuilder = new StringBuilder(capacity: s.Length);
        for(int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if (!char.IsWhiteSpace(c) || (char.IsWhiteSpace(c) && !char.IsWhiteSpace(s[i - 1])))
                stringBuilder.Append(c);
        }

        return stringBuilder.ToString().Trim();
    }

    /// <summary>
    /// Calculate the length of the longest common substring of two strings.
    /// </summary>
    /// <param name="a">The string supposedly containing common substrings.</param>
    /// <param name="b">The string compared to <paramref name="a"/>, containing the characters for the substring.</param>
    /// <returns>The length of the longest common substring.</returns>
    public static int LongestCommonSubstringLength(this string a, string b)
    {
        int m = a.Length;
        int n = b.Length;
        int[,] matrix = new int[m+1,n+1];
        int result = 0;

        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                if (i == 0 || j == 0)
                {
                    matrix[i, j] = 0;
                }
                else if (a[i-1] == b[j-1])
                {
                    matrix[i, j] = matrix[i-1,j-1] + 1;
                    result = Math.Max(result, matrix[i, j]);
                }
                else
                {
                    matrix[i, j] = 0;
                }
            }
        }
        
        return result;
    }
}