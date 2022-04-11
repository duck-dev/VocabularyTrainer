using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ReactiveUI;
using VocabularyTrainer.Interfaces;

namespace VocabularyTrainer.Extensions
{
    public static partial class Extensions
    {
        public static void CalculateIndexReactive<TItem, TParent>(this ObservableCollection<TItem> collection, 
            TParent callerParent, bool updateInstantly = false, params string[] propertiesToUpdate) 
            where TItem : class, IIndexable where TParent : ReactiveObject
        {
            collection.CollectionChanged += UpdateIndex;
            UpdateIndex(null,null);
            
            void UpdateIndex(object? sender, NotifyCollectionChangedEventArgs? args)
            {
                foreach(string property in propertiesToUpdate)
                    callerParent.RaisePropertyChanged(property);
                for (int i = 0; i < collection.Count; i++)
                    collection[i].Index = i;
            }
        }

        public static void CalculateIndex<TItem, TParent>(this ObservableCollection<TItem> collection, 
            TParent callerParent, bool updateInstantly = false, params string[] propertiesToUpdate) 
            where TItem : class, IIndexable where TParent : INotifyPropertyChangedHelper
        {
            collection.CollectionChanged += UpdateIndex;
            UpdateIndex(null,null);

            void UpdateIndex(object? sender, NotifyCollectionChangedEventArgs? args)
            {
                foreach(string property in propertiesToUpdate)
                    callerParent.NotifyPropertyChanged(property);
                for (int i = 0; i < collection.Count; i++)
                    collection[i].Index = i;
            }
        }
    }
}