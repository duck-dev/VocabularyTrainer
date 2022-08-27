using System.Collections.Generic;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Models;
using VocabularyTrainer.UtilityCollection;

namespace VocabularyTrainer.ViewModels.LearningModes;

public sealed class MultipleChoiceViewModel : AnswerViewModelBase
{
    public MultipleChoiceViewModel(Lesson lesson) : base(lesson)
    {
        VerifyAndSetItem(SetWord);
    }
    
    protected override void Initialize(bool initializeWords)
    {
        SetLearningMode(LearningModeType.MultipleChoice, "Multiple Choice");
        base.Initialize(initializeWords);
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

    private void CheckAnswer(string answer)
    {
        // TODO: Start transition for button colors (green for correct one, red for wrong given answer if necessary)
        // TODO: Enable button for next word
        Utilities.ChangeLearningState(CurrentWord, this, answer.Equals(Definition), considerOverallState: true);
    }

    private void AddPossibleDefinitions()
    {
        var list = new List<string> {Definition};
    }
}