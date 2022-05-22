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
        
        private readonly SolidColorBrush _fallbackColorBlack = new(Color.Parse("#000000"));
        private readonly SolidColorBrush _fallbackColorRed = new(Color.Parse("#FF0000"));

        protected AnswerViewModelBase(Lesson lesson) : base(lesson)
        {
            _answerColor = this.AnswerColor = Utilities.GetResourceFromStyle<SolidColorBrush,Application>
                (Application.Current, "OppositeAccent", 1) ?? _fallbackColorBlack;
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
            
        }
        
        protected void ShowSolution()
        {
            this.SolutionPanel ??= new SolutionPanelViewModel
            {
                AnswerViewModel = this
            };
            this.SolutionPanel.Term = this.DisplayedTerm;
            this.SolutionPanel.Definition = this.Definition;
            
            this.IsSolutionShown = true;
            this.AnswerColor = Utilities.GetResourceFromStyle<SolidColorBrush, Application>
                (Application.Current, "MainRed", 1) ?? _fallbackColorRed;
        }
    }
}