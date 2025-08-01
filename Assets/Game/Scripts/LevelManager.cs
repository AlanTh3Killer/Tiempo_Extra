using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //[Header("Tutorial")]
    //public GameObject tutorialPanel;
    //public float tutorialTime = 5f;

    [Header("Configuración de Nivel")]
    public string nextSceneName; // Nombre de la siguiente escena

    [Header("Mensajes UI")]
    public GameObject levelStartPanel; // Mensaje de inicio (arrastrar desde el Canvas)
    public float startMessageTime = 3f; // Tiempo que muestra el mensaje inicial
    public GameObject levelCompletePanel; // Panel de "Nivel Completado"
    public float levelCompleteDelay = 2f; // Tiempo antes de cambiar de nivel

    [Header("Pausa")]
    public GameObject pausePanel;
    public GameObject resumePanel;
    public float resumeDelay = 1.5f;

    private bool isPaused = false;
    private int totalEnemies;

    [Header("Dialogo")]
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private DialogueLines dialogueLines; // Asignar desde inspector

    private void Start()
    {
        //if (SceneManager.GetActiveScene().name == "LEVEL 1" && tutorialPanel != null)
        //{
        //    StartCoroutine(ShowTutorial());
        //}

        //if (SceneManager.GetActiveScene().name == "LEVEL 1" && tutorialPanel != null)
        //{
        //    StartCoroutine(ShowTutorialThenDialogue());
        //}
        //else
        //{
        //    // Si no es el primer nivel, puedes iniciar el diálogo directamente
        //    if (dialogueLines != null && dialogueLines.startDialogue.Count > 0)
        //    {
        //        dialogueController.ShowDialogue(DialogueController.DialogueType.LevelStart, dialogueLines.startDialogue);
        //    }
        //}

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (resumePanel != null)
            resumePanel.SetActive(false);

        // Contar enemigos al inicio
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        Debug.Log($"Enemigos en nivel: {totalEnemies}");


        // Mostrar diálogo de inicio (en vez del panel directo)
        if (dialogueLines != null && dialogueLines.startDialogue.Count > 0)
        {
            dialogueController.ShowDialogue(DialogueController.DialogueType.LevelStart, dialogueLines.startDialogue);
        }

        // Mostrar mensaje inicial
        //if (levelStartPanel != null)
        //{
        //    StartCoroutine(ShowStartMessage());
        //}
        //else
        //{
        //    Debug.LogWarning("No hay panel de inicio asignado");
        //}

        // Ocultar panel de completado (si existe)
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                StartCoroutine(ResumeSequence());
            }
        }
    }

    private void PauseGame()
    {
        Debug.Log("Juego Pausado");
        isPaused = true;
        pausePanel.SetActive(true);

        FreezzeGame.SetLevelUIPanelActive(true);
    }

    private IEnumerator ResumeSequence()
    {
        Debug.Log("Reanudando...");
        pausePanel.SetActive(false);

        yield return new WaitForSecondsRealtime(resumeDelay);

        if (resumePanel != null)
        {
            resumePanel.SetActive(true);
            yield return new WaitForSecondsRealtime(1f);
            resumePanel.SetActive(false);
        }

        isPaused = false;
        FreezzeGame.SetLevelUIPanelActive(false);
    }

    //private IEnumerator ShowTutorialThenDialogue()
    //{
    //    // Muestra el panel tutorial (sin congelar el tiempo)
    //    tutorialPanel.SetActive(true);
    //    Debug.Log("Mostrando tutorial");

    //    yield return new WaitForSeconds(tutorialTime); // Esto funciona porque aún no se congeló el tiempo

    //    tutorialPanel.SetActive(false);

    //    // Ahora sí, inicia el diálogo que congela el juego
    //    if (dialogueLines != null && dialogueLines.startDialogue.Count > 0)
    //    {
    //        dialogueController.ShowDialogue(DialogueController.DialogueType.LevelStart, dialogueLines.startDialogue);
    //    }
    //}

    //private IEnumerator ShowTutorial()
    //{
    //    Debug.Log("Mostrando tutorial");
    //    tutorialPanel.SetActive(true);
    //    yield return new WaitForSeconds(tutorialTime);
    //    tutorialPanel.SetActive(false);
    //}

    // Corrutina para mensaje inicial
    private IEnumerator ShowStartMessage()
    {
        levelStartPanel.SetActive(true);
        FreezzeGame.SetLevelUIPanelActive(true);

        yield return new WaitForSecondsRealtime(startMessageTime); // Usar tiempo real porque Time.timeScale = 0
        levelStartPanel.SetActive(false);
        FreezzeGame.SetLevelUIPanelActive(false);
    }

    public void EnemyDefeated()
    {
        totalEnemies--;
        Debug.Log($"Enemigos restantes: {totalEnemies}");
        FindObjectOfType<PlayerBuffManager>().ApplyRandomBuff();
        if (totalEnemies <= 0)
        {
            if (dialogueLines != null && dialogueLines.endDialogue.Count > 0)
            {
                // Asigna el callback solo para este diálogo
                dialogueController.OnDialogueFinished = () =>
                {
                    StartCoroutine(LevelCompleted());
                };

                dialogueController.ShowDialogue(DialogueController.DialogueType.LevelEnd, dialogueLines.endDialogue);
            }
            else
            {
                StartCoroutine(LevelCompleted());
            }
        }
        //if (totalEnemies <= 0)
        //{
        //    if (dialogueLines != null && dialogueLines.endDialogue.Count > 0)
        //    {
        //        dialogueController.ShowDialogue(DialogueController.DialogueType.LevelEnd, dialogueLines.endDialogue);
        //    }
        //    else
        //    {
        //        StartCoroutine(LevelCompleted());
        //    }
        //}
    }

    private IEnumerator LevelCompleted()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
            //FreezzeGame.SetLevelUIPanelActive(true);
        }

        yield return new WaitForSecondsRealtime(levelCompleteDelay); // También usar tiempo real
        //FreezzeGame.SetLevelUIPanelActive(false);

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
