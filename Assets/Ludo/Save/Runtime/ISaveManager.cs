using UnityEngine;

namespace Ludo.Save
{
    public interface ISaveManager
    {
        Awaitable Save<T>(string key, T data);
        Awaitable<T> Load<T>(string key);
        Awaitable<bool> Exists(string key);
        Awaitable Delete(string key);
    }
}