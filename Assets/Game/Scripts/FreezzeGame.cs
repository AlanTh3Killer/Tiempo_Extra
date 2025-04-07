using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezzeGame : MonoBehaviour
{
    private static bool isDialogueActive = false;
    private static bool isLevelUIPanelActive = false;

    public static bool IsFrozen => isDialogueActive || isLevelUIPanelActive;

    public static void StartDialogue()
    {
        isDialogueActive = true;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public static void EndDialogue()
    {
        isDialogueActive = false;
        CheckUnfreeze();
    }

    public static void SetLevelUIPanelActive(bool state)
    {
        isLevelUIPanelActive = state;

        if (state)
        {
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            CheckUnfreeze();
        }
    }

    private static void CheckUnfreeze()
    {
        if (!isDialogueActive && !isLevelUIPanelActive)
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
