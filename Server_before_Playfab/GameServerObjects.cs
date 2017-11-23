using System.Collections.Generic;

namespace HapsServerInterface
{
    [System.Serializable]
    public class Credentials
    {
        public string ClientId;
        public string ClientToken;
        public string DeviceId;
    }

    [System.Serializable]
    public class IconProbability
    {
        public string Id;
        public double P;
    }

    [System.Serializable]
    public class Roll
    {
        public long BatchId;
        public List<int> Icons = new List<int>();
    }

    public enum PlayerOwnedKey { };
    [System.Serializable]
    class PlayerOwnedData
    {
        public Dictionary<string, string> KeyValue = new Dictionary<string, string>();
        }

    [System.Serializable]
    public class PlayerBaseData
    {
        // These values are read once at startup.
        // The client cannot write them back, updates are done via other actions.

        public string UserId; // User id is generated server side when a user is first seen (TODO: FB id? Multiple devices)
        public string DisplayName;
        public string TitlePreId;
        public string TitlePostId;
        public long Xp;
        public bool IsAnonymous;
        public string CurrentDoneeId;
    }
}
