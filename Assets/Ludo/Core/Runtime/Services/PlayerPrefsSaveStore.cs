using UnityEngine;

namespace Ludo.Core.Services
{
    public class PlayerPrefsSaveStore : ISaveStore
    {
        public void Save<T>(string key, T data)
        {
            if (data == null)
            {
                PlayerPrefs.DeleteKey(key);
                return;
            }

            switch (data)
            {
                case int i:
                    PlayerPrefs.SetInt(key, i);
                    break;
                case float f:
                    PlayerPrefs.SetFloat(key, f);
                    break;
                case string s:
                    PlayerPrefs.SetString(key, s);
                    break;
                default:
                    var json = JsonUtility.ToJson(data);
                    PlayerPrefs.SetString(key, json);
                    break;
            }
        }

        public T Load<T>(string key, T defaultValue = default)
        {
            if (!PlayerPrefs.HasKey(key))
                return defaultValue;

            var type = typeof(T);
            if (type == typeof(int))
                return (T)(object)PlayerPrefs.GetInt(key);
            if (type == typeof(float))
                return (T)(object)PlayerPrefs.GetFloat(key);
            if (type == typeof(string))
                return (T)(object)PlayerPrefs.GetString(key);

            var json = PlayerPrefs.GetString(key);
            try
            {
                return JsonUtility.FromJson<T>(json);
            }
            catch
            {
                return defaultValue;
            }
        }

        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public void Flush()
        {
            PlayerPrefs.Save();
        }
    }
}
