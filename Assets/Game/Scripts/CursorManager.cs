using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public void UpdateCursorForScene(string sceneName)
    {
        bool showCursor = sceneName.Contains("MainMenu") ||
                         sceneName.Contains("Pause") ||
                         sceneName.Contains("Controles") ||
                         sceneName.Contains("Creditos");

        Cursor.visible = showCursor;
        Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
