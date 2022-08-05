using System;
using System.Linq;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.LearningModes;

public abstract class LearningModeViewModelBase : ViewModelBase
{
    private bool _shuffleButtonEnabled;
    private bool _shuffleWordsAutomatically;
    private bool _isSeenWordsEnabled;
    private Word[] _wordsList = Array.Empty<Word>();
        
    protected LearningModeViewModelBase(Lesson lesson)
    {
        this.CurrentLesson = lesson;
        WordsList = lesson.VocabularyItems.ToArray();
        foreach (Word word in WordsList)
            word.VocabularyReferences = null;
        ShuffleButtonEnabled = true;
    }
        
    protected internal LearningModeType LearningMode { get; private set; }

    protected internal Word[] WordsList
    {
        get => _wordsList; 
        set => this.RaiseAndSetIfChanged(ref _wordsList, value);
    }
    
    protected Lesson CurrentLesson { get; }

    protected bool ShuffleButtonEnabled
    {
        get => _shuffleButtonEnabled;
        set => this.RaiseAndSetIfChanged(ref _shuffleButtonEnabled, value);
    }
        
    protected bool IsAnswerMode { get; init; }

    protected bool IsSeenWordsEnabled
    {
        get => _isSeenWordsEnabled; 
        set => this.RaiseAndSetIfChanged(ref _isSeenWordsEnabled, value);
    }

    protected bool ShuffleWordsAutomatically
    {
        get => _shuffleWordsAutomatically;
        set
        {
            this.RaiseAndSetIfChanged(ref _shuffleWordsAutomatically, value);
            CurrentLesson.LearningModeSettings.ShuffleWordsAutomatically[LearningMode] = value;
            DataManager.SaveData();
        }
    }

    protected virtual bool ShufflingAllowed => true; 

    protected void ReturnToLesson()
    {
        if (MainViewModel is not null)
            MainViewModel.Content = new LessonViewModel(this.CurrentLesson);
    }

    protected virtual void ShuffleWords()
    {
        if (!ShufflingAllowed)
            return;

        var rnd = new Random();
        WordsList = WordsList.OrderBy(_ => rnd.Next()).ToArray();
        CurrentLesson.IsShuffledInModes[this.LearningMode] = true;
        DataManager.SaveData();
    }

    protected void SetLearningMode(LearningModeType mode) 
        => this.LearningMode = mode;

    protected void InitializeSettings()
    {
        LearningModeOptions settings = CurrentLesson.LearningModeSettings;
        if(settings.ShuffleWordsAutomatically.ContainsKey(LearningMode))
            this.ShuffleWordsAutomatically = settings.ShuffleWordsAutomatically[LearningMode];

        ApplySettings();
    }

    protected virtual void ResetInitialWordsOrder()
        => WordsList = CurrentLesson.VocabularyItems.ToArray();
    
    private void ApplySettings()
    {
        if(ShuffleWordsAutomatically && ShufflingAllowed)
            ShuffleWords();
    }
}