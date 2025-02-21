using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;

    [SerializeField] private float countDown;

    [SerializeField] private GameObject spawnPoint;

    public Wave[] waves;

    public int currentWaveIndex = 0;

    private bool readyToCountDown;

    [SerializeField] private GameObject forceField; // Referencia al campo de fuerza

    private void Start()
    {
        readyToCountDown = true;

        for (int i = 0; i < waves.Length; i++)
        {
            waves[i].enemiesLeft = waves[i].enemies.Length;
            Debug.Log($"Wave {i} initialized with {waves[i].enemies.Length} enemies.");
        }
    }

    private void Update()
    {
        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("Ypu survive every wave");
            return;
        }

        if (readyToCountDown == true)
        {
            countDown -= Time.deltaTime;
        }

        if (countDown <= 0)
        {
            readyToCountDown = false;

            countDown = waves[currentWaveIndex].timeToNextWave;

            StartCoroutine(SpawnWave());
        }

        if (waves[currentWaveIndex].enemiesLeft == 0)
        {
            Debug.Log($"Wave {currentWaveIndex} completed.");
            readyToCountDown = true;
            currentWaveIndex++;
        }
    }

    private IEnumerator SpawnWave()
    {
        if (currentWaveIndex < waves.Length)
        {
            for (int i = 0; i < waves[currentWaveIndex].enemies.Length; i++)
            {
                // Instanciar el enemigo
                EnemyAI enemy = Instantiate(waves[currentWaveIndex].enemies[i], spawnPoint.transform);

                Debug.Log("Spawned enemy: " + enemy.name);

                // Asignar el objetivo al enemigo (si tiene inteligencia enemiga)
                //enemy.GetComponent<EnemyAI>().SetTarget(targetTransform);

                // Asignar el WaveSpawner al script Health del enemigo
                Health enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.SetWaveSpawner(this); // Pasar la referencia del WaveSpawner
                }

                // Asignar el campo de fuerza al enemigo
                //enemy.SetForceField(forceField); // Pasar la referencia del campo de fuerza

                // Asegurarse de que el enemigo esté correctamente posicionado dentro del spawnPoint
                enemy.transform.SetParent(spawnPoint.transform);

                // Esperar un tiempo antes de instanciar el siguiente enemigo
                yield return new WaitForSeconds(waves[currentWaveIndex].timeToNextEnemy);
            }
            //for (int i = 0; i < waves[currentWaveIndex].enemies.Length; i++)
            //{
            //    EnemyAI enemy = Instantiate(waves[currentWaveIndex].enemies[i], spawnPoint.transform);

            //    Debug.Log("Spawned enemy: " + enemy.name);

            //    enemy.GetComponent<EnemyAI>().SetTarget(targetTransform);

            //    enemy.transform.SetParent(spawnPoint.transform);

            //    yield return new WaitForSeconds(waves[currentWaveIndex].timeToNextEnemy);
            //}
        }
    }
}

[System.Serializable]
public class Wave
{
    public EnemyAI[] enemies;
    public float timeToNextEnemy;
    public float timeToNextWave;
    [HideInInspector] public int enemiesLeft;
}
