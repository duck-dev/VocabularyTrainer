using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.LearningModes
{
    public abstract class SingleWordViewModelBase : LearningModeViewModelBase
    {
        private int _wordIndex;
        private string? _displayedTerm;

        protected SingleWordViewModelBase(Lesson lesson) : base(lesson)
        {
            CurrentWord = WordsList[_wordIndex];
        }
        
        protected Word CurrentWord { get; private set; }
        protected string? DisplayedTerm
        {
            get => _displayedTerm;
            set => this.RaiseAndSetIfChanged(ref _displayedTerm, value);
        }
        protected string? Definition { get; set; }

        protected int WordIndexCorrected => _wordIndex + 1;

        protected bool IsCurrentWordDifficult => this.CurrentWord.IsDifficult;

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
            this.RaisePropertyChanged(nameof(IsCurrentWordDifficult));
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
            PickWord(true);
        }

        protected void SetWord()
        {
            this.DisplayedTerm = CurrentWord.Term;
            this.Definition = CurrentWord.Definition;
        }

        protected void SetThesaurus()
        {
            
        }
        
        internal virtual void SetDifficultTerm(VocabularyItem? item = null) 
            => (item ?? CurrentWord).IsDifficult = true; // Remove from lists specifically for difficult items if needed

        internal virtual void RemoveDifficultTerm(VocabularyItem? item = null) 
            => (item ?? CurrentWord).IsDifficult = false; // Remove from lists specifically for difficult items if needed
    }
}