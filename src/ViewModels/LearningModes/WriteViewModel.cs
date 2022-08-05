using System.Collections.Generic;
using System.Linq;
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

        VerifyAndSetItem(SetWord);
    }

    private bool AcceptSynonyms
    {
        get => _acceptSynonyms;
        set
        {
            this.RaiseAndSetIfChanged(ref _acceptSynonyms, value);
            AddPossibleDefinitions();
            
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
    
    protected override void PickWordProgressive()
    {
        base.PickWordProgressive();
        SetWord();
    }

    protected override void SetWord()
    {
        base.SetWord();
        AddPossibleDefinitions();
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

    private void AddPossibleDefinitions()
    {
        var list = new List<string> { Definition };
        if(AcceptSynonyms)
            list.AddRange(CurrentWord.Synonyms.Select(x => x.Definition));
        PossibleDefinitions = list;
    }
}