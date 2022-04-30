using VocabularyTrainer.Models;

namespace VocabularyTrainer.ViewModels
{
    public sealed class LearningModesViewModel : ViewModelBase
    {
        private LearningModeItem[] LearningModes { get; } =
        {
            new ("avalonia-logo.ico", "Flashcards", "Memorize vocabulary super fast by flipping flashcards.",
                () => UtilityCollection.Utilities.Log("Flashcards")),
            new ("avalonia-logo.ico", "Write", 
                "The best solution to learn the exact spelling of a word and it's quite similar to an exam too.",
                () => UtilityCollection.Utilities.Log("Write")),
            new ("avalonia-logo.ico", "Multiple Choice", "Choose from 4 options and pick the correct answer.",
                () => UtilityCollection.Utilities.Log("Multiple Choice")),
            new ("avalonia-logo.ico", "Synonyms and Antonyms", "Focus on learning synonyms and antonyms only.",
                () => UtilityCollection.Utilities.Log("Synonyms and Antonyms")),
            new ("avalonia-logo.ico", "Vocabulary list", 
                "Do you prefer just looking at a list of words with their term and definition? Then this is your choice.",
                () => UtilityCollection.Utilities.Log("Vocabulary list")),
        };
    }
}