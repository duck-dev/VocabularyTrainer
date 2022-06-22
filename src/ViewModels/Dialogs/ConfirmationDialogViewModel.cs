using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using VocabularyTrainer.ViewModels.BaseClasses;

namespace VocabularyTrainer.ViewModels.Dialogs;

public class ConfirmationDialogViewModel : DialogViewModelBase
{
    private enum ActionType
    {
        Confirm,
        Cancel
    }
        
    private readonly Action? _confirmAction;
    private readonly Action? _cancelAction;

    public ConfirmationDialogViewModel(string title,
        IEnumerable<SolidColorBrush> buttonColors,
        IEnumerable<SolidColorBrush> buttonTextColors,
        IEnumerable<string> buttonTexts,
        Action? confirmAction,
        Action? cancelAction = null) : base(title, buttonColors, buttonTextColors, buttonTexts)
    {
        _confirmAction = confirmAction;
        _cancelAction = cancelAction;
    }

    public ConfirmationDialogViewModel(string title,
        IEnumerable<Color> buttonColors,
        IEnumerable<Color> buttonTextColors,
        IEnumerable<string> buttonTexts,
        Action? confirmAction,
        Action? cancelAction = null) : this(title,
                                            buttonColors.Select(x => new SolidColorBrush(x)),
                                            buttonTextColors.Select(x => new SolidColorBrush(x)),
                                            buttonTexts,
                                            confirmAction,
                                            cancelAction)
    { }

    private void Command(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.Confirm:
                //IgnoreDialog(); // Set a variable in the settings that makes sure this dialog will be ignored in the future
                _confirmAction?.Invoke();
                break;
            case ActionType.Cancel:
                _cancelAction?.Invoke();
                break;
        }
        CloseDialog();
    }
}