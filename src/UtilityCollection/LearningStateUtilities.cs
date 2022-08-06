using VocabularyTrainer.Enums;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Models;
using VocabularyTrainer.ViewModels.LearningModes;

namespace VocabularyTrainer.UtilityCollection;

public static partial class Utilities
{
    /// <summary>
    /// Add a specific <see cref="LearningState"/> to a <see cref="Word"/> in a specific learning mode.
    /// </summary>
    /// <param name="item"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool,bool,bool)"/></param>
    /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
    /// <param name="state">The state to be added.</param>
    /// <param name="considerOverallState">Should the overall learning state (<see cref="Word.LearningStatus"/>)
    /// be retrieved as the initial status that will be changed? If false, the learning state that's saved
    /// for the current learning mode will be used.</param>
    public static void AddLearningState(VocabularyItem item, SingleWordViewModelBase singleWordViewModel, LearningState state, bool considerOverallState)
    {
        LearningModeType learningMode = singleWordViewModel.LearningMode;
        LearningState result = considerOverallState && item is Word word ? word.LearningStatus : item.LearningStateInModes[learningMode];
        result |= state;
        InvokeChangingState(item, singleWordViewModel, result, considerOverallState);
    }

    /// <summary>
    /// Remove a specific <see cref="LearningState"/> from a <see cref="Word"/> in a specific learning mode.
    /// </summary>
    /// <param name="item"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool,bool,bool)"/></param>
    /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
    /// <param name="state">The state to be removed.</param>
    /// <param name="considerOverallState">Should the overall learning state (<see cref="Word.LearningStatus"/>)
    /// be retrieved as the initial status that will be changed? If false, the learning state that's saved
    /// for the current learning mode will be used.</param>
    public static void RemoveLearningState(VocabularyItem item, SingleWordViewModelBase singleWordViewModel, LearningState state, bool considerOverallState)
    {
        LearningModeType learningMode = singleWordViewModel.LearningMode;
        LearningState result = considerOverallState && item is Word word ? word.LearningStatus : item.LearningStateInModes[learningMode];
        result &= ~state;
        InvokeChangingState(item, singleWordViewModel, result, considerOverallState);
    }

    /// <summary>
    /// Automatically change the <see cref="LearningState"/> of a <see cref="Word"/> in a specific learning mode by one step.
    /// </summary>
    /// <param name="item">The <see cref="Word"/> whose learning state should be changed.</param>
    /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
    /// <param name="known">Did the user answer correctly or not?</param>
    /// <param name="visualize">Whether the progress should be visualized in the progress bars. This is always desired, 
    /// unless this method is called from <see cref="ChangeLearningStateThesaurus"/>.</param>
    /// <param name="considerOverallState">Should the overall learning state (<see cref="Word.LearningStatus"/>)
    /// be retrieved as the initial status that will be changed? If false, the learning state that's saved
    /// for the current learning mode will be used.</param>
    public static void ChangeLearningState(VocabularyItem item, SingleWordViewModelBase singleWordViewModel, 
        bool known, bool considerOverallState, bool visualize = true)
    {
        LearningModeType learningMode = singleWordViewModel.LearningMode;
        LearningState originalState = considerOverallState && item is Word word ? word.LearningStatus : item.LearningStateInModes[learningMode];
        LearningState newState = originalState;
        
        if (originalState == LearningState.NotAsked)
        {
            ChangeLearningState(item, singleWordViewModel, known ? LearningState.KnownOnce : LearningState.WrongOnce, considerOverallState, visualize);
            return;
        }

        newState &= ~LearningState.NotAsked;
        newState = known ? newState.Next(false, LearningState.NotAsked) 
            : newState.Previous(false, LearningState.NotAsked);
        
        ApplyModifications(item, newState, learningMode, learningMode != LearningModeType.Thesaurus || considerOverallState);
        if(visualize)
            singleWordViewModel.VisualizeLearningProgress(originalState, newState);
    }

    /// <summary>
    /// Change the <see cref="LearningState"/> of a <see cref="Word"/> in a specific learning mode to a specified value.
    /// </summary>
    /// <param name="item"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool,bool,bool)"/></param>
    /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
    /// <param name="result">The value (<see cref="LearningState"/>) to be set.</param>
    /// <param name="visualize">Whether the progress should be visualized in the progress bars. This is always desired, 
    /// unless this method is called from <see cref="ChangeLearningStateThesaurus"/>.</param>
    /// <param name="considerOverallState">Should the overall learning state (<see cref="Word.LearningStatus"/>)
    /// be retrieved as the initial status that will be changed? If false, the learning state that's saved
    /// for the current learning mode will be used.</param>
    public static void ChangeLearningState(VocabularyItem item, SingleWordViewModelBase singleWordViewModel, LearningState result, 
        bool considerOverallState, bool visualize = true)
    {
        LearningModeType learningMode = singleWordViewModel.LearningMode;
        LearningState previousState = considerOverallState && item is Word word ? word.LearningStatus : item.LearningStateInModes[learningMode];
        ApplyModifications(item, result, learningMode, learningMode != LearningModeType.Thesaurus || considerOverallState);
        if(visualize)
            singleWordViewModel.VisualizeLearningProgress(previousState, result);
    }

    /// <summary>
    /// This method changes the learning state of the given <paramref name="item"/> and the learning state of all it's vocabulary references.
    /// </summary>
    /// <param name="item">The <see cref="VocabularyItem"/> whose learning state and the states of it's vocabulary references should be changed.</param>
    /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
    /// <param name="known">Did the user answer correctly or not?</param>
    public static void ChangeLearningStateThesaurus(VocabularyItem item, SingleWordViewModelBase singleWordViewModel, bool known)
    {
        if (item.VocabularyReferences is null)
            return;
        
        foreach (VocabularyItem reference in item.VocabularyReferences)
            ChangeLearningState(reference, singleWordViewModel, known, considerOverallState: false, visualize: false);
        ChangeLearningState(item, singleWordViewModel, known, considerOverallState: false);
    }
    
    /// <summary>
    /// This method is used by <see cref="AddLearningState"/> and <see cref="RemoveLearningState"/> to call
    /// <see cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,LearningState,bool,bool)"/>
    /// for the passed <paramref name="item"/> and for all items in it's <see cref="VocabularyItem.VocabularyReferences"/> collection./>
    /// </summary>
    /// <param name="item"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,LearningState,bool,bool)"/></param>
    /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
    /// <param name="result">The value (<see cref="LearningState"/>) to be set.</param>
    /// <param name="considerOverallState">Should the overall learning state (<see cref="Word.LearningStatus"/>)
    /// be retrieved as the initial status that will be changed? If false, the learning state that's saved
    /// for the current learning mode will be used.</param>
    private static void InvokeChangingState(VocabularyItem item, SingleWordViewModelBase singleWordViewModel, LearningState result,
        bool considerOverallState)
    {
        if (item.VocabularyReferences is {Count: > 0})
        {
            foreach(VocabularyItem reference in item.VocabularyReferences)
                ChangeLearningState(reference, singleWordViewModel, result, considerOverallState: false);
        }
        ChangeLearningState(item, singleWordViewModel, result, considerOverallState);
    }

    /// <summary>
    /// Applies the modifications of the learning state to the passed learning mode
    /// in the <see cref="VocabularyItem.LearningStateInModes"/> dictionary
    /// and the overall learning status (<see cref="Word.LearningStatus"/>) if desired.
    /// </summary>
    /// <param name="item">The <see cref="VocabularyItem"/> whose learning state should be modified.</param>
    /// <param name="result">The resulting <see cref="LearningState"/> to be applied.</param>
    /// <param name="mode">The learning mode for which the learning state should be changed.</param>
    /// <param name="setOverallStatus">Specifies whether the overall learning status (<see cref="Word.LearningStatus"/>)
    /// should be changed or not.</param>
    private static void ApplyModifications(VocabularyItem item, LearningState result, LearningModeType mode, bool setOverallStatus = true)
    {
        if(setOverallStatus && item is Word word)
            word.LearningStatus = result;
        item.LearningStateInModes[mode] = result;
    }
}