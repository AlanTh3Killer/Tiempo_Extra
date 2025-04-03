using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    private GameObject targetObject;

    [SerializeField] private string sceneName;
    [SerializeField] private AudioClip buttonClickSound; // Sonido de botón
    [SerializeField] private float sceneChangeDelay = 0.2f; // Tiempo antes de cambiar de escena

    private void Awake()
    {
        targetObject = GetComponent<GameObject>();
    }

    public void ShowWindow(GameObject targetObject)
    {
        targetObject.SetActive(true);
    }

    public void HideWindow(GameObject targetObject)
    {
        targetObject.SetActive(false);
    }

    //  Método mejorado: Reproduce sonido antes de cambiar de escena
    public void LoadAScene(string sceneName)
    {
        StartCoroutine(PlaySoundAndLoadScene(sceneName));
    }

    private IEnumerator PlaySoundAndLoadScene(string sceneName)
    {
        if (SoundManager.instance != null && buttonClickSound != null)
        {
            SoundManager.instance.PlayUISound(buttonClickSound);
        }

        yield return new WaitForSeconds(sceneChangeDelay);

        // Notifica al SoundManager sobre el cambio de escena
        if (SoundManager.instance != null)
        {
            SoundManager.instance.HandleSceneChange(sceneName);
        }

        SceneManager.LoadScene(sceneName);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

}
