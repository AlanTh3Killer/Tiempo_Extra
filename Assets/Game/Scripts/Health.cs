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
    private Animator animator;


    private void Start()
    {
        animator = GetComponentInChildren<Animator>();  // Busca en el hijo donde están las animaciones
        currentHealth = maxHealth;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (healthBar != null)
        {
            healthBarFullWidth = healthBar.sizeDelta.x; // Guardamos el tama�o original
        }

        UpdateHealthBar(); // Asegura que la barra empiece con el tama�o correcto
    }

    public void Damage(int damage)
    {
        if (isInvulnerable || isDead) return;

        currentHealth -= damage;
        UpdateHealthBar(); // Actualiza la barra de vida al recibir da�o

        Debug.Log($"{gameObject.name} recibi� {damage} de da�o. Vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return; // No curar si el personaje est� muerto

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthBar(); //  ACTUALIZA LA BARRA DE VIDA AL CURARSE
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

            // 🔹 Llamar a Die() en EnemyAI para manejar la animación
            EnemyAI enemyAI = GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.Die();
            }
            else
            {
                Destroy(gameObject, 1f); // En caso de que no tenga EnemyAI, se destruye rápido
            }
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

    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }
}
