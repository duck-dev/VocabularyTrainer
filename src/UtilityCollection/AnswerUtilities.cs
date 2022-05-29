using VocabularyTrainer.Enums;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Models;
using VocabularyTrainer.ViewModels.LearningModes;

namespace VocabularyTrainer.UtilityCollection
{
    public static partial class Utilities
    {
        
        /// <summary>
        /// Add a specific range of <see cref="LearningState"/> to a <see cref="Word"/> in a specific learning mode.
        /// </summary>
        /// <param name="word"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.Word,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool)"/></param>
        /// <param name="singleWordViewModel"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.Word,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool)"/></param>
        /// <param name="states">The range of states to be added.</param>
        public static void AddLearningState(Word word, SingleWordViewModelBase singleWordViewModel, LearningState state)
        {
            LearningModeType learningMode = singleWordViewModel.LearningMode;
            LearningState result = word.LearningStateInModes[learningMode];
            result |= state;
            
            ChangeLearningState(word, singleWordViewModel, result);
        }

        /// <summary>
        /// Remove a specific range of <see cref="LearningState"/> from a <see cref="Word"/> in a specific learning mode.
        /// </summary>
        /// <param name="word"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.Word,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool)"/></param>
        /// <param name="singleWordViewModel"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.Word,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool)"/></param>
        /// <param name="states">The range of states to be removed.</param>
        public static void RemoveLearningState(Word word, SingleWordViewModelBase singleWordViewModel, LearningState state)
        {
            LearningModeType learningMode = singleWordViewModel.LearningMode;
            LearningState result = word.LearningStateInModes[learningMode];
            result &= ~state;
            
            ChangeLearningState(word, singleWordViewModel, result);
        }
        
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
                ChangeLearningState(word, singleWordViewModel, known ? LearningState.KnownOnce : LearningState.WrongOnce);
                return;
            }

            newState &= ~LearningState.NotAsked;
            newState = known ? newState.Next(false, LearningState.NotAsked) : newState.Previous(false, LearningState.NotAsked);

            singleWordViewModel.VisualizeLearningProgress(originalState, newState);
            word.LearningStateInModes[learningMode] = newState;
        }
        
        /// <summary>
        /// Change the <see cref="LearningState"/> of a <see cref="Word"/> in a specific learning mode to a specified value.
        /// </summary>
        /// <param name="word"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.Word,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool)"/></param>
        /// <param name="singleWordViewModel"><inheritdoc cref="ChangeLearningState(VocabularyTrainer.Models.Word,VocabularyTrainer.ViewModels.LearningModes.SingleWordViewModelBase,bool)"/></param>
        /// <param name="result">The value (<see cref="LearningState"/>) to be set.</param>
        public static void ChangeLearningState(Word word, SingleWordViewModelBase singleWordViewModel, LearningState result)
        {
            LearningModeType learningMode = singleWordViewModel.LearningMode;
            LearningState previousState = word.LearningStateInModes[learningMode];
            word.LearningStateInModes[learningMode] = result;
            singleWordViewModel.VisualizeLearningProgress(previousState, result);
        }
    }
}