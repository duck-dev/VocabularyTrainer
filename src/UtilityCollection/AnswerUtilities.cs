using VocabularyTrainer.Enums;
using VocabularyTrainer.Models;
using VocabularyTrainer.ViewModels.LearningModes;

namespace VocabularyTrainer.UtilityCollection
{
    public static partial class Utilities
    {
        /// <summary>
        /// Automatically change the learning state of a <see cref="Word"/> in a specific learning mode by one step.
        /// </summary>
        /// <param name="word">The <see cref="Word"/> whose learning state should be changed.</param>
        /// <param name="singleWordViewModel">The currently active <see cref="SingleWordViewModelBase"/> (learning mode).</param>
        /// <param name="known">Did the user answer correctly or not?</param>
        public static void ChangeLearningState(Word word, SingleWordViewModelBase singleWordViewModel, bool known)
        {
            var learningMode = singleWordViewModel.LearningMode;
            var state = word.LearningStateInModes[learningMode];
            word.LearningStateInModes[learningMode] = known switch
            {
                true when state < LearningState.KnownPerfectly && state != LearningState.WrongOnce => state + 1,
                true when state == LearningState.WrongOnce => LearningState.KnownOnce,
                false when state > LearningState.VeryHard && state != LearningState.KnownOnce => state - 1,
                false when state == LearningState.KnownOnce => LearningState.WrongOnce,
                _ => word.LearningStateInModes[learningMode]
            };
            singleWordViewModel.VisualizeLearningProgress(state, word.LearningStateInModes[learningMode]);
        }
        
        /// <summary>
        /// Change the learning state of a <see cref="Word"/> in a specific learning mode to a specified value.
        /// This method is mostly used for the "Flashcards" learning mode.
        /// </summary>
        /// <param name="word"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.Word,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool)"/></param>
        /// <param name="singleWordViewModel"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.Word,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool)"/></param>
        /// <param name="state">The value (state) to be set.</param>
        public static void ChangeLearningState(Word word, SingleWordViewModelBase singleWordViewModel, LearningState state)
        {
            var learningMode = singleWordViewModel.LearningMode;
            var previousState = word.LearningStateInModes[learningMode];
            word.LearningStateInModes[learningMode] = state;
            singleWordViewModel.VisualizeLearningProgress(previousState, state);
        }
    }
}