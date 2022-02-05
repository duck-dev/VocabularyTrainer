using Avalonia.Controls;
using ReactiveUI;

namespace VocabularyTrainer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _content;

        public MainWindowViewModel(Window assignedWindow)
        {
            AssignedView = assignedWindow;
            ParentWindow = assignedWindow;
            _content = Content = new LessonListViewModel(assignedWindow);
        } 

        private ViewModelBase Content
        {
            get => _content; 
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }
    }
}