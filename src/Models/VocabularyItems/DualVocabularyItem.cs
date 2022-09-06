using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using VocabularyTrainer.Extensions;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.UtilityCollection;

namespace VocabularyTrainer.Models;

public class DualVocabularyItem : VocabularyItem, IContentVerification<DualVocabularyItem>, ICloneable
{
    private string _term = string.Empty;
    private string _changedTerm = string.Empty;
        
    protected DualVocabularyItem(IList? containerCollection = null) 
        : base(containerCollection) { }

    public string Term
    {
        get => _term; 
        set => this.ChangedTerm = _term = value.Trim();
    }

    internal string ChangedTerm
    {
        get => _changedTerm;
        set
        {
            _changedTerm = value.Trim();
            InvokeNotifyChanged();
        }
    }

    public bool MatchesUnsavedContent(IEnumerable<DualVocabularyItem> collection, out DualVocabularyItem? identicalItem)
    {
        identicalItem = collection.FirstOrDefault(x => x.ChangedAction == NotifyCollectionChangedAction.Remove
                                                       && x.ChangedDefinition.Equals(this.ChangedDefinition)
                                                       && x.ChangedTerm.Equals(this.ChangedTerm)
                                                       && !ReferenceEquals(x, this));
        return identicalItem is not null;
    }

    public override void EqualizeChangedData()
    {
        base.EqualizeChangedData();
        this.ChangedTerm = this.Term;
    }

    public override void SaveChanges()
    {
        base.SaveChanges();
        this.Term = ChangedTerm;
    }
    
    public override object Clone() => new DualVocabularyItem(ContainerCollection)
    {
        Term = _term,
        Definition = this.Definition,
        IsDifficult = this.IsDifficult,
        LearningStateInModes = this.LearningStateInModes
    };

    protected internal override bool ContainsTerm(string search) // `search` assumed to be modified already with `Utilities.ModifyAnswer`
    {
        if (base.ContainsTerm(search))
            return true;
        string modifiedTerm = Utilities.ModifyAnswer(ChangedTerm, ModificationSettings);
        int tolerance = search.Length / SearchToleranceDivisor;
        return modifiedTerm.Contains(search) || modifiedTerm.LongestCommonSubstringLength(search) >= search.Length - tolerance;
    }
}