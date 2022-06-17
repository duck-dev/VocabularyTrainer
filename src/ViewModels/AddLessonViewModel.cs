using System.Collections.ObjectModel;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.Models;
using VocabularyTrainer.ViewModels.BaseClasses;

namespace VocabularyTrainer.ViewModels
{
    public sealed class AddLessonViewModel : LessonViewModelBase, IVocabularyContainer<Word>
    {
        public AddLessonViewModel()
        {
            VocabularyItems.CalculateIndexReactive(this, false, nameof(AdjustableItemsString));
            Initialize();
        }

        public ObservableCollection<Word> VocabularyItems { get; } = new() { new Word() };

        private string CurrentName { get; set; } = string.Empty;
        private string CurrentDescription { get; set; } = string.Empty;
        private LessonOptions CurrentOptions { get; set; }
        private string AdjustableItemsString => VocabularyItems.Count == 1 ? "item" : "items";
        
        protected override void ChangeTolerance(ErrorTolerance newTolerance)
        {
            if(newTolerance != ErrorTolerance.Custom)
                CurrentOptions = LessonOptions.MatchTolerance(newTolerance);
        }

        private void AddWord()
        {
            var newWord = new Word();
            VocabularyItems.Add(newWord);
        }

        private void CreateLesson()
        {
            var lesson = new Lesson(CurrentName, CurrentDescription, VocabularyItems, Lesson.InitShuffledDictionary(), false);
            DataManager.AddData(lesson);
            MainViewModel?.ReturnHome();
        }
    }
}