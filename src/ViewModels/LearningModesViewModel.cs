using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Models;
using VocabularyTrainer.UtilityCollection;
using VocabularyTrainer.ViewModels.LearningModes;

namespace VocabularyTrainer.ViewModels;

public sealed class LearningModesViewModel : ViewModelBase
{
    private readonly Lesson? _currentLesson;
    private int _knownWords;
    private int _wrongWords;
    
    public LearningModesViewModel()
    {
        _currentLesson = MainWindowViewModel.CurrentLesson;
        if (_currentLesson is null)
            return;

        var words = _currentLesson.VocabularyItems;
        words.CollectionChanged += (sender, args) => RetrieveLearningProgress(words);
        RetrieveLearningProgress(words);
    }
    
    private LearningModeItem[] LearningModes { get; } =
    {
        new ("Flashcards.png", "Flashcards", "Memorize vocabulary super fast by flipping flashcards.",
            OpenLearningMode<FlashcardsViewModel>, LearningModeType.Flashcards),
        new ("Write-Dark.png", "Write", 
            "The best solution for learning the exact spelling of a word, and it's quite similar to an exam too.",
            OpenLearningMode<WriteViewModel>, LearningModeType.Write),
        new ("MultipleChoice-Dark.png", "Multiple Choice", "Choose the correct answer from 4 options.",
            OpenLearningMode<MultipleChoiceViewModel>, LearningModeType.MultipleChoice),
        new ("SynonymsAndAntonyms-Vertical-Dark.png", "Synonyms and Antonyms", "Focus on learning synonyms and antonyms only.",
            OpenLearningMode<ThesaurusViewModel>, LearningModeType.Thesaurus),
        new ("List-Dark.png", "Vocabulary list", 
            "Do you prefer just looking at a list of words with their term and definition? Then this is your choice.",
            OpenLearningMode<VocabularyListViewModel>, LearningModeType.VocabularyList),
    };

    private int MaximumItems => _currentLesson is not { } lesson ? 0 : lesson.VocabularyItems.Count;

    private int KnownWords
    {
        get => _knownWords; 
        set => this.RaiseAndSetIfChanged(ref _knownWords, value);
    }

    private int WrongWords
    {
        get => _wrongWords; 
        set => this.RaiseAndSetIfChanged(ref _wrongWords, value);
    }

    private void RetrieveLearningProgress(ICollection<Word> words)
    {
        this.KnownWords = words.Count(x => x.LearningStatus.CustomHasFlag(Utilities.KnownFlags));
        this.WrongWords = words.Count(x => x.LearningStatus.CustomHasFlag(Utilities.WrongFlags));
        this.RaisePropertyChanged(nameof(MaximumItems));
    }

    private static void OpenLearningMode<T>() where T : LearningModeViewModelBase
    {
        if (MainWindowViewModel.CurrentLesson is null)
            throw new Exception("MainWindowViewModel.CurrentLesson is null!");
            
        var viewModelObj = Activator.CreateInstance(typeof(T), MainWindowViewModel.CurrentLesson);
        if(MainWindowViewModel.Instance is { } mainInstance && viewModelObj is T viewModel)
            mainInstance.Content = viewModel;
    }
}