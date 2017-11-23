using UnityEngine;
using System.Collections;
using Assets.Script;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    //string IOSAdsId = "1205587";
    //string AndroidAdsId = "1205586";

    [Header("Gameplay Flags")]
    public bool ClearHoldFlagsOnSpin;
    public bool EnableAuto;
    public bool EnableWinHint;
    public bool EnableWinHelp;
    [Space(10)]

    [System.NonSerialized]
    public int[] WinLineCells = new int[] { 3, 4, 5 };

    public GameObject FruitCanvas;
    public Button[] HoldButtons = new Button[3];
    public Button ButtonStep;
    public Button ButtonSpin;
    public GameObject GameCanvas;
    public Image XpBarImage;
    public TextMeshProUGUI XpText;
    public TextMeshProUGUI LevelText;
    public float TimeScale = 1.0f;

    public ParticleSystem ParticlesLevel; // Replace by cutscene

    public enum SpinStateEnum { None, FullSpinRequired, Spinning, ShowCutscene, AwaitingUserDecisions };
    public SpinStateEnum SpinState = SpinStateEnum.None;
     
    WinLine winLine_;
    bool autoSpin_;

    Image[] holdButtonImage_ = new Image[3];
    Image buttonStepImage_;

    RollSequencer rollSequencer_;

    private Wheel wheel_;
    private bool[] holdFlags_ = new bool[3];
    private bool lastSpinWasFull_;

    private Color textColorNormal;
    private Color textColorDisabled;

    private void Awake()
    {
        rollSequencer_ = new RollSequencer(this);
        winLine_ = GameObject.Find("WinLine").GetComponent<WinLine>();

        wheel_ = FruitCanvas.GetComponent<Wheel>();
        wheel_.SetFinal9Provider(rollSequencer_);

        for (int i = 0; i < 3; ++i)
            holdButtonImage_[i] = HoldButtons[i].GetComponent<Image>();

        buttonStepImage_ = ButtonStep.GetComponent<Image>();

        textColorNormal = ButtonSpin.gameObject.GetComponent<Image>().color;
        textColorDisabled = Color.gray;
    }

    void Start()
    {
        ShowButtons(fullSpinRequired: true);
        StartCoroutine(GameLoop());
    }

    IEnumerator CutSceneWrapper(IEnumerator cutScene)
    {
        SpinStateEnum savedGameState = SpinState;
        SetGameState(SpinStateEnum.ShowCutscene);

        // Disable main game

        // Execute cutscene
        yield return cutScene;

        // Enable main game
        SetGameState(savedGameState);
    }

    IEnumerator ServerColdStart()
    {
        // Call at startup, maybe also resume etc.
        yield return Server.Instance.DoLoginCo();
        yield return GameData.Instance.GetAllValuesFromServer();

        rollSequencer_.SetDistribution(GameData.Instance.Distribution);
    }

    IEnumerator InitGame()
    {
        yield return ServerColdStart();
        UpdateXpControls();

        SetGameState(SpinStateEnum.FullSpinRequired);
    }

    void UpdateXpControls()
    {
        LevelText.text = string.Format("{0}", GameData.Instance.Level);
        XpText.text = string.Format("{0} / {1}", GameData.Instance.XpInLevel, GameData.Instance.XpForLevel);
        XpBarImage.fillAmount = GameData.Instance.PctInLevel;
    }

    Tweener xpBarFlashTween;
    private void UpdateXp(int amount)
    {
        if (amount == 0)
            return;

        int prevLevel = GameData.Instance.Level;
        GameData.Instance.AddXp(amount);

        if (xpBarFlashTween == null || !xpBarFlashTween.IsPlaying())
            xpBarFlashTween = XpBarImage.DOColor(Color.white, 0.1f).SetEase(Ease.Flash).SetLoops(2, LoopType.Yoyo);

        bool levelUp = GameData.Instance.Level != 1 && prevLevel != GameData.Instance.Level;
        if (levelUp)
            ParticlesLevel.Play();

        UpdateXpControls();
    }

    IEnumerator WaitForState(SpinStateEnum state, float timeout = -1)
    {
        float endTime = Time.time + timeout;
        while (SpinState != state)
        {
            if (timeout != -1 && Time.time >= endTime)
                break;

            yield return null;
        }
    }

    void ShowButtons(bool fullSpinRequired)
    {
        float speed = 0.5f;
        Color color = fullSpinRequired ? textColorDisabled : textColorNormal;
        buttonStepImage_.DOColor(color, speed);
        for (int i = 0; i < 3; ++i)
            holdButtonImage_[i].DOColor(color, speed);
    }

    IEnumerator GameLoop()
    {
        SpinState = SpinStateEnum.None;

        yield return InitGame();

        // Outer game loop
        while (true)
        {
NewRound:
            ShowButtons(fullSpinRequired: true);

            SetGameState(SpinStateEnum.FullSpinRequired);

            holdFlags_[0] = false;
            holdFlags_[1] = false;
            holdFlags_[2] = false;
            UpdateHoldButtons();

            var spinLabel = ButtonSpin.GetComponentInChildren<TextMeshProUGUI>();
            var spinLabelTween = spinLabel.transform.DOPunchScale(new Vector2(0.3f, 0.3f), 1.0f, 2, 1).SetLoops(-1);

            // Wait for user doing a full spin
            yield return WaitForState(SpinStateEnum.Spinning);
            spinLabelTween.Kill();
            spinLabel.transform.DOScale(1.0f, 0.1f);

            SetWinLine(new int[] { 3, 4, 5}); // Default middle

            int winIdx;
            bool playerInterfered = false;
            while (!playerInterfered)
            {
                ShowButtons(fullSpinRequired: false);

                // Wait for spin to be done
                while (wheel_.IsSpinning)
                    yield return null;

                if (CheckWin(out winIdx))
                {
                    // The player won something on initial spin, celebrate a bit, then back to full spin
                    SetGameState(SpinStateEnum.ShowCutscene);
                    yield return ShowWin(winIdx);
                    goto NewRound;
                }

                // Player did not auto-win on initial spin
                SetGameState(SpinStateEnum.AwaitingUserDecisions);
                bool canWin = CheckForWinHint() > 0;
                if (!canWin && autoSpin_)
                {
                    // Nothing to win and auto-spin is enabled. Start all over.
                    yield return new WaitForSeconds(0.5f);
                    ExecuteSpin(fullSpin: true);
                    continue;
                }

                yield return WaitForState(SpinStateEnum.Spinning);

                CheckForWinHint(hide: true);

                bool playerUsedHold = holdFlags_[0] != false || holdFlags_[1] != false || holdFlags_[2] != false;
                // Keep looping full spins until player holds or nudges
                playerInterfered = playerUsedHold || !lastSpinWasFull_;

                // Reset win line to default if player didn't interfere, since it doesn't matter where it is
                if (!playerInterfered)
                    SetWinLine(new int[] { 3, 4, 5 }); // Default middle
            }

            // Player used hold and/or nudge. Wait for spin to be done
            while (wheel_.IsSpinning)
                yield return null;

            if (CheckWin(out winIdx))
            {
                // The player won something on secondary spin, celebrate a bit, then back to full spin
                SetGameState(SpinStateEnum.ShowCutscene);
                yield return ShowWin(winIdx);
                continue;
            }

            // The player didn't win anything this round. Back to square one.
        }
    }

    public void SetAutoSpin(bool auto)
    {
        autoSpin_ = auto;
    }

    private IEnumerator ShowWin(int winIdx)
    {
        // Add displayedXp_
        ShowXpGain(77);
        yield break;
    }

    private bool CheckWin(out int winIdx)
    {
        winIdx = -1;
        var final9 = wheel_.GetFinal9();
        int id0 = WinLineCells[0];
        int id1 = WinLineCells[1];
        int id2 = WinLineCells[2];
        if ((final9[id0] == final9[id1]) && (final9[id1] == final9[id2]))
        {
            // All 3 are equal
            winIdx = final9[id0];
            AppLog.StaticLogInfo("Matched 3: {0}", winIdx);
            return true;
        }

        return false;
    }

    void SetGameState(SpinStateEnum newState)
    {
        AppLog.StaticLogInfo("Setting new game state: {0}", newState);
        SpinState = newState;
    }

    private void ExecuteSpin(bool fullSpin)
    {
        if (!fullSpin)
        {
            wheel_.DoNudge(holdFlags_);
        }
        else
        {
            if (ClearHoldFlagsOnSpin)
            {
                holdFlags_[0] = false;
                holdFlags_[1] = false;
                holdFlags_[2] = false;
                UpdateHoldButtons();
            }

            var row0 = new List<int>();
            var row1 = new List<int>();
            var row2 = new List<int>();
            rollSequencer_.GetRandom(row0, row1, row2);
            wheel_.SetNextSpin(row0, row1, row2);
            wheel_.DoFullSpin(holdFlags_);
        }

        lastSpinWasFull_ = fullSpin;
        SetGameState(SpinStateEnum.Spinning);
    }

    void ExecuteHoldButton(int idxHoldButton)
    {
        int idxOther1 = 0;
        int idxOther2 = 0;
        switch (idxHoldButton)
        {
            case 0: idxOther1 = 1; idxOther2 = 2; break;
            case 1: idxOther1 = 0; idxOther2 = 2; break;
            case 2: idxOther1 = 0; idxOther2 = 1; break;
            default: Debug.Log("Unknown hold idx: " + idxHoldButton); break;
        }

        bool bothOtherAreOn = holdFlags_[idxOther1] && holdFlags_[idxOther2];
        // POLISH: Could show error or play nah-sound here
        holdFlags_[idxHoldButton] = !holdFlags_[idxHoldButton] && !bothOtherAreOn;
        UpdateHoldButton(idxHoldButton);
    }

    public void OnHold0Click()
    {
        if (SpinState != SpinStateEnum.AwaitingUserDecisions)
            return;

        ExecuteHoldButton(0);
    }

    public void OnHold1Click()
    {
        if (SpinState != SpinStateEnum.AwaitingUserDecisions)
            return;

        ExecuteHoldButton(1);
    }

    public void OnHold2Click()
    {
        if (SpinState != SpinStateEnum.AwaitingUserDecisions)
            return;

        ExecuteHoldButton(2);
    }

    void UpdateHoldButton(int idx)
    {
        // TODO: Color is hardcoded here
        Color color = holdFlags_[idx] ? Color.gray : Color.white;
        var colorBlock = HoldButtons[idx].colors;
        colorBlock.normalColor = color;
        colorBlock.highlightedColor = color;
        HoldButtons[idx].colors = colorBlock;
        HoldButtons[idx].GetComponent<HoldButtonScript>().SetLocked(holdFlags_[idx]);
    }

    void UpdateHoldButtons()
    {
        UpdateHoldButton(0);
        UpdateHoldButton(1);
        UpdateHoldButton(2);
    }

    public void OnGoClick()
    {
        // TODO: Checking state like this is fragile
        if (SpinState != SpinStateEnum.AwaitingUserDecisions && SpinState != SpinStateEnum.FullSpinRequired)
            return;

        ExecuteSpin(fullSpin: true);
    }

    public void OnNudgeClick()
    {
        if (SpinState != SpinStateEnum.AwaitingUserDecisions)
            return;

        ExecuteSpin(fullSpin: false);
    }

    int CheckForWinHint(bool hide = false)
    {
        // TODO: Fast re-spin can mess this up since transition is not completed
        if (hide)
        {
            HintScript.Instance.HideDialog();
            return 0;
        }

        var final9 = wheel_.GetFinal9();

        List<int> lineIfWin = new List<int> { 0, 0, 0 };
        List<int> iconsTobeMatched = new List<int> { 0, 0, 0 };
        int potentialWinCount = PotentialWins.CheckForWin(PotentialWins.Difficulty.Any, final9, lineIfWin, iconsTobeMatched);
        if (potentialWinCount > 0)
        {
            HintScript.Instance.ShowHint(PotentialWins.Difficulty.Any, final9, lineIfWin, winLine_, iconsTobeMatched);
        }

        return potentialWinCount;
    }

    public void SetWinLine(int[] cells)
    {
        WinLineCells = cells;
        winLine_.UpdateTarget(cells, 50);
    }

    public void XpAddCallback(int amount)
    {
        UpdateXp(amount);
    }

    public void ShowXpGain(int amount)
    {
        // TODO: Instance, block or queue. Starting two messes things up.
        XpCelebrationLarge.Instance.Show(amount, XpAddCallback);
    }

    public void ShowYard()
    {
        // TODO: Merge with main scene?
        SceneManager.LoadScene("YardScene");
    }

    public void ButtonAddXp()
    {
        ShowXpGain(66);
    }

    public void ShowQuestion()
    {
        string q = "Hvor længe kan en sæl holde vejret?";
        QuestionScript.ShowQuestionDialogStatic(q, "80 sekunder", "80 minutter", "1000 XP", QuestionScript.Answer.B, OnQuestionAnswered);
    }

    public void DeleteData()
    {

    }

    void OnQuestionAnswered(bool correctAnswer)
    {

    }

    private void Update()
    {
        Time.timeScale = TimeScale;
    }
}
