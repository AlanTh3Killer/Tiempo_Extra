using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffManager : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] private GameObject[] buffParticles; // 0 = velocidad, 1 = cooldown ataque, 2 = cooldown dash
    [SerializeField] private Transform vfxSpawnPoint;

    [Header("Referencias")]
    [SerializeField] private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void ApplySpeedBuff(float buffMultiplier, float duration)
    {
        StartCoroutine(SpeedBuffRoutine(buffMultiplier, duration));
    }

    public void ApplyAttackCooldownBuff(float reductionFactor, float duration)
    {
        StartCoroutine(AttackCooldownBuffRoutine(reductionFactor, duration));
    }

    public void ApplyDashCooldownBuff(float reductionFactor, float duration)
    {
        StartCoroutine(DashCooldownRoutine(reductionFactor, duration));
    }

    private IEnumerator SpeedBuffRoutine(float multiplier, float duration)
    {
        float originalSpeed = playerController.speed;
        playerController.speed *= multiplier;

        yield return new WaitForSeconds(duration);

        playerController.speed = originalSpeed;
    }

    private IEnumerator AttackCooldownBuffRoutine(float factor, float duration)
    {
        float originalCooldown = playerController.attackCooldown;
        playerController.attackCooldown *= factor;

        yield return new WaitForSeconds(duration);

        playerController.attackCooldown = originalCooldown;
    }

    private IEnumerator DashCooldownRoutine(float factor, float duration)
    {
        float originalCooldown = playerController.dashCooldown;
        playerController.dashCooldown *= factor;

        yield return new WaitForSeconds(duration);

        playerController.dashCooldown = originalCooldown;
    }

    public void ApplyRandomBuff()
    {
        int randomBuff = Random.Range(0, 3);

        switch (randomBuff)
        {
            case 0:
                ApplySpeedBuff(1.3f, 5f);  // +30% velocidad durante 5 segundos
                SpawnBuffVFX(0);
                Debug.Log("Buff aplicado: Velocidad aumentada");
                break;

            case 1:
                ApplyAttackCooldownBuff(0.5f, 5f);  // 50% menos cooldown de ataque durante 5 segundos
                SpawnBuffVFX(1);
                Debug.Log("Buff aplicado: Reducción cooldown ataque");
                break;

            case 2:
                ApplyDashCooldownBuff(0.5f, 5f);  // 50% menos cooldown de dash durante 5 segundos
                SpawnBuffVFX(2);
                Debug.Log("Buff aplicado: Reducción cooldown dash");
                break;
        }
    }

    private void SpawnBuffVFX(int index)
    {
        if (buffParticles != null && index >= 0 && index < buffParticles.Length && buffParticles[index] != null)
        {
            // Instanciamos como hijo del punto de aparición (para que siga al jugador)
            GameObject vfxInstance = Instantiate(
                buffParticles[index],
                vfxSpawnPoint.position,
                Quaternion.identity,
                vfxSpawnPoint // Lo hace hijo para que se mueva con el jugador
            );

            // Buscamos el sistema de partículas dentro del prefab instanciado
            ParticleSystem ps = vfxInstance.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                float duration = ps.main.duration + ps.main.startLifetime.constantMax;
                Destroy(vfxInstance, duration); // Se destruye cuando termina la animación
            }
            else
            {
                Destroy(vfxInstance, 2f); // Fallback: destruir tras 2 segundos si no hay ParticleSystem
            }
        }
    }

}
