using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script
{
    public interface ILocalization
    {
        void SetLanguage(string langId);
        string GetLocalizedText(string key);
    }

    public class LocalizationManager : ILocalization
    {
        IAppLog appLog_;

        Dictionary<string, Dictionary<string, string>> languageMap_ = new Dictionary<string, Dictionary<string, string>>();
        string langId_;

        public LocalizationManager(string resource)
        {
            appLog_ = ServiceManager.GameServices.GetAppLog();

            var textAsset = Resources.Load<TextAsset>(resource);
            appLog_.LogErrorIfNull(textAsset, resource);

            var lines = textAsset.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; ++i)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;

                List<string> header = new List<string>();
                var items = lines[i].Split('\t');
                if (i == 0)
                {
                    header.AddRange(items);
                    for (int j = 1; j < items.Length; ++j)
                    {
                        string lang = header[j];
                        languageMap_[lang] = new Dictionary<string, string>();
                    }
                }
                else
                {
                    string key = items[0];
                    for (int j = 1; j < items.Length; ++j)
                    {
                        string lang = header[j];
                        string value = items[j];
                        languageMap_[lang][key] = value;
                    }
                }
            }
        }

        public string GetLocalizedText(string key)
        {
            Dictionary<string, string> map;
            if (!languageMap_.TryGetValue(langId_, out map))
            {
                return string.Format("{0}: unknown lang id", langId_);
            }

            string result;
            if (!map.TryGetValue(key, out result))
            {
                return string.Format("{0}:{1}: unknown key", key, langId_);
            }
            return result;
        }

        public void SetLanguage(string langId)
        {
            appLog_.LogErrorIfFalse(languageMap_.ContainsKey(langId), "I8N: Trying to set unknown language id: {0}", langId);
            langId_ = langId;
        }
    }
}