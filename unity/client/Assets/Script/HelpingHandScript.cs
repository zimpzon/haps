using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HandCommand { }
public class HandCommandPointerUp : HandCommand { }
public class HandCommandPointerDown : HandCommand { }
public class HandCommandPause : HandCommand
{
    public static HandCommandPause Create(float time)
    {
        return new HandCommandPause() { Time = time };
    }

    public float Time;
}

public class HandCommandMoveTo : HandCommand
{
    public static HandCommandMoveTo Create(float x, float y, float z, float speed)
    {
        return HandCommandMoveTo.Create(new Vector3(x, y, z), speed);
    }

    public static HandCommandMoveTo Create(Vector3 target, float speed)
    {
        return new HandCommandMoveTo() { Target = target, Speed = speed };
    }

    public Vector3 Target; public float Speed;
}

public class HelpingHandScript : MonoBehaviour
{
    public Transform HandRoot;
    public Image HandImage;
    public ParticleSystem Particles;
    public Sprite HandUp;
    public Sprite HandDown;

    const float HandBaseScale = 1.2f;
    const float HandDownScale = 0.8f;
    bool isVisible_;

    List<HandCommand> commands_ = new List<HandCommand>();

    public void ClearCommands()
    {
        commands_.Clear();
    }

    public void AddCommand(HandCommand command)
    {
        commands_.Add(command);
    }

    public IEnumerator RunCommands()
    {
        HandImage.sprite = HandUp;

        foreach (var command in commands_)
        {
            if (command is HandCommandPause)
            {
                var typedCommand = command as HandCommandPause;
                yield return new WaitForSeconds(typedCommand.Time);
            }
            else if (command is HandCommandMoveTo)
            {
                var typedCommand = command as HandCommandMoveTo;
                if (!isVisible_)
                    yield return AppearAt(typedCommand.Target);

                yield return MoveTo(typedCommand.Target, typedCommand.Speed);
            }
            else if (command is HandCommandPointerDown)
            {
                Particles.Play();
                yield return HandImage.transform.DOScale(Vector3.one * HandBaseScale * HandDownScale, 0.1f);
            }
            else if (command is HandCommandPointerUp)
            {
                yield return HandImage.transform.DOScale(Vector3.one * HandBaseScale, 0.1f);
                Particles.Stop();
            }
            else
            {
                Debug.Log("Unknown hand command");
            }
        }

        if (isVisible_)
            yield return Hide();
    }

    private IEnumerator AppearAt(Vector3 target)
    {
        const float AppearTime = 1.0f;

        // Move in
        HandRoot.localPosition = Vector3.zero;
        HandRoot.DOLocalMove(target, AppearTime).SetEase(Ease.OutCirc);

        // Scale in
        HandImage.transform.DOScale(20.0f, 0.0f);
        HandImage.transform.DOScale(HandBaseScale, AppearTime * 0.9f).SetEase(Ease.OutSine);

        // Fade in
        HandImage.DOFade(0.0f, 0.0f);
        HandImage.DOFade(1.0f, AppearTime * 0.8f);

        yield return new WaitForSeconds(AppearTime);
        isVisible_ = true;
    }

    private IEnumerator Hide()
    {
        const float HideTime = 0.5f;
        // Move out
        HandRoot.DOLocalMove(Vector3.zero, HideTime).SetEase(Ease.InCirc);

        // Do scale
        HandImage.transform.DOScale(12.0f, HideTime).SetEase(Ease.InSine);

        // Do fade
        HandImage.DOFade(0.0f, HideTime);

        yield return new WaitForSeconds(HideTime);
        isVisible_ = false;
    }

    private IEnumerator MoveTo(Vector3 target, float speed)
    {
        float distance = (HandRoot.position - target).magnitude;
        float moveTime = distance / speed;
        if (speed < 0.0f || moveTime < 0.05f)
        {
            HandRoot.position = target;
            yield break;
        }

        var tweener = HandRoot.DOLocalMove(target, moveTime).SetEase(Ease.OutQuint);
        while (tweener.IsPlaying())
            yield return null;

        tweener.Kill();
    }
}
