using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Models;
using VocabularyTrainer.ResourcesNamespace;
using VocabularyTrainer.UtilityCollection;

namespace VocabularyTrainer.ViewModels.LearningModes;

public abstract class AnswerViewModelBase : SingleWordViewModelBase
{
    private string _answer = string.Empty;
    private bool _isSolutionShown;
    private SolutionPanelViewModel? _solutionPanel;
    private SolidColorBrush _answerColor;

    private int _knownWords;
    private int _wrongWords;

    private readonly SolidColorBrush _blackColor 
        = Utilities.GetResourceFromStyle<SolidColorBrush,Application>(Application.Current, "OppositeAccent", Resources.StyleIndex) 
          ?? new(Color.Parse("#000000"));
    private readonly SolidColorBrush _greenColor
        = Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "MainGreen", Resources.StyleIndex)
          ?? new(Color.Parse("#0CA079"));
    private readonly SolidColorBrush _redColor 
        = Utilities.GetResourceFromStyle<SolidColorBrush, Application>(Application.Current, "MainRed", Resources.StyleIndex) 
          ?? new(Color.Parse("#FF0000"));

    public event EventHandler? ReadyToFocus;

    protected AnswerViewModelBase(Lesson lesson, bool initializeWords = true) : base(lesson, initializeWords)
    {
        _answerColor = this.AnswerColor = _blackColor;
        this.IsSolutionShown = false;
        this.IsAnswerMode = true;
        this.KnownWords = WordsList.Count(x => x.LearningStatus.CustomHasFlag(Utilities.KnownFlags));
        this.WrongWords = WordsList.Count(x => x.LearningStatus.CustomHasFlag(Utilities.WrongFlags));
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
        
    protected virtual bool IsSolutionShown
    {
        get => _isSolutionShown;
        set
        {
            this.RaiseAndSetIfChanged(ref _isSolutionShown, value);
            this.ShuffleButtonEnabled = ShufflingAllowed && !value;
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

    protected IEnumerable<string>? PossibleDefinitions { get; set; }

    protected internal override void VisualizeLearningProgress(LearningState previousState, LearningState newState, bool hadNotAsked)
    {
        base.VisualizeLearningProgress(previousState, newState, hadNotAsked);
        if (newState.CustomHasFlag(Utilities.KnownFlags) && !previousState.CustomHasFlag(Utilities.KnownFlags))
        {
            if (previousState.CustomHasFlag(Utilities.WrongFlags))
                this.WrongWords--;
            this.KnownWords++;
        } else if (newState.CustomHasFlag(Utilities.WrongFlags) && !previousState.CustomHasFlag(Utilities.WrongFlags))
        {
            if (previousState.CustomHasFlag(Utilities.KnownFlags))
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
        int mistakeTolerance = CurrentLesson.Options.CorrectionSteps;
        bool tolerateTransposition = CurrentLesson.Options.TolerateSwappedLetters;

        PossibleDefinitions ??= new List<string> {Definition};
        string finalDefinition = string.Join(", ", this.PossibleDefinitions);
        int minDistance = mistakeTolerance + 1;
        foreach (string definition in this.PossibleDefinitions)
        {
            string modifiedDefinition = Utilities.ModifyAnswer(definition, CurrentLesson);
            if (modifiedDefinition.Equals(modifiedAnswer))
            {
                minDistance = 0;
                finalDefinition = definition;
                break;
            }
            
            int distance = Utilities.LevenshteinDistance(modifiedDefinition, modifiedAnswer, tolerateTransposition);
            if (distance >= minDistance) 
                continue;
            minDistance = distance;
            finalDefinition = definition;
        }
        
        bool correct = minDistance <= mistakeTolerance;
        OpenSolutionPanel(this.DisplayedTerm, finalDefinition, correct);
        if(LearningMode == LearningModeType.Thesaurus)
            Utilities.ChangeLearningStateThesaurus(CurrentWord, this, correct);
        else
            Utilities.ChangeLearningState(CurrentWord, this, correct, considerOverallState: true);
    }
        
    protected void ShowSolution()
    {
        PossibleDefinitions ??= new List<string> { Definition };
        OpenSolutionPanel(this.DisplayedTerm, string.Join(", ", PossibleDefinitions), false);
        Utilities.ChangeLearningState(CurrentWord, this, false, considerOverallState: true);
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
    
    protected override void PickWordProgressive()
    {
        base.PickWordProgressive();
        ReadyToFocus?.Invoke(this, EventArgs.Empty);
    }

    internal void CountCorrect()
    {
        Utilities.ChangeLearningState(CurrentWord, this, true, considerOverallState: true);
        NextWord();
    }
}