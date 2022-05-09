using System;
using System.Linq;
using ReactiveUI;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.LearningModes
{
    public sealed class FlashcardsViewModel : LearningModeViewModelBase
    {
        private Word[] _wordsList;
        private int _wordIndex;

        private Word _currentWord;
        
#pragma warning disable CS8618
        public FlashcardsViewModel(Lesson lesson) : base(lesson)
#pragma warning restore CS8618
        {
            _wordsList = lesson.VocabularyItems.ToArray();
            NextWord();
        }

        private Word CurrentWord
        {
            get => _currentWord; 
            set => this.RaiseAndSetIfChanged(ref _currentWord, value);
        }

        private void PreviousWord()
        {
            if (_wordIndex < 0 || _wordIndex >= _wordsList.Length)
                _wordIndex = _wordsList.Length - 1;

            this.CurrentWord = _wordsList[_wordIndex--];
            UtilityCollection.Utilities.Log($"{this.CurrentWord.Term}/{this.CurrentWord.Definition}");
        }

        private void NextWord()
        {
            if (_wordIndex >= _wordsList.Length)
                _wordIndex = 0;

            this.CurrentWord = _wordsList[_wordIndex++];
        }

        private void ShuffleWords()
        {
            var rnd = new Random();
            _wordsList = _wordsList.OrderBy(item => rnd.Next()).ToArray();
            
            _wordIndex = 0;
            this.CurrentWord = _wordsList[0];
        }
    }
}