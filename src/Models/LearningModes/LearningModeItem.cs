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
using VocabularyTrainer.ViewModels;

namespace VocabularyTrainer.Models;

public class LearningModeItem : INotifyPropertyChangedHelper
{
    private const string AssetsPath = "avares://VocabularyTrainer/Assets/";
    
    public event PropertyChangedEventHandler? PropertyChanged;

    public LearningModeItem(string iconFileName, string name, string description, Action clickAction, LearningModeType learningMode)
    {
        this.ModeIcon = CreateImage(iconFileName);
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
            if (LearningMode == LearningModeType.Thesaurus)
                thesaurus = currentLesson.VocabularyItems.Any(x => x.Synonyms.Count > 0 || x.Antonyms.Count > 0);
            return words && thesaurus;
        }
    }

    private static Bitmap? CreateImage(string fileName)
    {
        string path = $"{AssetsPath}{fileName}";
        var uri = new Uri(path);
        
        Stream? asset = null;
        try
        {
            IAssetLoader? assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            asset = assets?.Open(uri);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return asset is null ? null : new Bitmap(asset);
    }
        
    public void NotifyPropertyChanged([CallerMemberName] string propertyName = "") 
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}