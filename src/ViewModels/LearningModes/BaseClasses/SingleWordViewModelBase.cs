using Avalonia;
using Avalonia.Media;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Models;
using VocabularyTrainer.UtilityCollection;

namespace VocabularyTrainer.ViewModels.LearningModes
{
    public abstract class SingleWordViewModelBase : LearningModeViewModelBase
    {
        private int _wordIndex;
        private string _displayedTerm;
        private string? _answer;
        private SolidColorBrush _answerColor;
        private bool _isAnswerReadonly;

        private readonly SolidColorBrush _fallbackColorBlack = new(Color.Parse("#000000"));
        private readonly SolidColorBrush _fallbackColorRed = new(Color.Parse("#FF0000"));

        protected SingleWordViewModelBase(Lesson lesson) : base(lesson)
        {
            CurrentWord = WordsList[_wordIndex];
            _displayedTerm = this.DisplayedTerm = CurrentWord.Term;
            _answerColor = this.AnswerColor = Utilities.GetResourceFromStyle<SolidColorBrush,Application>
                                              (Application.Current, "OppositeAccent", 1) ?? _fallbackColorBlack;
        }
        
        protected Word CurrentWord { get; private set; }
        protected string DisplayedTerm
        {
            get => _displayedTerm;
            set => this.RaiseAndSetIfChanged(ref _displayedTerm, value);
        }

        protected int WordIndexCorrected => _wordIndex + 1;

        protected string? Answer
        {
            get => _answer; 
            set => this.RaiseAndSetIfChanged(ref _answer, value);
        }

        protected SolidColorBrush AnswerColor
        {
            get => _answerColor; 
            set => this.RaiseAndSetIfChanged(ref _answerColor, value);
        }

        protected bool IsAnswerReadonly
        {
            get => _isAnswerReadonly; 
            set => this.RaiseAndSetIfChanged(ref _isAnswerReadonly, value);
        }

        protected void CheckAnswer()
        {
            
        }

        protected void ShowSolution()
        {
            this.Answer = CurrentWord.Definition;
            this.AnswerColor = Utilities.GetResourceFromStyle<SolidColorBrush, Application>
                               (Application.Current, "MainRed", 1) ?? _fallbackColorRed;
            this.IsAnswerReadonly = true;
        }
        
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