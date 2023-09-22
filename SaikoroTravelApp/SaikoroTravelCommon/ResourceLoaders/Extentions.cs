using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace SaikoroTravelCommon.ResourceLoaders
{
    internal static class Extensions
    {
        private static T ThrowNull<T>(string name)
        {
            throw new FormatException($"jsonプロパティ '{name}' の値を認識できません。");
        }

        public static T[] ToArray<T>(this JsonElement element)
        {
            var length = element.GetArrayLength();
            var result = new T[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = element[i].GetValueAs<T>("(array-item)");
            }
            return result;
        }

        public static IEnumerable<JsonElement> AsEnumerable(this JsonElement element)
        {
            var length = element.GetArrayLength();
            for (var i = 0; i < length; i++)
            {
                yield return element[i];
            }
        }

        public static IEnumerator<JsonElement> GetEnumerator(this JsonElement element)
        {
            return element.AsEnumerable().GetEnumerator();
        }

        public static T GetPropertyAs<T>(this JsonElement element, string name)
        {
            return element.GetProperty(name).GetValueAs<T>(name);
        }

        [return: MaybeNull]
        public static T GetPropertyAsOrDefault<T>(this JsonElement element, string name) where T : class
        {
            return element.TryGetProperty(name, out JsonElement prop) ? prop.GetValueAs<T>(name) : default;
        }

        [return: MaybeNull]
        public static T? GetPropertyAsOrNull<T>(this JsonElement element, string name) where T : struct
        {
            return element.TryGetProperty(name, out JsonElement prop) ? prop.GetValueAs<T>(name) : (T?)default;
        }

        public static T GetValueAs<T>(this JsonElement element, string name)
        {
            if (typeof(T) == typeof(string))
            {
                var value = element.GetString() ?? ThrowNull<string>(name);
                return Unsafe.As<string, T>(ref value);
            }
            if (typeof(T) == typeof(int))
            {
                var value = element.GetInt32();
                return Unsafe.As<int, T>(ref value);
            }
            if (typeof(T) == typeof(bool))
            {
                var value = element.GetBoolean();
                return Unsafe.As<bool, T>(ref value);
            }
            throw new NotSupportedException($"type '{typeof(T)}' is not supported");
        }

        public static DateTime GetTime(this JsonElement element, DateTime referenceTime, int day, string name)
        {
            return GetTime(referenceTime, day, element.GetString() ?? ThrowNull<string>(name));
        }

        private static DateTime GetTime(DateTime referenceTime, int day, string timeString)
        {
            const string defaultTimeString = "00:00:00Z";
            var defaultTime = DateTime.Parse(defaultTimeString);
            TimeSpan sub = DateTime.Parse(timeString) - defaultTime;
            DateTime time = referenceTime.Add(sub).AddDays(day - 1);
            return time;
        }
    }
}
