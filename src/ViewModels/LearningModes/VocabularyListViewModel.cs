using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels.LearningModes;

public sealed class VocabularyListViewModel : LearningModeViewModelBase
{
    private bool _showThesaurus;
    
    public VocabularyListViewModel(Lesson lesson) : base(lesson)
    {
        SetLearningMode(LearningModeType.VocabularyList);
        LearningModeOptions settings = CurrentLesson.LearningModeSettings;
        if (settings.ShowThesaurusInModes.ContainsKey(LearningMode))
            this.ShowThesaurus = settings.ShowThesaurusInModes[LearningMode];
    }
    
    internal bool ShowThesaurus
    {
        get => _showThesaurus;
        set
        {
            this.RaiseAndSetIfChanged(ref _showThesaurus, value);
            CurrentLesson.LearningModeSettings.ShowThesaurusInModes[LearningMode] = value;
            DataManager.SaveData();
        }
    }
}