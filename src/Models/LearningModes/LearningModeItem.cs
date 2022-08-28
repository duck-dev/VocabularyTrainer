using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.UtilityCollection;
using VocabularyTrainer.ViewModels;

namespace VocabularyTrainer.Models;

public class LearningModeItem : INotifyPropertyChangedHelper
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public LearningModeItem(string iconFileName, string name, string description, Action clickAction, LearningModeType learningMode)
    {
        this.ModeIcon = Utilities.CreateImage($"{Utilities.AssetsPath}{iconFileName}");
        this.Name = name;
        this.Description = description;
        this.ClickCommand = ReactiveCommand.Create(clickAction);
        this.LearningMode = learningMode;

        if(MainWindowViewModel.CurrentLesson is { } lesson)
            lesson.NotifyCollectionsChanged += () => NotifyPropertyChanged(nameof(Enabled));
    }
        
    internal Bitmap? ModeIcon { get; }
    internal string Name { get; }
    internal string Description { get; }
    internal ReactiveCommand<Unit,Unit> ClickCommand { get; }
    internal LearningModeType LearningMode { get; }

    internal bool Enabled
    {
        get
        {
            if (MainWindowViewModel.CurrentLesson is not { } currentLesson)
                return false;
                
            bool words = currentLesson.VocabularyItems.Count > 0;
            bool thesaurus = true;
            bool multipleChoice = true;
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (LearningMode == LearningModeType.Thesaurus)
                thesaurus = currentLesson.VocabularyItems.Any(x => x.Synonyms.Count > 0 || x.Antonyms.Count > 0);
            else if (LearningMode == LearningModeType.MultipleChoice)
                multipleChoice = currentLesson.VocabularyItems.Count >= 4;
            return words && thesaurus && multipleChoice;
        }
    }

    public void NotifyPropertyChanged([CallerMemberName] string propertyName = "") 
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}