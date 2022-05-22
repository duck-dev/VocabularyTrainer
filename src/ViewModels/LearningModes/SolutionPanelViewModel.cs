using ReactiveUI;

namespace VocabularyTrainer.ViewModels.LearningModes
{
    public class SolutionPanelViewModel : ViewModelBase
    {
        private const string ExplanationCorrect = "Correct!";
        private const string ExplanationWrong = "Your answer is incorrect. The seeked definition was...";
        
        private string? _term;
        private string? _definition;
        private string? _explanationText;

        internal string? Term
        {
            get => _term; 
            set => this.RaiseAndSetIfChanged(ref _term, value);
        }

        internal string? Definition
        {
            get => _definition; 
            set => this.RaiseAndSetIfChanged(ref _definition, value);
        }
        
        internal AnswerViewModelBase? AnswerViewModel { get; init; }

        private string? ExplanationText
        {
            get => _explanationText; 
            set => this.RaiseAndSetIfChanged(ref _explanationText, value);
        }

        internal void SetExplanationText(bool answerCorrect) 
            => this.ExplanationText = answerCorrect ? ExplanationCorrect : ExplanationWrong;
    }
}