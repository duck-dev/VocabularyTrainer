using System;
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
            
            var lessons = Array.Empty<object>(); // TODO: Retrieve saved lessons
            _content = Content = new LessonListViewModel(lessons, assignedWindow);
        } 

        private ViewModelBase Content
        {
            get => _content; 
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }
    }
}