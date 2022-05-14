using System;
using System.Linq;
//using Avalonia.Media.Transformation;
//using Avalonia.VisualTree;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.LearningModes
{
    public sealed class FlashcardsViewModel : LearningModeViewModelBase
    {
        private int _wordIndex;
        
        private Word _currentWord;
        private string _displayedTerm;
        private bool _flipped;

#pragma warning disable CS8618
        public FlashcardsViewModel(Lesson lesson) : base(lesson)
#pragma warning restore CS8618
        {
            _currentWord = WordsList[_wordIndex];
            
            this.DisplayedTerm = _currentWord.Term;
            this.LearningMode = LearningModeType.Flashcards;
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
            bool resetWords = _wordIndex >= WordsList.Length || _wordIndex < 0;
            if (resetWords)
                _wordIndex = WordsList.Length - 1;
            PickWord(resetWords);
        }

        private void NextWord()
        {
            _wordIndex++;
            bool resetWords = _wordIndex >= WordsList.Length || _wordIndex < 0;
            if (resetWords)
                _wordIndex = 0;
            PickWord(resetWords);
        }

        private void PickWord(bool resetKnownWords = false)
        {
            _flipped = false;
            var word = WordsList[_wordIndex];
            
            _currentWord = word;
            this.DisplayedTerm = _currentWord.Term;
            this.RaisePropertyChanged(nameof(WordIndexCorrected));

            var knownState = word.KnownInModes[this.LearningMode];
            if(knownState < LearningState.KnownOnce)
                ChangeLearningState(word, LearningState.KnownOnce);
            
            // Create display when the words should be reset and only reset upon confirming
            // Remove parameter in PickWord(), move this code below to this specific confirmation method,
            // ... set NextWord() and PreviousWord() back to normal.
            if(resetKnownWords)
                ResetKnownWords();
        }

        private void ShuffleWords()
        {
            var rnd = new Random();
            WordsList = WordsList.OrderBy(_ => rnd.Next()).ToArray();
            
            _wordIndex = 0;
            _currentWord = WordsList[0];
            this.DisplayedTerm = _currentWord.Term;
            
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