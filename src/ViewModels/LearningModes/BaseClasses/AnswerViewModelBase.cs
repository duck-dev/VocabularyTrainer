using System;
using Avalonia;
using Avalonia.Media;
using ReactiveUI;
using VocabularyTrainer.Models;
using VocabularyTrainer.UtilityCollection;

namespace VocabularyTrainer.ViewModels.LearningModes
{
    public abstract class AnswerViewModelBase : SingleWordViewModelBase
    {
        private string? _answer;
        private bool _isSolutionShown;
        private SolutionPanelViewModel? _solutionPanel;
        private SolidColorBrush _answerColor;
        
        private readonly SolidColorBrush _blackColor 
            = Utilities.GetResourceFromStyle<SolidColorBrush,Application>(Application.Current, "OppositeAccent", 1) 
              ?? new(Color.Parse("#000000"));
        private readonly SolidColorBrush _greenColor
            = Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "MainGreen", 1)
              ?? new(Color.Parse("#0CA079"));
        private readonly SolidColorBrush _redColor 
            = Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "MainRed", 1) 
              ?? new(Color.Parse("#FF0000"));

        public event EventHandler? ReadyToFocus;

        protected AnswerViewModelBase(Lesson lesson) : base(lesson)
        {
            _answerColor = this.AnswerColor = _blackColor;
            this.IsSolutionShown = false;
        }
        
        protected string? Answer
        {
            get => _answer; 
            set => this.RaiseAndSetIfChanged(ref _answer, value);
        }
        
        protected bool IsSolutionShown
        {
            get => _isSolutionShown;
            set
            {
                this.RaiseAndSetIfChanged(ref _isSolutionShown, value);
                this.ShuffleButtonEnabled = !value;
            }
        }
        
        protected SolutionPanelViewModel? SolutionPanel
        {
            get => _solutionPanel;
            set => this.RaiseAndSetIfChanged(ref _solutionPanel, value);
        }
        
        protected internal SolidColorBrush AnswerColor
        {
            get => _answerColor; 
            set => this.RaiseAndSetIfChanged(ref _answerColor, value);
        }
        
        protected void CheckAnswer()
        {
            OpenSolutionPanel(this.DisplayedTerm, this.Answer, true); // TODO: Check answer
        }
        
        protected void ShowSolution()
        {
            OpenSolutionPanel(this.DisplayedTerm, this.Definition, false);
        }

        protected override void PickWord(bool resetKnownWords = false)
        {
            base.PickWord(resetKnownWords);
            ReadyToFocus?.Invoke(this, EventArgs.Empty);
        }

        private void OpenSolutionPanel(string? term, string? definition, bool answerCorrect)
        {
            this.SolutionPanel ??= new SolutionPanelViewModel
            {
                AnswerViewModel = this
            };
            this.SolutionPanel.Term = term;
            this.SolutionPanel.Definition = definition;
            this.SolutionPanel.SetExplanationText(answerCorrect);

            this.AnswerColor = answerCorrect ? _greenColor : _redColor;
            this.IsSolutionShown = true;
        }
    }
}