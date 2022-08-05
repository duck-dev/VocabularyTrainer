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
    /// <param name="word"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool,bool)"/></param>
    /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
    /// <param name="state">The state to be added.</param>
    public static void AddLearningState(VocabularyItem word, SingleWordViewModelBase singleWordViewModel, LearningState state)
    {
        LearningModeType learningMode = singleWordViewModel.LearningMode;
        LearningState result = word.LearningStateInModes[learningMode];
        result |= state;
        InvokeChangingState(word, singleWordViewModel, result);
    }

    /// <summary>
    /// Remove a specific <see cref="LearningState"/> from a <see cref="Word"/> in a specific learning mode.
    /// </summary>
    /// <param name="word"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool,bool)"/></param>
    /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
    /// <param name="state">The state to be removed.</param>
    public static void RemoveLearningState(VocabularyItem word, SingleWordViewModelBase singleWordViewModel, LearningState state)
    {
        LearningModeType learningMode = singleWordViewModel.LearningMode;
        LearningState result = word.LearningStateInModes[learningMode];
        result &= ~state;
        InvokeChangingState(word, singleWordViewModel, result);
    }

    /// <summary>
    /// Automatically change the <see cref="LearningState"/> of a <see cref="Word"/> in a specific learning mode by one step.
    /// </summary>
    /// <param name="word">The <see cref="Word"/> whose learning state should be changed.</param>
    /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
    /// <param name="known">Did the user answer correctly or not?</param>
    /// <param name="visualize">Whether the progress should be visualized in the progress bars. This is always desired, 
    /// unless this method is called from <see cref="ChangeLearningStateThesaurus"/>.</param>
    public static void ChangeLearningState(VocabularyItem word, SingleWordViewModelBase singleWordViewModel, bool known, bool visualize = true)
    {
        LearningModeType learningMode = singleWordViewModel.LearningMode;
        LearningState originalState = word.LearningStateInModes[learningMode];
        LearningState newState = originalState;
        
        if (originalState == LearningState.NotAsked)
        {
            ChangeLearningState(word, singleWordViewModel, known ? LearningState.KnownOnce : LearningState.WrongOnce, visualize);
            return;
        }

        newState &= ~LearningState.NotAsked;
        newState = known ? newState.Next(false, LearningState.NotAsked) 
            : newState.Previous(false, LearningState.NotAsked);
        
        word.LearningStateInModes[learningMode] = newState;
        if(visualize)
            singleWordViewModel.VisualizeLearningProgress(originalState, newState);
    }

    /// <summary>
    /// Change the <see cref="LearningState"/> of a <see cref="Word"/> in a specific learning mode to a specified value.
    /// </summary>
    /// <param name="word"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool,bool)"/></param>
    /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
    /// <param name="result">The value (<see cref="LearningState"/>) to be set.</param>
    /// <param name="visualize">Whether the progress should be visualized in the progress bars. This is always desired, 
    /// unless this method is called from <see cref="ChangeLearningStateThesaurus"/>.</param>
    public static void ChangeLearningState(VocabularyItem word, SingleWordViewModelBase singleWordViewModel, LearningState result, bool visualize = true)
    {
        LearningModeType learningMode = singleWordViewModel.LearningMode;
        LearningState previousState = word.LearningStateInModes[learningMode];
        word.LearningStateInModes[learningMode] = result;
        if(visualize)
            singleWordViewModel.VisualizeLearningProgress(previousState, result);
    }

    /// <summary>
    /// This method changes the learning state of the given <paramref name="word"/> and the learning state of all it's vocabulary references.
    /// </summary>
    /// <param name="word">The <see cref="Word"/> whose learning state and the states of it's vocabulary references should be changed.</param>
    /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
    /// <param name="known">Did the user answer correctly or not?</param>
    public static void ChangeLearningStateThesaurus(VocabularyItem word, SingleWordViewModelBase singleWordViewModel, bool known)
    {
        if (word.VocabularyReferences is null)
            return;
        
        foreach (VocabularyItem item in word.VocabularyReferences)
            ChangeLearningState(item, singleWordViewModel, known, false);
        ChangeLearningState(word, singleWordViewModel, known);
    }
    
    /// <summary>
    /// This method is used by <see cref="AddLearningState"/> and <see cref="RemoveLearningState"/> to call
    /// <see cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,LearningState,bool)"/>
    /// for the passed <paramref name="word"/> and for all items in it's <see cref="VocabularyItem.VocabularyReferences"/> collection./>
    /// </summary>
    /// <param name="word"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.VocabularyItem,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,LearningState,bool)"/></param>
    /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
    /// <param name="result">The value (<see cref="LearningState"/>) to be set.</param>
    private static void InvokeChangingState(VocabularyItem word, SingleWordViewModelBase singleWordViewModel, LearningState result)
    {
        if (word.VocabularyReferences is {Count: > 0})
        {
            foreach(VocabularyItem item in word.VocabularyReferences)
                ChangeLearningState(item, singleWordViewModel, result, false);
        }
        ChangeLearningState(word, singleWordViewModel, result);
    }
}