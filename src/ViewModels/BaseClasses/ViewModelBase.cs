using ReactiveUI;

namespace VocabularyTrainer.ViewModels;

public abstract class ViewModelBase : ReactiveObject
{
    protected ViewModelBase()
    {
        MainViewModel = MainWindowViewModel.Instance;
    }
        
    protected MainWindowViewModel? MainViewModel { get;  }
}