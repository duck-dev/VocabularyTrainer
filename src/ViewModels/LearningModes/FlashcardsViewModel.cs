//using Avalonia.Media.Transformation;
//using Avalonia.VisualTree;

using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.LearningModes;

public sealed class FlashcardsViewModel : SingleWordViewModelBase
{
    private bool _flipped;
    private bool _showThesaurus;

    public FlashcardsViewModel(Lesson lesson) : base(lesson)
    {
        SetWord();
        
        LearningModeOptions settings = CurrentLesson.LearningModeSettings;
        if (settings.ShowThesaurusInModes.ContainsKey(LearningMode))
            this.ShowThesaurus = settings.ShowThesaurusInModes[LearningMode];
    }

    private bool ShowThesaurus
    {
        get => _showThesaurus;
        set
        {
            this.RaiseAndSetIfChanged(ref _showThesaurus, value);
            CurrentLesson.LearningModeSettings.ShowThesaurusInModes[LearningMode] = value;
            DataManager.SaveData();
        }
    }

    protected override void PickWord(bool resetKnownWords = false, bool goForward = true, bool changeLearningState = true)
    {
        base.PickWord(resetKnownWords, goForward, changeLearningState);
        SetWord();
        _flipped = false;
    }

    protected override void ShuffleWords()
    {
        base.ShuffleWords();
        SetWord();
    }
        
    protected override void InitCurrentWord()
    {
        SetLearningMode(LearningModeType.Flashcards);
        base.InitCurrentWord();
    }

    private void FlipCard()
    {
        // Rotation test
        // var operation = _flipped ? "rotate(0deg)" : "rotate(180deg)";
        // button.RenderTransform = TransformOperations.Parse(operation); // button = parameter of type `IVisual`
            
        _flipped ^= true; // Toggle bool condition
        this.DisplayedTerm = _flipped ? CurrentWord.Definition : CurrentWord.Term;
    }
}