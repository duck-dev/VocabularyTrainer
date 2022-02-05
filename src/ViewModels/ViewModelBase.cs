using Avalonia.Controls;
using ReactiveUI;

namespace VocabularyTrainer.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
        protected internal ContentControl? AssignedView { get; set; } // Will be needed later
        protected internal Window? ParentWindow { get; set; }
    }
}