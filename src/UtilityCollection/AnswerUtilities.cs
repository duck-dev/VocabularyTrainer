using VocabularyTrainer.Enums;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Models;
using VocabularyTrainer.ViewModels.LearningModes;

namespace VocabularyTrainer.UtilityCollection
{
    public static partial class Utilities
    {
        /// <summary>
        /// Automatically change the <see cref="LearningState"/> of a <see cref="Word"/> in a specific learning mode by one step.
        /// </summary>
        /// <param name="word">The <see cref="Word"/> whose learning state should be changed.</param>
        /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
        /// <param name="known">Did the user answer correctly or not?</param>
        public static void ChangeLearningState(Word word, SingleWordViewModelBase singleWordViewModel, bool known)
        {
            LearningModeType learningMode = singleWordViewModel.LearningMode;
            LearningState originalState = word.LearningStateInModes[learningMode];
            LearningState newState = originalState;

            if (originalState == LearningState.NotAsked)
            {
                newState = known ? LearningState.KnownOnce : LearningState.WrongOnce;
                singleWordViewModel.VisualizeLearningProgress(originalState, newState);
                word.LearningStateInModes[learningMode] = newState;
                return;
            }

            newState &= ~LearningState.NotAsked;
            newState = known ? newState.Next(false, LearningState.NotAsked) : newState.Previous(false, LearningState.NotAsked);

            singleWordViewModel.VisualizeLearningProgress(originalState, newState);
            word.LearningStateInModes[learningMode] = newState;
        }
        
        /// <summary>
        /// Change the <see cref="LearningState"/> of a <see cref="Word"/> in a specific learning mode to a specified value.
        /// This method is mostly used for the "Flashcards" learning mode.
        /// </summary>
        /// <param name="word"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.Word,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool)"/></param>
        /// <param name="singleWordViewModel"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.Word,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool)"/></param>
        /// <param name="newState">The value (<see cref="LearningState"/>) to be set.</param>
        public static void ChangeLearningState(Word word, SingleWordViewModelBase singleWordViewModel, LearningState newState)
        {
            LearningModeType learningMode = singleWordViewModel.LearningMode;
            LearningState previousState = word.LearningStateInModes[learningMode];
            if (previousState == newState)
                return;
            word.LearningStateInModes[learningMode] = newState;
            singleWordViewModel.VisualizeLearningProgress(previousState, newState);
        }
    }
}