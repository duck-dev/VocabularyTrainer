using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.LearningModes;

public sealed class WriteViewModel : AnswerViewModelBase
{
    private bool _acceptSynonyms;
    
    public WriteViewModel(Lesson lesson) : base(lesson)
    {
        LearningModeOptions settings = CurrentLesson.LearningModeSettings;
        this.AcceptSynonyms = settings.AcceptSynonyms;
        
        SetWord();
    }

    private bool AcceptSynonyms
    {
        get => _acceptSynonyms;
        set
        {
            this.RaiseAndSetIfChanged(ref _acceptSynonyms, value);
            LearningModeOptions options = CurrentLesson.LearningModeSettings;
            options.AcceptSynonyms = value;
            CurrentLesson.LearningModeSettings = options;
            DataManager.SaveData();
        }
    }

    protected override void PickWord(bool resetKnownWords = false, bool goForward = true, bool changeLearningState = true)
    {
        base.PickWord(resetKnownWords, goForward, changeLearningState);
        SetWord();
    }

    protected override void ShuffleWords()
    {
        base.ShuffleWords();
        SetWord();
    }

    protected override void InitCurrentWord()
    {
        SetLearningMode(LearningModeType.Write);
        if(MainWindowViewModel.Instance is { } instance)
            instance.CurrentLearningMode = "Write";
        base.InitCurrentWord();
    }
}