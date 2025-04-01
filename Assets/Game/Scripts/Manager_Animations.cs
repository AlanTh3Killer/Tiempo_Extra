using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Animations : MonoBehaviour
{
    public Animator animator;
    public string[] animationNames; // Lista de animaciones de reacci�n

    public float minIdleTime = 5f;
    public float maxIdleTime = 15f;

    private string lastAnimation = ""; // Guardar la �ltima animaci�n usada

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

            // Elegir una animaci�n aleatoria que no sea la �ltima
            string newAnimation;
            do
            {
                newAnimation = animationNames[Random.Range(0, animationNames.Length)];
            } while (newAnimation == lastAnimation && animationNames.Length > 1); // Evita repetir si hay m�s de una opci�n

            lastAnimation = newAnimation; // Guardamos la animaci�n usada

            // Reproducir la animaci�n
            animator.Play(newAnimation);

            // Esperar a que termine antes de volver a Idle
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }
    }
}
