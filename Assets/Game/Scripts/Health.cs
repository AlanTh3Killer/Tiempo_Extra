using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour, IDamagable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    private bool isDead = false;
    private bool isInvulnerable = false; // Nueva variable para la defensa

    [SerializeField] UnityEvent onDeath;
    [SerializeField] private string sceneName;

    private WaveSpawner waveSpawner;

    [Header("Moneda")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinAmount = 3;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Damage(int damage)
    {
        if (isInvulnerable || isDead) return; // No recibir daño si está defendiendo o ya está muerto

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void SetInvulnerable(bool value)
    {
        isInvulnerable = value;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        onDeath.Invoke();
        SpawnCoins();
        Destroy(gameObject);
    }

    private void SpawnCoins()
    {
        if (coinPrefab == null) return;

        for (int i = 0; i < coinAmount; i++)
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 1.5f;
            spawnPosition.y = transform.position.y;
            Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public void ReloadTheScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ActivateDeathScreen(GameObject deathScreen)
    {
        deathScreen.SetActive(true);
    }
}
