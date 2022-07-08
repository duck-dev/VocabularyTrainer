using System.Collections.Generic;
using System.Linq;

namespace VocabularyTrainer.Models.EqualityComparers;

/// <summary>
/// Compares the equality of two <see cref="Word"/> instances
/// based on the <see cref="Word.Definition"/>, the <see cref="Word.Synonyms"/> and the <see cref="Word.Antonyms"/>.
/// It doesn't consider the <see cref="Word.Term"/> or the order of synonyms and antonyms.
/// This equality comparer is specifically designed for the "Synonyms and Antonyms" learning mode.
/// </summary>
public class WordEqualityComparer : IEqualityComparer<Word>
{
    public bool Equals(Word? x, Word? y)
    {
        if (x is null && y is null)
            return true;
        return x is not null 
               && y is not null 
               && x.Definition.Equals(y.Definition) 
               && x.Synonyms.SequenceEqual(y.Synonyms) 
               && x.Antonyms.SequenceEqual(y.Antonyms);
    }

    public int GetHashCode(Word obj)
    {
        int synonymsHash = 0;
        int antonymsHash = 0;
        foreach (VocabularyItem synonym in obj.Synonyms)
            synonymsHash ^= new VocabularyItemEqualityComparer().GetHashCode(synonym);
        foreach (VocabularyItem antonym in obj.Antonyms)
            antonymsHash ^= new VocabularyItemEqualityComparer().GetHashCode(antonym);

        return obj.Definition.GetHashCode() ^ synonymsHash ^ antonymsHash;
    }
}