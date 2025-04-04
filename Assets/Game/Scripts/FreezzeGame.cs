using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezzeGame : MonoBehaviour
{
    private static bool isDialogueActive = false;

    public static bool IsDialogueActive
    {
        get { return isDialogueActive; }
    }

    // Llamar esto al mostrar diálogos
    public static void StartDialogue()
    {
        isDialogueActive = true;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Llamar esto al terminar diálogos
    public static void EndDialogue()
    {
        isDialogueActive = false;
        Time.timeScale = 1f;
    }
}
