using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : MonoBehaviour
{
    public int healAmount = 20; // Cantidad de vida que restaura
    public float lifetime = 10f; // Tiempo antes de que el objeto desaparezca

    [Header("Sonidos")]
    public AudioClip pickupSound;
    private void Start()
    {
        // Destruir el objeto despu�s de cierto tiempo si no se recoge
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que lo toca es el jugador
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
                // Sonido al recoger
                if (pickupSound != null)
                {
                    SoundManager.instance.PlaySFX(pickupSound, 0.8f);
                }
                Destroy(gameObject); // Destruir el objeto despu�s de recogerlo
            }
        }
    }
}
