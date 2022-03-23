using System.Collections.ObjectModel;
using ReactiveUI;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels
{
    public class AddLessonViewModel : ViewModelBase, IVocabularyContainer<Word>
    {
        public AddLessonViewModel()
        {
            VocabularyItems.CollectionChanged += (sender, args) =>
            {
                this.RaisePropertyChanged(nameof(AdjustableItemsString));
                for (int i = 0; i < VocabularyItems.Count; i++)
                    VocabularyItems[i].Index = i;
            };
        }
        
        private string? CurrentName { get; set; }
        private string? CurrentDescription { get; set; }
        public ObservableCollection<Word> VocabularyItems { get; } = new() { new Word() };
        private string AdjustableItemsString => VocabularyItems.Count == 1 ? "item" : "items";

        private void AddWord()
        {
            var newWord = new Word();
            VocabularyItems.Add(newWord);
        }
    }
}