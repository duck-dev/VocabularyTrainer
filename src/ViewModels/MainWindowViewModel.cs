using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.Models;
using VocabularyTrainer.ResourcesNamespace;
using VocabularyTrainer.UtilityCollection;
using VocabularyTrainer.ViewModels.BaseClasses;
using VocabularyTrainer.ViewModels.Dialogs;
using VocabularyTrainer.ViewModels.LearningModes;

namespace VocabularyTrainer.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _content;
    private DialogViewModelBase? _currentDialog;
    private string _currentLearningMode = string.Empty;

    public MainWindowViewModel()
    {
        Instance = this;
        DataManager.LoadData();
        _content = Content = NewLessonList;
    } 
        
    internal static MainWindowViewModel? Instance { get; private set; }
    internal static Lesson? CurrentLesson { get; set; }
    internal static LessonListViewModel NewLessonList => new(DataManager.Lessons);

    internal ViewModelBase Content
    {
        get => _content;
        set
        {
            this.RaiseAndSetIfChanged(ref _content, value);
            this.RaisePropertyChanged(nameof(IsLessonListOpen));
            this.RaisePropertyChanged(nameof(IsLearningModeOpen));
            
            if (value is LearningModeViewModelBase) 
                return;
            CurrentLearningMode = string.Empty;
        }
    }
        
    internal DialogViewModelBase? CurrentDialog
    {
        get => _currentDialog;
        set => this.RaiseAndSetIfChanged(ref _currentDialog, value);
    }
    
    internal string CurrentLearningMode
    {
        get => _currentLearningMode; 
        set => this.RaiseAndSetIfChanged(ref _currentLearningMode, value);
    }

    private bool IsLessonListOpen => Content is LessonListViewModel;
    private bool IsLearningModeOpen => Content is LearningModeViewModelBase;

    private bool CanExportData => DataManager.LessonsFileExists;

    internal void ReturnHome(bool discardChanges = true)
    {
        switch (_content)
        {
            case LessonListViewModel:
                return;
            case IDiscardableChanges discardable when discardChanges:
            {
                if (discardable.DataChanged)
                    discardable.ConfirmDiscarding();
                else
                    this.Content = NewLessonList;
                break;
            }
            default:
                this.Content = NewLessonList;
                break;
        }

        CurrentLesson = null;
    }

    private async Task ImportData()
    {
        string directory = ApplicationVariables.RecentUploadLocation;
        FileDialogFilter filter = new FileDialogFilter
        {
            Extensions = new List<string> { "json" }
        };
        string[]? result = await Utilities.InvokeOpenFileDialog("Select a file containing your lessons", false, directory, null, new List<FileDialogFilter> {filter} );
        if (result is null || result.Length != 1)
            return;

        ApplicationVariables.RecentDownloadLocation = result[0];

        string dialogTitle;
        Action confirmAction = () =>
        {
            string dataFilePath = result[0];
            FileInfo file = new FileInfo(dataFilePath);
            string tempPath = Path.Combine(Utilities.FilesParentPath, $"{file.Name}-Temp");
            FileInfo tempFile = file.CopyTo(tempPath, false);
            
            if (!DataManager.TestLoadData(tempPath))
            {
                dialogTitle = "The chosen data is invalid. Please try a valid export file for VocabularyTrainer!";
                CurrentDialog = new InformationDialogViewModel(dialogTitle,
                    new[] { Resources.AppBlueBrush },
                    new[] { Resources.SameAccentBrush },
                    new[] { "OK" });
                tempFile.Delete();
                return;
            }

            tempFile.MoveTo(DataManager.LessonsFilePath, true);
            DataManager.LoadData();
            if(Content is LessonListViewModel viewModel)
                viewModel.UpdateLessons(DataManager.Lessons);
        };
        dialogTitle = "Do you really want to overwrite the existing data? All data will be lost and replaced by the new data.";
        CurrentDialog = new ConfirmationDialogViewModel(dialogTitle,
            new [] { Color.Parse("#D64045"), Color.Parse("#808080") },
            new[] { Colors.White, Colors.White },
            new[] { "Yes, overwrite data!", "Cancel" },
            confirmAction);
    }

    private async Task ExportData()
    {
        const string title = "Save your data to a destination to a destination";
        DateTime now = DateTime.Now;
        string fileName = $"VocabularyTrainer_Data_{now.Year}-{now.Month}-{now.Day}";
        const string extension = ".json";
        string directory = ApplicationVariables.RecentUploadLocation;

        string? location = await Utilities.InvokeSaveFileDialog(title, fileName, extension, directory);
        if (location != null)
            Utilities.SaveFile(new FileInfo(DataManager.LessonsFilePath), location, true);
    }
}