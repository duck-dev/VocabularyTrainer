using ReactiveUI;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _content;

        public MainWindowViewModel()
        {
            DataManager.LoadData();
            _content = Content = NewLessonList;
        } 

        internal ViewModelBase Content
        {
            get => _content; 
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        private LessonListViewModel NewLessonList => new(DataManager.Lessons, this);

        internal void ReturnHome()
        {
            this.Content = NewLessonList;
        }
    }
}