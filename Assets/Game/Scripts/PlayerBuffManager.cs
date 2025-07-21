using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffManager : MonoBehaviour
{
    [Header("Referencias")]
    private PlayerController playerController;

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
                Debug.Log("Buff aplicado: Velocidad aumentada");
                break;

            case 1:
                ApplyAttackCooldownBuff(0.5f, 5f);  // 50% menos cooldown de ataque durante 5 segundos
                Debug.Log("Buff aplicado: Reducción cooldown ataque");
                break;

            case 2:
                ApplyDashCooldownBuff(0.5f, 5f);  // 50% menos cooldown de dash durante 5 segundos
                Debug.Log("Buff aplicado: Reducción cooldown dash");
                break;
        }
    }

}
