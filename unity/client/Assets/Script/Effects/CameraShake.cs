using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public float shakeAmount;//The amount to shake this frame.
    public float shakeDuration;//The duration this frame.

    float startAmount;//The initial shake amount (to determine percentage), set when ShakeCamera is called.
    float startDuration;//The initial shake duration, set when ShakeCamera is called.

    bool isRunning = false; //Is the coroutine running right now?

    Vector2 offset;
    Vector2 velocity;

    public void ShakeCamera(float amount, float duration)
    {
        shakeAmount = amount;//Add to the current amount.
        startAmount = shakeAmount;//Reset the start amount, to determine percentage.
        shakeDuration = duration;//Add to the current time.
        startDuration = shakeDuration;//Reset the start time.

        Vector2 dir = Random.insideUnitCircle.normalized;
        offset = dir * shakeAmount;
        velocity = Vector2.zero;

        if (!isRunning) StartCoroutine(Shake());//Only call the coroutine if it isn't currently running. Otherwise, just set the variables.
    }

    IEnumerator Shake()
    {
        isRunning = true;

        while (shakeDuration > 0.01f)
        {
            transform.localPosition = offset;
            offset = Vector2.SmoothDamp(offset, Vector2.zero, ref velocity, shakeDuration, 10.0f, Time.deltaTime);
            shakeDuration -= Time.deltaTime;
            yield return null;
        }

        transform.localPosition = Vector2.zero;
        isRunning = false;
    }

}