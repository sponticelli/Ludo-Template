using System;
using System.IO;
using UnityEngine;

namespace Ludo.Save
{
    public class FileSystemSaveManager : ASaveManager
    {
        [SerializeField] private string saveDirectory = "SaveData";
        
        private string SavePath => Path.Combine(Application.persistentDataPath, saveDirectory);

        protected override void HandleInitialization()
        {
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
        }

        protected override void HandleUninitialization()
        {
        }

        public override async Awaitable Save<T>(string key, T data)
        {
            await Awaitable.BackgroundThreadAsync();
            
            try
            {
                var json = JsonUtility.ToJson(data, true);
                var filePath = GetFilePath(key);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                await Awaitable.MainThreadAsync();
                Debug.LogError($"Failed to save data with key '{key}': {ex.Message}");
                throw;
            }
            
            await Awaitable.MainThreadAsync();
        }

        public override async Awaitable<T> Load<T>(string key)
        {
            await Awaitable.BackgroundThreadAsync();
            
            try
            {
                var filePath = GetFilePath(key);
                if (!File.Exists(filePath))
                {
                    await Awaitable.MainThreadAsync();
                    return default(T);
                }

                var json = await File.ReadAllTextAsync(filePath);
                await Awaitable.MainThreadAsync();
                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception ex)
            {
                await Awaitable.MainThreadAsync();
                Debug.LogError($"Failed to load data with key '{key}': {ex.Message}");
                throw;
            }
        }

        public override async Awaitable<bool> Exists(string key)
        {
            await Awaitable.BackgroundThreadAsync();
            var exists = File.Exists(GetFilePath(key));
            await Awaitable.MainThreadAsync();
            return exists;
        }

        public override async Awaitable Delete(string key)
        {
            await Awaitable.BackgroundThreadAsync();
            
            try
            {
                var filePath = GetFilePath(key);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                await Awaitable.MainThreadAsync();
                Debug.LogError($"Failed to delete data with key '{key}': {ex.Message}");
                throw;
            }
            
            await Awaitable.MainThreadAsync();
        }

        private string GetFilePath(string key)
        {
            var fileName = $"{key}.json";
            return Path.Combine(SavePath, fileName);
        }
    }
}