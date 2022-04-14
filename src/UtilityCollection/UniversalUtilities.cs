using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.UtilityCollection
{
    public static partial class Utilities
    {
        public delegate void NotifyItemAddedEventHandler(VocabularyItem item);
        public static event NotifyItemAddedEventHandler? NotifyItemAdded;
        
        /// <summary>
        /// The parent path of all settings- and data-files
        /// </summary>
        public static string FilesParentPath
        {
            get
            {
                var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                return directory ?? throw new DirectoryNotFoundException("Directory name of the currently executing assembly is null.");
            }
        }
        
        /// <summary>
        /// Log a message to the console (for debugging purposes).
        /// </summary>
        /// <param name="message">The message to be logged as a string.</param>
        public static void Log(string? message) => System.Diagnostics.Trace.WriteLine(message);
        
        /// <summary>
        /// Add added/removed elements of a collection whose changes should be resettable to a list
        /// that is used to reset the changes if necessary.
        /// </summary>
        /// <param name="collection">The collection, which will keep track of all changes to the original collection
        /// (added/removed items), so it can be used to reset those changes.</param>
        /// <param name="args">Arguments of the CollectionChanged event of a <see cref="ObservableCollection{T}"/>
        /// with the changed (added/removed) items and other information about the changes.</param>
        /// <typeparam name="T">The type of the collection items.</typeparam>
        public static void AddChangedItems<T>(ICollection<T> collection, NotifyCollectionChangedEventArgs args) 
            where T : VocabularyItem, IContentVerification<T>
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newItems = args.NewItems?.Cast<T>();
                    LoopItems(newItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var oldItems = args.OldItems?.Cast<T>();
                    LoopItems(oldItems);
                    break;
            }

            void LoopItems(IEnumerable<T>? items)
            {
                if (items is null)
                    return;
                
                foreach (var item in items)
                {
                    item.ChangedAction = args.Action;
                    NotifyItemAdded?.Invoke(item);
                    if (CheckUnsavedContent(item, collection))
                        continue;
                    
                    if (collection.Contains(item))
                        collection.Remove(item);
                    else
                        collection.Add(item);
                }
            }
        }

        /// <summary>
        /// Check if an item resolves an unsaved removal of a vocabulary item.
        /// </summary>
        /// <param name="item">The new item that could potentially resolve the unsaved removal.</param>
        /// <param name="collection">The collection that contains unsolved removals and additions.</param>
        /// <typeparam name="T">Type that implements <see cref="IContentVerification{T}"/></typeparam>
        /// <returns>Whether the item resolved the unsaved change or not.</returns>
        public static bool CheckUnsavedContent<T>(T item, ICollection<T> collection) where T : IContentVerification<T>
        {
            if (!item.MatchesUnsavedContent(collection, out T? identicalItem)) 
                return false;
            
            collection.Remove(identicalItem!);
            collection.Remove(item);
            item.EqualizeChangedData();
            
            return true;
        }
    }
}