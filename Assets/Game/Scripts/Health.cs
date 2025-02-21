using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour, IDamagable
{
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] UnityEvent onDeath;
    [SerializeField] private string sceneName;

    private WaveSpawner waveSpawner;
    private bool isDead = false; // Nueva variable de control

    [Header("Moneda")]
    [SerializeField] private GameObject coinPrefab; // Prefab de moneda
    [SerializeField] private int coinAmount = 3;   // Número de monedas a instanciar

    private void Start()
    {
        health = maxHealth;
    }

    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            onDeath.Invoke();
            SpawnCoins();
            MyDestroy(); //Esto es un cambio que puede que no funcione
        }
    }

    private void SpawnCoins()
    {
        if (coinPrefab == null) return;

        for (int i = 0; i < coinAmount; i++)
        {
            // Generar monedas alrededor del enemigo
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 1.5f;
            spawnPosition.y = transform.position.y; // Mantener la altura
            Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public void SetWaveSpawner(WaveSpawner spawner)
    {
        waveSpawner = spawner; // Asignar el WaveSpawner
    }

    [ContextMenu("InstaDeath")]
    public void InstaDeath()
    {
        Damage(health);
    }

    public void MyDestroy()
    {
        if (isDead) return; // Prevenir que este método se ejecute más de una vez
        isDead = true; // Marcar como "muerto"

        //if (this.CompareTag("Enemy") && waveSpawner != null)
        //{
        //    Debug.Log("Enemy destroyed. Updating wave info.");
        //    if (waveSpawner.currentWaveIndex < waveSpawner.waves.Length)
        //    {
        //        waveSpawner.waves[waveSpawner.currentWaveIndex].enemiesLeft--;
        //        Debug.Log($"Enemies left in wave {waveSpawner.currentWaveIndex}: {waveSpawner.waves[waveSpawner.currentWaveIndex].enemiesLeft}");
        //    }
        //    else
        //    {
        //        Debug.LogWarning("Current wave index out of range!");
        //    }
        //}
        //else
        //{
        //    Debug.Log("It was not an enemy or waveSpawner was null.");
        //}

        Destroy(this.gameObject); // Finalmente destruir el objeto

        //if (this.CompareTag("Enemy") && waveSpawner != null) // Verificar si es un enemigo y tiene referencia al WaveSpawner
        //{
        //    Debug.Log("Enemy destroyed. Updating wave info.");
        //    if (waveSpawner.currentWaveIndex < waveSpawner.waves.Length) // Verificar que la ola actual sea válida
        //    {
        //        waveSpawner.waves[waveSpawner.currentWaveIndex].enemiesLeft--; // Reducir el contador de enemigos
        //        Debug.Log($"Enemies left in wave {waveSpawner.currentWaveIndex}: {waveSpawner.waves[waveSpawner.currentWaveIndex].enemiesLeft}");
        //    }
        //    else
        //    {
        //        Debug.LogWarning("Current wave index out of range!");
        //    }
        //    Destroy(this.gameObject); // Destruir el objeto
        //}
        //else
        //{
        //    Debug.Log("It was not an enemy or waveSpawner was null.");
        //    Destroy(this.gameObject);
        //}

        //if (this.CompareTag("Enemy"))
        //{
        //    Debug.Log("It was enemy");
        //    waveSpawner.waves[waveSpawner.currentWaveIndex].enemiesLeft--;
        //    Debug.Log("Decreasing the number of the enemies left");
        //    Destroy(this.gameObject);
        //}
        //Debug.Log("It was not enemy");
        //Destroy(this.gameObject);

    }

    public void ReloadTheScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ActivateDeathScreen(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }
}
