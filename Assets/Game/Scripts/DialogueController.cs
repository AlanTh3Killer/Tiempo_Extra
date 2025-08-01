using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public Action OnDialogueFinished;

    // Referencias asignadas desde el inspector
    [Header("Paneles de di�logo")]
    //[SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject levelStartPanel;
    [SerializeField] private GameObject levelEndPanel;

    [Header("Textos")]
    //[SerializeField] private TMP_Text tutorialText;
    [SerializeField] private TMP_Text startText;
    [SerializeField] private TMP_Text endText;

    // Variables internas de control
    private List<string> currentLines = new List<string>();
    private int currentIndex = 0;
    private GameObject currentPanel;
    private TMP_Text currentText;

    void Update()
    {
        if (currentPanel != null && currentPanel.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            currentIndex++;
            if (currentIndex < currentLines.Count)
            {
                currentText.text = currentLines[currentIndex];
            }
            else
            {
                EndDialogue();
            }
        }
    }

    public void ShowDialogue(DialogueType type, List<string> lines)
    {
        if (lines == null || lines.Count == 0)
        {
            Debug.LogWarning("No hay l�neas para mostrar");
            return;
        }

        // Se congela el juego al iniciar di�logo
        FreezzeGame.StartDialogue();

        // Desactiva todos los paneles
        //tutorialPanel.SetActive(false);
        levelStartPanel.SetActive(false);
        levelEndPanel.SetActive(false);

        currentLines = lines;
        currentIndex = 0;

        switch (type)
        {
            //case DialogueType.Tutorial:
            //    currentText = tutorialText;
            //    currentPanel = tutorialPanel;
            //    break;
            case DialogueType.LevelStart:
                currentText = startText;
                currentPanel = levelStartPanel;
                break;
            case DialogueType.LevelEnd:
                currentText = endText;
                currentPanel = levelEndPanel;
                break;
        }

        currentPanel.SetActive(true);
        currentText.text = currentLines[0];
    }

    private void EndDialogue()
    {
        if (currentPanel != null)
            currentPanel.SetActive(false);

        // Descongela el juego al terminar el di�logo
        FreezzeGame.EndDialogue();

        if (OnDialogueFinished != null)
            OnDialogueFinished.Invoke();
    }

    public enum DialogueType
    {
        //Tutorial,
        LevelStart,
        LevelEnd
    }
}
