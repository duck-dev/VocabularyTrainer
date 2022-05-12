using System;
using System.Linq;
//using Avalonia.Media.Transformation;
//using Avalonia.VisualTree;
using ReactiveUI;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.LearningModes
{
    public sealed class FlashcardsViewModel : LearningModeViewModelBase
    {
        private Word[] _wordsList;
        private int _wordIndex;
        
        private Word _currentWord;
        private string _displayedTerm;
        private bool _flipped;

#pragma warning disable CS8618
        public FlashcardsViewModel(Lesson lesson) : base(lesson)
#pragma warning restore CS8618
        {
            _wordsList = lesson.VocabularyItems.ToArray();
            _currentWord = _wordsList[_wordIndex];
            this.DisplayedTerm = _currentWord.Term;
        }

        private string DisplayedTerm
        {
            get => _displayedTerm;
            set => this.RaiseAndSetIfChanged(ref _displayedTerm, value);
        }

        private int WordIndexCorrected => _wordIndex + 1;

        private void PreviousWord()
        {
            _wordIndex--;
            if (_wordIndex < 0 || _wordIndex >= _wordsList.Length)
                _wordIndex = _wordsList.Length - 1;
            PickWord();
        }

        private void NextWord()
        {
            _wordIndex++;
            if (_wordIndex >= _wordsList.Length || _wordIndex < 0)
                _wordIndex = 0;
            PickWord();
        }

        private void PickWord()
        {
            _flipped = false;
            _currentWord = _wordsList[_wordIndex];
            this.DisplayedTerm = _currentWord.Term;
            this.RaisePropertyChanged(nameof(WordIndexCorrected));
        }

        private void ShuffleWords()
        {
            var rnd = new Random();
            _wordsList = _wordsList.OrderBy(_ => rnd.Next()).ToArray();
            
            _wordIndex = 0;
            _currentWord = _wordsList[0];
            
            this.RaisePropertyChanged(nameof(WordIndexCorrected));
        }

        private void FlipCard()
        {
            // Rotation test
            // var operation = _flipped ? "rotate(0deg)" : "rotate(180deg)";
            // button.RenderTransform = TransformOperations.Parse(operation); // button = parameter of type `IVisual`
            
            _flipped ^= true; // Toggle bool condition
            this.DisplayedTerm = _flipped ? _currentWord.Definition : _currentWord.Term;
        }
    }
}