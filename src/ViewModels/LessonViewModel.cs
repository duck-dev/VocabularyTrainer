using VocabularyTrainer.Enums;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.Models;
using VocabularyTrainer.ViewModels.BaseClasses;

namespace VocabularyTrainer.ViewModels
{
    public class LessonViewModel : LessonViewModelBase, IDiscardableChanges
    {
        public LessonViewModel(Lesson lesson) => Initialize(lesson);

        private Lesson CurrentLesson { get; set; } = null!;
        private string AdjustableItemsString => CurrentLesson.VocabularyItems.Count == 1 ? "item" : "items";

        public void DiscardChanges()
        {
            CurrentLesson.DiscardChanges();
            MainViewModel?.ReturnHome(false);
        }
        
        protected override void ChangeTolerance(ErrorTolerance newTolerance)
        {
            if(newTolerance != ErrorTolerance.Custom)
                CurrentLesson.ChangedOptions = LessonOptions.MatchTolerance(newTolerance);
        }

        protected sealed override void Initialize(Lesson? lesson = null)
        {
            if (lesson is null)
                return;
            this.CurrentLesson = lesson;
            lesson.VocabularyItems.CalculateIndexReactive(this, true, nameof(AdjustableItemsString));
            base.Initialize(lesson); // Must be at the end
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