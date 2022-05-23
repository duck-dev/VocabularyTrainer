using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Models;
using VocabularyTrainer.UtilityCollection;

namespace VocabularyTrainer.ViewModels.LearningModes
{
    public abstract class SingleWordViewModelBase : LearningModeViewModelBase
    {
        private int _wordIndex;
        private string? _displayedTerm;
        private int _seenWords;

        protected SingleWordViewModelBase(Lesson lesson) : base(lesson) 
            => CurrentWord = WordsList[_wordIndex];

        protected Word CurrentWord { get; private set; }
        protected string? DisplayedTerm
        {
            get => _displayedTerm;
            set => this.RaiseAndSetIfChanged(ref _displayedTerm, value);
        }
        protected string? Definition { get; set; }

        protected int WordIndexCorrected => _wordIndex + 1;

        protected bool IsCurrentWordDifficult => this.CurrentWord.IsDifficult;
        
        protected int SeenWords
        {
            get => _seenWords; 
            private set => this.RaiseAndSetIfChanged(ref _seenWords, value);
        }

        protected void PreviousWord()
        {
            _wordIndex--;
            bool resetWords = _wordIndex >= WordsList.Length || _wordIndex < 0;
            if (resetWords)
                _wordIndex = WordsList.Length - 1;
            PickWord(resetWords);
        }

        protected virtual void NextWord()
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

            var knownState = word.LearningStateInModes[this.LearningMode];
            if(knownState < LearningState.KnownOnce)
                Utilities.ChangeLearningState(word, this, LearningState.KnownOnce);
            
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

        protected internal virtual void VisualizeLearningProgress(LearningState previousState, LearningState newState)
        {
            if (previousState is LearningState.WrongOnce or LearningState.VeryHard)
                return;

            switch (newState)
            {
                case >= LearningState.KnownOnce:
                    this.SeenWords++;
                    break;
                case LearningState.WrongOnce or LearningState.VeryHard:
                    this.SeenWords--;
                    break;
            }
        }

        protected virtual void ResetKnownWords()
        {
            this.SeenWords = 0;
            foreach (var word in WordsList)
                word.LearningStateInModes[this.LearningMode] = LearningState.NotAsked;
        }

        internal virtual void SetDifficultTerm(VocabularyItem? item = null)
        {
            (item ?? CurrentWord).IsDifficult = true; // Remove from lists specifically for difficult items if needed
            DataManager.SaveData();
        }

        internal virtual void RemoveDifficultTerm(VocabularyItem? item = null)
        {
            (item ?? CurrentWord).IsDifficult = false; // Remove from lists specifically for difficult items if needed
            DataManager.SaveData();
        }
    }
}