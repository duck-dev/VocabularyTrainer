using Avalonia.Controls;
using ReactiveUI;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Views;

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
        
        private void OpenAddPage()
        {
            var window = new AddLessonWindow
            {
                DataContext = new AddLessonViewModel()
            };
            
            if(AssignedView is Window parentWindow)
                window.ShowDialogSafe(parentWindow);
        }
    }
}