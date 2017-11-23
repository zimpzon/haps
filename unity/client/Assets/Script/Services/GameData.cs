using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconProbability
{
    // Must match json from server. Case-sensitive.
    public string Id { get; set; }
    public double P { get; set; }
}

public class Roll
{
    public List<int> Icons = new List<int>();
}

public class Donee
{
    // Must match json from server. Case-sensitive.
    public string Id { get; set; }
    public string Name { get; set; }
    public string Link { get; set; }
}

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    public long Xp { get; private set; }
    public int Credits { get; private set; }
    public int Tickets { get; private set; }
    public string MagicWordLetters { get; private set; }
    public List<string> UnlockedTitlesPre { get; private set; }
    public List<string> UnlockedTitlesPost { get; private set; }
    public string CurrentTitlePre;
    public string CurrentTitlePost;

    public List<Donee> Donees = new List<Donee>();
    public List<IconProbability> Distribution = new List<IconProbability>();

    public int Level { get; private set; }

    public long XpInLevel { get; private set; }
    public long XpForLevel { get; private set; }
    public float PctInLevel { get; private set; }

    const string KeyXp = "xp";
    const string KeyTickets = "TI";
    const string KeyCredits = "CR";

    void Awake()
    {
        Instance = this;
    }

    public IEnumerator GetAllValuesFromServer()
    {
        // Xp
        // Tickets
        // Credits
        // Jackpot status
        // Minigame status
        // Flashing between login and getting data is annoying.
        yield return Server.Instance.GetAllPlayerData();
        var result = (GetPlayerCombinedInfoResult)Server.Instance.LastResult;
        // set distribution in rollSequencer
        // update jackpot letters
        // update credits and tickets
        // TODO: jackpot timestamp and growthrate

        Credits = ValueUtil.GetValue<int, int>(result.InfoResultPayload.UserVirtualCurrency, KeyCredits);
        Tickets = ValueUtil.GetValue<int, int>(result.InfoResultPayload.UserVirtualCurrency, KeyTickets);
        Xp = ValueUtil.GetUserReadOnlyValue<long>(result.InfoResultPayload.UserReadOnlyData, KeyXp);
        Donees = ValueUtil.ParseJsonData<List<Donee>>(result.InfoResultPayload.TitleData, "donees");
        Distribution = ValueUtil.ParseJsonData<List<IconProbability>>(result.InfoResultPayload.TitleData, "externalDistribution");

        CalcDerivedValuesFromXp(Xp);
    }

    public void AddXp(int amount)
    {
        Xp += amount;
        CalcDerivedValuesFromXp(Xp);
    }

    void CalcDerivedValuesFromXp(long xp)
    {
        // For now: Simple linear
        const long XpPerLevel = 100;
        Level = (int)(xp / XpPerLevel) + 1;
        XpInLevel = Xp % XpPerLevel;
        XpForLevel = 100;
        PctInLevel = (float)XpInLevel / XpForLevel;
    }
}
