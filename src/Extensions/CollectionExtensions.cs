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
    
    /// <summary>
    /// Create a deep copy of a collection of type <typeparamref name="TCollection"/>.
    /// </summary>
    /// <param name="collection"><inheritdoc cref="Clone{T}"/></param>
    /// <typeparam name="TCollection">The type of the extended collection, which must implement <see cref="IEnumerable{T}"/>.</typeparam>
    /// <typeparam name="TItem">The type of the elements inside the collection. All of them ought to inherit <see cref="ICloneable"/></typeparam>
    /// <returns><inheritdoc cref="Clone{T}"/></returns>
    public static TCollection? Clone<TCollection, TItem>(this TCollection collection)
        where TCollection : class, IEnumerable<TItem> 
        where TItem : ICloneable
    {
        var newEnumerable = collection.Select(x => (TItem)x.Clone());
        return Activator.CreateInstance(typeof(TCollection), newEnumerable) as TCollection;
    }

    /// <summary>
    /// Shuffles the elements of an <see cref="IEnumerable{T}"/> in a random order.
    /// </summary>
    /// <param name="collection">The extended collection to be shuffled.</param>
    /// <typeparam name="T">The generic type of the collection's elements.</typeparam>
    /// <returns>The shuffled collection as an <see cref="IEnumerable{T}"/></returns>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
        => collection.OrderBy(_ => Random.Shared.Next());
}