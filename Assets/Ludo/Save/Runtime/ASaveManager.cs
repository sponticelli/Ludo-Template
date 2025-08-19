using Ludo.Core;
using UnityEngine;

namespace Ludo.Save
{
    public abstract class ASaveManager : AModule, ISaveManager
    {
        public abstract Awaitable Save<T>(string key, T data);
        public abstract Awaitable<T> Load<T>(string key);
        public abstract Awaitable<bool> Exists(string key);
        public abstract Awaitable Delete(string key);
        
    }
}