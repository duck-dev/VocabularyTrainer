using System.Collections.Generic;

namespace VocabularyTrainer.Models.EqualityComparers;

/// <summary>
/// Compares the equality of two <see cref="VocabularyItem"/> instances based on the <see cref="VocabularyItem.Definition"/>.
/// </summary>
public class VocabularyItemEqualityComparer : IEqualityComparer<VocabularyItem>
{
    public bool Equals(VocabularyItem? x, VocabularyItem? y)
    {
        if (x is null && y is null)
            return true;
        return x is not null && y is not null && x.Definition.Equals(y.Definition);
    }

    public int GetHashCode(VocabularyItem obj)
    {
        return obj.Definition.GetHashCode();
    }
}