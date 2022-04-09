using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels
{
    public class LessonViewModel : ViewModelBase
    {
        public LessonViewModel(Lesson lesson)
            => this.CurrentLesson = lesson;
        
        private Lesson CurrentLesson { get; }
        private string AdjustableItemsString => CurrentLesson.VocabularyItems.Count == 1 ? "item" : "items";

        private void SaveChanges() => DataManager.SaveData();
    }
}