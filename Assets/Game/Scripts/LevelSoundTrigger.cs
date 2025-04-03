using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSoundTrigger : MonoBehaviour
{
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        TriggerLevelSounds();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TriggerLevelSounds();
    }

    private void TriggerLevelSounds()
    {
        if (SoundManager.instance == null) return;

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.ToLower().Contains("level"))
        {
            SoundManager.instance.PlayMusic(SoundManager.instance.battleMusic);
            SoundManager.instance.PlayCrowdAmbience();
        }
        else if (sceneName.ToLower().Contains("menu"))
        {
            SoundManager.instance.PlayMusic(SoundManager.instance.menuMusic);
            SoundManager.instance.StopAmbience();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
