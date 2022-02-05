using Avalonia.Controls;
using ReactiveUI;

namespace VocabularyTrainer.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
        protected internal ContentControl? AssignedView { get; init; }
    }
}