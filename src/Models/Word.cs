using System.Collections.Generic;

namespace VocabularyTrainer.Models
{
    public class Word
    {
        internal string? Term { get; set; }
        internal string? Definition { get; set; }
        internal List<Word>? Synonyms { get; private set; }
        internal List<Word>? Antonyms { get; private set; }
    }
}