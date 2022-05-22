//using Avalonia.Media.Transformation;
//using Avalonia.VisualTree;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.LearningModes
{
    public sealed class FlashcardsViewModel : SingleWordViewModelBase
    {
        private bool _flipped;

        public FlashcardsViewModel(Lesson lesson) : base(lesson)
        {
            SetWord();
            this.LearningMode = LearningModeType.Flashcards;
        }

        protected override void PickWord(bool resetKnownWords = false)
        {
            base.PickWord(resetKnownWords);
            SetWord();
            _flipped = false;
        }

        protected override void ShuffleWords()
        {
            base.ShuffleWords();
            SetWord();
        }

        private void FlipCard()
        {
            // Rotation test
            // var operation = _flipped ? "rotate(0deg)" : "rotate(180deg)";
            // button.RenderTransform = TransformOperations.Parse(operation); // button = parameter of type `IVisual`
            
            _flipped ^= true; // Toggle bool condition
            this.DisplayedTerm = _flipped ? CurrentWord.Definition : CurrentWord.Term;
        }
    }
}