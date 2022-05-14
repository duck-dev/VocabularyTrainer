namespace VocabularyTrainer.Enums
{
    public enum LearningState
    {
        NotAsked = -2,
        
        VeryHard = -1,
        WrongOnce = 0,
        
        KnownOnce = 1,
        KnownPerfectly = 2
    }
}