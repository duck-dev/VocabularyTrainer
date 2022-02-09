using System;
using ReactiveUI;

namespace VocabularyTrainer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _content;

        public MainWindowViewModel()
        {
            var lessons = Array.Empty<object>(); // TODO: Retrieve saved lessons
            _content = Content = new LessonListViewModel(lessons, this);
        } 

        internal ViewModelBase Content
        {
            get => _content; 
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }
    }
}