using System.Collections.ObjectModel;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels
{
    public class AddLessonViewModel : ViewModelBase, IVocabularyContainer<Word>
    {
        private string? CurrentName { get; set; }
        private string? CurrentDescription { get; set; }
        public ObservableCollection<Word> VocabularyItems { get; } = new() { new Word() };

        private void AddWord()
        {
            var newWord = new Word();
            VocabularyItems.Add(newWord);
        }
    }
}