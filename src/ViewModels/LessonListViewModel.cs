using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace VocabularyTrainer.ViewModels
{
    public class LessonListViewModel : ViewModelBase
    {
        private readonly ObservableCollection<object> _items; // TODO: Change to lesson-type
        private readonly MainWindowViewModel _parentViewModel;

        public LessonListViewModel(IEnumerable<object> items, MainWindowViewModel parentViewModel)
        {
            _parentViewModel = parentViewModel;
            _items = Items = new ObservableCollection<object>(items);
        }

        private ObservableCollection<object> Items // TODO: Change to lesson-type
        {
            get => _items;
            init => this.RaiseAndSetIfChanged(ref _items, value);
        }

        private void OpenAddPage()
        {
            _parentViewModel.Content = new AddLessonViewModel();
        }
    }
}