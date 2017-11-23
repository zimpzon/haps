using System.Collections;

public interface IServer
{
    // --- Begin DoServerColdStart ---
    IEnumerable DoServerColdStart();
    // --- Calls executed by DoServerColdStart: ---
    IEnumerator DoLoginCo();
    IEnumerator GetAllPlayerData();
    // --- End DoServerColdStart ---

    IEnumerator GetRollData();
}
