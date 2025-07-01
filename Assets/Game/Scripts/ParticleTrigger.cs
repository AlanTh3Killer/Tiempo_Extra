using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTrigger : MonoBehaviour
{
    [Tooltip("Part�culas que puedes asignar desde el inspector.")]
    public ParticleSystem[] particles;

    private void Awake()
    {
        // Detener todas las part�culas al iniciar el juego
        if (particles != null)
        {
            foreach (var ps in particles)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }

    /// <summary>
    /// Reproduce una part�cula seg�n su �ndice en la lista 'particles'.
    /// Llamar desde evento de animaci�n como: PlayParticleByIndex(0)
    /// </summary>
    public void PlayParticleByIndex(int index)
    {
        if (index >= 0 && index < particles.Length && particles[index] != null)
        {
            particles[index].Play();
        }
        else
        {
            Debug.LogWarning($"[ModularParticleTrigger] No se pudo reproducir part�cula en �ndice {index}");
        }
    }

    /// <summary>
    /// Reproduce directamente una part�cula espec�fica desde otro objeto.
    /// Se puede usar si el evento de animaci�n acepta pasar referencias (menos com�n).
    /// </summary>
    public void PlayParticleDirect(ParticleSystem ps)
    {
        if (ps != null)
        {
            ps.Play();
        }
    }
}
