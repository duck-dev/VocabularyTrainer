using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels
{
    public class LessonListViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Lesson> _items;
        private readonly MainWindowViewModel _parentViewModel;

        public LessonListViewModel(IEnumerable<Lesson> items, MainWindowViewModel parentViewModel)
        {
            _parentViewModel = parentViewModel;
            _items = Items = new ObservableCollection<Lesson>(items);
        }

        private ObservableCollection<Lesson> Items
        {
            get => _items;
            init => this.RaiseAndSetIfChanged(ref _items, value);
        }

        private void OpenAddPage()
        {
            _parentViewModel.Content = new AddLessonViewModel
            {
                MainWindowRef = _parentViewModel
            };
        }

        private void OpenLesson(Lesson lesson)
        {
            _parentViewModel.Content = new LessonViewModel(lesson)
            {
                MainViewModel = _parentViewModel
            };
        }
    }
}