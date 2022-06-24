using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using ReactiveUI;
using VocabularyTrainer.Interfaces;

namespace VocabularyTrainer.Extensions;

public static partial class Extensions
{
    /// <summary>
    /// Calculate the new index of the elements in the extended <see cref="ObservableCollection{T}"/>.
    /// This method is used to numerate elements and update their index according to their (new) position in the collection.
    /// </summary>
    /// <param name="collection">The <see cref="ObservableCollection{T}"/> with elements that are supposed to be numerated.</param>
    /// <param name="updateInstantly">Should the index be updated right at the beginning?
    /// This is advised if there can be more than 1 element in the collection when this method is called, otherwise
    /// the index would be 0 + 1 by default for each element in the collection.</param>
    /// <typeparam name="T">The generic type of the <see cref="ObservableCollection{T}"/>, which must
    /// implement the <see cref="IIndexable"/> interface.</typeparam>
    public static void CalculateIndex<T>(this ObservableCollection<T> collection, bool updateInstantly = true) 
        where T : class, IIndexable
    {
        collection.CollectionChanged += UpdateIndex;
        if(updateInstantly)
            UpdateIndex(null,null);
            
        void UpdateIndex(object? sender, NotifyCollectionChangedEventArgs? args)
        {
            for (int i = 0; i < collection.Count; i++)
                collection[i].Index = i;
        }
    }
        
    /// <summary>
    /// <inheritdoc cref="CalculateIndex{T}"/>
    /// Additionally, it notifies changed properties of a <see cref="INotifyPropertyChangedHelper"/>, which
    /// in turn implements <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    /// <param name="collection"><inheritdoc cref="CalculateIndex{T}"/></param>
    /// <param name="callerParent">The <see cref="INotifyPropertyChangedHelper"/> that contains properties
    /// to be notified about their changes. (See: <see cref="propertiesToUpdate"/>)</param>
    /// <param name="updateInstantly"><inheritdoc cref="CalculateIndex{T}"/></param>
    /// <param name="propertiesToUpdate">An array of property names to be notified about the change of their respective properties.</param>
    /// <typeparam name="TItem"><inheritdoc cref="CalculateIndex{T}"/></typeparam>
    /// <typeparam name="TParent">A type that implements <see cref="INotifyPropertyChangedHelper"/>, which in turn
    /// implements <see cref="INotifyPropertyChanged"/></typeparam>
    public static void CalculateIndex<TItem, TParent>(this ObservableCollection<TItem> collection, 
        TParent callerParent, bool updateInstantly = true, params string[] propertiesToUpdate) 
        where TItem : class, IIndexable where TParent : INotifyPropertyChangedHelper
    {
        CalculateIndex(collection, updateInstantly);
        collection.CollectionChanged += (sender, args) =>
        {
            foreach (string property in propertiesToUpdate)
                callerParent.NotifyPropertyChanged(property);
        };
    }
        
    /// <summary>
    /// <inheritdoc cref="CalculateIndex{T}"/>
    /// Additionally, it notifies changed properties of a class that inherits <see cref="ReactiveObject"/>.
    /// </summary>
    /// <param name="collection"><inheritdoc cref="CalculateIndex{T}"/></param>
    /// <param name="callerParent">The class that inherits <see cref="ReactiveObject"/> that contains properties
    /// to be notified about their changes. (See: <see cref="propertiesToUpdate"/>)</param>
    /// <param name="updateInstantly"><inheritdoc cref="CalculateIndex{T}"/></param>
    /// <param name="propertiesToUpdate"><inheritdoc cref="CalculateIndexReactive{TItem,TParent}"/></param>
    /// <typeparam name="TItem"><inheritdoc cref="CalculateIndex{T}"/></typeparam>
    /// <typeparam name="TParent">A class that inherits <see cref="ReactiveObject"/> to access the functionality
    /// for notifying property changes and updating the UI properly.</typeparam>
    public static void CalculateIndexReactive<TItem, TParent>(this ObservableCollection<TItem> collection, 
        TParent callerParent, bool updateInstantly = true, params string[] propertiesToUpdate) 
        where TItem : class, IIndexable where TParent : ReactiveObject
    {
        CalculateIndex(collection, updateInstantly);
        collection.CollectionChanged += (sender, args) =>
        {
            foreach (string property in propertiesToUpdate)
                callerParent.RaisePropertyChanged(property);
        };
    }
}