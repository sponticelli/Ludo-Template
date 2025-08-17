using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Ludo.Core.Events;
using UnityEngine;

namespace Ludo.Localization
{
    
    public class LocalizationChangedEvent : AEvent {}
    
    
    public static class LocalizationManager
    {
        public static LocalizationChangedEvent LocalizationChanged = new LocalizationChangedEvent();

        private static readonly Dictionary<string, Dictionary<string, string>> Dictionary =
            new Dictionary<string, Dictionary<string, string>>();

        private static string _currentLanguage = "en-US";

        public const string ResourcesFolder = "Localization";

        public const string LocFolder = "Assets/Resources/" + ResourcesFolder;

        private static readonly Regex MatchRegex = new Regex("\".+?\"");

        public static string Language
        {
            get => _currentLanguage;
            set
            {
                _currentLanguage = value;
                LocalizationChanged.Invoke();
            }
        }

        public static void AutoLanguage()
        {
            Language = "en-US";
        }

        public static void Read(string path = LocFolder)
        {
            if (!Application.isPlaying)
            {
                Dictionary.Clear();
            }

            if (Dictionary.Count > 0)
            {
                return;
            }

            TextAsset[] array = Resources.LoadAll<TextAsset>(ResourcesFolder);
            for (int i = 0; i < array.Length; i++)
            {
                string text = ReplaceMarkers(array[i].text);
                foreach (Match item in MatchRegex.Matches(text))
                {
                    text = text.Replace(item.Value, item.Value.Replace("\"", null).Replace(",", "[comma]"));
                }

                string[] array2 = text.Split(new string[1] { Environment.NewLine },
                    StringSplitOptions.RemoveEmptyEntries);
                string[] array3 = array2[0].Trim().Split(',').ToArray();
                for (int k = 1; k < array3.Length; k++)
                {
                    if (!Dictionary.ContainsKey(array3[k]))
                    {
                        Dictionary.Add(array3[k], new Dictionary<string, string>());
                    }
                }

                for (int l = 1; l < array2.Length; l++)
                {
                    string[] array4 = (from j in array2[l].Split(',')
                        select j.Replace("[comma]", ",")).ToArray();
                    string text2 = array4[0];
                    if (string.IsNullOrEmpty(text2))
                    {
                        continue;
                    }

                    for (int m = 1; m < array3.Length; m++)
                    {
                        if (m < array4.Length)
                        {
                            if (!Dictionary[array3[m]].ContainsKey(text2))
                            {
                                Dictionary[array3[m]].Add(text2, array4[m]);
                                continue;
                            }

                            Debug.LogErrorFormat("Duplicate localization key found: {0}", text2);
                        }
                    }
                }
            }

            AutoLanguage();
        }

        public static string Localize(string localizationKey)
        {
            if (Dictionary.Count == 0)
            {
                Read();
            }

            if (!Dictionary.ContainsKey(Language))
            {
                return $"[{Language}] <{localizationKey}>";
            }

            if (!Dictionary[Language].ContainsKey(localizationKey))
            {
                return $"<{localizationKey}>";
            }

            return Dictionary[Language][localizationKey];
        }

        public static string Localize(string localizationKey, params object[] args)
        {
            return string.Format(Localize(localizationKey), args);
        }

        private static string ReplaceMarkers(string text)
        {
            return text.Replace("[Newline]", "\n");
        }

        public static Dictionary<string, string> GetLanguage(string code)
        {
            if (Dictionary.ContainsKey(code))
            {
                return Dictionary[code];
            }

            return null;
        }

        public static string GetTextForLanguage(string code, string key)
        {
            if (Dictionary.ContainsKey(code))
            {
                Dictionary.TryGetValue(code, out var value);
                if (value.ContainsKey(key))
                {
                    value.TryGetValue(key, out var value2);
                    if (value2 == string.Empty)
                    {
                        return $"<{code}>";
                    }

                    return value2;
                }
            }

            return $"<{code}>";
        }
    }
}