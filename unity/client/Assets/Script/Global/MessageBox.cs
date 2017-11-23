using System;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    class WaitForMessageBoxClosed : CustomYieldInstruction
    {
        public MessageBox MessageBox;
        public override bool keepWaiting { get { return MessageBox.LastResult == MessageBox.Result.None; } }
        public WaitForMessageBoxClosed(MessageBox messageBox)
        {
            MessageBox = messageBox;
        }
    }

    public static MessageBox Instance;

    public enum Buttons { Ok, OkCancel };
    public enum Result { None, Ok, Cancel };

    public Image Background;
    public Image Box;
    public TextMeshProUGUI Text;
    public GameObject ButtonOk;
    public GameObject ButtonCancel;
    Action<Result> onClose_;
    Vector3 buttonOkBasePos_;
    Vector3 buttonOkCenterPos_;

    [NonSerialized]
    public Result LastResult = Result.None;

    void Awake()
    {
        Instance = this;

        buttonOkBasePos_ = ButtonOk.transform.localPosition;
        buttonOkCenterPos_ = buttonOkBasePos_;
        buttonOkCenterPos_.x = 0;

        Hide();
    }

    void Hide()
    {
        this.gameObject.SetActive(false);
        this.gameObject.transform.position = Vector3.left * 3000;
    }

    public CustomYieldInstruction Show(string text, Buttons buttons, bool fadeInBackground = true, Action<Result> onClose = null)
    {
        Text.text = text;
        onClose_ = onClose;
        LastResult = Result.None;

        switch(buttons)
        {
            case Buttons.Ok:
                ButtonOk.transform.localPosition = buttonOkCenterPos_;
                ButtonCancel.SetActive(false);
                break;
            case Buttons.OkCancel:
                ButtonOk.transform.localPosition = buttonOkBasePos_;
                ButtonCancel.SetActive(true);
                break;
        }

        Background.DOFade(0.0f, 0.0f);
        Background.DOFade(0.8f, fadeInBackground ? 0.6f : 0.0f);
        Box.transform.DOScale(0.5f, 0.0f);
        Box.transform.DOScale(1.0f, 0.3f).SetEase(Ease.OutQuad);
        Box.DOFade(0.0f, 0.0f);
        Box.DOFade(1.0f, 0.3f);

        this.gameObject.transform.position = Vector3.zero;
        this.gameObject.SetActive(true);

        return new WaitForMessageBoxClosed(this);
    }

    void Close(Result result)
    {
        LastResult = result;
        Hide();

        if (onClose_ != null)
            onClose_(result);
    }

    public void ButtonOkClicked()
    {
        Close(Result.Ok);
    }

    public void ButtonCancelClicked()
    {
        Close(Result.Cancel);
    }
}
