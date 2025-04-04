using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public void ToggleFullscreen(bool isFullscreen)
    {
        Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }
}
