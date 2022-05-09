using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.LearningModes
{
    public abstract class LearningModeViewModelBase : ViewModelBase
    {
        protected LearningModeViewModelBase(Lesson lesson)
        {
            this.CurrentLesson = lesson;
        }
        
        protected Lesson CurrentLesson { get; }
    }
}