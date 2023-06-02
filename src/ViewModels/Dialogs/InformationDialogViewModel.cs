using System.Collections.Generic;
using Avalonia.Media;
using VocabularyTrainer.ViewModels.BaseClasses;

namespace VocabularyTrainer.ViewModels.Dialogs;

public class InformationDialogViewModel : DialogViewModelBase
{
    public InformationDialogViewModel(string title, 
        IEnumerable<SolidColorBrush> buttonColors, 
        IEnumerable<SolidColorBrush> buttonTextColors, 
        IEnumerable<string> buttonTexts) : base(title, buttonColors, buttonTextColors, buttonTexts)
    {
    }
}