using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels
{
    public class LessonListViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Lesson> _items;

        public LessonListViewModel(IEnumerable<Lesson> items)
        {
            _items = Items = new ObservableCollection<Lesson>(items);
        }

        private ObservableCollection<Lesson> Items
        {
            get => _items;
            init => this.RaiseAndSetIfChanged(ref _items, value);
        }

        private void OpenAddPage()
        {
            if(MainViewModel is not null)
                MainViewModel.Content = new AddLessonViewModel();
        }

        private void OpenLesson(Lesson lesson)
        {
            if(MainViewModel is not null)
                MainViewModel.Content = new LessonViewModel(lesson);
            MainWindowViewModel.CurrentLesson = lesson;
        }
    }
}