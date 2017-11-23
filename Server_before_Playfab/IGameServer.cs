using System;
using System.Collections.Generic;

namespace HapsServerInterface
{
    public enum GameServerResult { None, Success, CredentialsError, NoConnection, UnknownError };

    public interface IGameSecurity
    {
        void Authenticate(Credentials credentials, Action<GameServerResult, string> callback);
    }

    public interface IGameRules
    {
        void GetIconDisplayProbabilities(Credentials client, Action<GameServerResult, List<IconProbability>> callback);
        void GetPlayerBaseData(Credentials client, Action<GameServerResult, PlayerBaseData> callback);
        void ClaimWin(Credentials client, List<int> final9, long batchId, Action<GameServerResult> callback);
        void GetRollSequence(Credentials client, Action<GameServerResult, List<Roll>> callback);
    }

    public interface IGameStatistics
    {
    }

    public interface IGameSocial
    {
    }

    public interface IGameLogging
    {
    }

    public interface IGameServer : IGameSecurity, IGameRules, IGameStatistics, IGameSocial, IGameLogging
    {
    }
}
