using UnityEngine;
using System.Collections;
using System;

public class BusyScript : MonoBehaviour
{
    public static BusyScript Instance;
    public Transform Indicator;

    [NonSerialized]
    public bool IsShown;

    void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show()
    {
        if (IsShown)
            return;

        this.transform.position = Vector3.zero;
        this.gameObject.SetActive(true);
        IsShown = true;
        StartCoroutine(RotateCo());
    }

    public void Hide()
    {
        this.transform.position = Vector3.right * 3000;
        this.gameObject.SetActive(false);
        IsShown = false;
    }

    IEnumerator RotateCo()
    {
        while (IsShown)
        {
            Indicator.rotation = Quaternion.Euler(0.0f, 0.0f, -Time.time * 200);

            float springSpeed = 8.0f;
            float springY = Mathf.Sin(Time.time * springSpeed) * 0.15f;
            float springX = -springY;
            Indicator.localScale = new Vector3(1 - springX, 1 - springY, 1.0f);

            yield return null;
        }
    }
}
