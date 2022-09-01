using VocabularyTrainer.Models;

namespace VocabularyTrainer.Extensions;

public static partial class Extensions
{
    /// <summary>
    /// Return what is considered as the displayed term from a <see cref="DualVocabularyItem"/>.
    /// </summary>
    /// <param name="word">The word to get the term/definition from.</param>
    /// <param name="isTermChosen">True if the actual term of the word is displayed as the term to define,
    /// False if the definition is displayed and the user needs to answer with the term.</param>
    /// <returns>The adjusted term as a string.
    /// If the actual term of a word is displayed as the term to define, <see cref="DualVocabularyItem.Term"/> is returned.
    /// In case the definition is the displayed term, <see cref="DualVocabularyItem.Definition"/> is returned.</returns>
    public static string GetAdjustedTerm(this DualVocabularyItem word, bool isTermChosen) 
        => isTermChosen ? word.Term : word.Definition;

    /// <summary>
    /// Return what is considered as the displayed definition from a <see cref="DualVocabularyItem"/>.
    /// </summary>
    /// <param name="word">The word to get the term/definition from.</param>
    /// <param name="isTermChosen">True if the actual term of the word is displayed as the term to define,
    /// False if the definition is displayed and the user needs to answer with the term.</param>
    /// <returns>The adjusted definition as a string.
    /// If the actual term of a word is displayed as the term to define, <see cref="DualVocabularyItem.Definition"/> is returned.
    /// In case the definition is the displayed term and the term is seeked as the answer, <see cref="DualVocabularyItem.Term"/> is returned.</returns>
    public static string GetAdjustedDefinition(this DualVocabularyItem word, bool isTermChosen)
        => isTermChosen ? word.Definition : word.Term;
}