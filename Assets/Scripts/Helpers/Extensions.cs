using System;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Helpers
{
    public static class Extensions
    {
        private static Random random = new Random();

        public static T Next<T>(this T enumValue) where T : Enum
        {
            var values = (T[])Enum.GetValues(typeof(T));
            var index = Array.IndexOf(values, enumValue);
            index = (index + 1) % values.Length;
            return values[index];
        }
        
        public static T NextSteps<T>(this T src, int steps) where T : Enum
        {
            for (var i = 0; i < steps; i++)
            {
                src = src.Next();
            }

            return src;
        }

        public static T Previous<T>(this T enumValue) where T : Enum
        {
            var values = (T[])Enum.GetValues(typeof(T));
            var index = Array.IndexOf(values, enumValue);
            index = (index - 1 + values.Length) % values.Length;
            return values[index];
        }

        public static T GetRandomEnumBetween<T>(this T start, T end) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            Array values = Enum.GetValues(typeof(T));
            int startIndex = Array.IndexOf(values, start);
            int endIndex = Array.IndexOf(values, end);

            int randomIndex = random.Next(Math.Min(startIndex, endIndex), Math.Max(startIndex, endIndex) + 1);
            return (T)values.GetValue(randomIndex);
        }
    }
}