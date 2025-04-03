using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    [Header("Combate")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float defenseDuration = 1f;

    private bool isAttacking = false;
    private bool isDefending = false;
    private Health healthComponent;

    private void Start()
    {
        healthComponent = GetComponent<Health>();
    }

    public void Attack(Health targetHealth)
    {
        if (!isAttacking && targetHealth != null)
        {
            StartCoroutine(PerformAttack(targetHealth));
        }
    }

    private IEnumerator PerformAttack(Health targetHealth)
    {
        isAttacking = true;
        targetHealth.Damage(attackDamage);
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    public void StartDefense()
    {
        if (!isDefending)
        {
            StartCoroutine(PerformDefense());
        }
    }

    private IEnumerator PerformDefense()
    {
        isDefending = true;
        healthComponent.SetInvulnerable(true);
        yield return new WaitForSeconds(defenseDuration);
        healthComponent.SetInvulnerable(false);
        isDefending = false;
    }

    public bool IsDefending()
    {
        return isDefending;  //  Esto devuelve si el enemigo esta en defensa
    }

    public void StopCombatActions()
    {
        StopAllCoroutines(); //  Detiene cualquier ataque o defensa en curso
        isAttacking = false;
        isDefending = false;
    }
}
