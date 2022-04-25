using VocabularyTrainer.Extensions;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels
{
    public class LessonViewModel : ViewModelBase, IDiscardableChanges
    {
        public LessonViewModel(Lesson lesson)
        {
            this.CurrentLesson = lesson;
            lesson.VocabularyItems.CalculateIndexReactive(this, true, nameof(AdjustableItemsString));
        }
        
        internal MainWindowViewModel? MainViewModel { get; init; }

        private Lesson CurrentLesson { get; }
        private string AdjustableItemsString => CurrentLesson.VocabularyItems.Count == 1 ? "item" : "items";

        public void DiscardChanges()
        {
            CurrentLesson.DiscardChanges();
            MainViewModel?.ReturnHome(false);
        }
        
        private void SaveChanges()
        {
            CurrentLesson.SaveChanges();
            DataManager.SaveData();
        }

        // private void DebugUnsavedChanges()
        // {
        //     UtilityCollection.Utilities.Log("--------------------------------------------------------------------");
        //     CurrentLesson.DebugUnsavedChanges();
        // }
    }
}