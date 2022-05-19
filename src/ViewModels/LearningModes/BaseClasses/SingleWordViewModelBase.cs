using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.LearningModes
{
    public abstract class SingleWordViewModelBase : AnswerViewModelBase
    {
        private int _wordIndex;
        private string _displayedTerm;

        protected SingleWordViewModelBase(Lesson lesson) : base(lesson)
        {
            CurrentWord = WordsList[_wordIndex];
            _displayedTerm = this.DisplayedTerm = CurrentWord.Term;
        }
        
        protected Word CurrentWord { get; private set; }
        protected string DisplayedTerm
        {
            get => _displayedTerm;
            set => this.RaiseAndSetIfChanged(ref _displayedTerm, value);
        }

        protected int WordIndexCorrected => _wordIndex + 1;
        
        protected void PreviousWord()
        {
            _wordIndex--;
            bool resetWords = _wordIndex >= WordsList.Length || _wordIndex < 0;
            if (resetWords)
                _wordIndex = WordsList.Length - 1;
            PickWord(resetWords);
        }

        protected void NextWord()
        {
            _wordIndex++;
            bool resetWords = _wordIndex >= WordsList.Length || _wordIndex < 0;
            if (resetWords)
                _wordIndex = 0;
            PickWord(resetWords);
        }

        protected virtual void PickWord(bool resetKnownWords = false)
        {
            var word = WordsList[_wordIndex];
            
            CurrentWord = word;
            this.DisplayedTerm = CurrentWord.Term;
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
        
        protected override void ShuffleWords()
        {
            base.ShuffleWords();

            _wordIndex = 0;
            CurrentWord = WordsList[0];
            this.DisplayedTerm = CurrentWord.Term;
            this.RaisePropertyChanged(nameof(WordIndexCorrected));
        }
    }
}