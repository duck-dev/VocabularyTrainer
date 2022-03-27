using System;
using ReactiveUI;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _content;

        public MainWindowViewModel()
        {
            var lessons = Array.Empty<Lesson>(); // TODO: Retrieve saved lessons
            _content = Content = new LessonListViewModel(lessons, this);
        } 

        internal ViewModelBase Content
        {
            get => _content; 
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        internal void ReturnHome()
        {
            this.Content = new LessonListViewModel(Array.Empty<Lesson>(), this); // TODO: Retrieve saved lessons
        }
    }
}