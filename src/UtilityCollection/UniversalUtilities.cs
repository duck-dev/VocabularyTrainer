namespace VocabularyTrainer.UtilityCollection
{
    public static partial class Utilities
    {
        /// <summary>
        /// Log a message to the console (for debugging purposes).
        /// </summary>
        /// <param name="message">The message to be logged as a string.</param>
        public static void Log(string? message) => System.Diagnostics.Trace.WriteLine(message);
    }
}