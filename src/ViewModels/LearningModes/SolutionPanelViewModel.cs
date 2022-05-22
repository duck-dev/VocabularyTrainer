using ReactiveUI;

namespace VocabularyTrainer.ViewModels.LearningModes
{
    public class SolutionPanelViewModel : ViewModelBase
    {
        private string? _term;
        private string? _definition;

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
    }
}