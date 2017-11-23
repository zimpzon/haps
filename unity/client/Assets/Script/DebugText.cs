using System.Collections.Generic;
using UnityEngine;

public class DebugText : MonoBehaviour
{
    static Dictionary<string, string> Lines = new Dictionary<string, string>();

    public static void SetLine(object key, string text, params object[] args)
    {
        Lines[key.ToString()] = string.Format("{0}: {1}", key, string.Format(text, args));
    }

    public static void SetLine(object key, object obj)
    {
        Lines[key.ToString()] = string.Format("{0}: {1}", key, obj);
    }

    public static void RemoveLine(object key)
    {
        Lines.Remove(key.ToString());
    }

    public bool Show = true;
    GUIStyle style = new GUIStyle();

    private void OnGUI()
    {
        if (!Show)
            return;

        style.fontSize = 6;

        float y = 10.0f;
        foreach(var pair in Lines)
        {
            GUI.color = Color.black;
            GUI.Label(new Rect(11, y + 1, 600, 30), pair.Value, style);
            GUI.color = Color.yellow;
            GUI.Label(new Rect(10, y, 600, 30), pair.Value, style);
            y += 25;
        }
    }
}
