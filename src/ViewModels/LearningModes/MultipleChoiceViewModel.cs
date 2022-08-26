using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.LearningModes;

public sealed class MultipleChoiceViewModel : AnswerViewModelBase
{
    public MultipleChoiceViewModel(Lesson lesson) : base(lesson) { }
}