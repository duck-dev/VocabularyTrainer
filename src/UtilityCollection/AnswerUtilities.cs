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
            var learningMode = singleWordViewModel.LearningMode;
            var state = word.LearningStateInModes[learningMode];

            if (state == LearningState.NotAsked)
            {
                state = known ? LearningState.KnownOnce : LearningState.WrongOnce;
                singleWordViewModel.VisualizeLearningProgress(state, word.LearningStateInModes[learningMode]);
                return;
            }

            state &= ~LearningState.NotAsked;
            state = known ? state.Next(false, LearningState.NotAsked) : state.Previous(false, LearningState.NotAsked);

            singleWordViewModel.VisualizeLearningProgress(state, word.LearningStateInModes[learningMode]);
        }
        
        /// <summary>
        /// Change the <see cref="LearningState"/> of a <see cref="Word"/> in a specific learning mode to a specified value.
        /// This method is mostly used for the "Flashcards" learning mode.
        /// </summary>
        /// <param name="word"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.Word,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool)"/></param>
        /// <param name="singleWordViewModel"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.Word,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool)"/></param>
        /// <param name="state">The value (<see cref="LearningState"/>) to be set.</param>
        public static void ChangeLearningState(Word word, SingleWordViewModelBase singleWordViewModel, LearningState state)
        {
            var learningMode = singleWordViewModel.LearningMode;
            var previousState = word.LearningStateInModes[learningMode];
            if (previousState == state)
                return;
            word.LearningStateInModes[learningMode] = state;
            singleWordViewModel.VisualizeLearningProgress(previousState, state);
        }
    }
}