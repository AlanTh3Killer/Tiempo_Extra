using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHealing : MonoBehaviour
{
    public GameObject healthPickupPrefab; // Prefab del objeto curativo
    public Transform[] spawnPoints; // Lista de puntos donde se pueden instanciar
    public float spawnInterval = 10f; // Tiempo entre cada aparición

    private void Start()
    {
        StartCoroutine(SpawnHealthPickups());
    }

    IEnumerator SpawnHealthPickups()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Elegir un punto aleatorio
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instanciar el objeto curativo
            Instantiate(healthPickupPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
