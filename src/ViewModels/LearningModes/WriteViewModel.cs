using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.LearningModes
{
    public sealed class WriteViewModel : AnswerViewModelBase
    {
        public WriteViewModel(Lesson lesson) : base(lesson)
        {
            SetWord();
        }

        protected override void PickWord(bool resetKnownWords = false)
        {
            base.PickWord(resetKnownWords);
            SetWord();
        }

        protected override void ShuffleWords()
        {
            base.ShuffleWords();
            SetWord();
        }
    }
}