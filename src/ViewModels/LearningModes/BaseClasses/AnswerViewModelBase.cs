using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.LearningModes
{
    public abstract class AnswerViewModelBase : LearningModeViewModelBase
    {
        protected AnswerViewModelBase(Lesson lesson) : base(lesson) { }
    }
}