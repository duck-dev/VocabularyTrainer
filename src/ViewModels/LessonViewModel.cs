using VocabularyTrainer.Extensions;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels
{
    public class LessonViewModel : ViewModelBase
    {
        public LessonViewModel(Lesson lesson)
        {
            this.CurrentLesson = lesson;
            lesson.VocabularyItems.CalculateIndexReactive(this, true, nameof(AdjustableItemsString));
        }
        
        internal MainWindowViewModel? MainViewModel { get; init; }

        private Lesson CurrentLesson { get; }
        private string AdjustableItemsString => CurrentLesson.VocabularyItems.Count == 1 ? "item" : "items";

        private void SaveChanges()
        {
            CurrentLesson.SaveChanges();
            DataManager.SaveData();
        }

        private void DiscardChanges()
        {
            CurrentLesson.DiscardChanges();
            MainViewModel?.ReturnHome();
        }

        // private void DebugUnsavedChanges()
        // {
        //     UtilityCollection.Utilities.Log("--------------------------------------------------------------------");
        //     CurrentLesson.DebugUnsavedChanges();
        // }
    }
}