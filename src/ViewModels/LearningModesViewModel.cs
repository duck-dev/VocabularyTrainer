using System;
using VocabularyTrainer.Models;
using VocabularyTrainer.ViewModels.LearningModes;

namespace VocabularyTrainer.ViewModels
{
    public sealed class LearningModesViewModel : ViewModelBase
    {
        private LearningModeItem[] LearningModes { get; } =
        {
            new ("avalonia-logo.ico", "Flashcards", "Memorize vocabulary super fast by flipping flashcards.",
                OpenLearningMode<FlashcardsViewModel>),
            new ("avalonia-logo.ico", "Write", 
                "The best solution to learn the exact spelling of a word and it's quite similar to an exam too.",
                OpenLearningMode<WritingViewModel>),
            new ("avalonia-logo.ico", "Multiple Choice", "Choose from 4 options and pick the correct answer.",
                OpenLearningMode<MultipleChoiceViewModel>),
            new ("avalonia-logo.ico", "Synonyms and Antonyms", "Focus on learning synonyms and antonyms only.",
                OpenLearningMode<ThesaurusViewModel>),
            new ("avalonia-logo.ico", "Vocabulary list", 
                "Do you prefer just looking at a list of words with their term and definition? Then this is your choice.",
                OpenLearningMode<VocabularyListViewModel>),
        };

        private static void OpenLearningMode<T>() where T : LearningModeViewModelBase
        {
            if (MainWindowViewModel.CurrentLesson is null)
                throw new Exception("MainWindowViewModel.CurrentLesson is null!");
            
            var viewModelObj = Activator.CreateInstance(typeof(T), MainWindowViewModel.CurrentLesson);
            if(MainWindowViewModel.Instance is { } mainInstance && viewModelObj is T viewModel)
                mainInstance.Content = viewModel;
        }
    }
}