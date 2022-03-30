using System.IO;

namespace VocabularyTrainer.UtilityCollection
{
    public static partial class Utilities
    {
        /// <summary>
        /// The parent path of all settings- and data-files
        /// </summary>
        public static string FilesParentPath
        {
            get
            {
                var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                return directory ?? throw new DirectoryNotFoundException("Directory name of the currently executing assembly is null.");
            }
        }
        
        /// <summary>
        /// Log a message to the console (for debugging purposes).
        /// </summary>
        /// <param name="message">The message to be logged as a string.</param>
        public static void Log(string? message) => System.Diagnostics.Trace.WriteLine(message);
    }
}