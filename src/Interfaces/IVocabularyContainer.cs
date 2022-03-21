using System.Collections.ObjectModel;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.Interfaces
{
    public interface IVocabularyContainer<T> where T : VocabularyItem
    {
        ObservableCollection<T> VocabularyItems { get; }
    }
}