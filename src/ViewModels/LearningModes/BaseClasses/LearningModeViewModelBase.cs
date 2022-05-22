using System;
using System.Linq;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.LearningModes
{
    public abstract class LearningModeViewModelBase : ViewModelBase
    {
        private bool _shuffleButtonEnabled;
        
        protected LearningModeViewModelBase(Lesson lesson)
        {
            this.CurrentLesson = lesson;
            WordsList = lesson.VocabularyItems.ToArray();
            ShuffleButtonEnabled = true;
        }
        
        protected Lesson CurrentLesson { get; }
        protected Word[] WordsList { get; set; }
        protected LearningModeType LearningMode { get; init; }

        protected bool ShuffleButtonEnabled
        {
            get => _shuffleButtonEnabled;
            set => this.RaiseAndSetIfChanged(ref _shuffleButtonEnabled, value);
        }

        protected void ReturnToLesson()
        {
            if(MainViewModel is not null)
                MainViewModel.Content = new LessonViewModel(this.CurrentLesson);
        }

        protected virtual void ShuffleWords()
        {
            var rnd = new Random();
            WordsList = WordsList.OrderBy(_ => rnd.Next()).ToArray();
        }
    }
}