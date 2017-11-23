using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

public static class ValueUtil
{
    public static TResult GetValue<TDict, TResult>(Dictionary<string, TDict> dict, string key)
    {
        TDict value;
        dict.TryGetValue(key, out value);
        TResult result = (TResult)Convert.ChangeType(value, typeof(TResult));
        return result;
    }

    public static TResult GetUserReadOnlyValue<TResult>(Dictionary<string, UserDataRecord> dict, string key)
    {
        UserDataRecord rec;
        dict.TryGetValue(key, out rec);
        TResult result = rec == null ? default(TResult) : (TResult)Convert.ChangeType(rec.Value, typeof(TResult));
        return result;
    }

    public static TResult ParseJsonData<TResult>(Dictionary<string, string> dict, string key)
    {
        string json;
        dict.TryGetValue(key, out json);
        if (string.IsNullOrEmpty(json))
            return default(TResult);

        TResult result = PlayFab.Json.PlayFabSimpleJson.DeserializeObject<TResult>(json);
        return result;
    }

    internal static GetPlayerCombinedInfoResult GetTestCombinedInfo()
    {
        GetPlayerCombinedInfoResult result = new GetPlayerCombinedInfoResult();
        result.PlayFabId = "xxxx";
        result.InfoResultPayload = new GetPlayerCombinedInfoResultPayload();
        var pl = result.InfoResultPayload;
        pl.AccountInfo = new UserAccountInfo();
        pl.CharacterInventories = new List<CharacterInventory>();
        pl.PlayerProfile = new PlayerProfileModel();
        pl.PlayerStatistics = new List<StatisticValue>();

        pl.TitleData = new Dictionary<string, string>();
        pl.TitleData.Add("donees", OfflineJson.jsonDonees);
        pl.TitleData.Add("titles", OfflineJson.jsonTitles);
        pl.TitleData.Add("gameplayFlags", OfflineJson.jsonGameplayFlags);
        pl.TitleData.Add("externalDistribution", OfflineJson.jsonExternalDistribution);

        pl.UserData = new Dictionary<string, UserDataRecord>();
        pl.UserInventory = new List<ItemInstance>();
        pl.UserReadOnlyData = new Dictionary<string, UserDataRecord>();
        pl.UserReadOnlyData.Add("xp", new UserDataRecord() { Value = "0" });

        pl.UserVirtualCurrency = new Dictionary<string, int>();
        pl.UserVirtualCurrency.Add("CR", 1000);
        pl.UserVirtualCurrency.Add("TI", 100);
        pl.UserVirtualCurrencyRechargeTimes = new Dictionary<string, VirtualCurrencyRechargeTime>();
        return result;
    }
}
