using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UIDisableInput
{
    // Keep a list of pointer input modules that we have disabled so that we can re-enable them
    static List<PointerInputModule> disabled = new List<PointerInputModule>();

    static int disableCount = 0;    // How many times has disable been called?

    public static void Disable()
    {
        if (disableCount++ == 0)
        {
            UpdateState(false);
        }
    }

    public static void Enable(bool enable)
    {
        if (!enable)
        {
            Disable();
            return;
        }
        if (--disableCount == 0)
        {
            UpdateState(true);
            if (disableCount > 0)
            {
                Debug.LogWarning("Warning UIDisableInput.Enable called more than Disable");
            }
        }
    }

    static void UpdateState(bool enabled)
    {
        // First re-enable all systems
        for (int i = 0; i < disabled.Count; i++)
        {
            if (disabled[i])
            {
                disabled[i].enabled = true;
            }
        }
        disabled.Clear();

        EventSystem es = EventSystem.current;
        if (es == null) return;

        es.sendNavigationEvents = enabled;
        if (!enabled)
        {
            // Find all PointerInputModules and disable them
            PointerInputModule[] pointerInput = es.GetComponents<PointerInputModule>();
            if (pointerInput != null)
            {
                for (int i = 0; i < pointerInput.Length; i++)
                {
                    PointerInputModule pim = pointerInput[i];
                    if (pim.enabled)
                    {
                        pim.enabled = false;
                        // Keep a list of disabled ones
                        disabled.Add(pim);
                    }
                }
            }

            // Cause EventSystem to update it's list of modules
            es.enabled = false;
            es.enabled = true;
        }
    }
}
