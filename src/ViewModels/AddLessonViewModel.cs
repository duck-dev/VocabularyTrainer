using System.Collections.ObjectModel;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels
{
    public class AddLessonViewModel : ViewModelBase
    {
        private string? CurrentName { get; set; }
        private string? CurrentDescription { get; set; }
        internal ObservableCollection<Word> Words { get; } = new() { new Word() };

        private void AddWord()
        {
            var newWord = new Word
            {
                ParentLesson = this
            };
            Words.Add(newWord);
        }
    }
}