using System.Collections.ObjectModel;

namespace VocabularyTrainer.ViewModels
{
    public class AddLessonViewModel : ViewModelBase
    {
        private string? CurrentName { get; set; }
        private string? CurrentDescription { get; set; }
        private ObservableCollection<object>? Words { get; } = new(); // TODO: Change to word type
    }
}