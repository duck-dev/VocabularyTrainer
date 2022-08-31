using System;
using System.Linq;
using Avalonia.Media.Imaging;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Models;
using VocabularyTrainer.UtilityCollection;

namespace VocabularyTrainer.ViewModels.LearningModes;

public abstract class LearningModeViewModelBase : ViewModelBase
{
    private const string NormalShuffleIconPath = $"{Utilities.AssetsPath}Shuffle-Green.png";
    private const string DisabledShuffleIconPath = $"{Utilities.AssetsPath}Shuffle-Disabled.png";
    
    private bool _shuffleButtonEnabled;
    private bool _shuffleWordsAutomatically;
    private bool _isSeenWordsEnabled;
    private Word[] _wordsList = Array.Empty<Word>();
    private Bitmap? _shuffleIcon;

    private readonly Bitmap? _normalShuffleIcon;
    private readonly Bitmap? _disabledShuffleIcon;
        
    protected LearningModeViewModelBase(Lesson lesson)
    {
        _normalShuffleIcon = Utilities.CreateImage(NormalShuffleIconPath);
        _disabledShuffleIcon = Utilities.CreateImage(DisabledShuffleIconPath);
        
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
        set
        {
            this.RaiseAndSetIfChanged(ref _shuffleButtonEnabled, value);
            ShuffleIcon = value == true ? _normalShuffleIcon : _disabledShuffleIcon;
        }
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

    protected Bitmap? ShuffleIcon
    {
        get => _shuffleIcon; 
        private set => this.RaiseAndSetIfChanged(ref _shuffleIcon, value);
    }

    protected void ReturnToLesson()
    {
        if (MainViewModel is not null)
            MainViewModel.Content = new LessonViewModel(this.CurrentLesson);
    }

    protected virtual void ShuffleWords()
    {
        if (!ShufflingAllowed)
            return;
        
        WordsList = WordsList.Shuffle().ToArray();
        CurrentLesson.IsShuffledInModes[this.LearningMode] = true;
        DataManager.SaveData();
    }

    protected void InitializeSettings()
    {
        LearningModeOptions settings = CurrentLesson.LearningModeSettings;
        if(settings.ShuffleWordsAutomatically.ContainsKey(LearningMode))
            this.ShuffleWordsAutomatically = settings.ShuffleWordsAutomatically[LearningMode];

        ApplySettings();
    }

    protected virtual void ResetInitialWordsOrder()
        => WordsList = CurrentLesson.VocabularyItems.ToArray();

    protected void SetLearningMode(LearningModeType mode, string modeName)
    {
        this.LearningMode = mode;
        if(MainWindowViewModel.Instance is { } instance)
            instance.CurrentLearningMode = modeName;
    }

    private void ApplySettings()
    {
        if(ShuffleWordsAutomatically && ShufflingAllowed)
            ShuffleWords();
    }
}