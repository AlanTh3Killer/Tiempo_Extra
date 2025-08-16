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

    [Header("Sonidos")]
    public AudioClip hurtSound;

    [Header("VFX")]
    [SerializeField] private ParticleSystem blockParticle; // polvo al bloquear
    [SerializeField] private ParticleSystem bloodParticle; // sangre al recibir daño

    public Transform blockPoint; // Asignar en inspector

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
        //Logica vieja solo esta linea
        //if (isInvulnerable || isDead) return;
        if (isDead) return;

        // ELEGIR PARTÍCULA SEGÚN ESTADO
        if (isInvulnerable)
        {
            if (isInvulnerable && blockParticle != null)
            {
                var blockInstance = Instantiate(blockParticle, transform.position, Quaternion.identity);
                Destroy(blockInstance.gameObject, blockInstance.main.duration);
            }
            Debug.Log($"{gameObject.name} BLOQUEÓ el golpe.");
            return; // Bloquea, no recibe daño
        }
        else
        {
            if (bloodParticle != null)
            {
                var bloodInstance = Instantiate(bloodParticle, transform.position, Quaternion.identity);
                Destroy(bloodInstance.gameObject, bloodInstance.main.duration);
            }
        }

        currentHealth -= damage;
        UpdateHealthBar(); // Actualiza la barra de vida al recibir da�o

        Debug.Log($"{gameObject.name} recibio {damage} de danio. Vida restante: {currentHealth}");
        if (!isDead && hurtSound != null)
        {
            SoundManager.instance.PlaySFX(hurtSound, 0.7f);
        }

        if (currentHealth <= 0)
        {
            Die();
        }

        // Efecto visual
        StartCoroutine(HitEffect());
    }

    private Dictionary<Material, Color> originalMaterialColors = new Dictionary<Material, Color>();

    private IEnumerator HitEffect()
    {
        Renderer[] allRenderers = GetComponentsInChildren<Renderer>();

        // Primera pasada: guardar colores y aplicar rojo
        foreach (Renderer renderer in allRenderers)
        {
            foreach (Material material in renderer.materials)
            {
                if (!originalMaterialColors.ContainsKey(material))
                {
                    originalMaterialColors[material] = material.color;
                }
                material.color = Color.red;
            }
        }

        yield return new WaitForSeconds(0.1f);

        // Segunda pasada: restaurar colores
        foreach (var kvp in originalMaterialColors)
        {
            kvp.Key.color = kvp.Value;
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return; // No curar si el personaje esta muerto

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthBar(); //  ACTUALIZA LA BARRA DE VIDA AL CURARSE
    }

    public void SetInvulnerable(bool value)
    {
        Debug.Log($"[{gameObject.name}] Invulnerable = {value}");
        isInvulnerable = value;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // Debug de confirmación
        Debug.Log($"Muerte confirmada en {gameObject.name}");

        if (gameObject.CompareTag("Player"))
        {
            ShowGameOverScreen();
        }
        else
        {
            EnemyAI enemyAI = GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.Die(); // Ahora Die() notifica al LevelManager
            }
            else
            {
                // Fallback directo
                NotifyLevelManager();
                Destroy(gameObject, 1f);
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

        // Notifica al SoundManager antes de recargar
        if (SoundManager.instance != null)
        {
            SoundManager.instance.HandleSceneChange(sceneName);
        }

        SceneManager.LoadScene(sceneName);
    }

    public void NotifyLevelManager() // Asegúrate que es público
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.EnemyDefeated();
            Debug.Log("Enemigo notificó al LevelManager correctamente");
        }
        else
        {
            Debug.LogError("LevelManager no encontrado en escena");
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
