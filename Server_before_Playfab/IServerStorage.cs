using System;
using System.Collections.Generic;

namespace HapsServer
{
    public class HighFreqClientData
    {
        // Purpose: Data needed to verify frequent requests, like rolling and claiming wins.
        public string ClientId; // Also key
        public string ClientToken; // Given to client on login
        public DateTime TimeCachedUtc; // When was this data cached?
        public long Xp; // Infer level from this
    }

    public interface IPersistentStorage
    {
        void SetValue(string key, string value, bool forceWrite);
        string GetValue(string key, string value, bool forceRead);
    }

    public interface ICacheStorage
    {
        void SetValue(string key, string value);
        string GetValue(string key, string value);
    }

    public class FakePersistent : IPersistentStorage
    {
        Dictionary<string, string> store = new Dictionary<string, string>();

        public string GetValue(string key, string value, bool forceRead)
        {
            return store[key];
        }

        public void SetValue(string key, string value, bool forceWrite)
        {
            store[key] = value;
        }
    }

    public class DictionaryCache : ICacheStorage
    {
        Dictionary<string, string> store = new Dictionary<string, string>();

        public string GetValue(string key, string value)
        {
            return store[key];
        }

        public void SetValue(string key, string value)
        {
            store[key] = value;
        }
    }
}
