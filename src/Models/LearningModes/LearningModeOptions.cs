using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using VocabularyTrainer.Enums;

namespace VocabularyTrainer.Models;

public struct LearningModeOptions
{
    private readonly LearningModeType[] _askTermModes = { LearningModeType.Flashcards, LearningModeType.Write, LearningModeType.MultipleChoice };
    private readonly LearningModeType[] _askDefinitionModes = { LearningModeType.Flashcards, LearningModeType.Write, LearningModeType.MultipleChoice };
    private readonly LearningModeType[] _showThesaurusModes = { LearningModeType.Flashcards, LearningModeType.VocabularyList };

    public LearningModeOptions()
    {
        var modes = Enum.GetValues<LearningModeType>();
        foreach (LearningModeType mode in modes)
        {
            ShuffleWordsAutomatically.Add(mode, true);
            if (_askTermModes.Contains(mode))
                AskTermInModes.Add(mode, true);
            if (_askDefinitionModes.Contains(mode))
                AskDefinitionInModes.Add(mode, false);
            if (_showThesaurusModes.Contains(mode))
                ShowThesaurusInModes.Add(mode, true);
        }
    }

    [JsonConstructor]
    public LearningModeOptions(Dictionary<LearningModeType, bool> shuffleWordsAutomatically, 
                               Dictionary<LearningModeType, bool> askTermInModes,
                               Dictionary<LearningModeType, bool> askDefinitionInModes,
                               Dictionary<LearningModeType, bool> showThesaurusInModes,
                               bool askSynonyms, bool askAntonyms,
                               bool acceptSynonyms)
    {
        this.ShuffleWordsAutomatically = shuffleWordsAutomatically;
        this.AskTermInModes = askTermInModes;
        this.AskDefinitionInModes = askDefinitionInModes;
        this.ShowThesaurusInModes = showThesaurusInModes;
        this.AskSynonyms = askSynonyms;
        this.AskAntonyms = askAntonyms;
        this.AcceptSynonyms = acceptSynonyms;
    }
    
    // All modes
    public Dictionary<LearningModeType, bool> ShuffleWordsAutomatically { get; set; } = new();
    
    // Flashcards, Write, Multiple Choice
    public Dictionary<LearningModeType, bool> AskTermInModes { get; set; } = new();
    public Dictionary<LearningModeType, bool> AskDefinitionInModes { get; set; } = new();
    
    // Flashcards, Vocabulary List
    public Dictionary<LearningModeType, bool> ShowThesaurusInModes { get; set; } = new();
    
    // Synonyms and Antonyms
    public bool AskSynonyms { get; set; } = true;
    public bool AskAntonyms { get; set; } = true;
    
    // Write
    public bool AcceptSynonyms { get; set; } = false;
}