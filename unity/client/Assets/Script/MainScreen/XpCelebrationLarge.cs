using System.Collections;
using TMPro;
using DG.Tweening;
using UnityEngine;
using FlipWebApps.BeautifulTransitions.Scripts.Transitions.GameObject;
using System;

public class XpCelebrationLarge : MonoBehaviour
{
    public static XpCelebrationLarge Instance { get; private set; }

    public GameObject RootCanvas;
    public TextMeshProUGUI XpText;
    public ParticleSystem Fireworks;
    public TransitionScale Transition;

    int xp_;
    float timeShown_;
    float textScale_;
    bool showFireworks_;
    Action<int> cb_;

    IEnumerator LifeCycle()
    {
        XpText.text = string.Format("{0} XP", xp_);
        XpText.gameObject.transform.localScale = new Vector3(textScale_, textScale_, 1.0f);
        XpText.transform.DOLocalMoveY(216, 0.8f).SetEase(Ease.OutBounce);

        Transition.InitTransitionIn();
        Transition.TransitionIn();

        if (showFireworks_)
            Fireworks.Play(true);

        yield return new WaitForSeconds(1.0f);

        const int Steps = 20;
        const float MsPerStep = 32;

        float amountLeft = xp_;
        float amountPerStep = xp_ / (float)Steps;

        float xpToSend = 0.0f;
        for (int i = 0; i < Steps; ++i)
        {
            // Count xp with fractions but report integers back. Save the fractional part for next round.
            xpToSend += amountPerStep;
            int roundedAmount = Mathf.RoundToInt(xpToSend);
            cb_(roundedAmount);
            xpToSend = xpToSend - roundedAmount;

            amountLeft = Mathf.Max(amountLeft - amountPerStep, 0);
            XpText.text = string.Format("{0} XP", Mathf.RoundToInt(amountLeft));
            if (amountLeft == 0)
            {
                // There could be a fraction left when we are done. Round it and send.
                cb_(Mathf.RoundToInt(roundedAmount));
                break;
            }

            XpText.transform.rotation = Quaternion.Euler(0.0f, 0.0f, UnityEngine.Random.Range(-15.0f, 15.0f));

            yield return new WaitForSeconds(MsPerStep * 0.001f);
        }

        XpText.transform.rotation = Quaternion.identity;

        yield return new WaitForSeconds(0.5f);

        Transition.InitTransitionOut();
        Transition.TransitionOut();
        Fireworks.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        yield return new WaitForSeconds(0.5f);
        ResetAll();
    }

    // particleCount: 10 = few 30 = many
    public void Show(int xp, Action<int> xpAddCallback,  float timeShown = 2.0f, float textScale = 1.0f, bool showFireworks = false)
    {
        xp_ = xp;
        cb_ = xpAddCallback;
        timeShown_ = timeShown;
        textScale_ = textScale;
        showFireworks_ = showFireworks;

        RootCanvas.SetActive(true);
        StartCoroutine(LifeCycle());
    }

    void ResetAll()
    {
        RootCanvas.SetActive(false);
        XpText.transform.rotation = Quaternion.identity;
        XpText.transform.DOLocalMoveY(-160, 0.0f);
    }

    private void Awake()
    {
        Instance = this;
        ResetAll();
    }
}
