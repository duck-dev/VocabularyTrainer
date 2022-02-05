using ReactiveUI;

namespace VocabularyTrainer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _content;
        
        public MainWindowViewModel() => _content = Content = new LessonListViewModel();

        private ViewModelBase Content
        {
            get => _content; 
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }
    }
}