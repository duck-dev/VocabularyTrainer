namespace VocabularyTrainer.Interfaces
{
    public interface IDiscardableChanges
    {
        bool DataChanged { get; }
        void ConfirmDiscarding();
    }
}