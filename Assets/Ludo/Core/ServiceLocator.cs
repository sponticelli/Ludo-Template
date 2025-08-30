using System;
using System.Collections.Generic;

namespace Ludo.Core
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Map = new();
        public static bool Exist<T>() => Map.ContainsKey(typeof(T));
        public static void Register<T>(T instance) => Map[typeof(T)] = instance!;
        public static void Unregister<T>() => Map.Remove(typeof(T));
        public static T Get<T>() => (T)Map[typeof(T)];

        public static bool TryGet<T>(out T v)
        {
            if (Map.TryGetValue(typeof(T), out var o))
            {
                v = (T)o;
                return true;
            }

            v = default!;
            return false;
        }

        public static void Clear() => Map.Clear();
    }
}