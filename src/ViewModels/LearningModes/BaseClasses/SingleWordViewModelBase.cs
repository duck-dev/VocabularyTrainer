using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Models;
using VocabularyTrainer.UtilityCollection;

namespace VocabularyTrainer.ViewModels.LearningModes
{
    public abstract class SingleWordViewModelBase : LearningModeViewModelBase
    {
        private int _wordIndex;
        private string? _displayedTerm;
        private int _seenWords;

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        protected SingleWordViewModelBase(Lesson lesson) : base(lesson)
            => InitCurrentWord();

        protected Word CurrentWord { get; private set; } = null!;

        protected string? DisplayedTerm
        {
            get => _displayedTerm;
            set => this.RaiseAndSetIfChanged(ref _displayedTerm, value);
        }
        protected string? Definition { get; set; }

        protected int WordIndexCorrected => _wordIndex + 1;

        protected bool IsCurrentWordDifficult => this.CurrentWord.IsDifficult;
        
        protected int SeenWords
        {
            get => _seenWords; 
            private set => this.RaiseAndSetIfChanged(ref _seenWords, value);
        }

        protected bool AskTerm { get; set; }
        protected bool AskDefinition { get; set; }

        protected void PreviousWord()
        {
            _wordIndex--;
            WrapWords(WordsList.Length - 1, false);
        }

        protected virtual void NextWord()
        {
            _wordIndex++;
            WrapWords(0, true);
        }

        protected virtual void PickWord(bool resetKnownWords = false, bool goForward = true)
        {
            var word = WordsList[_wordIndex];
            
            CurrentWord = word;
            this.RaisePropertyChanged(nameof(IsCurrentWordDifficult));
            this.RaisePropertyChanged(nameof(WordIndexCorrected));

            // Create display when the words should be reset and only reset upon confirming
            // Remove parameter in PickWord(), move this code below to this specific confirmation method,
            // ... set NextWord() and PreviousWord() back to normal.
            if ((this.SeenWords == WordsList.Length && ((goForward && _wordIndex == 0) || (!goForward && _wordIndex == WordsList.Length - 1))) 
                || resetKnownWords) // Looking at micro-performance, checking `resetKnownWords` should come first, but it looks horrible
            {
                ResetKnownWords();
                return;
            }
            
            int factor = goForward ? -1 : 1;
            int newIndex = _wordIndex + factor;
            if (newIndex < 0)
                newIndex = WordsList.Length - 1;
            else if (newIndex >= WordsList.Length)
                newIndex = 0;

            Word previousWord = WordsList[newIndex];
            LearningState knownState = previousWord.LearningStateInModes[this.LearningMode];
            if(knownState.CustomHasFlag(LearningState.NotAsked))
                Utilities.RemoveLearningState(previousWord, this, LearningState.NotAsked);
        }
        
        protected override void ShuffleWords()
        {
            base.ShuffleWords();
            PickWord(true);
        }

        protected void SetWord()
        {
            this.DisplayedTerm = CurrentWord.Term;
            this.Definition = CurrentWord.Definition;
        }

        protected void SetThesaurus()
        {
            
        }

        protected internal virtual void VisualizeLearningProgress(LearningState previousState, LearningState newState)
        {
            if(!newState.CustomHasFlag(LearningState.NotAsked) && previousState.CustomHasFlag(LearningState.NotAsked))
                this.SeenWords++;
            DataManager.SaveData();
        }

        protected virtual void ResetKnownWords()
        {
            this.SeenWords = 0;
            foreach (var word in WordsList)
                Utilities.AddLearningState(word, this, LearningState.NotAsked);
        }

        protected virtual void InitCurrentWord()
        {
            Dictionary<LearningModeType, bool> shuffledDict = CurrentLesson.IsShuffledInModes;
            if (shuffledDict[this.LearningMode] == true 
                || WordsList.All(x => !x.LearningStateInModes[this.LearningMode].CustomHasFlag(LearningState.NotAsked)))
            {
                shuffledDict[this.LearningMode] = false;
                _wordIndex = 0;
                PickWord(true);
                return;
            }
            
            for (int i = 0; i < WordsList.Length; i++)
            {
                if (!WordsList[i].LearningStateInModes[this.LearningMode].CustomHasFlag(LearningState.NotAsked)) 
                    continue;
                
                this.SeenWords = WordsList.Count(x => !x.LearningStateInModes[this.LearningMode].CustomHasFlag(LearningState.NotAsked));
                _wordIndex = i;
                break;
            }

            PickWord(_wordIndex == 0);
        }

        internal virtual void SetDifficultTerm(bool difficult, VocabularyItem? item = null)
        {
            (item ?? CurrentWord).IsDifficult = difficult; // Remove from lists specifically for difficult items if needed
            DataManager.SaveData();
        }

        private void WrapWords(int newIndex, bool goForward)
        {
            bool wrapWords = _wordIndex >= WordsList.Length || _wordIndex < 0;
            if (wrapWords)
                _wordIndex = newIndex;

            bool resetWords = wrapWords && (this.SeenWords == WordsList.Length || this.SeenWords == WordsList.Length - 1);
            PickWord(resetWords, goForward);
        }
    }
}