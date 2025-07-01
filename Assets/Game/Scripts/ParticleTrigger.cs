using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTrigger : MonoBehaviour
{
    [Tooltip("Partículas que puedes asignar desde el inspector.")]
    public ParticleSystem[] particles;

    private void Awake()
    {
        // Detener todas las partículas al iniciar el juego
        if (particles != null)
        {
            foreach (var ps in particles)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }

    /// <summary>
    /// Reproduce una partícula según su índice en la lista 'particles'.
    /// Llamar desde evento de animación como: PlayParticleByIndex(0)
    /// </summary>
    public void PlayParticleByIndex(int index)
    {
        if (index >= 0 && index < particles.Length && particles[index] != null)
        {
            particles[index].Play();
        }
        else
        {
            Debug.LogWarning($"[ModularParticleTrigger] No se pudo reproducir partícula en índice {index}");
        }
    }

    /// <summary>
    /// Reproduce directamente una partícula específica desde otro objeto.
    /// Se puede usar si el evento de animación acepta pasar referencias (menos común).
    /// </summary>
    public void PlayParticleDirect(ParticleSystem ps)
    {
        if (ps != null)
        {
            ps.Play();
        }
    }
}
