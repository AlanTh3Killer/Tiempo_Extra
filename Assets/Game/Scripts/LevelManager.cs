using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject levelCompletePanel; // Panel de "Nivel Completado"
    public float levelCompleteDelay = 2f; // Tiempo antes de cambiar de nivel
    public string nextSceneName; // Nombre de la siguiente escena

    private int totalEnemies;

    private void Start()
    {
        // Contamos cu�ntos enemigos hay al inicio de la escena
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        // Ocultamos el panel al inicio
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }
    }

    public void EnemyDefeated()
    {
        totalEnemies--;

        if (totalEnemies <= 0)
        {
            StartCoroutine(LevelCompleted());
        }
    }

    IEnumerator LevelCompleted()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        yield return new WaitForSeconds(levelCompleteDelay);

        // Notifica al SoundManager sobre el cambio de escena
        if (SoundManager.instance != null)
        {
            SoundManager.instance.HandleSceneChange(nextSceneName);
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("No se ha asignado el nombre de la siguiente escena en LevelManager");
        }
    }
}
