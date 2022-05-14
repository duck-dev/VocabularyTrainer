using System.Linq;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.LearningModes
{
    public abstract class LearningModeViewModelBase : ViewModelBase
    {
        private int _knownWords;
        
        protected LearningModeViewModelBase(Lesson lesson)
        {
            this.CurrentLesson = lesson;
            WordsList = lesson.VocabularyItems.ToArray();
        }
        
        protected Lesson CurrentLesson { get; }
        protected Word[] WordsList { get; set; }
        protected LearningModeType LearningMode { get; init; }

        protected int KnownWords
        {
            get => _knownWords; 
            private set => this.RaiseAndSetIfChanged(ref _knownWords, value);
        }

        protected void ReturnToLesson()
        {
            if(MainViewModel is not null)
                MainViewModel.Content = new LessonViewModel(this.CurrentLesson);
        }

        protected void ChangeLearningState(Word word, bool known)
        {
            var state = word.KnownInModes[this.LearningMode];
            word.KnownInModes[this.LearningMode] = known switch
            {
                true when state < LearningState.KnownPerfectly => ++state,
                false when state > LearningState.VeryHard => --state,
                _ => word.KnownInModes[this.LearningMode]
            };
            VisualizeLearningProgress(state, word.KnownInModes[this.LearningMode]);
        }

        protected void ChangeLearningState(Word word, LearningState state)
        {
            var previousState = word.KnownInModes[this.LearningMode];
            word.KnownInModes[this.LearningMode] = state;
            VisualizeLearningProgress(previousState, state);
        }

        protected virtual void VisualizeLearningProgress(LearningState previousState, LearningState newState)
        {
            if (previousState is LearningState.WrongOnce or LearningState.VeryHard)
                return;

            switch (newState)
            {
                case >= LearningState.KnownOnce:
                    this.KnownWords++;
                    break;
                case LearningState.WrongOnce or LearningState.VeryHard:
                    this.KnownWords--;
                    break;
            }
        }

        protected virtual void ResetKnownWords()
        {
            this.KnownWords = 0;
            foreach (var word in WordsList)
                word.KnownInModes[this.LearningMode] = LearningState.NotAsked;
        }
    }
}