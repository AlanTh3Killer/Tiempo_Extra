using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Animations : MonoBehaviour
{
    public Animator animator;
    public string[] animationNames; // Lista de animaciones de reacción

    public float minIdleTime = 5f;
    public float maxIdleTime = 15f;

    private string lastAnimation = ""; // Guardar la última animación usada

    private void Start()
    {
        StartCoroutine(IdleLoop());
    }

    IEnumerator IdleLoop()
    {
        while (true)
        {
            // Mantener en Idle por un tiempo aleatorio
            animator.Play("Idle");
            yield return new WaitForSeconds(Random.Range(minIdleTime, maxIdleTime));

            // Elegir una animación aleatoria que no sea la última
            string newAnimation;
            do
            {
                newAnimation = animationNames[Random.Range(0, animationNames.Length)];
            } while (newAnimation == lastAnimation && animationNames.Length > 1); // Evita repetir si hay más de una opción

            lastAnimation = newAnimation; // Guardamos la animación usada

            // Reproducir la animación
            animator.Play(newAnimation);

            // Esperar a que termine antes de volver a Idle
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }
    }
}
