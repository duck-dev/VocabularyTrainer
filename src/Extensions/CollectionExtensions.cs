using System;
using System.Collections.Generic;
using System.Linq;

namespace VocabularyTrainer.Extensions;

public static partial class Extensions
{
    /// <summary>
    /// Create a deep copy of an <see cref="IEnumerable{T}"/> collection.
    /// </summary>
    /// <param name="collection">The list to be cloned.</param>
    /// <typeparam name="T">The type of the elements inside the collection. All of them ought to inherit <see cref="ICloneable"/>
    /// </typeparam>
    /// <returns>The cloned collection.</returns>
    public static IEnumerable<T> Clone<T>(this IEnumerable<T> collection) where T : ICloneable 
        => collection.Select(x => (T)x.Clone());
}