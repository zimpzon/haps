using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Script
{
    public class Util
    {
        //public static T FromString<T>(string value)
        //{
        //    return (T)Convert.ChangeType(value, typeof(T));
        //}

        //[Serializable]
        //private class Wrapper<T>
        //{
        //    public T inner = default(T);
        //}

        //public static T FromJson<T>(string json)
        //{
        //    string newJson = "{ \"inner\": " + json + "}";
        //    Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        //    return wrapper.inner;
        //}

        public class CoroutineWithData<T>
        {
            private IEnumerator target_;
            public T Result;
            public Coroutine Coroutine { get; private set; }

            public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
            {
                target_ = target;
                Coroutine = owner.StartCoroutine(Run());
            }

            private IEnumerator Run()
            {
                while (target_.MoveNext())
                {
                    Result = (T)target_.Current;
                    yield return Result;
                }
            }
        }

        public static void UpdateIfChanged(TextMeshProUGUI label, int newValue, ref int oldValue)
        {
            if (newValue != oldValue)
            {
                label.SetText(newValue.ToString());
                oldValue = newValue;
            }
        }

        public static void UpdateIfChanged(TextMeshProUGUI label, string format, float newValue, ref float oldValue)
        {
            if (newValue != oldValue)
            {
                label.SetText(string.Format(format, newValue));
                oldValue = newValue;
            }
        }
    }
}
