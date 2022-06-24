using System;

namespace VocabularyTrainer.Enums;

[Flags]
public enum LearningState
{
    VeryHard = 1,
    WrongOnce = 2,
        
    NotAsked = 4,
        
    KnownOnce = 8,
    KnownPerfectly = 16
}