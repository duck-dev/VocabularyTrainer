using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using System.Text.Json.Serialization;
using ReactiveUI;

namespace VocabularyTrainer.Models
{
    public class VocabularyItem
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