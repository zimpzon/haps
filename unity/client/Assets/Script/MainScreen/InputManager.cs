using UnityEngine;

public class InputManager
{
    private static int DisableCount = 0;

    public static void EnableDirectInput(bool enable)
    {
        if (enable)
        {
            DisableCount--;
            if (DisableCount < 0)
                Debug.LogErrorFormat("InputManager: DisableCount < 0 : {0}", DisableCount);
        }
        else
        {
            DisableCount++;
        }
    }

    public static Vector3 GetMousePosition()
    {
        return Input.mousePosition;
    }

    public static bool GetMouseButtonDown(int button)
    {
        return DisableCount > 0 ? false : Input.GetMouseButtonDown(button);
    }

    public static bool GetMouseButton(int button)
    {
        return DisableCount > 0 ? false : Input.GetMouseButton(button);
    }

    public static bool GetMouseButtonUp(int button)
    {
        return DisableCount > 0 ? false : Input.GetMouseButtonUp(button);
    }

    public static bool DeviceBackButtonActivated()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
}
