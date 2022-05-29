using System;
using System.Collections.Generic;
using System.Linq;

namespace VocabularyTrainer.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// Faster version of <see cref="Enum.HasFlag"/>
        /// </summary>
        /// <param name="enumeration">The extended enum value.</param>
        /// <param name="value">The flag value.</param>
        /// <returns>Has flag?</returns>
        public static bool CustomHasFlag<T>(this T enumeration, T value)
            where T : struct, Enum, IComparable, IConvertible, IFormattable
        {
            var newEnumeration = enumeration.GenericEnumToInt();
            var newValue = value.GenericEnumToInt();

            return (newEnumeration & newValue) != 0;
        }

        /// <summary>
        /// Get the next enum value.
        /// </summary>
        /// <param name="enumeration">The initial enum value.</param>
        /// <param name="wrap">Should the selection wrap to the first item if it exceeds the upper limit? Default: false
        /// If it doesn't wrap, it will simply not change (last item).</param>
        /// <param name="ignoredItems">Items that will be skipped in this process.</param>
        /// <typeparam name="T">The generic enum type.</typeparam>
        /// <returns>The next enum value as an <see cref="int"/></returns>
        public static T Next<T>(this T enumeration, bool wrap = false, params T[] ignoredItems) 
            where T : struct, Enum, IComparable, IConvertible, IFormattable
        {
            List<T> allValues = Enum.GetValues<T>().Where(x => !ignoredItems.Contains(x)).ToList();
            int index = allValues.IndexOf(enumeration) + 1;
            
            if (allValues.Count <= 0)
                return enumeration;
            
            if (index < allValues.Count && index >= 0)
                return allValues[index];
            
            T nextItem = wrap ? allValues[0] : allValues.Last();
            return nextItem;

        }
        
        /// <summary>
        /// Get the previous enum value.
        /// </summary>
        /// <param name="enumeration"><inheritdoc cref="Next{T}"/></param>
        /// <param name="wrap">Should the selection wrap to the last item if it exceeds the lower limit? Default: false
        /// If it doesn't wrap, it will simply not change (index = 0).</param>
        /// <param name="ignoredItems">Items that will be skipped in this process.</param>
        /// <typeparam name="T"><inheritdoc cref="Next{T}"/></typeparam>
        /// <returns>The previous enum value as an <see cref="int"/></returns>
        public static T Previous<T>(this T enumeration, bool wrap = false, params T[] ignoredItems) 
            where T : struct, Enum, IComparable, IConvertible, IFormattable
        {
            List<T> allValues = Enum.GetValues<T>().Where(x => !ignoredItems.Contains(x)).ToList();
            int index = allValues.IndexOf(enumeration) - 1;

            if (allValues.Count <= 0)
                return enumeration;
            
            if (index >= 0 && index < allValues.Count) 
                return allValues[index];
            
            T nextItem = wrap ? allValues.Last() : allValues[0];
            return nextItem;

        }

        /// <summary>
        /// Convert an enum value of a generic enum type to an <see cref="int"/>.
        /// </summary>
        /// <param name="enumeration">The enum value to be converted.</param>
        /// <typeparam name="T">The generic enum type.</typeparam>
        /// <returns>The converted <see cref="int"/>.</returns>
        private static int GenericEnumToInt<T>(this T enumeration) where T : struct, Enum, IComparable, IConvertible, IFormattable
        {
            var parsed = Enum.Parse(typeof(T), enumeration.ToString()) as Enum;
            var value = Convert.ToInt32(parsed);

            return value;
        }
    }
}