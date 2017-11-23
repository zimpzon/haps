using FlipWebApps.BeautifulTransitions.Scripts.Transitions.GameObject;
using System;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class QuestionScript : MonoBehaviour, IPointerClickHandler
{
    public enum Answer { None, A, B };

    public static void ShowQuestionDialogStatic(string q, string answerA, string answerB, string reward, Answer correctAnswer, Action<bool> callback)
    {
        Instance.ShowQuestionDialog(q, answerA, answerB, reward, correctAnswer, callback);
    }

    enum State { Showing, AwaitingAnswer, AnswerDetermined, AwaitingHide, Hiding, Hidden }

    public GameObject RootCanvas;
    public Image BackgroundPanel;
    public TextMeshProUGUI TextQuestion;
    public TextMeshProUGUI TextAnswers;
    public TextMeshProUGUI TextTimeLeft;
    public TextMeshProUGUI TextReward;
    public TextMeshProUGUI TextRevealAnswer;
    public Button ButtonA;
    public Button ButtonB;
    public Button ButtonBackArrow;
    public TransitionMoveTraget Transition;
    public int TimeToAnswer = 30;
    public int ShowResultTime = int.MaxValue;

    static QuestionScript Instance;

    State state_;
    Action<bool> callback_;
    Answer correctAnswer_;
    string questionTemplate_;
    string timeLeftTemplate_;
    string rewardTemplate_;
    string answersTemplate_;
    string revealTemplate_;
    string revealNoAnswerTemplate_;
    string answerA_;
    string answerB_;
    bool wasCorrectAnswer_;
    Vector2 buttonAOriginalPos_;
    Vector2 buttonBOriginalPos_;
    float endTime_;
    int lastShownTime_;

    void Awake()
    {
        Instance = this;

        questionTemplate_ = TextQuestion.text;
        answersTemplate_ = TextAnswers.text;
        timeLeftTemplate_ = TextTimeLeft.text;
        rewardTemplate_ = TextReward.text;
        revealTemplate_ = TextRevealAnswer.text;
        revealNoAnswerTemplate_ = "Du nåede ikke at svare inden tiden gik!";

        buttonAOriginalPos_ = ButtonA.transform.position;
        buttonBOriginalPos_ = ButtonB.transform.position;
    }

    void ResetAll()
    {
        BackgroundPanel.DOFade(0.0f, 0.0f);
        TextAnswers.gameObject.SetActive(true);
        TextReward.gameObject.SetActive(true);
        TextTimeLeft.gameObject.SetActive(true);
        ButtonA.gameObject.SetActive(true);
        ButtonBackArrow.gameObject.SetActive(false);
        ButtonB.gameObject.SetActive(true);
        lastShownTime_ = 0;
        TextRevealAnswer.gameObject.SetActive(false);
        state_ = State.Hidden;

        RootCanvas.SetActive(false);
    }

    void Start()
    {
        ResetAll();
    }

    public void ShowQuestionDialog(string question, string answerA, string answerB, string reward, Answer correctAnswer, Action<bool> callback)
    {
        // Randomly switch A and B to avoid patterns
        if (UnityEngine.Random.value < 0.5)
        {
            string temp = answerB;
            answerB = answerA;
            answerA = temp;
            correctAnswer = correctAnswer == Answer.A ? Answer.B : Answer.A;
        }

        TextQuestion.text = string.Format(questionTemplate_, question);
        TextAnswers.text = string.Format(answersTemplate_, answerA, answerB);
        TextReward.text = string.Format(rewardTemplate_, reward);

        correctAnswer_ = correctAnswer;
        callback_ = callback;
        answerA_ = answerA;
        answerB_ = answerB;
        endTime_ = Time.time + TimeToAnswer;

        RootCanvas.SetActive(true);

        Transition.InitTransitionIn();
        Transition.TransitionIn();
        BackgroundPanel.DOFade(0.8f, 0.5f);
        state_ = State.Showing;
        InputManager.EnableDirectInput(false);
    }

    void HideDialog()
    {
        state_ = State.Hiding;
        Transition.InitTransitionOut();
        Transition.TransitionOut();
        BackgroundPanel.DOFade(0.0f, 0.1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (state_ == State.AwaitingHide)
            state_ = State.Hiding;
    }

    public void OnBackButton()
    {
        state_ = State.Hiding;
    }

    IEnumerator AwaitHide()
    {
        state_ = State.AwaitingHide;

        ButtonBackArrow.gameObject.SetActive(true);

        float autoHideTime = Time.time + ShowResultTime;
        while (Time.time < autoHideTime && state_ == State.AwaitingHide)
        {
            if (InputManager.DeviceBackButtonActivated())
                break;

            yield return null;
        }

        HideDialog();
    }

    IEnumerator RevealText(TextMeshProUGUI label, float delayLast = 0.0f)
    {
        label.ForceMeshUpdate();

        int totalChars = label.textInfo.characterCount;
        int charsShown = 0;
        while (charsShown < totalChars)
        {
            label.maxVisibleCharacters = ++charsShown;
            bool nextIsLastChar = charsShown == totalChars - 1;
            yield return new WaitForSeconds(delayLast > 0.0f && nextIsLastChar ? delayLast : 0.05f);
        }
    }

    IEnumerator ShowCorrectAnswer(Answer selected)
    {
        state_ = State.AnswerDetermined;
        wasCorrectAnswer_ = selected == correctAnswer_;

        ButtonA.gameObject.SetActive(false);
        ButtonB.gameObject.SetActive(false);
        TextTimeLeft.gameObject.SetActive(false);

        if (selected == Answer.None)
        {
            TextRevealAnswer.text = revealNoAnswerTemplate_;
            TextRevealAnswer.gameObject.SetActive(true);

            yield return RevealText(TextRevealAnswer);

            SoundManager.Instance.PlayPrimary(SoundManager.Instance.ClipUIQuestionWrong);

            StartCoroutine(AwaitHide());
            yield break;
        }

        string selectedAnswerA = selected == Answer.A ?
            string.Format("<#ffffff>{0}</color>", answerA_) : answerA_;

        string selectedAnswerB = selected == Answer.B ?
            string.Format("<#00ff00>{0}</color>", answerB_) : answerB_;

        TextAnswers.text = string.Format(answersTemplate_, selectedAnswerA, selectedAnswerB);

        TextRevealAnswer.text = string.Format(revealTemplate_, selected.ToString(), correctAnswer_.ToString());
        TextRevealAnswer.gameObject.SetActive(true);

        yield return RevealText(TextRevealAnswer, 1.0f);

        SoundManager.Instance.PlayPrimary(wasCorrectAnswer_ ?
            SoundManager.Instance.ClipUIQuestionCorrect :
            SoundManager.Instance.ClipUIQuestionWrong);

        string finalAnswerA = correctAnswer_ == Answer.A ?
            string.Format("<#00ff00>{0}</color>", answerA_) :
            string.Format("<s>{0}</s>", answerA_);

        string finalAnswerB = correctAnswer_ == Answer.B ?
            string.Format("<#00ff00>{0}</color>", answerB_) :
            string.Format("<s>{0}</s>", answerB_);

        TextAnswers.text = string.Format(answersTemplate_, finalAnswerA, finalAnswerB);

        StartCoroutine(AwaitHide());
    }

    public void OnButtonClickA()
    {
        StartCoroutine(ShowCorrectAnswer(Answer.A));
    }

    public void OnButtonClickB()
    {
        StartCoroutine(ShowCorrectAnswer(Answer.B));
    }

    public void OnTransitionOutDone()
    {
        ResetAll();
        InputManager.EnableDirectInput(true);
        callback_(wasCorrectAnswer_);
    }

    public void OnTransitionInDone()
    {
        state_ = State.AwaitingAnswer;
    }

    void UpdateTimer(float currentTime)
    {
        if (state_ != State.AwaitingAnswer)
            return;

        int secondsLeft = Mathf.Max(0, (int)(endTime_ - currentTime));

        if (secondsLeft != lastShownTime_)
        {
            if (secondsLeft < 10)
            {
                SoundManager.Instance.PlayPrimary(SoundManager.Instance.ClipClockTick);
                TextTimeLeft.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.0f), 0.5f);
            }

            TextTimeLeft.text = string.Format(timeLeftTemplate_, secondsLeft);
            lastShownTime_ = secondsLeft;
        }

        if (secondsLeft == 0)
            StartCoroutine(ShowCorrectAnswer(Answer.None));
    }

    void Update()
    {
        UpdateTimer(Time.time);
    }
}

