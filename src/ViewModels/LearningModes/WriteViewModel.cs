using VocabularyTrainer.Enums;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.LearningModes;

public sealed class WriteViewModel : AnswerViewModelBase
{
    public WriteViewModel(Lesson lesson) : base(lesson)
    {
        SetWord();
    }

    protected override void PickWord(bool resetKnownWords = false, bool goForward = true, bool changeLearningState = true)
    {
        base.PickWord(resetKnownWords, goForward, changeLearningState);
        SetWord();
    }

    protected override void ShuffleWords()
    {
        base.ShuffleWords();
        SetWord();
    }

    protected override void InitCurrentWord()
    {
        SetLearningMode(LearningModeType.Write);
        base.InitCurrentWord();
    }
}