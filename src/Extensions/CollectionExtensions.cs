using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ReactiveUI;
using VocabularyTrainer.Interfaces;

namespace VocabularyTrainer.Extensions
{
    public static partial class Extensions
    {
        public static void CalculateIndex<T>(this ObservableCollection<T> collection, bool updateInstantly = false) 
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
        
        public static void CalculateIndexReactive<TItem, TParent>(this ObservableCollection<TItem> collection, 
            TParent callerParent, bool updateInstantly = false, params string[] propertiesToUpdate) 
            where TItem : class, IIndexable where TParent : ReactiveObject
        {
            CalculateIndex(collection, updateInstantly);
            collection.CollectionChanged += (sender, args) =>
            {
                foreach (string property in propertiesToUpdate)
                    callerParent.RaisePropertyChanged(property);
            };
        }

        public static void CalculateIndex<TItem, TParent>(this ObservableCollection<TItem> collection, 
            TParent callerParent, bool updateInstantly = false, params string[] propertiesToUpdate) 
            where TItem : class, IIndexable where TParent : INotifyPropertyChangedHelper
        {
            CalculateIndex(collection, updateInstantly);
            collection.CollectionChanged += (sender, args) =>
            {
                foreach (string property in propertiesToUpdate)
                    callerParent.NotifyPropertyChanged(property);
            };
        }
    }
}