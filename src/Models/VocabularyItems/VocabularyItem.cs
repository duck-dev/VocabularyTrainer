using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text.Json.Serialization;
using ReactiveUI;
using VocabularyTrainer.Interfaces;
#pragma warning disable CS0659

namespace VocabularyTrainer.Models
{
    public class VocabularyItem : IContentVerification<VocabularyItem>, IEquatable<VocabularyItem>
    {
        private string _definition = string.Empty;
        private string _changedDefinition = string.Empty;

        public delegate void NotificationEventHandler();
        public event NotificationEventHandler? NotifyChanged;

        public VocabularyItem(IList? containerCollection = null)
        {
            this.ContainerCollection = containerCollection;
            this.RemoveCommandCollection = ReactiveCommand.Create<ICollection<VocabularyItem>>(Remove);
        }

        [JsonConstructor]
        public VocabularyItem(string definition) : this()
            => this.Definition = definition;

        public string Definition
        {
            get => _definition; 
            set => this.ChangedDefinition = _definition = value.Trim();
        }

        internal string ChangedDefinition
        {
            get => _changedDefinition;
            set
            {
                _changedDefinition = value.Trim();
                InvokeNotifyChanged();
            }
        }
        
        internal IList? ContainerCollection { get; set; }
        
        protected ReactiveCommand<ICollection<VocabularyItem>, Unit> RemoveCommandCollection { get; }

        public bool MatchesUnsavedContent(IEnumerable<VocabularyItem> collection, out VocabularyItem? identicalItem)
        {
            identicalItem = collection.FirstOrDefault(x => x.ChangedDefinition.Equals(this.ChangedDefinition));
            return identicalItem is not null && !ReferenceEquals(identicalItem, this);
        }

        public bool Equals(VocabularyItem? other)
            => other is not null && other.ChangedDefinition.Equals(this.ChangedDefinition);

        public override bool Equals(object? obj) => Equals(obj as VocabularyItem);

        protected void InvokeNotifyChanged()
            => NotifyChanged?.Invoke();

        protected internal virtual void SaveChanges()
            => this.Definition = ChangedDefinition;

        protected virtual void Remove()
            => ContainerCollection?.Remove(this);

        protected void Remove(ICollection<VocabularyItem> collection)
            => collection.Remove(this);
    }
}