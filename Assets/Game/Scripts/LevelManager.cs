using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Tutorial")]
    public GameObject tutorialPanel;
    public float tutorialTime = 5f;

    [Header("Configuración de Nivel")]
    public string nextSceneName; // Nombre de la siguiente escena

    [Header("Mensajes UI")]
    public GameObject levelStartPanel; // Mensaje de inicio (arrastrar desde el Canvas)
    public float startMessageTime = 3f; // Tiempo que muestra el mensaje inicial
    public GameObject levelCompletePanel; // Panel de "Nivel Completado"
    public float levelCompleteDelay = 2f; // Tiempo antes de cambiar de nivel

    private int totalEnemies;

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 3 && tutorialPanel != null) // Asumiendo que el primer nivel es buildIndex 1
        {
            StartCoroutine(ShowTutorial());
        }

        // Contar enemigos al inicio
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        Debug.Log($"Enemigos en nivel: {totalEnemies}");

        // Mostrar mensaje inicial
        if (levelStartPanel != null)
        {
            StartCoroutine(ShowStartMessage());
        }
        else
        {
            Debug.LogWarning("No hay panel de inicio asignado");
        }

        // Ocultar panel de completado (si existe)
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }
    }

    private IEnumerator ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        yield return new WaitForSeconds(tutorialTime);
        tutorialPanel.SetActive(false);
    }

    // Corrutina para mensaje inicial
    private IEnumerator ShowStartMessage()
    {
        levelStartPanel.SetActive(true);
        yield return new WaitForSeconds(startMessageTime);
        levelStartPanel.SetActive(false);
    }

    public void EnemyDefeated()
    {
        totalEnemies--;
        Debug.Log($"Enemigos restantes: {totalEnemies}");

        if (totalEnemies <= 0)
        {
            StartCoroutine(LevelCompleted());
        }
    }

    // Corrutina para completar nivel (sin cambios)
    private IEnumerator LevelCompleted()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        yield return new WaitForSeconds(levelCompleteDelay);

        // Notificar al SoundManager
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
            Debug.LogError("Nombre de siguiente escena no asignado");
        }
    }
}
