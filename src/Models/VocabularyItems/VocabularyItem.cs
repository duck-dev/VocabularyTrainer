using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Text.Json.Serialization;
using ReactiveUI;
using VocabularyTrainer.Enums;
using VocabularyTrainer.Interfaces;
#pragma warning disable CS0659

namespace VocabularyTrainer.Models;

public class VocabularyItem : IContentVerification<VocabularyItem>, IEquatable<VocabularyItem>, ICloneable
{
    private string _definition = string.Empty;
    private string _changedDefinition = string.Empty;

    public delegate void NotificationEventHandler();
    public event NotificationEventHandler? NotifyChanged;

    public VocabularyItem(IList? containerCollection = null)
    {
        this.ContainerCollection = containerCollection;
        this.RemoveCommandCollection = ReactiveCommand.Create<ICollection<VocabularyItem>>(Remove);
        this.LearningStateInModes.Add(LearningModeType.Thesaurus, LearningState.NotAsked);
        
        if (VocabularyReferences is null || VocabularyReferences.Count <= 0)
            return;
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (VocabularyItem item in VocabularyReferences)
        {
            if(!item.LearningStateInModes.ContainsKey(LearningModeType.Thesaurus))
                item.LearningStateInModes.Add(LearningModeType.Thesaurus, LearningState.NotAsked);
        }
    }

    [JsonConstructor]
    public VocabularyItem(string definition, bool isDifficult, Dictionary<LearningModeType, LearningState> learningStateInModes) 
        : this()
    {
        this.Definition = definition;
        this.IsDifficult = isDifficult;
        this.LearningStateInModes = learningStateInModes;
    }

    public string Definition
    {
        get => _definition; 
        set => this.ChangedDefinition = _definition = value.Trim();
    }

    public bool IsDifficult { get; set; }

    // `init`-accessor must be `public` for JSON deserialization
    public Dictionary<LearningModeType, LearningState> LearningStateInModes { get; init; } = new();
    
    protected internal string ChangedDefinition
    {
        get => _changedDefinition;
        set
        {
            _changedDefinition = value.Trim();
            InvokeNotifyChanged();
        }
    }
    
    protected internal NotifyCollectionChangedAction ChangedAction { get; set; } = NotifyCollectionChangedAction.Reset;
    
    protected ReactiveCommand<ICollection<VocabularyItem>, Unit> RemoveCommandCollection { get; }

    internal IList? ContainerCollection { get; set; }

    internal List<VocabularyItem>? VocabularyReferences { get; set; }

    public bool MatchesUnsavedContent(IEnumerable<VocabularyItem> collection, out VocabularyItem? identicalItem)
    {
        identicalItem = collection.FirstOrDefault(x => x.ChangedAction == NotifyCollectionChangedAction.Remove && 
                                                       x.ChangedDefinition.Equals(this.ChangedDefinition));
        return identicalItem is not null && !ReferenceEquals(identicalItem, this);
    }

    public virtual void EqualizeChangedData() => this.ChangedDefinition = this.Definition;

    public bool Equals(VocabularyItem? other)
        => other is not null && other.ChangedDefinition.Equals(this.ChangedDefinition);

    public override bool Equals(object? obj) => Equals(obj as VocabularyItem);
    public virtual object Clone() => new VocabularyItem(_definition, IsDifficult, LearningStateInModes)
    {
        ContainerCollection = this.ContainerCollection
    };

    protected void InvokeNotifyChanged()
        => NotifyChanged?.Invoke();

    public virtual void SaveChanges()
        => this.Definition = ChangedDefinition;

    protected virtual void Remove()
        => ContainerCollection?.Remove(this);

    protected void Remove(ICollection<VocabularyItem> collection)
        => collection.Remove(this);
}