using System.Collections.ObjectModel;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels
{
    public class AddLessonViewModel : ViewModelBase, IVocabularyContainer<Word>
    {
        public AddLessonViewModel() 
            => VocabularyItems.CalculateIndexReactive(this, false, nameof(AdjustableItemsString));

        public ObservableCollection<Word> VocabularyItems { get; } = new() { new Word() };

        private string CurrentName { get; set; } = string.Empty;
        private string CurrentDescription { get; set; } = string.Empty;
        private string AdjustableItemsString => VocabularyItems.Count == 1 ? "item" : "items";

        private void AddWord()
        {
            var newWord = new Word();
            VocabularyItems.Add(newWord);
        }

        private void CreateLesson()
        {
            var lesson = new Lesson(CurrentName, CurrentDescription, VocabularyItems);
            DataManager.AddData(lesson);
            MainViewModel?.ReturnHome();
        }
    }
}