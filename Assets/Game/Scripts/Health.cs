using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour, IDamagable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    private bool isDead = false;
    private bool isInvulnerable = false;

    [Header("Eventos")]
    [SerializeField] private UnityEngine.Events.UnityEvent onDeath;

    [Header("Escena")]
    [SerializeField] private string sceneName;

    [Header("UI Game Over")]
    public GameObject gameOverPanel;
    public float gameOverDelay = 2f;

    [Header("Barra de Vida")]
    public RectTransform healthBar; // Referencia al RectTransform de la barra de vida
    private float healthBarFullWidth; // Ancho original de la barra de vida

    private void Start()
    {
        currentHealth = maxHealth;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (healthBar != null)
        {
            healthBarFullWidth = healthBar.sizeDelta.x; // Guardamos el tamaño original
        }

        UpdateHealthBar(); // Asegura que la barra empiece con el tamaño correcto
    }

    public void Damage(int damage)
    {
        if (isInvulnerable || isDead) return;

        currentHealth -= damage;
        UpdateHealthBar(); // Actualiza la barra de vida al recibir daño

        Debug.Log($"{gameObject.name} recibió {damage} de daño. Vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void SetInvulnerable(bool value)
    {
        isInvulnerable = value;
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        onDeath?.Invoke();

        if (gameObject.CompareTag("Player"))
        {
            ShowGameOverScreen();
        }
        else
        {
            NotifyLevelManager();
            Destroy(gameObject, 1f);
        }
    }

    void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            StartCoroutine(ReloadSceneAfterDelay());
        }
    }

    IEnumerator ReloadSceneAfterDelay()
    {
        yield return new WaitForSeconds(gameOverDelay);
        SceneManager.LoadScene(sceneName);
    }

    void NotifyLevelManager()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.EnemyDefeated();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            float healthPercent = (float)currentHealth / maxHealth;
            healthBar.sizeDelta = new Vector2(healthBarFullWidth * healthPercent, healthBar.sizeDelta.y);
        }
    }
}
