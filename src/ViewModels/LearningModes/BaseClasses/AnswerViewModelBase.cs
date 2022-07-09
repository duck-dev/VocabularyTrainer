using System;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Models;
using VocabularyTrainer.UtilityCollection;

namespace VocabularyTrainer.ViewModels.LearningModes;

public abstract class AnswerViewModelBase : SingleWordViewModelBase
{
    protected const LearningState KnownFlags = LearningState.KnownOnce | LearningState.KnownPerfectly;
    protected const LearningState WrongFlags = LearningState.WrongOnce | LearningState.VeryHard;

    private string _answer = string.Empty;
    private bool _isSolutionShown;
    private SolutionPanelViewModel? _solutionPanel;
    private SolidColorBrush _answerColor;

    private int _knownWords;
    private int _wrongWords;

    private readonly SolidColorBrush _blackColor 
        = Utilities.GetResourceFromStyle<SolidColorBrush,Application>(Application.Current, "OppositeAccent", 2) 
          ?? new(Color.Parse("#000000"));
    private readonly SolidColorBrush _greenColor
        = Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "MainGreen", 2)
          ?? new(Color.Parse("#0CA079"));
    private readonly SolidColorBrush _redColor 
        = Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "MainRed", 2) 
          ?? new(Color.Parse("#FF0000"));

    public event EventHandler? ReadyToFocus;

    protected AnswerViewModelBase(Lesson lesson, bool initializeWords = true) : base(lesson, initializeWords)
    {
        _answerColor = this.AnswerColor = _blackColor;
        this.IsSolutionShown = false;
        this.IsAnswerMode = true;
        this.KnownWords = lesson.VocabularyItems.Count(x => x.LearningStateInModes[LearningMode].CustomHasFlag(KnownFlags));
        this.WrongWords = lesson.VocabularyItems.Count(x => x.LearningStateInModes[LearningMode].CustomHasFlag(WrongFlags));
    }
        
    protected internal SolidColorBrush AnswerColor
    {
        get => _answerColor; 
        set => this.RaiseAndSetIfChanged(ref _answerColor, value);
    }
        
    protected string Answer
    {
        get => _answer; 
        set => this.RaiseAndSetIfChanged(ref _answer, value);
    }
        
    protected bool IsSolutionShown
    {
        get => _isSolutionShown;
        set
        {
            this.RaiseAndSetIfChanged(ref _isSolutionShown, value);
            this.ShuffleButtonEnabled = !value;
        }
    }
        
    protected SolutionPanelViewModel? SolutionPanel
    {
        get => _solutionPanel;
        set => this.RaiseAndSetIfChanged(ref _solutionPanel, value);
    }

    protected int KnownWords
    {
        get => _knownWords;
        set => this.RaiseAndSetIfChanged(ref _knownWords, value);
    }

    protected int WrongWords
    {
        get => _wrongWords;
        set => this.RaiseAndSetIfChanged(ref _wrongWords, value);
    }

    protected internal override void VisualizeLearningProgress(LearningState previousState, LearningState newState)
    {
        base.VisualizeLearningProgress(previousState, newState);
        if (newState.CustomHasFlag(KnownFlags) && !previousState.CustomHasFlag(KnownFlags))
        {
            if (previousState.CustomHasFlag(WrongFlags))
                this.WrongWords--;
            this.KnownWords++;
        } else if (newState.CustomHasFlag(WrongFlags) && !previousState.CustomHasFlag(WrongFlags))
        {
            if (previousState.CustomHasFlag(KnownFlags))
                this.KnownWords--;
            this.WrongWords++;
        }
    }

    protected override void NextWord(bool changeLearningState = true)
    {
        base.NextWord(changeLearningState);
        this.IsSolutionShown = false;
        this.AnswerColor = _blackColor;
        this.Answer = string.Empty;
    }

    protected void CheckAnswer()
    {
        string modifiedAnswer = Utilities.ModifyAnswer(Answer, CurrentLesson);
        string modifiedDefinition = Utilities.ModifyAnswer(Definition, CurrentLesson);
        int mistakeTolerance = CurrentLesson.Options.CorrectionSteps;

        bool tolerateTransposition = CurrentLesson.Options.TolerateSwappedLetters;
        bool correct = modifiedDefinition.Equals(modifiedAnswer) 
                       || Utilities.LevenshteinDistance(modifiedDefinition, modifiedAnswer, tolerateTransposition) <= mistakeTolerance;
        OpenSolutionPanel(this.DisplayedTerm, this.Definition, correct);
        Utilities.ChangeLearningState(CurrentWord, this, correct);
    }
        
    protected void ShowSolution()
    {
        OpenSolutionPanel(this.DisplayedTerm, this.Definition, false);
        Utilities.ChangeLearningState(CurrentWord, this, false);
    }
    
    protected void OpenSolutionPanel(string? term, string? definition, bool answerCorrect)
    {
        this.SolutionPanel ??= new SolutionPanelViewModel
        {
            AnswerViewModel = this
        };
        this.SolutionPanel.Term = term;
        this.SolutionPanel.Definition = definition;
        this.SolutionPanel.SetExplanationText(answerCorrect);

        this.AnswerColor = answerCorrect ? _greenColor : _redColor;
        this.IsSolutionShown = true;
    }

    protected override void PickWord(bool resetKnownWords = false, bool goForward = true, bool changeLearningState = true)
    {
        base.PickWord(resetKnownWords, goForward, changeLearningState);
        ReadyToFocus?.Invoke(this, EventArgs.Empty);
    }

    internal void CountCorrect()
    {
        Utilities.ChangeLearningState(CurrentWord, this, true);
        NextWord();
    }
}