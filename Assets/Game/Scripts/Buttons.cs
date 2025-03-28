using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    private GameObject targetObject;

    [SerializeField] private string sceneName;

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

    public void LoadAScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

}
