using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlipWebApps.BeautifulTransitions.Scripts.Transitions.GameObject;

public class HintScript : MonoBehaviour
{
    public static HintScript Instance;

    public GameObject HandProto;
    public TransitionMoveTraget Transition;
    public GameObject InputBlocker;

    [System.NonSerialized]
    public bool IsShown;
    const float PointerMoveSpeed = 400.0f;

    List<int> final9_;
    List<int> lineIfWin_;
    List<int> iconsToBeMatched_;
    WinLine winline_;

    public void ShowHint(PotentialWins.Difficulty level, List<int> final9, List<int> lineIfWin, WinLine winLine, List<int> iconsToBeMatched)
    {
        IsShown = true;

        final9_ = final9;
        lineIfWin_ = lineIfWin;
        winline_ = winLine;
        iconsToBeMatched_ = iconsToBeMatched;

        Transition.InitTransitionIn();
        Transition.TransitionIn();
    }

    public void HideDialog()
    {
        if (!IsShown)
            return;

        Transition.InitTransitionOut();
        Transition.TransitionOut();
        IsShown = false;
    }

    void Awake()
    {
        Instance = this;
        InputBlocker.SetActive(false);
    }

    public void OnHelpMe()
    {
        // Use this to debug wrong hints
        //List<int> lineIfWin = new List<int> { 0, 0, 0 };
        //List<int> iconsTobeMatched = new List<int> { 0, 0, 0 };
        //bool canWin = PotentialWins.CheckForWin(PotentialWins.Difficulty.Any, final9_, lineIfWin, iconsTobeMatched);
        StartCoroutine(ShowHand());
    }

    Vector2 GetCenterFromTag(string tag)
    {
        var go = GameObject.FindGameObjectWithTag(tag);
        var pos = go.transform.position;
        return pos;
    }

    void PointerMoveTo(Vector3 target, float speed, HelpingHandScript handScript)
    {
        handScript.AddCommand(HandCommandMoveTo.Create(target, speed));
    }

    void MoveToObjectWithTag(string tag, HelpingHandScript hand, float speed = PointerMoveSpeed)
    {
        PointerMoveTo(GetCenterFromTag(tag), speed, hand);
    }

    void PointerShowHoldIfSet(bool isSet, string tag, HelpingHandScript handScript)
    {
        if (!isSet)
            return;

        const float HoldPointTime = 0.5f;

        MoveToObjectWithTag(tag, handScript);
        handScript.AddCommand(new HandCommandPointerDown());
        handScript.AddCommand(HandCommandPause.Create(HoldPointTime));
        handScript.AddCommand(new HandCommandPointerUp());
        handScript.AddCommand(HandCommandPause.Create(HoldPointTime * 0.5f));
    }

    // 0 1 2
    // 3 4 5
    // 6 7 8
    void PointerShowWinLine(int idx0, int idx1, HelpingHandScript handScript)
    {
        int line0 = (idx0 / 3) - 1; // -1 -> 1
        int line1 = (idx1 / 3) - 1; // -1 -> 1
        bool isDiagonal = line0 != line1;

        var go = GameObject.FindGameObjectWithTag("SpinWheel");
        var rectTrans = go.transform.rectTransform();
        var pos = go.transform.position;
        float x0 = pos.x - rectTrans.rect.width * 0.45f;
        float x1 = pos.x + rectTrans.rect.width * 0.45f;
        float y0 = pos.y + rectTrans.rect.height * (isDiagonal ? 0.6f : 0.5f);
        float y1 = pos.y - rectTrans.rect.height * (isDiagonal ? 0.6f : 0.5f);
        float w = x1 - x0;
        float h = y1 - y0;
        float iconH = h / 3;
        float iconW = w / 3;

        var startPoint = pos + (new Vector3(x0, iconH * line0, 0.0f));
        var endPoint = pos + (new Vector3(x1, iconH * line1, 0.0f));

        PointerMoveTo(startPoint, PointerMoveSpeed, handScript);
        handScript.AddCommand(new HandCommandPointerDown());
        handScript.AddCommand(HandCommandPause.Create(0.5f));
        PointerMoveTo(endPoint, PointerMoveSpeed, handScript);
        handScript.AddCommand(new HandCommandPointerUp());
        handScript.AddCommand(HandCommandPause.Create(0.5f));
    }

    IEnumerator ShowHand()
    {
        InputBlocker.SetActive(true);
        InputManager.EnableDirectInput(false);

        var handGo = Instantiate(HandProto, Vector3.zero, Quaternion.identity);
        handGo.SetActive(true);
        var handScript = handGo.GetComponentInChildren<HelpingHandScript>();

        var currentWinLine = winline_.Line;
        bool lineIsAlreadyCorrect =
            lineIfWin_[0] == currentWinLine[0] &&
            lineIfWin_[1] == currentWinLine[1] &&
            lineIfWin_[2] == currentWinLine[2];

        if (!lineIsAlreadyCorrect)
            PointerShowWinLine(lineIfWin_[0], lineIfWin_[2], handScript);

        var holdFlags = new bool[3] { false, false, false };
        PotentialWins.GetHoldFlags(holdFlags, final9_, lineIfWin_, iconsToBeMatched_);

        PointerShowHoldIfSet(holdFlags[0], "LockButton0", handScript);
        PointerShowHoldIfSet(holdFlags[1], "LockButton1", handScript);
        PointerShowHoldIfSet(holdFlags[2], "LockButton2", handScript);

        // Always end with Nudge. Short moves seems slower? Speed it up for now.
        MoveToObjectWithTag("NudgeButton", handScript, PointerMoveSpeed * 2.0f);

        const float NudgePointTime = 0.5f;
        handScript.AddCommand(new HandCommandPointerDown());
        handScript.AddCommand(HandCommandPause.Create(NudgePointTime));
        handScript.AddCommand(new HandCommandPointerUp());

        yield return handScript.RunCommands();

        GameObject.Destroy(handGo);

        InputBlocker.SetActive(false);
        InputManager.EnableDirectInput(true);
    }
}
